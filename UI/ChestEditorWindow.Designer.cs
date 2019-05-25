namespace MinishMaker.UI
{
	partial class ChestEditorWindow
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
			this.entityType = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.entityId = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.itemName = new System.Windows.Forms.ComboBox();
			this.amountLabel = new System.Windows.Forms.Label();
			this.itemAmount = new System.Windows.Forms.TextBox();
			this.kinstoneLabel = new System.Windows.Forms.Label();
			this.kinstoneType = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.xPosition = new System.Windows.Forms.TextBox();
			this.yPosition = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.indexLabel = new System.Windows.Forms.Label();
			this.nextButton = new System.Windows.Forms.Button();
			this.prevButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// entityType
			// 
			this.entityType.Location = new System.Drawing.Point(79, 75);
			this.entityType.MaxLength = 2;
			this.entityType.Name = "entityType";
			this.entityType.Size = new System.Drawing.Size(20, 20);
			this.entityType.TabIndex = 0;
			this.entityType.Text = "00";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 78);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Entity Type";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 105);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Entity Id";
			// 
			// entityId
			// 
			this.entityId.Location = new System.Drawing.Point(79, 102);
			this.entityId.MaxLength = 2;
			this.entityId.Name = "entityId";
			this.entityId.Size = new System.Drawing.Size(20, 20);
			this.entityId.TabIndex = 2;
			this.entityId.Text = "00";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(114, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(27, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Item";
			// 
			// itemName
			// 
			this.itemName.FormattingEnabled = true;
			this.itemName.Location = new System.Drawing.Point(196, 75);
			this.itemName.Name = "itemName";
			this.itemName.Size = new System.Drawing.Size(121, 21);
			this.itemName.TabIndex = 6;
			this.itemName.SelectedIndexChanged += new System.EventHandler(this.itemName_SelectedIndexChanged);
			// 
			// amountLabel
			// 
			this.amountLabel.AutoSize = true;
			this.amountLabel.Location = new System.Drawing.Point(114, 105);
			this.amountLabel.Name = "amountLabel";
			this.amountLabel.Size = new System.Drawing.Size(43, 13);
			this.amountLabel.TabIndex = 7;
			this.amountLabel.Text = "Amount";
			this.amountLabel.Visible = false;
			// 
			// itemAmount
			// 
			this.itemAmount.Location = new System.Drawing.Point(196, 102);
			this.itemAmount.MaxLength = 3;
			this.itemAmount.Name = "itemAmount";
			this.itemAmount.Size = new System.Drawing.Size(27, 20);
			this.itemAmount.TabIndex = 8;
			this.itemAmount.Text = "00";
			this.itemAmount.Visible = false;
			// 
			// kinstoneLabel
			// 
			this.kinstoneLabel.AutoSize = true;
			this.kinstoneLabel.Location = new System.Drawing.Point(114, 105);
			this.kinstoneLabel.Name = "kinstoneLabel";
			this.kinstoneLabel.Size = new System.Drawing.Size(75, 13);
			this.kinstoneLabel.TabIndex = 9;
			this.kinstoneLabel.Text = "Kinstone Type";
			this.kinstoneLabel.Visible = false;
			// 
			// kinstoneType
			// 
			this.kinstoneType.FormattingEnabled = true;
			this.kinstoneType.Location = new System.Drawing.Point(196, 102);
			this.kinstoneType.Name = "kinstoneType";
			this.kinstoneType.Size = new System.Drawing.Size(121, 21);
			this.kinstoneType.TabIndex = 10;
			this.kinstoneType.Visible = false;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 131);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(44, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "Position";
			// 
			// xPosition
			// 
			this.xPosition.Location = new System.Drawing.Point(79, 128);
			this.xPosition.MaxLength = 2;
			this.xPosition.Name = "xPosition";
			this.xPosition.Size = new System.Drawing.Size(20, 20);
			this.xPosition.TabIndex = 12;
			this.xPosition.Text = "00";
			// 
			// yPosition
			// 
			this.yPosition.Location = new System.Drawing.Point(120, 128);
			this.yPosition.MaxLength = 2;
			this.yPosition.Name = "yPosition";
			this.yPosition.Size = new System.Drawing.Size(20, 20);
			this.yPosition.TabIndex = 13;
			this.yPosition.Text = "00";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(63, 131);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(14, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "X";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(105, 131);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(14, 13);
			this.label8.TabIndex = 15;
			this.label8.Text = "Y";
			// 
			// indexLabel
			// 
			this.indexLabel.AutoSize = true;
			this.indexLabel.Location = new System.Drawing.Point(38, 16);
			this.indexLabel.Name = "indexLabel";
			this.indexLabel.Size = new System.Drawing.Size(13, 13);
			this.indexLabel.TabIndex = 16;
			this.indexLabel.Text = "0";
			// 
			// nextButton
			// 
			this.nextButton.Location = new System.Drawing.Point(57, 12);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(20, 20);
			this.nextButton.TabIndex = 17;
			this.nextButton.Text = ">";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// prevButton
			// 
			this.prevButton.Location = new System.Drawing.Point(12, 12);
			this.prevButton.Name = "prevButton";
			this.prevButton.Size = new System.Drawing.Size(20, 20);
			this.prevButton.TabIndex = 18;
			this.prevButton.Text = "<";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(16, 158);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(50, 20);
			this.saveButton.TabIndex = 19;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			// 
			// ChestEditorWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(333, 262);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.prevButton);
			this.Controls.Add(this.nextButton);
			this.Controls.Add(this.indexLabel);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.yPosition);
			this.Controls.Add(this.xPosition);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.kinstoneType);
			this.Controls.Add(this.kinstoneLabel);
			this.Controls.Add(this.itemAmount);
			this.Controls.Add(this.amountLabel);
			this.Controls.Add(this.itemName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.entityId);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.entityType);
			this.Name = "ChestEditorWindow";
			this.Text = "ChestEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox entityType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox entityId;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox itemName;
		private System.Windows.Forms.Label amountLabel;
		private System.Windows.Forms.TextBox itemAmount;
		private System.Windows.Forms.Label kinstoneLabel;
		private System.Windows.Forms.ComboBox kinstoneType;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox xPosition;
		private System.Windows.Forms.TextBox yPosition;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label indexLabel;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Button saveButton;
	}
}