namespace MinishMaker.UI
{
	partial class EnemyPlacementEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnemyPlacementEditor));
            this.removeButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.indexLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.objectType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.objectSub = new System.Windows.Forms.TextBox();
            this.posY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.posX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.subId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.unknown2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.unknown1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.unknown4 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.unknown3 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.enemyId = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(109, 12);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(20, 20);
            this.removeButton.TabIndex = 25;
            this.removeButton.Text = "-";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(83, 12);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(20, 20);
            this.newButton.TabIndex = 24;
            this.newButton.Text = "+";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(12, 12);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(20, 20);
            this.prevButton.TabIndex = 23;
            this.prevButton.Text = "<";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(57, 12);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(20, 20);
            this.nextButton.TabIndex = 22;
            this.nextButton.Text = ">";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // indexLabel
            // 
            this.indexLabel.AutoSize = true;
            this.indexLabel.Location = new System.Drawing.Point(38, 16);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Size = new System.Drawing.Size(0, 13);
            this.indexLabel.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Object Type 0x";
            // 
            // objectType
            // 
            this.objectType.Enabled = false;
            this.objectType.Location = new System.Drawing.Point(83, 51);
            this.objectType.MaxLength = 2;
            this.objectType.Name = "objectType";
            this.objectType.Size = new System.Drawing.Size(23, 20);
            this.objectType.TabIndex = 28;
            this.objectType.Text = "FF";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Object Sub  0x";
            // 
            // objectSub
            // 
            this.objectSub.Enabled = false;
            this.objectSub.Location = new System.Drawing.Point(83, 73);
            this.objectSub.MaxLength = 2;
            this.objectSub.Name = "objectSub";
            this.objectSub.Size = new System.Drawing.Size(23, 20);
            this.objectSub.TabIndex = 30;
            this.objectSub.Text = "FF";
            // 
            // posY
            // 
            this.posY.Location = new System.Drawing.Point(83, 121);
            this.posY.MaxLength = 4;
            this.posY.Name = "posY";
            this.posY.Size = new System.Drawing.Size(32, 20);
            this.posY.TabIndex = 34;
            this.posY.Text = "FFFF";
            this.posY.TextChanged += new System.EventHandler(this.posY_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Pos Y (pixel) 0x";
            // 
            // posX
            // 
            this.posX.Location = new System.Drawing.Point(83, 99);
            this.posX.MaxLength = 4;
            this.posX.Name = "posX";
            this.posX.Size = new System.Drawing.Size(32, 20);
            this.posX.TabIndex = 32;
            this.posX.Text = "FFFF";
            this.posX.TextChanged += new System.EventHandler(this.posX_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Pos X (pixel) 0x";
            // 
            // subId
            // 
            this.subId.Location = new System.Drawing.Point(191, 73);
            this.subId.MaxLength = 2;
            this.subId.Name = "subId";
            this.subId.Size = new System.Drawing.Size(23, 20);
            this.subId.TabIndex = 38;
            this.subId.Text = "FF";
            this.subId.TextChanged += new System.EventHandler(this.subId_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 37;
            this.label5.Text = "Enemy Sub Id 0x";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Enemy Id";
            // 
            // unknown2
            // 
            this.unknown2.Enabled = false;
            this.unknown2.Location = new System.Drawing.Point(83, 169);
            this.unknown2.MaxLength = 4;
            this.unknown2.Name = "unknown2";
            this.unknown2.Size = new System.Drawing.Size(32, 20);
            this.unknown2.TabIndex = 42;
            this.unknown2.Text = "FFFF";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Unknown2   0x";
            // 
            // unknown1
            // 
            this.unknown1.Enabled = false;
            this.unknown1.Location = new System.Drawing.Point(83, 147);
            this.unknown1.MaxLength = 4;
            this.unknown1.Name = "unknown1";
            this.unknown1.Size = new System.Drawing.Size(32, 20);
            this.unknown1.TabIndex = 40;
            this.unknown1.Text = "FFFF";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 150);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 39;
            this.label8.Text = "Unknown1   0x";
            // 
            // unknown4
            // 
            this.unknown4.Enabled = false;
            this.unknown4.Location = new System.Drawing.Point(83, 213);
            this.unknown4.MaxLength = 4;
            this.unknown4.Name = "unknown4";
            this.unknown4.Size = new System.Drawing.Size(32, 20);
            this.unknown4.TabIndex = 46;
            this.unknown4.Text = "FFFF";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 216);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 13);
            this.label9.TabIndex = 45;
            this.label9.Text = "Unknown4   0x";
            // 
            // unknown3
            // 
            this.unknown3.Enabled = false;
            this.unknown3.Location = new System.Drawing.Point(83, 191);
            this.unknown3.MaxLength = 4;
            this.unknown3.Name = "unknown3";
            this.unknown3.Size = new System.Drawing.Size(32, 20);
            this.unknown3.TabIndex = 44;
            this.unknown3.Text = "FFFF";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 194);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 43;
            this.label10.Text = "Unknown3   0x";
            // 
            // enemyId
            // 
            this.enemyId.FormattingEnabled = true;
            this.enemyId.Location = new System.Drawing.Point(166, 48);
            this.enemyId.Name = "enemyId";
            this.enemyId.Size = new System.Drawing.Size(121, 21);
            this.enemyId.TabIndex = 47;
            this.enemyId.SelectedIndexChanged += new System.EventHandler(this.enemyId_SelectedIndexChanged);
            // 
            // EnemyPlacementEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 242);
            this.Controls.Add(this.enemyId);
            this.Controls.Add(this.unknown4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.unknown3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.unknown2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.unknown1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.subId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.posY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.posX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.objectSub);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.objectType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.indexLabel);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.nextButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnemyPlacementEditor";
            this.Text = "EnemyPlacementEditor";
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
		private System.Windows.Forms.TextBox objectType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox objectSub;
		private System.Windows.Forms.TextBox posY;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox posX;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox subId;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox unknown2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox unknown1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox unknown4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox unknown3;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox enemyId;
	}
}