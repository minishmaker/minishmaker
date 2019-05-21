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

				int foundIndex = 0;
				int areaIndex = Convert.ToInt32( e.Node.Parent.Text.Split( ' ' )[1],16 );
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
				int roomIndex = Convert.ToInt32( e.Node.Text.Split( ' ' )[1] ,16);
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
				var room = area.Rooms[foundIndex];

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

		private void saveRoomChangesCtrlSToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if(currentRoom==null)
				return;

			currentRoom.SaveRoom();
			File.WriteAllBytes("testfile1.gba",ROM.Instance.romData);
			MessageBox.Show("Room has been saved");
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
				}
				else if(selectedLayer==2)
				{
					currentRoom.DrawTile(ref mapLayers[1], new Point(tileX*16,tileY*16), currentArea, selectedLayer, selectedTileData);
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
