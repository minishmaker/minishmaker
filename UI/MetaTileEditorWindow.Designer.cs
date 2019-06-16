namespace MinishMaker.UI
{
	partial class MetaTileEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaTileEditor));
            this.metaTileSetPanel = new System.Windows.Forms.Panel();
            this.metaTileGridBox = new MinishMaker.UI.GridBox();
            this.tileSetPanel = new System.Windows.Forms.Panel();
            this.tileSetGridBox = new MinishMaker.UI.GridBox();
            this.selectedMetaTilePanel = new System.Windows.Forms.Panel();
            this.selectedMetaTileBox = new System.Windows.Forms.PictureBox();
            this.selectedTilePanel = new System.Windows.Forms.Panel();
            this.selectedTileBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tLPalette = new System.Windows.Forms.TextBox();
            this.bLPalette = new System.Windows.Forms.TextBox();
            this.tRPalette = new System.Windows.Forms.TextBox();
            this.bRPalette = new System.Windows.Forms.TextBox();
            this.hFlipBox = new System.Windows.Forms.CheckBox();
            this.vFlipBox = new System.Windows.Forms.CheckBox();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.PaletteNum = new System.Windows.Forms.Label();
            this.layer1Button = new System.Windows.Forms.Button();
            this.layer2Button = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tileChange = new System.Windows.Forms.Button();
            this.tId1 = new System.Windows.Forms.TextBox();
            this.tId2 = new System.Windows.Forms.TextBox();
            this.tId3 = new System.Windows.Forms.TextBox();
            this.tId4 = new System.Windows.Forms.TextBox();
            this.sTId = new System.Windows.Forms.TextBox();
            this.mTId = new System.Windows.Forms.TextBox();
            this.metaTileSetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metaTileGridBox)).BeginInit();
            this.tileSetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetGridBox)).BeginInit();
            this.selectedMetaTilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedMetaTileBox)).BeginInit();
            this.selectedTilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedTileBox)).BeginInit();
            this.SuspendLayout();
            // 
            // metaTileSetPanel
            // 
            this.metaTileSetPanel.AutoScroll = true;
            this.metaTileSetPanel.Controls.Add(this.metaTileGridBox);
            this.metaTileSetPanel.Location = new System.Drawing.Point(132, 12);
            this.metaTileSetPanel.Name = "metaTileSetPanel";
            this.metaTileSetPanel.Size = new System.Drawing.Size(275, 240);
            this.metaTileSetPanel.TabIndex = 1;
            // 
            // metaTileGridBox
            // 
            this.metaTileGridBox.AllowMultiSelection = false;
            this.metaTileGridBox.BoxSize = new System.Drawing.Size(16, 16);
            this.metaTileGridBox.CanvasSize = new System.Drawing.Size(128, 128);
            this.metaTileGridBox.HoverBox = true;
            this.metaTileGridBox.HoverColor = System.Drawing.Color.White;
            this.metaTileGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.metaTileGridBox.Location = new System.Drawing.Point(0, 0);
            this.metaTileGridBox.Name = "metaTileGridBox";
            this.metaTileGridBox.Selectable = true;
            this.metaTileGridBox.SelectedIndex = 0;
            this.metaTileGridBox.SelectionColor = System.Drawing.Color.Red;
            this.metaTileGridBox.SelectionRectangle = new System.Drawing.Rectangle(0, 0, 1, 1);
            this.metaTileGridBox.Size = new System.Drawing.Size(128, 128);
            this.metaTileGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.metaTileGridBox.TabIndex = 1;
            this.metaTileGridBox.TabStop = false;
            this.metaTileGridBox.Click += new System.EventHandler(this.metaTileGridBox_Click);
            // 
            // tileSetPanel
            // 
            this.tileSetPanel.AutoScroll = true;
            this.tileSetPanel.Controls.Add(this.tileSetGridBox);
            this.tileSetPanel.Location = new System.Drawing.Point(413, 12);
            this.tileSetPanel.Name = "tileSetPanel";
            this.tileSetPanel.Size = new System.Drawing.Size(147, 240);
            this.tileSetPanel.TabIndex = 2;
            // 
            // tileSetGridBox
            // 
            this.tileSetGridBox.AllowMultiSelection = false;
            this.tileSetGridBox.BoxSize = new System.Drawing.Size(8, 8);
            this.tileSetGridBox.CanvasSize = new System.Drawing.Size(128, 128);
            this.tileSetGridBox.HoverBox = true;
            this.tileSetGridBox.HoverColor = System.Drawing.Color.White;
            this.tileSetGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.tileSetGridBox.Location = new System.Drawing.Point(0, 0);
            this.tileSetGridBox.Name = "tileSetGridBox";
            this.tileSetGridBox.Selectable = true;
            this.tileSetGridBox.SelectedIndex = 0;
            this.tileSetGridBox.SelectionColor = System.Drawing.Color.Red;
            this.tileSetGridBox.SelectionRectangle = new System.Drawing.Rectangle(0, 0, 1, 1);
            this.tileSetGridBox.Size = new System.Drawing.Size(128, 128);
            this.tileSetGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tileSetGridBox.TabIndex = 1;
            this.tileSetGridBox.TabStop = false;
            this.tileSetGridBox.Click += new System.EventHandler(this.tileSetGridBox_Click);
            // 
            // selectedMetaTilePanel
            // 
            this.selectedMetaTilePanel.AutoScroll = true;
            this.selectedMetaTilePanel.Controls.Add(this.selectedMetaTileBox);
            this.selectedMetaTilePanel.Location = new System.Drawing.Point(32, 12);
            this.selectedMetaTilePanel.Name = "selectedMetaTilePanel";
            this.selectedMetaTilePanel.Size = new System.Drawing.Size(64, 64);
            this.selectedMetaTilePanel.TabIndex = 2;
            // 
            // selectedMetaTileBox
            // 
            this.selectedMetaTileBox.Location = new System.Drawing.Point(0, 0);
            this.selectedMetaTileBox.Name = "selectedMetaTileBox";
            this.selectedMetaTileBox.Size = new System.Drawing.Size(32, 32);
            this.selectedMetaTileBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.selectedMetaTileBox.TabIndex = 1;
            this.selectedMetaTileBox.TabStop = false;
            this.selectedMetaTileBox.Click += new System.EventHandler(this.selectedMetaTileBox_Click);
            // 
            // selectedTilePanel
            // 
            this.selectedTilePanel.AutoScroll = true;
            this.selectedTilePanel.Controls.Add(this.selectedTileBox);
            this.selectedTilePanel.Location = new System.Drawing.Point(12, 102);
            this.selectedTilePanel.Name = "selectedTilePanel";
            this.selectedTilePanel.Size = new System.Drawing.Size(32, 32);
            this.selectedTilePanel.TabIndex = 3;
            // 
            // selectedTileBox
            // 
            this.selectedTileBox.Location = new System.Drawing.Point(0, 0);
            this.selectedTileBox.Name = "selectedTileBox";
            this.selectedTileBox.Size = new System.Drawing.Size(16, 16);
            this.selectedTileBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.selectedTileBox.TabIndex = 1;
            this.selectedTileBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(0, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 2);
            this.label1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(409, -5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2, 300);
            this.label2.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(128, -5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(2, 300);
            this.label3.TabIndex = 6;
            // 
            // tLPalette
            // 
            this.tLPalette.Location = new System.Drawing.Point(12, 0);
            this.tLPalette.MaxLength = 1;
            this.tLPalette.Name = "tLPalette";
            this.tLPalette.Size = new System.Drawing.Size(13, 20);
            this.tLPalette.TabIndex = 7;
            this.tLPalette.Text = "0";
            this.tLPalette.LostFocus += new System.EventHandler(this.tLPalette_LostFocus);
            // 
            // bLPalette
            // 
            this.bLPalette.Location = new System.Drawing.Point(12, 72);
            this.bLPalette.MaxLength = 1;
            this.bLPalette.Name = "bLPalette";
            this.bLPalette.Size = new System.Drawing.Size(13, 20);
            this.bLPalette.TabIndex = 8;
            this.bLPalette.Text = "0";
            this.bLPalette.LostFocus += new System.EventHandler(this.bLPalette_LostFocus);
            // 
            // tRPalette
            // 
            this.tRPalette.Location = new System.Drawing.Point(102, 0);
            this.tRPalette.MaxLength = 1;
            this.tRPalette.Name = "tRPalette";
            this.tRPalette.Size = new System.Drawing.Size(13, 20);
            this.tRPalette.TabIndex = 9;
            this.tRPalette.Text = "0";
            this.tRPalette.LostFocus += new System.EventHandler(this.tRPalette_LostFocus);
            // 
            // bRPalette
            // 
            this.bRPalette.Location = new System.Drawing.Point(102, 72);
            this.bRPalette.MaxLength = 1;
            this.bRPalette.Name = "bRPalette";
            this.bRPalette.Size = new System.Drawing.Size(13, 20);
            this.bRPalette.TabIndex = 10;
            this.bRPalette.Text = "0";
            this.bRPalette.LostFocus += new System.EventHandler(this.bRPalette_LostFocus);
            // 
            // hFlipBox
            // 
            this.hFlipBox.AutoSize = true;
            this.hFlipBox.Location = new System.Drawing.Point(12, 140);
            this.hFlipBox.Name = "hFlipBox";
            this.hFlipBox.Size = new System.Drawing.Size(92, 17);
            this.hFlipBox.TabIndex = 1;
            this.hFlipBox.Text = "Horizontal Flip";
            this.hFlipBox.UseVisualStyleBackColor = true;
            this.hFlipBox.CheckedChanged += new System.EventHandler(this.hFlip_CheckedChanged);
            // 
            // vFlipBox
            // 
            this.vFlipBox.AutoSize = true;
            this.vFlipBox.Location = new System.Drawing.Point(12, 163);
            this.vFlipBox.Name = "vFlipBox";
            this.vFlipBox.Size = new System.Drawing.Size(80, 17);
            this.vFlipBox.TabIndex = 11;
            this.vFlipBox.Text = "Vertical Flip";
            this.vFlipBox.UseVisualStyleBackColor = true;
            this.vFlipBox.CheckedChanged += new System.EventHandler(this.vFlipBox_CheckedChanged);
            // 
            // prevButton
            // 
            this.prevButton.Enabled = false;
            this.prevButton.Location = new System.Drawing.Point(11, 227);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(17, 23);
            this.prevButton.TabIndex = 12;
            this.prevButton.Text = "<";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(53, 227);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(17, 23);
            this.nextButton.TabIndex = 13;
            this.nextButton.Text = ">";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // PaletteNum
            // 
            this.PaletteNum.Location = new System.Drawing.Point(34, 232);
            this.PaletteNum.Name = "PaletteNum";
            this.PaletteNum.Size = new System.Drawing.Size(13, 13);
            this.PaletteNum.TabIndex = 14;
            this.PaletteNum.Text = "0";
            // 
            // layer1Button
            // 
            this.layer1Button.Enabled = false;
            this.layer1Button.Location = new System.Drawing.Point(76, 227);
            this.layer1Button.Name = "layer1Button";
            this.layer1Button.Size = new System.Drawing.Size(17, 23);
            this.layer1Button.TabIndex = 15;
            this.layer1Button.Text = "1";
            this.layer1Button.UseVisualStyleBackColor = true;
            this.layer1Button.Click += new System.EventHandler(this.layer1Button_Click);
            // 
            // layer2Button
            // 
            this.layer2Button.Location = new System.Drawing.Point(99, 227);
            this.layer2Button.Name = "layer2Button";
            this.layer2Button.Size = new System.Drawing.Size(17, 23);
            this.layer2Button.TabIndex = 16;
            this.layer2Button.Text = "2";
            this.layer2Button.UseVisualStyleBackColor = true;
            this.layer2Button.Click += new System.EventHandler(this.layer2Button_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Palette";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(78, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Layer";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 36);
            this.button1.TabIndex = 19;
            this.button1.Text = "Unfocus";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tileChange
            // 
            this.tileChange.Location = new System.Drawing.Point(11, 186);
            this.tileChange.Name = "tileChange";
            this.tileChange.Size = new System.Drawing.Size(114, 22);
            this.tileChange.TabIndex = 20;
            this.tileChange.Text = "Apply Tile Change";
            this.tileChange.UseVisualStyleBackColor = true;
            this.tileChange.Click += new System.EventHandler(this.tileChange_Click);
            // 
            // tId1
            // 
            this.tId1.Enabled = false;
            this.tId1.Location = new System.Drawing.Point(0, 24);
            this.tId1.MaxLength = 3;
            this.tId1.Name = "tId1";
            this.tId1.Size = new System.Drawing.Size(25, 20);
            this.tId1.TabIndex = 21;
            this.tId1.Text = "FFF";
            // 
            // tId2
            // 
            this.tId2.Enabled = false;
            this.tId2.Location = new System.Drawing.Point(102, 24);
            this.tId2.MaxLength = 3;
            this.tId2.Name = "tId2";
            this.tId2.Size = new System.Drawing.Size(25, 20);
            this.tId2.TabIndex = 22;
            this.tId2.Text = "FFF";
            // 
            // tId3
            // 
            this.tId3.Enabled = false;
            this.tId3.Location = new System.Drawing.Point(0, 50);
            this.tId3.MaxLength = 3;
            this.tId3.Name = "tId3";
            this.tId3.Size = new System.Drawing.Size(25, 20);
            this.tId3.TabIndex = 23;
            this.tId3.Text = "FFF";
            // 
            // tId4
            // 
            this.tId4.Enabled = false;
            this.tId4.Location = new System.Drawing.Point(102, 50);
            this.tId4.MaxLength = 3;
            this.tId4.Name = "tId4";
            this.tId4.Size = new System.Drawing.Size(25, 20);
            this.tId4.TabIndex = 24;
            this.tId4.Text = "FFF";
            // 
            // sTId
            // 
            this.sTId.Enabled = false;
            this.sTId.Location = new System.Drawing.Point(45, 111);
            this.sTId.MaxLength = 3;
            this.sTId.Name = "sTId";
            this.sTId.Size = new System.Drawing.Size(25, 20);
            this.sTId.TabIndex = 25;
            this.sTId.Text = "FFF";
            // 
            // mTId
            // 
            this.mTId.Enabled = false;
            this.mTId.Location = new System.Drawing.Point(53, 76);
            this.mTId.MaxLength = 3;
            this.mTId.Name = "mTId";
            this.mTId.Size = new System.Drawing.Size(25, 20);
            this.mTId.TabIndex = 26;
            this.mTId.Text = "FFF";
            // 
            // MetaTileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 262);
            this.Controls.Add(this.mTId);
            this.Controls.Add(this.sTId);
            this.Controls.Add(this.tId4);
            this.Controls.Add(this.tId3);
            this.Controls.Add(this.tId2);
            this.Controls.Add(this.tId1);
            this.Controls.Add(this.tileChange);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.layer2Button);
            this.Controls.Add(this.layer1Button);
            this.Controls.Add(this.PaletteNum);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.vFlipBox);
            this.Controls.Add(this.hFlipBox);
            this.Controls.Add(this.bRPalette);
            this.Controls.Add(this.tRPalette);
            this.Controls.Add(this.bLPalette);
            this.Controls.Add(this.tLPalette);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectedTilePanel);
            this.Controls.Add(this.selectedMetaTilePanel);
            this.Controls.Add(this.tileSetPanel);
            this.Controls.Add(this.metaTileSetPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetaTileEditor";
            this.Text = "Metatile Editor";
            this.metaTileSetPanel.ResumeLayout(false);
            this.metaTileSetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metaTileGridBox)).EndInit();
            this.tileSetPanel.ResumeLayout(false);
            this.tileSetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetGridBox)).EndInit();
            this.selectedMetaTilePanel.ResumeLayout(false);
            this.selectedMetaTilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedMetaTileBox)).EndInit();
            this.selectedTilePanel.ResumeLayout(false);
            this.selectedTilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedTileBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Panel metaTileSetPanel;
		private System.Windows.Forms.Panel tileSetPanel;
		private System.Windows.Forms.Panel selectedMetaTilePanel;
		private System.Windows.Forms.PictureBox selectedMetaTileBox;
		private System.Windows.Forms.Panel selectedTilePanel;
		private System.Windows.Forms.PictureBox selectedTileBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tLPalette;
		private System.Windows.Forms.TextBox bLPalette;
		private System.Windows.Forms.TextBox tRPalette;
		private System.Windows.Forms.TextBox bRPalette;
		private System.Windows.Forms.CheckBox hFlipBox;
		private System.Windows.Forms.CheckBox vFlipBox;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Label PaletteNum;
		private System.Windows.Forms.Button layer1Button;
		private System.Windows.Forms.Button layer2Button;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button tileChange;
		private System.Windows.Forms.TextBox tId1;
		private System.Windows.Forms.TextBox tId2;
		private System.Windows.Forms.TextBox tId3;
		private System.Windows.Forms.TextBox tId4;
		private System.Windows.Forms.TextBox sTId;
		private System.Windows.Forms.TextBox mTId;
        private GridBox metaTileGridBox;
        private GridBox tileSetGridBox;
    }
}