namespace MinishMaker.UI
{
	partial class AreaEditorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AreaEditorWindow));
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
            this.keysShown = new System.Windows.Forms.CheckBox();
            this.redName = new System.Windows.Forms.CheckBox();
            this.dungeonMap = new System.Windows.Forms.CheckBox();
            this.canFlute = new System.Windows.Forms.CheckBox();
            this.moleCave = new System.Windows.Forms.CheckBox();
            this.areaNameId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HiddenLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.areaLayout)).BeginInit();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.areaLayout);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 369);
            this.panel1.TabIndex = 0;
            // 
            // areaLayout
            // 
            this.areaLayout.BackColor = System.Drawing.Color.Transparent;
            this.areaLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.areaLayout.Location = new System.Drawing.Point(0, 0);
            this.areaLayout.Name = "areaLayout";
            this.areaLayout.Size = new System.Drawing.Size(400, 369);
            this.areaLayout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.areaLayout.TabIndex = 9;
            this.areaLayout.TabStop = false;
            this.areaLayout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_Click);
            // 
            // areaLabel
            // 
            this.areaLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.areaLabel, 2);
            this.areaLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.areaLabel.Location = new System.Drawing.Point(3, 0);
            this.areaLabel.Name = "areaLabel";
            this.areaLabel.Size = new System.Drawing.Size(84, 26);
            this.areaLabel.TabIndex = 1;
            this.areaLabel.Text = "Area: -";
            this.areaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // roomLabel
            // 
            this.roomLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.roomLabel, 2);
            this.roomLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roomLabel.Location = new System.Drawing.Point(3, 26);
            this.roomLabel.Name = "roomLabel";
            this.roomLabel.Size = new System.Drawing.Size(84, 26);
            this.roomLabel.TabIndex = 2;
            this.roomLabel.Text = "Selected room: -";
            this.roomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(93, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 26);
            this.label3.TabIndex = 3;
            this.label3.Text = "Map X:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(93, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 26);
            this.label4.TabIndex = 4;
            this.label4.Text = "Map Y:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mapY
            // 
            this.mapY.Enabled = false;
            this.mapY.Location = new System.Drawing.Point(140, 29);
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
            this.mapX.Location = new System.Drawing.Point(140, 3);
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
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(212, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 26);
            this.label5.TabIndex = 7;
            this.label5.Text = "Area song id:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // areaSongId
            // 
            this.areaSongId.Enabled = false;
            this.areaSongId.Location = new System.Drawing.Point(290, 3);
            this.areaSongId.MaxLength = 2;
            this.areaSongId.Name = "areaSongId";
            this.areaSongId.Size = new System.Drawing.Size(19, 20);
            this.areaSongId.TabIndex = 8;
            this.areaSongId.Text = "FF";
            this.areaSongId.LostFocus += new System.EventHandler(this.AreaChanged);
            // 
            // keysShown
            // 
            this.keysShown.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.keysShown, 2);
            this.keysShown.Enabled = false;
            this.keysShown.Location = new System.Drawing.Point(3, 55);
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
            this.tableLayoutPanel1.SetColumnSpan(this.redName, 2);
            this.redName.Enabled = false;
            this.redName.Location = new System.Drawing.Point(93, 55);
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
            this.tableLayoutPanel1.SetColumnSpan(this.dungeonMap, 2);
            this.dungeonMap.Enabled = false;
            this.dungeonMap.Location = new System.Drawing.Point(93, 78);
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
            this.tableLayoutPanel1.SetColumnSpan(this.canFlute, 2);
            this.canFlute.Enabled = false;
            this.canFlute.Location = new System.Drawing.Point(212, 55);
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
            this.tableLayoutPanel1.SetColumnSpan(this.moleCave, 2);
            this.moleCave.Enabled = false;
            this.moleCave.Location = new System.Drawing.Point(3, 78);
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
            this.areaNameId.Location = new System.Drawing.Point(290, 29);
            this.areaNameId.MaxLength = 4;
            this.areaNameId.Name = "areaNameId";
            this.areaNameId.Size = new System.Drawing.Size(19, 20);
            this.areaNameId.TabIndex = 16;
            this.areaNameId.Text = "FF";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(212, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 26);
            this.label1.TabIndex = 15;
            this.label1.Text = "Area name id:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Location = new System.Drawing.Point(12, 384);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 104);
            this.panel2.TabIndex = 17;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.areaLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.roomLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.canFlute, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.areaNameId, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.dungeonMap, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.keysShown, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.redName, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.moleCave, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.mapX, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.areaSongId, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.mapY, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 104);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // HiddenLabel
            // 
            this.HiddenLabel.AutoSize = true;
            this.HiddenLabel.Location = new System.Drawing.Point(214, 441);
            this.HiddenLabel.Name = "HiddenLabel";
            this.HiddenLabel.Size = new System.Drawing.Size(0, 13);
            this.HiddenLabel.TabIndex = 9;
            // 
            // AreaEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(424, 500);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.HiddenLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(440, 539);
            this.Name = "AreaEditorWindow";
            this.Text = "Area Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.areaLayout)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
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
		private System.Windows.Forms.CheckBox keysShown;
		private System.Windows.Forms.CheckBox redName;
		private System.Windows.Forms.CheckBox dungeonMap;
		private System.Windows.Forms.CheckBox canFlute;
		private System.Windows.Forms.CheckBox moleCave;
		private System.Windows.Forms.TextBox areaNameId;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label HiddenLabel;
    }
}
