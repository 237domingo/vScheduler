using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace vControler
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (SingleApplicationDetector.IsRunning())
            {
                MessageBox.Show("Already Running", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                SingleApplicationDetector.Close();
            }
            else
            {
                Application.Run(new vMixControler());
                SingleApplicationDetector.Close();
            }
        }
    }
}
