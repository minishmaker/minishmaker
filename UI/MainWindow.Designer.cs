namespace MinishMaker.UI
{
    partial class MainWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveRoomChangesCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bottomStatusStrip = new System.Windows.Forms.StatusStrip();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.roomPanel = new System.Windows.Forms.Panel();
			this.roomTreeView = new System.Windows.Forms.TreeView();
			this.tilePanel = new System.Windows.Forms.Panel();
			this.tileView = new System.Windows.Forms.PictureBox();
			this.tileSelectionBox = new System.Windows.Forms.PictureBox();
			this.mapPanel = new System.Windows.Forms.Panel();
			this.mapView = new System.Windows.Forms.PictureBox();
			this.mapSelectionBox = new System.Windows.Forms.PictureBox();
			this.menuStrip1.SuspendLayout();
			this.bottomStatusStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.roomPanel.SuspendLayout();
			this.tilePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tileView)).BeginInit();
			this.tileView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tileSelectionBox)).BeginInit();
			this.mapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapView)).BeginInit();
			this.mapView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapSelectionBox)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "mainMenuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.saveRoomChangesCtrlSToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenButtonClick);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitButtonClick);
			// 
			// saveRoomChangesCtrlSToolStripMenuItem
			// 
			this.saveRoomChangesCtrlSToolStripMenuItem.Name = "saveRoomChangesCtrlSToolStripMenuItem";
			this.saveRoomChangesCtrlSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveRoomChangesCtrlSToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.saveRoomChangesCtrlSToolStripMenuItem.Text = "Save Room Changes ";
			this.saveRoomChangesCtrlSToolStripMenuItem.Click += new System.EventHandler(this.saveRoomChangesCtrlSToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutButtonClick);
			// 
			// bottomStatusStrip
			// 
			this.bottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
			this.bottomStatusStrip.Location = new System.Drawing.Point(0, 428);
			this.bottomStatusStrip.Name = "bottomStatusStrip";
			this.bottomStatusStrip.Size = new System.Drawing.Size(800, 22);
			this.bottomStatusStrip.TabIndex = 7;
			this.bottomStatusStrip.Text = "bottomStatusStrip";
			// 
			// statusText
			// 
			this.statusText.Name = "statusText";
			this.statusText.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.toolStrip.Size = new System.Drawing.Size(800, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip";
			// 
			// openToolStripButton
			// 
			this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
			this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openToolStripButton.Name = "openToolStripButton";
			this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openToolStripButton.Text = "toolStripButton1";
			this.openToolStripButton.ToolTipText = "open a ROM.";
			this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
			// 
			// roomPanel
			// 
			this.roomPanel.Controls.Add(this.roomTreeView);
			this.roomPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.roomPanel.Location = new System.Drawing.Point(0, 49);
			this.roomPanel.Name = "roomPanel";
			this.roomPanel.Size = new System.Drawing.Size(150, 379);
			this.roomPanel.TabIndex = 8;
			// 
			// roomTreeView
			// 
			this.roomTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.roomTreeView.Location = new System.Drawing.Point(0, 0);
			this.roomTreeView.Name = "roomTreeView";
			this.roomTreeView.Size = new System.Drawing.Size(150, 379);
			this.roomTreeView.TabIndex = 0;
			this.roomTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.roomTreeView_NodeMouseDoubleClick);
			// 
			// tilePanel
			// 
			this.tilePanel.AutoScroll = true;
			this.tilePanel.Controls.Add(this.tileView);
			this.tilePanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.tilePanel.Location = new System.Drawing.Point(600, 49);
			this.tilePanel.Name = "tilePanel";
			this.tilePanel.Size = new System.Drawing.Size(200, 379);
			this.tilePanel.TabIndex = 9;
			// 
			// tileView
			// 
			this.tileView.Controls.Add(this.tileSelectionBox);
			this.tileView.Location = new System.Drawing.Point(0, 0);
			this.tileView.Name = "tileView";
			this.tileView.Size = new System.Drawing.Size(128, 128);
			this.tileView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.tileView.TabIndex = 10;
			this.tileView.TabStop = false;
			this.tileView.Click += new System.EventHandler(this.tileView_Click);
			// 
			// tileSelectionBox
			// 
			this.tileSelectionBox.Location = new System.Drawing.Point(0, 0);
			this.tileSelectionBox.Name = "tileSelectionBox";
			this.tileSelectionBox.Size = new System.Drawing.Size(16, 16);
			this.tileSelectionBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.tileSelectionBox.TabIndex = 11;
			this.tileSelectionBox.TabStop = false;
			this.tileSelectionBox.Visible = false;
			// 
			// mapPanel
			// 
			this.mapPanel.AutoScroll = true;
			this.mapPanel.Controls.Add(this.mapView);
			this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapPanel.Location = new System.Drawing.Point(150, 49);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(450, 379);
			this.mapPanel.TabIndex = 10;
			// 
			// mapView
			// 
			this.mapView.Controls.Add(this.mapSelectionBox);
			this.mapView.Location = new System.Drawing.Point(0, 0);
			this.mapView.Name = "mapView";
			this.mapView.Size = new System.Drawing.Size(128, 128);
			this.mapView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.mapView.TabIndex = 9;
			this.mapView.TabStop = false;
			this.mapView.Click += new System.EventHandler(this.mapView_Click);
			// 
			// mapSelectionBox
			// 
			this.mapSelectionBox.Location = new System.Drawing.Point(0, 0);
			this.mapSelectionBox.Name = "mapSelectionBox";
			this.mapSelectionBox.Size = new System.Drawing.Size(16, 16);
			this.mapSelectionBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.mapSelectionBox.TabIndex = 10;
			this.mapSelectionBox.TabStop = false;
			this.mapSelectionBox.Visible = false;
			// 
			// MainWindow
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.mapPanel);
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.roomPanel);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.bottomStatusStrip);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainWindow";
			this.Text = "Minish Maker";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWindow_DragDrop);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.bottomStatusStrip.ResumeLayout(false);
			this.bottomStatusStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.roomPanel.ResumeLayout(false);
			this.tilePanel.ResumeLayout(false);
			this.tilePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tileView)).EndInit();
			this.tileView.ResumeLayout(false);
			this.tileView.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tileSelectionBox)).EndInit();
			this.mapPanel.ResumeLayout(false);
			this.mapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapView)).EndInit();
			this.mapView.ResumeLayout(false);
			this.mapView.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapSelectionBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip bottomStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Panel roomPanel;
        private System.Windows.Forms.Panel tilePanel;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.TreeView roomTreeView;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.PictureBox mapView;
		private System.Windows.Forms.PictureBox tileView;
		private System.Windows.Forms.ToolStripMenuItem saveRoomChangesCtrlSToolStripMenuItem;
		private System.Windows.Forms.PictureBox tileSelectionBox;
		private System.Windows.Forms.PictureBox mapSelectionBox;
	}
}

