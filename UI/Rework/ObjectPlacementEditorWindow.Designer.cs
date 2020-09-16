namespace MinishMaker.UI.Rework
{
	partial class ObjectPlacementEditorWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectPlacementEditorWindow));
			this.indexLabel = new System.Windows.Forms.Label();
			this.removeButton = new System.Windows.Forms.Button();
			this.newButton = new System.Windows.Forms.Button();
			this.prevButton = new System.Windows.Forms.Button();
			this.nextButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.unknownBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.data1Box = new System.Windows.Forms.TextBox();
			this.data1Label = new System.Windows.Forms.Label();
			this.data2Box = new System.Windows.Forms.TextBox();
			this.data2Label = new System.Windows.Forms.Label();
			this.data3Box = new System.Windows.Forms.TextBox();
			this.data3Label = new System.Windows.Forms.Label();
			this.data4Box = new System.Windows.Forms.TextBox();
			this.data4Label = new System.Windows.Forms.Label();
			this.data5Box = new System.Windows.Forms.TextBox();
			this.data5Label = new System.Windows.Forms.Label();
			this.posYBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.posXBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.flag2Box = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.flag1Box = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.objectIdBox = new System.Windows.Forms.ComboBox();
			this.objectIdValue = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.listIndexLabel = new System.Windows.Forms.Label();
			this.prevListButton = new System.Windows.Forms.Button();
			this.nextListButton = new System.Windows.Forms.Button();
			this.objectTypeBox = new System.Windows.Forms.ComboBox();
			this.objectTypeValue = new System.Windows.Forms.TextBox();
			this.copyButton = new System.Windows.Forms.Button();
			this.pasteButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// indexLabel
			// 
			this.indexLabel.AutoSize = true;
			this.indexLabel.Location = new System.Drawing.Point(32, 23);
			this.indexLabel.Name = "indexLabel";
			this.indexLabel.Size = new System.Drawing.Size(0, 13);
			this.indexLabel.TabIndex = 31;
			// 
			// removeButton
			// 
			this.removeButton.Enabled = false;
			this.removeButton.Location = new System.Drawing.Point(103, 19);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(20, 20);
			this.removeButton.TabIndex = 30;
			this.removeButton.Text = "-";
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// newButton
			// 
			this.newButton.Enabled = false;
			this.newButton.Location = new System.Drawing.Point(77, 19);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(20, 20);
			this.newButton.TabIndex = 29;
			this.newButton.Text = "+";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// prevButton
			// 
			this.prevButton.Enabled = false;
			this.prevButton.Location = new System.Drawing.Point(6, 19);
			this.prevButton.Name = "prevButton";
			this.prevButton.Size = new System.Drawing.Size(20, 20);
			this.prevButton.TabIndex = 28;
			this.prevButton.Text = "<";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.Enabled = false;
			this.nextButton.Location = new System.Drawing.Point(51, 19);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(20, 20);
			this.nextButton.TabIndex = 27;
			this.nextButton.Text = ">";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 32;
			this.label1.Text = "Object type:";
			// 
			// unknownBox
			// 
			this.unknownBox.Enabled = false;
			this.unknownBox.Location = new System.Drawing.Point(249, 45);
			this.unknownBox.MaxLength = 2;
			this.unknownBox.Name = "unknownBox";
			this.unknownBox.Size = new System.Drawing.Size(23, 20);
			this.unknownBox.TabIndex = 36;
			this.unknownBox.Text = "FF";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(192, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 13);
			this.label2.TabIndex = 35;
			this.label2.Text = "Object sub:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 37;
			this.label3.Text = "Object id:";
			// 
			// data1Box
			// 
			this.data1Box.Enabled = false;
			this.data1Box.Location = new System.Drawing.Point(64, 105);
			this.data1Box.MaxLength = 2;
			this.data1Box.Name = "data1Box";
			this.data1Box.Size = new System.Drawing.Size(23, 20);
			this.data1Box.TabIndex = 40;
			this.data1Box.Text = "FF";
			this.data1Box.TextChanged += new System.EventHandler(this.data1Box_TextChanged);
			// 
			// data1Label
			// 
			this.data1Label.AutoSize = true;
			this.data1Label.Location = new System.Drawing.Point(3, 108);
			this.data1Label.Name = "data1Label";
			this.data1Label.Size = new System.Drawing.Size(42, 13);
			this.data1Label.TabIndex = 39;
			this.data1Label.Text = "Data 1:";
			// 
			// data2Box
			// 
			this.data2Box.Enabled = false;
			this.data2Box.Location = new System.Drawing.Point(64, 125);
			this.data2Box.MaxLength = 2;
			this.data2Box.Name = "data2Box";
			this.data2Box.Size = new System.Drawing.Size(23, 20);
			this.data2Box.TabIndex = 42;
			this.data2Box.Text = "FF";
			this.data2Box.TextChanged += new System.EventHandler(this.data2Box_TextChanged);
			// 
			// data2Label
			// 
			this.data2Label.AutoSize = true;
			this.data2Label.Location = new System.Drawing.Point(3, 128);
			this.data2Label.Name = "data2Label";
			this.data2Label.Size = new System.Drawing.Size(42, 13);
			this.data2Label.TabIndex = 41;
			this.data2Label.Text = "Data 2:";
			// 
			// data3Box
			// 
			this.data3Box.Enabled = false;
			this.data3Box.Location = new System.Drawing.Point(64, 145);
			this.data3Box.MaxLength = 2;
			this.data3Box.Name = "data3Box";
			this.data3Box.Size = new System.Drawing.Size(23, 20);
			this.data3Box.TabIndex = 44;
			this.data3Box.Text = "FF";
			this.data3Box.TextChanged += new System.EventHandler(this.data3Box_TextChanged);
			// 
			// data3Label
			// 
			this.data3Label.AutoSize = true;
			this.data3Label.Location = new System.Drawing.Point(3, 148);
			this.data3Label.Name = "data3Label";
			this.data3Label.Size = new System.Drawing.Size(42, 13);
			this.data3Label.TabIndex = 43;
			this.data3Label.Text = "Data 3:";
			// 
			// data4Box
			// 
			this.data4Box.Enabled = false;
			this.data4Box.Location = new System.Drawing.Point(64, 165);
			this.data4Box.MaxLength = 2;
			this.data4Box.Name = "data4Box";
			this.data4Box.Size = new System.Drawing.Size(23, 20);
			this.data4Box.TabIndex = 46;
			this.data4Box.Text = "FF";
			this.data4Box.TextChanged += new System.EventHandler(this.data4Box_TextChanged);
			// 
			// data4Label
			// 
			this.data4Label.AutoSize = true;
			this.data4Label.Location = new System.Drawing.Point(3, 168);
			this.data4Label.Name = "data4Label";
			this.data4Label.Size = new System.Drawing.Size(42, 13);
			this.data4Label.TabIndex = 45;
			this.data4Label.Text = "Data 4:";
			// 
			// data5Box
			// 
			this.data5Box.Enabled = false;
			this.data5Box.Location = new System.Drawing.Point(64, 185);
			this.data5Box.MaxLength = 2;
			this.data5Box.Name = "data5Box";
			this.data5Box.Size = new System.Drawing.Size(23, 20);
			this.data5Box.TabIndex = 48;
			this.data5Box.Text = "FF";
			this.data5Box.TextChanged += new System.EventHandler(this.data5Box_TextChanged);
			// 
			// data5Label
			// 
			this.data5Label.AutoSize = true;
			this.data5Label.Location = new System.Drawing.Point(3, 188);
			this.data5Label.Name = "data5Label";
			this.data5Label.Size = new System.Drawing.Size(42, 13);
			this.data5Label.TabIndex = 47;
			this.data5Label.Text = "Data 5:";
			// 
			// posYBox
			// 
			this.posYBox.Enabled = false;
			this.posYBox.Location = new System.Drawing.Point(167, 125);
			this.posYBox.MaxLength = 4;
			this.posYBox.Name = "posYBox";
			this.posYBox.Size = new System.Drawing.Size(32, 20);
			this.posYBox.TabIndex = 52;
			this.posYBox.Text = "FFFF";
			this.posYBox.TextChanged += new System.EventHandler(this.posYBox_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(93, 128);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 51;
			this.label4.Text = "Pos Y (pixel):";
			// 
			// posXBox
			// 
			this.posXBox.Enabled = false;
			this.posXBox.Location = new System.Drawing.Point(167, 105);
			this.posXBox.MaxLength = 4;
			this.posXBox.Name = "posXBox";
			this.posXBox.Size = new System.Drawing.Size(32, 20);
			this.posXBox.TabIndex = 50;
			this.posXBox.Text = "FFFF";
			this.posXBox.TextChanged += new System.EventHandler(this.posXBox_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(93, 108);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 13);
			this.label5.TabIndex = 49;
			this.label5.Text = "Pos X (pixel):";
			// 
			// flag2Box
			// 
			this.flag2Box.Enabled = false;
			this.flag2Box.Location = new System.Drawing.Point(167, 165);
			this.flag2Box.MaxLength = 4;
			this.flag2Box.Name = "flag2Box";
			this.flag2Box.Size = new System.Drawing.Size(32, 20);
			this.flag2Box.TabIndex = 56;
			this.flag2Box.Text = "FFFF";
			this.flag2Box.TextChanged += new System.EventHandler(this.flag2Box_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(93, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(39, 13);
			this.label6.TabIndex = 55;
			this.label6.Text = "Flag 2:";
			// 
			// flag1Box
			// 
			this.flag1Box.Enabled = false;
			this.flag1Box.Location = new System.Drawing.Point(167, 145);
			this.flag1Box.MaxLength = 4;
			this.flag1Box.Name = "flag1Box";
			this.flag1Box.Size = new System.Drawing.Size(32, 20);
			this.flag1Box.TabIndex = 54;
			this.flag1Box.Text = "FFFF";
			this.flag1Box.TextChanged += new System.EventHandler(this.flag1Box_TextChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(93, 148);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(39, 13);
			this.label7.TabIndex = 53;
			this.label7.Text = "Flag 1:";
			// 
			// objectIdBox
			// 
			this.objectIdBox.Enabled = false;
			this.objectIdBox.FormattingEnabled = true;
			this.objectIdBox.Location = new System.Drawing.Point(64, 66);
			this.objectIdBox.Name = "objectIdBox";
			this.objectIdBox.Size = new System.Drawing.Size(97, 21);
			this.objectIdBox.TabIndex = 57;
			this.objectIdBox.SelectedIndexChanged += new System.EventHandler(this.objectIdBox_SelectedIndexChanged);
			// 
			// objectIdValue
			// 
			this.objectIdValue.Enabled = false;
			this.objectIdValue.Location = new System.Drawing.Point(167, 66);
			this.objectIdValue.MaxLength = 2;
			this.objectIdValue.Name = "objectIdValue";
			this.objectIdValue.Size = new System.Drawing.Size(23, 20);
			this.objectIdValue.TabIndex = 58;
			this.objectIdValue.Text = "FF";
			this.objectIdValue.TextChanged += new System.EventHandler(this.objectIdValue_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(7, 3);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(66, 13);
			this.label8.TabIndex = 59;
			this.label8.Text = "Object index";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(214, 3);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(51, 13);
			this.label9.TabIndex = 63;
			this.label9.Text = "List index";
			// 
			// listIndexLabel
			// 
			this.listIndexLabel.AutoSize = true;
			this.listIndexLabel.Location = new System.Drawing.Point(233, 23);
			this.listIndexLabel.Name = "listIndexLabel";
			this.listIndexLabel.Size = new System.Drawing.Size(0, 13);
			this.listIndexLabel.TabIndex = 62;
			// 
			// prevListButton
			// 
			this.prevListButton.Enabled = false;
			this.prevListButton.Location = new System.Drawing.Point(207, 19);
			this.prevListButton.Name = "prevListButton";
			this.prevListButton.Size = new System.Drawing.Size(20, 20);
			this.prevListButton.TabIndex = 61;
			this.prevListButton.Text = "<";
			this.prevListButton.UseVisualStyleBackColor = true;
			this.prevListButton.Click += new System.EventHandler(this.prevListButton_Click);
			// 
			// nextListButton
			// 
			this.nextListButton.Enabled = false;
			this.nextListButton.Location = new System.Drawing.Point(252, 19);
			this.nextListButton.Name = "nextListButton";
			this.nextListButton.Size = new System.Drawing.Size(20, 20);
			this.nextListButton.TabIndex = 60;
			this.nextListButton.Text = ">";
			this.nextListButton.UseVisualStyleBackColor = true;
			this.nextListButton.Click += new System.EventHandler(this.nextListButton_Click);
			// 
			// objectTypeBox
			// 
			this.objectTypeBox.Enabled = false;
			this.objectTypeBox.FormattingEnabled = true;
			this.objectTypeBox.Location = new System.Drawing.Point(64, 44);
			this.objectTypeBox.Name = "objectTypeBox";
			this.objectTypeBox.Size = new System.Drawing.Size(97, 21);
			this.objectTypeBox.TabIndex = 64;
			this.objectTypeBox.SelectedIndexChanged += new System.EventHandler(this.objectTypeBox_SelectedIndexChanged);
			// 
			// objectTypeValue
			// 
			this.objectTypeValue.Enabled = false;
			this.objectTypeValue.Location = new System.Drawing.Point(167, 44);
			this.objectTypeValue.MaxLength = 2;
			this.objectTypeValue.Name = "objectTypeValue";
			this.objectTypeValue.Size = new System.Drawing.Size(23, 20);
			this.objectTypeValue.TabIndex = 65;
			this.objectTypeValue.Text = "FF";
			this.objectTypeValue.TextChanged += new System.EventHandler(this.objectTypeValue_TextChanged);
			// 
			// copyButton
			// 
			this.copyButton.Enabled = false;
			this.copyButton.Location = new System.Drawing.Point(103, 188);
			this.copyButton.Name = "copyButton";
			this.copyButton.Size = new System.Drawing.Size(52, 20);
			this.copyButton.TabIndex = 66;
			this.copyButton.Text = "Copy";
			this.copyButton.UseVisualStyleBackColor = true;
			this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
			// 
			// pasteButton
			// 
			this.pasteButton.Enabled = false;
			this.pasteButton.Location = new System.Drawing.Point(167, 188);
			this.pasteButton.Name = "pasteButton";
			this.pasteButton.Size = new System.Drawing.Size(52, 20);
			this.pasteButton.TabIndex = 67;
			this.pasteButton.Text = "Paste";
			this.pasteButton.UseVisualStyleBackColor = true;
			this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
			// 
			// ObjectPlacementEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 211);
			this.Controls.Add(this.pasteButton);
			this.Controls.Add(this.copyButton);
			this.Controls.Add(this.objectTypeValue);
			this.Controls.Add(this.objectTypeBox);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.listIndexLabel);
			this.Controls.Add(this.prevListButton);
			this.Controls.Add(this.nextListButton);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.objectIdValue);
			this.Controls.Add(this.objectIdBox);
			this.Controls.Add(this.flag2Box);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.flag1Box);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.posYBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.posXBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.data5Box);
			this.Controls.Add(this.data5Label);
			this.Controls.Add(this.data4Box);
			this.Controls.Add(this.data4Label);
			this.Controls.Add(this.data3Box);
			this.Controls.Add(this.data3Label);
			this.Controls.Add(this.data2Box);
			this.Controls.Add(this.data2Label);
			this.Controls.Add(this.data1Box);
			this.Controls.Add(this.data1Label);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.unknownBox);
			this.Controls.Add(this.label2);
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
			this.Name = "ObjectPlacementEditor";
			this.Text = "Object Placement Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label indexLabel;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox unknownBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox data1Box;
		private System.Windows.Forms.Label data1Label;
		private System.Windows.Forms.TextBox data2Box;
		private System.Windows.Forms.Label data2Label;
		private System.Windows.Forms.TextBox data3Box;
		private System.Windows.Forms.Label data3Label;
		private System.Windows.Forms.TextBox data4Box;
		private System.Windows.Forms.Label data4Label;
		private System.Windows.Forms.TextBox data5Box;
		private System.Windows.Forms.Label data5Label;
		private System.Windows.Forms.TextBox posYBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox posXBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox flag2Box;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox flag1Box;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox objectIdBox;
		private System.Windows.Forms.TextBox objectIdValue;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label listIndexLabel;
		private System.Windows.Forms.Button prevListButton;
		private System.Windows.Forms.Button nextListButton;
		private System.Windows.Forms.ComboBox objectTypeBox;
		private System.Windows.Forms.TextBox objectTypeValue;
		private System.Windows.Forms.Button copyButton;
		private System.Windows.Forms.Button pasteButton;
	}
}
