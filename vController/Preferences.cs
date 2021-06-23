using System;
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

        private bool _vMixAutoLoad = false;
        public bool vMixAutoLoad { get { return _vMixAutoLoad; } }

        public string PreferencesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler\\Settings.xml";

        private Xml settings;
        
        public vPreferences()
        {
            InitializeComponent();
            settings = new Xml();
            settings.LoadXml(PreferencesPath);
            ud_vMixPort.Value = _vMixPort = settings.GetValue("vMix", "vMixPort", 8088);
            tb_vMixIP.Text = _vMixIPadress = settings.GetValue("vMix", "vMixIPadress", "127.0.0.1");
            ud_preload.Value = _vMixPreload = settings.GetValue("vController", "MediaPreload", 5);
            ud_linger.Value = _vMixLinger = settings.GetValue("vController", "MediaLinger", 5);
            cb_autoload.Checked = _vMixAutoLoad = settings.GetValue("vController", "AutoLoad", false);
            if (!File.Exists(PreferencesPath)) settings.Save();    
        }

        public void SaveSettings()
        {
            settings.SetValue("vMix", "vMixPort", _vMixPort);
            settings.SetValue("vMix", "vMixIPadress", _vMixIPadress);
            settings.SetValue("vController", "MediaPreload", _vMixPreload);
            settings.SetValue("vController", "MediaLinger", _vMixLinger);
            settings.SetValue("vController", "AutoLoad", _vMixAutoLoad);
            settings.Save();
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
            SaveSettings();
        }

        private void cb_autoload_CheckedChanged(object sender, EventArgs e)
        {
            _vMixAutoLoad = cb_autoload.Checked;
        }

        private void tb_vMixIP_TextChanged(object sender, EventArgs e)
        {
            _vMixIPadress = tb_vMixIP.Text;
        }
    }
}
