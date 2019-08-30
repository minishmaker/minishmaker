namespace MinishMaker.UI
{
	partial class AreaEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AreaEditor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.areaLayout = new System.Windows.Forms.PictureBox();
            this.areaLabel = new System.Windows.Forms.Label();
            this.roomLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mapY = new System.Windows.Forms.TextBox();
            this.mapX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.areaSongId = new System.Windows.Forms.TextBox();
            this.HiddenLabel = new System.Windows.Forms.Label();
            this.keysShown = new System.Windows.Forms.CheckBox();
            this.redName = new System.Windows.Forms.CheckBox();
            this.dungeonMap = new System.Windows.Forms.CheckBox();
            this.canFlute = new System.Windows.Forms.CheckBox();
            this.moleCave = new System.Windows.Forms.CheckBox();
            this.areaNameId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.areaLayout)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.areaLayout);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 400);
            this.panel1.TabIndex = 0;
            // 
            // areaLayout
            // 
            this.areaLayout.BackColor = System.Drawing.Color.Transparent;
            this.areaLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.areaLayout.Location = new System.Drawing.Point(0, 0);
            this.areaLayout.Name = "areaLayout";
            this.areaLayout.Size = new System.Drawing.Size(400, 400);
            this.areaLayout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.areaLayout.TabIndex = 9;
            this.areaLayout.TabStop = false;
            this.areaLayout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_Click);
            // 
            // areaLabel
            // 
            this.areaLabel.AutoSize = true;
            this.areaLabel.Location = new System.Drawing.Point(63, 418);
            this.areaLabel.Name = "areaLabel";
            this.areaLabel.Size = new System.Drawing.Size(38, 13);
            this.areaLabel.TabIndex = 1;
            this.areaLabel.Text = "Area: -";
            // 
            // roomLabel
            // 
            this.roomLabel.AutoSize = true;
            this.roomLabel.Location = new System.Drawing.Point(17, 441);
            this.roomLabel.Name = "roomLabel";
            this.roomLabel.Size = new System.Drawing.Size(84, 13);
            this.roomLabel.TabIndex = 2;
            this.roomLabel.Text = "Selected room: -";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 418);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Map X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 441);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Map Y:";
            // 
            // mapY
            // 
            this.mapY.Enabled = false;
            this.mapY.Location = new System.Drawing.Point(157, 438);
            this.mapY.MaxLength = 4;
            this.mapY.Name = "mapY";
            this.mapY.Size = new System.Drawing.Size(26, 20);
            this.mapY.TabIndex = 5;
            this.mapY.Text = "FFF";
            this.mapY.LostFocus += new System.EventHandler(this.mapBox_LostFocus);
            // 
            // mapX
            // 
            this.mapX.Enabled = false;
            this.mapX.Location = new System.Drawing.Point(157, 415);
            this.mapX.MaxLength = 4;
            this.mapX.Name = "mapX";
            this.mapX.Size = new System.Drawing.Size(26, 20);
            this.mapX.TabIndex = 6;
            this.mapX.Text = "FFF";
            this.mapX.LostFocus += new System.EventHandler(this.mapBox_LostFocus);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(214, 418);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Area song id:";
            // 
            // areaSongId
            // 
            this.areaSongId.Enabled = false;
            this.areaSongId.Location = new System.Drawing.Point(282, 415);
            this.areaSongId.MaxLength = 2;
            this.areaSongId.Name = "areaSongId";
            this.areaSongId.Size = new System.Drawing.Size(19, 20);
            this.areaSongId.TabIndex = 8;
            this.areaSongId.Text = "FF";
            this.areaSongId.LostFocus += new System.EventHandler(this.AreaChanged);
            // 
            // HiddenLabel
            // 
            this.HiddenLabel.AutoSize = true;
            this.HiddenLabel.Location = new System.Drawing.Point(214, 441);
            this.HiddenLabel.Name = "HiddenLabel";
            this.HiddenLabel.Size = new System.Drawing.Size(0, 13);
            this.HiddenLabel.TabIndex = 9;
            // 
            // keysShown
            // 
            this.keysShown.AutoSize = true;
            this.keysShown.Enabled = false;
            this.keysShown.Location = new System.Drawing.Point(12, 460);
            this.keysShown.Name = "keysShown";
            this.keysShown.Size = new System.Drawing.Size(83, 17);
            this.keysShown.TabIndex = 10;
            this.keysShown.Text = "Keys shown";
            this.keysShown.UseVisualStyleBackColor = true;
            this.keysShown.CheckedChanged += new System.EventHandler(this.AreaChanged);
            // 
            // redName
            // 
            this.redName.AutoSize = true;
            this.redName.Enabled = false;
            this.redName.Location = new System.Drawing.Point(112, 460);
            this.redName.Name = "redName";
            this.redName.Size = new System.Drawing.Size(99, 17);
            this.redName.TabIndex = 11;
            this.redName.Text = "Red area name";
            this.redName.UseVisualStyleBackColor = true;
            this.redName.CheckedChanged += new System.EventHandler(this.AreaChanged);
            // 
            // dungeonMap
            // 
            this.dungeonMap.AutoSize = true;
            this.dungeonMap.Enabled = false;
            this.dungeonMap.Location = new System.Drawing.Point(112, 480);
            this.dungeonMap.Name = "dungeonMap";
            this.dungeonMap.Size = new System.Drawing.Size(113, 17);
            this.dungeonMap.TabIndex = 12;
            this.dungeonMap.Text = "Use dungeon map";
            this.dungeonMap.UseVisualStyleBackColor = true;
            this.dungeonMap.CheckedChanged += new System.EventHandler(this.AreaChanged);
            // 
            // canFlute
            // 
            this.canFlute.AutoSize = true;
            this.canFlute.Enabled = false;
            this.canFlute.Location = new System.Drawing.Point(240, 460);
            this.canFlute.Name = "canFlute";
            this.canFlute.Size = new System.Drawing.Size(88, 17);
            this.canFlute.TabIndex = 13;
            this.canFlute.Text = "Can use flute";
            this.canFlute.UseVisualStyleBackColor = true;
            this.canFlute.CheckedChanged += new System.EventHandler(this.AreaChanged);
            // 
            // moleCave
            // 
            this.moleCave.AutoSize = true;
            this.moleCave.Enabled = false;
            this.moleCave.Location = new System.Drawing.Point(12, 480);
            this.moleCave.Name = "moleCave";
            this.moleCave.Size = new System.Drawing.Size(76, 17);
            this.moleCave.TabIndex = 14;
            this.moleCave.Text = "Mole cave";
            this.moleCave.UseVisualStyleBackColor = true;
            this.moleCave.CheckedChanged += new System.EventHandler(this.AreaChanged);
            // 
            // areaNameId
            // 
            this.areaNameId.Enabled = false;
            this.areaNameId.Location = new System.Drawing.Point(282, 438);
            this.areaNameId.MaxLength = 4;
            this.areaNameId.Name = "areaNameId";
            this.areaNameId.Size = new System.Drawing.Size(19, 20);
            this.areaNameId.TabIndex = 16;
            this.areaNameId.Text = "FF";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 441);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Area name id:";
            // 
            // AreaEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 500);
            this.Controls.Add(this.areaNameId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.moleCave);
            this.Controls.Add(this.canFlute);
            this.Controls.Add(this.dungeonMap);
            this.Controls.Add(this.redName);
            this.Controls.Add(this.keysShown);
            this.Controls.Add(this.HiddenLabel);
            this.Controls.Add(this.areaSongId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mapX);
            this.Controls.Add(this.mapY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.roomLabel);
            this.Controls.Add(this.areaLabel);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AreaEditor";
            this.Text = "Area Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.areaLayout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label areaLabel;
		private System.Windows.Forms.Label roomLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox mapY;
		private System.Windows.Forms.TextBox mapX;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox areaSongId;
		private System.Windows.Forms.PictureBox areaLayout;
		private System.Windows.Forms.Label HiddenLabel;
		private System.Windows.Forms.CheckBox keysShown;
		private System.Windows.Forms.CheckBox redName;
		private System.Windows.Forms.CheckBox dungeonMap;
		private System.Windows.Forms.CheckBox canFlute;
		private System.Windows.Forms.CheckBox moleCave;
		private System.Windows.Forms.TextBox areaNameId;
		private System.Windows.Forms.Label label1;
	}
}