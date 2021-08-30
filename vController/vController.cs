using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;

namespace vControler
{
    public partial class vMixControler : Form
    {
        vPreferences settings;
        List<vMixEvent> EventList;
        List<vMixEvent>[] trackinglist = new List<vMixEvent>[5];
        List<vMixEvent>[] EventLists = new List<vMixEvent>[5];
        Semaphore EventListLock;

        //vMixScheduler MasterClock;
        vMixScheduler[] MasterClocks = new vMixScheduler[5];

        //BlockingCollection<vMixMicroEvent> Workload;
        BlockingCollection<vMixMicroEvent>[] Workloads = new BlockingCollection<vMixMicroEvent>[5] ;
        
        //Thread WorkloadThread;
        Thread[] WorkloadThreads = new Thread[5];
        //Thread to check if vMix is running and start or stop sending command or updating microevents list
        Thread OnlineThread;

        ThreadStart workstart;

        vMixWebClient WebClient;
        vMixWebClient[] WebClients = new vMixWebClient[5];
        
        bool exitApp = false;
        bool stopthread = true;
        int sleep = 0;
        int WorkLayer = 0;

        //FileSystemWatcher WatchDog to watch update sent by vManager
        FileSystemWatcher[] WatchDogs = new FileSystemWatcher[5];
        string ScheduleFolder;
        string[] ScheduleFile = { "vSchedule0.xml", "vSchedule1.xml", "vSchedule2.xml", "vSchedule3.xml", "vSchedule4.xml" };

        public vMixControler()
        {
            InitializeComponent();
            ScheduleFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";
            if (!Directory.Exists(ScheduleFolder)) Directory.CreateDirectory(ScheduleFolder);
            settings = new vPreferences();
            settings.PreferencesPath = ScheduleFolder + "\\Settings.xml";
            if (settings.vMixStartMini)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else this.WindowState = FormWindowState.Normal;
        }

        private void vMaster_Load(object sender, EventArgs e)
        {
            EventListLock = new Semaphore(1, 1);
            EventList = new List<vMixEvent>();
            for (int i = 0; i < 5; i++) { trackinglist[i] = new List<vMixEvent>(); }
            for (int i = 0; i < 5; i++) { EventLists[i] = new List<vMixEvent>(); }
            for (int i = 0; i < 5; i++) { Workloads[i] = new BlockingCollection<vMixMicroEvent>(); }

            for (int i = 0; i < 5; i++)
            {
                workstart = new ThreadStart(WorkloadFunc);
                WorkloadThreads[i] = new Thread(workstart);
                WorkloadThreads[i].Name = Convert.ToString(i);
            //    WorkloadThreads[i].Start();
            }
            
            for (int i = 0; i < 5; i++)
            {
                WatchDogs[i] = new FileSystemWatcher(ScheduleFolder, ScheduleFile[i]);
                WatchDogs[i].NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                WatchDogs[i].Changed += new FileSystemEventHandler(WatchDogBark);
                //WatchDogs[i].Created += new FileSystemEventHandler(WatchDogBark);
                WatchDogs[i].Deleted += new FileSystemEventHandler(WatchDogBark);
                WatchDogs[i].EnableRaisingEvents = true;
            }
            
            //WatchDog = new FileSystemWatcher(ScheduleFolder, "vMixSchedule.xml");
            //WatchDog.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
            //WatchDog.Changed += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.Created += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.Deleted += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.EnableRaisingEvents = true;

            //if (!File.Exists(ScheduleFolder + "Settings.xml")) File.Create(ScheduleFolder + "Settings.xml");

            //settings = new vPreferences();

            for (int i = 0; i < 5; i++) { MasterClocks[i] = new vMixScheduler(settings.vMixLate, settings.vMixPreload, settings.vMixLinger, Workloads[i]); }
            //MasterClock = new vMixScheduler(100, settings.vMixPreload , settings.vMixLinger, Workload);
            WebClient = new vMixWebClient(settings.vMixURL);
            for (int i = 0; i < 5; i++) { WebClients[i] = new vMixWebClient(settings.vMixURL); }

            //if (settings.vMixAutoLoad)
            //{
            //    this.Enabled = false;
            //    for (int i = 0; i < 5; i++) { WorkLayer = i; ReloadSchedule(); }
            //    this.Enabled = true;
            //}
            if (settings.vMixAutoLoadV)
            {
                System.Diagnostics.Process launched =  new System.Diagnostics.Process();
                launched.StartInfo = new System.Diagnostics.ProcessStartInfo(settings.vMixPath);
                string processname = Path.GetFileNameWithoutExtension(settings.vMixPath);
                try 
                {
                    if (System.Diagnostics.Process.GetProcessesByName(processname).Length == 0)
                        launched.Start();    
                }
                catch { }
            }
            sleep = settings.vMixRefresh;
            updatedsettings();            

            ThreadStart onlinestarter = new ThreadStart(onlinestarterfunc);
            OnlineThread = new Thread(onlinestarter);
            OnlineThread.Start();
        }

        private void onlinestarterfunc()
        {
            bool start, online;
            online = WebClient.UpdateData();
            start = settings.vMixAutoLoad;
            while (!exitApp)
            {
                WebClient.UpdateData();
                PaintTime();
                Thread.Sleep(500 + sleep);
                if (!settings.vMixAutoLoad)
                {
                    if (!stopthread)
                    {
                        //Monitor.Enter(Workloads);
                        for (int i = 0; i < 5; i++)
                        {
                            WorkloadThreads[i].Abort();
                            MasterClocks[i].Abort();
                            if (EventLists[i].Count > 0)
                                if (EventLists[i][0].IsCurrent)
                                {
                                    WebClients[1].CloseInput(EventLists[i][0].GUID);
                                    //MasterClocks[i].RevokeMicroEvents(EventLists[i][0], DateTime.Now);
                                }
                        }
                        vMixMicroEvent t;
                        int r;
                        do
                            r = BlockingCollection<vMixMicroEvent>.TryTakeFromAny(Workloads, out t);
                        while (r != -1);
                        //for (int i = 0; i < 5; i++) Workloads[i].Add(new vMixMicroEvent(vmMicroEventType.exit));
                        stopthread = true;
                        //bn_Statuts.BackColor = System.Drawing.Color.Red;
                        //bn_Statuts.Text = "OFFLINE";
                    }
                    else //if (online)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (EventLists[i].Count > 0)
                                if (EventLists[i][0].EventEnd < DateTime.Now - new TimeSpan(0, 0, settings.vMixPreload + 1))
                                {
                                    RemoveEvent(EventLists[i][0], i);
                                    MasterClocks[i].RemoveMicroEvents(EventLists[i][0]);
                                }
                                else EventLists[i][0].state = 0;
                        }
                    }
                    if (start) { updatedsettings(); start = false; }
                }
                else if (!start)
                {
                    updatedsettings();
                    start = true;
                }

                if (!WebClient.UpdateData())
                {
                    if (!stopthread)
                    {
                        //Monitor.Enter(Workloads);
                        for (int i = 0; i < 5; i++)
                        {
                        WorkloadThreads[i].Abort();
                        MasterClocks[i].Abort();
                        if (EventLists[i].Count > 0)
                            if (EventLists[i][0].IsCurrent)
                            {
                                WebClients[1].CloseInput(EventLists[i][0].GUID);
                                //MasterClocks[i].RevokeMicroEvents(EventLists[i][0], DateTime.Now);
                            }
                        }
                        vMixMicroEvent t;
                        int r;
                        do 
                            r = BlockingCollection<vMixMicroEvent>.TryTakeFromAny(Workloads, out t);
                        while (r != -1);
                        //for (int i = 0; i < 5; i++) Workloads[i].Add(new vMixMicroEvent(vmMicroEventType.exit));
                        stopthread = true;
                        //bn_Statuts.BackColor = System.Drawing.Color.Red;
                        //bn_Statuts.Text = "OFFLINE";
                    }
                    else if (start)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (EventLists[i].Count > 0)
                                if (EventLists[i][0].EventEnd < DateTime.Now - new TimeSpan(0, 0, settings.vMixPreload + 1))
                                {
                                    RemoveEvent(EventLists[i][0], i);
                                    MasterClocks[i].RemoveMicroEvents(EventLists[i][0]);
                                }
                                else EventLists[i][0].state = 0;
                        }
                    }
                    if (online) { updatedsettings(); online = false; }
                }
                else
                {
                    if (settings.vMixAutoLoad)
                    {
                        if (stopthread)
                        {
                            //restarting thread for pushing command to vMix
                            for (int i = 0; i < 5; i++)
                            {
                                ClearList(i);
                                if (!WorkloadThreads[i].IsAlive)
                                {
                                    workstart = new ThreadStart(WorkloadFunc);
                                    WorkloadThreads[i] = new Thread(workstart);
                                    WorkloadThreads[i].Name = Convert.ToString(i);
                                    WorkloadThreads[i].Start();
                                }
                                MasterClocks[i].Start();
                                WorkLayer = i;
                                ReloadSchedule(i);
                            }
                            stopthread = false;
                            updatedsettings();
                            //thread is stopped but vmix online and start is on = need the startup script;
                            if (settings.vMixRunScript) { Thread.Sleep(sleep); WebClient.Runscript(true, settings.vMixScript); }
                        }
                        //here the forced command
                        Thread.Sleep(sleep);//waiting abit for the asynchronus WebClient.UpdateData() method to complete otherwise WebClient is stuck
                        if (settings.vMixForceExternal) { WebClient.External(true); }
                        if (settings.vMixForceRecording) { WebClient.Recording(true); }
                        if (settings.vMixForceStreaming) { WebClient.Streaming(true); }
                        if (settings.vMixForceMulticorder) { WebClient.Multicorder(true); }
                    }
                    if (!online) { updatedsettings(); online = true; }
                }
            }
            CloseWindow();
        }

        private void WatchDogBark(object sender, FileSystemEventArgs e)
        {
            string name = e.Name[9].ToString();
            //for (int i = 0; i < 5; i++) 
            //{
            //    if (e.Name=="vMixSchedule"+0+".XML")
            //}
            int i = int.Parse(name);
            if (e.ChangeType == WatcherChangeTypes.Deleted)
                ClearList(int.Parse(name));
            else
            {
                //this.Enabled = false;
                ReloadSchedule(int.Parse(name));
                //this.Enabled = true;
                Thread.Sleep(1000);
                cleanvMix(i);
            }
        }

        private ListView FindListView(int WorkList) 
        {
            switch (WorkList) 
            {
                case 0: { return lvEventList0; }
                case 1: { return lvEventList1; }
                case 2: { return lvEventList2; }
                case 3: { return lvEventList3; }
                case 4: { return lvEventList4; }
            }
            return new ListView(); 
        }

        private void ClearList()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(ClearList));
            else
            {
                
                ClearList(WorkLayer);
                //lvEventList.SelectedIndices.Clear();
                //EventListLock.WaitOne();
                //for (int n = EventList.Count - 1; n >= 0; n--)
                //{
                //    MasterClock.RemoveMicroEvents(EventList[n]);
                //    if (EventList[n].EventType != vmEventType.input)
                //        WebClient.CloseInput(EventList[n].GUID);
                //    EventList.RemoveAt(n);
                //}
                //EventListLock.Release();
                //lvEventList.VirtualListSize = EventList.Count;
            }
        }

        private void ClearList(int WorkList) 
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate { ClearList(WorkList); }));
            else
            {
                ListView lvEventList = FindListView(WorkList);
                lvEventList.SelectedIndices.Clear();
                EventListLock.WaitOne();
                for (int n = EventLists[WorkList].Count - 1; n >= 0; n--)
                {
                        MasterClocks[WorkList].RemoveMicroEvents(EventLists[WorkList][n]);
                        //if (EventLists[WorkList][n].EventType != vmEventType.input)
                        //    WebClients[1].CloseInput(EventLists[WorkList][n].GUID);
                        EventLists[WorkList].RemoveAt(n);
                }
                cleanvMix(WorkList);
                if (trackinglist[WorkList].Count != 0) WebClients[1].CloseInput(trackinglist[WorkList][trackinglist[WorkList].Count - 1].GUID);
                lvEventList.VirtualListSize = 0;
                EventListLock.Release();
            }
        }

        private void ReloadSchedule()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(ReloadSchedule));
            else { 
                ReloadSchedule(WorkLayer);

            }
        }

        private void ReloadSchedule(int WorkList)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { ReloadSchedule( WorkList); }));
            }
            else
            {
                string schedulename = ScheduleFolder + "\\" + ScheduleFile[WorkList];
                if (File.Exists(schedulename))
                {
                    List<vMixEvent> vmes = new List<vMixEvent>();
                    XmlDocument d = new XmlDocument();
                    ListView lvEventList = FindListView(WorkList);
                    vMixEvent tokeep = new vMixEvent() ;
                    try
                    {
                        d.Load(schedulename);
                        lvEventList.SelectedIndices.Clear();

                        foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                        {
                            vMixEvent ne = new vMixEvent(n);
                            if (ne.EventEnd > DateTime.Now + new TimeSpan(0, 0, 0, settings.vMixPreload, 0))
                            {
                                vmes.Add(new vMixEvent(n));
                            }
                        }
                        vmes.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });

                        if (vmes.Count > 0)
                        {
                            DateTime revoke = vmes[0].EventStart;
                            for (int n = EventLists[WorkList].Count - 1; n > 0; n--)
                            {
                                {
                                    MasterClocks[WorkList].RemoveMicroEvents(EventLists[WorkList][n]);
                                    EventLists[WorkList].RemoveAt(n);
                                }
                                //else { tokeep = EventLists[WorkList][n]; ntokeep = n; }
                            }
                            if (EventLists[WorkList].Count > 0)
                            {
                                if (EventLists[WorkList][0].IsLike(vmes[0]))
                                    vmes.RemoveAt(0);
                                else if (revoke < DateTime.Now)
                                {
                                    MasterClocks[WorkList].RemoveMicroEvents(EventLists[WorkList][0]);
                                    WebClient.CloseInput(EventLists[WorkList][0].GUID);
                                    EventLists[WorkList].RemoveAt(0);
                                }
                                else MasterClocks[WorkList].RevokeMicroEvents(EventLists[WorkList][0], revoke);
                            }

                            foreach (vMixEvent vme in vmes) MasterClocks[WorkList].AddMicroEvents(vme);
                            
                            EventLists[WorkList].AddRange(vmes);
                            EventLists[WorkList].Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                            lvEventList.VirtualListSize = EventLists[WorkList].Count;
                            lvEventList.RedrawItems(0, EventLists[WorkList].Count - 1, false);
                            //if (vmes.Count > 0)
                            //{
                            //    if (EventLists[WorkList].Count != 0)
                            //        foreach (vMixEvent vme in vmes)
                            //            if (MasterClocks[WorkList].AddMicroEvents(vme))
                            //                EventLists[WorkList].Add(vme);

                            //    DateTime revoke = vmes[0].EventStart > DateTime.Now ? vmes[0].EventStart : DateTime.Now;
                            //    if (ntokeep == 0)
                            //    //foreach (vMixEvent ve in EventLists[WorkList])
                            //    {
                            //        MasterClocks[WorkList].RevokeMicroEvents(EventLists[WorkList][0], vmes[0].EventStart);
                            //    }
                            //    else
                            //    //foreach (vMixEvent ve in EventLists[WorkList])
                            //    {
                            //        //MasterClocks[WorkList].RevokeMicroEvents(tokeep, revoke);
                            //        //WebClient.CloseInput(tokeep.GUID);
                            //        //RemoveEvent(tokeep, WorkList);
                            //    }

                                
                            //}
                        }
                    }
                    catch { MessageBox.Show("Reloading failed"); }
                }
                cleanvMix(WorkList);
            }
        }

        private void cleanvMix(int WorkList)
        {
            int total = trackinglist[WorkList].Count;
            for (int j = 0; j < total - 1; j++)
            {
                int tic = 0;
                while (!WebClient.CloseInput(trackinglist[WorkList][j].GUID))
                {
                    Thread.Sleep(sleep);
                    tic++;
                    if (tic == 10) break;
                }
            }
       
        }

        private void bn_load_schedule_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            for (int i = 0; i < 5; i++) { WorkLayer = i; ReloadSchedule(i);}
            this.Enabled = true;
        }

        private void lvEventList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListView name = new ListView();

            for (int i = 0; i < 5; i++) 
            {
                name = FindListView(i);
                if (sender.Equals(name))
                {
                    if (e.ItemIndex < EventLists[i].Count)
                    e.Item = EventListItem(EventLists[i][e.ItemIndex]);
                }
            }
                
        }

        public ListViewItem EventListItem(vMixEvent vmixevent)
        {
            string[] caption = { 
                                   vmixevent.Title,
                                   vmixevent.EventStart.ToString("MM-dd  HH:mm:ss"),
                                   vmixevent.EventDuration.ToString(@"hh\:mm\:ss"),
                                   vmixevent.EventTypeString()                               
                               };
            ListViewItem lvi = new ListViewItem(caption);
            return lvi;
        }

        private void WorkloadFunc()
        {
            vMixMicroEvent vme;
            int i = int.Parse(Thread.CurrentThread.Name);
            
            while(!exitApp)
            {
                //PaintTime();
                if (Workloads[i].TryTake(out vme, 1000))
                {
                    vMixEvent evnt = vme.with;

                    if (vme.what.Equals(vmMicroEventType.exit))
                        break;

                    switch (vme.what)
                    {
                        case vmMicroEventType.prepare:
                            evnt.state = 0;//0 is not loaded, 1 is loded, 2 is running
                            while (evnt.EventEnd > DateTime.Now)
                            {
                                if (evnt.EventType == vmEventType.input)
                                {
                                    if (WebClients[1].GetGUID(evnt.Title, out evnt.GUID)) { evnt.state = 1; break; }
                                    else { Thread.Sleep(sleep); }
                                }
                                else if (evnt.EventType == vmEventType.black)
                                {
                                    if (WebClients[1].AddInput("Colour", evnt.Overlay, evnt.EventPath, evnt.GUID)) { evnt.state = 1; trackinglist[i].Add(evnt); break; }
                                    else { Thread.Sleep(sleep); }
                                }
                                else if (evnt.HasMedia)
                                {
                                    if (WebClients[1].AddInput(evnt.EventTypeString(), evnt.Overlay, evnt.EventPath, evnt.GUID))
                                    {
                                        evnt.state = 1;
                                        trackinglist[i].Add(evnt);
                                        break;
                                    }
                                    else { Thread.Sleep(sleep); }
                                }
                                else break;
                            }
                            break;
                        case vmMicroEventType.setup:
                            while (evnt.EventEnd > DateTime.Now)
                            {
                                if (!WebClients[1].SetupSlideshow(evnt.SlideshowInterval, evnt.SlideshowTypeString(), evnt.SlideshowTransitionTime, evnt.EventLooping, evnt.GUID)) Thread.Sleep(sleep);
                                else break;
                            }
                            break;
                        case  vmMicroEventType.fastforward :
                            int position;
                            while (evnt.EventEnd > DateTime.Now)
                            {
                                if (DateTime.Now > evnt.EventStart)
                                    position = (int)(DateTime.Now - evnt.EventStart + evnt.EventInPoint).TotalMilliseconds + 4000;
                                else
                                    position = (int)(evnt.EventInPoint).TotalMilliseconds;
                                if (!WebClients[1].ForwardTo(evnt.GUID, position)) Thread.Sleep(sleep);
                                else break;
                            }
                            break;
                        case vmMicroEventType.transition:
                            if (evnt.HasDuration) 
                            {
                                string type = evnt.TransitionTypeString();
                                int duration = evnt.EventTransitionTime;
                                while (evnt.EventEnd > DateTime.Now)
                                {
                                    if (!WebClients[1].Transition(evnt.GUID, evnt.Overlay, evnt.EventMuted, type, duration)) Thread.Sleep(sleep);
                                    else { evnt.state = 2; break; }
                                }
                            }
                            else if (evnt.EventType == vmEventType.manual)
                            {
                                string guid;
                                if (trackinglist[i].Count > 0)
                                {
                                    guid = trackinglist[i][trackinglist[i].Count - 1].GUID;
                                    if (trackinglist[i][trackinglist[i].Count - 1].EventType != vmEventType.input)
                                    while (evnt.EventEnd > DateTime.Now)
                                    {
                                        if (!WebClients[1].CloseInput(guid)) Thread.Sleep(sleep);
                                        else { evnt.state = 2; break; }
                                    }
                                }
                            }
                            else
                            {
                                string type = evnt.TransitionTypeString();
                                int duration = evnt.EventTransitionTime;
                                while (evnt.EventEnd > DateTime.Now)
                                {
                                    if (!WebClients[1].Transition(evnt.GUID, evnt.Overlay, type, duration)) Thread.Sleep(sleep);
                                    else { evnt.state = 2; break; }
                                }
                            }
                            break;
                        case vmMicroEventType.remove:
                            if (evnt.EventType != vmEventType.input)
                                for (int t = 0; t < 10; t++)
                                {
                                    if (!WebClients[1].CloseInput(evnt.GUID)) Thread.Sleep(sleep);
                                    else { break; }
                                }
                            //trackinglist[i].Remove(evnt); 
                            RemoveEvent(evnt, i);
                            break;                       
                    }
                }
                //EventListLock.Release();
                //Thread.Sleep(100);
            }
            //CloseWindow();
        }

        private void RemoveEvent(vMixEvent evnt, int WorkList)
        {
            
            try
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(delegate { RemoveEvent(evnt, WorkList); }));
                else
                {
                    ListView lvEventList = FindListView(WorkList);
                    //EventListLock.WaitOne();
                    EventLists[WorkList].Remove(evnt);
                    //EventListLock.Release();
                    lvEventList.VirtualListSize = EventLists[WorkList].Count;
                    lvEventList.RedrawItems(0, EventLists[WorkList].Count - 1, false);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);                
            }
        }

        private void PaintTime()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(PaintTime));
            else
                lbl_clock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void CloseWindow()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(CloseWindow));
            else
            {
                //for (int i = 0; i < 5; i++) { WorkloadThreads[i].Abort(); }
                settings.SaveSettings();
                this.Close();
            }
        }

        private void vMixControler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible && settings.vMixCloseTray)
            { e.Cancel = true; this.Hide(); }
            else
            {
                if (!exitApp)
                {
                    this.Hide();
                    // exitApp = true;
                    for (int i = 0; i < 5; i++)
                    {
                        WorkLayer = i;
                        exitApp = true;
                        //Workloads[i].Add(new vMixMicroEvent(vmMicroEventType.exit));
                        if (!stopthread) WorkloadThreads[i].Abort();
                        MasterClocks[i].Abort(); 
                        ClearList(i);
                    }
                    try { OnlineThread.Start(); }
                    catch { }
                    //Workload.Add(new vMixMicroEvent(vmMicroEventType.exit));
                    //MasterClock.Abort();
                    e.Cancel = true;
                }
            }
        }

        private void bn_showpreferences_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
            updatedsettings();
        }

        private void updatedsettings()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(updatedsettings));
            else
            {
                WebClient.URL = settings.vMixURL;
                for (int i = 0; i < 5; i++) WebClients[i].URL = settings.vMixURL;
                for (int i = 0; i < 5; i++)
                {
                    MasterClocks[i].Intervall = settings.vMixLate;
                    MasterClocks[i].MediaLinger = settings.vMixLinger;
                    MasterClocks[i].MediaPreload = settings.vMixPreload;
                }
                lb_address.Text = settings.vMixIP + ":" + settings.vMixPort.ToString();
                lb_load.Text = settings.vMixPreload.ToString() + "/" + settings.vMixLinger + " Sec";
                if (settings.vMixAutoLoad)
                {
                    bn_Autoload.Text = "ON";
                    bn_Autoload.BackColor = System.Drawing.Color.LimeGreen;
                    startToolStripMenuItem.Text = "Started";
                    startToolStripMenuItem.Checked = true;
                    startToolStripMenuItem.BackColor = System.Drawing.Color.LimeGreen;
                }
                else
                {
                    bn_Autoload.Text = "OFF";
                    bn_Autoload.BackColor = System.Drawing.Color.Red;
                    startToolStripMenuItem.Text = "Stopped";
                    startToolStripMenuItem.Checked = false;
                    startToolStripMenuItem.BackColor = System.Drawing.Color.Red;
                }
                if (WebClient.UpdateData())
                {
                    bn_Statuts.Text = "ONLINE";
                    bn_Statuts.BackColor = System.Drawing.Color.LimeGreen;
                    bn_Statuts.ToolTipText = "ONLINE";
                }
                else
                {
                    bn_Statuts.Text = "OFFLINE";
                    bn_Statuts.BackColor = System.Drawing.Color.Red;
                    bn_Statuts.ToolTipText = "OFFLINE";
                }
            }
        }

        private void bn_donate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/donate?hosted_button_id=8KWHCKS3TX54S");
        }

        private void TrayView_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }

        private void bn_Autoload_Click(object sender, EventArgs e)
        {
            if (!settings.vMixAutoLoad)
            {
                settings.vMixAutoLoad = true;
                bn_Autoload.Text = "ON";
                bn_Autoload.BackColor = System.Drawing.Color.LimeGreen;
            }
            else
            {
                settings.vMixAutoLoad = false;
                bn_Autoload.Text = "OFF";
                bn_Autoload.BackColor = System.Drawing.Color.Red;
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!settings.vMixAutoLoad)
            {
                settings.vMixAutoLoad = true;
                bn_Autoload.Text = "ON";
                bn_Autoload.BackColor = System.Drawing.Color.LimeGreen;
                startToolStripMenuItem.Text = "Started";
                startToolStripMenuItem.Checked = true;
                startToolStripMenuItem.ToolTipText = "click to stop";
                startToolStripMenuItem.BackColor = System.Drawing.Color.LimeGreen;
            }
            else
            {
                settings.vMixAutoLoad = false;
                bn_Autoload.Text = "OFF";
                bn_Autoload.BackColor = System.Drawing.Color.Red;
                startToolStripMenuItem.Text = "Stopped";
                startToolStripMenuItem.Checked = false;
                startToolStripMenuItem.ToolTipText = "click to start";
                startToolStripMenuItem.BackColor = System.Drawing.Color.Red;
            }
        }

        private void vMixControler_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
                if (settings.vMixMiniTray) this.Hide();
        }

        private void vMixControler_Shown(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                if (settings.vMixMiniTray) Hide();
        }

        private void settingsStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
            updatedsettings();
        }

        private void bn_vManager_Click(object sender, EventArgs e)
        {
            string vManagerPath = Path.GetDirectoryName(Application.ExecutablePath) + "//vManager.exe";
            string processname = Path.GetFileNameWithoutExtension("vManager");

            if (System.Diagnostics.Process.GetProcessesByName(processname).Length == 0)
            {
                if (File.Exists(vManagerPath))
                    System.Diagnostics.Process.Start(vManagerPath);
                else MessageBox.Show("vManager executable not found.\nCheck if vManager is in the same directory as me. ", "Cannot start vManager", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("vManager seems to be running", "Cannot start vManager", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void bn_about_Click(object sender, EventArgs e)
        {
            new vAbout().ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new vAbout().ShowDialog();
        }
    }
}
