using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vControler
{
    public partial class vAbout : Form
    {
        public vAbout()
        {
            InitializeComponent();
        }

        private void bn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bn_license_Click(object sender, EventArgs e)
        {
            vMixTextviewer vmt = new vMixTextviewer();
            vmt.Text = "VSCHEDULER - vController License";
            vmt.View(global::vControler.Properties.Resources.License);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forums.vmix.com/posts/t27113-vScheduler---vMixScheduler-Fork");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/237domingo/vScheduler/issues");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forums.vmix.com/profile/773-macjaeger");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Tim-R/vScheduler");
        }
    }
}
