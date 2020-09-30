using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI.Rework
{
    public partial class ObjectPlacementEditorWindow : SubWindow
    {
        private int objectIndex = 0;
        private int listIndex = 0;
        private int[] listKeys;
        private int currentListEntryAmount = 0;
        private Core.Rework.Room currentRoom = null;
        private List<byte> currentListEntry = null;
        private bool shouldTrigger = false;
        private List<byte> copy = null;
        public ObjectPlacementEditorWindow()
        {
            InitializeComponent();
            //setup reading of file
        }

        public override void Setup()
        {
            currentRoom = MainWindow.instance.currentRoom;
            if (currentRoom == null)
            {
                return;
            }
            objectIndex = 0;
            listIndex = 0;
            listKeys = currentRoom.MetaData.GetListInformationKeys();

            SetData();
        }

        public override void Cleanup()
        {
            
        }

        public void SetData()
        {
            shouldTrigger = false;
            newButton.Enabled = true;

            nextListButton.Enabled = listIndex < listKeys.Length - 1;
            prevListButton.Enabled = listIndex != 0;

            if (listKeys.Length == 0)
            {
                indexLabel.Text = "0";
                copyButton.Enabled = false;
                pasteButton.Enabled = false;

                removeButton.Enabled = false;
                prevButton.Enabled = false;
                nextButton.Enabled = false;

                ((MainWindow)Application.OpenForms[0]).HighlightListObject(-1, -1);
                return;
            }
            var currentListNumber = listKeys[listIndex];
            currentListEntryAmount = currentRoom.MetaData.GetListEntryAmount(currentListNumber);

            listIndexLabel.Text = (currentListNumber + " ");

            indexLabel.Text = objectIndex + "";

            nextButton.Enabled = objectIndex != currentListEntryAmount - 1;
            prevButton.Enabled = objectIndex != 0;

            removeButton.Enabled = currentListEntryAmount != 0;

            copyButton.Enabled = true;
            //pasteButton.Enabled = copy.HasValue;

            currentListEntry = currentRoom.MetaData.GetListInformationEntry(currentListNumber, objectIndex);
            /*((MainWindow)Application.OpenForms[0]).HighlightListObject(currentObject.pixelX, currentObject.pixelY);
            var trueType = currentObject.objectType & 0x0F;
            if (trueType == 6)
            {
                objectIdBox.DataSource = Enum.GetValues(typeof(Object6Types));
                objectIdBox.SelectedItem = (Object6Types)currentObject.objectId;

                if (currentObject.objectId == 0x0)
                {
                    data1Label.Text = "Item id:";
                    data2Label.Text = "Item sub:";
                    data3Label.Text = "data 3:";
                    data4Label.Text = "data 4:";
                    data5Label.Text = "data 5:";
                }
            }
            else if (trueType == 3)
            {
                objectIdBox.DataSource = Enum.GetValues(typeof(EnemyTypes));
                objectIdBox.SelectedItem = (EnemyTypes)currentObject.objectId;
            }
            else if (trueType == 7)
            {
                objectIdBox.DataSource = Enum.GetValues(typeof(NPCTypes));
                objectIdBox.SelectedItem = (NPCTypes)currentObject.objectId;
            }
            else
            {
                objectIdBox.Enabled = false;
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
            */
            shouldTrigger = true;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            if (listKeys.Length == 0)
            {
                return;
            }

            AddChange();
            currentRoom.MetaData.AddNewListInformation(listKeys[listIndex], objectIndex);

            objectIndex += 1;
            SetData();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (currentListEntryAmount == 0)
            {
                return;
            }

            AddChange();
            currentRoom.MetaData.RemoveListInformation(listKeys[listIndex], objectIndex);

            if (objectIndex != 0)
                objectIndex -= 1;

            SetData();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            objectIndex += 1;
            SetData();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            objectIndex -= 1;
            SetData();
        }

        private void prevListButton_Click(object sender, EventArgs e)
        {
            listIndex -= 1;
            objectIndex = 0;
            SetData();
        }

        private void nextListButton_Click(object sender, EventArgs e)
        {
            listIndex += 1;
            objectIndex = 0;
            SetData();
        }

        private void AddChange()
        {
            //ListDataChange = new ListDataChange(currentRoom.Parent.Id, currentRoom.Id, listKeys[listIndex]);
            /*if (listIndex == 0)
                change = new List1DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);
            else if (listIndex == 1)
                change = new List2DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);
            else
                change = new List3DataChange(MainWindow.currentArea, MainWindow.currentRoom.Index);

            Project.Instance.AddPendingChange(change);*/
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            copy = new List<byte>(currentListEntry);
            pasteButton.Enabled = true;
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            currentListEntry.Clear();
            currentListEntry.AddRange(copy);
            AddChange();
            SetData();
        }

        private void ChangedHandler(Action changeAction)
        {
            if (!shouldTrigger)
                return;

            changeAction.Invoke();

            AddChange();
        }

        //because why type the same 4 times
        private void HandleByteString(ref TextBox textBox, ref byte property)
        {
            try
            {
                var newVal = Convert.ToByte(textBox.Text, 16);

                property = newVal;
            }
            catch
            {
                textBox.Text = property.Hex();
            }
        }

        //or 5 times
        private void HandleUInt16String(ref TextBox textBox, ref ushort property)
        {
            try
            {
                var newVal = Convert.ToUInt16(textBox.Text, 16);

                property = newVal;
            }
            catch
            {
                textBox.Text = property.Hex();
            }
        }
    }
}
