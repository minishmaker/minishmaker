using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GBHL;
using MinishMaker.Core;
using MinishMaker.Utilities;
using System.Drawing;

namespace MinishMaker.UI
{
	public partial class MainWindow : Form
	{
		private ROM ROM_;
        private Project project_;
		private MapManager mapManager_;
		private ChestEditorWindow chestEditor = null;
		private MetaTileEditor metatileEditor = null;

		private Bitmap[] mapLayers;
		private Bitmap[] tileMaps;

        public Room currentRoom = null;
		private int currentArea = -1;
		private int selectedTileData = -1;
		private int selectedLayer = 2; //start with bg2
		private List<PendingData> unsavedChanges = new List<PendingData>();
        private Point lastTilePos;
	    private ViewLayer viewLayer = 0;

        struct RepointData
		{
			public int areaIndex;
			public int roomIndex;
			public DataType type;
			public int start;
			public int size;


			public RepointData( int areaIndex, int roomIndex, DataType type, int start, int size )
			{
				this.areaIndex = areaIndex;
				this.roomIndex = roomIndex;
				this.type = type;
				this.start = start;
				this.size = size;
			}
		}

		struct PendingData
		{
			public int areaIndex;
			public int roomIndex;
			public DataType dataType;

			public PendingData( int areaIndex, int roomIndex, DataType type )
			{
				this.areaIndex = areaIndex;
				this.roomIndex = roomIndex;
				this.dataType = type;
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
		}

        #region MenuBarButtons
        private void OpenButtonClick( object sender, EventArgs e )
		{
			LoadRom();
		}

        private void ExportROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportRom();
        }

        private void saveAllChangesCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
	    {
            SaveAllChanges();
	    }

        private void ExitButtonClick( object sender, EventArgs e )
		{
			Close();
		}

	    private void topLayerToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        UpdateViewLayer(ViewLayer.Top);
	    }

	    private void bottomLayerToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        UpdateViewLayer(ViewLayer.Bottom);
	    }

	    private void bothLayersToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        UpdateViewLayer(ViewLayer.Both);
	    }

	    private void chestEditorStripMenuItem_Click(object sender, EventArgs e)
	    {
	        OpenChestEditor();
	    }

        private void metatileEditorToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        OpenMetatileEditor();
	    }

        private void AboutButtonClick( object sender, EventArgs e )
		{
			Form aboutWindow = new AboutWindow();
			aboutWindow.Show();
		}
        #endregion

        #region ToolStripButtons
        private void openToolStripButton_Click( object sender, EventArgs e )
		{
			LoadRom();
		}

	    private void saveToolStripButton_Click(object sender, EventArgs e)
	    {
            SaveAllChanges();
	    }

	    private void chestToolStripButton_Click(object sender, EventArgs e)
	    {
	        OpenChestEditor();
	    }

	    private void metatileToolStripButton_Click(object sender, EventArgs e)
	    {
	        OpenMetatileEditor();
	    }

        #endregion

        #region OtherInteractions

        // Other interactions
        private void tileTabControl_SelectedIndexChanged(object sender, EventArgs e)
	    {
	        selectedLayer = tileTabControl.SelectedIndex + 1;

	    }

        private void MainWindow_DragDrop( object sender, DragEventArgs e )
		{

		}
	    #endregion

        private void LoadRom()
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				Filter = "GBA ROMs|*.gba|All Files|*.*",
				Title = "Select TMC ROM"
			};

			if( ofd.ShowDialog() != DialogResult.OK )
			{
				return;
			}

			try
			{
				ROM_ = new ROM( ofd.FileName );
			}
			catch( Exception e )
			{
				Console.WriteLine( e );
				throw;
			}

			if( ROM.Instance.version.Equals( RegionVersion.None ) )
			{
				MessageBox.Show( "Invalid TMC ROM. Please Open a valid ROM.", "Incorrect ROM", MessageBoxButtons.OK );
				statusText.Text = "Unable to determine ROM.";
				return;
			}

			mapGridBox.Image = new Bitmap(1,1); //reset some things on loading a rom
			bottomTileGridBox.Image = new Bitmap(1,1);
            topTileGridBox.Image = new Bitmap(1, 1);
			currentRoom = null;
			currentArea = -1;
			selectedTileData = -1;
			selectedLayer = 2; 
			unsavedChanges = new List<PendingData>();
            
            LoadMaps();
            project_ = new Project(ROM.Instance, mapManager_);

            statusText.Text = "Loaded: " + ROM.Instance.path;
		}

		private void LoadMaps()
		{
			mapManager_ = new MapManager();

			roomTreeView.Nodes.Clear();
			// Set up room list
			roomTreeView.BeginUpdate();
			int subsection = 0;

			foreach( MapManager.Area area in mapManager_.MapAreas )
			{
				roomTreeView.Nodes.Add( "Area " + StringUtil.AsStringHex2( area.Index ) );

				foreach( Room room in area.Rooms )
				{
					roomTreeView.Nodes[subsection].Nodes.Add( "Room " + StringUtil.AsStringHex2( room.Index ) );
				}

				subsection++;
			}

			roomTreeView.EndUpdate();
		}

        private void ExportRom()
        {
            if (unsavedChanges.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("You have unsaved changes. Save and export?", "Confirm Save", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaveAllChanges();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            bool success = Project.Instance.ExportToRom();

            if (success)
            {
                MessageBox.Show("Successfully built ROM");
            }
            else
            {
                MessageBox.Show("Error building ROM");
            }
        }

	    private void OpenChestEditor()
	    {
	        if (chestEditorStripMenuItem.Checked)
	            return; // dont open a second one

	        chestEditor = new ChestEditorWindow();

	        if (currentRoom != null)
	        {
	            var chestData = currentRoom.GetChestData();
	            chestEditor.SetData(chestData);
	        }
	        chestEditor.FormClosed += new FormClosedEventHandler(OnChestEditorClose);
	        chestEditorStripMenuItem.Checked = true;
	        chestEditor.Show();
	    }

	    private void OnChestEditorClose(object sender, FormClosedEventArgs e)
	    {
	        chestEditor = null;
	        chestEditorStripMenuItem.Checked = false;
	    }

	    private void OpenMetatileEditor()
	    {
	        if (metatileEditorToolStripMenuItem.Checked)
	            return; // dont open a second one

	        metatileEditor = new MetaTileEditor();

	        if (currentRoom != null)
	        {
	            metatileEditor.RedrawTiles(currentRoom);
	        }

	        metatileEditor.FormClosed += new FormClosedEventHandler(OnMetaTileEditorClose);
	        metatileEditorToolStripMenuItem.Checked = true;
	        metatileEditor.Show();
	    }

	    private void OnMetaTileEditorClose(object sender, FormClosedEventArgs e)
	    {
	        metatileEditor = null;
	        metatileEditorToolStripMenuItem.Checked = false;
	    }

        private void roomTreeView_NodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			if( e.Node.Parent != null )
			{
				Console.WriteLine( e.Node.Parent.Text.Split( ' ' )[1] + " " + e.Node.Text.Split( ' ' )[1] );
				int areaIndex = Convert.ToInt32( e.Node.Parent.Text.Split( ' ' )[1], 16 );
				int roomIndex = Convert.ToInt32( e.Node.Text.Split( ' ' )[1], 16 );
				var room = FindRoom( areaIndex, roomIndex );

				currentRoom = room;

				mapLayers = room.DrawRoom( areaIndex, true, true );

				selectedTileData = -1;
			    tileTabControl.SelectedIndex = 1; // Reset to bg2

				//0= bg1 (treetops and such)
				//1= bg2 (flooring)
				mapGridBox.Image = OverlayImage( mapLayers[1], mapLayers[0] );
				tileMaps = room.DrawTilesetImages( 16, currentArea );
				bottomTileGridBox.Image = tileMaps[1];
                topTileGridBox.Image = tileMaps[0];

                mapGridBox.Selectable = true;
                bottomTileGridBox.Selectable = true;
                topTileGridBox.Selectable = true;

                if (chestEditor != null)
                {
                    var chestData = currentRoom.GetChestData();
                    chestEditor.SetData(chestData);
                }

				if(metatileEditor != null)
				{
					metatileEditor.RedrawTiles(currentRoom);
				}
            }
		}

		public Bitmap OverlayImage( Bitmap baseImage, Bitmap overlay )
		{
			Bitmap finalImage = new Bitmap( baseImage.Width, baseImage.Height );

			using( Graphics g = Graphics.FromImage( finalImage ) )
			{
				//set background color
				g.Clear( Color.Black );

				g.DrawImage( baseImage, new Rectangle( 0, 0, baseImage.Width, baseImage.Height ) );
				g.DrawImage( overlay, new Rectangle( 0, 0, baseImage.Width, baseImage.Height ) );
			}
			//Draw the final image in the gridBox
			return finalImage;
		}

		private Room FindRoom( int areaIndex, int roomIndex )
		{
			int foundIndex = 0;

			currentArea = areaIndex;
			for( int i = 0; i < mapManager_.MapAreas.Count; i++ )
			{
				if( mapManager_.MapAreas[i].Index == areaIndex )
				{
					foundIndex = i;
					break;
				}
				if( i == mapManager_.MapAreas.Count - 1 )
				{
					throw new Exception( "Could not find any area with index: " + areaIndex.Hex() );
				}
			}

			var area = mapManager_.MapAreas[foundIndex];
			for( int j = 0; j < area.Rooms.Count(); j++ )
			{
				if( area.Rooms[j].Index == roomIndex )
				{
					foundIndex = j;
					break;
				}
				if( j == area.Rooms.Count - 1 )
				{
					throw new Exception( "Could not find any room with index: " + roomIndex.Hex() + " in area: " + areaIndex.Hex() );
				}
			}

			return area.Rooms[foundIndex];
		}

        private void SaveAllChanges()
        {
            unsavedChanges = unsavedChanges.Distinct().ToList();
            while (unsavedChanges.Count > 0)
            {
                PendingData data = unsavedChanges.ElementAt(0);
                Project.Instance.AddChange(data.areaIndex, data.roomIndex, data.dataType);
                unsavedChanges.RemoveAt(0);
            }

            Project.Instance.SaveProject();

            MessageBox.Show("Project Saved");
        }

		public void AddPendingChange(DataType type)
		{
			unsavedChanges.Add(new PendingData(currentArea,currentRoom.Index,type));
		}

	    private void UpdateViewLayer(ViewLayer layer)
	    {
	        if (currentRoom == null)
	            return;

	        switch (layer)
	        {
	            case ViewLayer.Both:
	                mapGridBox.Image = OverlayImage(mapLayers[1], mapLayers[0]);
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

        private void discardRoomChangesToolStripMenuItem_Click( object sender, EventArgs e )
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
            if(currentRoom == null)
                return;


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

        private void WriteTile (Point p, int pos, int tileData, int layer)
        {
            if (layer == 1 && currentRoom.Bg1Exists)
            {
                currentRoom.DrawTile(ref mapLayers[0], p, currentArea, selectedLayer, tileData);
                AddPendingChange(DataType.bg1Data);
            }
            else if (layer == 2 && currentRoom.Bg2Exists)
            {
                currentRoom.DrawTile(ref mapLayers[1], p, currentArea, selectedLayer, tileData);
                AddPendingChange(DataType.bg2Data);
            }

            currentRoom.SetTileData(selectedLayer, pos * 2, selectedTileData);

            // TODO switch on layer view
            UpdateViewLayer(viewLayer);
        }

		public static void Notify(string info, string title)
		{
			MessageBox.Show( info, title, MessageBoxButtons.OK );
		}
    }
}
