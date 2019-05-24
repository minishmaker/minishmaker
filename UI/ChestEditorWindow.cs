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
			}
		}

		public void SetData(List<ChestData> data)
		{
			this.data = data;
			entityType.Enabled = true;
			entityId.Enabled = true;
			itemName.Enabled = true;
			kinstoneType.Enabled =true;
			itemAmount.Enabled = true;
			xPosition.Enabled = true;
			yPosition.Enabled = true;
			nextButton.Enabled = true;
			prevButton.Enabled = false;
			chestIndex = 0;
		}

		private void itemName_SelectedIndexChanged( object sender, EventArgs e )
		{
			ItemType value = (ItemType)itemName.SelectedValue;
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
			//no loading yet
		}

		private void prevButton_Click( object sender, EventArgs e )
		{
			chestIndex--;
			nextButton.Enabled=true;
			if(chestIndex == 0)
			{
				prevButton.Enabled = false;
			}
			//no loading yet
		}
	}
}
