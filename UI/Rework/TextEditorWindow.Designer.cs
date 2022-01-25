namespace MinishMaker.UI.Rework
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
            this.languagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.decreaseBankButton = new System.Windows.Forms.Button();
            this.increaseBankButton = new System.Windows.Forms.Button();
            this.increaseEntryButton = new System.Windows.Forms.Button();
            this.decreaseEntryButton = new System.Windows.Forms.Button();
            this.bankLabel = new System.Windows.Forms.Label();
            this.entryLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // languagePanel
            // 
            this.languagePanel.ColumnCount = 2;
            this.languagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.19337F));
            this.languagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.80663F));
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
            this.languagePanel.Size = new System.Drawing.Size(724, 325);
            this.languagePanel.TabIndex = 0;
            // 
            // decreaseBankButton
            // 
            this.decreaseBankButton.Location = new System.Drawing.Point(12, 12);
            this.decreaseBankButton.Name = "decreaseBankButton";
            this.decreaseBankButton.Size = new System.Drawing.Size(25, 25);
            this.decreaseBankButton.TabIndex = 0;
            this.decreaseBankButton.Text = "<";
            this.decreaseBankButton.UseVisualStyleBackColor = true;
            this.decreaseBankButton.Click += new System.EventHandler(this.decreaseBankButton_Click);
            // 
            // increaseBankButton
            // 
            this.increaseBankButton.Location = new System.Drawing.Point(68, 12);
            this.increaseBankButton.Name = "increaseBankButton";
            this.increaseBankButton.Size = new System.Drawing.Size(25, 25);
            this.increaseBankButton.TabIndex = 1;
            this.increaseBankButton.Text = ">";
            this.increaseBankButton.UseVisualStyleBackColor = true;
            this.increaseBankButton.Click += new System.EventHandler(this.increaseBankButton_Click);
            // 
            // increaseEntryButton
            // 
            this.increaseEntryButton.Location = new System.Drawing.Point(233, 12);
            this.increaseEntryButton.Name = "increaseEntryButton";
            this.increaseEntryButton.Size = new System.Drawing.Size(25, 25);
            this.increaseEntryButton.TabIndex = 3;
            this.increaseEntryButton.Text = ">";
            this.increaseEntryButton.UseVisualStyleBackColor = true;
            this.increaseEntryButton.Click += new System.EventHandler(this.increaseEntryButton_Click);
            // 
            // decreaseEntryButton
            // 
            this.decreaseEntryButton.Location = new System.Drawing.Point(177, 12);
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
            this.bankLabel.Location = new System.Drawing.Point(43, 16);
            this.bankLabel.Name = "bankLabel";
            this.bankLabel.Size = new System.Drawing.Size(16, 17);
            this.bankLabel.TabIndex = 4;
            this.bankLabel.Text = "0";
            // 
            // entryLabel
            // 
            this.entryLabel.AutoSize = true;
            this.entryLabel.Location = new System.Drawing.Point(208, 16);
            this.entryLabel.Name = "entryLabel";
            this.entryLabel.Size = new System.Drawing.Size(16, 17);
            this.entryLabel.TabIndex = 5;
            this.entryLabel.Text = "0";
            // 
            // TextEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.entryLabel);
            this.Controls.Add(this.bankLabel);
            this.Controls.Add(this.increaseEntryButton);
            this.Controls.Add(this.decreaseEntryButton);
            this.Controls.Add(this.increaseBankButton);
            this.Controls.Add(this.decreaseBankButton);
            this.Controls.Add(this.languagePanel);
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
    }
}