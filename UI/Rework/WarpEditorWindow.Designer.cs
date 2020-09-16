namespace MinishMaker.UI.Rework
{
	partial class WarpEditorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarpEditorWindow));
            this.removeButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.indexLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.warpX = new System.Windows.Forms.TextBox();
            this.warpTypeBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.warpY = new System.Windows.Forms.TextBox();
            this.destY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.destX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.warpShape = new System.Windows.Forms.TextBox();
            this.destArea = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.destRoom = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.exitHeight = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.transitionTypeBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.facingBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.soundId = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.topLeftCheck = new System.Windows.Forms.CheckBox();
            this.topRightCheck = new System.Windows.Forms.CheckBox();
            this.bottomRightCheck = new System.Windows.Forms.CheckBox();
            this.bottomLeftCheck = new System.Windows.Forms.CheckBox();
            this.leftTopCheck = new System.Windows.Forms.CheckBox();
            this.leftBottomCheck = new System.Windows.Forms.CheckBox();
            this.rightBottomCheck = new System.Windows.Forms.CheckBox();
            this.rightTopCheck = new System.Windows.Forms.CheckBox();
            this.borderPanel = new System.Windows.Forms.Panel();
            this.blockImage = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.areaPanel = new System.Windows.Forms.Panel();
            this.travelButton = new System.Windows.Forms.Button();
            this.borderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockImage)).BeginInit();
            this.areaPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(109, 12);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(20, 20);
            this.removeButton.TabIndex = 26;
            this.removeButton.Text = "-";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.RemoveButton_Click); });
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(83, 12);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(20, 20);
            this.newButton.TabIndex = 25;
            this.newButton.Text = "+";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.NewButton_Click); });
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(12, 12);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(20, 20);
            this.prevButton.TabIndex = 24;
            this.prevButton.Text = "<";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(57, 12);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(20, 20);
            this.nextButton.TabIndex = 23;
            this.nextButton.Text = ">";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // indexLabel
            // 
            this.indexLabel.AutoSize = true;
            this.indexLabel.Location = new System.Drawing.Point(35, 16);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Size = new System.Drawing.Size(0, 13);
            this.indexLabel.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Warp Type:";
            // 
            // warpX
            // 
            this.warpX.Location = new System.Drawing.Point(74, 0);
            this.warpX.MaxLength = 4;
            this.warpX.Name = "warpX";
            this.warpX.Size = new System.Drawing.Size(30, 20);
            this.warpX.TabIndex = 28;
            this.warpX.Text = "FFFF";
            this.warpX.Visible = false;
            this.warpX.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.WarpX_TextChanged); });
            // 
            // warpTypeBox
            // 
            this.warpTypeBox.FormattingEnabled = true;
            this.warpTypeBox.Location = new System.Drawing.Point(109, 42);
            this.warpTypeBox.Name = "warpTypeBox";
            this.warpTypeBox.Size = new System.Drawing.Size(121, 21);
            this.warpTypeBox.TabIndex = 29;
            this.warpTypeBox.SelectedIndexChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.WarpTypeBox_SelectedIndexChanged); });
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Warp X pixel:";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Warp Y pixel:";
            this.label3.Visible = false;
            // 
            // warpY
            // 
            this.warpY.Location = new System.Drawing.Point(74, 20);
            this.warpY.MaxLength = 4;
            this.warpY.Name = "warpY";
            this.warpY.Size = new System.Drawing.Size(30, 20);
            this.warpY.TabIndex = 32;
            this.warpY.Text = "FFFF";
            this.warpY.Visible = false;
            this.warpY.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.WarpY_TextChanged); });
            // 
            // destY
            // 
            this.destY.Location = new System.Drawing.Point(109, 144);
            this.destY.MaxLength = 4;
            this.destY.Name = "destY";
            this.destY.Size = new System.Drawing.Size(30, 20);
            this.destY.TabIndex = 36;
            this.destY.Text = "FFFF";
            this.destY.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.DestY_TextChanged); });
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Destination Y pixel:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Destination X pixel:";
            // 
            // destX
            // 
            this.destX.Location = new System.Drawing.Point(109, 124);
            this.destX.MaxLength = 4;
            this.destX.Name = "destX";
            this.destX.Size = new System.Drawing.Size(30, 20);
            this.destX.TabIndex = 33;
            this.destX.Text = "FFFF";
            this.destX.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.DestX_TextChanged); });
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Warp Shape:";
            this.label7.Visible = false;
            // 
            // warpShape
            // 
            this.warpShape.Location = new System.Drawing.Point(74, 40);
            this.warpShape.MaxLength = 2;
            this.warpShape.Name = "warpShape";
            this.warpShape.Size = new System.Drawing.Size(20, 20);
            this.warpShape.TabIndex = 37;
            this.warpShape.Text = "FF";
            this.warpShape.Visible = false;
            this.warpShape.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.WarpShape_TextChanged); });
            // 
            // destArea
            // 
            this.destArea.Location = new System.Drawing.Point(109, 164);
            this.destArea.MaxLength = 4;
            this.destArea.Name = "destArea";
            this.destArea.Size = new System.Drawing.Size(20, 20);
            this.destArea.TabIndex = 40;
            this.destArea.Text = "FF";
            this.destArea.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.DestArea_TextChanged); });
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Destination Area:";
            // 
            // destRoom
            // 
            this.destRoom.Location = new System.Drawing.Point(109, 184);
            this.destRoom.MaxLength = 4;
            this.destRoom.Name = "destRoom";
            this.destRoom.Size = new System.Drawing.Size(20, 20);
            this.destRoom.TabIndex = 42;
            this.destRoom.Text = "FF";
            this.destRoom.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.DestRoom_TextChanged); });
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 41;
            this.label8.Text = "Destination Room:";
            // 
            // exitHeight
            // 
            this.exitHeight.Location = new System.Drawing.Point(109, 203);
            this.exitHeight.MaxLength = 2;
            this.exitHeight.Name = "exitHeight";
            this.exitHeight.Size = new System.Drawing.Size(20, 20);
            this.exitHeight.TabIndex = 44;
            this.exitHeight.Text = "FF";
            this.exitHeight.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.ExitHeight_TextChanged); });
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(48, 206);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Exit Height:";
            // 
            // transitionTypeBox
            // 
            this.transitionTypeBox.FormattingEnabled = true;
            this.transitionTypeBox.Location = new System.Drawing.Point(109, 68);
            this.transitionTypeBox.Name = "transitionTypeBox";
            this.transitionTypeBox.Size = new System.Drawing.Size(121, 21);
            this.transitionTypeBox.TabIndex = 46;
            this.transitionTypeBox.SelectedIndexChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.TransitionTypeBox_SelectedIndexChanged); });
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 45;
            this.label10.Text = "Transition Type:";
            // 
            // facingBox
            // 
            this.facingBox.FormattingEnabled = true;
            this.facingBox.Location = new System.Drawing.Point(109, 95);
            this.facingBox.Name = "facingBox";
            this.facingBox.Size = new System.Drawing.Size(121, 21);
            this.facingBox.TabIndex = 48;
            this.facingBox.SelectedIndexChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.FacingBox_SelectedIndexChanged); });
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(67, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Facing:";
            // 
            // soundId
            // 
            this.soundId.Location = new System.Drawing.Point(109, 224);
            this.soundId.MaxLength = 4;
            this.soundId.Name = "soundId";
            this.soundId.Size = new System.Drawing.Size(30, 20);
            this.soundId.TabIndex = 50;
            this.soundId.Text = "FFFF";
            this.soundId.TextChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.SoundId_TextChanged); });
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(56, 227);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 49;
            this.label12.Text = "Sound Id:";
            // 
            // topLeftCheck
            // 
            this.topLeftCheck.AutoSize = true;
            this.topLeftCheck.Location = new System.Drawing.Point(31, 23);
            this.topLeftCheck.Name = "topLeftCheck";
            this.topLeftCheck.Size = new System.Drawing.Size(15, 14);
            this.topLeftCheck.TabIndex = 51;
            this.topLeftCheck.UseVisualStyleBackColor = true;
            this.topLeftCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // topRightCheck
            // 
            this.topRightCheck.AutoSize = true;
            this.topRightCheck.Location = new System.Drawing.Point(48, 23);
            this.topRightCheck.Name = "topRightCheck";
            this.topRightCheck.Size = new System.Drawing.Size(15, 14);
            this.topRightCheck.TabIndex = 52;
            this.topRightCheck.UseVisualStyleBackColor = true;
            this.topRightCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // bottomRightCheck
            // 
            this.bottomRightCheck.AutoSize = true;
            this.bottomRightCheck.Location = new System.Drawing.Point(48, 81);
            this.bottomRightCheck.Name = "bottomRightCheck";
            this.bottomRightCheck.Size = new System.Drawing.Size(15, 14);
            this.bottomRightCheck.TabIndex = 54;
            this.bottomRightCheck.UseVisualStyleBackColor = true;
            this.bottomRightCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // bottomLeftCheck
            // 
            this.bottomLeftCheck.AutoSize = true;
            this.bottomLeftCheck.Location = new System.Drawing.Point(31, 81);
            this.bottomLeftCheck.Name = "bottomLeftCheck";
            this.bottomLeftCheck.Size = new System.Drawing.Size(15, 14);
            this.bottomLeftCheck.TabIndex = 53;
            this.bottomLeftCheck.UseVisualStyleBackColor = true;
            this.bottomLeftCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // leftTopCheck
            // 
            this.leftTopCheck.AutoSize = true;
            this.leftTopCheck.Location = new System.Drawing.Point(10, 43);
            this.leftTopCheck.Name = "leftTopCheck";
            this.leftTopCheck.Size = new System.Drawing.Size(15, 14);
            this.leftTopCheck.TabIndex = 55;
            this.leftTopCheck.UseVisualStyleBackColor = true;
            this.leftTopCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // leftBottomCheck
            // 
            this.leftBottomCheck.AutoSize = true;
            this.leftBottomCheck.Location = new System.Drawing.Point(10, 62);
            this.leftBottomCheck.Name = "leftBottomCheck";
            this.leftBottomCheck.Size = new System.Drawing.Size(15, 14);
            this.leftBottomCheck.TabIndex = 56;
            this.leftBottomCheck.UseVisualStyleBackColor = true;
            this.leftBottomCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // rightBottomCheck
            // 
            this.rightBottomCheck.AutoSize = true;
            this.rightBottomCheck.Location = new System.Drawing.Point(69, 61);
            this.rightBottomCheck.Name = "rightBottomCheck";
            this.rightBottomCheck.Size = new System.Drawing.Size(15, 14);
            this.rightBottomCheck.TabIndex = 58;
            this.rightBottomCheck.UseVisualStyleBackColor = true;
            this.rightBottomCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // rightTopCheck
            // 
            this.rightTopCheck.AutoSize = true;
            this.rightTopCheck.Location = new System.Drawing.Point(69, 43);
            this.rightTopCheck.Name = "rightTopCheck";
            this.rightTopCheck.Size = new System.Drawing.Size(15, 14);
            this.rightTopCheck.TabIndex = 57;
            this.rightTopCheck.UseVisualStyleBackColor = true;
            this.rightTopCheck.CheckedChanged += new System.EventHandler((object o, System.EventArgs e) => { this.ChangedHandler((System.Action)this.CheckboxChanged); });
            // 
            // borderPanel
            // 
            this.borderPanel.Controls.Add(this.blockImage);
            this.borderPanel.Controls.Add(this.label13);
            this.borderPanel.Controls.Add(this.rightTopCheck);
            this.borderPanel.Controls.Add(this.rightBottomCheck);
            this.borderPanel.Controls.Add(this.bottomRightCheck);
            this.borderPanel.Controls.Add(this.leftBottomCheck);
            this.borderPanel.Controls.Add(this.bottomLeftCheck);
            this.borderPanel.Controls.Add(this.leftTopCheck);
            this.borderPanel.Controls.Add(this.topLeftCheck);
            this.borderPanel.Controls.Add(this.topRightCheck);
            this.borderPanel.Location = new System.Drawing.Point(147, 122);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Size = new System.Drawing.Size(88, 100);
            this.borderPanel.TabIndex = 59;
            // 
            // blockImage
            // 
            this.blockImage.Location = new System.Drawing.Point(31, 43);
            this.blockImage.Name = "blockImage";
            this.blockImage.Size = new System.Drawing.Size(32, 32);
            this.blockImage.TabIndex = 61;
            this.blockImage.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 6);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 60;
            this.label13.Text = "Active Border";
            // 
            // areaPanel
            // 
            this.areaPanel.Controls.Add(this.label2);
            this.areaPanel.Controls.Add(this.warpX);
            this.areaPanel.Controls.Add(this.label3);
            this.areaPanel.Controls.Add(this.warpY);
            this.areaPanel.Controls.Add(this.warpShape);
            this.areaPanel.Controls.Add(this.label7);
            this.areaPanel.Location = new System.Drawing.Point(147, 122);
            this.areaPanel.Name = "areaPanel";
            this.areaPanel.Size = new System.Drawing.Size(109, 62);
            this.areaPanel.TabIndex = 60;
            //
            // travelButton
            // 
            this.travelButton.Location = new System.Drawing.Point(163, 223);
            this.travelButton.Name = "travelButton";
            this.travelButton.Size = new System.Drawing.Size(57, 20);
            this.travelButton.TabIndex = 61;
            this.travelButton.Text = "To room";
            this.travelButton.UseVisualStyleBackColor = true;
            this.travelButton.Click += new System.EventHandler(this.TravelButton_Click);
            // 
            // WarpEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 252);
            this.Controls.Add(this.areaPanel);
            this.Controls.Add(this.travelButton);
            this.Controls.Add(this.borderPanel);
            this.Controls.Add(this.soundId);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.facingBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.transitionTypeBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.exitHeight);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.destRoom);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.destArea);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.destY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.destX);
            this.Controls.Add(this.warpTypeBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.indexLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarpEditor";
            this.Text = "WarpEditor";
            this.borderPanel.ResumeLayout(false);
            this.borderPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockImage)).EndInit();
            this.areaPanel.ResumeLayout(false);
            this.areaPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Label indexLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox warpX;
		private System.Windows.Forms.ComboBox warpTypeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox warpY;
		private System.Windows.Forms.TextBox destY;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox destX;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox warpShape;
		private System.Windows.Forms.TextBox destArea;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox destRoom;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox exitHeight;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox transitionTypeBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox facingBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox soundId;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox topLeftCheck;
		private System.Windows.Forms.CheckBox topRightCheck;
		private System.Windows.Forms.CheckBox bottomRightCheck;
		private System.Windows.Forms.CheckBox bottomLeftCheck;
		private System.Windows.Forms.CheckBox leftTopCheck;
		private System.Windows.Forms.CheckBox leftBottomCheck;
		private System.Windows.Forms.CheckBox rightBottomCheck;
		private System.Windows.Forms.CheckBox rightTopCheck;
		private System.Windows.Forms.Panel borderPanel;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Panel areaPanel;
		private System.Windows.Forms.PictureBox blockImage;
		private System.Windows.Forms.Button travelButton;
	}
}
