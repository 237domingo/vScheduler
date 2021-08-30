namespace vControler
{
    partial class vPreferences
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vPreferences));
            this.ud_vMixPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.bn_testport = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ud_preload = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ud_linger = new System.Windows.Forms.NumericUpDown();
            this.cb_autoload = new System.Windows.Forms.CheckBox();
            this.tb_vMixIP = new System.Windows.Forms.TextBox();
            this.cb_autoloadV = new System.Windows.Forms.CheckBox();
            this.cb_autostart = new System.Windows.Forms.CheckBox();
            this.cb_startmini = new System.Windows.Forms.CheckBox();
            this.cb_closetray = new System.Windows.Forms.CheckBox();
            this.cb_minitray = new System.Windows.Forms.CheckBox();
            this.tb_path = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ud_late = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.ud_refresh = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tb_script = new System.Windows.Forms.TextBox();
            this.cb_forceExternal = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cb_forceMulticorder = new System.Windows.Forms.CheckBox();
            this.cb_forceRecording = new System.Windows.Forms.CheckBox();
            this.cb_forceStreaming = new System.Windows.Forms.CheckBox();
            this.cb_runScript = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ud_vMixPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_preload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_linger)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_late)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_refresh)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ud_vMixPort
            // 
            this.ud_vMixPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ud_vMixPort.Location = new System.Drawing.Point(304, 41);
            this.ud_vMixPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ud_vMixPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ud_vMixPort.Name = "ud_vMixPort";
            this.ud_vMixPort.Size = new System.Drawing.Size(52, 21);
            this.ud_vMixPort.TabIndex = 0;
            this.ud_vMixPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ud_vMixPort.Value = new decimal(new int[] {
            8088,
            0,
            0,
            0});
            this.ud_vMixPort.ValueChanged += new System.EventHandler(this.ud_vMixPort_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP adress of computer running vMix (default is 127.0.0.1):";
            // 
            // bn_testport
            // 
            this.bn_testport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_testport.Location = new System.Drawing.Point(356, 41);
            this.bn_testport.Name = "bn_testport";
            this.bn_testport.Size = new System.Drawing.Size(42, 21);
            this.bn_testport.TabIndex = 2;
            this.bn_testport.Text = "Test";
            this.bn_testport.UseVisualStyleBackColor = true;
            this.bn_testport.Click += new System.EventHandler(this.bn_testport_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Load Input this amount of time before event start:";
            // 
            // ud_preload
            // 
            this.ud_preload.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ud_preload.Location = new System.Drawing.Point(271, 19);
            this.ud_preload.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.ud_preload.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ud_preload.Name = "ud_preload";
            this.ud_preload.Size = new System.Drawing.Size(60, 21);
            this.ud_preload.TabIndex = 3;
            this.ud_preload.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ud_preload.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_preload.ValueChanged += new System.EventHandler(this.ud_preload_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(333, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Seconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(333, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Seconds";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(243, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Remove Input this amount of time after event end:";
            // 
            // ud_linger
            // 
            this.ud_linger.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ud_linger.Location = new System.Drawing.Point(271, 46);
            this.ud_linger.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.ud_linger.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ud_linger.Name = "ud_linger";
            this.ud_linger.Size = new System.Drawing.Size(60, 21);
            this.ud_linger.TabIndex = 6;
            this.ud_linger.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ud_linger.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_linger.ValueChanged += new System.EventHandler(this.ud_linger_ValueChanged);
            // 
            // cb_autoload
            // 
            this.cb_autoload.AutoSize = true;
            this.cb_autoload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_autoload.Location = new System.Drawing.Point(142, 19);
            this.cb_autoload.Name = "cb_autoload";
            this.cb_autoload.Size = new System.Drawing.Size(147, 17);
            this.cb_autoload.TabIndex = 9;
            this.cb_autoload.Text = "Start schedule on start up";
            this.cb_autoload.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_autoload.UseVisualStyleBackColor = true;
            this.cb_autoload.CheckedChanged += new System.EventHandler(this.cb_autoload_CheckedChanged);
            // 
            // tb_vMixIP
            // 
            this.tb_vMixIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_vMixIP.Location = new System.Drawing.Point(304, 13);
            this.tb_vMixIP.Name = "tb_vMixIP";
            this.tb_vMixIP.Size = new System.Drawing.Size(94, 20);
            this.tb_vMixIP.TabIndex = 11;
            this.tb_vMixIP.Text = "127.0.0.1";
            this.tb_vMixIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tb_vMixIP.TextChanged += new System.EventHandler(this.tb_vMixIP_TextChanged);
            // 
            // cb_autoloadV
            // 
            this.cb_autoloadV.AutoSize = true;
            this.cb_autoloadV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_autoloadV.Location = new System.Drawing.Point(6, 19);
            this.cb_autoloadV.Name = "cb_autoloadV";
            this.cb_autoloadV.Size = new System.Drawing.Size(373, 17);
            this.cb_autoloadV.TabIndex = 9;
            this.cb_autoloadV.Text = "Start vMix on vScheduler startup (vMix must be installed on this computer)";
            this.cb_autoloadV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_autoloadV.UseVisualStyleBackColor = true;
            this.cb_autoloadV.CheckedChanged += new System.EventHandler(this.cb_autoloadV_CheckedChanged);
            // 
            // cb_autostart
            // 
            this.cb_autostart.AutoSize = true;
            this.cb_autostart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_autostart.Location = new System.Drawing.Point(9, 19);
            this.cb_autostart.Name = "cb_autostart";
            this.cb_autostart.Size = new System.Drawing.Size(117, 17);
            this.cb_autostart.TabIndex = 9;
            this.cb_autostart.Text = "Start with computer";
            this.cb_autostart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_autostart.UseVisualStyleBackColor = true;
            this.cb_autostart.CheckedChanged += new System.EventHandler(this.cb_autostart_CheckedChanged);
            // 
            // cb_startmini
            // 
            this.cb_startmini.AutoSize = true;
            this.cb_startmini.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_startmini.Location = new System.Drawing.Point(305, 19);
            this.cb_startmini.Name = "cb_startmini";
            this.cb_startmini.Size = new System.Drawing.Size(96, 17);
            this.cb_startmini.TabIndex = 9;
            this.cb_startmini.Text = "Start minimized";
            this.cb_startmini.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_startmini.UseVisualStyleBackColor = true;
            this.cb_startmini.CheckedChanged += new System.EventHandler(this.cb_startmini_CheckedChanged);
            // 
            // cb_closetray
            // 
            this.cb_closetray.AutoSize = true;
            this.cb_closetray.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_closetray.Location = new System.Drawing.Point(142, 42);
            this.cb_closetray.Name = "cb_closetray";
            this.cb_closetray.Size = new System.Drawing.Size(84, 17);
            this.cb_closetray.TabIndex = 9;
            this.cb_closetray.Text = "Close to tray";
            this.cb_closetray.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_closetray.UseVisualStyleBackColor = true;
            this.cb_closetray.Visible = false;
            this.cb_closetray.CheckedChanged += new System.EventHandler(this.cb_closetray_CheckedChanged);
            // 
            // cb_minitray
            // 
            this.cb_minitray.AutoSize = true;
            this.cb_minitray.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_minitray.Location = new System.Drawing.Point(9, 42);
            this.cb_minitray.Name = "cb_minitray";
            this.cb_minitray.Size = new System.Drawing.Size(98, 17);
            this.cb_minitray.TabIndex = 9;
            this.cb_minitray.Text = "Minimize to tray";
            this.cb_minitray.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_minitray.UseVisualStyleBackColor = true;
            this.cb_minitray.CheckedChanged += new System.EventHandler(this.cb_minitray_CheckedChanged);
            // 
            // tb_path
            // 
            this.tb_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_path.Location = new System.Drawing.Point(140, 42);
            this.tb_path.Name = "tb_path";
            this.tb_path.Size = new System.Drawing.Size(255, 20);
            this.tb_path.TabIndex = 12;
            this.toolTip1.SetToolTip(this.tb_path, "Double click to change path");
            this.tb_path.DoubleClick += new System.EventHandler(this.tb_path_DoubleClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(233, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "vMix API port as in vMix config (default is 8088):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_vMixIP);
            this.groupBox1.Controls.Add(this.bn_testport);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ud_vMixPort);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 74);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "vMix API";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(24, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "vMix executable path: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.ud_preload);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.ud_late);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.ud_refresh);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.ud_linger);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 203);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 125);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Performance settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(229, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Latency (vMix, network and computer related) :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(249, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Interval of time between vMix API requests attempt:";
            // 
            // ud_late
            // 
            this.ud_late.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ud_late.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_late.Location = new System.Drawing.Point(271, 100);
            this.ud_late.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ud_late.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_late.Name = "ud_late";
            this.ud_late.Size = new System.Drawing.Size(60, 21);
            this.ud_late.TabIndex = 6;
            this.ud_late.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ud_late.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ud_late.ValueChanged += new System.EventHandler(this.ud_late_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(334, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Milliseconds";
            // 
            // ud_refresh
            // 
            this.ud_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ud_refresh.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_refresh.Location = new System.Drawing.Point(271, 73);
            this.ud_refresh.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ud_refresh.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ud_refresh.Name = "ud_refresh";
            this.ud_refresh.Size = new System.Drawing.Size(60, 21);
            this.ud_refresh.TabIndex = 6;
            this.ud_refresh.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ud_refresh.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ud_refresh.ValueChanged += new System.EventHandler(this.ud_refresh_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(334, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Milliseconds";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_autostart);
            this.groupBox3.Controls.Add(this.cb_startmini);
            this.groupBox3.Controls.Add(this.cb_minitray);
            this.groupBox3.Controls.Add(this.cb_closetray);
            this.groupBox3.Controls.Add(this.cb_autoload);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(10, 334);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(407, 71);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Misc";
            // 
            // tb_script
            // 
            this.tb_script.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_script.Location = new System.Drawing.Point(140, 89);
            this.tb_script.Name = "tb_script";
            this.tb_script.Size = new System.Drawing.Size(255, 20);
            this.tb_script.TabIndex = 14;
            this.toolTip1.SetToolTip(this.tb_script, "must be in vMix list of script in settings panel");
            // 
            // cb_forceExternal
            // 
            this.cb_forceExternal.AutoSize = true;
            this.cb_forceExternal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_forceExternal.Location = new System.Drawing.Point(6, 71);
            this.cb_forceExternal.Name = "cb_forceExternal";
            this.cb_forceExternal.Size = new System.Drawing.Size(93, 17);
            this.cb_forceExternal.TabIndex = 9;
            this.cb_forceExternal.Text = "Force external";
            this.cb_forceExternal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_forceExternal.UseVisualStyleBackColor = true;
            this.cb_forceExternal.CheckedChanged += new System.EventHandler(this.cb_forceExternal_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tb_script);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cb_autoloadV);
            this.groupBox4.Controls.Add(this.tb_path);
            this.groupBox4.Controls.Add(this.cb_forceMulticorder);
            this.groupBox4.Controls.Add(this.cb_forceRecording);
            this.groupBox4.Controls.Add(this.cb_forceStreaming);
            this.groupBox4.Controls.Add(this.cb_runScript);
            this.groupBox4.Controls.Add(this.cb_forceExternal);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(10, 82);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(407, 115);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "vMix startup settings";
            // 
            // cb_forceMulticorder
            // 
            this.cb_forceMulticorder.AutoSize = true;
            this.cb_forceMulticorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_forceMulticorder.Location = new System.Drawing.Point(300, 71);
            this.cb_forceMulticorder.Name = "cb_forceMulticorder";
            this.cb_forceMulticorder.Size = new System.Drawing.Size(107, 17);
            this.cb_forceMulticorder.TabIndex = 9;
            this.cb_forceMulticorder.Text = "Force multicorder";
            this.cb_forceMulticorder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_forceMulticorder.UseVisualStyleBackColor = true;
            this.cb_forceMulticorder.CheckedChanged += new System.EventHandler(this.cb_forceMulticorder_CheckedChanged);
            // 
            // cb_forceRecording
            // 
            this.cb_forceRecording.AutoSize = true;
            this.cb_forceRecording.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_forceRecording.Location = new System.Drawing.Point(99, 71);
            this.cb_forceRecording.Name = "cb_forceRecording";
            this.cb_forceRecording.Size = new System.Drawing.Size(100, 17);
            this.cb_forceRecording.TabIndex = 9;
            this.cb_forceRecording.Text = "Force recording";
            this.cb_forceRecording.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_forceRecording.UseVisualStyleBackColor = true;
            this.cb_forceRecording.CheckedChanged += new System.EventHandler(this.cb_forceRecording_CheckedChanged);
            // 
            // cb_forceStreaming
            // 
            this.cb_forceStreaming.AutoSize = true;
            this.cb_forceStreaming.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_forceStreaming.Location = new System.Drawing.Point(201, 71);
            this.cb_forceStreaming.Name = "cb_forceStreaming";
            this.cb_forceStreaming.Size = new System.Drawing.Size(101, 17);
            this.cb_forceStreaming.TabIndex = 9;
            this.cb_forceStreaming.Text = "Force streaming";
            this.cb_forceStreaming.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_forceStreaming.UseVisualStyleBackColor = true;
            this.cb_forceStreaming.CheckedChanged += new System.EventHandler(this.cb_forceStreaming_CheckedChanged);
            // 
            // cb_runScript
            // 
            this.cb_runScript.AutoSize = true;
            this.cb_runScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_runScript.Location = new System.Drawing.Point(6, 91);
            this.cb_runScript.Name = "cb_runScript";
            this.cb_runScript.Size = new System.Drawing.Size(128, 17);
            this.cb_runScript.TabIndex = 9;
            this.cb_runScript.Text = "Run this script (name)";
            this.cb_runScript.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_runScript.UseVisualStyleBackColor = true;
            this.cb_runScript.CheckedChanged += new System.EventHandler(this.cb_runScript_CheckedChanged);
            // 
            // vPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 415);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(440, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 380);
            this.Name = "vPreferences";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.vMixPreferences_FormClosing);
            this.Load += new System.EventHandler(this.vPreferences_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ud_vMixPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_preload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_linger)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_late)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_refresh)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown ud_vMixPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bn_testport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ud_preload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown ud_linger;
        private System.Windows.Forms.CheckBox cb_autoload;
        private System.Windows.Forms.TextBox tb_vMixIP;
        private System.Windows.Forms.CheckBox cb_autoloadV;
        private System.Windows.Forms.CheckBox cb_autostart;
        private System.Windows.Forms.CheckBox cb_startmini;
        private System.Windows.Forms.CheckBox cb_closetray;
        private System.Windows.Forms.CheckBox cb_minitray;
        private System.Windows.Forms.TextBox tb_path;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown ud_refresh;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown ud_late;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cb_forceExternal;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tb_script;
        private System.Windows.Forms.CheckBox cb_forceMulticorder;
        private System.Windows.Forms.CheckBox cb_forceRecording;
        private System.Windows.Forms.CheckBox cb_forceStreaming;
        private System.Windows.Forms.CheckBox cb_runScript;
    }
}