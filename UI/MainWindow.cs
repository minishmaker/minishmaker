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

		private Bitmap[] mapLayers;
		private Bitmap[] tileMaps;

		private Bitmap selectorImage = new Bitmap( 16, 16 );
        public Room currentRoom = null;
		private int currentArea = -1;
		private int selectedTileData = -1;
		private int selectedLayer = 2; //start with bg2
		private List<PendingData> unsavedChanges = new List<PendingData>();
        private Point lastTilePos;

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
        #endregion

        // Other interactions
        private void MainWindow_DragDrop( object sender, DragEventArgs e )
		{

		}

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

			mapView.Image = new Bitmap(1,1); //reset some things on loading a rom
			tileView.Image = new Bitmap(1,1);
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

				tileSelectionBox.Visible = false;
				mapSelectionBox.Visible = false;
				selectedTileData = -1;

				//0= bg1 (treetops and such)
				//1= bg2 (flooring)
				mapView.Image = OverlayImage( mapLayers[1], mapLayers[0] );
				tileMaps = room.DrawTilesetImages( 11, currentArea );
				tileView.Image = tileMaps[1];

                if (chestEditor != null)
                {
                    var chestData = currentRoom.GetChestData();
                    chestEditor.SetData(chestData);
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
			//Draw the final image in the pictureBox
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

		private void discardRoomChangesToolStripMenuItem_Click( object sender, EventArgs e )
		{
			//TODO
		}

		private void mapView_MouseDown( object sender, MouseEventArgs me )
		{
			if( currentRoom == null )
				return;

			if( mapSelectionBox.Image == null )
			{
				GenerateSelectorImage();
				tileSelectionBox.Image = selectorImage;
				mapSelectionBox.Image = selectorImage;
			}

			mapSelectionBox.Visible = true;

			var mTileWidth = mapLayers[0].Width / 16;
			var tsTileWidth = tileMaps[0].Width / 16;

			var partialX = me.X % 16;
			var partialY = me.Y % 16;

			int tileX = (me.X - partialX) / 16;
			int tileY = (me.Y - partialY) / 16;

            lastTilePos = new Point(tileX, tileY);

            mapSelectionBox.Location = new Point( me.X - partialX, me.Y - partialY );
			var pos = tileY * mTileWidth + tileX; //tilenumber if they were all in a line

			if( me.Button == MouseButtons.Right )
			{
				selectedTileData = currentRoom.GetTileData( selectedLayer, pos * 2 );//*2 as each tile is 2 bytes
				var newX = selectedTileData % tsTileWidth;
				var newY = (selectedTileData - newX) / tsTileWidth;

				tileSelectionBox.Location = new Point( newX * 16, newY * 16 );
				tileSelectionBox.Visible = true;
			}
			else if( me.Button == MouseButtons.Left )
			{
				if( selectedTileData == -1 ) //no selected tile, nothing to paste
					return;

                WriteTile(tileX, tileY, pos, selectedTileData, selectedLayer);
            }
		}

        private void mapView_MouseMove( object sender, MouseEventArgs me )
        {
            if (me.Button != MouseButtons.None)
            {
                if (currentRoom == null)
                    return;

                var mTileWidth = mapLayers[0].Width / 16;
                var tsTileWidth = tileMaps[0].Width / 16;

                var partialX = me.X % 16;
                var partialY = me.Y % 16;

                int tileX = (me.X - partialX) / 16;
                int tileY = (me.Y - partialY) / 16;

                Point tilePos = new Point(tileX, tileY);

                if (!lastTilePos.Equals(tilePos))
                {

                    if (mapSelectionBox.Image == null)
                    {
                        GenerateSelectorImage();
                        tileSelectionBox.Image = selectorImage;
                        mapSelectionBox.Image = selectorImage;
                    }

                    mapSelectionBox.Visible = true;

                    mapSelectionBox.Location = new Point(me.X - partialX, me.Y - partialY);
                    var pos = tileY * mTileWidth + tileX; //tilenumber if they were all in a line

                    if (me.Button == MouseButtons.Right)
                    {
                        // TODO: Select box
                    }
                    else if (me.Button == MouseButtons.Left)
                    {
                        lastTilePos = tilePos;
                        if (selectedTileData == -1) //no selected tile, nothing to paste
                            return;

                        WriteTile(tileX, tileY, pos, selectedTileData, selectedLayer);
                    }
                }
            }
        }

		private void tileView_Click( object sender, EventArgs e )
		{
			if( currentRoom == null )
				return;

			if( tileSelectionBox.Image == null )
			{
                GenerateSelectorImage();
			    tileSelectionBox.BackColor = Color.Transparent;
			    mapSelectionBox.BackColor = Color.Transparent;
                tileSelectionBox.Image = selectorImage;
				mapSelectionBox.Image = selectorImage;
			}
			tileSelectionBox.Visible = true;

			var mTileWidth = mapLayers[0].Width / 16;
			var tsTileWidth = tileMaps[0].Width / 16;

			var me = (MouseEventArgs)e;

			var partialX = me.X % 16;
			var partialY = me.Y % 16;

			int tileX = (me.X - partialX) / 16;
			int tileY = (me.Y - partialY) / 16;

			tileSelectionBox.Location = new Point( me.X - partialX, me.Y - partialY );

			selectedTileData = tileX + tileY * tsTileWidth;
		}

		private void GenerateSelectorImage()
		{
			using( Graphics g = Graphics.FromImage( selectorImage ) )
			{
				selectorImage.MakeTransparent();
				g.DrawRectangle( new Pen( Color.Red, 4 ), 0, 0, 16, 16 );
			}
		}

        private void WriteTile (int tileX, int tileY, int pos, int tileData, int layer)
        {
            if (layer == 1)
            {
                currentRoom.DrawTile(ref mapLayers[0], new Point(tileX * 16, tileY * 16), currentArea, selectedLayer, tileData);
                AddPendingChange(DataType.bg1Data);
            }
            else if (layer == 2)
            {
                currentRoom.DrawTile(ref mapLayers[1], new Point(tileX * 16, tileY * 16), currentArea, selectedLayer, tileData);
                AddPendingChange(DataType.bg2Data);
            }

            currentRoom.SetTileData(selectedLayer, pos * 2, selectedTileData);
            mapView.Image = OverlayImage(mapLayers[1], mapLayers[0]);
        }

		private void chestEditorStripMenuItem_Click( object sender, EventArgs e )
		{
			if(chestEditorStripMenuItem.Checked)
				return; // dont open a second one

			chestEditor = new ChestEditorWindow();

			if(currentRoom != null) {
				var chestData = currentRoom.GetChestData();
				chestEditor.SetData(chestData);
			}
			chestEditor.FormClosed +=new FormClosedEventHandler(onChestEditorClose);
			chestEditorStripMenuItem.Checked = true;
			chestEditor.Show();
		}

		void onChestEditorClose(object sender, FormClosedEventArgs e)
		{
			chestEditor = null;
			chestEditorStripMenuItem.Checked = false;
		}
    }
}
