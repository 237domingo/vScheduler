using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.IO;
using MediaInfoLib;
using vManager;

namespace vManager
{
    public partial class vMixManager : Form
    {
        List<vMixEvent> vMixEvents;
        List<vMixEvent> vMixEvents0 = new List<vMixEvent>();
        List<vMixEvent> vMixEvents1 = new List<vMixEvent>();
        List<vMixEvent> vMixEvents2 = new List<vMixEvent>();
        List<vMixEvent> vMixEvents3 = new List<vMixEvent>();
        List<vMixEvent> vMixEvents4 = new List<vMixEvent>();
        List<vMixEvent>[] ListOfvMixEvents = new List<vMixEvent>[5];
        ListView[] ListOfEventList = new ListView[5];
        vMixEvent ActiveEvent;
        MediaInfo FileInfo;
        ListView EventList;
        String ActiveOverlay;
        List<vMixEvent> copybuffer = new List<vMixEvent>();
        bool rebuildhasoccur = false;
        string formtitle;
        string openedfile;
        readonly string defaultfilename = "untitled.xml";
        public delegate void SelectedOverlay(bool[] Overlay);
        public delegate int FixPath(out vMixEvent Event, vMixEvent evnt);
        bool donotredraw = false;
        //
        //settings for vManager
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";
        private List<string> recentfiles = new List<string>();
        int maxrecents = 5;
        bool fixonload;
        Xml settings;
        FindBox finder = new FindBox();
        //
        //Library init
        vLibrary library;

        public vMixManager()
        {
            InitializeComponent();
            FileInfo = new MediaInfo();
            ListOfvMixEvents[0] = vMixEvents0;
            ListOfvMixEvents[1] = vMixEvents1;
            ListOfvMixEvents[2] = vMixEvents2;
            ListOfvMixEvents[3] = vMixEvents3;
            ListOfvMixEvents[4] = vMixEvents4;
            ListOfEventList[0] = EventList0;
            ListOfEventList[1] = EventList1;
            ListOfEventList[2] = EventList2;
            ListOfEventList[3] = EventList3;
            ListOfEventList[4] = EventList4;
            if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);
            settings = new Xml();
            settings.LoadXml(SettingsPath + "\\Settings.xml");
            library = new vLibrary(settings);
            recentfiles.AddRange(settings.GetValue("vManager", "recentfiles", "").Split('|'));
            maxrecents = settings.GetValue("vManager", "maxrecents", 5);
            fixonload = settings.GetValue("vManager", "fixonload", false);
            vMixEvents = vMixEvents0;
            EventList = EventList0;
            ActiveOverlay = "0";
            formtitle = Text;
        }

        private void vMixManager_Load(object sender, EventArgs e)
        {
            dtp_timetable.Value = DateTime.Today + new TimeSpan(1, 0, 0, 0, 0);
            lb_event.Text = "Video";
            lb_transition.Text = "Fade";
            lb_slideshow_transition.Text = "Fade";
            Text = formtitle + " - " + defaultfilename;
            openedfile = defaultfilename;
            autoFixOnLoadToolStripMenuItem.Checked = fixonload;
            foreach (string s in recentfiles) updaterecentfilestoolstripmenu(s);
        }

        private void updaterecentfilestoolstripmenu(string filepath)
        {
            if (filepath == "") return;
            ToolStripItem toolstripitem;
            toolstripitem = recentfilestoolStripMenuItem.DropDownItems[filepath];
            if (toolstripitem != null)
            {
                recentfilestoolStripMenuItem.DropDownItems.Remove(toolstripitem);
            }
            else
            {
                if (recentfilestoolStripMenuItem.DropDownItems.Count >= maxrecents + 1)
                {
                    toolstripitem = recentfilestoolStripMenuItem.DropDownItems[maxrecents - 1];
                    toolstripitem.Text = filepath;
                    recentfilestoolStripMenuItem.DropDownItems.Remove(toolstripitem);
                }
                else
                {
                    toolstripitem = recentfilestoolStripMenuItem.DropDownItems.Add(filepath);
                    toolstripitem.Click += new System.EventHandler(recentfilestoolStripItem_Click);
                }
                toolstripitem.Name = filepath;
            }
            toolstripitem.Visible = true;
            recentfilestoolStripMenuItem.DropDownItems.Insert(0, toolstripitem);
            recentfilestoolStripMenuItem.Enabled = true;
        }

        private void EventList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListView name = new ListView();

            for (int i = 0; i < 5; i++)
            {
                name = ListOfEventList[i];
                if (sender.Equals(name))
                {
                    if (e.ItemIndex < ListOfvMixEvents[i].Count)
                        e.Item = EventListItem(ListOfvMixEvents[i][e.ItemIndex]);
                    return;
                }
            }
        }
        
        public ListViewItem EventListItem(vMixEvent vmixevent)
        {
            string[] caption = { 
                                   vmixevent.Title,
                                   vmixevent.EventStart.ToString("MM-dd  HH:mm:ss"),
                                   vmixevent.EventDuration.ToString(@"hh\:mm\:ss"),
                                   vmixevent.EventTypeString(),
                                   vmixevent.EventPath 
                               };
            ListViewItem lvi = new ListViewItem(caption);
            lvi.ToolTipText = vmixevent.EventInfoText;
            if (vmixevent.MediaType >= 2) {if (!File.Exists(vmixevent.EventPath)) lvi.BackColor = Color.Red; }
            else if (vmixevent.MediaType == 1) { if (!Directory.Exists(vmixevent.EventPath)) lvi.BackColor = Color.Red; }
            if (vmixevent.EventType == vmEventType.black) { if (ColorFromString(vmixevent.EventPath).IsEmpty) lvi.BackColor = Color.Red; }
            return lvi;
        }

        private void DefaultDetailsView()
        {
            tb_title.Text = "";
            dtp_start.Text = "00:00:00";
            dtp_inpoint.Text = "00:00:00";
            dtp_duration.Text = "00:00:00";
            dtp_end.Text = "00:00:00";
            cb_looping.Checked = false;
            cb_keep_duration.Checked = false;
            lb_transition.SelectedIndex = 0;
            ud_transition_time.Value = 500;
            rtb_fileinfo.Text = "";
            pnl_slideshow.Visible = false;
            cb_audio.Checked = false;
            EventDetails.Enabled = false;
            pictureBox1.Image = null;
        }

        private void UpdateDisplay()
        {
            if (donotredraw)
                return;

            donotredraw = true;

            if (ActiveEvent != null)
            {
                EventDetails.Enabled = true; 
                tb_title.Text = ActiveEvent.Title;
                dtp_start.Value = ActiveEvent.EventStart;
                dtp_duration.Text = ActiveEvent.EventDuration.ToString("c");
                dtp_inpoint.Text = ActiveEvent.EventInPoint.ToString("c");
                dtp_end.Value = ActiveEvent.EventEnd;

                cb_audio.Checked = !ActiveEvent.EventMuted;
                cb_audio.Enabled = (ActiveEvent.EventType == vmEventType.video);

                cb_keep_duration.Checked = ActiveEvent.KeepDuration;
                cb_keep_duration.Enabled = ActiveEvent.HasDuration;

                dtp_duration.Enabled = !ActiveEvent.KeepDuration || !ActiveEvent.HasDuration;
                dtp_inpoint.Enabled = ActiveEvent.HasDuration && !ActiveEvent.KeepDuration;
                
                lb_transition.Text = ActiveEvent.TransitionTypeString();
                
                ud_transition_time.Enabled = (ActiveEvent.EventTransition != vmTransitionType.cut);
                ud_transition_time.Value = ActiveEvent.EventTransitionTime;
                
                rtb_fileinfo.Text = ActiveEvent.EventInfoText;

                pnl_slideshow.Visible = (ActiveEvent.EventType == vmEventType.photos);
                ud_slideshow_interval.Value = ActiveEvent.SlideshowInterval;
                lb_slideshow_transition.Text = ActiveEvent.SlideshowTypeString();
                ud_slideshow_transition.Value = ActiveEvent.SlideshowTransitionTime;
                cb_looping.Checked = ActiveEvent.EventLooping;

                switch (ActiveEvent.EventType)
                {
                    case vmEventType.image:
                        pictureBox1.ImageLocation = ActiveEvent.EventPath;
                        break;
                    case vmEventType.photos:
                        pictureBox1.Image = null;
                        List <string> i = Directory.EnumerateFiles(ActiveEvent.EventPath).ToList(); 
                        foreach (string a in i)
                         {
                            if (a.ToLower().Contains(".jpg")) { pictureBox1.ImageLocation = a; break; }
                            if (a.ToLower().Contains(".jpeg")) { pictureBox1.ImageLocation = a; break; }
                            if (a.ToLower().Contains(".png")) { pictureBox1.ImageLocation = a; break; }
                            if (a.ToLower().Contains(".bmp")) { pictureBox1.ImageLocation = a; break; }
                          }   
                        break;
                    case vmEventType.black:
                        pictureBox1.BackColor = ColorFromString(ActiveEvent.EventPath);
                        break;
                    default:
                        pictureBox1.Image = null;
                        break;
                }
            }
            else 
            {
                DefaultDetailsView();
            }
            donotredraw = false;
        }

        private void EventList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveEvent = null;
            if (EventList.SelectedIndices.Count == 0) { UpdateDisplay(); return; }
            if (EventList.SelectedIndices.Count == 1)
            {
                ActiveEvent = vMixEvents[EventList.SelectedIndices[0]];
                bn_move_up.Enabled = (EventList.SelectedIndices[0] > 0);
                bn_move_down.Enabled = (EventList.SelectedIndices[0] < vMixEvents.Count - 1);
                bn_remove.Enabled = true;
                bn_clone.Enabled = true;
                bn_splice.Enabled = true;
                bn_shuffle.Enabled = false;
            }
            else
            {
                int lastindex = EventList.SelectedIndices[EventList.SelectedIndices.Count-1];
                ActiveEvent = vMixEvents[lastindex];
                bn_move_up.Enabled = false;
                bn_move_down.Enabled = false;
                bn_remove.Enabled = true;
                bn_clone.Enabled = false;
                bn_splice.Enabled = false;
                bn_shuffle.Enabled = true;
            }
            UpdateDisplay();
        }

        private void tb_title_TextChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
            {
                ActiveEvent.Title = tb_title.Text;
                EventList.RedrawItems(EventList.SelectedIndices[0], EventList.SelectedIndices[0], false);
            }
        }

        private void lb_transition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
            {
                ActiveEvent.EventTransition = ActiveEvent.TransitionTypeFromString(lb_transition.Text);
                if (ActiveEvent.EventTransition == vmTransitionType.cut)
                {
                    ActiveEvent.EventTransitionTime = 0;
                    RebuildTimetable();
                }
                UpdateDisplay();
            }
        }

        private void cb_looping_CheckedChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
                ActiveEvent.EventLooping = cb_looping.Checked;
        }

        private void cb_audio_CheckedChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
                ActiveEvent.EventMuted = !cb_audio.Checked;
        }

        private void ud_transition_time_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
            {
                ActiveEvent.EventTransitionTime = (int)ud_transition_time.Value;
                RebuildTimetable();
                UpdateDisplay();
            }
        }

        private void cb_keep_duration_CheckedChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent == null) return;
            if (cb_keep_duration.Checked)
            {
                ActiveEvent.EventInPoint = new TimeSpan(0);
                ActiveEvent.EventDuration = ActiveEvent.MediaDuration;
                ActiveEvent.KeepDuration = true;
                dtp_duration.Enabled = false;
                dtp_inpoint.Enabled = false;
                RebuildTimetable();
                UpdateDisplay();
            }
            else
            {
                ActiveEvent.KeepDuration = false;
                UpdateDisplay();
            }
        }

        private void RebuildTimetable()
        {
            if (vMixEvents.Count == 0) return;
            DateTime nextstart = dtp_timetable.Value;
            for (int i = 0; i < vMixEvents.Count; i++)
            {
                vMixEvent e = vMixEvents [i];
                if (i > 0)
                    nextstart -= new TimeSpan(0, 0, 0, 0, e.EventTransitionTime);
                if (e.EventStart != nextstart)
                    e.EventStart = nextstart;
                nextstart += e.EventDuration;
            }
            EventList.RedrawItems(0, vMixEvents.Count -1, false);
            if (vMixEvents.Count > 0)
            {
                dtp_endtime.Text = vMixEvents[vMixEvents.Count - 1].EventEnd.ToString("yyyy/MM/dd HH:mm:ss");
            }
            rebuildhasoccur = true;
        }

        private void SpliceEvent()
        {
            //vMixEvent v = vMixEvents.Find(delegate(vMixEvent e) { return e.EventEnd > timetofind;});
            if (ActiveEvent == null) return;
            if (EventList.SelectedIndices.Count != 1) return;
            int position = vMixEvents.IndexOf(ActiveEvent) + 1;
            donotredraw = true;
            vMixEvent copy = new vMixEvent(ActiveEvent);
            ActiveEvent.EventDuration -= new TimeSpan(ActiveEvent.EventDuration.Ticks/2);
            copy.EventDuration = ActiveEvent.EventDuration;
            copy.EventInPoint += ActiveEvent.EventDuration;
            vMixEvents.Insert(position, copy);
            ActiveEvent = copy;
            EventList.VirtualListSize = vMixEvents.Count;
            EventList.SelectedIndices.Clear();
            EventList.SelectedIndices.Add(position);
            RebuildTimetable();
            donotredraw = false;
            UpdateDisplay();

        }

        private void shuffle()
        {
            if (EventList.SelectedIndices.Count > 1)
            {
                Random x = new Random();
                vMixEvent temp1;
                vMixEvent temp2;
                int b, a;//exchanging a random element "b" from de selected item with an element "a"
                int eventnumber = EventList.SelectedIndices.Count - 1;
                for (int n = 0; n < eventnumber ; n++)
                {
                    a = EventList.SelectedIndices[n];
                    b = EventList.SelectedIndices[x.Next(n + 1, eventnumber)];
                    temp1 = vMixEvents[a];
                    temp2 = vMixEvents[b];
                    vMixEvents.RemoveAt(a);
                    vMixEvents.Insert(a, temp2);
                    vMixEvents.RemoveAt(b);
                    vMixEvents.Insert(b, temp1);
                }
                RebuildTimetable();
            }
        }

        private void find(int startindex, int count, bool first, DateTime date)
        {
            int i;
            if (first) i = vMixEvents.FindIndex(startindex, count, delegate(vMixEvent e1) { return e1.EventEnd > date; });
            else i = vMixEvents.FindLastIndex(startindex, count, delegate(vMixEvent e1) { return e1.EventEnd > date; });
            if (i >= 0)
            {
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(i);
            }
        }

        private void find(int startindex, int count, bool first, string title)
        {
            if (title == "") return;
            int i;
            if (first) i = vMixEvents.FindIndex(startindex, count, delegate(vMixEvent e1) { return e1.Title.ToLower().Contains(title.ToLower()); });
            else i = vMixEvents.FindLastIndex(startindex, count, delegate(vMixEvent e1) { return e1.Title.ToLower().Contains(title.ToLower()); });
            if (i >= 0)
            {
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(i);
            }
        }

        private void selectall()
        {
            EventList.SelectedIndices.Clear();
            EventList.SuspendLayout();
            for (int n = 0; n < EventList.VirtualListSize; n++)
            { EventList.SelectedIndices.Add(n); }
            EventList.ResumeLayout(true);
        }

        private void dtp_timetable_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (vMixEvents.Count > 0)
            {
                if (dtp_timetable.Focused)
                {
                    vMixEvents[0].EventStart = dtp_timetable.Value;
                    RebuildTimetable();
                    UpdateDisplay();
                }
            }
        }

        private void dtp_duration_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
            {
                TimeSpan dr = dtp_duration.Value.TimeOfDay;
                if (ActiveEvent.HasDuration && dr + ActiveEvent.EventInPoint > ActiveEvent.MediaDuration)
                    dr = ActiveEvent.MediaDuration - ActiveEvent.EventInPoint;
                if (dr > new TimeSpan(0, 0, 0))
                    ActiveEvent.EventDuration = dr;
                else
                    ActiveEvent.EventDuration = new TimeSpan(0);
                RebuildTimetable();
                UpdateDisplay();
            }
        }

        private void dtp_inpoint_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                TimeSpan ip = dtp_inpoint.Value.TimeOfDay;
                if (ip > ActiveEvent.MediaDuration )
                {
                    ip = ActiveEvent .MediaDuration ;
                    dtp_inpoint.Text = ip.ToString("c");
                }
                if (ip > new TimeSpan(0))
                    ActiveEvent.EventInPoint = ip;
                else
                    ActiveEvent.EventInPoint = new TimeSpan(0);
                if (ip + ActiveEvent.EventDuration > ActiveEvent.MediaDuration)
                    ActiveEvent.EventDuration = ActiveEvent.MediaDuration - ip;

                RebuildTimetable();
                UpdateDisplay();
            }
        }

        private void ud_slideshow_interval_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
                ActiveEvent.SlideshowInterval = (int)ud_slideshow_interval.Value;
        }
        private void lb_slideshow_transition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
                ActiveEvent.SlideshowTransition = ActiveEvent.TransitionTypeFromString(lb_slideshow_transition.Text);
        }
        private void ud_slideshow_transition_ValueChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            if (ActiveEvent != null)
                ActiveEvent.SlideshowTransitionTime = (int)ud_slideshow_transition.Value;
        }
        private void bn_move_up_Click(object sender, EventArgs e)
        {
            if (ActiveEvent == null) return;
            int position = vMixEvents.IndexOf(ActiveEvent);
            if (position > 0)
            {
                donotredraw = true;
                vMixEvents.RemoveAt(position);
                vMixEvents.Insert(position - 1, ActiveEvent);
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(position - 1);
                RebuildTimetable();
                donotredraw = false;
                UpdateDisplay();
            }
        }

        private void bn_move_down_Click(object sender, EventArgs e)
        {
            if (ActiveEvent == null) return;
            int position = vMixEvents.IndexOf(ActiveEvent);
            if (position < vMixEvents.Count - 1)
            {
                donotredraw = true;
                vMixEvents.RemoveAt(position);
                vMixEvents.Insert(position + 1, ActiveEvent);
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(position + 1);
                RebuildTimetable();
                donotredraw = false;
                UpdateDisplay();
            }
        }
        private void bn_clone_Click(object sender, EventArgs e)
        {
            if (ActiveEvent == null) return;
            int position = vMixEvents.IndexOf(ActiveEvent) + 1;
            donotredraw = true;
            vMixEvent copy = new vMixEvent(ActiveEvent);
            vMixEvents.Insert(position, copy);
            ActiveEvent = copy;
            EventList.VirtualListSize = vMixEvents.Count;
            EventList.SelectedIndices.Clear();
            EventList.SelectedIndices.Add(position);
            RebuildTimetable();
            donotredraw = false;
            UpdateDisplay();
        }
        private void bn_remove_Click(object sender, EventArgs e)
        {
            if (ActiveEvent == null) return;
            int position = vMixEvents.IndexOf(ActiveEvent);
            List<vMixEvent> toremove = new List<vMixEvent>();
            donotredraw = true;
            foreach (int l in EventList.SelectedIndices) toremove.Add(vMixEvents[l]);
            foreach (vMixEvent remove in toremove) vMixEvents.Remove(remove);
            ActiveEvent = null;
            EventList.VirtualListSize = vMixEvents.Count;
            EventList.SelectedIndices.Clear();
            RebuildTimetable();
            donotredraw = false;
            UpdateDisplay();
        }

        private void save_click(object sender, EventArgs e)
        {
            AutoSave(sender, e);
        }
        
        private bool AutoSave(object sender, EventArgs e)
        {
            if (openedfile == defaultfilename) return save_as();
            else { save(openedfile); return true; }
        }

        private void save(string filename)
        {
            XmlDocument d = new XmlDocument();
            XmlNode root = d.CreateElement("vMixManager");
            d.AppendChild(root);
            XmlNode events = d.CreateElement("Events");
            root.AppendChild(events);
            int eventcount = 0;
            foreach (List<vMixEvent> lvme in ListOfvMixEvents)
            {
                foreach (vMixEvent vme in lvme)
                events.AppendChild(vme.ToXMLNode(d));
                eventcount = eventcount + lvme.Count;
            }
            d.Save(filename);
            MessageBox.Show(eventcount.ToString() + " events saved to xml.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openedfile = filename;
            Text = formtitle + "(" + Path.GetFileName(filename) + ")";
            updaterecentfiles(filename);
            updaterecentfilestoolstripmenu(filename);
            rebuildhasoccur = false;
        }

        private bool save_as()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML-File|*.xml|all Files|*.*";
            sfd.FileName = dtp_timetable.Value.ToString("yyyy-MM-dd");
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save(sfd.FileName);
                return true;
            }
            else return false;
        }

        private void bn_clear_Click(object sender, EventArgs e)
        {
            vOverlaySelection vOver = new vOverlaySelection(ActiveOverlay, "Wich layers\r\nto clear", true, true);
            vOver.SelectedOverlayValue = new SelectedOverlay(ClearPlaylist);
            vOver.ShowDialog();
        }

        private void ClearPlaylist(bool[] Overlay) 
        {
            List<vMixEvent> tempvMixEvents;
            ListView tempEventList;
            tempEventList = EventList; //backup active list box of event
            tempvMixEvents = vMixEvents; //backup active list of event

            for (int i = 0; i < Overlay.Length; i++)
            {
                if (Overlay[i]) 
                {
                    EventList = ListOfEventList[i] ;
                    vMixEvents = ListOfvMixEvents[i] ;
                    EventList.SelectedIndices.Clear();
                    donotredraw = true;
                    vMixEvents.Clear();
                    if (ActiveOverlay == Convert.ToString(i)) 
                    {
                        ActiveEvent = null;
                        UpdateDisplay();
                    }
                    EventList.VirtualListSize = 0;
                    donotredraw = false;
                }
            }
            EventList = tempEventList; //restore
            vMixEvents = tempvMixEvents; //restore
            
        }

        private void bn_load_Click(object sender, EventArgs e)
        {
            if (rebuildhasoccur)
            {
                DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes) 
                    if (!AutoSave(sender, e)) return;
                if (result == DialogResult.Cancel) return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML-Files|*.xml|all Files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                load(ofd.FileName);
            }
        }

        private void load (string filename)
        {
            List<vMixEvent> vmes = new List<vMixEvent>();
            List<vMixEvent>[] ListOfvmes = new List<vMixEvent>[5];
            XmlDocument d = new XmlDocument();
            d.Load(filename);

            bool[] a = { true, true, true, true, true };
            ClearPlaylist(a);
            int eventcount = 0;
            vMixEvent k;
            foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
            {
                k = new vMixEvent(n);
                if (fixonload) fixpath(out k, k);
                try
                {
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)].Add(new vMixEvent(k));
                    eventcount++;
                }
                catch
                {
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)] = new List<vMixEvent>();
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)].Add(new vMixEvent(k));
                    eventcount++;
                }
            }

            donotredraw = true;
            ListView tempEventList = EventList;
            List<vMixEvent> tempvMixEvents = vMixEvents;
            string tempActiveOverlay = ActiveOverlay;
            for (int i = 0; i < 5; i++)
            {
                int count = ListOfvMixEvents[i].Count;

                if (count > 0)
                {
                    ActiveEvent = null;
                    ListOfvMixEvents[i].Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                    vMixEvents = ListOfvMixEvents[i];
                    ActiveOverlay = Convert.ToString(i);
                    EventList = ListOfEventList[i];
                    EventList.SelectedIndices.Clear();
                    EventList.VirtualListSize = count;
                    dtp_timetable.Value = ListOfvMixEvents[i][0].EventStart;
                    RebuildTimetable();
                    //UpdateDisplay();
                }

            }
            donotredraw = false;
            EventList = tempEventList;
            vMixEvents = tempvMixEvents;
            ActiveOverlay = tempActiveOverlay;
            if (vMixEvents.Count > 0)
            {
                dtp_timetable.Value = vMixEvents[0].EventStart;
                dtp_endtime.Text = vMixEvents[0].EventEnd.ToString("yyyy/MM/dd HH:mm:ss");
            }
            rebuildhasoccur = false;
            MessageBox.Show(eventcount.ToString() + " events loaded from xml.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openedfile = filename;
            Text = formtitle + "(" + Path.GetFileName(filename) + ")";
            updaterecentfiles(filename);
            updaterecentfilestoolstripmenu(filename);
        }

        private void updaterecentfiles(string filepath)
        {
            if (recentfiles.Contains(filepath)) recentfiles.Remove(filepath);
            if (recentfiles.Count == maxrecents + 1) recentfiles.RemoveAt(0);
            recentfiles.Add(filepath);
            settings.SetValue("vManager", "recentfiles", ConcateArrayOfString(recentfiles));
            settings.Save();
        }

        private string ConcateArrayOfString(List<string> toconcat)
        {
            if (toconcat.Count == 0) return "";
            string result = toconcat[0];
            if (toconcat.Count == 1) return result;
            if (toconcat.Count > 2)
            {
                for (int i = 1; i < toconcat.Count - 1; i++)
                { result = result + "|" + toconcat[i]; }
            }
            return result + "|" + toconcat[toconcat.Count - 1];
        }

        private void append(bool[] overlay)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML-Files|*.xml|all Files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<vMixEvent> vmes = new List<vMixEvent>();
                XmlDocument d = new XmlDocument();
                d.Load(ofd.FileName);
                vMixEvent k;
                foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                {
                    k = new vMixEvent(n);
                    if (fixonload) fixpath(out k, k);
                    if (overlay[int.Parse(k.Overlay)])
                        vmes.Add(new vMixEvent(k));
                }
                if (vmes.Count > 0)
                {
                    vmes.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                    donotredraw = true;
                    vMixEvents.AddRange(vmes);
                    EventList.VirtualListSize = vMixEvents.Count;
                    RebuildTimetable();
                    UpdateDisplay();
                    donotredraw = false;
                    MessageBox.Show(vmes.Count.ToString() + " events appended from xml.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void bn_append_Click(object sender, EventArgs e)
        {
            vOverlaySelection vOver = new vOverlaySelection(ActiveOverlay, "Add selected\r\nto displayed", true, true);
            vOver.SelectedOverlayValue = new SelectedOverlay(append);
            vOver.ShowDialog();
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "XML-Files|*.xml|all Files|*.*";
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    List<vMixEvent> vmes = new List<vMixEvent>();
            //    XmlDocument d = new XmlDocument();
            //    d.Load(ofd.FileName);
            //    foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
            //        vmes.Add(new vMixEvent(n));
            //    if (vmes.Count > 0)
            //    {
            //        vmes.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
            //        donotredraw = true;
            //        vMixEvents.AddRange(vmes);
            //        EventList.VirtualListSize = vMixEvents .Count;
            //        RebuildTimetable();
            //        UpdateDisplay();
            //        donotredraw = false;
            //        MessageBox.Show(vmes.Count.ToString() + " events appended from xml.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
        }

        private void bn_add_input_Click(object sender, EventArgs e)
        {
            int position;
            if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
            if (add_input()) 
            {
                ActiveEvent = vMixEvents[position];
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(position);
                UpdateDisplay();
            }
        }

        private bool add_input()
        {
            vMixEvent new_event = new vMixEvent(vmEventType.input, dtp_timetable.Value, new TimeSpan(0, 1, 0));
            if (new_event != null)
            {
                new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                new_event.EventTransitionTime = (int)ud_transition_time.Value;
                new_event.Overlay = ActiveOverlay;
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                vMixEvents.Insert(position, new_event);
                EventList.VirtualListSize = vMixEvents.Count;
                RebuildTimetable();
                return true;
            }
            else return false;
        }

        private void bn_add_black_Click(object sender, EventArgs e)
        {
            ColorDialog colorpicker = new ColorDialog();
            if (colorpicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string color = ColorToString(colorpicker.Color);
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                if (add_black(color))
                {
                    ActiveEvent = vMixEvents[position];
                    EventList.SelectedIndices.Clear();
                    EventList.SelectedIndices.Add(position);
                    UpdateDisplay();
                }
            }   
        }

        private string ColorToString(Color color)
        {
            string result;
            if (color.IsNamedColor) result = color.Name;
            else result = color.A.ToString() + ";" 
                + color.R.ToString() + ";" 
                + color.G.ToString() + ";" 
                + color.B.ToString();
            return result;
        }

        private Color ColorFromString(string color)
        {
            Color result;
            result = Color.FromName(color);
            if (!result.IsKnownColor)
            {
                result = Color.Empty;
                string[] argb = color.Split(';');
                if (argb.Length != 4) return result;
                result = Color.FromArgb
                    (
                    Convert.ToInt32(argb[0]), 
                    Convert.ToInt32(argb[1]), 
                    Convert.ToInt32(argb[2]), 
                    Convert.ToInt32(argb[3])
                    );

            }
            return result;
        }

        private bool add_black(string color)
        {
            vMixEvent new_event = new vMixEvent(vmEventType.black, dtp_timetable.Value, new TimeSpan(0, 0, 10));
            if (new_event != null)
            {
                new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                new_event.EventTransitionTime = (int)ud_transition_time.Value;
                new_event.EventPath = color;
                new_event.Overlay = ActiveOverlay;
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                vMixEvents.Insert(position, new_event);
                EventList.VirtualListSize = vMixEvents.Count;
                RebuildTimetable();
                return true;
            }
            else return false;
        }

        private void bn_add_manual_Click(object sender, EventArgs e)
        {
            int position;
            if (ActiveEvent != null)
                position = vMixEvents.IndexOf(ActiveEvent) + 1;
            else
                position = vMixEvents.Count;
            if (add_manual())
            {
                ActiveEvent = vMixEvents[position];
                EventList.SelectedIndices.Clear();
                EventList.SelectedIndices.Add(position);
                UpdateDisplay();
            }
        }

        private bool add_manual()
        {
            vMixEvent new_event = new vMixEvent(vmEventType.manual, dtp_timetable.Value, new TimeSpan(1, 0, 0));
            if (new_event != null)
            {
                new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                new_event.EventTransitionTime = (int)ud_transition_time.Value;
                new_event.Overlay = ActiveOverlay;
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                vMixEvents.Insert(position, new_event);
                EventList.VirtualListSize = vMixEvents.Count;
                RebuildTimetable();
                return true;
            }
            else return false;
        }

        private void bn_add_video_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent);
                else
                    position = vMixEvents.Count - 1;
                int n = add_video(ofd.FileNames);
                ActiveEvent = vMixEvents[position + n];
                EventList.SelectedIndices.Clear();
                for (int i = 1 ; i < n; i++) EventList.SelectedIndices.Add(position + i);
                EventList.SelectedIndices.Add(position + n);
                UpdateDisplay();
            }
        }

        private int add_video(string [] Filenames)
        {
            int position;
            int n = 0;
            if (ActiveEvent != null)
                position = vMixEvents.IndexOf(ActiveEvent);
            else
                position = vMixEvents.Count - 1;
            foreach (string file in Filenames)
                {
                    vMixEvent new_event = ParseVideoData(file);
                    if (new_event != null)
                    {
                        new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                        new_event.EventTransitionTime = (int)ud_transition_time.Value;
                        new_event.Overlay = ActiveOverlay;
                        if (ActiveOverlay != "0") new_event.EventMuted = true;
                        position++;
                        vMixEvents.Insert(position, new_event);
                        n++;
                        //EventList.SelectedIndices.Clear();
                        //EventList.SelectedIndices.Add(position);
                    }
                }
            EventList.VirtualListSize = vMixEvents.Count;
            RebuildTimetable();
            return n;
        }

        private vMixEvent ParseVideoData(string path)
        {
            vMixEvent new_event = null;
            string infotext = path;
            FileInfo.Open(path);

            string result = FileInfo.Get(StreamKind.General, 0, "Video_Format_List");
            if (result != "")
            {
                infotext += "\r\nVideo: " + result;
                result = FileInfo.Get(StreamKind.General, 0, "Audio_Format_List");
                if (result != "") infotext += "\r\nAudio: " + result;

                double milliseconds = -1;
                TimeSpan duration = new TimeSpan(0);
                result = FileInfo.Get(StreamKind.General, 0, "Duration");
                CultureInfo cult = CultureInfo.CreateSpecificCulture("en-GB");
                if (result != "" && double.TryParse(result, NumberStyles.Float | NumberStyles.AllowDecimalPoint, cult,out milliseconds))
                {
                    duration = new TimeSpan(0, 0, 0, 0, (int) milliseconds);
                    infotext += "\r\nDuration: " + duration.ToString(@"hh\:mm\:ss");
                }
                else
                    MessageBox.Show("I can't decode this files duration!", "No Duration?", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                new_event = new vMixEvent(System.IO.Path.GetFileNameWithoutExtension(path),
                    path,
                    vmEventType.video,
                    dtp_timetable.Value,
                    new TimeSpan (0),
                    duration,
                    duration,
                    true,
                    vmTransitionType.cut,
                    1000,
                    false);
                new_event.EventInfoText = infotext;
            }
            else
                MessageBox.Show("I can't recognize the video format!", "No Video?", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FileInfo.Close();
            return new_event;
        }

        private void bn_add_image_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent);
                else
                    position = vMixEvents.Count - 1;
                int n = add_image(ofd.FileNames);
                ActiveEvent = vMixEvents[position + n];
                EventList.SelectedIndices.Clear();
                for (int i = 1; i < n; i++) EventList.SelectedIndices.Add(position + i);
                EventList.SelectedIndices.Add(position + n);
                UpdateDisplay();
            }
        }

        private int add_image(string[] Filenames)
        {
            int position;
            int n = 0;
            if (ActiveEvent != null)
                position = vMixEvents.IndexOf(ActiveEvent);
            else
                position = vMixEvents.Count - 1;
            foreach (string file in Filenames)
            {
                vMixEvent new_event = ParseImageData(file);
                if (new_event != null)
                {
                    new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                    new_event.EventTransitionTime = (int)ud_transition_time.Value;
                    new_event.Overlay = ActiveOverlay;
                    if (ActiveOverlay != "0") new_event.EventMuted = true;
                    position++;
                    vMixEvents.Insert(position, new_event);
                    n++;
                    //EventList.SelectedIndices.Clear();
                    //EventList.SelectedIndices.Add(position);
                }
            }
            EventList.VirtualListSize = vMixEvents.Count;
            RebuildTimetable();
            return n;
        }

        private vMixEvent ParseImageData(string path)
        {
            vMixEvent new_event = null;
            string infotext = path;
            FileInfo.Open(path);

            string result = FileInfo.Get(StreamKind.General, 0, "Video_Format_List");
            if (result != "JPEG")
                result = FileInfo.Get(StreamKind.General, 0, "Image_Format_List");
            if (result != "")
            {
                infotext += "\r\nImage: " + result;
                TimeSpan duration = new TimeSpan(0,0,10);
                new_event = new vMixEvent(System.IO.Path.GetFileNameWithoutExtension(path),
                    path,
                    vmEventType.image,
                    dtp_timetable.Value,
                    new TimeSpan(0),
                    duration,
                    duration,
                    false,
                    vmTransitionType.cut,
                    500,
                    false);
                new_event.EventInfoText = infotext;
            }
            else
                MessageBox.Show("I can't recognize the image format!", "No Image?", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FileInfo.Close();
            return new_event;
        }

        private void bn_add_audio_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent);
                else
                    position = vMixEvents.Count - 1;
                int n = add_audio(ofd.FileNames);
                ActiveEvent = vMixEvents[position + n];
                EventList.SelectedIndices.Clear();
                for (int i = 1; i < n; i++) EventList.SelectedIndices.Add(position + i);
                EventList.SelectedIndices.Add(position + n);
                UpdateDisplay();
            }
        }

        private int add_audio(string[] Filenames)
        {
            int position;
            int n = 0;
            if (ActiveEvent != null)
                position = vMixEvents.IndexOf(ActiveEvent);
            else
                position = vMixEvents.Count - 1;
            foreach (string file in Filenames)
            {
                vMixEvent new_event = ParseAudioData(file);
                if (new_event != null)
                {
                    new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                    new_event.EventTransitionTime = (int)ud_transition_time.Value;
                    new_event.Overlay = ActiveOverlay;
                    new_event.EventMuted = false;
                    position++;
                    vMixEvents.Insert(position, new_event);
                    n++;
                    //EventList.SelectedIndices.Clear();
                    //EventList.SelectedIndices.Add(position);
                }
            }
            EventList.VirtualListSize = vMixEvents.Count;
            RebuildTimetable();
            return n;
        }

        private vMixEvent ParseAudioData(string path)
        {
            vMixEvent new_event = null;
            string infotext = path;
            FileInfo.Open(path);

            string result = FileInfo.Get(StreamKind.General, 0, "Audio_Format_List");
            if (result != "")
            {
                infotext += "\r\nAudio: " + result;

                double milliseconds = -1;
                TimeSpan duration = new TimeSpan(0);
                result = FileInfo.Get(StreamKind.General, 0, "Duration");
                CultureInfo cult = CultureInfo.CreateSpecificCulture("en-GB");
                if (result != "" && double.TryParse(result, NumberStyles.Float | NumberStyles.AllowDecimalPoint, cult, out milliseconds))
                {
                    duration = new TimeSpan(0, 0, 0, 0, (int)milliseconds);
                    infotext += "\r\nDuration: " + duration.ToString(@"hh\:mm\:ss");
                }
                else
                    MessageBox.Show("I can't decode this files duration!", "No Duration?", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                new_event = new vMixEvent(System.IO.Path.GetFileName(path),
                    path,
                    vmEventType.audio,
                    dtp_timetable.Value,
                    new TimeSpan(0),
                    duration,
                    duration,
                    true,
                    vmTransitionType.cut,
                    500,
                    false);
                new_event.EventInfoText = infotext;
            }
            else
                MessageBox.Show("I can't recognize the audio format!", "No Audio?", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FileInfo.Close();
            return new_event;
        }

        //private void bn_add_photos_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog fbd = new FolderBrowserDialog();
        //    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        string path = fbd.SelectedPath;
        //        TimeSpan ts = new TimeSpan(0, 3, 0);
        //        vMixEvent new_event = new vMixEvent(System.IO.Path.GetFileName (path),
        //            path,
        //            vmEventType.photos,
        //            dtp_timetable.Value,
        //            new TimeSpan(0),
        //            ts,
        //            ts,
        //            false,
        //            vmTransitionType.fade,
        //            500,
        //            false);

        //        if (new_event != null)
        //        {
        //            new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
        //            new_event.EventTransitionTime = (int)ud_transition_time.Value;
        //            new_event.SlideshowInterval = (int)ud_slideshow_interval.Value;
        //            new_event.SlideshowTransition = new_event.TransitionTypeFromString(lb_slideshow_transition.Text);
        //            new_event.SlideshowTransitionTime = (int)ud_slideshow_transition.Value;
        //            new_event.EventInfoText = "slideshow";
        //            new_event.Overlay = ActiveOverlay;

        //            int position;
        //            if (ActiveEvent != null)
        //                position = vMixEvents.IndexOf(ActiveEvent) + 1;
        //            else
        //                position = vMixEvents.Count;
        //            vMixEvents.Insert(position, new_event);
        //            ActiveEvent = new_event;
        //            EventList.VirtualListSize = vMixEvents.Count;
        //            EventList.SelectedIndices.Clear();
        //            RebuildTimetable();
        //            EventList.SelectedIndices.Add(position);
        //            UpdateDisplay();
        //        }
        //    }
        //}

        private void bn_add_photos_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                if (add_photos(fbd.SelectedPath))
                {
                    ActiveEvent = vMixEvents[position];
                    EventList.SelectedIndices.Clear();
                    EventList.SelectedIndices.Add(position);
                    UpdateDisplay();
                }
            }
        }

        private bool add_photos(string path)
        {
            TimeSpan ts = new TimeSpan(0, 5, 0);
            vMixEvent new_event = new vMixEvent(System.IO.Path.GetFileName(path),
                path,
                vmEventType.photos,
                dtp_timetable.Value,
                new TimeSpan(0),
                ts,
                ts,
                false,
                vmTransitionType.fade,
                500,
                false);

            if (new_event != null)
            {
                new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                new_event.EventTransitionTime = (int)ud_transition_time.Value;
                new_event.SlideshowInterval = (int)ud_slideshow_interval.Value;
                new_event.SlideshowTransition = new_event.TransitionTypeFromString(lb_slideshow_transition.Text);
                new_event.SlideshowTransitionTime = (int)ud_slideshow_transition.Value;
                new_event.EventInfoText = "slideshow";
                new_event.Overlay = ActiveOverlay;

                int position;
                if (ActiveEvent != null)
                    position = vMixEvents.IndexOf(ActiveEvent) + 1;
                else
                    position = vMixEvents.Count;
                vMixEvents.Insert(position, new_event);
                EventList.VirtualListSize = vMixEvents.Count;
                RebuildTimetable();
                return true;
            }
            else return false;
        }

        private void bn_schedule_Click(object sender, EventArgs e)
        {
            vOverlaySelection vOver = new vOverlaySelection(ActiveOverlay, "Wich layers\nto Schedule", true, true);
            vOver.SelectedOverlayValue = new SelectedOverlay(SchedulePlaylist);
            vOver.ShowDialog();
        }

        private void SchedulePlaylist(bool[] Overlay)
        {
            List<vMixEvent> tempvMixEvents;
            ListView tempEventList;
            tempEventList = EventList; //backup active list box of event
            tempvMixEvents = vMixEvents; //backup active list of event

            for (int i = 0; i < Overlay.Length; i++)
            {
                if (ListOfvMixEvents[i].Equals(null)) 
                    continue;
                if (Overlay[i])
                {
                    EventList = ListOfEventList[i];
                    vMixEvents = ListOfvMixEvents[i];

                    if (vMixEvents.Count == 0)
                        continue;

                    DateTime start = vMixEvents[0].EventStart;
                    DateTime end = vMixEvents[vMixEvents.Count - 1].EventEnd;

                    string datafolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";
                    if (!System.IO.Directory.Exists(datafolder))
                        System.IO.Directory.CreateDirectory(datafolder);

                    string schedulename = datafolder + "\\vSchedule"+ Convert.ToString(i) +".xml";

                    List<vMixEvent> vmes = new List<vMixEvent>();
                    XmlDocument d = new XmlDocument();

                    if (System.IO.File.Exists(schedulename))
                    {
                        d.Load(schedulename);
                        foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                        {
                            vMixEvent vme = new vMixEvent(n);
                            if (vme.EventStart > DateTime.Now && (vme.EventStart < start || vme.EventStart > end))
                                vmes.Add(vme);
                        }
                        d = new XmlDocument();
                    }
                    vmes.AddRange(vMixEvents);
                    vmes.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });

                    XmlNode root = d.CreateElement("vMixManager");
                    d.AppendChild(root);
                    XmlNode events = d.CreateElement("Events");
                    root.AppendChild(events);
                    foreach (vMixEvent vme in vmes)
                        events.AppendChild(vme.ToXMLNode(d));
                    d.Save(schedulename);

                }
            }
            EventList = tempEventList; //restore
            vMixEvents = tempvMixEvents; //restore
            MessageBox.Show("Events scheduled.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private bool PullPlaylist()
        {
            ActiveEvent = null;
            bool result = false;
            ClearPlaylist(new bool[] {true,true,true,true,true});
            openedfile = defaultfilename;
            this.Text = formtitle + "(" + openedfile + ")";
            string datafolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";

            List<vMixEvent>[] ListOfvmes = new List<vMixEvent>[5];
            for (int i = 0; i < 5; i++)
            {
                string schedulename = datafolder + "\\vSchedule" + Convert.ToString(i) + ".xml";

                List<vMixEvent> vmes = new List<vMixEvent>();
                XmlDocument d = new XmlDocument();
                if (System.IO.File.Exists(schedulename))
                {
                    d.Load(schedulename);
                    foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                    {
                        vMixEvent vme = new vMixEvent(n);
                        if (vme.EventEnd > DateTime.Now)
                        {
                            if (fixonload) fixpath(out vme, vme);
                            vmes.Add(vme);
                        }
                    }
                    d = new XmlDocument();
                    result = true;
                }
                ListOfvmes[i] = vmes;
            }
                
            donotredraw = true;
            ListView tempEventList = EventList;
            List<vMixEvent> tempvMixEvents = vMixEvents;
            string tempActiveOverlay = ActiveOverlay;
            for (int i = 0; i < 5; i++)
            {
                int count = ListOfvmes[i].Count;
                if (count > 0)
                {
                    vMixEvents = ListOfvMixEvents[i];
                    vMixEvents.AddRange(ListOfvmes[i]);
                    vMixEvents.Sort(delegate(vMixEvent e1, vMixEvent e2) { return e1.EventStart.CompareTo(e2.EventStart); });
                    ActiveOverlay = Convert.ToString(i);
                    EventList = ListOfEventList[i];
                    EventList.SelectedIndices.Clear();
                    EventList.VirtualListSize = count;
                    dtp_timetable.Value = vMixEvents[0].EventStart;
                    RebuildTimetable();
                    //UpdateDisplay();
                }
            }
            donotredraw = false;
            EventList = tempEventList;
            vMixEvents = tempvMixEvents;
            ActiveOverlay = tempActiveOverlay;
            if (vMixEvents.Count > 0)
            {
                dtp_timetable.Value = vMixEvents[0].EventStart;
                dtp_endtime.Text = vMixEvents[0].EventEnd.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return result;
        }

        private void bn_erase_schedule_Click(object sender, EventArgs e)
        {
            vOverlaySelection vOver = new vOverlaySelection(ActiveOverlay, "Wich layers\nto erase?", true, true);
            vOver.SelectedOverlayValue = new SelectedOverlay(Erase_SchedulePlaylist);
            vOver.ShowDialog();
        }

        private void Erase_SchedulePlaylist(bool[] Overlay) 
        {
            string datafolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler";
            if (MessageBox.Show("This will erase ALL currently scheduled Events,\r\nincluding the ones scheduled earlier;\r\nrunning events will be terminated.\r\n\r\nAre you sure?", "Beware!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0; i < Overlay.Length; i++)
                {
                    if (Overlay[i])
                    {
                        string schedulename = datafolder + "\\vSchedule" + Convert.ToString(i) + ".xml";
                        if (System.IO.File.Exists(schedulename))
                            System.IO.File.Delete(schedulename);
                    }
                }
            }
        }

        private void bn_view_text_Click(object sender, EventArgs e)
        {
            vMixTextviewer vmt = new vMixTextviewer();
            vmt.ViewText(vMixEvents);
        }

        private void bn_view_html_Click(object sender, EventArgs e)
        {
            vMixTextviewer vmt = new vMixTextviewer();
            vmt.ViewHTML(vMixEvents);
        }

        private void bn_view_bbcode_Click(object sender, EventArgs e)
        {
            vMixTextviewer vmt = new vMixTextviewer();
            vmt.ViewBBCode(vMixEvents);
        }

        private void bn_now_Click(object sender, EventArgs e)
        {
            dtp_timetable.Focus();
            dtp_timetable.Value = DateTime.Now + new TimeSpan(0, 0, 10);
        }

        private void bn_settime_0_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date;
        }

        private void bn_settime_4_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date + new TimeSpan(4,0,0);
        }

        private void bn_settime_8_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date + new TimeSpan(8, 0, 0);
        }

        private void bn_settime_12_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date + new TimeSpan(12, 0, 0);
        }

        private void bn_settime_16_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date + new TimeSpan(16, 0, 0);
        }

        private void bn_settime_20_Click(object sender, EventArgs e)
        {
            dtp_timetable.Value = dtp_timetable.Value.Date + new TimeSpan(20, 0, 0);
        }

        private void FixInPointAndDuration()
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                if (ActiveEvent.EventInPoint > ActiveEvent.MediaDuration)
                    ActiveEvent.EventInPoint = ActiveEvent.MediaDuration;
                if (ActiveEvent.EventInPoint + ActiveEvent.EventDuration > ActiveEvent.MediaDuration)
                    ActiveEvent.EventDuration = ActiveEvent.MediaDuration - ActiveEvent.EventInPoint;
                RebuildTimetable();
                UpdateDisplay();
            }
        }

        private void bn_ip_zero_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventInPoint = new TimeSpan(0);
                FixInPointAndDuration();
            }
        }

        private void bn_ip_50_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventInPoint += new TimeSpan(ActiveEvent.MediaDuration.Ticks / 2);
                FixInPointAndDuration();
            }
        }

        private void bn_ip_33_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventInPoint += new TimeSpan(ActiveEvent.MediaDuration.Ticks / 3);
                FixInPointAndDuration();
            }

        }

        private void bn_ip_25_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventInPoint += new TimeSpan(ActiveEvent.MediaDuration.Ticks / 4);
                FixInPointAndDuration();
            }

        }

        private void bn_dr_100_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventDuration = ActiveEvent.MediaDuration;
                FixInPointAndDuration();
            }
        }

        private void bn_dr_50_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventDuration = new TimeSpan(ActiveEvent.MediaDuration.Ticks/2);
                FixInPointAndDuration();
            }
        }

        private void bn_dr_33_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventDuration = new TimeSpan(ActiveEvent.MediaDuration.Ticks / 3);
                FixInPointAndDuration();
            }
        }

        private void bn_dr_25_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null && ActiveEvent.HasDuration)
            {
                ActiveEvent.EventDuration = new TimeSpan(ActiveEvent.MediaDuration.Ticks / 4);
                FixInPointAndDuration();
            }
        }

        private void lb_overlay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (donotredraw) return;
            string ovrl = Convert.ToString(tabControl1.SelectedTab.Name[7]);
            ovrl = Convert.ToString(tabControl1.SelectedIndex);
            if (ActiveOverlay == ovrl)
                return;
            else
            {
                //EventList0.Visible = (ovrl == "0");
                //EventList1.Visible = (ovrl == "1");
                //EventList2.Visible = (ovrl == "2");
                //EventList3.Visible = (ovrl == "3");
                //EventList4.Visible = (ovrl == "4");
                ActiveOverlay = ovrl;
                switch (ovrl) 
                {
                    case "0":
                        EventList = EventList0;
                        vMixEvents = vMixEvents0;
                        break;
                    case "1":
                        EventList = EventList1;
                        vMixEvents = vMixEvents1;
                        break;
                    case "2":
                        EventList = EventList2;
                        vMixEvents = vMixEvents2;
                        break;
                    case "3":
                        EventList = EventList3;
                        vMixEvents = vMixEvents3;
                        break;
                    case "4":
                        EventList = EventList4;
                        vMixEvents = vMixEvents4;
                        break;
                }
            }
            if (vMixEvents.Count > 0)
            {
                dtp_timetable.Value = vMixEvents[0].EventStart;
                dtp_endtime.Text = vMixEvents[vMixEvents.Count - 1].EventEnd.ToString("yyyy/MM/dd HH:mm:ss");
            }
            EventList_SelectedIndexChanged(sender,e);
        }

        private void bn_sync_Click(object sender, EventArgs e)
        {
            vOverlaySelection vOver = new vOverlaySelection(ActiveOverlay, "Wich layers\rto synchrone:");
            vOver.SelectedOverlayValue = new SelectedOverlay(this.SyncOverlay);
            vOver.ShowDialog();
        }

        private void SyncOverlay(bool[] Overlay) 
        {
            List<vMixEvent> tempvMixEvents;
            System.Windows.Forms.ListView tempEventList;
            tempEventList = EventList; //backup active list box of event
            tempvMixEvents = vMixEvents; //backup active list of event
            for (int i = 0; i < 5; i++)
            {
                if (Overlay[i] && ActiveOverlay != Convert.ToString(i))
                {
                    EventList = ListOfEventList[i];
                    vMixEvents = ListOfvMixEvents[i];
                    RebuildTimetable();
                }
            }
            EventList = tempEventList;
            vMixEvents = tempvMixEvents;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rebuildhasoccur)
            {
                DialogResult result = MessageBox.Show("Do you want to save before creating an new playlist?", "You are about to create a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    if (!AutoSave(sender, e)) return ;
                if (result == DialogResult.Cancel) { return; }
            }
            bool[] a = { true, true, true, true, true };
            ClearPlaylist(a);
            Text = formtitle + "(" + defaultfilename + ")";
            openedfile = defaultfilename;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void spliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpliceEvent();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender, e);
            bn_remove_Click(sender,e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            donotredraw = true;
            if (copybuffer.Count > 0) copybuffer.Clear();
            copybuffer.Capacity = EventList.SelectedIndices.Count;
            int i = 0;
            foreach (int l in EventList.SelectedIndices) 
            {
                copybuffer.Insert(i,vMixEvents[l]);

                i++;
            }
            donotredraw = false;
            pasteToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem1.Enabled = true;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveEvent == null) return;
            int position, initial;
            initial = EventList.SelectedIndices[EventList.SelectedIndices.Count -1];
            position = initial + 1;
            donotredraw = true;
            if (copybuffer.Count == 0) return;
            foreach (vMixEvent v in copybuffer)
            {
                vMixEvents.Insert(position, v);
                position++;
            }
            EventList.VirtualListSize = vMixEvents.Count;
            RebuildTimetable();
            donotredraw = false;
            EventList.SelectedIndices.Clear();
            for (int i = initial + 1; i <= initial + copybuffer.Count; i++)
                EventList.SelectedIndices.Add(i);
            
            UpdateDisplay();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finder.ShowDialog();
            if (vMixEvents.Count > 0)
            {
                if (finder.findtype == 0) find(0, vMixEvents.Count, true, finder.date);
                if (finder.findtype == 1) find(0, vMixEvents.Count, true, finder.title);
            }

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectall();
        }

        private void bn_donate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/donate?hosted_button_id=8KWHCKS3TX54S");
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_as();
        }

        private void vMixManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rebuildhasoccur == true)
            {
                DialogResult result = MessageBox.Show("Do you want to save before closing?", "You are abour to leave!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    if (!AutoSave(sender, e)) e.Cancel = true;
                if (result == DialogResult.Cancel) { e.Cancel = true; }
            }
            settings.Save();
        }

        private void eraseallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = recentfiles.Count - 1;
            for (int i = 0; i < n; i++ )
            {
                recentfiles.RemoveAt(n - i);
                recentfilestoolStripMenuItem.DropDownItems[i].Name = "";
                recentfilestoolStripMenuItem.DropDownItems[i].Text = "";
                recentfilestoolStripMenuItem.DropDownItems[i].Visible = false;
            }
            recentfilestoolStripMenuItem.Enabled = false;
            settings.SetValue("vManager", "recentfiles", ConcateArrayOfString(recentfiles));
            settings.Save();
        }

        private void recentfilestoolStripItem_Click(object sender, EventArgs e)
        {
            if (rebuildhasoccur == true)
            {
                DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes) 
                    if (!AutoSave(sender, e)) return;
                if (result == DialogResult.Cancel) return;
            }
            load(sender.ToString());
        }

        private void bn_add_Click(object sender, EventArgs e)
        {
            string s = lb_event.Text;
            switch (s)
            {
                case "Video":
                    bn_add_video_Click(sender, e);
                    break;
                case "Audio":
                    bn_add_audio_Click(sender, e);
                    break;
                case "Image":
                    bn_add_image_Click(sender, e);
                    break;
                case "Slideshow":
                    bn_add_photos_Click(sender, e);
                    break;
                case "Color":
                    bn_add_black_Click(sender, e);
                    break;
                case "Input":
                    bn_add_input_Click(sender, e);
                    break;
                case "Operator":
                    bn_add_manual_Click(sender, e);
                    break;
            }
        }

        private void bn_add_replace_Click(object sender, EventArgs e)
        {
            int position;
            if (ActiveEvent != null)
                position = vMixEvents.IndexOf(ActiveEvent) + 1 - EventList.SelectedIndices.Count;
            else
                position = vMixEvents.Count - EventList.SelectedIndices.Count;
            string s = lb_event.Text;
            OpenFileDialog ofd = new OpenFileDialog();
            switch (s)
            {
                case "Video":
                    ofd.Multiselect = true;
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        position--;
                        int n = add_video(ofd.FileNames);
                        if (n > 0) 
                        {
                            bn_remove_Click(sender, e);
                            ActiveEvent = vMixEvents[position + n];
                            EventList.SelectedIndices.Clear();
                            for (int i = 1; i < n; i++) EventList.SelectedIndices.Add(position + i);
                            EventList.SelectedIndices.Add(position + n);
                            UpdateDisplay();
                            return;
                        }
                    }
                    break;
                case "Audio":
                    ofd.Multiselect = true;
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        position--;
                        int n = add_audio(ofd.FileNames);
                        if (n > 0) 
                        {
                            bn_remove_Click(sender, e);
                            ActiveEvent = vMixEvents[position + n];
                            EventList.SelectedIndices.Clear();
                            for (int i = 1; i < n; i++) EventList.SelectedIndices.Add(position + i);
                            EventList.SelectedIndices.Add(position + n);
                            UpdateDisplay();
                            return;
                        }
                    }
                    break;
                case "Image":
                    ofd.Multiselect = true;
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        position--;
                        int n = add_image(ofd.FileNames);
                        if (n > 0) 
                        {
                            bn_remove_Click(sender, e);
                            ActiveEvent = vMixEvents[position + n];
                            EventList.SelectedIndices.Clear();
                            for (int i = 1; i < n; i++) EventList.SelectedIndices.Add(position + i);
                            EventList.SelectedIndices.Add(position + n);
                            UpdateDisplay();
                            return;
                        }
                    }
                    break;
                case "Slideshow":
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (add_photos(fbd.SelectedPath))
                        {
                            bn_remove_Click(sender, e);
                            ActiveEvent = vMixEvents[position];
                            EventList.SelectedIndices.Clear();
                            EventList.SelectedIndices.Add(position);
                            UpdateDisplay();
                        }
                    }
                    break;
                case "Color":
                    ColorDialog colorpicker = new ColorDialog();
                    if (colorpicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string color = ColorToString(colorpicker.Color);
                        if (add_black(color))
                        {
                            bn_remove_Click(sender, e);
                            ActiveEvent = vMixEvents[position];
                            EventList.SelectedIndices.Clear();
                            EventList.SelectedIndices.Add(position);
                            UpdateDisplay();
                        }
                    }
                    break;
                case "Input":
                    if (add_input())
                    {
                        bn_remove_Click(sender, e);
                        ActiveEvent = vMixEvents[position];
                        EventList.SelectedIndices.Clear();
                        EventList.SelectedIndices.Add(position);
                        UpdateDisplay();
                    }
                    break;
                case "Operator":
                    if (add_manual())
                    {
                        bn_remove_Click(sender, e);
                        ActiveEvent = vMixEvents[position];
                        EventList.SelectedIndices.Clear();
                        EventList.SelectedIndices.Add(position);
                        UpdateDisplay();
                    }
                    break;
            }
        }

        private void bn_splice_Click(object sender, EventArgs e)
        {
            SpliceEvent();
        }

        private void shuffleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shuffle();
        }

        private void shuffleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            shuffle();
        }

        private void bn_shuffle_Click(object sender, EventArgs e)
        {
            shuffle();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            selectall();
        }

        private void findnextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = EventList.SelectedIndices[EventList.SelectedIndices.Count -1];
            if (i != vMixEvents.Count - 1)
            {
                if (finder.findtype == 0) find(i + 1, vMixEvents.Count - i - 1, true, finder.date);
                if (finder.findtype == 1) find(i + 1, vMixEvents.Count - i - 1, true, finder.title);
            }
        }

        private void findpreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = EventList.SelectedIndices[EventList.SelectedIndices.Count - 1];
            if (i != 0)
            {
                if (finder.findtype == 0) find(i - 1, i, false, finder.date);
                if (finder.findtype == 1) find(i - 1, i, false, finder.title);
            }
        }

        private void bn_pull_Click(object sender, EventArgs e)
        {
            if (rebuildhasoccur)
            {
                DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    if (!AutoSave(sender, e)) return;
                if (result == DialogResult.Cancel) return;
            }
            if (PullPlaylist()) MessageBox.Show("Events Pull Completed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Nothing to Pull!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rebuildhasoccur)
            {
                DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    if (!AutoSave(sender, e)) return;
                if (result == DialogResult.Cancel) return;
            }
            if (PullPlaylist()) MessageBox.Show("Events Pull Completed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Nothing to Pull!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int fixpath(out vMixEvent Event, vMixEvent evnt)
        {
            //Event is the same as evnt plus file path fixed. must reference the same vMixEvent from the vMixEvent list
            int result = 0;
            if (evnt != null)
            {
                string filename = ""; //file to find
                string newpath = "";
                List<string> folders = library.folders;//collection of folders
                List<string> contentfile = library.files; //collection of file in folders

                filename = Path.GetFileName(evnt.EventPath);
                if (evnt.MediaType >= 2)
                {
                    if (!File.Exists(evnt.EventPath))
                    {
                        result = -1;
                        newpath = contentfile.Find(delegate(string e1) { return File.Exists(e1 + "\\" + filename); });
                        if (newpath != null && newpath != "") 
                        { 
                            evnt.EventPath = newpath + "\\" + filename; 
                            result = 1; 
                        }
                    }
                }
                else if (evnt.MediaType == 1)
                {
                    if (!Directory.Exists(evnt.EventPath))
                    {
                        result = -1;
                        newpath = contentfile.Find(delegate(string e1) { return Directory.Exists(e1 + "\\" + filename); });
                        if (newpath != null && newpath != "")
                        {
                            evnt.EventPath = newpath + "\\" + filename;
                            result = 1;
                        }
                    }
                }
                if (evnt.EventType == vmEventType.black)
                {
                    if (ColorFromString(filename).IsEmpty) evnt.EventPath = "Black";
                }
            }
            Event = evnt;
            return result;
        }

        private void libraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            library.fixpath = fixpath;
            library.ShowDialog();
            settings.Save();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            library.bn_update_Click(sender, e);
        }

        private void fixPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vMixEvent evnt;
            foreach (int i in EventList.SelectedIndices)
            {
                evnt = vMixEvents[i];
                if (fixpath(out evnt, evnt) == 1)
                {
                    vMixEvents[i] = evnt;
                    EventList.RedrawItems(i, i, false);
                }
            }
        }

        private void fixAllPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vMixEvent evnt;
            List<vMixEvent> v;
            for (int j = 0; j < ListOfvMixEvents.Length; j++)
            {
                v = ListOfvMixEvents[j];
                for (int i = 0; i < v.Count; i++)
                {
                    evnt = v[i];
                    if (fixpath(out evnt, evnt) == 1)
                    {
                        v[i] = evnt;
                        ListOfEventList[j].RedrawItems(i, i, false);
                    }
                }
            }
        }

        private void autoFixOnLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fixonload = autoFixOnLoadToolStripMenuItem.Checked;
            settings.SetValue("vManager", "fixonload", fixonload);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new vAbout().ShowDialog();
        }

        private void openfilelocationTollStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveEvent != null)
            {
                string path = "";
                try
                {
                    path = Path.GetDirectoryName(ActiveEvent.EventPath);
                }
                catch { }
                if (Directory.Exists(path)) System.Diagnostics.Process.Start(path);
            }
        }
    }
}