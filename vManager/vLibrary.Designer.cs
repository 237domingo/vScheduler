namespace vManager
{
    partial class vLibrary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.bn_add = new System.Windows.Forms.Button();
            this.bn_remove = new System.Windows.Forms.Button();
            this.bn_update = new System.Windows.Forms.Button();
            this.dataSet1 = new System.Data.DataSet();
            this.folderlist = new System.Data.DataTable();
            this.Foldername = new System.Data.DataColumn();
            this.Folderpath = new System.Data.DataColumn();
            this.filecollection = new System.Data.DataTable();
            this.Filepath = new System.Data.DataColumn();
            this.Filename = new System.Data.DataColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.folderlist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filecollection)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(-1, 1);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(320, 134);
            this.listBox1.TabIndex = 0;
            // 
            // bn_add
            // 
            this.bn_add.Location = new System.Drawing.Point(325, 12);
            this.bn_add.Name = "bn_add";
            this.bn_add.Size = new System.Drawing.Size(69, 33);
            this.bn_add.TabIndex = 1;
            this.bn_add.Text = "Add";
            this.bn_add.UseVisualStyleBackColor = true;
            this.bn_add.Click += new System.EventHandler(this.bn_add_Click);
            // 
            // bn_remove
            // 
            this.bn_remove.Location = new System.Drawing.Point(325, 51);
            this.bn_remove.Name = "bn_remove";
            this.bn_remove.Size = new System.Drawing.Size(67, 34);
            this.bn_remove.TabIndex = 2;
            this.bn_remove.Text = "Remove";
            this.bn_remove.UseVisualStyleBackColor = true;
            this.bn_remove.Click += new System.EventHandler(this.bn_remove_Click);
            // 
            // bn_update
            // 
            this.bn_update.Location = new System.Drawing.Point(325, 91);
            this.bn_update.Name = "bn_update";
            this.bn_update.Size = new System.Drawing.Size(66, 33);
            this.bn_update.TabIndex = 3;
            this.bn_update.Text = "Update";
            this.bn_update.UseVisualStyleBackColor = true;
            this.bn_update.Click += new System.EventHandler(this.bn_update_Click);
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "vLibrary";
            this.dataSet1.Locale = new System.Globalization.CultureInfo("");
            this.dataSet1.RemotingFormat = System.Data.SerializationFormat.Binary;
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.folderlist,
            this.filecollection});
            // 
            // folderlist
            // 
            this.folderlist.Columns.AddRange(new System.Data.DataColumn[] {
            this.Foldername,
            this.Folderpath});
            this.folderlist.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Folderpath"}, true)});
            this.folderlist.Locale = new System.Globalization.CultureInfo("");
            this.folderlist.PrimaryKey = new System.Data.DataColumn[] {
        this.Folderpath};
            this.folderlist.RemotingFormat = System.Data.SerializationFormat.Binary;
            this.folderlist.TableName = "folderlist";
            // 
            // Foldername
            // 
            this.Foldername.AllowDBNull = false;
            this.Foldername.ColumnName = "Foldername";
            this.Foldername.DefaultValue = "Foldername";
            // 
            // Folderpath
            // 
            this.Folderpath.AllowDBNull = false;
            this.Folderpath.ColumnName = "Folderpath";
            this.Folderpath.DefaultValue = "Folderpath";
            // 
            // filecollection
            // 
            this.filecollection.Columns.AddRange(new System.Data.DataColumn[] {
            this.Filepath,
            this.Filename});
            this.filecollection.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Filename"}, true),
            new System.Data.ForeignKeyConstraint("Constraint2", "folderlist", new string[] {
                        "Folderpath"}, new string[] {
                        "Filepath"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.filecollection.PrimaryKey = new System.Data.DataColumn[] {
        this.Filename};
            this.filecollection.RemotingFormat = System.Data.SerializationFormat.Binary;
            this.filecollection.TableName = "filecollection";
            // 
            // Filepath
            // 
            this.Filepath.AllowDBNull = false;
            this.Filepath.Caption = "Filepath";
            this.Filepath.ColumnName = "Filepath";
            this.Filepath.DefaultValue = "Filepath";
            // 
            // Filename
            // 
            this.Filename.AllowDBNull = false;
            this.Filename.ColumnName = "Filename";
            this.Filename.DefaultValue = "Filename";
            // 
            // vLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(400, 139);
            this.Controls.Add(this.bn_update);
            this.Controls.Add(this.bn_remove);
            this.Controls.Add(this.bn_add);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(416, 178);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(416, 178);
            this.Name = "vLibrary";
            this.Icon = global::vManager.Properties.Resources.pencil;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Library";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.vLibrary_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.folderlist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filecollection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button bn_add;
        private System.Windows.Forms.Button bn_remove;
        private System.Windows.Forms.Button bn_update;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable folderlist;
        private System.Data.DataColumn Foldername;
        private System.Data.DataColumn Folderpath;
        private System.Data.DataTable filecollection;
        private System.Data.DataColumn Filepath;
        private System.Data.DataColumn Filename;


    }
}