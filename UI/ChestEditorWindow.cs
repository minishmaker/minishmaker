using MinishMaker.Core;
using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MinishMaker.Core.RoomMetaData;
using static MinishMaker.Core.Project;

namespace MinishMaker.UI
{
	public partial class ChestEditorWindow : Form
	{
		private int chestIndex = -1;
		private List<ChestData> chestDataList;
		
		
		public ChestEditorWindow()
		{
			InitializeComponent();
			itemName.DropDownStyle = ComboBoxStyle.DropDownList;
			kinstoneType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.itemName.DataSource = Enum.GetValues(typeof(ItemType));
			this.kinstoneType.DataSource = Enum.GetValues(typeof(KinstoneType));
			chestIndex = -1;
			
			if(chestDataList==null)
			{
				//lock all stuff
				entityType.Enabled = false;
				entityId.Enabled = false;
				itemName.Enabled = false;
				kinstoneType.Enabled =false;
				itemAmount.Enabled = false;
				xPosition.Enabled = false;
				yPosition.Enabled = false;
				nextButton.Enabled = false;
				prevButton.Enabled = false;
			}
		}

		public void SetData(List<ChestData> data)
		{
			this.chestDataList = data;
											 
			prevButton.Enabled = false;
			nextButton.Enabled = false;

			if(data.Count == 0)
			{
				chestIndex = -1;
				entityType.Enabled = false;
				entityId.Enabled = false;
				itemName.Enabled = false;
				kinstoneType.Enabled =false;
				itemAmount.Enabled = false;
				xPosition.Enabled = false;
				yPosition.Enabled = false;
				nextButton.Enabled = false;
				newButton.Visible = true;
			}
            else
            {
                entityType.Enabled = true;
                entityId.Enabled = true;
                itemName.Enabled = true;
                kinstoneType.Enabled = true;
                itemAmount.Enabled = true;
                xPosition.Enabled = true;
                yPosition.Enabled = true;
				
                chestIndex = 0;

                LoadChestData(0);

				if(data.Count>1)
				{
					nextButton.Enabled = true;
				}
            }

			indexLabel.Text = chestIndex.ToString();
		}

  
		private void nextButton_Click( object sender, EventArgs e )
		{

            chestIndex++;
			prevButton.Enabled=true;
			if(chestIndex == chestDataList.Count-1)
			{
				nextButton.Enabled = false;
			}

            indexLabel.Text = chestIndex.ToString();

            LoadChestData(chestIndex);
        }

		private void prevButton_Click( object sender, EventArgs e )
		{
			chestIndex--;
			nextButton.Enabled = true;			 
			if(chestIndex == 0)
			{
				prevButton.Enabled = false;
			}

            indexLabel.Text = chestIndex.ToString();

            LoadChestData(chestIndex);
		}

		private void newButton_Click( object sender, EventArgs e )
		{
			MainWindow main = (MainWindow)Application.OpenForms[0];//if the main closes everything closes
            chestIndex = chestDataList.Count;
            indexLabel.Text = chestIndex.ToString();
			main.AddPendingChange(DataType.chestData);
			main.currentRoom.AddChestData(new ChestData(0x02,0,0,0,0,0));
			LoadChestData(chestIndex);

            prevButton.Enabled = true;
            nextButton.Enabled = false;
        }

        private void LoadChestData(int chest)
        {
            ChestData chestData = chestDataList[chest];
            entityType.Text = StringUtil.AsStringHex2(chestData.type);

            if ((TileEntityType)chestData.type == TileEntityType.Chest || (TileEntityType)chestData.type == TileEntityType.BigChest)
            {
                entityId.Text = StringUtil.AsStringHex2(chestData.chestId);
                itemName.SelectedItem = (ItemType)chestData.itemId;
                kinstoneType.SelectedItem = (KinstoneType)chestData.itemSubNumber;
                itemAmount.Text = chestData.itemSubNumber.ToString();

                ushort chestPos = chestData.chestLocation;
				int yPos = chestPos>>6;
				int xPos = chestPos - (yPos<<6);
                xPosition.Text = xPos.ToString();
                yPosition.Text = yPos.ToString();

                itemName.Enabled = true;
                kinstoneType.Enabled = true;
                itemAmount.Enabled = true;
                xPosition.Enabled = true;
                yPosition.Enabled = true;
                entityId.Enabled = true;
            }
			else
			{
                entityId.Text = "00";
                itemName.SelectedItem = ItemType.Untyped;
                kinstoneType.SelectedItem = KinstoneType.UnTyped;
                itemAmount.Text = "0";
                xPosition.Text = "0";
                yPosition.Text = "0";

                itemName.Enabled = false;
				kinstoneType.Enabled = false;
				itemAmount.Enabled = false;
				xPosition.Enabled = false;
				yPosition.Enabled = false;
				entityId.Enabled = false;
            }
        }

		private void entityType_LostFocus( object sender, EventArgs e )												
		{
			if(!entityType.IsHandleCreated)
				return;		 

			var chest = chestDataList[chestIndex];
			try
			{
				var type = Convert.ToByte(entityType.Text,16);

				if(type == chest.type)
					return;
	
				var main = (MainWindow)(Application.OpenForms[0]);
				main.AddPendingChange(DataType.chestData);

				chest.type = type;
			}
			catch
			{
				entityType.Text = StringUtil.AsStringHex2(chest.type);
			}

			chestDataList[chestIndex] = chest;
		}

		private void entityId_LostFocus( object sender, EventArgs e )
		{
			if(!entityId.IsHandleCreated)
				return;

			var chest = chestDataList[chestIndex];
			try
			{
				var chestId = Convert.ToByte(entityId.Text,16);

				if(chestId == chest.chestId)
					return;
	
				var main = (MainWindow)(Application.OpenForms[0]);
				main.AddPendingChange(DataType.chestData);

				chest.chestId = chestId;
			}
			catch
			{
				entityId.Text = StringUtil.AsStringHex2(chest.chestId);		  
			}
			chestDataList[chestIndex] = chest;
		}

		private void xPosition_LostFocus( object sender, EventArgs e )
		{
			if(!xPosition.IsHandleCreated)
				return;

			var chest = chestDataList[chestIndex];
			try
			{
				ushort location = (ushort)(Convert.ToByte(xPosition.Text) + (Convert.ToByte(yPosition.Text)<<6));

				if(location == chest.chestLocation)
					return;
	
				var main = (MainWindow)(Application.OpenForms[0]);
				main.AddPendingChange(DataType.chestData);

				chest.chestLocation = location;
			}
			catch
			{
				var yPos = (chest.chestLocation>>6);
				var xPos = (chest.chestLocation- (yPos<<6));
				xPosition.Text = xPos.ToString();
			}
			chestDataList[chestIndex] = chest;
		}

		private void yPosition_LostFocus( object sender, EventArgs e )
		{
			if(!yPosition.IsHandleCreated)
				return;

			var chest = chestDataList[chestIndex];
			try
			{
				ushort location = (ushort)(Convert.ToByte(xPosition.Text) + (Convert.ToByte(yPosition.Text)<<6));

				if(location == chest.chestLocation)
					return;
	
				var main = (MainWindow)(Application.OpenForms[0]);
				main.AddPendingChange(DataType.chestData);

				chest.chestLocation = location;
			}
			catch
			{
				var yPos = (chest.chestLocation>>6);
				yPosition.Text = yPos.ToString();
			}
			
			chestDataList[chestIndex] = chest;
		}

		private void kinstoneType_SelectedIndexChanged( object sender, EventArgs e )
		{
			if(!kinstoneType.IsHandleCreated)
				return;

			var chest = chestDataList[chestIndex];
   
			var type = (byte)((int)kinstoneType.SelectedValue);//cant directly go to byte?
	
			if(type == chest.itemSubNumber)
				return;
	
			var main = (MainWindow)(Application.OpenForms[0]);
			main.AddPendingChange(DataType.chestData);

			itemAmount.Text = type.ToString();
			chest.itemSubNumber = type;

			chestDataList[chestIndex] = chest;
		}

		private void itemAmount_LostFocus( object sender, EventArgs e )
		{
			if(!itemAmount.IsHandleCreated)
				return;

			var chest = chestDataList[chestIndex];
			try
			{
				var type = Convert.ToByte(itemAmount.Text);

				if(type == chest.itemSubNumber)
					return;
	
				var main = (MainWindow)(Application.OpenForms[0]);
				main.AddPendingChange(DataType.chestData);

				chest.itemSubNumber = type;
				kinstoneType.SelectedValue = (KinstoneType)type;
			}
			catch
			{
				itemAmount.Text = chest.itemSubNumber.ToString();
			}

			chestDataList[chestIndex] = chest;
		}

		private void itemName_SelectedIndexChanged( object sender, EventArgs e )
		{
            if(!itemName.IsHandleCreated)
				return;

			ItemType value = (ItemType)itemName.SelectedValue;

			var chest = chestDataList[chestIndex];

			if((int)value == chest.itemId)
				return;
	
			var main = (MainWindow)(Application.OpenForms[0]);
			main.AddPendingChange(DataType.chestData);

			chest.itemId = (byte)value;
			chestDataList[chestIndex]=chest;

			amountLabel.Hide();
			itemAmount.Hide();
			kinstoneLabel.Hide();
			kinstoneType.Hide();

			if(value == ItemType.KinstoneX)
			{
				kinstoneLabel.Show();
				kinstoneType.Show();
			}
			else if(value == ItemType.ShellsX)
			{
				amountLabel.Show();
				itemAmount.Show();
			}
		}
	}
}