using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vManager
{
    public partial class vOverlaySelection : Form
    {

        public vOverlaySelection(string ActiveOverlay, string message, bool check = true, bool enabled = false)
        {
            InitializeComponent();
            label1.Text = message;
            cb_overlay0.Checked = ActiveOverlay == "0" && check;
            cb_overlay0.Enabled = !(ActiveOverlay == "0" && !enabled);
            cb_overlay1.Checked = ActiveOverlay == "1" && check;
            cb_overlay1.Enabled = !(ActiveOverlay == "1" && !enabled);
            cb_overlay2.Checked = ActiveOverlay == "2" && check;
            cb_overlay2.Enabled = !(ActiveOverlay == "2" && !enabled);
            cb_overlay3.Checked = ActiveOverlay == "3" && check;
            cb_overlay3.Enabled = !(ActiveOverlay == "3" && !enabled);
            cb_overlay4.Checked = ActiveOverlay == "4" && check;
            cb_overlay4.Enabled = !(ActiveOverlay == "4" && !enabled);
        }

        public vMixManager.SelectedOverlay SelectedOverlayValue;

        public void ActiveOverlay(string ActiveOverlay) 
        {
            cb_overlay0.Checked = (ActiveOverlay == "0");
            cb_overlay0.Enabled = !(ActiveOverlay == "0");
            cb_overlay1.Checked = (ActiveOverlay == "1");
            cb_overlay1.Enabled = !(ActiveOverlay == "1");
            cb_overlay2.Checked = (ActiveOverlay == "2");
            cb_overlay2.Enabled = !(ActiveOverlay == "2");
            cb_overlay3.Checked = (ActiveOverlay == "3");
            cb_overlay3.Enabled = !(ActiveOverlay == "3");
            cb_overlay4.Checked = (ActiveOverlay == "4");
            cb_overlay4.Enabled = !(ActiveOverlay == "4");
        }
       
        private void bn_ok_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            bool[] result = 
            {
                cb_overlay0.Checked, 
                cb_overlay1.Checked,
                cb_overlay2.Checked,
                cb_overlay3.Checked,
                cb_overlay4.Checked
            };
            SelectedOverlayValue(result);
            this.Close();
            MessageBox.Show("Done", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
