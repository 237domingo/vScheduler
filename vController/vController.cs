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
        List<vMixEvent>[] EventLists = new List<vMixEvent>[5];
        Semaphore EventListLock;

        //vMixScheduler MasterClock;
        vMixScheduler[] MasterClocks = new vMixScheduler[5];

        //BlockingCollection<vMixMicroEvent> Workload;
        BlockingCollection<vMixMicroEvent>[] Workloads = new BlockingCollection<vMixMicroEvent>[5] ;
        
        
        //Thread WorkloadThread;
        Thread[] WorkloadThreads = new Thread[5];

        vMixWebClient WebClient;
        
        bool exitApp = false;
        bool[] Reloading = { false, false, false, false, false };
        int WorkLayer = 0;

        //FileSystemWatcher WatchDog;
        FileSystemWatcher[] WatchDogs = new FileSystemWatcher[5];
        string ScheduleFolder;
        string[] ScheduleFile = { "vMixSchedule0.xml", "vMixSchedule1.xml", "vMixSchedule2.xml", "vMixSchedule3.xml", "vMixSchedule4.xml" };

        public vMixControler()
        {
            InitializeComponent();
        }

        private void vMaster_Load(object sender, EventArgs e)
        {

            EventListLock = new Semaphore(1, 1);
            EventList = new List<vMixEvent>();
            for (int i = 0; i < 5; i++) { EventLists[i] = new List<vMixEvent>(); }
            for (int i = 0; i < 5; i++) { Workloads[i] = new BlockingCollection<vMixMicroEvent>(); }

            for (int i = 0; i < 5; i++)
            {
                //Workload = Workloads[i];
                ThreadStart workstart = new ThreadStart(WorkloadFunc);
                WorkloadThreads[i] = new Thread(workstart);
                WorkloadThreads[i].Name = Convert.ToString(i);
                WorkloadThreads[i].Start();
            }
            //Workload = Workloads[0];
            //WorkloadThread = WorkloadThreads[0];
            
            ScheduleFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";
            if (!Directory.Exists (ScheduleFolder)) Directory.CreateDirectory (ScheduleFolder);
            
            for (int i = 0; i < 5; i++)
            {
                WatchDogs[i] = new FileSystemWatcher(ScheduleFolder, ScheduleFile[i]);
                WatchDogs[i].NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                WatchDogs[i].Changed += new FileSystemEventHandler(WatchDogBark);
                WatchDogs[i].Created += new FileSystemEventHandler(WatchDogBark);
                WatchDogs[i].Deleted += new FileSystemEventHandler(WatchDogBark);
                WatchDogs[i].EnableRaisingEvents = true;
            }
            
            //WatchDog = new FileSystemWatcher(ScheduleFolder, "vMixSchedule.xml");
            //WatchDog.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
            //WatchDog.Changed += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.Created += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.Deleted += new FileSystemEventHandler(WatchDogBark);
            //WatchDog.EnableRaisingEvents = true;

            if (!File.Exists(ScheduleFolder + "Settings.xml")) File.Create(ScheduleFolder + "Settings.xml");

            settings = new vPreferences();

            for (int i = 0; i < 5; i++) { MasterClocks[i] = new vMixScheduler(100, settings.vMixPreload, settings.vMixLinger, Workloads[i]); }
            //MasterClock = new vMixScheduler(100, settings.vMixPreload , settings.vMixLinger, Workload);
            WebClient = new vMixWebClient(settings.vMixURL);

            if (settings.vMixAutoLoad)
            {
                this.Enabled = false;
                for (int i = 0; i < 5; i++) { WorkLayer = i; ReloadSchedule(); }
                this.Enabled = true;
            }
        }

        private void WatchDogBark(object sender, FileSystemEventArgs e)
        {
            string name = e.Name[12].ToString();
            //for (int i = 0; i < 5; i++) 
            //{
            //    if (e.Name=="vMixSchedule"+0+".XML")
            //}
            WorkLayer = int.Parse(name);
            if (e.ChangeType == WatcherChangeTypes.Deleted)
                ClearList();
            else
            {
                //this.Enabled = false;
                ReloadSchedule();
                //this.Enabled = true;
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
            ListView lvEventList = FindListView(WorkList);
            lvEventList.SelectedIndices.Clear();
            EventListLock.WaitOne();
            for (int n = EventLists[WorkList].Count - 1; n >= 0; n--)
            {
                MasterClocks[WorkList].RemoveMicroEvents(EventLists[WorkList][n]);
                if (EventLists[WorkList][n].EventType != vmEventType.input)
                    WebClient.CloseInput(EventLists[WorkList][n].GUID);
                EventLists[WorkList].RemoveAt(n);
            }
            EventListLock.Release();
            lvEventList.VirtualListSize = EventLists[WorkList].Count;
            
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
            string schedulename = ScheduleFolder + "\\" + ScheduleFile[WorkList];
            if (File.Exists(schedulename))
            {
                List<vMixEvent> vmes = new List<vMixEvent>();
                XmlDocument d = new XmlDocument();
                ListView lvEventList = FindListView(WorkList);
                try
                {
                    d.Load(schedulename);
                    lvEventList.SelectedIndices.Clear();
                    vmes.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                    for (int n = EventLists[WorkList].Count - 1; n >= 0; n--)
                    {
                        if (!EventLists[WorkList][n].IsLoaded)
                        {
                            MasterClocks[WorkList].RemoveMicroEvents(EventLists[WorkList][n]);
                            EventLists[WorkList].RemoveAt(n);
                        }
                    }

                    foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                    {
                        vMixEvent ne = new vMixEvent(n);
                        if (ne.EventEnd > DateTime.Now)
                        {
                            bool found = false;
                            foreach (vMixEvent ve in EventLists[WorkList])
                                if (ve.IsLike(ne))
                                    found = true;
                            if (!found)
                                vmes.Add(new vMixEvent(n));
                        }
                    }

                    if (vmes.Count > 0)
                    {
                        DateTime revoke = vmes[0].EventStart > DateTime.Now ? vmes[0].EventStart : DateTime.Now;
                        foreach (vMixEvent ve in EventLists[WorkList])
                            MasterClocks[WorkList].RevokeMicroEvents(ve, revoke);
                        foreach (vMixEvent vme in vmes)
                            if (MasterClocks[WorkList].AddMicroEvents(vme))
                                EventLists[WorkList].Add(vme);
                        EventLists[WorkList].Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                        lvEventList.VirtualListSize = EventLists[WorkList].Count;
                        lvEventList.RedrawItems(0, EventLists[WorkList].Count - 1, false);
                    }
                }
                catch { }
            }
        }

        private void bn_load_schedule_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            for (int i = 0; i < 5; i++) { WorkLayer = i; ReloadSchedule();}
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
                PaintTime();
                if (Workloads[i].TryTake(out vme, 1000))
                {
                    vMixEvent evnt = vme.with;

                    if (vme.what.Equals(vmMicroEventType.exit))
                        break;

                    switch (vme.what)
                    {
                        case vmMicroEventType.prepare:
                            evnt.state = 1;
                            if (evnt.EventType == vmEventType.input)
                            {
                                while (!WebClient.GetGUID(evnt.Title, out evnt.GUID))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            else if (evnt.EventType == vmEventType.black)
                            {
                                while (!WebClient.AddInput("Colour", "black", evnt.GUID))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            else if (evnt.HasMedia)
                            {
                                while (!WebClient.AddInput(evnt.EventTypeString(), evnt.EventPath, evnt.GUID))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            break;
                        case vmMicroEventType.setup:
                            {
                                while (!WebClient.SetupSlideshow (evnt.SlideshowInterval,evnt.SlideshowTypeString(),evnt.SlideshowTransitionTime, evnt.GUID))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            break;
                        case  vmMicroEventType.fastforward :
                            if (evnt.HasDuration)
                            {
                                int position = (int)(DateTime.Now - evnt.EventStart + evnt.EventInPoint).TotalMilliseconds;
                                while (!WebClient.ForwardTo(evnt.GUID, position))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            break;
                        case vmMicroEventType.transition:
                            evnt.state = 2;
                            if (evnt.EventType == vmEventType.input)
                            {
                                string type = evnt.TransitionTypeString();
                                int duration = evnt.EventTransitionTime;
                                while (!WebClient.Transition(evnt.GUID, evnt.Overlay, type, duration))
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            else if (evnt.HasMedia)
                            {
                                string type = evnt.TransitionTypeString();
                                int duration = evnt.EventTransitionTime;
                                while (!WebClient.Transition(evnt.GUID, evnt.Overlay, type, duration))
                                {
                                    Thread.Sleep(0);
                                }
                                while(!WebClient.MuteAudio(evnt.EventMuted,evnt.GUID))
                                    Thread.Sleep(0);
                            }
                            break;
                        case vmMicroEventType.remove:
                            if (evnt.EventType != vmEventType.input)
                                while (!WebClient.CloseInput(evnt.GUID))
                                {
                                    Thread.Sleep(0);
                                }
                            RemoveEvent(evnt, i);
                            break;                       
                    }
                }
            }
            CloseWindow();
            
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
                    EventListLock.WaitOne();
                    EventLists[WorkList].Remove(evnt);
                    EventListLock.Release();
                    lvEventList.VirtualListSize = EventLists[WorkList].Count;
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
                for (int i = 0; i < 5; i++) { WorkloadThreads[i].Abort(); }
                this.Close();
            }
        }

        private void vMixControler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exitApp)
            {
                exitApp = true;
                for (int i = 0; i < 5; i++) {
                    WorkLayer = i;
                    Workloads[i].Add(new vMixMicroEvent(vmMicroEventType.exit));
                    MasterClocks[i].Abort();
                    ClearList();
                }
                //Workload.Add(new vMixMicroEvent(vmMicroEventType.exit));
                //MasterClock.Abort();
                e.Cancel = true;
            }
        }

        private void bn_showpreferences_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
            WebClient.URL = settings.vMixURL;
            for (int i = 0; i < 5; i++) 
            {
                MasterClocks[i].Intervall = 100;
                MasterClocks[i].MediaLinger = settings.vMixLinger;
                MasterClocks[i].MediaPreload = settings.vMixPreload;
            }
                
        }
    }
}
