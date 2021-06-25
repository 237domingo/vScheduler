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
    public partial class FindBox : Form
    {
        public int findtype;
        public DateTime date;
        public string title;
        public FindBox()
        {
            InitializeComponent();
            findtype = 1;
            date = DateTime.Now;
            title = "";
        }

        private void bn_date_Click(object sender, EventArgs e)
        {
            date = dtp_time.Value;
            findtype = 0;
            Hide();
        }

        private void bn_title_Click(object sender, EventArgs e)
        {
            title = tb_title.Text;
            findtype = 1;
            Hide();
        }

    }
}
