using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace vManager
{
    public partial class vLibrary : Form
    {
        string libraryfile;
        public List<string> files;
        public List<string> folders;
        public vMixManager.FixPath fixpath;
        public vLibrary(Xml settings)
        {
            InitializeComponent();
            files = new List<string>();
            folders = new List<string>();
            libraryfile = settings.GetValue("vManager", "Library", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\vScheduler\\vLibrary.data" );
            if (File.Exists(libraryfile)) dataSet1.ReadXml(libraryfile);
            foreach (DataRow r in dataSet1.Tables[0].Rows)
                folders.Add(r.ItemArray[1].ToString());
            listBox1.Items.AddRange(folders.ToArray());
            foreach (DataRow r in dataSet1.Tables[1].Rows)
                files.Add(r.ItemArray[1].ToString());
        }

        private string ConcateArrayOfString(List<string> toconcat)
        {
            int numberofelement = toconcat.Count;
            if (numberofelement == 0) return "";
            string result = toconcat[0];
            if (numberofelement == 1) return result;
            if (result != "") result += "|";
            if (numberofelement > 2)
            {
                for (int i = 1; i < numberofelement - 1; i++)
                { result = result + toconcat[i] + "|"; }
            }
            return result + toconcat[numberofelement - 1];
        }

        private void bn_add_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderselect = new FolderBrowserDialog();
            if (folderselect.ShowDialog() == DialogResult.OK)
            {
                string[] entry = { System.IO.Path.GetFileName(folderselect.SelectedPath), folderselect.SelectedPath };
                int i = dataSet1.Tables[0].Rows.Count;
                int j = dataSet1.Tables[1].Rows.Count;
                dataSet1.Tables[0].LoadDataRow(entry, true);
                if (i != dataSet1.Tables[0].Rows.Count)
                {
                    listBox1.Items.Add(folderselect.SelectedPath);
                    dataSet1.Tables[1].LoadDataRow(new string[]{folderselect.SelectedPath, folderselect.SelectedPath}, true);
                }
            }
        }

        private void bn_remove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            dataSet1.Tables[0].Select("Folderpath='" + listBox1.SelectedItem.ToString()+"'", "Folderpath", DataViewRowState.CurrentRows)[0].Delete();
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void vLibrary_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataSet1.AcceptChanges();
            dataSet1.WriteXml(libraryfile);
        }

        public void bn_update_Click(object sender, EventArgs e)
        {
            folders.Clear();
            files.Clear();
            foreach (DataRow r in dataSet1.Tables[0].Rows)
            {
                folders.Add(r.ItemArray[1].ToString());
            }
            dataSet1.Tables[1].Clear();
            foreach (string l in folders)
            {
                dataSet1.Tables[1].LoadDataRow(new string[]{l, l}, true);
                List<string> dir = Directory.EnumerateDirectories(l, "*", SearchOption.AllDirectories).ToList();
                foreach (string s in dir)
                {
                    dataSet1.Tables[1].LoadDataRow(new string[] { l, s }, true);
                }
            }
            foreach (DataRow r in dataSet1.Tables[1].Rows)
                files.Add(r.ItemArray[1].ToString());
        }

    }
}
