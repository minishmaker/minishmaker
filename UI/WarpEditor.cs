using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MinishMaker.Core.RoomMetaData;

namespace MinishMaker.UI
{
	public partial class WarpEditor : Form
	{
		int warpIndex = -1;
		private List<WarpData> warpDataList;
		private bool shouldTrigger = false;

		public WarpEditor()
		{
			InitializeComponent();
			warpTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
			warpTypeBox.DataSource = Enum.GetValues( typeof( WarpType ) );
			transitionTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
			transitionTypeBox.DataSource = Enum.GetValues( typeof( TransitionType ) );
			facingBox.DropDownStyle = ComboBoxStyle.DropDownList;
			facingBox.DataSource = Enum.GetValues( typeof( Facing ) );

			warpIndex = -1;

			warpTypeBox.Enabled = false;
			transitionTypeBox.Enabled = false;
			facingBox.Enabled = false;

			destArea.Enabled = false;
			destRoom.Enabled = false;
			exitHeight.Enabled = false;

			destX.Enabled = false;
			destY.Enabled = false;

			soundId.Enabled = false;

			prevButton.Enabled = false;
			nextButton.Enabled = false;

			newButton.Enabled = false;
			removeButton.Enabled = false;

			ControlBorderPanel(false);
			ControlAreaPanel(false);

			LoadData();
		}

		public enum WarpType
		{
			Border = 0x00,
			Area = 0x01
		}

		public enum TransitionType
		{
			Normal = 0x00,
			MinishInstant = 0x01,
			DropIn = 0x02,
			Instant = 0x03,
			StepForward = 0x04,
			//Crash = 0x05,
			MinishDropIn = 0x06,
			StairExit = 0x07,
			//StairExit2 = 0x08,
			//Crash2 = 0x09,
			HopInForward = 0x0A,
			HopIn = 0x0B,
			FlyIn = 0x0C
		}

		public enum Facing
		{
			Keep = 0x00,
			Up = 0x01,
			Left = 0x02,
			Down = 0x03,
			Right = 0x04,
		}

		public void LoadData()
		{
			if( MainWindow.currentRoom != null )
			{
				warpIndex = 0;
				warpDataList = MainWindow.currentRoom.GetWarpData();
				SetData();
			}
		}

		public void SetData()
		{
			shouldTrigger = false;
			newButton.Enabled = true;

			if( warpDataList.Count == 0 )
			{
				indexLabel.Text = "0";
				warpTypeBox.Enabled = false;
				transitionTypeBox.Enabled = false;
				facingBox.Enabled = false;

				destArea.Enabled = false;
				destRoom.Enabled = false;
				exitHeight.Enabled = false;

				destX.Enabled = false;
				destY.Enabled = false;

				soundId.Enabled = false;

				prevButton.Enabled = false;
				nextButton.Enabled = false;

				newButton.Enabled = false;
				removeButton.Enabled = false;

				ControlBorderPanel(false);
				ControlAreaPanel(false);

				warpTypeBox.SelectedItem = WarpType.Border;
				transitionTypeBox.SelectedItem = TransitionType.Normal;
				facingBox.SelectedItem = Facing.Keep;

				destX.Text = "0";
				destY.Text = "0";
				destArea.Text = "0";
				destRoom.Text = "0";
				exitHeight.Text = "0";
				soundId.Text = "0";

				((MainWindow)Application.OpenForms[0]).HighlightWarp( -1, -1 );
				return;
			}

			indexLabel.Text = warpIndex + "";
			var warp = warpDataList[warpIndex];

			if( warp.warpType != 0 && warp.warpType != 1 )
			{

				warpTypeBox.Enabled = false;
				transitionTypeBox.Enabled = false;
				facingBox.Enabled = false;

				destArea.Enabled = false;
				destRoom.Enabled = false;
				exitHeight.Enabled = false;

				destX.Enabled = false;
				destY.Enabled = false;

				soundId.Enabled = false;

				prevButton.Enabled = false;
				nextButton.Enabled = false;

				newButton.Enabled = false;
				removeButton.Enabled = false;

				ControlBorderPanel(false);
				ControlAreaPanel(false);

				((MainWindow)Application.OpenForms[0]).HighlightWarp( -1, -1 );
			}
			else
			{
				warpTypeBox.Enabled = true;
				transitionTypeBox.Enabled = true;
				facingBox.Enabled = true;

				destArea.Enabled = true;
				destRoom.Enabled = true;
				exitHeight.Enabled = true;

				destX.Enabled = true;
				destY.Enabled = true;

				soundId.Enabled = true;

				prevButton.Enabled = true;
				nextButton.Enabled = true;

				newButton.Enabled = true;
				removeButton.Enabled = true;

				if( warp.warpType != 1 )
				{
					ControlAreaPanel(false);
					ControlBorderPanel(true);
					topLeftCheck.Checked =		((warp.warpVar & 0x1) == 0x1);
					topRightCheck.Checked =		((warp.warpVar & 0x2) == 0x2);
					rightTopCheck.Checked =		((warp.warpVar & 0x4) == 0x4);
					rightBottomCheck.Checked =	((warp.warpVar & 0x8) == 0x8);
					bottomLeftCheck.Checked =	((warp.warpVar & 0x10) == 0x10);
					bottomRightCheck.Checked =	((warp.warpVar & 0x20) == 0x20);
					leftTopCheck.Checked =		((warp.warpVar & 0x40) == 0x40);
					leftBottomCheck.Checked =	((warp.warpVar & 0x80) == 0x80);
					warpX.Text = "0";
					warpY.Text = "0";
					RedrawBlock();
					((MainWindow)Application.OpenForms[0]).HighlightWarp( -1, -1 );
				}
				else
				{
					ControlBorderPanel(false);
					ControlAreaPanel(true);
					warpX.Text = warp.warpXPixel.Hex();
					warpY.Text = warp.warpYPixel.Hex();
					warpShape.Text = warp.warpVar.Hex();
					((MainWindow)Application.OpenForms[0]).HighlightWarp( warp.warpXPixel, warp.warpYPixel );
				}
			}

			warpTypeBox.SelectedItem = (WarpType)warp.warpType;
			transitionTypeBox.SelectedItem = (TransitionType)warp.transitionType;
			facingBox.SelectedItem = (Facing)warp.facing;

			destX.Text = warp.destXPixel.Hex();
			destY.Text = warp.destYPixel.Hex();
			destArea.Text = warp.destArea.Hex();
			destRoom.Text = warp.destRoom.Hex();
			exitHeight.Text = warp.exitHeight.Hex();
			soundId.Text = warp.soundId.Hex();


			if( warpIndex >= warpDataList.Count - 1 )
			{
				nextButton.Enabled = false;
			}
			else
			{
				nextButton.Enabled = true;
			}

			if( warpIndex <= 0 )
			{
				prevButton.Enabled = false;
			}
			else
			{
				prevButton.Enabled = true;
			}

			shouldTrigger = true;
		}

		public void ControlAreaPanel(bool visible)
		{
			areaPanel.Visible = visible;
			warpX.Visible = visible;
			warpY.Visible = visible;
			warpShape.Visible = visible;
			label2.Visible = visible;
			label3.Visible = visible;
			label7.Visible = visible;
		}

		public void ControlBorderPanel(bool visible)
		{
			borderPanel.Visible = visible;
			topLeftCheck.Visible = visible;
			topRightCheck.Visible = visible;
			leftTopCheck.Visible = visible;
			leftBottomCheck.Visible = visible;
			rightTopCheck.Visible = visible;
			rightBottomCheck.Visible = visible;
			bottomLeftCheck.Visible = visible;
			bottomRightCheck.Visible = visible;
			label13.Visible = visible;
		}

		public void RedrawBlock()
		{
			var graphic = new Bitmap(32,32);
			using( Graphics g = Graphics.FromImage( graphic ) )
			{
				var warp = warpDataList[warpIndex];
				var redpen = new Pen(Color.Red,2);
				var blackPen = new Pen(Color.Black,2);

				var pen1 = ((warp.warpVar & 0x1) == 0x1)? redpen:blackPen;
				var pen2 = ((warp.warpVar & 0x2) == 0x2)? redpen:blackPen;
				var pen3 = ((warp.warpVar & 0x4) == 0x4)? redpen:blackPen;
				var pen4 = ((warp.warpVar & 0x8) == 0x8)? redpen:blackPen;
				var pen5 = ((warp.warpVar & 0x10) == 0x10)? redpen:blackPen;
				var pen6 = ((warp.warpVar & 0x20) == 0x20)? redpen:blackPen;
				var pen7 = ((warp.warpVar & 0x40) == 0x40)? redpen:blackPen;
				var pen8 = ((warp.warpVar & 0x80) == 0x80)? redpen:blackPen;
				g.DrawLine(pen1,1, 1, 16,1);//TL
				g.DrawLine(pen2,16,1, 31,1);//TR
				g.DrawLine(pen3,31,1, 31,16);//RT
				g.DrawLine(pen4,31,16,31,31);//RB
				g.DrawLine(pen5,1, 31,16,31);//BL
				g.DrawLine(pen6,16,31,31,31);//BR
				g.DrawLine(pen7,1, 1, 1, 16);//LT
				g.DrawLine(pen8,1, 16,1, 31);//LB
			}
			blockImage.Image = graphic;
		}

		public void CheckboxChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			var data =
				(topLeftCheck.Checked ? 0x1 : 0) +
				(topRightCheck.Checked ? 0x2 : 0) +
				(rightTopCheck.Checked ? 0x4 : 0) +
				(rightBottomCheck.Checked ? 0x8 : 0) +
				(bottomLeftCheck.Checked ? 0x10 : 0) +
				(bottomRightCheck.Checked ? 0x20 : 0) +
				(leftTopCheck.Checked ? 0x40 : 0) +
				(leftBottomCheck.Checked ? 0x80 : 0);

			

			var warp = warpDataList[warpIndex];
			warp.warpVar = (byte)data;
			warpDataList[warpIndex] = warp;
			SetData();

			MainWindow.AddPendingChange( new WarpDataChange( MainWindow.currentArea, MainWindow.currentRoom.Index ) );
		}

		private void warpTypeBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = (byte)(int)warpTypeBox.SelectedItem;
				var warp = warpDataList[warpIndex];
				warp.warpType = newVal;
				warpDataList[warpIndex] = warp;
				SetData();
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void transitionTypeBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = (byte)(int)transitionTypeBox.SelectedItem;
				var warp = warpDataList[warpIndex];
				warp.transitionType = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void facingBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = (byte)(int)facingBox.SelectedItem;
				var warp = warpDataList[warpIndex];
				warp.facing = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void destX_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = Convert.ToUInt16(destX.Text,16);
				var warp = warpDataList[warpIndex];
				warp.destXPixel = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void destY_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = Convert.ToUInt16(destY.Text,16);
				var warp = warpDataList[warpIndex];
				warp.destYPixel = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void destArea_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = Convert.ToByte(destArea.Text,16);;
				var warp = warpDataList[warpIndex];
				warp.destArea = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void destRoom_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = Convert.ToByte(destRoom.Text,16);
				var warp = warpDataList[warpIndex];
				warp.destRoom = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void exitHeight_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = Convert.ToByte(exitHeight.Text,16);
				var warp = warpDataList[warpIndex];
				warp.exitHeight = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void soundId_TextChanged( object sender, EventArgs e )
		{
			if( !shouldTrigger )
				return;

			try
			{
				var newVal = (byte)(int)warpTypeBox.SelectedItem;
				var warp = warpDataList[warpIndex];
				warp.warpType = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void warpShape_TextChanged( object sender, EventArgs e )
		{
			try
			{
				var newVal = Convert.ToByte(warpShape.Text,16);
				var warp = warpDataList[warpIndex];
				warp.warpVar = newVal;
				warpDataList[warpIndex] = warp;
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void warpY_TextChanged( object sender, EventArgs e )
		{
			try
			{
				var newVal = Convert.ToUInt16(warpY.Text,16);
				var warp = warpDataList[warpIndex];
				warp.warpYPixel = newVal;
				warpDataList[warpIndex] = warp;
				SetData();
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void warpX_TextChanged( object sender, EventArgs e )
		{
			try
			{
				var newVal = Convert.ToUInt16(warpX.Text,16);
				var warp = warpDataList[warpIndex];
				warp.warpXPixel = newVal;
				warpDataList[warpIndex] = warp;
				SetData();
				MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void newButton_Click( object sender, EventArgs e )
		{
			if(warpDataList==null)
			{
				return;
			}

			MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			var data = new byte[20] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			warpDataList.Insert(warpIndex+1,new WarpData(data,0)); //add an empty warp
			warpIndex +=1;
			SetData();
		}

		private void removeButton_Click( object sender, EventArgs e )
		{
			if(warpDataList==null)
			{
				return;
			}

			MainWindow.AddPendingChange(new WarpDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			var warp = warpDataList[warpIndex];
			warpDataList.Remove(warp);

			if(warpIndex!=0)
				warpIndex-=1;

			SetData();
		}

		private void nextButton_Click( object sender, EventArgs e )
		{
			warpIndex+=1;
			SetData();
		}

		private void prevButton_Click( object sender, EventArgs e )
		{
			warpIndex -=1;
			SetData();
		}
	}
}
