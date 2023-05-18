using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using MinishMaker.Core;
using MinishMaker.Utilities;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Properties;
using PaletteException = MinishMaker.Core.PaletteException;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace MinishMaker.UI
{
    public partial class MainWindow : Form
    {
        public static MainWindow instance;
        private Project project_;

        private UndoRedoManager uRManager_;
        private List<SubWindowHolder> subWindows = new List<SubWindowHolder>();
        private NewProjectWindow newProjectWindow = null;
        private RenameDialog renameWindow = new RenameDialog();

        private Bitmap[] mapLayers;
        private Bitmap scaledMap;

        private Bitmap[] tileMaps;

        public Area currentArea = null;
        public Room currentRoom = null;
        private int selectedLayer = 2; //start with bg2
        private ViewLayer viewLayer = ViewLayer.Both;

        private int currentScale = 1;
        private Point lastTilePos;
        private Point boxSize;
        private int[] selectedTileData;

        private bool highlightSame = false;
        private int markerId = 1;

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
            uRManager_ = UndoRedoManager.Get();
            if (fileName != null){
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
            viewLayer = ViewLayer.Top;
            UpdateViewLayer();
        }

        private void BottomLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewLayer = ViewLayer.Bottom;
            UpdateViewLayer();
        }

        private void BothLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewLayer = ViewLayer.Both;
            UpdateViewLayer();
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
            subWindows.Add(new SubWindowHolder(new MetaTileEditorWindow(), metatileEditorToolStripMenuItem, metatileToolStripButton));//metatile
            subWindows.Add(new SubWindowHolder(new AreaEditorWindow(), areaEditorToolStripMenuItem, areaToolStripButton));//area
            subWindows.Add(new SubWindowHolder(new ObjectPlacementEditorWindow(), objectPlacementEditorToolStripMenuItem, objectPlacementToolStripButton)); //object
            subWindows.Add(new SubWindowHolder(new WarpEditorWindow(), warpEditorToolStripMenuItem, warpToolStripButton)); //warp
            subWindows.Add(new SubWindowHolder(new TextEditorWindow(), textEditorToolStripMenuItem, textToolStripButton)); //text
            subWindows.Add(new SubWindowHolder(new PaletteEditorWindow(), paletteEditorToolStripMenuItem, paletteEditorToolStripButton)); //palette
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
                    renameWindow.Hide();

                project_ = newProjectWindow.project;
                LoadProjectData();
                EnableEditor(true);
                SetStatusBar("Created new project: " + project_.ProjectPath + "/" + project_.ProjectName + ".mmproj");

            }
            else
                SetStatusBar("Project creation aborted.");
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
            project_ = new Project(fileName);

            LoadProjectData();
            EnableEditor(true);
            SetStatusBar("Loaded: " + fileName);
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
            
            metatileEditorToolStripMenuItem.Enabled = enabled;
            areaEditorToolStripMenuItem.Enabled = enabled;
            objectPlacementEditorToolStripMenuItem.Enabled = enabled;
            warpEditorToolStripMenuItem.Enabled = enabled;
            textEditorToolStripMenuItem.Enabled = enabled;

            saveToolStripButton.Enabled = enabled;
            buildToolStripButton.Enabled = enabled;

            metatileToolStripButton.Enabled = enabled;
            areaToolStripButton.Enabled = enabled;
            objectPlacementToolStripButton.Enabled = enabled;
            warpToolStripButton.Enabled = enabled;
            textToolStripButton.Enabled = enabled;
            paletteEditorToolStripButton.Enabled = enabled;
        }

        public void BuildProject()
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
                SetStatusBar("Build Completed. Output file: " + project_.ProjectPath + "\\" + project_.ProjectName + ".gba");
            }
            else
            {
                MessageBox.Show("There was a problem building the project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatusBar("Failed to build project.");
            }
            // TODO check for build completing correctly, probably needs deeper integration with ColorzCore

        }

        #endregion

        private void LoadMaps()
        {
            roomTreeView.Nodes.Clear();
            // Set up room list
            roomTreeView.BeginUpdate();

            foreach (Area area in MapManager.Instance.GetAllAreas())
            {
                var areaNode = roomTreeView.Nodes.Add($"0x{area.Id.Hex().PadLeft(2, '0')} - {area.NiceName}");
                areaNode.Name = StringUtil.AsStringHex(area.Id, 1);

                foreach (Room room in area.GetAllRooms())
                {
                    var roomNode = areaNode.Nodes.Add($"0x{room.Id.Hex().PadLeft(2, '0')} - {room.NiceName}");
                    roomNode.Name = StringUtil.AsStringHex(room.Id, 1);
                }
            }

            roomTreeView.EndUpdate();
        }

        private void roomTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                //Console.WriteLine(e.Node.Parent.Name + " " + e.Node.Name);
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
                    node.Text = $"0x{areaNodeName.PadLeft(2, '0')} - {newAreaName}";
                    break;
                }
            }

            if (newRoomName == "")
                return;

            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Name == roomNodeName)
                {
                    node.Text = $"0x{roomNodeName.PadLeft(2, '0')} - {newRoomName}";
                    break;
                }
            }
        }

        public void SaveAllChanges()
        {
            if (project_ == null)
                return;

            project_.Save();

            project_.CreateProjectFile();

            MessageBox.Show("Project Saved");
        }
        public void AddMarker(int id, Object data, Func<Object, Tuple<Point[], Brush>> pixelFunc)
        {
            mapGridBox.AddMarker(id, data, pixelFunc);
        }

        public void RemoveMarker(int id)
        {
            mapGridBox.RemoveMarker(id);
        }

        private void UpdateViewLayer()
        {
            if (currentRoom == null)
                return;
            Bitmap overlayedImage;
            switch (viewLayer)
            {
                case ViewLayer.Both:
                    overlayedImage = DrawingUtil.OverlayImage(mapLayers[1], mapLayers[0]);
                    topTileTab.Enabled = true;
                    bottomTileTab.Enabled = true;
                    break;
                case ViewLayer.Top:
                    overlayedImage = mapLayers[0];
                    tileTabControl.SelectedIndex = 0;
                    //selectedTileData = topTileGridBox.SelectedIndex;
                    topTileTab.Enabled = true;
                    bottomTileTab.Enabled = false;
                    break;
                case ViewLayer.Bottom:
                    overlayedImage = mapLayers[1];
                    tileTabControl.SelectedIndex = 1;
                    //selectedTileData = bottomTileGridBox.SelectedIndex;
                    bottomTileTab.Enabled = true;
                    topTileTab.Enabled = false;
                    break;
                default:
                    throw new ArgumentException("Use a correct ViewLayer");
            }

            scaledMap = DrawingUtil.ResizeBitmap(overlayedImage, currentScale);
            mapGridBox.Image = scaledMap;
        }

        private void discardRoomChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private UndoRedoEntry generateWriteMultiEntry() {
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
            topleft.Y = (mapGridBox.HoverIndex - topleft.X) / currentRoom.RoomSize.X;
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
                //mapGridBox.SelectedIndex = mapGridBox.HoverIndex;
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

                SetBottomValues("", endTilePos.X, endTilePos.Y);

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
            XPosLabel.Text = "X: 0x" + xspot.Hex(2);
            YPosLabel.Text = "Y: 0x" + yspot.Hex(2);

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
            {
                return;
            }
            var index = bottomTileGridBox.HoverIndex;
            bottomTileGridBox.SelectedIndex = index;
            selectedLayer = 2;
            selectedTileData = new int[1];
            boxSize.X = 1;
            boxSize.Y = 1;
            selectedTileData[0] = index;
            SetBottomValues("B ", index%16, index/16);
        }

        private void topTileGridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentRoom == null)
            {
                return;
            }

            var index = topTileGridBox.HoverIndex;
            topTileGridBox.SelectedIndex = index;
            selectedLayer = 1;
            selectedTileData = new int[1];
            boxSize.X = 1;
            boxSize.Y = 1;
            selectedTileData[0] = index;
            SetBottomValues("T ", index%16, index/16);
        }

        private void SetBottomValues(string prefix, int x = -1, int y = -1)
        {
            RoomIdLabel.Text = "Room Id: 0x" + currentRoom.Id.Hex(2);
            AreaIdLabel.Text = "Area Id: 0x" + currentRoom.Parent.Id.Hex(2);
            SelectedXPosLabel.Text = $"Selected X: 0x--";
            SelectedYPosLabel.Text = $"Selected Y: 0x--";
            SelectedSizeLabel.Text = "Selected Size: --- x ---";
            SelectedBehaviourLabel.Text = $"Selected Behaviour: 0x--";
            if (x == -1 || y == -1)
            {
                return;
            }

            if (boxSize.X == 1 && boxSize.Y == 1)
            {
                SelectedXPosLabel.Text = $"Selected X: {prefix}0x{x.Hex(2)}";
                SelectedYPosLabel.Text = $"Selected Y: {prefix}0x{y.Hex(2)}";
                var currentTileType = new byte[2];
                currentArea.GetMetaTileData(ref currentTileType, selectedTileData[0], selectedLayer);
                SelectedBehaviourLabel.Text = $"Selected Behaviour: {prefix}0x{(currentTileType[0] + (currentTileType[1] << 8) & 0x3ff).Hex(3)}";
            }
            else
            {
                SelectedSizeLabel.Text = $"Selected Size: {boxSize.X} x {boxSize.Y}";
            }
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
            var tileset = currentArea.Tilesets[currentRoom.MetaData.tilesetId];
            var colors = PaletteSetManager.Get().GetSet(tileset.paletteSetId).Colors;
            //var colors = currentRoom.MetaData.PaletteSet.Colors;
            var maplayer = mapLayers[layer - 1];
            //var newPos = (hoverIndex + x) + (y * currentRoom.RoomSize.X);

            var topLeftTile = new Point();
            topLeftTile.X = (hoverIndex % currentRoom.RoomSize.X);
            topLeftTile.Y = ((hoverIndex - topLeftTile.X) / currentRoom.RoomSize.X);
            Rectangle r = new Rectangle(topLeftTile.X * 16, topLeftTile.Y * 16, boxSize.X * 16, boxSize.Y * 16);
            Rectangle sr = new Rectangle(topLeftTile.X * 16 * currentScale, topLeftTile.Y * 16 * currentScale, boxSize.X * 16 * currentScale, boxSize.Y * 16 * currentScale);
            var bdScaled = scaledMap.LockBits(sr, ImageLockMode.WriteOnly, scaledMap.PixelFormat);
            var bdBase = maplayer.LockBits(r, ImageLockMode.WriteOnly, maplayer.PixelFormat);
            for (var y = 0; y < boxSize.Y; y++)
            {
                for (var x = 0; x < boxSize.X; x++)
                {
                    Point tilePos = new Point(x, y);
                    if (tilePos.X < 0 || tilePos.Y < 0 || tilePos.X > currentRoom.RoomSize.X|| tilePos.Y > currentRoom.RoomSize.Y)
                        continue;

                    var tileId = tileData[x + (y * selectedAreaSize.X)];
                    WriteTile(topLeftTile, tilePos, tileId, tileset, colors, ref bdScaled, ref bdBase, layer);
                }
            }
            scaledMap.UnlockBits(bdScaled);
            maplayer.UnlockBits(bdBase);
            mapGridBox.Image = scaledMap;
            // TODO switch on layer view
            //UpdateViewLayer(viewLayer);
        }

        private void WriteTile(Point topLeftTile, Point tilePos, int tileId, Tileset tileset, Color[] colors, ref BitmapData bdScaled, ref BitmapData bdBase, int layer)
        {
            Point pixelPos = new Point(tilePos.X * 16, tilePos.Y * 16);
            var pos = topLeftTile.X + tilePos.X + ((topLeftTile.Y + tilePos.Y) * currentRoom.RoomSize.X);
            currentRoom.SetTileData(layer, pos, tileId);
            if (currentRoom.Bg2Exists && (viewLayer == ViewLayer.Both || viewLayer == ViewLayer.Bottom))
            {
                var bg2TileId = currentRoom.GetTileData(2, pos);
                var tileData = currentArea.Bg2MetaTileset.GetMetaTileData(bg2TileId);
                bdScaled = DrawingUtil.DrawMetaTileData(bdScaled, tileData, tileset, pixelPos, colors, false, true, currentScale);
                if(layer == 2)
                {
                    bdBase = DrawingUtil.DrawMetaTileData(bdBase, tileData, tileset, pixelPos, colors, false, true);
                }
            }
            if (currentRoom.Bg1Exists && (viewLayer == ViewLayer.Both || viewLayer == ViewLayer.Top))
            {
                var bg1TileId = currentRoom.GetTileData(1, pos);
                var tileData = currentArea.Bg1MetaTileset.GetMetaTileData(bg1TileId);
                bdScaled = DrawingUtil.DrawMetaTileData(bdScaled, tileData, tileset, pixelPos, colors, true, viewLayer == ViewLayer.Top || !currentRoom.Bg1Exists, currentScale);
                if (layer == 1)
                {
                    bdBase = DrawingUtil.DrawMetaTileData(bdBase, tileData, tileset, pixelPos, colors, true, true);
                }
            }
            project_.AddPendingChange(new BgDataChange(currentRoom.Parent.Id, currentRoom.Id, layer));
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
            var roomId = -1;
            var areaId = -1;
            var areaName = "";
            var roomName = "";
            if (node.Parent != null)
            {
                areaId = Convert.ToInt32(node.Parent.Name, 16);
                roomId = Convert.ToInt32(node.Name, 16);
                roomName = node.Text.Substring(7);
                areaName = node.Parent.Text.Substring(7);
            }
            else
            {
                areaId = Convert.ToInt32(node.Name, 16);
                areaName = node.Text.Substring(7);
            }

            renameWindow.SetTarget(areaId, roomId, areaName, roomName);
        }

        public void ChangeRoom(int areaId, int roomId)
        {
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
            currentArea = MapManager.Instance.GetArea(areaId);
            var room = currentArea.GetRoom(roomId);

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
                currentRoom = room;
                UpdateViewLayer();
                SetBottomValues("", -1, -1);
            }
            catch (PaletteException exception)
            {
                Notify(exception.Message, "Invalid Room");
                SetStatusBar("Room load aborted.");
                return;
            }

            selectedTileData = new int[0];
            tileTabControl.SelectedIndex = 1; // Reset to bg2

            //0= bg1 (treetops and such)
            //1= bg2 (flooring)
            tileMaps = DrawingUtil.DrawMetatileImages(room, 16);
            bottomTileGridBox.Image = tileMaps[1];
            topTileGridBox.Image = tileMaps[0];

            mapGridBox.Selectable = true;
            mapGridBox.SelectedIndex = -1;
            bottomTileGridBox.Selectable = true;
            topTileGridBox.Selectable = true;

            subWindows.ForEach(w => w.Change());
            //RunTests();
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
                UpdateViewLayer();
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
            
            mapGridBox.Invalidate(getMapViewport());
        }

        public Rectangle getMapViewport()
        {
            return new Rectangle(mapPanel.AutoScrollOffset.X * -1, mapPanel.AutoScrollOffset.Y * -1, mapPanel.Width, mapPanel.Height);
        }

        private Tuple<Point[], Brush> CreateSameMarker(Object obj)
        {
            //marker unused here, but can be used to hold all data
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

            return new Tuple<Point[],Brush>(pixels, Brushes.Red);
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

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentScale += 1;
            mapGridBox.SetScale(currentScale);
            if (currentRoom == null)
            {
                return;
            }
            UpdateViewLayer();
            //mapPanel.HorizontalScroll.Value = mapPanel.HorizontalScroll.Value / (currentScale - 1) * currentScale;
            //mapPanel.VerticalScroll.Value = mapPanel.VerticalScroll.Value / (currentScale - 1) * currentScale;
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScale == 1)
            {
                return;
            }
            currentScale -= 1;
            mapGridBox.SetScale(currentScale);
            if (currentRoom == null)
            {
                return;
            }
            UpdateViewLayer();
            //mapPanel.HorizontalScroll.Value = mapPanel.HorizontalScroll.Value / (currentScale + 1) * currentScale;
            //mapPanel.VerticalScroll.Value = mapPanel.VerticalScroll.Value / (currentScale + 1) * currentScale;
        }



        public void RedrawRoom()
        {
            tileMaps = DrawingUtil.DrawMetatileImages(currentRoom, 16);
            bottomTileGridBox.Image = tileMaps[1];
            topTileGridBox.Image = tileMaps[0];
            mapLayers = DrawingUtil.DrawRoom(currentRoom);
            UpdateViewLayer();
        }

        private void SetStatusBar(string value)
        {
            statusText.Text = value;
            Task.Factory.StartNew(() => 
                {
                    Thread.Sleep(5 * 1000);
                    statusText.Text = "";
                }
            );
        }

        public void RunTests()
        {
            return;
            var stringArr = new string[5];
            stringArr[0] = "a";
            stringArr[1] = "b";
            stringArr[2] = "c";
            stringArr[3] = "d";
            stringArr[4] = "e";
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(stringArr);
            Debug.WriteLine(json);
            var r = ROM.Instance.reader;

            var header = ROM.Instance.headers;
            int paletteSetTableLoc = header.paletteSetTableLoc;
            var dict = new SortedDictionary<ushort, Dictionary<Tuple<byte, byte>,int>>();
            for (int b = 0; b <= 0xff; b++)
            {
                int addr = r.ReadAddr(paletteSetTableLoc + (b * 4)); //4 byte entries
                ushort palId = r.ReadUInt16(addr);
                byte start = r.ReadByte();
                byte amount = r.ReadByte();

                if(!dict.ContainsKey(palId))
                {
                    dict.Add(palId, new Dictionary<Tuple<byte, byte>, int>());
                }
                var tuple = new Tuple<byte, byte>(start, amount);
                if (!dict[palId].ContainsKey(tuple))
                {
                    dict[palId].Add(tuple, 0);
                }
                dict[palId][tuple]+=1;
            }

            foreach(var dictEntry in dict)
            {
                Debug.WriteLine("");
                Debug.WriteLine($"----{dictEntry.Key}----");
                foreach(var tupleEntry in dictEntry.Value)
                {
                    Debug.WriteLine($"start:{tupleEntry.Key.Item1} | amount:{tupleEntry.Key.Item2} | repeats:{tupleEntry.Value}");
                }
            }
            var a = 0;

        }
    }
}
