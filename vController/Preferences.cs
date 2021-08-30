using System;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;

namespace vControler
{
    public partial class vPreferences : Form
    {
        private string _vMixIPadress = "127.0.0.1";
        public string vMixIP { get { return _vMixIPadress; } }

        private int _vMixPort = 8088;
        public int vMixPort { get { return _vMixPort; } }
        public string vMixURL { get { return "http://" + _vMixIPadress + ":" + _vMixPort.ToString(); } }

        private int _vMixPreload = 5;
        public int vMixPreload { get { return _vMixPreload; } }

        private int _vMixLinger = 5;
        public int vMixLinger { get { return _vMixLinger; } }

        private int _vMixLate = 10;
        public int vMixLate { get { return _vMixLate; } }

        private int _vMixRefresh = 10;
        public int vMixRefresh { get { return _vMixRefresh; } }

        private bool _vMixAutoLoad = false;
        public bool vMixAutoLoad { get { return _vMixAutoLoad; } set { _vMixAutoLoad = value; } }

        private bool _vMixAutoLoadV = false;
        public bool vMixAutoLoadV { get { return _vMixAutoLoadV; } }

        private bool _vMixAutoStart = false;
        public bool vMixAutoStart { get { return _vMixAutoStart; } }

        private bool _vMixStartMini = false;
        public bool vMixStartMini { get { return _vMixStartMini; } }

        private bool _vMixCloseTray = false;
        public bool vMixCloseTray { get { return _vMixCloseTray; } }

        private bool _vMixMiniTray = false;
        public bool vMixMiniTray { get { return _vMixMiniTray; } }

        private string _vMixPath ="";
        public string vMixPath { get { return _vMixPath; } }

        private bool _vMixForceExternal = false;
        public bool vMixForceExternal { get { return _vMixForceExternal; } }

        private bool _vMixForceRecording = false;
        public bool vMixForceRecording { get { return _vMixForceRecording; } }

        private bool _vMixForceStreaming = false;
        public bool vMixForceStreaming { get { return _vMixForceStreaming; } }

        private bool _vMixForceMulticorder = false;
        public bool vMixForceMulticorder { get { return _vMixForceMulticorder; } }

        private bool _vMixRunScript = false;
        public bool vMixRunScript { get { return _vMixRunScript; } }

        private string _vMixScript = "";
        public string vMixScript { get { return _vMixScript; } }

        string vControllerPath = Directory.GetCurrentDirectory();

        public string PreferencesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler\\Settings.xml";

        private Xml settings;
        
        public vPreferences()
        {
            InitializeComponent();
            this.tb_path.Cursor = Cursors.Hand;
            settings = new Xml();
            settings.LoadXml(PreferencesPath);
            ud_vMixPort.Value = _vMixPort = settings.GetValue("vMix", "vMixPort", 8088);
            tb_vMixIP.Text = _vMixIPadress = settings.GetValue("vMix", "vMixIPadress", "127.0.0.1");
            ud_preload.Value = _vMixPreload = settings.GetValue("vController", "MediaPreload", 5);
            ud_linger.Value = _vMixLinger = settings.GetValue("vController", "MediaLinger", 5);
            ud_late.Value = _vMixLate = settings.GetValue("vController", "Latency", 100);
            ud_refresh.Value = _vMixRefresh = settings.GetValue("vController", "vMixRefresh", 100);
            cb_autoload.Checked = _vMixAutoLoad = settings.GetValue("vController", "AutoLoad", false);
            cb_autoloadV.Checked = _vMixAutoLoadV = settings.GetValue("vController", "AutoLoadV", false);
            tb_path.Text = _vMixPath = settings.GetValue("vController", "vMixPath", "");
            cb_forceExternal.Checked = _vMixForceExternal = settings.GetValue("vController", "ForceExternal", false);
            cb_forceRecording.Checked = _vMixForceRecording = settings.GetValue("vController", "ForceRecording", false);
            cb_forceStreaming.Checked = _vMixForceStreaming = settings.GetValue("vController", "ForceStreaming", false);
            cb_forceMulticorder.Checked = _vMixForceMulticorder = settings.GetValue("vController", "ForceMulticorder", false);
            cb_runScript.Checked = _vMixRunScript = settings.GetValue("vController", "RunScript", false);
            tb_script.Text = _vMixScript = settings.GetValue("vController", "StartScript", "");
            cb_autostart.Checked = _vMixAutoStart = settings.GetValue("vController", "AutoStartV", false);
            RegWrite("Software\\Microsoft\\Windows\\CurrentVersion\\Run","vScheduler",vControllerPath + "\\vController.exe", !_vMixAutoStart);
            cb_startmini.Checked = _vMixStartMini = settings.GetValue("vController", "StartMini", false);
            cb_closetray.Checked = _vMixCloseTray = settings.GetValue("vController", "CloseTray", false);
            cb_minitray.Checked = _vMixMiniTray = settings.GetValue("vController", "MiniTray", false);
        }

        public void SaveSettings()
        {
            settings.SetValue("vMix", "vMixPort", _vMixPort);
            settings.SetValue("vMix", "vMixIPadress", _vMixIPadress);
            settings.SetValue("vController", "MediaPreload", _vMixPreload);
            settings.SetValue("vController", "MediaLinger", _vMixLinger);
            settings.SetValue("vController", "Latency", _vMixLate);
            settings.SetValue("vController", "vMixRefresh", _vMixRefresh);
            settings.SetValue("vController", "AutoLoad", _vMixAutoLoad);
            settings.SetValue("vController", "AutoLoadV", _vMixAutoLoadV);
            settings.SetValue("vController", "vMixPath", _vMixPath);
            settings.SetValue("vController", "ForceExternal", _vMixForceExternal);
            settings.SetValue("vController", "ForceRecording", _vMixForceRecording);
            settings.SetValue("vController", "ForceStreaming", _vMixForceStreaming);
            settings.SetValue("vController", "ForceMulticorder", _vMixForceMulticorder);
            settings.SetValue("vController", "RunScript", _vMixRunScript);
            settings.SetValue("vController", "StartScript", _vMixScript);
            settings.SetValue("vController", "AutoStartV", _vMixAutoStart);
            settings.SetValue("vController", "StartMini", _vMixStartMini);
            settings.SetValue("vController", "CloseTray", _vMixCloseTray);
            settings.SetValue("vController", "MiniTray", _vMixMiniTray);
            settings.Save();
        }

        private void RegWrite(string key, string valuename, string value, bool delete = false)
        {
            RegistryKey subkey = Registry.CurrentUser.CreateSubKey(key);
            string keyvaluecontent = subkey.GetValue(valuename, "").ToString();
            if (delete)
            {
                subkey.DeleteValue(valuename, false);
            }
            else if (keyvaluecontent != value)
            {
                subkey.SetValue(valuename, value, RegistryValueKind.String);
            }
            subkey.Close();
        }
        private void bn_testport_Click(object sender, EventArgs e)
        {
            WebClient vMix = new WebClient ();
            vMix.BaseAddress = vMixURL;
            XmlDocument doc = new XmlDocument();

            bool result = false;
            try
            {
                doc.LoadXml(vMix.DownloadString("api"));
                if (doc.SelectNodes("vmix/inputs/input").Count > 1)
                    result = true;
            }
            catch { }
            if (result)
                MessageBox.Show("vMix seems fine, or at least there\r\nis a responsive webserver at this URL:\r\n\r\n"+vMixURL,"vMix OK");
            else
                MessageBox.Show("vMix didn't respond at this URL:\r\n\r\n"+vMixURL,"vMix not found");
        }

        private void ud_vMixPort_ValueChanged(object sender, EventArgs e)
        {
            _vMixPort = (int)ud_vMixPort.Value;
        }

        private void ud_preload_ValueChanged(object sender, EventArgs e)
        {
            _vMixPreload = (int)ud_preload.Value;
        }

        private void ud_linger_ValueChanged(object sender, EventArgs e)
        {
            _vMixLinger = (int)ud_linger.Value;
        }

        private void vMixPreferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            _vMixPath = tb_path.Text;
            _vMixScript = tb_script.Text;
            SaveSettings();
            RegWrite("Software\\Microsoft\\Windows\\CurrentVersion\\Run", "vScheduler", vControllerPath + "\\vController.exe", !_vMixAutoStart);
        }

        private void cb_autoload_CheckedChanged(object sender, EventArgs e)
        {
            _vMixAutoLoad = cb_autoload.Checked;
        }

        private void tb_vMixIP_TextChanged(object sender, EventArgs e)
        {
            _vMixIPadress = tb_vMixIP.Text;
        }

        private void vPreferences_Load(object sender, EventArgs e)
        {
            ud_vMixPort.Value = _vMixPort;
            tb_vMixIP.Text = _vMixIPadress;
            ud_preload.Value = _vMixPreload;
            ud_linger.Value = _vMixLinger;
            ud_late.Value = _vMixLate;
            ud_refresh.Value = _vMixRefresh;
            cb_autoload.Checked = _vMixAutoLoad;
            cb_autoloadV.Checked = _vMixAutoLoadV;
            tb_path.Text = _vMixPath;
            cb_autostart.Checked = _vMixAutoStart;
            cb_startmini.Checked = _vMixStartMini;
            cb_closetray.Checked = _vMixCloseTray;
            cb_minitray.Checked = _vMixMiniTray;
        }

        private void cb_startmini_CheckedChanged(object sender, EventArgs e)
        {
            _vMixStartMini = cb_startmini.Checked;
        }

        private void cb_closetray_CheckedChanged(object sender, EventArgs e)
        {
            _vMixCloseTray = cb_closetray.Checked;
        }

        private void cb_minitray_CheckedChanged(object sender, EventArgs e)
        {
            _vMixMiniTray = cb_minitray.Checked;
        }

        private void cb_autoloadV_CheckedChanged(object sender, EventArgs e)
        {
            _vMixAutoLoadV = cb_autoloadV.Checked;
        }

        private void cb_autostart_CheckedChanged(object sender, EventArgs e)
        {
            _vMixAutoStart = cb_autostart.Checked;
        }

        private void tb_path_DoubleClick(object sender, EventArgs e)
        {
            FileDialog fd;
            fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                tb_path.Text = fd.FileName;
                _vMixPath = tb_path.Text;
            }
        }

        private void ud_refresh_ValueChanged(object sender, EventArgs e)
        {
            _vMixRefresh = (int)ud_refresh.Value;
        }

        private void ud_late_ValueChanged(object sender, EventArgs e)
        {
            _vMixLate = (int)ud_late.Value;
        }

        private void cb_forceExternal_CheckedChanged(object sender, EventArgs e)
        {
            _vMixForceExternal = cb_forceExternal.Checked;
        }

        private void cb_forceRecording_CheckedChanged(object sender, EventArgs e)
        {
            _vMixForceRecording = cb_forceRecording.Checked;
        }

        private void cb_forceStreaming_CheckedChanged(object sender, EventArgs e)
        {
            _vMixForceStreaming = cb_forceStreaming.Checked;
        }

        private void cb_forceMulticorder_CheckedChanged(object sender, EventArgs e)
        {
            _vMixForceMulticorder = cb_forceMulticorder.Checked;
        }

        private void cb_runScript_CheckedChanged(object sender, EventArgs e)
        {
            _vMixRunScript = cb_runScript.Checked;
        }
    }
}
