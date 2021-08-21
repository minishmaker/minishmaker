using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using MinishMaker.Core;
using MinishMaker.Utilities;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Properties;
using MinishMaker.Core.Rework;
using PaletteException = MinishMaker.Core.Rework.PaletteException;

namespace MinishMaker.UI.Rework
{
    public partial class MainWindow : Form
    {
        public static MainWindow instance;
        private Core.Rework.Project project_;

        private Utilities.Rework.MapManager mapManager_;
        private List<SubWindowHolder> subWindows = new List<SubWindowHolder>();
        private NewProjectWindow newProjectWindow = null;
        private RenameDialog renameWindow = null;

        private Bitmap[] mapLayers;
        private Bitmap[] tileMaps;

        public Core.Rework.Room currentRoom = null;
        private int selectedTileData = -1;
        private int selectedLayer = 2; //start with bg2
        private Point lastTilePos;
        private ViewLayer viewLayer = ViewLayer.Both;
        private int currentScale = 1;

        struct RepointData
        {
            public int areaIndex;
            public int roomIndex;
            public Core.Rework.DataType type;
            public int start;
            public int size;


            public RepointData(int areaIndex, int roomIndex, Core.Rework.DataType type, int start, int size)
            {
                this.areaIndex = areaIndex;
                this.roomIndex = roomIndex;
                this.type = type;
                this.start = start;
                this.size = size;
            }
        }


        public enum ViewLayer
        {
            Both,
            Top,
            Bottom
        }

        public MainWindow()
        {
            InitializeComponent();
            EnableEditor(false);
            UpdateWindowTitle();
            SetupSubWindows();
            instance = this;
        }

        private void UpdateWindowTitle()
        {
#if DEBUG
            this.Text = $"{ProductName} {AssemblyInfo.GetGitTag()} DEBUG-{AssemblyInfo.GetGitHash()}";
#else
            this.Text = $"{ProductName} {AssemblyInfo.GetGitTag()}";
#endif
        }

        #region MenuBarButtons

        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void OpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void SaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllChanges();
        }

        private void BuildProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildProject();
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
        }
        private void ResizeRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizeRoom();
        }

        private void TopLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateViewLayer(ViewLayer.Top);
        }

        private void BottomLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateViewLayer(ViewLayer.Bottom);
        }

        private void BothLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateViewLayer(ViewLayer.Both);
        }

        private void DocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://docs.minishmaker.com/minish-maker/minish-maker");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void AboutButtonClick(object sender, EventArgs e)
        {
            Form aboutWindow = new AboutDialog();
            aboutWindow.Show();
        }
        #endregion

        #region ToolStripButtons

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveAllChanges();
        }
        private void BuildToolStripButton_Click(object sender, EventArgs e)
        {
            BuildProject();
        }

        private void SetupSubWindows()
        {
            //subWindows.Add(new SubWindowHolder(new ChestEditorWindow(this), chestEditorStripMenuItem, chestToolStripButton));//chest
            subWindows.Add(new SubWindowHolder(new MetaTileEditorWindow(), metatileEditorToolStripMenuItem, metatileToolStripButton));//metatile
            subWindows.Add(new SubWindowHolder(new AreaEditorWindow(), areaEditorToolStripMenuItem, areaToolStripButton));//area
            subWindows.Add(new SubWindowHolder(new ObjectPlacementEditorWindow(), objectPlacementEditorToolStripMenuItem, objectPlacementToolStripButton)); //object
            subWindows.Add(new SubWindowHolder(new WarpEditorWindow(), warpEditorToolStripMenuItem, warpToolStripButton)); //warp
        }
        #endregion

        #region OtherInteractions

        // Other interactions
        private void tileTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLayer = tileTabControl.SelectedIndex + 1;

        }

        private void MainWindow_DragDrop(object sender, DragEventArgs e)
        {

        }
        #endregion

        #region ProjectManagement

        private void NewProject()
        {
            if (newProjectWindow != null)
                return;

            newProjectWindow = new NewProjectWindow();
            newProjectWindow.FormClosed += OnNewProjectWindowClosed;
            newProjectWindow.Show();
        }

        private void OnNewProjectWindowClosed(object sender, FormClosedEventArgs e)
        {
            if (newProjectWindow.project != null)
            {
                if (renameWindow != null)
                    renameWindow.Close();

                project_ = newProjectWindow.project2;
                if (project_.Loaded)
                {
                    LoadProjectData();
                    EnableEditor(true);
                    statusText.Text = "Created new project: " + project_.ProjectPath + "/" + project_.ProjectName + ".mmproj";
                }
                else
                    statusText.Text = "Could not load project.";
            }
            else
                statusText.Text = "Project creation aborted.";
            newProjectWindow = null;
        }

        private void OpenProject()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Minish Maker Project|*.mmproj|All Files|*.*",
                Title = "Select Project File"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }


            if (renameWindow != null)
                renameWindow.Hide();

            subWindows.ForEach(sw => sw.HideWindow());

            if (project_ == null)
            {
                project_ = new Core.Rework.Project(ofd.FileName);
            }

            if (project_.Loaded)
            {
                LoadProjectData();
                EnableEditor(true);
                statusText.Text = "Loaded: " + ofd.FileName;
            }
            else
            {
                statusText.Text = "Project load failed.";
            }
        }

        private void LoadProjectData()
        {
            mapGridBox.Image = new Bitmap(1, 1); //reset some things on loading a rom
            bottomTileGridBox.Image = new Bitmap(1, 1);
            topTileGridBox.Image = new Bitmap(1, 1);
            currentRoom = null;
            selectedTileData = -1;
            selectedLayer = 2;
            LoadMaps();
        }

        private void EnableEditor(bool enabled)
        {
            // Enable or disable all components
            saveProjectToolStripMenuItem.Enabled = enabled;
            buildProjectToolStripMenuItem.Enabled = enabled;

            resizeRoomToolStripMenuItem.Enabled = enabled;

            viewToolStripMenuItem.Enabled = enabled;

            chestEditorStripMenuItem.Enabled = enabled;
            metatileEditorToolStripMenuItem.Enabled = enabled;
            areaEditorToolStripMenuItem.Enabled = enabled;
            objectPlacementEditorToolStripMenuItem.Enabled = enabled;
            warpEditorToolStripMenuItem.Enabled = enabled;

            saveToolStripButton.Enabled = enabled;
            buildToolStripButton.Enabled = enabled;

            chestToolStripButton.Enabled = enabled;
            metatileToolStripButton.Enabled = enabled;
            areaToolStripButton.Enabled = enabled;
            objectPlacementToolStripButton.Enabled = enabled;
            warpToolStripButton.Enabled = enabled;
        }

        private void BuildProject()
        {
            if (project_ == null)
            {
                MessageBox.Show("No project loaded!");
                return;
            }
            // TODO check for pending changes before building, and prompt user
            SaveAllChanges();
            if (project_.BuildProject())
            {
                MessageBox.Show("Build Completed!");
                statusText.Text = "Build Completed. Output file: " + project_.ProjectPath + "\\" + project_.ProjectName + ".gba";
            }
            else
            {
                MessageBox.Show("There was a problem building the project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusText.Text = "Failed to build project.";
            }
            // TODO check for build completing correctly, probably needs deeper integration with ColorzCore

        }

        #endregion

        private void LoadMaps()
        {
            mapManager_ = Utilities.Rework.MapManager.Get();

            roomTreeView.Nodes.Clear();
            // Set up room list
            roomTreeView.BeginUpdate();
            int subsection = 0;

            foreach (Area area in mapManager_.GetAllAreas())
            {
                var areaName = "Area " + StringUtil.AsStringHex2(area.Id);
                var areaKey = new Tuple<int, int>(area.Id, -1);

                if (project_.roomNames.ContainsKey(areaKey))
                {
                    areaName = project_.roomNames[areaKey];
                }

                var areaNode = roomTreeView.Nodes.Add(areaName);
                areaNode.Name = StringUtil.AsStringHex(area.Id, 1);

                foreach (Core.Rework.Room room in area.GetAllRooms())
                {
                    var roomName = "Room " + StringUtil.AsStringHex2(room.Id);
                    var roomKey = new Tuple<int, int>(area.Id, room.Id);

                    if (project_.roomNames.ContainsKey(roomKey))
                    {
                        roomName = project_.roomNames[roomKey];
                    }

                    var roomNode = areaNode.Nodes.Add(roomName);
                    roomNode.Name = StringUtil.AsStringHex(room.Id, 1);
                }

                subsection++;
            }

            roomTreeView.EndUpdate();
        }

        private void roomTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                Console.WriteLine(e.Node.Parent.Name + " " + e.Node.Name);
                int areaIndex = Convert.ToInt32(e.Node.Parent.Name, 16);
                int roomIndex = Convert.ToInt32(e.Node.Name, 16);
                statusRoomIdText.Text = "Room Id:" + roomIndex.Hex().PadLeft(2, '0'); ;
                statusAreaIdText.Text = "Area Id:" + areaIndex.Hex().PadLeft(2, '0'); ;
                ChangeRoom(areaIndex, roomIndex);
            }
        }

        private void roomTreeView_NodeMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode ClickNode = roomTreeView.GetNodeAt(ClickPoint);
                if (ClickNode == null) return;
                roomTreeView.SelectedNode = ClickNode;
                // Convert from Tree coordinates to Screen coordinates    
                Point ScreenPoint = roomTreeView.PointToScreen(ClickPoint);
                // Convert from Screen coordinates to Form coordinates    
                Point FormPoint = this.PointToClient(ScreenPoint);
                // Show context menu   
                nodeContextMenu.Show(ScreenPoint);
            }
        }



        public void ChangeNodeName(string areaNodeName, string newAreaName, string roomNodeName, string newRoomName)
        {
            TreeNode parentNode = null;
            foreach (TreeNode node in roomTreeView.Nodes)
            {
                if (node.Name == areaNodeName)
                {
                    parentNode = node;
                    node.Text = newAreaName;
                    break;
                }
            }

            if (newRoomName == "")
                return;

            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Name == roomNodeName)
                {
                    node.Text = newRoomName;
                    break;
                }
            }
        }

        private void SaveAllChanges()
        {
            if (project_ == null)
                return;

            project_.StartSave();
            project_.Save();
            project_.EndSave();

            project_.CreateProjectFile();

            MessageBox.Show("Project Saved");
        }

        public void HighlightChest(int tileX, int tileY)
        {
            mapGridBox.chestHighlightPoint = new Point(tileX, tileY);
            mapGridBox.Invalidate();
        }

        public void HighlightListObject(int pixelX, int pixelY)
        {
            mapGridBox.listObjectHighlightPoint = new Point(pixelX, pixelY);
            mapGridBox.Invalidate();
        }

        public void HighlightWarp(int pixelX, int pixelY)
        {
            mapGridBox.warpHighlightPoint = new Point(pixelX, pixelY);
            mapGridBox.Invalidate();
        }

        private void UpdateViewLayer(ViewLayer layer)
        {
            if (currentRoom == null)
                return;

            switch (layer)
            {
                case ViewLayer.Both:
                    mapGridBox.Image = DrawingUtil.OverlayImage(mapLayers[1], mapLayers[0]);
                    viewLayer = ViewLayer.Both;
                    topTileTab.Enabled = true;
                    bottomTileTab.Enabled = true;
                    break;
                case ViewLayer.Top:
                    mapGridBox.Image = mapLayers[0];
                    tileTabControl.SelectedIndex = 0;
                    viewLayer = ViewLayer.Top;
                    selectedTileData = topTileGridBox.SelectedIndex;
                    topTileTab.Enabled = true;
                    bottomTileTab.Enabled = false;
                    break;
                case ViewLayer.Bottom:
                    mapGridBox.Image = mapLayers[1];
                    tileTabControl.SelectedIndex = 1;
                    viewLayer = ViewLayer.Bottom;
                    selectedTileData = bottomTileGridBox.SelectedIndex;
                    bottomTileTab.Enabled = true;
                    topTileTab.Enabled = false;
                    break;
            }
        }

        private void discardRoomChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }

        #region MapInteraction
        private void mapGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            var tsTileWidth = tileMaps[0].Width / 16;

            lastTilePos = mapGridBox.GetIndexPoint(mapGridBox.HoverIndex);

            if (e.Button == MouseButtons.Right)
            {
                selectedTileData = currentRoom.GetTileData(selectedLayer, mapGridBox.HoverIndex * 2);//*2 as each tile is 2 bytes
                mapGridBox.SelectedIndex = mapGridBox.HoverIndex;
                var newX = selectedTileData % tsTileWidth;
                var newY = (selectedTileData - newX) / tsTileWidth;
                // bad practice, entire map selection functions could do with refactor like the tile selection
                if (selectedLayer == 2)
                {
                    bottomTileGridBox.SelectedIndex = selectedTileData;
                }
                else
                {
                    topTileGridBox.SelectedIndex = selectedTileData;
                }

            }
            else if (e.Button == MouseButtons.Left)
            {
                if (selectedTileData == -1) //no selected tile, nothing to paste
                    return;

                WriteTile(mapGridBox.GetIndexPoint(mapGridBox.HoverIndex), mapGridBox.HoverIndex, selectedTileData, selectedLayer);
            }
        }

        private void mapGridBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            var xspot = mapGridBox.HoverIndex % currentRoom.RoomSize.X;
            var yspot = mapGridBox.HoverIndex / currentRoom.RoomSize.X;
            statusXposText.Text = "X:" + xspot.Hex().PadLeft(2, '0');
            statusYposText.Text = "Y:" + yspot.Hex().PadLeft(2, '0');

            if (e.Button == MouseButtons.Left)
            {
                var currentPos = mapGridBox.GetIndexPoint(mapGridBox.HoverIndex);

                if (!lastTilePos.Equals(currentPos))
                {
                    if (selectedTileData == -1) //no selected tile, nothing to paste
                        return;

                    lastTilePos = currentPos;
                    mapGridBox.SelectedIndex = mapGridBox.HoverIndex;

                    WriteTile(mapGridBox.GetIndexPoint(mapGridBox.HoverIndex), mapGridBox.HoverIndex, selectedTileData, selectedLayer);
                }
            }
        }
        #endregion

        #region TilesetInteraction
        private void bottomTileGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            bottomTileGridBox.SelectedIndex = bottomTileGridBox.HoverIndex;
            selectedLayer = 2;
            selectedTileData = bottomTileGridBox.SelectedIndex;
        }

        private void topTileGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            topTileGridBox.SelectedIndex = topTileGridBox.HoverIndex;
            selectedLayer = 1;
            selectedTileData = topTileGridBox.SelectedIndex;
        }
        #endregion

        private void WriteTile(Point p, int pos, int tileData, int layer)
        {
            if (p.X < 0 || p.Y < 0 || p.X > currentRoom.RoomSize.X * 16 || p.Y > currentRoom.RoomSize.Y * 16)
                return;

            if (layer == 1 && currentRoom.Bg1Exists)
            {
                DrawingUtil.DrawTileId(ref mapLayers[0], currentScale, currentRoom.Bg1MetaTiles, currentRoom.MetaData.TileSet, p, currentRoom.MetaData.PaletteSet, tileData, true);
                project_.AddPendingChange(new Core.ChangeTypes.Rework.BgDataChange(currentRoom.Parent.Id, currentRoom.Id, 1));
            }
            else if (layer == 2 && currentRoom.Bg2Exists)
            {
                DrawingUtil.DrawTileId(ref mapLayers[1], currentScale, currentRoom.Bg2MetaTiles, currentRoom.MetaData.TileSet, p, currentRoom.MetaData.PaletteSet, tileData, true);
                project_.AddPendingChange(new Core.ChangeTypes.Rework.BgDataChange(currentRoom.Parent.Id, currentRoom.Id, 2));
            }

            currentRoom.SetTileData(selectedLayer, pos * 2, selectedTileData);

            // TODO switch on layer view
            UpdateViewLayer(viewLayer);
        }

        public static void Notify(string info, string title)
        {
            MessageBox.Show(info, title, MessageBoxButtons.OK);
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = roomTreeView.SelectedNode;
            RoomRenameContext(node);
        }

        private void RoomRenameContext(TreeNode node)
        {
            renameWindow.Show();
            var room = -1;
            var area = -1;
            var areaName = "";
            var roomName = "";
            if (node.Parent != null)
            {
                area = Convert.ToInt32(node.Parent.Name, 16);
                room = Convert.ToInt32(node.Name, 16);
                areaName = node.Parent.Text;
                roomName = node.Text;
            }
            else
            {
                area = Convert.ToInt32(node.Name, 16);
                areaName = node.Text;
            }

            renameWindow.SetTarget(area, room, areaName, roomName);
        }

        public void ChangeRoom(int areaId, int roomId)
        {
            var room = Utilities.Rework.MapManager.Get().GetRoom(areaId, roomId);
            try
            {
                room.LoadRoom();
            }
            catch (RoomException ex)
            {
                Notify(ex.Message, "Room Unloadable");
                return;
            }
            try
            {
                mapLayers = DrawingUtil.DrawRoom(room);
            }
            catch (PaletteException exception)
            {
                Notify(exception.Message, "Invalid Room");
                statusText.Text = "Room load aborted.";
                return;
            }

            statusRoomIdText.Text = "Room Id:" + roomId.Hex().PadLeft(2, '0');
            statusAreaIdText.Text = "Area Id:" + areaId.Hex().PadLeft(2, '0');

            currentRoom = room;

            selectedTileData = -1;
            tileTabControl.SelectedIndex = 1; // Reset to bg2

            //0= bg1 (treetops and such)
            //1= bg2 (flooring)
            mapGridBox.Image = DrawingUtil.OverlayImage(mapLayers[1], mapLayers[0]);
            tileMaps = DrawingUtil.DrawMetatileImages(room, 16);
            bottomTileGridBox.Image = tileMaps[1];
            topTileGridBox.Image = tileMaps[0];

            mapGridBox.Selectable = true;
            mapGridBox.SelectedIndex = -1;
            bottomTileGridBox.Selectable = true;
            topTileGridBox.Selectable = true;

            subWindows.ForEach(w => w.Change());
        }

        private void ResizeRoom()
        {
            if (currentRoom == null)
                return;

            using (var dimPrompt = new ResizeRoomDialog(currentRoom.RoomSize))
            {
                if (dimPrompt.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var dimensions = dimPrompt.GetDims();

                if (dimensions.X == currentRoom.RoomSize.X && dimensions.Y == currentRoom.RoomSize.Y)// no changes
                    return;

                //if above minimum
                if (dimensions.X < 15 || dimensions.Y < 10)
                {
                    Notify("Make sure the room is at least 15 by 10.", "Invalid room size");
                }

                currentRoom.ResizeRoom(dimensions.X, dimensions.Y);
                mapLayers = DrawingUtil.DrawRoom(currentRoom);
                UpdateViewLayer(viewLayer);
            }
        }

        private class SubWindowHolder
        {
            private SubWindow subWindow;
            private ToolStripMenuItem toolStripItem;
            private ToolStripButton toolStripButton;

            public SubWindowHolder(SubWindow window, ToolStripMenuItem toolItem, ToolStripButton toolButton)
            {
                this.subWindow = window;
                this.toolStripItem = toolItem;
                this.toolStripButton = toolButton;
                this.subWindow.FormClosed += new FormClosedEventHandler(Hide);
                this.toolStripItem.Click += new EventHandler(Open);
                this.toolStripButton.Click += new EventHandler(Open);
                window.FormClosing += new FormClosingEventHandler(Closing); //dont dispose
            }

            private void Open(object sender, EventArgs e)
            {
                if (toolStripItem.Checked)
                {
                    subWindow.Focus();
                    return;
                }

                toolStripItem.Checked = true;
                subWindow.Setup();
                subWindow.Show();
            }

            public void Change()
            {
                if(toolStripItem.Checked)
                {
                    subWindow.Setup();
                 }
            }

            public void HideWindow()
            {
                Hide(null, null);
            }

            private void Hide(object sender, FormClosedEventArgs e)
            {
                if (toolStripItem.Checked)
                {
                    subWindow.Hide();
                }
                toolStripItem.Checked = false;
                subWindow.Cleanup();
            }

            private void Closing(object sender, FormClosingEventArgs e)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    Hide(null, null);
                    e.Cancel = true;
                }
            }
        }
    }
}
