namespace vManager
{
    partial class FindBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindBox));
            this.dtp_time = new System.Windows.Forms.DateTimePicker();
            this.tb_title = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bn_title = new System.Windows.Forms.Button();
            this.bn_date = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dtp_time
            // 
            this.dtp_time.CustomFormat = "yyyy/MM/dd        HH:mm:ss";
            this.dtp_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtp_time.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_time.Location = new System.Drawing.Point(72, 12);
            this.dtp_time.Name = "dtp_time";
            this.dtp_time.Size = new System.Drawing.Size(185, 23);
            this.dtp_time.TabIndex = 0;
            // 
            // tb_title
            // 
            this.tb_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_title.Location = new System.Drawing.Point(72, 48);
            this.tb_title.Name = "tb_title";
            this.tb_title.Size = new System.Drawing.Size(185, 23);
            this.tb_title.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "By Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "By Title";
            // 
            // bn_title
            // 
            this.bn_title.Location = new System.Drawing.Point(263, 48);
            this.bn_title.Name = "bn_title";
            this.bn_title.Size = new System.Drawing.Size(58, 23);
            this.bn_title.TabIndex = 3;
            this.bn_title.Text = "Go";
            this.bn_title.UseVisualStyleBackColor = true;
            this.bn_title.Click += new System.EventHandler(this.bn_title_Click);
            // 
            // bn_date
            // 
            this.bn_date.Location = new System.Drawing.Point(263, 12);
            this.bn_date.Name = "bn_date";
            this.bn_date.Size = new System.Drawing.Size(58, 23);
            this.bn_date.TabIndex = 3;
            this.bn_date.Text = "Go";
            this.bn_date.UseVisualStyleBackColor = true;
            this.bn_date.Click += new System.EventHandler(this.bn_date_Click);
            // 
            // FindBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 78);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bn_date);
            this.Controls.Add(this.bn_title);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_title);
            this.Controls.Add(this.dtp_time);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindBox";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp_time;
        private System.Windows.Forms.TextBox tb_title;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bn_title;
        private System.Windows.Forms.Button bn_date;
    }
}