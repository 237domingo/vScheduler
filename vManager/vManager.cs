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
        bool donotredraw = false;
        //
        //settings for vManager
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler\\settings.xml";
        private List<string> recentfiles = new List<string>();
        int maxrecents = 5;
        private Xml settings;

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
            settings = new Xml();
            settings.LoadXml(SettingsPath);
            recentfiles.AddRange(settings.GetValue("vManager", "recentfiles", "...").Split('|'));
            maxrecents = settings.GetValue("vManager", "maxrecents", 5);
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
            foreach (string s in recentfiles) updaterecentfilestoolstripmenu(s);
            //openToolStripMenuItem.DropDownItems.Add()
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
            return lvi;
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
                if (ActiveEvent.EventLooping)
                    rb_looping.Checked = true;
                else
                    rb_toblack.Checked = true;
                if (ActiveEvent.HasDuration)
                {
                    cb_keep_duration.Checked = ActiveEvent.KeepDuration;
                    cb_keep_duration.Enabled = true;
                }
                else
                {
                    cb_keep_duration.Checked = false;
                    cb_keep_duration.Enabled = false;
                }
                dtp_duration.Enabled = !ActiveEvent.KeepDuration;
                dtp_inpoint.Enabled = ActiveEvent.HasDuration && !ActiveEvent.KeepDuration;
                lb_transition.Text = ActiveEvent.TransitionTypeString();
                ud_transition_time.Enabled = (ActiveEvent.EventTransition != vmTransitionType.cut);
                ud_transition_time.Value = ActiveEvent.EventTransitionTime;
                rtb_fileinfo.Text = ActiveEvent.EventInfoText;

                if (ActiveEvent.EventType == vmEventType.photos)
                {
                    pnl_slideshow.Visible = true;
                    ud_slideshow_interval.Value = ActiveEvent.SlideshowInterval;
                    lb_slideshow_transition.Text = ActiveEvent.SlideshowTypeString();
                    ud_slideshow_transition.Value = ActiveEvent.SlideshowTransitionTime;
                }
                else
                    pnl_slideshow.Visible = false;

                if (ActiveEvent.EventType == vmEventType.video)
                {
                    cb_audio.Enabled = true;
                }
                else
                    cb_audio.Enabled = false;
            }
            else 
            {
                tb_title.Text = "";
                dtp_start.Text = "00:00:00";
                dtp_inpoint.Text = "00:00:00";
                dtp_duration.Text = "00:00:00";
                dtp_end.Text = "00:00:00";
                rb_toblack.Checked = false;
                rb_looping.Checked = false;
                cb_keep_duration.Checked = false;
                lb_transition.SelectedIndex = 0;
                ud_transition_time.Value = 1000;
                rtb_fileinfo.Text = "";
                pnl_slideshow.Visible = false;
                cb_audio.Checked = false; 
                EventDetails.Enabled = false;
            }
            donotredraw = false;
        }

        private void EventList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveEvent = null;
            if (EventList.SelectedIndices.Count == 0) return;
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
                int lastindex = EventList.SelectedIndices[0];
                int previousindex = EventList.SelectedIndices[1];
                if (lastindex < previousindex) ActiveEvent = vMixEvents[lastindex];
                else ActiveEvent = vMixEvents[previousindex];
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
                ActiveEvent.EventLooping = rb_looping.Checked;
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
                //dtp_end.Enabled = false;
                rb_looping.Enabled = false;
                rb_toblack.Enabled = false;
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
        }

        private void SpliceEvent()
        {
            //vMixEvent v = vMixEvents.Find(delegate(vMixEvent e) { return e.EventEnd > timetofind;});
            if (ActiveEvent == null) return;
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

        private void dtp_timetable_ValueChanged(object sender, EventArgs e)
        {
            RebuildTimetable();
            UpdateDisplay();
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
            if (ActiveEvent != null)
                ActiveEvent.SlideshowInterval = (int)ud_slideshow_interval.Value;
        }
        private void lb_slideshow_transition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveEvent != null)
                ActiveEvent.SlideshowTransition = ActiveEvent.TransitionTypeFromString(lb_slideshow_transition.Text);
        }
        private void ud_slideshow_transition_ValueChanged(object sender, EventArgs e)
        {
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
            if (position >= vMixEvents.Count) position = vMixEvents.Count-1 ;
            if (position >= 0)
                EventList.SelectedIndices.Add(position);
            RebuildTimetable();
            donotredraw = false;
            UpdateDisplay();
        }

        private void bn_save_Click(object sender, EventArgs e)
        {
            if (openedfile == defaultfilename) save_as();
            else save(openedfile);
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
            Text = formtitle + " - " + Path.GetFileName(filename);
            updaterecentfiles(filename);
            updaterecentfilestoolstripmenu(filename);
        }

        private void save_as()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML-File|*.xml|all Files|*.*";
            sfd.FileName = dtp_timetable.Value.ToString("yyyy-MM-dd");
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save(sfd.FileName);
            }
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
            DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes) bn_save_Click(sender, e);
            if (result == DialogResult.No) { }
            else return;
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
            foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
            {
                try
                {
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)].Add(new vMixEvent(n));
                    eventcount++;
                }
                catch
                {
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)] = new List<vMixEvent>();
                    ListOfvMixEvents[int.Parse(n.Attributes.GetNamedItem("Overlay").Value)].Add(new vMixEvent(n));
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
            MessageBox.Show(eventcount.ToString() + " events loaded from xml.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openedfile = filename;
            Text = formtitle + " - " + Path.GetFileName(filename);
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
                foreach (XmlNode n in d.SelectNodes("//vMixManager//Events//Event"))
                    if ( overlay[Convert.ToInt32(n.Attributes.GetNamedItem("Overlay").Value)] )
                    vmes.Add(new vMixEvent(n));
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
                ActiveEvent = new_event;
                EventList.VirtualListSize = vMixEvents.Count;
                EventList.SelectedIndices.Clear();
                RebuildTimetable();
                EventList.SelectedIndices.Add(position);
                UpdateDisplay();
            }

        }

        private void bn_add_black_Click(object sender, EventArgs e)
        {
            vMixEvent new_event = new vMixEvent (vmEventType.black, dtp_timetable.Value,new TimeSpan (0,0,10));
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
                ActiveEvent = new_event;
                EventList.VirtualListSize = vMixEvents.Count;
                EventList.SelectedIndices.Clear();
                RebuildTimetable();
                EventList.SelectedIndices.Add(position);
                UpdateDisplay();
            }
        }

        private void bn_add_manual_Click(object sender, EventArgs e)
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
                ActiveEvent = new_event;
                EventList.VirtualListSize = vMixEvents.Count;
                EventList.SelectedIndices.Clear();
                RebuildTimetable();
                EventList.SelectedIndices.Add(position);
                UpdateDisplay();
            }
        }

        private void bn_add_video_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    vMixEvent new_event = ParseVideoData(file);
                    if (new_event != null)
                    {
                        new_event.EventTransition = new_event.TransitionTypeFromString(lb_transition.Text);
                        new_event.EventTransitionTime = (int)ud_transition_time.Value;
                        new_event.Overlay = ActiveOverlay;
                        if (ActiveOverlay != "0") new_event.EventMuted = true;
                        int position;
                        if (ActiveEvent != null)
                            position = vMixEvents.IndexOf(ActiveEvent) + 1;
                        else
                            position = vMixEvents.Count;
                        vMixEvents.Insert(position, new_event);
                        ActiveEvent = new_event;
                        EventList.VirtualListSize = vMixEvents.Count;
                        EventList.SelectedIndices.Clear();
                        RebuildTimetable();
                        EventList.SelectedIndices.Add(position);
                        UpdateDisplay();
                    }
                }
            }
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
                    true);
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
                foreach (string file in ofd.FileNames)
                {
                    vMixEvent new_event = ParseImageData(file);
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
                        ActiveEvent = new_event;
                        EventList.VirtualListSize = vMixEvents.Count;
                        EventList.SelectedIndices.Clear();
                        RebuildTimetable();
                        EventList.SelectedIndices.Add(position);
                        UpdateDisplay();
                    }
                }
            }
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
                    1000,
                    true);
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
                foreach (string file in ofd.FileNames)
                {
                    vMixEvent new_event = ParseAudioData(file);
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
                        ActiveEvent = new_event;
                        EventList.VirtualListSize = vMixEvents.Count;
                        EventList.SelectedIndices.Clear();
                        RebuildTimetable();
                        EventList.SelectedIndices.Add(position);
                        UpdateDisplay();
                    }
                }
            }
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

                new_event = new vMixEvent(System.IO.Path.GetFileNameWithoutExtension(path),
                    path,
                    vmEventType.audio,
                    dtp_timetable.Value,
                    new TimeSpan(0),
                    duration,
                    duration,
                    true,
                    vmTransitionType.cut,
                    1000,
                    true);
                new_event.EventInfoText = infotext;
            }
            else
                MessageBox.Show("I can't recognize the audio format!", "No Audio?", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FileInfo.Close();
            return new_event;
        }

        private void bn_add_photos_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                TimeSpan ts = new TimeSpan(0, 3, 0);
                vMixEvent new_event = new vMixEvent(System.IO.Path.GetFileName (path),
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
                    ActiveEvent = new_event;
                    EventList.VirtualListSize = vMixEvents.Count;
                    EventList.SelectedIndices.Clear();
                    RebuildTimetable();
                    EventList.SelectedIndices.Add(position);
                    UpdateDisplay();
                }
            }
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
            //EventList_SelectedIndexChanged(sender,e);
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
            DialogResult result = MessageBox.Show("Do you want to save before creating an new playlist?", "You are about to create a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                bn_save_Click(sender, e);
                bool[] a = { true, true, true, true, true };
                ClearPlaylist(a);
                Text = formtitle + " - " + defaultfilename;
                openedfile = defaultfilename;
            }
            if (result==DialogResult.No)
            {
                bool[] a = { true, true, true, true, true };
                ClearPlaylist(a);
                Text = formtitle + " - " + defaultfilename;
                openedfile = defaultfilename;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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
            int position = vMixEvents.IndexOf(ActiveEvent);
            donotredraw = true;
            if (copybuffer.Count == 0) return;

            EventList.SelectedIndices.Clear();
            EventList.VirtualListSize = vMixEvents.Count + copybuffer.Count;
            foreach (vMixEvent v in copybuffer)
            {
                vMixEvents.Insert(position, v);
            }
            for (int i = position; i < position + copybuffer.Count; i++) EventList.SelectedIndices.Add(i);
            RebuildTimetable();
            donotredraw = false;
            UpdateDisplay();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
            DialogResult result = MessageBox.Show("Do you want to save before closing?", "You are abour to leave!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                bn_save_Click(sender, e);
            }
            if (result == DialogResult.Cancel)
            {
                return;
            }
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
            DialogResult result = MessageBox.Show("Do you want to save before opening an new playlist?", "You are about to load a new playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes) bn_save_Click(sender, e);
            if (result == DialogResult.No) { }
            else return;
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
            bn_remove_Click(sender, e);
            bn_add_Click(sender, e);
        }

        private void bn_splice_Click(object sender, EventArgs e)
        {
            SpliceEvent();
        }

    }
}