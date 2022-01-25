using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MinishMaker.Utilities;
using MinishMaker.Utilities.Rework;
using static MinishMaker.Utilities.Rework.ListFileParser;

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
        private int markerId = 100;
        private List<int> usedMarkers = new List<int>();

        public ObjectPlacementEditorWindow()
        {
            InitializeComponent();
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
            foreach(int markerId in usedMarkers)
            {
                MainWindow.instance.RemoveMarker(markerId);
            }
            usedMarkers.Clear();
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
                
                //((MainWindow)Application.OpenForms[0]).HighlightListObject(-1, -1);
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
            pasteButton.Enabled = copy != null;

            currentListEntry = currentRoom.MetaData.GetListInformationEntry(currentListNumber, objectIndex);
            GetRepresentation();
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
            Core.Rework.Project.Instance.AddPendingChange(new Core.ChangeTypes.Rework.ListDataChange(currentRoom.Parent.Id, currentRoom.Id, listKeys[listIndex] ));
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

        private void ChangedHandler(FormElement element, int newValue)
        {
            if (!shouldTrigger)
                return;
            /*switch (targetType.ToLower())
            {
                case "part":
                    int startBytePos = targetPos / 8;
                    var startBitPos = targetPos &0x7; //0-7
                    int byteCount = (int)Math.Ceiling((startBitPos + valueLength + 1)/8d);

                    for (int i = 0; i<byteCount; i++) {
                        var val = currentListEntry[i + startBytePos] << (i * 8);
                        dataValue += val;
                    }

                    var mask = 0;
                    for(int i = 0; i< valueLength; i++) { //create a mask for valueLength - 1 bits
                        mask += (int)Math.Pow(2, i);
                    }
                    dataValue = (dataValue >> startBitPos);
                    dataValue = dataValue & mask; //shave off the first 0-7 bits, then apply mask to get the value

                    break;
            }
            */
            //changeAction.Invoke();
            bool hasChanged = false;
            var targetPos = element.valuePos - 1;
            switch(element.valueType) {
                case "bit":
                    var bytePos = targetPos / 8;
                    var bitPos = targetPos & 0x7;
                    byte shifted = (byte)(1 << bitPos);
                    if(newValue == 1) {
                        currentListEntry[bytePos] += shifted; //turn on bit
                    } else {
                        currentListEntry[bytePos] -= shifted; //turn off bit
                    }
                    hasChanged = true;
                    break;
                case "int":
                    byte intval = (byte)(newValue >> 24);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 3] != intval);
                    currentListEntry[targetPos + 3] = intval;
                    goto case "tri";
                case "tri":
                    byte trival = (byte)(newValue >> 16);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 2] != trival);
                    currentListEntry[targetPos + 2] = trival;
                    goto case "short";
                case "short":
                    byte shortval = (byte)(newValue >> 8);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 1] != shortval);
                    currentListEntry[targetPos + 1] = shortval;
                    goto case "byte";
                case "byte":
                    byte byteval = (byte)(newValue);
                    hasChanged = hasChanged || (currentListEntry[targetPos] != byteval);
                    currentListEntry[targetPos] = byteval;
                    break;
                default:
                    break;
            }
            //currentListEntry();
            if (hasChanged)
            {
                AddChange();
            }
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

        private void GetRepresentation()
        {
            
            Console.WriteLine();
            Console.WriteLine("----------");
            var byteRep = "";
            for(int i = 0; i< currentListEntry.Count; i++){
                byteRep += currentListEntry[i].Hex() + " ";
            }
            Console.WriteLine(byteRep);
            List<Filter> filters = ListFileParser.FilterData(currentListEntry.ToArray(), 0, listKeys[listIndex]).Item1;
            //done filtering, get all elements

            var tabIndex = 30;
            for (int i = elementTable.Controls.Count - 1; i >= 0; --i)
                elementTable.Controls[i].Dispose();

            elementTable.Controls.Clear();
            elementTable.RowCount = 0;
            elementTable.ColumnCount = 0;
            elementTable.SuspendLayout();
            foreach (var filter in filters)
            {
                foreach (var element in filter.elements)
                {
                    CreateElement(element, ref tabIndex);
                }
                foreach(var marker in filter.markers)
                {
                    
                }
            }
            elementTable.ResumeLayout();
        }

        private void CreateElement(FormElement element, ref int tabIndex)
        {
            switch(element.type.ToLower())
            {
                case "enum":
                    CreateEnumElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case "number":
                    CreateNumberElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case "hexnumber":
                    CreateHexNumberElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case "text":
                    CreateLabelElement(element);
                    break;
                default:
                    throw new ArgumentException($"This should not be possible as validation already happened. {element.type}");
            }
        }

        private void CreateLabelElement(FormElement element)
        {
            Label labelElement = new Label();
            labelElement.AutoSize = true;
            labelElement.Margin = new Padding(4, 0, 4, 0);
            labelElement.Text = element.label;
            labelElement.Anchor = AnchorStyles.Right;

            elementTable.Controls.Add(labelElement, element.column-1, element.row-1);
        }

        private void CreateEnumElement(FormElement element, int tabIndex)
        {
            var column = element.column;
            if(element.label.Length != 0)
            {
                CreateLabelElement(element);
                column += 1;
            }
            int value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength);
            var enumSource = ListFileParser.GetEnum(element.enumType);

            var enumElement = new ComboBox();

            enumElement.FormattingEnabled = true;
            enumElement.DisplayMember = "entryName";
            enumElement.Width = 50;
            enumElement.DropDownWidth = 200;
            foreach(var entryId in enumSource.Keys)
            {
                var entryName = enumSource[entryId];
                var eObj = new EnumObject() { entryName = entryName, entryId = entryId };
                enumElement.Items.Add(eObj);
                if (entryId == value)
                {
                    enumElement.SelectedItem = eObj;
                }
            }

            if(enumElement.SelectedItem == null)
            {
                enumElement.SelectedItem = enumElement.Items[0];
            }

            enumElement.DropDownStyle = ComboBoxStyle.DropDownList;
            enumElement.TabIndex = tabIndex;

            //TODO: add change logic
            enumElement.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(element, ((EnumObject)enumElement.SelectedItem).entryId); });

            elementTable.Controls.Add(enumElement, column-1, element.row-1);
        }

        private void CreateHexNumberElement(FormElement element, int tabIndex)
        {
            int value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength);
            var column = element.column;
            if (element.label.Length != 0)
            {
                CreateLabelElement(element);
                column += 1;
            }

            var hexNumberElement = new TextBox();
            hexNumberElement.TabIndex = tabIndex;
            hexNumberElement.Text = "0x" + value.Hex();
            hexNumberElement.LostFocus += new EventHandler((object o, EventArgs e) => { TextboxLostFocus(hexNumberElement, element); });
            var size = 0;
            switch(element.valueType.ToLower())
            {
                case "byte":
                    size = 16;
                    break;
                case "short":
                    size = 32;
                    break;
                case "tri":
                    size = 48;
                    break;
                case "int":
                    size = 64;
                    break;
                case "part":
                    size = 16 * (int)Math.Ceiling((double)element.valueLength/8);
                    break;
                default:
                    break;
            }
            hexNumberElement.Size = new Size(16+size, hexNumberElement.Size.Height);

            elementTable.Controls.Add(hexNumberElement, column-1, element.row-1);
        }

        private void CreateNumberElement(FormElement element, int tabIndex)
        {
            var value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength);
            var column = element.column;
            if (element.label.Length != 0)
            {
                CreateLabelElement(element);
                column += 1;
            }

            var numberElement = new TextBox();
            numberElement.TabIndex = tabIndex;
            numberElement.Text = "" + value;
            numberElement.LostFocus += new EventHandler((object o, EventArgs e) => { TextboxLostFocus(numberElement, element); });

            elementTable.Controls.Add(numberElement, column-1, element.row-1);
        }

        private void TextboxLostFocus(TextBox textBoxElement, FormElement element)
        {
            var value = 0;
            var success = NumberUtil.ParseInt(textBoxElement.Text, ref value);
            if (!success)
            {
                textBoxElement.Text = "" + GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength);
            }
            ChangedHandler(element, value);
        }

        private int GetTargetValue(string targetType, int targetPos, int valueLength)
        {
            return ListFileParser.GetTargetValue(currentListEntry.ToArray(), listKeys[listIndex], targetType, targetPos, valueLength);
        }
    }
}

public class EnumObject
{
    public string entryName { get; set; }
    public int entryId { get; set; }
}
