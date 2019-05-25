using MinishMaker.Core;
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
	public partial class ChestEditorWindow : Form
	{
		private int chestIndex = -1;
		private List<ChestData> data;

		public ChestEditorWindow()
		{
			InitializeComponent();
			itemName.DropDownStyle = ComboBoxStyle.DropDownList;
			kinstoneType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.itemName.DataSource = Enum.GetValues(typeof(ItemType));
			this.kinstoneType.DataSource = Enum.GetValues(typeof(KinstoneType));
			chestIndex = -1;
			if(data==null)
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
				saveButton.Enabled = false;
			}
		}

		public void SetData(List<ChestData> data)
		{
			
			this.data = data;
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
				saveButton.Enabled = false;
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
                saveButton.Enabled = true;
				
                chestIndex = 0;

                LoadChestData(0);

				if(data.Count>1)
				{
					nextButton.Enabled = true;
				}
            }

			indexLabel.Text = chestIndex.ToString();
		}

		private void itemName_SelectedIndexChanged( object sender, EventArgs e )
		{
			ItemType value = (ItemType)itemName.SelectedValue;
            Console.WriteLine(value);
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

		private void nextButton_Click( object sender, EventArgs e )
		{
			chestIndex++;
			prevButton.Enabled=true;
			if(chestIndex == data.Count-1)
			{
				nextButton.Enabled = false;
			}

            indexLabel.Text = chestIndex.ToString();

            LoadChestData(chestIndex);
        }

		private void prevButton_Click( object sender, EventArgs e )
		{
			chestIndex--;
			nextButton.Enabled=true;
			if(chestIndex == 0)
			{
				prevButton.Enabled = false;
			}

            indexLabel.Text = chestIndex.ToString();

            LoadChestData(chestIndex);
		}

        private void LoadChestData(int chest)
        {
            ChestData chestData = data[chest];
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
	}
}
