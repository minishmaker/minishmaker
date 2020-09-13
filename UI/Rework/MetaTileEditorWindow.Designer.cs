namespace MinishMaker.UI.Rework
{
    partial class MetaTileEditorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaTileEditor));
            this.metaTileSetPanel = new System.Windows.Forms.Panel();
            this.metaTileGridBox = new MinishMaker.UI.GridBoxComponent();
            this.tileSetPanel = new System.Windows.Forms.Panel();
            this.tileSetGridBox = new MinishMaker.UI.GridBoxComponent();
            this.selectedMetaTilePanel = new System.Windows.Forms.Panel();
            this.selectedMetaGridBox = new MinishMaker.UI.GridBoxComponent();
            this.selectedTilePanel = new System.Windows.Forms.Panel();
            this.selectedTileBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tLPalette = new System.Windows.Forms.TextBox();
            this.bLPalette = new System.Windows.Forms.TextBox();
            this.tRPalette = new System.Windows.Forms.TextBox();
            this.bRPalette = new System.Windows.Forms.TextBox();
            this.hFlipBox = new System.Windows.Forms.CheckBox();
            this.vFlipBox = new System.Windows.Forms.CheckBox();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.PaletteNum = new System.Windows.Forms.Label();
            this.layer1Button = new System.Windows.Forms.Button();
            this.layer2Button = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tileChange = new System.Windows.Forms.Button();
            this.tId1 = new System.Windows.Forms.TextBox();
            this.tId2 = new System.Windows.Forms.TextBox();
            this.tId3 = new System.Windows.Forms.TextBox();
            this.tId4 = new System.Windows.Forms.TextBox();
            this.sTId = new System.Windows.Forms.TextBox();
            this.mTId = new System.Windows.Forms.TextBox();
            this.HiddenLabel = new System.Windows.Forms.Label();
            this.mTType = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bg1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bg2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bg1ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bg2ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.commonToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.metaTileSetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metaTileGridBox)).BeginInit();
            this.tileSetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetGridBox)).BeginInit();
            this.selectedMetaTilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedMetaGridBox)).BeginInit();
            this.selectedTilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedTileBox)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // metaTileSetPanel
            // 
            this.metaTileSetPanel.AutoScroll = true;
            this.metaTileSetPanel.Controls.Add(this.metaTileGridBox);
            this.metaTileSetPanel.Location = new System.Drawing.Point(132, 42);
            this.metaTileSetPanel.Name = "metaTileSetPanel";
            this.metaTileSetPanel.Size = new System.Drawing.Size(275, 240);
            this.metaTileSetPanel.TabIndex = 1;
            // 
            // metaTileGridBox
            // 
            this.metaTileGridBox.AllowMultiSelection = false;
            this.metaTileGridBox.BoxSize = new System.Drawing.Size(16, 16);
            this.metaTileGridBox.CanvasSize = new System.Drawing.Size(128, 128);
            this.metaTileGridBox.HoverBox = true;
            this.metaTileGridBox.HoverColor = System.Drawing.Color.White;
            this.metaTileGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.metaTileGridBox.Location = new System.Drawing.Point(0, 0);
            this.metaTileGridBox.Name = "metaTileGridBox";
            this.metaTileGridBox.Selectable = false;
            this.metaTileGridBox.SelectedIndex = -1;
            this.metaTileGridBox.SelectionColor = System.Drawing.Color.Red;
            this.metaTileGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
            this.metaTileGridBox.Size = new System.Drawing.Size(128, 128);
            this.metaTileGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.metaTileGridBox.TabIndex = 1;
            this.metaTileGridBox.TabStop = false;
            this.metaTileGridBox.Click += new System.EventHandler(this.MetaTileGridBox_Click);
            // 
            // tileSetPanel
            // 
            this.tileSetPanel.AutoScroll = true;
            this.tileSetPanel.Controls.Add(this.tileSetGridBox);
            this.tileSetPanel.Location = new System.Drawing.Point(413, 42);
            this.tileSetPanel.Name = "tileSetPanel";
            this.tileSetPanel.Size = new System.Drawing.Size(256, 256);
            this.tileSetPanel.TabIndex = 2;
            // 
            // tileSetGridBox
            // 
            this.tileSetGridBox.AllowMultiSelection = false;
            this.tileSetGridBox.BoxSize = new System.Drawing.Size(8, 8);
            this.tileSetGridBox.CanvasSize = new System.Drawing.Size(128, 128);
            this.tileSetGridBox.HoverBox = true;
            this.tileSetGridBox.HoverColor = System.Drawing.Color.White;
            this.tileSetGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.tileSetGridBox.Location = new System.Drawing.Point(0, 0);
            this.tileSetGridBox.Name = "tileSetGridBox";
            this.tileSetGridBox.Selectable = false;
            this.tileSetGridBox.SelectedIndex = -1;
            this.tileSetGridBox.SelectionColor = System.Drawing.Color.Red;
            this.tileSetGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
            this.tileSetGridBox.Size = new System.Drawing.Size(128, 128);
            this.tileSetGridBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tileSetGridBox.TabIndex = 1;
            this.tileSetGridBox.TabStop = false;
            this.tileSetGridBox.Click += new System.EventHandler(this.TileSetGridBox_Click);
            // 
            // selectedMetaTilePanel
            // 
            this.selectedMetaTilePanel.AutoScroll = true;
            this.selectedMetaTilePanel.Controls.Add(this.selectedMetaGridBox);
            this.selectedMetaTilePanel.Location = new System.Drawing.Point(31, 42);
            this.selectedMetaTilePanel.Name = "selectedMetaTilePanel";
            this.selectedMetaTilePanel.Size = new System.Drawing.Size(64, 64);
            this.selectedMetaTilePanel.TabIndex = 2;
            // 
            // selectedMetaGridBox
            // 
            this.selectedMetaGridBox.AllowMultiSelection = false;
            this.selectedMetaGridBox.BoxSize = new System.Drawing.Size(32, 32);
            this.selectedMetaGridBox.CanvasSize = new System.Drawing.Size(64, 64);
            this.selectedMetaGridBox.HoverBox = true;
            this.selectedMetaGridBox.HoverColor = System.Drawing.Color.White;
            this.selectedMetaGridBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.selectedMetaGridBox.Location = new System.Drawing.Point(0, 0);
            this.selectedMetaGridBox.Name = "selectedMetaGridBox";
            this.selectedMetaGridBox.Selectable = false;
            this.selectedMetaGridBox.SelectedIndex = -1;
            this.selectedMetaGridBox.SelectionColor = System.Drawing.Color.White;
            this.selectedMetaGridBox.SelectionRectangle = new System.Drawing.Rectangle(-1, 0, 1, 1);
            this.selectedMetaGridBox.Size = new System.Drawing.Size(64, 64);
            this.selectedMetaGridBox.TabIndex = 2;
            this.selectedMetaGridBox.TabStop = false;
            // 
            // selectedTilePanel
            // 
            this.selectedTilePanel.AutoScroll = true;
            this.selectedTilePanel.Controls.Add(this.selectedTileBox);
            this.selectedTilePanel.Location = new System.Drawing.Point(11, 132);
            this.selectedTilePanel.Name = "selectedTilePanel";
            this.selectedTilePanel.Size = new System.Drawing.Size(32, 32);
            this.selectedTilePanel.TabIndex = 3;
            // 
            // selectedTileBox
            // 
            this.selectedTileBox.Location = new System.Drawing.Point(0, 0);
            this.selectedTileBox.Name = "selectedTileBox";
            this.selectedTileBox.Size = new System.Drawing.Size(16, 16);
            this.selectedTileBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.selectedTileBox.TabIndex = 1;
            this.selectedTileBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(-1, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 2);
            this.label1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(409, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2, 300);
            this.label2.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(128, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(2, 300);
            this.label3.TabIndex = 6;
            // 
            // tLPalette
            // 
            this.tLPalette.Location = new System.Drawing.Point(11, 30);
            this.tLPalette.MaxLength = 1;
            this.tLPalette.Name = "tLPalette";
            this.tLPalette.Size = new System.Drawing.Size(13, 20);
            this.tLPalette.TabIndex = 7;
            this.tLPalette.Text = "0";
            this.tLPalette.LostFocus += new System.EventHandler(this.TLPalette_LostFocus);
            // 
            // bLPalette
            // 
            this.bLPalette.Location = new System.Drawing.Point(11, 102);
            this.bLPalette.MaxLength = 1;
            this.bLPalette.Name = "bLPalette";
            this.bLPalette.Size = new System.Drawing.Size(13, 20);
            this.bLPalette.TabIndex = 8;
            this.bLPalette.Text = "0";
            this.bLPalette.LostFocus += new System.EventHandler(this.BLPalette_LostFocus);
            // 
            // tRPalette
            // 
            this.tRPalette.Location = new System.Drawing.Point(101, 30);
            this.tRPalette.MaxLength = 1;
            this.tRPalette.Name = "tRPalette";
            this.tRPalette.Size = new System.Drawing.Size(13, 20);
            this.tRPalette.TabIndex = 9;
            this.tRPalette.Text = "0";
            this.tRPalette.LostFocus += new System.EventHandler(this.TRPalette_LostFocus);
            // 
            // bRPalette
            // 
            this.bRPalette.Location = new System.Drawing.Point(101, 102);
            this.bRPalette.MaxLength = 1;
            this.bRPalette.Name = "bRPalette";
            this.bRPalette.Size = new System.Drawing.Size(13, 20);
            this.bRPalette.TabIndex = 10;
            this.bRPalette.Text = "0";
            this.bRPalette.LostFocus += new System.EventHandler(this.BRPalette_LostFocus);
            // 
            // hFlipBox
            // 
            this.hFlipBox.AutoSize = true;
            this.hFlipBox.Location = new System.Drawing.Point(11, 170);
            this.hFlipBox.Name = "hFlipBox";
            this.hFlipBox.Size = new System.Drawing.Size(92, 17);
            this.hFlipBox.TabIndex = 1;
            this.hFlipBox.Text = "Horizontal Flip";
            this.hFlipBox.UseVisualStyleBackColor = true;
            this.hFlipBox.CheckedChanged += new System.EventHandler(this.HFlip_CheckedChanged);
            // 
            // vFlipBox
            // 
            this.vFlipBox.AutoSize = true;
            this.vFlipBox.Location = new System.Drawing.Point(11, 193);
            this.vFlipBox.Name = "vFlipBox";
            this.vFlipBox.Size = new System.Drawing.Size(80, 17);
            this.vFlipBox.TabIndex = 11;
            this.vFlipBox.Text = "Vertical Flip";
            this.vFlipBox.UseVisualStyleBackColor = true;
            this.vFlipBox.CheckedChanged += new System.EventHandler(this.VFlipBox_CheckedChanged);
            // 
            // prevButton
            // 
            this.prevButton.Enabled = false;
            this.prevButton.Location = new System.Drawing.Point(10, 257);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(17, 23);
            this.prevButton.TabIndex = 12;
            this.prevButton.Text = "<";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(52, 257);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(17, 23);
            this.nextButton.TabIndex = 13;
            this.nextButton.Text = ">";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // PaletteNum
            // 
            this.PaletteNum.Location = new System.Drawing.Point(33, 262);
            this.PaletteNum.Name = "PaletteNum";
            this.PaletteNum.Size = new System.Drawing.Size(13, 13);
            this.PaletteNum.TabIndex = 14;
            this.PaletteNum.Text = "0";
            // 
            // layer1Button
            // 
            this.layer1Button.Enabled = false;
            this.layer1Button.Location = new System.Drawing.Point(75, 257);
            this.layer1Button.Name = "layer1Button";
            this.layer1Button.Size = new System.Drawing.Size(17, 23);
            this.layer1Button.TabIndex = 15;
            this.layer1Button.Text = "1";
            this.layer1Button.UseVisualStyleBackColor = true;
            this.layer1Button.Click += new System.EventHandler(this.Layer1Button_Click);
            // 
            // layer2Button
            // 
            this.layer2Button.Location = new System.Drawing.Point(98, 257);
            this.layer2Button.Name = "layer2Button";
            this.layer2Button.Size = new System.Drawing.Size(17, 23);
            this.layer2Button.TabIndex = 16;
            this.layer2Button.Text = "2";
            this.layer2Button.UseVisualStyleBackColor = true;
            this.layer2Button.Click += new System.EventHandler(this.Layer2Button_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Palette";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(77, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Layer";
            // 
            // tileChange
            // 
            this.tileChange.Location = new System.Drawing.Point(10, 216);
            this.tileChange.Name = "tileChange";
            this.tileChange.Size = new System.Drawing.Size(114, 22);
            this.tileChange.TabIndex = 20;
            this.tileChange.Text = "Apply Tile Change";
            this.tileChange.UseVisualStyleBackColor = true;
            this.tileChange.Click += new System.EventHandler(this.TileChange_Click);
            // 
            // tId1
            // 
            this.tId1.Enabled = false;
            this.tId1.Location = new System.Drawing.Point(-1, 54);
            this.tId1.MaxLength = 3;
            this.tId1.Name = "tId1";
            this.tId1.Size = new System.Drawing.Size(25, 20);
            this.tId1.TabIndex = 21;
            this.tId1.Text = "FFF";
            // 
            // tId2
            // 
            this.tId2.Enabled = false;
            this.tId2.Location = new System.Drawing.Point(101, 54);
            this.tId2.MaxLength = 3;
            this.tId2.Name = "tId2";
            this.tId2.Size = new System.Drawing.Size(25, 20);
            this.tId2.TabIndex = 22;
            this.tId2.Text = "FFF";
            // 
            // tId3
            // 
            this.tId3.Enabled = false;
            this.tId3.Location = new System.Drawing.Point(-1, 80);
            this.tId3.MaxLength = 3;
            this.tId3.Name = "tId3";
            this.tId3.Size = new System.Drawing.Size(25, 20);
            this.tId3.TabIndex = 23;
            this.tId3.Text = "FFF";
            // 
            // tId4
            // 
            this.tId4.Enabled = false;
            this.tId4.Location = new System.Drawing.Point(101, 80);
            this.tId4.MaxLength = 3;
            this.tId4.Name = "tId4";
            this.tId4.Size = new System.Drawing.Size(25, 20);
            this.tId4.TabIndex = 24;
            this.tId4.Text = "FFF";
            // 
            // sTId
            // 
            this.sTId.Enabled = false;
            this.sTId.Location = new System.Drawing.Point(44, 141);
            this.sTId.MaxLength = 3;
            this.sTId.Name = "sTId";
            this.sTId.Size = new System.Drawing.Size(25, 20);
            this.sTId.TabIndex = 25;
            this.sTId.Text = "FFF";
            // 
            // mTId
            // 
            this.mTId.Enabled = false;
            this.mTId.Location = new System.Drawing.Point(34, 106);
            this.mTId.MaxLength = 3;
            this.mTId.Name = "mTId";
            this.mTId.Size = new System.Drawing.Size(25, 20);
            this.mTId.TabIndex = 26;
            this.mTId.Text = "FFF";
            // 
            // HiddenLabel
            // 
            this.HiddenLabel.AutoSize = true;
            this.HiddenLabel.Location = new System.Drawing.Point(77, 141);
            this.HiddenLabel.Name = "HiddenLabel";
            this.HiddenLabel.Size = new System.Drawing.Size(0, 13);
            this.HiddenLabel.TabIndex = 27;
            // 
            // mTType
            // 
            this.mTType.Enabled = false;
            this.mTType.Location = new System.Drawing.Point(67, 106);
            this.mTType.MaxLength = 3;
            this.mTType.Name = "mTType";
            this.mTType.Size = new System.Drawing.Size(25, 20);
            this.mTType.TabIndex = 28;
            this.mTType.Text = "FFF";
            this.mTType.LostFocus += new System.EventHandler(this.MTType_LostFocus);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(675, 24);
            this.menuStrip.TabIndex = 29;
            this.menuStrip.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bg1ToolStripMenuItem,
            this.bg2ToolStripMenuItem,
            this.commonToolStripMenuItem,
            this.paletteToolStripMenuItem});
            this.importToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importToolStripMenuItem.Image")));
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // bg1ToolStripMenuItem
            // 
            this.bg1ToolStripMenuItem.Name = "bg1ToolStripMenuItem";
            this.bg1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bg1ToolStripMenuItem.Text = "Bg1";
            this.bg1ToolStripMenuItem.Click += new System.EventHandler(this.Bg1ToolStripMenuItem_Click);
            // 
            // bg2ToolStripMenuItem
            // 
            this.bg2ToolStripMenuItem.Name = "bg2ToolStripMenuItem";
            this.bg2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bg2ToolStripMenuItem.Text = "Bg2";
            this.bg2ToolStripMenuItem.Click += new System.EventHandler(this.Bg2ToolStripMenuItem_Click);
            // 
            // commonToolStripMenuItem
            // 
            this.commonToolStripMenuItem.Name = "commonToolStripMenuItem";
            this.commonToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.commonToolStripMenuItem.Text = "Common";
            this.commonToolStripMenuItem.Click += new System.EventHandler(this.CommonToolStripMenuItem_Click);
            // 
            // paletteToolStripMenuItem
            // 
            this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
            this.paletteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.paletteToolStripMenuItem.Text = "Palette";
            this.paletteToolStripMenuItem.Click += new System.EventHandler(this.paletteToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bg1ToolStripMenuItem1,
            this.bg2ToolStripMenuItem1,
            this.commonToolStripMenuItem1,
            this.paletteToolStripMenuItem1});
            this.exportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportToolStripMenuItem.Image")));
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // bg1ToolStripMenuItem1
            // 
            this.bg1ToolStripMenuItem1.Name = "bg1ToolStripMenuItem1";
            this.bg1ToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.bg1ToolStripMenuItem1.Text = "Bg1";
            this.bg1ToolStripMenuItem1.Click += new System.EventHandler(this.Bg1ToolStripMenuItem1_Click);
            // 
            // bg2ToolStripMenuItem1
            // 
            this.bg2ToolStripMenuItem1.Name = "bg2ToolStripMenuItem1";
            this.bg2ToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.bg2ToolStripMenuItem1.Text = "Bg2";
            this.bg2ToolStripMenuItem1.Click += new System.EventHandler(this.Bg2ToolStripMenuItem1_Click);
            // 
            // commonToolStripMenuItem1
            // 
            this.commonToolStripMenuItem1.Name = "commonToolStripMenuItem1";
            this.commonToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.commonToolStripMenuItem1.Text = "Common";
            this.commonToolStripMenuItem1.Click += new System.EventHandler(this.CommonToolStripMenuItem1_Click);
            // 
            // paletteToolStripMenuItem1
            // 
            this.paletteToolStripMenuItem1.Name = "paletteToolStripMenuItem1";
            this.paletteToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.paletteToolStripMenuItem1.Text = "Palette";
            this.paletteToolStripMenuItem1.Click += new System.EventHandler(this.PaletteToolStripMenuItem1_Click);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(0, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(680, 2);
            this.label6.TabIndex = 30;
            // 
            // MetaTileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 305);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.mTType);
            this.Controls.Add(this.HiddenLabel);
            this.Controls.Add(this.mTId);
            this.Controls.Add(this.sTId);
            this.Controls.Add(this.tId4);
            this.Controls.Add(this.tId3);
            this.Controls.Add(this.tId2);
            this.Controls.Add(this.tId1);
            this.Controls.Add(this.tileChange);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.layer2Button);
            this.Controls.Add(this.layer1Button);
            this.Controls.Add(this.PaletteNum);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.vFlipBox);
            this.Controls.Add(this.hFlipBox);
            this.Controls.Add(this.bRPalette);
            this.Controls.Add(this.tRPalette);
            this.Controls.Add(this.bLPalette);
            this.Controls.Add(this.tLPalette);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectedTilePanel);
            this.Controls.Add(this.selectedMetaTilePanel);
            this.Controls.Add(this.tileSetPanel);
            this.Controls.Add(this.metaTileSetPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetaTileEditor";
            this.Text = "Metatile Editor";
            this.metaTileSetPanel.ResumeLayout(false);
            this.metaTileSetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metaTileGridBox)).EndInit();
            this.tileSetPanel.ResumeLayout(false);
            this.tileSetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSetGridBox)).EndInit();
            this.selectedMetaTilePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectedMetaGridBox)).EndInit();
            this.selectedTilePanel.ResumeLayout(false);
            this.selectedTilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedTileBox)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel metaTileSetPanel;
        private System.Windows.Forms.Panel tileSetPanel;
        private System.Windows.Forms.Panel selectedMetaTilePanel;
        private System.Windows.Forms.Panel selectedTilePanel;
        private System.Windows.Forms.PictureBox selectedTileBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tLPalette;
        private System.Windows.Forms.TextBox bLPalette;
        private System.Windows.Forms.TextBox tRPalette;
        private System.Windows.Forms.TextBox bRPalette;
        private System.Windows.Forms.CheckBox hFlipBox;
        private System.Windows.Forms.CheckBox vFlipBox;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Label PaletteNum;
        private System.Windows.Forms.Button layer1Button;
        private System.Windows.Forms.Button layer2Button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button tileChange;
        private System.Windows.Forms.TextBox tId1;
        private System.Windows.Forms.TextBox tId2;
        private System.Windows.Forms.TextBox tId3;
        private System.Windows.Forms.TextBox tId4;
        private System.Windows.Forms.TextBox sTId;
        private System.Windows.Forms.TextBox mTId;
        private GridBoxComponent metaTileGridBox;
        private GridBoxComponent tileSetGridBox;
        private GridBoxComponent selectedMetaGridBox;
        private System.Windows.Forms.Label HiddenLabel;
        private System.Windows.Forms.TextBox mTType;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bg1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bg2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bg1ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bg2ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem commonToolStripMenuItem1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem1;
    }
}
