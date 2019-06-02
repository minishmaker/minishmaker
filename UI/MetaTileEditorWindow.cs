using MinishMaker.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinishMaker.UI
{
	public partial class MetaTileEditor : Form
	{
		Bitmap[] tileset = new Bitmap[2];
		Bitmap[] metaTiles = new Bitmap[2];
		int pnum = 0;
		int currentLayer = 0;
		int selectedTileNum = -1;
		int selectedMetaTileNum = -1;

		public MetaTileEditor()
		{
			InitializeComponent();
		}

		public void RedrawTiles(Room room)
		{
			metaTiles = room.DrawTilesetImages(16, 0); //areaindex currently unused because what even is swaptiles
			DrawTileset(room.tileSet, room.palettes);
			metaTileSetBox.Image = metaTiles[currentLayer];
			tileSetBox.Image = tileset[currentLayer];
		}

		public void DrawTileset( TileSet tset, Color[][] palettes)
		{
			tileset[0] = new Bitmap(128, 0x100); //0x200 * 8 /16 = 0x200 * 0.5
			tileset[1] = new Bitmap(128, 0x100);

			for(int tnum = 0; tnum<0x200; tnum++)
			{
				var xpos = tnum % 0x10; //tiles
				var ypos = (tnum-xpos) / 0x10; //tiles
				ypos *= 8; //pixels
				xpos *= 8; //pixels
				tset.DrawQuarterTile(ref tileset[0], new Point(xpos, ypos ),tnum, palettes[pnum],false,false,false);
				tset.DrawQuarterTile(ref tileset[1], new Point(xpos, ypos ),tnum+0x200, palettes[pnum],false,false,false);
			}
		}

		
		private void tileSetBox_Click( object sender, EventArgs e )
		{
			if (tileSetBox.Image==null)
				return;

			MouseEventArgs me = (MouseEventArgs)e;

			int xpos = me.X / 0x8; 
			int ypos = me.Y / 0x8;
			
			var enlarged = Magnify(tileset[currentLayer], new Rectangle(xpos*8,ypos*8,8,8), 4);

			selectedTileNum = xpos + (ypos * 0x10);

			selectedTileBox.Image = enlarged;
		}


		private void metaTileSetBox_Click( object sender, EventArgs e )
		{
			if (metaTileSetBox.Image==null)
				return;

			MouseEventArgs me = (MouseEventArgs)e;

			int xpos = me.X / 0x10; 
			int ypos = me.Y / 0x10;

			var enlarged = Magnify(metaTiles[currentLayer], new Rectangle(xpos*16,ypos*16,16,16),4);

			selectedMetaTileNum = xpos + (ypos * 0x10);

			selectedMetaTileBox.Image = enlarged;
		}

		private Bitmap Magnify(Bitmap b, Rectangle rect, int scaleMod)
		{
			Bitmap ret = new Bitmap(rect.Width*scaleMod, rect.Height*scaleMod);
			for(int i = 0; i<rect.Width; i++)
			{
				for (int j = 0; j<rect.Width; j++)
				{
					var xpos = scaleMod*i;
					var ypos = scaleMod*j;

					var color = b.GetPixel(rect.X+i,rect.Y+j);
					for(int k=0; k< scaleMod; k++)
					{
						for(int l = 0; l< scaleMod; l++)
						{
							ret.SetPixel(xpos+k,ypos+l,color);
						}
					}
				}
			}
			return ret;
		}
	}
}
