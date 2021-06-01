namespace vMixManager
{
    partial class vOverlaySelection
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
            this.bn_ok = new System.Windows.Forms.Button();
            this.bn_cancel = new System.Windows.Forms.Button();
            this.cb_overlay0 = new System.Windows.Forms.CheckBox();
            this.cb_overlay1 = new System.Windows.Forms.CheckBox();
            this.cb_overlay3 = new System.Windows.Forms.CheckBox();
            this.cb_overlay2 = new System.Windows.Forms.CheckBox();
            this.cb_overlay4 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bn_ok
            // 
            this.bn_ok.Location = new System.Drawing.Point(150, 3);
            this.bn_ok.Name = "bn_ok";
            this.bn_ok.Size = new System.Drawing.Size(96, 26);
            this.bn_ok.TabIndex = 1;
            this.bn_ok.Text = "OK";
            this.bn_ok.UseVisualStyleBackColor = true;
            this.bn_ok.Click += new System.EventHandler(this.bn_ok_Click);
            // 
            // bn_cancel
            // 
            this.bn_cancel.Location = new System.Drawing.Point(150, 35);
            this.bn_cancel.Name = "bn_cancel";
            this.bn_cancel.Size = new System.Drawing.Size(96, 26);
            this.bn_cancel.TabIndex = 2;
            this.bn_cancel.Text = "CANCEL";
            this.bn_cancel.UseVisualStyleBackColor = true;
            this.bn_cancel.Click += new System.EventHandler(this.bn_cancel_Click);
            // 
            // cb_overlay0
            // 
            this.cb_overlay0.AutoSize = true;
            this.cb_overlay0.Checked = true;
            this.cb_overlay0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_overlay0.Enabled = false;
            this.cb_overlay0.Location = new System.Drawing.Point(9, 26);
            this.cb_overlay0.Name = "cb_overlay0";
            this.cb_overlay0.Size = new System.Drawing.Size(68, 17);
            this.cb_overlay0.TabIndex = 4;
            this.cb_overlay0.Text = "Overlay0";
            this.cb_overlay0.UseVisualStyleBackColor = true;
            // 
            // cb_overlay1
            // 
            this.cb_overlay1.AutoSize = true;
            this.cb_overlay1.Location = new System.Drawing.Point(9, 49);
            this.cb_overlay1.Name = "cb_overlay1";
            this.cb_overlay1.Size = new System.Drawing.Size(68, 17);
            this.cb_overlay1.TabIndex = 5;
            this.cb_overlay1.Text = "Overlay1";
            this.cb_overlay1.UseVisualStyleBackColor = true;
            // 
            // cb_overlay3
            // 
            this.cb_overlay3.AutoSize = true;
            this.cb_overlay3.Location = new System.Drawing.Point(82, 26);
            this.cb_overlay3.Name = "cb_overlay3";
            this.cb_overlay3.Size = new System.Drawing.Size(68, 17);
            this.cb_overlay3.TabIndex = 6;
            this.cb_overlay3.Text = "Overlay3";
            this.cb_overlay3.UseVisualStyleBackColor = true;
            // 
            // cb_overlay2
            // 
            this.cb_overlay2.AutoSize = true;
            this.cb_overlay2.Location = new System.Drawing.Point(82, 3);
            this.cb_overlay2.Name = "cb_overlay2";
            this.cb_overlay2.Size = new System.Drawing.Size(68, 17);
            this.cb_overlay2.TabIndex = 7;
            this.cb_overlay2.Text = "Overlay2";
            this.cb_overlay2.UseVisualStyleBackColor = true;
            // 
            // cb_overlay4
            // 
            this.cb_overlay4.AutoSize = true;
            this.cb_overlay4.Location = new System.Drawing.Point(82, 49);
            this.cb_overlay4.Name = "cb_overlay4";
            this.cb_overlay4.Size = new System.Drawing.Size(68, 17);
            this.cb_overlay4.TabIndex = 8;
            this.cb_overlay4.Text = "Overlay4";
            this.cb_overlay4.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 26);
            this.label1.TabIndex = 9;
            this.label1.Text = "Active overlay\r\nis locked";
            // 
            // vOverlaySelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 69);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_overlay4);
            this.Controls.Add(this.cb_overlay2);
            this.Controls.Add(this.cb_overlay3);
            this.Controls.Add(this.cb_overlay1);
            this.Controls.Add(this.cb_overlay0);
            this.Controls.Add(this.bn_cancel);
            this.Controls.Add(this.bn_ok);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(265, 108);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(265, 108);
            this.Name = "vOverlaySelection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Overlays";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bn_ok;
        private System.Windows.Forms.Button bn_cancel;
        private  System.Windows.Forms.CheckBox cb_overlay0;
        private System.Windows.Forms.CheckBox cb_overlay1;
        private System.Windows.Forms.CheckBox cb_overlay3;
        private System.Windows.Forms.CheckBox cb_overlay2;
        private System.Windows.Forms.CheckBox cb_overlay4;
        private System.Windows.Forms.Label label1;

    }
}