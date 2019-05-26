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
		private MapManager mapManager_;
		private ChestEditorWindow chestEditor = null;

		private Bitmap[] mapLayers;
		private Bitmap[] tileMaps;

		private Bitmap selectorImage = new Bitmap( 16, 16 );
        private Room currentRoom = null;
		private int currentArea = -1;
		private int selectedTileData = -1;
		private int selectedLayer = 2; //start with bg2
		private List<PendingData> unsavedChanges = new List<PendingData>();
		private List<RepointData> dataPositions = new List<RepointData>();
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

		public enum DataType
		{
			bg1Data,
			bg2Data,
			roomMetaData,
			tileSet,
			metaTileSet,
            chestData
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
			dataPositions = new List<RepointData>();

			LoadMaps();


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
	        if (unsavedChanges.Count == 0)
	        {
	            return;
	        }
	        unsavedChanges = unsavedChanges.Distinct().ToList();
	        //foreach(PendingData pendingData in unsavedChanges)
	        while (unsavedChanges.Count > 0)
	        {
	            var pendingData = unsavedChanges.ElementAt(0);
	            var room = FindRoom(pendingData.areaIndex, pendingData.roomIndex);
	            byte[] saveData = null;
                long size = room.GetSaveData(ref saveData, pendingData.dataType);
                long pointerAddress = room.GetPointerLoc(pendingData.dataType, pendingData.areaIndex);
                uint newSource = 0;

                var currentSourceIndex = GetCurrentSource(pendingData.areaIndex, pendingData.roomIndex, pendingData.dataType);
                newSource = FindNewSource((uint)size & 0x7FFFFFFF, currentSourceIndex);

                if (newSource == 0)
                {
                    MessageBox.Show("Unable to allocate enough space for data of type, \"" + pendingData.dataType.ToString() + "\" in area:" + pendingData.areaIndex + " room:" + pendingData.roomIndex + " with size:" + size);
                    continue;
                }

                dataPositions.Add(new RepointData(pendingData.areaIndex, pendingData.roomIndex, pendingData.dataType, (int)newSource, (int)size & 0x7FFFFFFF));
                size = size | 0x80000000; //sets the compression bit

	            SaveToRom(newSource, pointerAddress, saveData, size);

	            unsavedChanges.RemoveAt(0);//saved, remove from pending to avoid re-save
	        }


	        File.WriteAllBytes(ROM.Instance.path, ROM.Instance.romData);
	        //File.WriteAllBytes( "testfile1.gba", ROM.Instance.romData );
	        SaveRepointFile();

	        MessageBox.Show("All changes have been saved");
        }

		private int GetCurrentSource( int area, int room, DataType type )
		{
			//load in all data repoints so far
			if( dataPositions.Count == 0 )
			{   //							shave pre-path		shave data extension
				var name = ROM.Instance.path.Split( '\\' ).Last().Split( '.' )[0];

				try { 
					var rawData = System.IO.File.ReadAllText( name + ".pdat" );
					var rawEntries = rawData.Split( '|' );
					foreach( var entry in rawEntries )
					{
						var attributes = entry.Split( ',' );
						var areaIndex = Convert.ToInt32( attributes[0] );
						var roomIndex = Convert.ToInt32( attributes[1] );
						DataType dataType = (DataType)Enum.Parse(typeof(DataType),attributes[2]);
						var startPoint = Convert.ToInt32( attributes[3] );
						var size = Convert.ToInt32( attributes[4] );

						dataPositions.Add( new RepointData( areaIndex, roomIndex, dataType, startPoint, size ) );
					}
				}
				catch
				{
					Debug.WriteLine(name+".pdat does not yet exist");
					System.IO.File.WriteAllText(name+".pdat","");
				}
			}

			return dataPositions.FindIndex( x => x.areaIndex == area && x.roomIndex == room && x.type == type ); //probably very slow, but works for now
		}

		private uint FindNewSource( uint size, int currentSourceIndex )
		{
			var r = ROM.Instance.reader;
			var emptySpaces = Space.Spaces;

			if( currentSourceIndex != -1 )
			{
				//engage the reshuffling of verious chunks of data
				var src = dataPositions[currentSourceIndex];

				if( size <= src.size ) //its smaller so put it in the same spot but shuffle stuff back
				{
					//everything needs to shift this amount
					var sizeChange = src.size - size;
					r.SetPosition( src.start + src.size );//go to next possible chunk of data

					ReOrderData(r, sizeChange); //reorder chunks behind last bit
					
					dataPositions.RemoveAt(currentSourceIndex); //let other stuff re-add it
					return (uint)src.start;//start is still in the same spot, just less long
				}
				else // its larger, remove from current spot, shift everything in by its size and find a new spot
				{
					r.SetPosition(src.start +src.size);
					ReOrderData(r,src.size);
					dataPositions.RemoveAt(currentSourceIndex); //let other stuff re-add it, we dont have room, area or type
				}
			}


			foreach( var space in emptySpaces )
			{
				if( space.size < size )//wont fit in there no matter what
					continue; 

				r.SetPosition( space.start );
				bool foundSpot = true;
				uint offset = 0;

				while( r.PeekByte() == 0x10 )//magicbyte from compression
				{
					RepointData dat = dataPositions.SingleOrDefault(x=>x.start == r.Position);//never returns null still, but size 0 means nothing is there

					if(dat.size==0)//something here, but not any repointed data, stray byte?
					{
						break;
					}

					RepointData repointData = dat; //some repointed data is here, set next point to after that data
					offset+=(uint)repointData.size;
					
					if(offset+size>space.size) //still enough space to hold this chunk after this data?
					{
						foundSpot = false;//not enough space next space
						break; 
					}

					r.SetPosition(space.start+offset);//set position to check next data chunk
				}

				if( foundSpot )
				{
					return space.start + offset;
				}
			}

			return 0;
		}

		private void ReOrderData(Reader r, long sizeChange)
		{
			while( r.PeekByte() == 0x10 )//is there another compressed chunk here? (dont move position)
			{
				int index = dataPositions.FindIndex(x => x.start == r.Position);
			
				if( index == -1 ) //there is compressed data byte but the space is not used by any repoints
				{
					break;
				}

				var repointData = dataPositions[index];
				var dataCopy = r.ReadBytes( repointData.size );//reads all compressed data AND sets it to supposed next 0x10
				var pointerTableLoc = FindRoom( repointData.areaIndex, repointData.roomIndex ).GetPointerLoc( repointData.type, repointData.areaIndex );
				r.SetPosition(repointData.start+repointData.size);
				var newPos = repointData.start - sizeChange;
				repointData.start = (int)newPos;

				dataPositions[index]= repointData; //modify repoint data for new position
				
				SaveToRom((uint)newPos,pointerTableLoc,dataCopy);
			}
		}

		private void SaveToRom( uint newSource, long pointerAddress, byte[] data, long size = 0 )
		{
			using( MemoryStream m = new MemoryStream( ROM.Instance.romData ) )
			{
				Writer w = new Writer( m );
				w.SetPosition( newSource ); //actually write the data somewhere
				w.WriteBytes( data );

				newSource = (uint)(newSource - ROM.Instance.headers.gfxSourceBase);
				w.SetPosition( pointerAddress );
				w.WriteUInt32( newSource | 0x80000000 ); //byte 1-4 is source, high bit was removed before

				if( size != 0 ) // this is a reshuffle, no need to adjust size
				{
					w.SetPosition( w.Position + 4 );//byte 5-8 is dest, skip
					w.WriteUInt32( (uint)size );//byte 9-12 is size and compressed
				}

			}
		}

		private void SaveRepointFile()
		{
			string s = "";
			foreach(var entry in dataPositions)
			{
				s+=entry.areaIndex+","+entry.roomIndex+","+(int)entry.type+","+entry.start+","+entry.size+"|";
			}
			s = s.Remove(s.Length-1);
			var name = ROM.Instance.path.Split( '\\' ).Last().Split( '.' )[0];

			System.IO.File.WriteAllText(name+".pdat",s);
			//System.IO.File.WriteAllText("testfile1.pdat",s);
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
                unsavedChanges.Add(new PendingData(currentArea, currentRoom.Index, DataType.bg1Data));
            }
            else if (layer == 2)
            {
                currentRoom.DrawTile(ref mapLayers[1], new Point(tileX * 16, tileY * 16), currentArea, selectedLayer, tileData);
                unsavedChanges.Add(new PendingData(currentArea, currentRoom.Index, DataType.bg2Data));
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
