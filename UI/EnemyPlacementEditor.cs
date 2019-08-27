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
	public partial class EnemyPlacementEditor : Form
	{
		private int enemyIndex = -1;
		private List<EnemyData> enemyDataList;
		private bool shouldTrigger = false;

		public EnemyPlacementEditor()
		{
			InitializeComponent();
			enemyId.DropDownStyle = ComboBoxStyle.DropDownList;
			enemyId.DataSource = Enum.GetValues(typeof(EnemyTypes));
			enemyIndex = -1;
			enemyId.Enabled=false;
			subId.Enabled=false;
			posX.Enabled=false;
			posY.Enabled=false;
			prevButton.Enabled=false;
			nextButton.Enabled=false;
			newButton.Enabled=false;
			removeButton.Enabled=false;
			LoadData();
		}

		public void LoadData()
		{
			if(MainWindow.currentRoom!=null)
			{
				enemyIndex = 0;
				enemyDataList = MainWindow.currentRoom.GetEnemyPlacementData();
				SetData();
			}
		}

		public void SetData()
		{
			shouldTrigger=false;
			newButton.Enabled=true;
			if(enemyDataList.Count==0)
			{
				indexLabel.Text = "0";
				enemyId.Enabled=false;
				subId.Enabled=false;
				posX.Enabled=false;
				posY.Enabled=false;
				prevButton.Enabled=false;
				nextButton.Enabled=false;
				removeButton.Enabled=false;
				objectType.Text = "0";
				objectSub.Text = "0";
				enemyId.SelectedItem= EnemyTypes.Octorok;
				subId.Text = "0";
				unknown1.Text = "0";
				unknown2.Text = "0";
				posX.Text = "0";
				posY.Text = "0";
				unknown3.Text = "0";
				unknown4.Text = "0";
				((MainWindow)Application.OpenForms[0]).HighlightEnemy(-1,-1);
				return;
			}

			indexLabel.Text = enemyIndex+"";
			var enemy = enemyDataList[enemyIndex];
			if(enemyDataList[enemyIndex].objectType!=3)
			{
				enemyId.Enabled=false;
				subId.Enabled=false;
				posX.Enabled=false;
				posY.Enabled=false;
				((MainWindow)Application.OpenForms[0]).HighlightEnemy(-1,-1);
			}
			else
			{		
				enemyId.Enabled=true;
				subId.Enabled=true;
				posX.Enabled=true;
				posY.Enabled=true;
				((MainWindow)Application.OpenForms[0]).HighlightEnemy(enemy.xpos,enemy.ypos);
			}

			objectType.Text = enemy.objectType.Hex();
			objectSub.Text = enemy.objectSub.Hex();
			enemyId.SelectedItem = (EnemyTypes)enemy.id;
			subId.Text = enemy.subId.Hex();
			unknown1.Text = enemy.unknown1.Hex();
			unknown2.Text = enemy.unknown2.Hex();
			posX.Text = enemy.xpos.Hex();
			posY.Text = enemy.ypos.Hex();
			unknown3.Text = enemy.unknown3.Hex();
			unknown4.Text = enemy.unknown4.Hex();
			
			if(enemyIndex>=enemyDataList.Count-1)
			{
				nextButton.Enabled=false;
			}
			else
			{
				nextButton.Enabled=true;
			}

			if(enemyIndex<=0)
			{
				prevButton.Enabled = false;
			}
			else
			{
				prevButton.Enabled=true;
			}

			shouldTrigger=true;
		}

		private void posX_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;

			try
			{
				var newVal = Convert.ToUInt16(posX.Text,16);
				var en = enemyDataList[enemyIndex];
				en.xpos = newVal;
				enemyDataList[enemyIndex] = en;
				MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void posY_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;

			try
			{
				var newVal = Convert.ToUInt16(posY.Text,16);
				var en = enemyDataList[enemyIndex];
				en.ypos = newVal;
				enemyDataList[enemyIndex] = en;
				MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void enemyId_SelectedIndexChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;

			try
			{
				var newVal = (byte)(int)enemyId.SelectedItem;
				var en = enemyDataList[enemyIndex];
				en.id = newVal;
				enemyDataList[enemyIndex] = en;
				MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void subId_TextChanged( object sender, EventArgs e )
		{
			if(!shouldTrigger)
				return;
			try
			{
				var newVal = Convert.ToByte(subId.Text,16);
				var en = enemyDataList[enemyIndex];
				en.subId = newVal;
				enemyDataList[enemyIndex] = en;
				MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			}
			catch
			{
				SetData();
			}
		}

		private void newButton_Click( object sender, EventArgs e )
		{
			if(enemyDataList==null)
			{
				return;
			}

			MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			enemyDataList.Add(new EnemyData(3,15,0,0,0,0,0,0,0,0)); //add a basic octorok
			enemyIndex = enemyDataList.Count()-1;
			SetData();
		}

		private void removeButton_Click( object sender, EventArgs e )
		{
			if(enemyDataList==null)
			{
				return;
			}

			MainWindow.AddPendingChange(new EnemyPlacementDataChange(MainWindow.currentArea, MainWindow.currentRoom.Index));
			var curEnemy = enemyDataList[enemyIndex];
			enemyDataList.Remove(curEnemy);

			if(enemyIndex!=0)
				enemyIndex-=1;

			SetData();
		}

		private void nextButton_Click( object sender, EventArgs e )
		{
			enemyIndex+=1;
			SetData();
		}

		private void prevButton_Click( object sender, EventArgs e )
		{
			enemyIndex -=1;
			SetData();
		}
	}
}
