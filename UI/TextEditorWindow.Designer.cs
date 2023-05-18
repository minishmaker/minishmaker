namespace MinishMaker.UI
{
    partial class TextEditorWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorWindow));
            this.languagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.decreaseBankButton = new System.Windows.Forms.Button();
            this.increaseBankButton = new System.Windows.Forms.Button();
            this.increaseEntryButton = new System.Windows.Forms.Button();
            this.decreaseEntryButton = new System.Windows.Forms.Button();
            this.bankLabel = new System.Windows.Forms.Label();
            this.entryLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.largeEditBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // languagePanel
            // 
            this.languagePanel.ColumnCount = 2;
            this.languagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.66949F));
            this.languagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.33051F));
            this.languagePanel.Location = new System.Drawing.Point(12, 57);
            this.languagePanel.Name = "languagePanel";
            this.languagePanel.RowCount = 8;
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.languagePanel.Size = new System.Drawing.Size(357, 371);
            this.languagePanel.TabIndex = 0;
            // 
            // decreaseBankButton
            // 
            this.decreaseBankButton.Location = new System.Drawing.Point(102, 12);
            this.decreaseBankButton.Name = "decreaseBankButton";
            this.decreaseBankButton.Size = new System.Drawing.Size(25, 25);
            this.decreaseBankButton.TabIndex = 0;
            this.decreaseBankButton.Text = "<";
            this.decreaseBankButton.UseVisualStyleBackColor = true;
            this.decreaseBankButton.Click += new System.EventHandler(this.decreaseBankButton_Click);
            // 
            // increaseBankButton
            // 
            this.increaseBankButton.Location = new System.Drawing.Point(158, 12);
            this.increaseBankButton.Name = "increaseBankButton";
            this.increaseBankButton.Size = new System.Drawing.Size(25, 25);
            this.increaseBankButton.TabIndex = 1;
            this.increaseBankButton.Text = ">";
            this.increaseBankButton.UseVisualStyleBackColor = true;
            this.increaseBankButton.Click += new System.EventHandler(this.increaseBankButton_Click);
            // 
            // increaseEntryButton
            // 
            this.increaseEntryButton.Location = new System.Drawing.Point(323, 12);
            this.increaseEntryButton.Name = "increaseEntryButton";
            this.increaseEntryButton.Size = new System.Drawing.Size(25, 25);
            this.increaseEntryButton.TabIndex = 3;
            this.increaseEntryButton.Text = ">";
            this.increaseEntryButton.UseVisualStyleBackColor = true;
            this.increaseEntryButton.Click += new System.EventHandler(this.increaseEntryButton_Click);
            // 
            // decreaseEntryButton
            // 
            this.decreaseEntryButton.Location = new System.Drawing.Point(267, 12);
            this.decreaseEntryButton.Name = "decreaseEntryButton";
            this.decreaseEntryButton.Size = new System.Drawing.Size(25, 25);
            this.decreaseEntryButton.TabIndex = 2;
            this.decreaseEntryButton.Text = "<";
            this.decreaseEntryButton.UseVisualStyleBackColor = true;
            this.decreaseEntryButton.Click += new System.EventHandler(this.decreaseEntryButton_Click);
            // 
            // bankLabel
            // 
            this.bankLabel.AutoSize = true;
            this.bankLabel.Location = new System.Drawing.Point(133, 16);
            this.bankLabel.Name = "bankLabel";
            this.bankLabel.Size = new System.Drawing.Size(16, 17);
            this.bankLabel.TabIndex = 4;
            this.bankLabel.Text = "0";
            // 
            // entryLabel
            // 
            this.entryLabel.AutoSize = true;
            this.entryLabel.Location = new System.Drawing.Point(298, 16);
            this.entryLabel.Name = "entryLabel";
            this.entryLabel.Size = new System.Drawing.Size(16, 17);
            this.entryLabel.TabIndex = 5;
            this.entryLabel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text bank :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Entry :";
            // 
            // largeEditBox
            // 
            this.largeEditBox.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.largeEditBox.Location = new System.Drawing.Point(375, 57);
            this.largeEditBox.Multiline = true;
            this.largeEditBox.Name = "largeEditBox";
            this.largeEditBox.ReadOnly = true;
            this.largeEditBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.largeEditBox.Size = new System.Drawing.Size(413, 371);
            this.largeEditBox.TabIndex = 0;
            // 
            // TextEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 440);
            this.Controls.Add(this.largeEditBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.entryLabel);
            this.Controls.Add(this.bankLabel);
            this.Controls.Add(this.increaseEntryButton);
            this.Controls.Add(this.decreaseEntryButton);
            this.Controls.Add(this.increaseBankButton);
            this.Controls.Add(this.decreaseBankButton);
            this.Controls.Add(this.languagePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextEditorWindow";
            this.Text = "TextEditorWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel languagePanel;
        private System.Windows.Forms.Button decreaseBankButton;
        private System.Windows.Forms.Button increaseBankButton;
        private System.Windows.Forms.Button increaseEntryButton;
        private System.Windows.Forms.Button decreaseEntryButton;
        private System.Windows.Forms.Label bankLabel;
        private System.Windows.Forms.Label entryLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox largeEditBox;
    }
}
