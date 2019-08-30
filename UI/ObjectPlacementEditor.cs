using MinishMaker.Core;
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
	public partial class ObjectPlacementEditor : Form
	{
		private int objectIndex = -1;
		private int listIndex = -1;
		private List<List<ObjectData>> lists = new List<List<ObjectData>>();
		private List<ObjectData> currentList;
		private bool shouldTrigger = false;

		public ObjectPlacementEditor()
		{
			InitializeComponent();
			objectTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
			objectTypeBox.DataSource = Enum.GetValues(typeof(ObjectCategories));
			objectIdBox.DropDownStyle = ComboBoxStyle.DropDownList;

			LoadData();
		}

		public void LoadData()
		{
			if(MainWindow.currentRoom!=null)
			{
				objectIndex = 0;
				lists.Clear();
				this.lists.Add(MainWindow.currentRoom.GetList1Data());
				this.lists.Add(MainWindow.currentRoom.GetList2Data());
				this.lists.Add(MainWindow.currentRoom.GetList3Data());
				listIndex = 0;
				currentList = lists[listIndex];
				SetData();
			}
		}

		public void SetData()
		{
			shouldTrigger=false;
			newButton.Enabled=true;
			data1Label.Text = "data 1:";
			data2Label.Text = "data 2:";
			data3Label.Text = "data 3:";
			data4Label.Text = "data 4:";
			data5Label.Text = "data 5:";

			nextListButton.Enabled = listIndex!=2;
			prevListButton.Enabled = listIndex!=0;
			listIndexLabel.Text = (listIndex+1)+" ";

			if(currentList.Count==0)
			{
				indexLabel.Text = "0";
				objectTypeBox.Enabled = false;
				objectIdBox.Enabled = false;
				data1Box.Enabled = false;
				data2Box.Enabled = false;
				data3Box.Enabled = false;
				data4Box.Enabled = false;
				data5Box.Enabled = false;
				posXBox.Enabled = false;
				posYBox.Enabled = false;
				flag1Box.Enabled = false;
				flag2Box.Enabled = false;

				removeButton.Enabled = false;
				prevButton.Enabled = false;
				nextButton.Enabled = false;

				objectTypeBox.Text = "0";
				unknownBox.Text = "0";
				objectIdBox.SelectedItem= Object6Types.Item;
				data1Box.Text = "0";
				data2Box.Text = "0";
				data3Box.Text = "0";
				data4Box.Text = "0";
				data5Box.Text = "0";
				posXBox.Text = "0";
				posYBox.Text = "0";
				flag1Box.Text = "0";
				flag2Box.Text = "0";

				((MainWindow)Application.OpenForms[0]).HighlightListObject(-1,-1);
				return;
			}

			indexLabel.Text = objectIndex+"";
			nextButton.Enabled = objectIndex!=currentList.Count-1;
			
			prevButton.Enabled = objectIndex!=0;

			removeButton.Enabled = currentList.Count!=0;
			objectTypeBox.Enabled = true;
			objectIdBox.Enabled = true;
			data1Box.Enabled = true;
			data2Box.Enabled = true;
			data3Box.Enabled = true;
			data4Box.Enabled = true;
			data5Box.Enabled = true;
			posXBox.Enabled = true;
			posYBox.Enabled = true;
			flag1Box.Enabled = true;
			flag2Box.Enabled = true;

			var currentObject = currentList[objectIndex];
			((MainWindow)Application.OpenForms[0]).HighlightListObject(currentObject.pixelX,currentObject.pixelY);
			var trueType = currentObject.objectType & 0x0F;
			if (trueType == 6)
			{
				objectIdBox.DataSource = Enum.GetValues(typeof(Object6Types));
				objectIdBox.SelectedItem = (Object6Types)currentObject.objectId;

				if(currentObject.objectId == 0x0)
				{
					data1Label.Text = "Item id:";
					data2Label.Text = "Item sub:";
					data3Label.Text = "data 3:";
					data4Label.Text = "data 4:";
					data5Label.Text = "data 5:";
				}
			}
			else if(trueType==3)
			{
				objectIdBox.DataSource = Enum.GetValues(typeof(EnemyTypes));
				objectIdBox.SelectedItem = (EnemyTypes)currentObject.objectId;
			}
			else if(trueType==7)
			{
				objectIdBox.DataSource = Enum.GetValues(typeof(NPCTypes));
				objectIdBox.SelectedItem = (NPCTypes)currentObject.objectId;
			}
			else if(trueType==9)
			{
				objectIdBox.Enabled = false;
				data1Box.Enabled = false;
				data2Box.Enabled = false;
				data3Box.Enabled = false;
				data4Box.Enabled = false;
				data5Box.Enabled = false;
				posXBox.Enabled = false;
				posYBox.Enabled = false;
				flag1Box.Enabled = false;
				flag2Box.Enabled = false;
			}
			else
			{
				objectIdBox.Enabled = false;
				data1Box.Enabled = false;
				data2Box.Enabled = false;
				data3Box.Enabled = false;
				data4Box.Enabled = false;
				data5Box.Enabled = false;
				posXBox.Enabled = false;
				posYBox.Enabled = false;
				flag1Box.Enabled = false;
				flag2Box.Enabled = false;
			}

			objectTypeBox.SelectedItem = (ObjectCategories)currentObject.objectType;
			unknownBox.Text = currentObject.objectSub.Hex();
			
			objectTypeValue.Text = currentObject.objectType.Hex();
			objectIdValue.Text = currentObject.objectId.Hex();
			data1Box.Text = currentObject.d1item.Hex();
			data2Box.Text = currentObject.d2itemSub.Hex();
			data3Box.Text = currentObject.d3.Hex();
			data4Box.Text = currentObject.d4.Hex();
			data5Box.Text = currentObject.d5.Hex();
			posXBox.Text = currentObject.pixelX.Hex();
			posYBox.Text = currentObject.pixelY.Hex();
			flag1Box.Text = currentObject.flag1.Hex();
			flag2Box.Text = currentObject.flag2.Hex();

			shouldTrigger=true;
		}

		private void newButton_Click( object sender, EventArgs e )
		{
			if(currentList==null)
			{
				return;
			}

			AddChange();
			var data = new byte[16] {6,0x0F,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			currentList.Add(new ObjectData(data,0)); //add a basic octorok
			objectIndex = currentList.Count()-1;
			SetData();
		}

		private void removeButton_Click( object sender, EventArgs e )
		{
			if(currentList==null)
			{
				return;
			}

			AddChange();
			var curObject = currentList[objectIndex];
			currentList.Remove(curObject);

			if(objectIndex!=0)
				objectIndex-=1;

			SetData();
		}

		private void nextButton_Click( object sender, EventArgs e )
		{
			objectIndex+=1;
			SetData();
		}

		private void prevButton_Click( object sender, EventArgs e )
		{
			objectIndex -=1;
			SetData();
		}

		private void posXBox_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToUInt16(posXBox.Text,16);
				var obj = currentList[objectIndex];
				obj.pixelX = newVal;
				currentList[objectIndex] = obj;
				AddChange();
				SetData();
			}
			catch
			{
				SetData();
			}
		}

		private void posYBox_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToUInt16(posYBox.Text,16);
				var obj = currentList[objectIndex];
				obj.pixelY = newVal;
				currentList[objectIndex] = obj;
				AddChange();
				SetData();
			}
			catch
			{
				SetData();
			}
		}

		private void flag1Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToUInt16(flag1Box.Text,16);
				var obj = currentList[objectIndex];
				obj.flag1 = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void flag2Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToUInt16(flag2Box.Text,16);
				var obj = currentList[objectIndex];
				obj.flag2 = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void data5Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(data5Box.Text,16);
				var obj = currentList[objectIndex];
				obj.d5 = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void data4Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(data4Box.Text,16);
				var obj = currentList[objectIndex];
				obj.d4 = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void data3Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(data3Box.Text,16);
				var obj = currentList[objectIndex];
				obj.d3 = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void data2Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(data2Box.Text,16);
				var obj = currentList[objectIndex];
				obj.d2itemSub = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void data1Box_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(data1Box.Text,16);
				var obj = currentList[objectIndex];
				obj.d1item = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void objectIdBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = (byte)(int)objectIdBox.SelectedItem;
				var obj = currentList[objectIndex];
				obj.objectId = newVal;
				currentList[objectIndex] = obj;
				AddChange();
			}
			catch
			{
				SetData();
			}
		}

		private void objectTypeBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = (byte)(int)objectTypeBox.SelectedItem;
				var obj = currentList[objectIndex];
				obj.objectType = newVal;
				currentList[objectIndex] = obj;
				AddChange();
				SetData();
			}
			catch
			{
				SetData();
			}
		}

		private void AddChange()
		{
			Change change;
			if(listIndex==0)
				change = new List1DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);
			else if(listIndex==1)
				change = new List2DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);
			else
				change = new List3DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);

			MainWindow.AddPendingChange(change);
		}

		private void prevListButton_Click( object sender, EventArgs e )
		{
			listIndex -= 1;
			objectIndex = 0;
			currentList = lists[listIndex];
			SetData();
		}

		private void nextListButton_Click( object sender, EventArgs e )
		{
			listIndex +=1;
			objectIndex = 0;
			currentList = lists[listIndex];
			SetData();
		}
	}
}
