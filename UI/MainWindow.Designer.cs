﻿using System;

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
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveRoomChangesCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.topLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bottomLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bothLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chestEditorStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.metatileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bottomStatusStrip = new System.Windows.Forms.StatusStrip();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.chestToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.metatileToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.roomPanel = new System.Windows.Forms.Panel();
			this.roomTreeView = new System.Windows.Forms.TreeView();
			this.tilePanel = new System.Windows.Forms.Panel();
			this.tileTabControl = new System.Windows.Forms.TabControl();
			this.topTileTab = new System.Windows.Forms.TabPage();
			this.bottomTileTab = new System.Windows.Forms.TabPage();
			this.mapPanel = new System.Windows.Forms.Panel();
			this.exportAsImageToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.mapGridBox = new MinishMaker.UI.GridBox();
			this.topTileGridBox = new MinishMaker.UI.GridBox();
			this.bottomTileGridBox = new MinishMaker.UI.GridBox();
			this.menuStrip.SuspendLayout();
			this.bottomStatusStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.roomPanel.SuspendLayout();
			this.tilePanel.SuspendLayout();
			this.tileTabControl.SuspendLayout();
			this.topTileTab.SuspendLayout();
			this.bottomTileTab.SuspendLayout();
			this.mapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapGridBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.topTileGridBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bottomTileGridBox)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(1284, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "mainMenuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectRomToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveRoomChangesCtrlSToolStripMenuItem,
            this.buildProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// selectRomToolStripMenuItem
			// 
			this.selectRomToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectRomToolStripMenuItem.Image")));
			this.selectRomToolStripMenuItem.Name = "selectRomToolStripMenuItem";
			this.selectRomToolStripMenuItem.ShowShortcutKeys = false;
			this.selectRomToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.selectRomToolStripMenuItem.Text = "Select ROM";
			this.selectRomToolStripMenuItem.Click += new System.EventHandler(this.OpenButtonClick);
			// 
			// openProjectToolStripMenuItem
			// 
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.openProjectToolStripMenuItem.Text = "Open Project";
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.SelectProjectButtonClick);
			// 
			// saveRoomChangesCtrlSToolStripMenuItem
			// 
			this.saveRoomChangesCtrlSToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveRoomChangesCtrlSToolStripMenuItem.Image")));
			this.saveRoomChangesCtrlSToolStripMenuItem.Name = "saveRoomChangesCtrlSToolStripMenuItem";
			this.saveRoomChangesCtrlSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveRoomChangesCtrlSToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveRoomChangesCtrlSToolStripMenuItem.Text = "Save Project";
			this.saveRoomChangesCtrlSToolStripMenuItem.Click += new System.EventHandler(this.saveAllChangesCtrlSToolStripMenuItem_Click);
			// 
			// buildProjectToolStripMenuItem
			// 
			this.buildProjectToolStripMenuItem.Name = "buildProjectToolStripMenuItem";
			this.buildProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.buildProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.buildProjectToolStripMenuItem.Text = "Build Project";
			this.buildProjectToolStripMenuItem.Click += new System.EventHandler(this.BuildProjectToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitButtonClick);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topLayerToolStripMenuItem,
            this.bottomLayerToolStripMenuItem,
            this.bothLayersToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// topLayerToolStripMenuItem
			// 
			this.topLayerToolStripMenuItem.Name = "topLayerToolStripMenuItem";
			this.topLayerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
			this.topLayerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.topLayerToolStripMenuItem.Text = "Top Layer";
			this.topLayerToolStripMenuItem.Click += new System.EventHandler(this.topLayerToolStripMenuItem_Click);
			// 
			// bottomLayerToolStripMenuItem
			// 
			this.bottomLayerToolStripMenuItem.Name = "bottomLayerToolStripMenuItem";
			this.bottomLayerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
			this.bottomLayerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.bottomLayerToolStripMenuItem.Text = "Bottom Layer";
			this.bottomLayerToolStripMenuItem.Click += new System.EventHandler(this.bottomLayerToolStripMenuItem_Click);
			// 
			// bothLayersToolStripMenuItem
			// 
			this.bothLayersToolStripMenuItem.Name = "bothLayersToolStripMenuItem";
			this.bothLayersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
			this.bothLayersToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.bothLayersToolStripMenuItem.Text = "Both Layers";
			this.bothLayersToolStripMenuItem.Click += new System.EventHandler(this.bothLayersToolStripMenuItem_Click);
			// 
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chestEditorStripMenuItem,
            this.metatileEditorToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.windowToolStripMenuItem.Text = "Window";
			// 
			// chestEditorStripMenuItem
			// 
			this.chestEditorStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("chestEditorStripMenuItem.Image")));
			this.chestEditorStripMenuItem.Name = "chestEditorStripMenuItem";
			this.chestEditorStripMenuItem.ShowShortcutKeys = false;
			this.chestEditorStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.chestEditorStripMenuItem.Text = "Chest Editor";
			this.chestEditorStripMenuItem.Click += new System.EventHandler(this.chestEditorStripMenuItem_Click);
			// 
			// metatileEditorToolStripMenuItem
			// 
			this.metatileEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("metatileEditorToolStripMenuItem.Image")));
			this.metatileEditorToolStripMenuItem.Name = "metatileEditorToolStripMenuItem";
			this.metatileEditorToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.metatileEditorToolStripMenuItem.Text = "Metatile Editor";
			this.metatileEditorToolStripMenuItem.Click += new System.EventHandler(this.metatileEditorToolStripMenuItem_Click);
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
			this.bottomStatusStrip.Location = new System.Drawing.Point(0, 659);
			this.bottomStatusStrip.Name = "bottomStatusStrip";
			this.bottomStatusStrip.Size = new System.Drawing.Size(1284, 22);
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
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.chestToolStripButton,
            this.metatileToolStripButton,
            this.exportAsImageToolStripButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.toolStrip.Size = new System.Drawing.Size(1284, 25);
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
			// saveToolStripButton
			// 
			this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
			this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveToolStripButton.Name = "saveToolStripButton";
			this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveToolStripButton.Text = "toolStripButton1";
			this.saveToolStripButton.ToolTipText = "Save all changes.";
			this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// chestToolStripButton
			// 
			this.chestToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.chestToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("chestToolStripButton.Image")));
			this.chestToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.chestToolStripButton.Name = "chestToolStripButton";
			this.chestToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.chestToolStripButton.Text = "Chest Editor";
			this.chestToolStripButton.Click += new System.EventHandler(this.chestToolStripButton_Click);
			// 
			// metatileToolStripButton
			// 
			this.metatileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.metatileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("metatileToolStripButton.Image")));
			this.metatileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.metatileToolStripButton.Name = "metatileToolStripButton";
			this.metatileToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.metatileToolStripButton.Text = "Metatile Editor";
			this.metatileToolStripButton.Click += new System.EventHandler(this.metatileToolStripButton_Click);
			// 
			// roomPanel
			// 
			this.roomPanel.Controls.Add(this.roomTreeView);
			this.roomPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.roomPanel.Location = new System.Drawing.Point(0, 49);
			this.roomPanel.Name = "roomPanel";
			this.roomPanel.Size = new System.Drawing.Size(150, 610);
			this.roomPanel.TabIndex = 8;
			// 
			// roomTreeView
			// 
			this.roomTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.roomTreeView.Location = new System.Drawing.Point(0, 0);
			this.roomTreeView.Name = "roomTreeView";
			this.roomTreeView.Size = new System.Drawing.Size(150, 610);
			this.roomTreeView.TabIndex = 0;
			this.roomTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.roomTreeView_NodeMouseDoubleClick);
			// 
			// tilePanel
			// 
			this.tilePanel.AutoScroll = true;
			this.tilePanel.Controls.Add(this.tileTabControl);
			this.tilePanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.tilePanel.Location = new System.Drawing.Point(1003, 49);
			this.tilePanel.Name = "tilePanel";
			this.tilePanel.Size = new System.Drawing.Size(281, 610);
			this.tilePanel.TabIndex = 9;
			// 
			// tileTabControl
			// 
			this.tileTabControl.Controls.Add(this.topTileTab);
			this.tileTabControl.Controls.Add(this.bottomTileTab);
			this.tileTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileTabControl.Location = new System.Drawing.Point(0, 0);
			this.tileTabControl.Name = "tileTabControl";
			this.tileTabControl.SelectedIndex = 0;
			this.tileTabControl.Size = new System.Drawing.Size(281, 610);
			this.tileTabControl.TabIndex = 11;
			this.tileTabControl.SelectedIndexChanged += new System.EventHandler(this.tileTabControl_SelectedIndexChanged);
			// 
			// topTileTab
			// 
			this.topTileTab.AutoScroll = true;
			this.topTileTab.Controls.Add(this.topTileGridBox);
			this.topTileTab.Location = new System.Drawing.Point(4, 22);
			this.topTileTab.Name = "topTileTab";
			this.topTileTab.Padding = new System.Windows.Forms.Padding(3);
			this.topTileTab.Size = new System.Drawing.Size(273, 584);
			this.topTileTab.TabIndex = 1;
			this.topTileTab.Text = "Top Tiles";
			this.topTileTab.UseVisualStyleBackColor = true;
			// 
			// bottomTileTab
			// 
			this.bottomTileTab.AutoScroll = true;
			this.bottomTileTab.Controls.Add(this.bottomTileGridBox);
			this.bottomTileTab.Location = new System.Drawing.Point(4, 22);
			this.bottomTileTab.Name = "bottomTileTab";
			this.bottomTileTab.Padding = new System.Windows.Forms.Padding(3);
			this.bottomTileTab.Size = new System.Drawing.Size(273, 584);
			this.bottomTileTab.TabIndex = 0;
			this.bottomTileTab.Text = "Bottom Tiles";
			this.bottomTileTab.UseVisualStyleBackColor = true;
			// 
			// mapPanel
			// 
			this.mapPanel.AutoScroll = true;
			this.mapPanel.Controls.Add(this.mapGridBox);
			this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapPanel.Location = new System.Drawing.Point(150, 49);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(853, 610);
			this.mapPanel.TabIndex = 10;
			// 
			// exportAsImageToolStripButton
			// 
			this.exportAsImageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.exportAsImageToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("exportAsImageToolStripButton.Image")));
			this.exportAsImageToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.exportAsImageToolStripButton.Name = "exportAsImageToolStripButton";
			this.exportAsImageToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.exportAsImageToolStripButton.Text = "exportAsImageToolStripButton";
			this.exportAsImageToolStripButton.Click += new System.EventHandler(this.exportAsImageToolStripButton_Click);
			// 
			// mapGridBox
			// 
			this.mapGridBox.AllowMultiSelection = false;
			this.mapGridBox.BoxSize = new System.Drawing.Size(16, 16);
			this.mapGridBox.CanvasSize = new System.Drawing.Size(128, 128);
			this.mapGridBox.HoverBox = true;
			this.mapGridBox.HoverColor = System.Drawing.Color.White;
			this.mapGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.mapGridBox.Location = new System.Drawing.Point(0, 0);
			this.mapGridBox.Name = "mapGridBox";
			this.mapGridBox.Selectable = false;
			this.mapGridBox.SelectedIndex = -1;
			this.mapGridBox.SelectionColor = System.Drawing.Color.Red;
			this.mapGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
			this.mapGridBox.Size = new System.Drawing.Size(128, 128);
			this.mapGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.mapGridBox.TabIndex = 10;
			this.mapGridBox.TabStop = false;
			this.mapGridBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapGridBox_MouseDown);
			this.mapGridBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapGridBox_MouseMove);
			// 
			// topTileGridBox
			// 
			this.topTileGridBox.AllowMultiSelection = false;
			this.topTileGridBox.BoxSize = new System.Drawing.Size(16, 16);
			this.topTileGridBox.CanvasSize = new System.Drawing.Size(128, 128);
			this.topTileGridBox.HoverBox = true;
			this.topTileGridBox.HoverColor = System.Drawing.Color.White;
			this.topTileGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.topTileGridBox.Location = new System.Drawing.Point(0, 0);
			this.topTileGridBox.Name = "topTileGridBox";
			this.topTileGridBox.Selectable = false;
			this.topTileGridBox.SelectedIndex = -1;
			this.topTileGridBox.SelectionColor = System.Drawing.Color.Red;
			this.topTileGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
			this.topTileGridBox.Size = new System.Drawing.Size(128, 128);
			this.topTileGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.topTileGridBox.TabIndex = 12;
			this.topTileGridBox.TabStop = false;
			this.topTileGridBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.topTileGridBox_MouseDown);
			// 
			// bottomTileGridBox
			// 
			this.bottomTileGridBox.AllowMultiSelection = false;
			this.bottomTileGridBox.BoxSize = new System.Drawing.Size(16, 16);
			this.bottomTileGridBox.CanvasSize = new System.Drawing.Size(128, 128);
			this.bottomTileGridBox.HoverBox = true;
			this.bottomTileGridBox.HoverColor = System.Drawing.Color.White;
			this.bottomTileGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.bottomTileGridBox.Location = new System.Drawing.Point(0, 0);
			this.bottomTileGridBox.Name = "bottomTileGridBox";
			this.bottomTileGridBox.Selectable = false;
			this.bottomTileGridBox.SelectedIndex = -1;
			this.bottomTileGridBox.SelectionColor = System.Drawing.Color.Red;
			this.bottomTileGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
			this.bottomTileGridBox.Size = new System.Drawing.Size(128, 128);
			this.bottomTileGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.bottomTileGridBox.TabIndex = 11;
			this.bottomTileGridBox.TabStop = false;
			this.bottomTileGridBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bottomTileGridBox_MouseDown);
			// 
			// MainWindow
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1284, 681);
			this.Controls.Add(this.mapPanel);
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.roomPanel);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.bottomStatusStrip);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "MainWindow";
			this.Text = "Minish Maker";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.bottomStatusStrip.ResumeLayout(false);
			this.bottomStatusStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.roomPanel.ResumeLayout(false);
			this.tilePanel.ResumeLayout(false);
			this.tileTabControl.ResumeLayout(false);
			this.topTileTab.ResumeLayout(false);
			this.topTileTab.PerformLayout();
			this.bottomTileTab.ResumeLayout(false);
			this.bottomTileTab.PerformLayout();
			this.mapPanel.ResumeLayout(false);
			this.mapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapGridBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.topTileGridBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bottomTileGridBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectRomToolStripMenuItem;
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
		private System.Windows.Forms.ToolStripMenuItem saveRoomChangesCtrlSToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem chestEditorStripMenuItem;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.TabControl tileTabControl;
        private System.Windows.Forms.TabPage bottomTileTab;
        private System.Windows.Forms.TabPage topTileTab;
        private GridBox mapGridBox;
        private GridBox bottomTileGridBox;
        private GridBox topTileGridBox;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bothLayersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomLayerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem metatileEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton chestToolStripButton;
        private System.Windows.Forms.ToolStripButton metatileToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton exportAsImageToolStripButton;
	}
}

