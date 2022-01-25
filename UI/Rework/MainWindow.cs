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
using MinishMaker.Utilities.Rework;

namespace MinishMaker.UI.Rework
{
    public partial class MainWindow : Form
    {
        public static MainWindow instance;
        private Core.Rework.Project project_;

        private Utilities.Rework.MapManager mapManager_;
        private Utilities.Rework.UndoRedoManager uRManager_;
        private List<SubWindowHolder> subWindows = new List<SubWindowHolder>();
        private NewProjectWindow newProjectWindow = null;
        private RenameDialog renameWindow = null;

        private Bitmap[] mapLayers;
        private Bitmap[] tileMaps;

        public Core.Rework.Room currentRoom = null;
        //private int selectedTileData = -1;
        private int selectedLayer = 2; //start with bg2
        private ViewLayer viewLayer = ViewLayer.Both;

        private int currentScale = 1;
        private Point lastTilePos;
        private Point boxSize;
        private int[] selectedTileData;

        private bool highlightSame = false;
        private int markerId = 1;
        private int[] sameTileList;
        public enum ViewLayer
        {
            Both,
            Top,
            Bottom
        }

        public MainWindow(string fileName)
        {
            InitializeComponent();
            EnableEditor(false);
            UpdateWindowTitle();
            SetupSubWindows();
            AddMarker(markerId, new Point(0, 0), CreateSameMarker);
            this.KeyDown += MainWindow_OnKeyDown;
            this.KeyPreview = true;
            mapPanel.Width = this.Width - tileTabControl.Right - roomTreeView.Right - 20;
            instance = this;
            if(fileName != null){
                OpenProject(fileName);
            }
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
            subWindows.Add(new SubWindowHolder(new TextEditorWindow(), textEditorToolStripMenuItem, textToolStripButton)); //text
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

            OpenProject(ofd.FileName);
        }

        private void OpenProject(string fileName)
        { 
            if (renameWindow != null)
                renameWindow.Hide();

            subWindows.ForEach(sw => sw.HideWindow());

            if (project_ == null)
            {
                project_ = new Core.Rework.Project(fileName);
            }
            uRManager_ = UndoRedoManager.Get();
            //uRManager_.Clear();
            if (project_.Loaded)
            {
                LoadProjectData();
                EnableEditor(true);
                statusText.Text = "Loaded: " + fileName;
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
            selectedTileData = new int[0];
            lastTilePos.X = -1;
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
            textEditorToolStripMenuItem.Enabled = enabled;

            saveToolStripButton.Enabled = enabled;
            buildToolStripButton.Enabled = enabled;

            chestToolStripButton.Enabled = enabled;
            metatileToolStripButton.Enabled = enabled;
            areaToolStripButton.Enabled = enabled;
            objectPlacementToolStripButton.Enabled = enabled;
            warpToolStripButton.Enabled = enabled;
            textToolStripButton.Enabled = enabled;
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
        public void AddMarker(int id, Point position, Func<Tuple<Point[], Brush>> pixelFunc)
        {
            mapGridBox.AddMarker(id, position, pixelFunc);
        }

        public void RemoveMarker(int id)
        {
            mapGridBox.RemoveMarker(id);
        }

        /*public void HighlightChest(int tileX, int tileY)
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
        }*/

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
                    //selectedTileData = topTileGridBox.SelectedIndex;
                    topTileTab.Enabled = true;
                    bottomTileTab.Enabled = false;
                    break;
                case ViewLayer.Bottom:
                    mapGridBox.Image = mapLayers[1];
                    tileTabControl.SelectedIndex = 1;
                    viewLayer = ViewLayer.Bottom;
                    //selectedTileData = bottomTileGridBox.SelectedIndex;
                    bottomTileTab.Enabled = true;
                    topTileTab.Enabled = false;
                    break;
            }
        }

        private void discardRoomChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private UndoRedoEntry generateWriteMultiEntry() {
            var HI = mapGridBox.HoverIndex;
            object[] redo = new object[4];
            object[] undo = new object[4];
            var topleft = new Point();

            redo[0] = mapGridBox.HoverIndex;
            redo[1] = boxSize;
            redo[2] = selectedLayer;
            redo[3] = selectedTileData;
            undo[0] = mapGridBox.HoverIndex;
            undo[1] = boxSize;
            undo[2] = selectedLayer;

            topleft.X = mapGridBox.HoverIndex % currentRoom.RoomSize.X;
            topleft.Y = (mapGridBox.HoverIndex - lastTilePos.X) / currentRoom.RoomSize.X;
            var undoTileData = new int[boxSize.X *boxSize.Y]; 
            for (var y = 0; y < boxSize.Y; y++)
            {
                for (var x = 0; x < boxSize.X; x++)
                {
                    var pos = topleft.X + x + (topleft.Y + y) * currentRoom.RoomSize.X;
                    undoTileData[x + (boxSize.X * y)] = currentRoom.GetTileData(selectedLayer, pos);
                }
            }

            undo[3] = undoTileData;
            return new UndoRedoEntry(undo, DoWriteMultiTile, redo, DoWriteMultiTile, UndoRedoEntry.ActionEnum.EDIT_BG);
        }

        #region MapInteraction
        private void mapGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            lastTilePos.X = mapGridBox.HoverIndex % currentRoom.RoomSize.X;
            lastTilePos.Y = (mapGridBox.HoverIndex - lastTilePos.X) / currentRoom.RoomSize.X;

            if (e.Button == MouseButtons.Left)
            {
                if (selectedTileData.Length == 0) //no selected tile, nothing to paste
                    return;

                mapGridBox.SelectedIndex = mapGridBox.HoverIndex;
                WriteMultiTile();
            }
        }

        private void mapGridBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (lastTilePos.X == -1)
                return;

            var startTilePos = new Point(lastTilePos.X, lastTilePos.Y);

            lastTilePos.X = -1;

            if (e.Button == MouseButtons.Right)
            {
                mapGridBox.SelectedIndex = mapGridBox.HoverIndex;
                // bad practice, entire map selection functions could do with refactor like the tile selection

                var endTilePos = new Point();
                endTilePos.X = mapGridBox.HoverIndex % currentRoom.RoomSize.X;
                endTilePos.Y = (mapGridBox.HoverIndex - endTilePos.X) / currentRoom.RoomSize.X;
                boxSize.X = endTilePos.X - startTilePos.X;
                var left = startTilePos.X;
                
                if (boxSize.X < 0) 
                {
                    boxSize.X *= -1;
                    left = endTilePos.X;
                }

                boxSize.X++;

                boxSize.Y = endTilePos.Y - startTilePos.Y;
                var top = startTilePos.Y;

                if (boxSize.Y < 0)
                {
                    boxSize.Y *= -1;
                    top = endTilePos.Y;
                }

                boxSize.Y++;
                selectedTileData = new int[boxSize.X * boxSize.Y];
                for (var y = 0; y < boxSize.Y; y++)
                {
                    for (var x = 0; x < boxSize.X; x++)
                    {
                        var pos = (left+x) + ((top+y) * currentRoom.RoomSize.X);
                        selectedTileData[x + (boxSize.X * y)] = currentRoom.GetTileData(selectedLayer, pos);
                    }
                }

                if (boxSize.X == 1 && boxSize.Y == 1)
                {
                    if (selectedLayer == 2)
                    {
                        bottomTileGridBox.SelectedIndex = selectedTileData[0];
                    }
                    else
                    {
                        topTileGridBox.SelectedIndex = selectedTileData[0];
                    }
                }
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

                if (lastTilePos.Equals(currentPos))
                    return;
                    
                if (selectedTileData.Length == 0) //no selected tile, nothing to paste
                    return;

                lastTilePos = currentPos;
                
                mapGridBox.SelectedIndex = mapGridBox.HoverIndex;
                WriteMultiTile();
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
            selectedTileData = new int[1];
            boxSize.X = 1;
            boxSize.Y = 1;
            selectedTileData[0] = bottomTileGridBox.SelectedIndex;
        }

        private void topTileGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
                return;

            topTileGridBox.SelectedIndex = topTileGridBox.HoverIndex;
            selectedLayer = 1;
            selectedTileData = new int[1];
            boxSize.X = 1;
            boxSize.Y = 1;
            selectedTileData[0] = topTileGridBox.SelectedIndex;
        }
        #endregion

        public void WriteMultiTile()
        {
            var entry = generateWriteMultiEntry();
            entry.Redo();
            uRManager_.AddEntry(entry);
        }

        private void DoWriteMultiTile(object[] data)
        {
            int hoverIndex = (int)data[0];
            Point selectedAreaSize = (Point)data[1];
            int layer = (int)data[2];
            int[] tileData = (int[])data[3];

            for (var y = 0; y < boxSize.Y; y++)
            {
                for (var x = 0; x < boxSize.X; x++)
                {
                    var newPos = (hoverIndex + x) + (y * currentRoom.RoomSize.X);
                    var tile = tileData[x + (y * selectedAreaSize.X)];
                    WriteTile(newPos, tile, layer, selectedAreaSize);
                }
            }
        }

        private void WriteTile(int pos, int tileData, int layer, Point selectedAreaSize)
        {
            var tilePos = new Point();
            tilePos.X = pos % currentRoom.RoomSize.X;
            tilePos.Y = (pos - tilePos.X) / currentRoom.RoomSize.X;
            if (tilePos.X < 0 || tilePos.Y < 0 || tilePos.X > currentRoom.RoomSize.X || tilePos.Y > currentRoom.RoomSize.Y)
                return;

            var pixelPos = new Point();
            pixelPos.X = tilePos.X * 16 * currentScale;
            pixelPos.Y = tilePos.Y * 16 * currentScale;
            if (layer == 1 && currentRoom.Bg1Exists)
            {
                DrawingUtil.DrawTileId(ref mapLayers[0], currentScale, currentRoom.Bg1MetaTiles, currentRoom.MetaData.TileSet, pixelPos, currentRoom.MetaData.PaletteSet, tileData, true);
                project_.AddPendingChange(new Core.ChangeTypes.Rework.BgDataChange(currentRoom.Parent.Id, currentRoom.Id, 1));
            }
            else if (layer == 2 && currentRoom.Bg2Exists)
            {
                DrawingUtil.DrawTileId(ref mapLayers[1], currentScale, currentRoom.Bg2MetaTiles, currentRoom.MetaData.TileSet, pixelPos, currentRoom.MetaData.PaletteSet, tileData, true);
                project_.AddPendingChange(new Core.ChangeTypes.Rework.BgDataChange(currentRoom.Parent.Id, currentRoom.Id, 2));
            }

            currentRoom.SetTileData(layer, pos, tileData);

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
            statusRoomIdText.Text = "Room Id:" + roomId.Hex().PadLeft(2, '0');
            statusAreaIdText.Text = "Area Id:" + areaId.Hex().PadLeft(2, '0');
            var redoData = new object[] { areaId, roomId };
            if (currentRoom != null)
            {
                var undoData = new object[] { currentRoom.Parent.Id, currentRoom.Id };
                var entry = new UndoRedoEntry(undoData, DoChangeRoom, redoData, DoChangeRoom, UndoRedoEntry.ActionEnum.CHANGE_ROOM);
                entry.Redo(); //do entry first, will error if bad
                uRManager_.AddEntry(entry); //add if entry worked
            } else {
                DoChangeRoom(redoData);
            }
        }

        private void DoChangeRoom(object[] data)
        {
            var areaId = (int)data[0];
            var roomId = (int)data[1];
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
            selectedTileData = new int[0];
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

        private void highlightSameToolStripButton_Click(object sender, EventArgs e)
        {
            highlightSame = !highlightSame;
            highlightSameToolStripButton.Checked = highlightSame;
            mapGridBox.Invalidate();
        }

        private Tuple<Point[], Brush> CreateSameMarker()
        {
            if (!highlightSame || selectedTileData.Length == 0 || boxSize.X != 1 || boxSize.Y != 1 ) //if its not on, no tiles are selected or multiple tiles are selected, dont show the same
            {
                return null;
            }

            var sameTiles = currentRoom.GetSameTiles(selectedLayer, selectedTileData[0]);
            Point[] pixels = new Point[30 * sameTiles.Length];
            for(int i = 0; i < sameTiles.Length; i++) 
            {
                int tileX = (sameTiles[i] % currentRoom.RoomSize.X) * 16;
                int tileY = (sameTiles[i] / currentRoom.RoomSize.X) * 16;
                var start = i * 30;
                for(int j = 0; j < 30; j++)
                {
                    if(j < 8)
                    {
                        pixels[start + j] = new Point(tileX + (j * 2), tileY + 0);
                    }
                    else if(j < 22)
                    {
                        var x = j % 2 == 0 ? 15 : 0;
                        pixels[start + j] = new Point(tileX + x, j - 7 + tileY);
                    }
                    else
                    {
                        pixels[start + j] = new Point(tileX + 1 + ((j - 22) * 2), tileY + 15);
                    }
                }
            }

            return new Tuple<Point[],Brush>(pixels,Brushes.Red);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uRManager_.Redo();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uRManager_.Undo();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Modifiers == (Keys.Control | Keys.Shift))
            {
                if(e.KeyCode == Keys.Z) {
                    uRManager_.Redo();
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
