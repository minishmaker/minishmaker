namespace MinishMaker.UI
{
	partial class ResizeRoomDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.xDimBox = new System.Windows.Forms.TextBox();
            this.yDimBox = new System.Windows.Forms.TextBox();
            this.confirmation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set the new dimensions for the room.\r\n(decimal)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "X:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y:";
            // 
            // xDimBox
            // 
            this.xDimBox.Location = new System.Drawing.Point(35, 36);
            this.xDimBox.MaxLength = 2;
            this.xDimBox.Name = "xDimBox";
            this.xDimBox.Size = new System.Drawing.Size(38, 20);
            this.xDimBox.TabIndex = 3;
            // 
            // yDimBox
            // 
            this.yDimBox.Location = new System.Drawing.Point(102, 36);
            this.yDimBox.MaxLength = 2;
            this.yDimBox.Name = "yDimBox";
            this.yDimBox.Size = new System.Drawing.Size(38, 20);
            this.yDimBox.TabIndex = 4;
            // 
            // confirmation
            // 
            this.confirmation.Location = new System.Drawing.Point(65, 62);
            this.confirmation.Name = "confirmation";
            this.confirmation.Size = new System.Drawing.Size(75, 23);
            this.confirmation.TabIndex = 5;
            this.confirmation.Text = "Ok";
            this.confirmation.UseVisualStyleBackColor = true;
            // 
            // ResizeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 92);
            this.Controls.Add(this.confirmation);
            this.Controls.Add(this.yDimBox);
            this.Controls.Add(this.xDimBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResizeDialog";
            this.Text = "Resize Room";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox xDimBox;
		private System.Windows.Forms.TextBox yDimBox;
		private System.Windows.Forms.Button confirmation;
	}
}