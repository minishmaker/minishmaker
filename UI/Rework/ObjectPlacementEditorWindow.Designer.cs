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
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.listIndexLabel = new System.Windows.Forms.Label();
			this.prevListButton = new System.Windows.Forms.Button();
			this.nextListButton = new System.Windows.Forms.Button();
			this.copyButton = new System.Windows.Forms.Button();
			this.pasteButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// indexLabel
			// 
			this.indexLabel.AutoSize = true;
			this.indexLabel.Location = new System.Drawing.Point(43, 28);
			this.indexLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.indexLabel.Name = "indexLabel";
			this.indexLabel.Size = new System.Drawing.Size(0, 17);
			this.indexLabel.TabIndex = 31;
			// 
			// removeButton
			// 
			this.removeButton.Enabled = false;
			this.removeButton.Location = new System.Drawing.Point(137, 23);
			this.removeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(27, 25);
			this.removeButton.TabIndex = 30;
			this.removeButton.Text = "-";
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// newButton
			// 
			this.newButton.Enabled = false;
			this.newButton.Location = new System.Drawing.Point(103, 23);
			this.newButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(27, 25);
			this.newButton.TabIndex = 29;
			this.newButton.Text = "+";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// prevButton
			// 
			this.prevButton.Enabled = false;
			this.prevButton.Location = new System.Drawing.Point(8, 23);
			this.prevButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.prevButton.Name = "prevButton";
			this.prevButton.Size = new System.Drawing.Size(27, 25);
			this.prevButton.TabIndex = 28;
			this.prevButton.Text = "<";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.Enabled = false;
			this.nextButton.Location = new System.Drawing.Point(68, 23);
			this.nextButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(27, 25);
			this.nextButton.TabIndex = 27;
			this.nextButton.Text = ">";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(9, 4);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(86, 17);
			this.label8.TabIndex = 59;
			this.label8.Text = "Object index";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(285, 4);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(67, 17);
			this.label9.TabIndex = 63;
			this.label9.Text = "List index";
			// 
			// listIndexLabel
			// 
			this.listIndexLabel.AutoSize = true;
			this.listIndexLabel.Location = new System.Drawing.Point(311, 28);
			this.listIndexLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.listIndexLabel.Name = "listIndexLabel";
			this.listIndexLabel.Size = new System.Drawing.Size(0, 17);
			this.listIndexLabel.TabIndex = 62;
			// 
			// prevListButton
			// 
			this.prevListButton.Enabled = false;
			this.prevListButton.Location = new System.Drawing.Point(276, 23);
			this.prevListButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.prevListButton.Name = "prevListButton";
			this.prevListButton.Size = new System.Drawing.Size(27, 25);
			this.prevListButton.TabIndex = 61;
			this.prevListButton.Text = "<";
			this.prevListButton.UseVisualStyleBackColor = true;
			this.prevListButton.Click += new System.EventHandler(this.prevListButton_Click);
			// 
			// nextListButton
			// 
			this.nextListButton.Enabled = false;
			this.nextListButton.Location = new System.Drawing.Point(336, 23);
			this.nextListButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nextListButton.Name = "nextListButton";
			this.nextListButton.Size = new System.Drawing.Size(27, 25);
			this.nextListButton.TabIndex = 60;
			this.nextListButton.Text = ">";
			this.nextListButton.UseVisualStyleBackColor = true;
			this.nextListButton.Click += new System.EventHandler(this.nextListButton_Click);
			// 
			// copyButton
			// 
			this.copyButton.Enabled = false;
			this.copyButton.Location = new System.Drawing.Point(137, 231);
			this.copyButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.copyButton.Name = "copyButton";
			this.copyButton.Size = new System.Drawing.Size(69, 25);
			this.copyButton.TabIndex = 66;
			this.copyButton.Text = "Copy";
			this.copyButton.UseVisualStyleBackColor = true;
			this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
			// 
			// pasteButton
			// 
			this.pasteButton.Enabled = false;
			this.pasteButton.Location = new System.Drawing.Point(223, 231);
			this.pasteButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pasteButton.Name = "pasteButton";
			this.pasteButton.Size = new System.Drawing.Size(69, 25);
			this.pasteButton.TabIndex = 67;
			this.pasteButton.Text = "Paste";
			this.pasteButton.UseVisualStyleBackColor = true;
			this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
			// 
			// ObjectPlacementEditorWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 260);
			this.Controls.Add(this.pasteButton);
			this.Controls.Add(this.copyButton);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.listIndexLabel);
			this.Controls.Add(this.prevListButton);
			this.Controls.Add(this.nextListButton);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.indexLabel);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.newButton);
			this.Controls.Add(this.prevButton);
			this.Controls.Add(this.nextButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ObjectPlacementEditorWindow";
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
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label listIndexLabel;
		private System.Windows.Forms.Button prevListButton;
		private System.Windows.Forms.Button nextListButton;
		private System.Windows.Forms.Button copyButton;
		private System.Windows.Forms.Button pasteButton;
	}
}
