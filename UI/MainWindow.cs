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

		private Bitmap[] mapLayers;
		private Bitmap[] tileMaps;

		private Bitmap selectorImage = new Bitmap(16,16);
		private Room currentRoom=null;
		private int currentArea = -1;
		private int selectedTileData=-1;
		private int selectedLayer = 2; //start with bg2
		private List<PendingData> unsavedChanges = new List<PendingData>();

		struct PendingData
		{
			public int areaIndex;
			public int roomIndex;
			public DataType dataType;

			public PendingData(int areaIndex, int roomIndex, DataType type)
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

		}

		public MainWindow()
		{
			InitializeComponent();
		}

		// MenuBar Buttons
		private void OpenButtonClick( object sender, EventArgs e )
		{
			LoadRom();
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

		// ToolStrip Buttons
		private void openToolStripButton_Click( object sender, EventArgs e )
		{
			LoadRom();
		}

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
				int areaIndex = Convert.ToInt32( e.Node.Parent.Text.Split( ' ' )[1],16 );
				int roomIndex = Convert.ToInt32( e.Node.Text.Split( ' ' )[1] ,16);
				var room = FindRoom(areaIndex, roomIndex);

				currentRoom = room;

				mapLayers = room.DrawRoom( areaIndex, true, true );
				
				tileSelectionBox.Visible=false;
				mapSelectionBox.Visible=false;
				selectedTileData=-1;

				//0= bg1 (treetops and such)
				//1= bg2 (flooring)
				mapView.Image = OverlayImage(mapLayers[1],mapLayers[0]);
				tileMaps = room.DrawTilesetImages(11,currentArea);
				tileView.Image = tileMaps[1];
			}
		}

		public Bitmap OverlayImage(Bitmap baseImage, Bitmap overlay)
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

		private Room FindRoom(int areaIndex, int roomIndex)
		{
			int foundIndex = 0;
				
				currentArea= areaIndex;
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

		private void saveAllChangesCtrlSToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if(unsavedChanges.Count==0)
			{
				return;
			}
			unsavedChanges = unsavedChanges.Distinct().ToList();
			//foreach(PendingData pendingData in unsavedChanges)
			while(unsavedChanges.Count>0)
			{
				var pendingData =unsavedChanges.ElementAt(0);
				var room = FindRoom(pendingData.areaIndex,pendingData.roomIndex);
				byte[] compressedData = null;
				long size = room.CompressRoomData(ref compressedData, pendingData.dataType);
				long pointerAddress = room.GetPointerLoc(pendingData.dataType ,pendingData.areaIndex);

				//TODO: write data to open area and repoint
				uint newSource = FindNewSource((uint)size);

				if(newSource==0)
				{
					MessageBox.Show("Unable to allocate enough space for data in area:"+pendingData.areaIndex+" room:"+pendingData.roomIndex+" with size:"+size);
					continue;
				}

				

				size = size | 0x80000000; //sets the compression bit
				using(MemoryStream m = new MemoryStream( ROM.Instance.romData )) {
					Writer w = new Writer(m);
					w.SetPosition(newSource);//actually write the compressed data somewhere
					w.WriteBytes(compressedData);

					newSource = (uint)(newSource - ROM.Instance.headers.gfxSourceBase);
					w.SetPosition(pointerAddress);
					w.WriteUInt32(newSource | 0x80000000);//byte 1-4 is source, high bit was removed before

					w.SetPosition(w.Position+4);//byte 5-8 is dest, skip
					w.WriteUInt32((uint)size);//byte 9-12 is size and compressed

					
				}

				unsavedChanges.RemoveAt(0);//saved, remove from pending to avoid re-save
			}
			

			File.WriteAllBytes("testfile1.gba",ROM.Instance.romData);
			MessageBox.Show("All changes have been saved");
		}

		private uint FindNewSource(uint size)
		{
			var r = ROM.Instance.reader;
			var emptySpaces = Space.Spaces;

			foreach(var space in emptySpaces)
			{
				if(space.size<size)
					continue;

				r.SetPosition(space.start);
				bool foundSpot = true;
				uint offset=0;

				while(r.ReadByte()==0x10)//magicbyte from compression
				{
					//TODO:check the compressed data size, requires all addrdata to be loaded to check through to find this location
					//compressed data itself only holds the uncompressed size

					//compressedSize = data.size
					//offset+=compressedSize;
					//if(offset+size>space.size)
					//{
					//	foundSpot = false;
					//	break; 
					//}
					//r.SetPosition(space.start+offset)
					//
					foundSpot = false;
					break;
				}

				if(foundSpot)
				{
					return space.start+offset;
				}

			}

			return 0;
		}

		private void discardRoomChangesToolStripMenuItem_Click( object sender, EventArgs e )
		{
			//TODO
		}

		private void mapView_Click( object sender, EventArgs e )
		{
			if(currentRoom == null)
				return;

			if(mapSelectionBox.Image==null)
			{
				GenerateSelectorImage();
				tileSelectionBox.BackColor=Color.Transparent;
				mapSelectionBox.BackColor=Color.Transparent;
				tileSelectionBox.Image = selectorImage;
				mapSelectionBox.Image = selectorImage;
			}
			mapSelectionBox.Visible=true;
			
			var mTileWidth = mapLayers[0].Width/16;
			var tsTileWidth = tileMaps[0].Width/16;

			var me = (MouseEventArgs)e;

			var partialX = me.X%16;
			var partialY = me.Y%16;

			int tileX = (me.X-partialX) /16;
			int tileY = (me.Y-partialY) /16;

			mapSelectionBox.Location= new Point(me.X-partialX, me.Y-partialY);
			var pos = tileY *mTileWidth +tileX; //tilenumber if they were all in a line

			if( me.Button==MouseButtons.Right)
			{
				selectedTileData = currentRoom.GetTileData(selectedLayer, pos*2);//*2 as each tile is 2 bytes
				var newX = selectedTileData%tsTileWidth;
				var newY = (selectedTileData-newX)/tsTileWidth;

				tileSelectionBox.Location= new Point(newX*16, newY*16);
				tileSelectionBox.Visible=true;
			}
			else if (me.Button == MouseButtons.Left)
			{
				if(selectedTileData ==-1) //no selected tile, nothing to paste
					return;

				if(selectedLayer==1)
				{
					currentRoom.DrawTile(ref mapLayers[0], new Point(tileX*16,tileY*16), currentArea, selectedLayer, selectedTileData);
					unsavedChanges.Add(new PendingData(currentArea,currentRoom.Index,DataType.bg1Data));
				}
				else if(selectedLayer==2)
				{
					currentRoom.DrawTile(ref mapLayers[1], new Point(tileX*16,tileY*16), currentArea, selectedLayer, selectedTileData);
					unsavedChanges.Add(new PendingData(currentArea,currentRoom.Index,DataType.bg2Data));
				}
				
				currentRoom.SetTileData(selectedLayer, pos*2, selectedTileData);
				mapView.Image=OverlayImage(mapLayers[1],mapLayers[0]);
			}
		}

		private void tileView_Click( object sender, EventArgs e )
		{
			if(currentRoom == null)
				return;

			if(tileSelectionBox.Image==null)
			{
				GenerateSelectorImage();
				tileSelectionBox.Image = selectorImage;
				mapSelectionBox.Image = selectorImage;
			}
			tileSelectionBox.Visible = true;

			var mTileWidth = mapLayers[0].Width/16;
			var tsTileWidth = tileMaps[0].Width/16;

			var me = (MouseEventArgs)e;

			var partialX = me.X%16;
			var partialY = me.Y%16;

			int tileX = (me.X-partialX) /16;
			int tileY = (me.Y-partialY) /16;

			tileSelectionBox.Location= new Point(me.X-partialX, me.Y-partialY);

			selectedTileData = tileX + tileY*tsTileWidth;
		}

		private void GenerateSelectorImage()
		{
			using (Graphics g = Graphics.FromImage(selectorImage))
			{
				selectorImage.MakeTransparent();
				g.DrawRectangle(new Pen(Color.Red,4),0,0,16,16);
			}
		}
	}
}
