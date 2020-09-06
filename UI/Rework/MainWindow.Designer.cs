using System;

namespace MinishMaker.UI.Rework
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resizeRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.topLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bottomLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bothLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chestEditorStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.metatileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.areaEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objectPlacementEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.warpEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bottomStatusStrip = new System.Windows.Forms.StatusStrip();
			this.statusXposText = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusYposText = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusAreaIdText = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusRoomIdText = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.buildToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.chestToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.metatileToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.areaToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.objectPlacementToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.warpToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.roomPanel = new System.Windows.Forms.Panel();
			this.roomTreeView = new System.Windows.Forms.TreeView();
			this.tilePanel = new System.Windows.Forms.Panel();
			this.tileTabControl = new System.Windows.Forms.TabControl();
			this.topTileTab = new System.Windows.Forms.TabPage();
			this.topTileGridBox = new MinishMaker.UI.GridBoxComponent();
			this.bottomTileTab = new System.Windows.Forms.TabPage();
			this.bottomTileGridBox = new MinishMaker.UI.GridBoxComponent();
			this.mapPanel = new System.Windows.Forms.Panel();
			this.mapGridBox = new MinishMaker.UI.GridBoxComponent();
			this.nodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.bottomStatusStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.roomPanel.SuspendLayout();
			this.tilePanel.SuspendLayout();
			this.tileTabControl.SuspendLayout();
			this.topTileTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.topTileGridBox)).BeginInit();
			this.bottomTileTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bottomTileGridBox)).BeginInit();
			this.mapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapGridBox)).BeginInit();
			this.nodeContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
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
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.buildProjectToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newProjectToolStripMenuItem
			// 
			this.newProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripMenuItem.Image")));
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.newProjectToolStripMenuItem.Text = "New Project";
			this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.NewProjectToolStripMenuItem_Click);
			// 
			// openProjectToolStripMenuItem
			// 
			this.openProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openProjectToolStripMenuItem.Image")));
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.openProjectToolStripMenuItem.Text = "Open Project";
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectToolStripMenuItem_Click);
			// 
			// saveProjectToolStripMenuItem
			// 
			this.saveProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveProjectToolStripMenuItem.Image")));
			this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
			this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveProjectToolStripMenuItem.Text = "Save Project";
			this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.SaveProjectToolStripMenuItem_Click);
			// 
			// buildProjectToolStripMenuItem
			// 
			this.buildProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("buildProjectToolStripMenuItem.Image")));
			this.buildProjectToolStripMenuItem.Name = "buildProjectToolStripMenuItem";
			this.buildProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.buildProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.buildProjectToolStripMenuItem.Text = "Build Project";
			this.buildProjectToolStripMenuItem.Click += new System.EventHandler(this.BuildProjectToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(183, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitButtonClick);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resizeRoomToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// resizeRoomToolStripMenuItem
			// 
			this.resizeRoomToolStripMenuItem.Name = "resizeRoomToolStripMenuItem";
			this.resizeRoomToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.resizeRoomToolStripMenuItem.Text = "Resize Room";
			this.resizeRoomToolStripMenuItem.Click += new System.EventHandler(this.ResizeRoomToolStripMenuItem_Click);
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
			this.topLayerToolStripMenuItem.Click += new System.EventHandler(this.TopLayerToolStripMenuItem_Click);
			// 
			// bottomLayerToolStripMenuItem
			// 
			this.bottomLayerToolStripMenuItem.Name = "bottomLayerToolStripMenuItem";
			this.bottomLayerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
			this.bottomLayerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.bottomLayerToolStripMenuItem.Text = "Bottom Layer";
			this.bottomLayerToolStripMenuItem.Click += new System.EventHandler(this.BottomLayerToolStripMenuItem_Click);
			// 
			// bothLayersToolStripMenuItem
			// 
			this.bothLayersToolStripMenuItem.Name = "bothLayersToolStripMenuItem";
			this.bothLayersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
			this.bothLayersToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.bothLayersToolStripMenuItem.Text = "Both Layers";
			this.bothLayersToolStripMenuItem.Click += new System.EventHandler(this.BothLayersToolStripMenuItem_Click);
			// 
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chestEditorStripMenuItem,
            this.metatileEditorToolStripMenuItem,
            this.areaEditorToolStripMenuItem,
            this.objectPlacementEditorToolStripMenuItem,
            this.warpEditorToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.windowToolStripMenuItem.Text = "Window";
			// 
			// chestEditorStripMenuItem
			// 
			this.chestEditorStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("chestEditorStripMenuItem.Image")));
			this.chestEditorStripMenuItem.Name = "chestEditorStripMenuItem";
			this.chestEditorStripMenuItem.ShowShortcutKeys = false;
			this.chestEditorStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.chestEditorStripMenuItem.Text = "Chest Editor";
			// 
			// metatileEditorToolStripMenuItem
			// 
			this.metatileEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("metatileEditorToolStripMenuItem.Image")));
			this.metatileEditorToolStripMenuItem.Name = "metatileEditorToolStripMenuItem";
			this.metatileEditorToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.metatileEditorToolStripMenuItem.Text = "Metatile Editor";
			// 
			// areaEditorToolStripMenuItem
			// 
			this.areaEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("areaEditorToolStripMenuItem.Image")));
			this.areaEditorToolStripMenuItem.Name = "areaEditorToolStripMenuItem";
			this.areaEditorToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.areaEditorToolStripMenuItem.Text = "Area Editor";
			// 
			// objectPlacementEditorToolStripMenuItem
			// 
			this.objectPlacementEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("objectPlacementEditorToolStripMenuItem.Image")));
			this.objectPlacementEditorToolStripMenuItem.Name = "objectPlacementEditorToolStripMenuItem";
			this.objectPlacementEditorToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.objectPlacementEditorToolStripMenuItem.Text = "Object Placement Editor";
			// 
			// warpEditorToolStripMenuItem
			// 
			this.warpEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("warpEditorToolStripMenuItem.Image")));
			this.warpEditorToolStripMenuItem.Name = "warpEditorToolStripMenuItem";
			this.warpEditorToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.warpEditorToolStripMenuItem.Text = "Warp Editor";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripMenuItem,
            this.toolStripSeparator2,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// documentationToolStripMenuItem
			// 
			this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
			this.documentationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.documentationToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.documentationToolStripMenuItem.Text = "Documentation";
			this.documentationToolStripMenuItem.Click += new System.EventHandler(this.DocumentationToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutButtonClick);
			// 
			// bottomStatusStrip
			// 
			this.bottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusXposText,
            this.statusYposText,
            this.statusAreaIdText,
            this.statusRoomIdText,
            this.statusText});
			this.bottomStatusStrip.Location = new System.Drawing.Point(0, 659);
			this.bottomStatusStrip.Name = "bottomStatusStrip";
			this.bottomStatusStrip.Size = new System.Drawing.Size(1284, 22);
			this.bottomStatusStrip.TabIndex = 7;
			this.bottomStatusStrip.Text = "bottomStatusStrip";
			// 
			// statusXposText
			// 
			this.statusXposText.AutoSize = false;
			this.statusXposText.Name = "statusXposText";
			this.statusXposText.Size = new System.Drawing.Size(29, 17);
			this.statusXposText.Text = "X:00";
			this.statusXposText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// statusYposText
			// 
			this.statusYposText.AutoSize = false;
			this.statusYposText.Name = "statusYposText";
			this.statusYposText.Size = new System.Drawing.Size(29, 17);
			this.statusYposText.Text = "Y:00";
			this.statusYposText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// statusAreaIdText
			// 
			this.statusAreaIdText.AutoSize = false;
			this.statusAreaIdText.Name = "statusAreaIdText";
			this.statusAreaIdText.Size = new System.Drawing.Size(60, 17);
			this.statusAreaIdText.Text = "Area Id:00";
			this.statusAreaIdText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// statusRoomIdText
			// 
			this.statusRoomIdText.AutoSize = false;
			this.statusRoomIdText.Name = "statusRoomIdText";
			this.statusRoomIdText.Size = new System.Drawing.Size(65, 17);
			this.statusRoomIdText.Text = "Room Id:00";
			this.statusRoomIdText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.buildToolStripButton,
            this.toolStripSeparator1,
            this.chestToolStripButton,
            this.metatileToolStripButton,
            this.areaToolStripButton,
            this.objectPlacementToolStripButton,
            this.warpToolStripButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.toolStrip.Size = new System.Drawing.Size(1284, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip";
			// 
			// newToolStripButton
			// 
			this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
			this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newToolStripButton.Name = "newToolStripButton";
			this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.newToolStripButton.Text = "newToolStripButton";
			this.newToolStripButton.ToolTipText = "Create a project.";
			this.newToolStripButton.Click += new System.EventHandler(this.NewToolStripButton_Click);
			// 
			// openToolStripButton
			// 
			this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
			this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openToolStripButton.Name = "openToolStripButton";
			this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openToolStripButton.Text = "toolStripButton1";
			this.openToolStripButton.ToolTipText = "Open a project.";
			this.openToolStripButton.Click += new System.EventHandler(this.OpenToolStripButton_Click);
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
			this.saveToolStripButton.Click += new System.EventHandler(this.SaveToolStripButton_Click);
			// 
			// buildToolStripButton
			// 
			this.buildToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buildToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("buildToolStripButton.Image")));
			this.buildToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buildToolStripButton.Name = "buildToolStripButton";
			this.buildToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.buildToolStripButton.Text = "buildToolStripButton";
			this.buildToolStripButton.ToolTipText = "Build project.";
			this.buildToolStripButton.Click += new System.EventHandler(this.BuildToolStripButton_Click);
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
			// 
			// metatileToolStripButton
			// 
			this.metatileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.metatileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("metatileToolStripButton.Image")));
			this.metatileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.metatileToolStripButton.Name = "metatileToolStripButton";
			this.metatileToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.metatileToolStripButton.Text = "Metatile Editor";
			// 
			// areaToolStripButton
			// 
			this.areaToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.areaToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("areaToolStripButton.Image")));
			this.areaToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.areaToolStripButton.Name = "areaToolStripButton";
			this.areaToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.areaToolStripButton.Text = "Area Editor";
			// 
			// objectPlacementToolStripButton
			// 
			this.objectPlacementToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.objectPlacementToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("objectPlacementToolStripButton.Image")));
			this.objectPlacementToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.objectPlacementToolStripButton.Name = "objectPlacementToolStripButton";
			this.objectPlacementToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.objectPlacementToolStripButton.Text = "toolStripButton1";
			this.objectPlacementToolStripButton.ToolTipText = "Object Placement Editor";
			// 
			// warpToolStripButton
			// 
			this.warpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.warpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("warpToolStripButton.Image")));
			this.warpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.warpToolStripButton.Name = "warpToolStripButton";
			this.warpToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.warpToolStripButton.Text = "toolStripButton2";
			this.warpToolStripButton.ToolTipText = "Warp Editor";
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
			this.roomTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.roomTreeView_NodeMouseClick);
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
			// mapPanel
			// 
			this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mapPanel.AutoScroll = true;
			this.mapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.mapPanel.Controls.Add(this.mapGridBox);
			this.mapPanel.Location = new System.Drawing.Point(150, 49);
			this.mapPanel.MinimumSize = new System.Drawing.Size(256, 256);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(853, 610);
			this.mapPanel.TabIndex = 10;
			// 
			// mapGridBox
			// 
			this.mapGridBox.AllowMultiSelection = false;
			this.mapGridBox.BoxSize = new System.Drawing.Size(16, 16);
			this.mapGridBox.CanvasSize = new System.Drawing.Size(256, 256);
			this.mapGridBox.HoverBox = true;
			this.mapGridBox.HoverColor = System.Drawing.Color.White;
			this.mapGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.mapGridBox.Location = new System.Drawing.Point(0, 0);
			this.mapGridBox.Name = "mapGridBox";
			this.mapGridBox.Selectable = false;
			this.mapGridBox.SelectedIndex = -1;
			this.mapGridBox.SelectionColor = System.Drawing.Color.Red;
			this.mapGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
			this.mapGridBox.Size = new System.Drawing.Size(256, 256);
			this.mapGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.mapGridBox.TabIndex = 10;
			this.mapGridBox.TabStop = false;
			this.mapGridBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapGridBox_MouseDown);
			this.mapGridBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapGridBox_MouseMove);
			// 
			// nodeContextMenu
			// 
			this.nodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem});
			this.nodeContextMenu.Name = "NodeContextMenu";
			this.nodeContextMenu.Size = new System.Drawing.Size(118, 26);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameToolStripMenuItem_Click);
			// 
			// MainWindow
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1284, 681);
			this.Controls.Add(this.roomPanel);
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.mapPanel);
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
			((System.ComponentModel.ISupportInitialize)(this.topTileGridBox)).EndInit();
			this.bottomTileTab.ResumeLayout(false);
			this.bottomTileTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bottomTileGridBox)).EndInit();
			this.mapPanel.ResumeLayout(false);
			this.mapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapGridBox)).EndInit();
			this.nodeContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chestEditorStripMenuItem;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.TabControl tileTabControl;
        private System.Windows.Forms.TabPage bottomTileTab;
        private System.Windows.Forms.TabPage topTileTab;
        private GridBoxComponent mapGridBox;
        private GridBoxComponent bottomTileGridBox;
        private GridBoxComponent topTileGridBox;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bothLayersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metatileEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton chestToolStripButton;
        private System.Windows.Forms.ToolStripButton metatileToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem areaEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectPlacementEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem warpEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton areaToolStripButton;
        private System.Windows.Forms.ToolStripButton objectPlacementToolStripButton;
        private System.Windows.Forms.ToolStripButton warpToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel statusXposText;
        private System.Windows.Forms.ToolStripStatusLabel statusRoomIdText;
        private System.Windows.Forms.ToolStripStatusLabel statusYposText;
        private System.Windows.Forms.ToolStripStatusLabel statusAreaIdText;
        private System.Windows.Forms.ContextMenuStrip nodeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeRoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton buildToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}

