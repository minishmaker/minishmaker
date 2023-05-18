using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;
using static MinishMaker.Utilities.ObjectDefinitionParser;

namespace MinishMaker.UI
{
    public partial class ObjectPlacementEditorWindow : SubWindow
    {
        private int objectIndex = 0;
        private int listIndex = 0;
        private int[] listKeys;
        private int currentListEntryAmount = 0;
        private Room currentRoom = null;
        private List<byte> currentListEntry = null;
        private bool shouldTrigger = false;
        private List<byte> copy = null;
        private int markerId = 100;
        private List<byte> currentLinkedLists = new List<byte>();
        private List<int> usedMarkers = new List<int>();

        public ObjectPlacementEditorWindow()
        {
            InitializeComponent();
            ListLinkBox.KeyDown += EnterUnfocus;
            ObjectDefinitionParser.AddSetupFunc("cross", CreateCrossMarkerFunc);
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
            foreach (int markerId in usedMarkers)
            {
                MainWindow.instance.RemoveMarker(markerId);
            }
            usedMarkers.Clear();
        }

        public void SetData()
        {
            shouldTrigger = false;

            nextListButton.Enabled = listIndex < listKeys.Length - 1;
            prevListButton.Enabled = listIndex != 0;

            newButton.Enabled = true;

            if (listKeys.Length == 0)
            {
                indexLabel.Text = "0";
                copyButton.Enabled = false;
                pasteButton.Enabled = false;

                removeButton.Enabled = false;
                prevButton.Enabled = false;
                nextButton.Enabled = false;
                ListLinkBox.Enabled = false;
                //((MainWindow)Application.OpenForms[0]).HighlightListObject(-1, -1);
                return;
            }
            ListLinkBox.Enabled = true;
            var currentListNumber = listKeys[listIndex];
            currentListEntryAmount = currentRoom.MetaData.GetListEntryAmount(currentListNumber);

            listIndexLabel.Text = (currentListNumber + " ");

            indexLabel.Text = objectIndex + "";

            nextButton.Enabled = objectIndex != currentListEntryAmount - 1;
            prevButton.Enabled = objectIndex != 0;

            removeButton.Enabled = currentListEntryAmount != 0;

            copyButton.Enabled = true;
            pasteButton.Enabled = copy != null;

            ListLinkBox.Text = "";

            var link = currentRoom.MetaData.GetLinkFor(listKeys[listIndex]);

            if (link != 0xFF)
            {
                copyButton.Enabled = false;
                pasteButton.Enabled = false;
                newButton.Enabled = false;
                removeButton.Enabled = false;
                prevButton.Enabled = false;
                nextButton.Enabled = false;
                ListLinkBox.Text = link + "";
            }

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
            currentLinkedLists = currentRoom.MetaData.GetAllLinkedTo(listKeys[listIndex]);
            objectIndex = 0;
            SetData();
        }

        private void nextListButton_Click(object sender, EventArgs e)
        {
            listIndex += 1;
            currentLinkedLists = currentRoom.MetaData.GetAllLinkedTo(listKeys[listIndex]);
            objectIndex = 0;
            SetData();
        }

        private void AddChange()
        {
            //current status, need to find a way to see if the link is to original data or a change
            Project.Instance.AddPendingChange(new ListDataChange(currentRoom.Parent.Id, currentRoom.Id, listKeys[listIndex]));
            for (var i = 0; i < currentLinkedLists.Count; i++)
            {
                Project.Instance.AddPendingChange(new ListDataChange(currentRoom.Parent.Id, currentRoom.Id, currentLinkedLists[i])); //add changes to linked lists so they dont point to vanilla data
            }
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
            
            /*
            var undoData = new object[] { currentRoom.Parent.Id, currentRoom.Id };
            var entry = new UndoRedoEntry(undoData, DoChangeRoom, redoData, DoChangeRoom, UndoRedoEntry.ActionEnum.CHANGE_ROOM);
            entry.Redo(); //do entry first, will error if bad
            uRManager_.AddEntry(entry); //add if entry worked
            */
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
            switch (element.valueType)
            {
                case ObjectValueType.BIT:
                    var bytePos = targetPos / 8;
                    var bitPos = targetPos & 0x7;
                    byte shifted = (byte)(1 << bitPos);
                    if (newValue == 1)
                    {
                        currentListEntry[bytePos] += shifted; //turn on bit
                    }
                    else
                    {
                        currentListEntry[bytePos] -= shifted; //turn off bit
                    }
                    hasChanged = true;
                    break;
                case ObjectValueType.INT:
                    byte intval = (byte)(newValue >> 24);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 3] != intval);
                    currentListEntry[targetPos + 3] = intval;
                    goto case ObjectValueType.TRI;
                case ObjectValueType.TRI:
                    byte trival = (byte)(newValue >> 16);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 2] != trival);
                    currentListEntry[targetPos + 2] = trival;
                    goto case ObjectValueType.SHORT;
                case ObjectValueType.SHORT:
                    byte shortval = (byte)(newValue >> 8);
                    hasChanged = hasChanged || (currentListEntry[targetPos + 1] != shortval);
                    currentListEntry[targetPos + 1] = shortval;
                    goto case ObjectValueType.BYTE;
                case ObjectValueType.BYTE:
                    byte byteval = (byte)(newValue);
                    hasChanged = hasChanged || (currentListEntry[targetPos] != byteval);
                    currentListEntry[targetPos] = byteval;
                    break;
                case ObjectValueType.PART: //example, data pos 46->45, valuelength 4 byte 5 = 20, byte 6 = 22// 39->38, length 6, byte 1 = byte 2 = 
                    var maxValue = (int)Math.Pow(2, element.valueLength)-1;    
                    int startBytePos = targetPos / 8; //start byte 5
                    var startBitPos = targetPos & 0x7; //0-7 //start bit 5
                    newValue = newValue << startBitPos; //align to bytes
                    var fullMask = maxValue << startBitPos;
                    int byteCount = (int)Math.Ceiling((startBitPos + element.valueLength) / 8d);// 5+4 = 9 , /8 = 1r1 , ceil = 2

                    for (int i = 0; i < byteCount; i++)
                    {
                        var val = currentListEntry[i + startBytePos] << (i * 8); //20 << 40 + 20 << 48
                        //newValue += val;

                        var inversePartMask = 0b11111111 - ((fullMask >> (i * 8)) & 0xFF);
                        val = (val & inversePartMask) + ((newValue >> (i * 8)) & 0xFF);
                        hasChanged = hasChanged || (currentListEntry[i + startBytePos] != val);
                        currentListEntry[i + startBytePos] = (byte)val; 
                    }
                    break;
                default:
                    break;
            }

            if (hasChanged)
            {
                //currentRoom.MetaData.GetListInformationEntry(currentListNumber, objectIndex);
                AddChange();
                SetData();
            }
        }
        

        private void GetRepresentation()
        {
            elementTable.Hide();
            elementTable.SuspendLayout();
            
            foreach (int markerId in usedMarkers)
            {
                MainWindow.instance.RemoveMarker(markerId);
            }
            usedMarkers.Clear();

            var tabIndex = 30;
            for (int i = elementTable.Controls.Count - 1; i >= 0; --i)
            {
                elementTable.Controls[i].Dispose();
            }

            elementTable.Controls.Clear();
            elementTable.RowCount = 0;
            elementTable.ColumnCount = 0;

            var link = currentRoom.MetaData.GetLinkFor(listKeys[listIndex]);
            if (link != 0xFF)
            {
                elementTable.ResumeLayout();
                elementTable.Show();
                return;
            }

            System.Console.WriteLine();
            System.Console.WriteLine("----------");
            var byteRep = "";
            for (int i = 0; i < currentListEntry.Count; i++)
            {
                byteRep += currentListEntry[i].Hex() + " ";
            }
            System.Console.WriteLine(byteRep);
            int dataSize;
            Filter filter = FilterData(out dataSize, currentListEntry, listKeys[listIndex]);
            //done filtering, get all elements


            while (true)
            {
                foreach (var element in filter.elements)
                {
                    if (element.valueType == ObjectValueType.TABLEPOINTER)
                    {
                        nextButton.Enabled = false;
                        prevButton.Enabled = false;
                        newButton.Enabled = false;
                        removeButton.Enabled = false;
                    }
                    CreateElement(element, ref tabIndex);
                }

                foreach (var marker in filter.markers)
                {
                    var markerData = ObjectDefinitionParser.SetupMarker(marker.markerType, marker.data);
                    var id = markerId + usedMarkers.Count();
                    usedMarkers.Add(id);

                    MainWindow.instance.AddMarker(id, markerData.Item1, markerData.Item2);
                }
                if (filter.parent == null)
                {
                    break;
                }
                filter = filter.parent;
            }
            elementTable.ResumeLayout();


            if (elementTable.Width > Width)
            {
                Width = elementTable.Width + 20;
            }

            elementTable.Show();
        }

        private void CreateElement(FormElement element, ref int tabIndex)
        {
            switch (element.type)
            {
                case FormVisualType.ENUM:
                    CreateEnumElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case FormVisualType.NUMBER:
                    CreateNumberElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case FormVisualType.HEXNUMBER:
                    CreateHexNumberElement(element, tabIndex);
                    tabIndex += 1;
                    break;
                case FormVisualType.TEXT:
                    CreateLabelElement(element);
                    break;
                default:
                    throw new System.ArgumentException($"This should not be possible as validation already happened. {element.type}");
            }
        }

        private void CreateLabelElement(FormElement element, bool useColspan = false)
        {
            var colspan = useColspan ? element.colspan : 1;
            for (int i = 0; i < colspan; i++)
            {
                if (elementTable.GetControlFromPosition(element.column - 1 + i, element.row - 1) != null)
                {
                    return;
                }
            }

            Label labelElement = new Label();
            labelElement.AutoSize = true;
            labelElement.Margin = new Padding(4, 0, 4, 0);
            labelElement.Text = element.label;
            labelElement.Anchor = AnchorStyles.Right;

            elementTable.Controls.Add(labelElement, element.column - 1, element.row - 1);
            elementTable.SetColumnSpan(labelElement, colspan);
        }

        private void CreateEnumElement(FormElement element, int tabIndex)
        {
            var column = element.column;
            for (int i = 0; i < element.colspan; i++)
            {
                if (elementTable.GetControlFromPosition(column - 1 + i, element.row - 1) != null)
                {
                    return;
                }
            }

            if (element.label.Length != 0)
            {
                if (elementTable.GetControlFromPosition(column - 1 + element.colspan, element.row - 1) != null)
                {
                    return;
                }

                CreateLabelElement(element, false);
                column += 1;
            }
            int value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength, currentListEntry);
            var enumSource = ObjectDefinitionParser.GetEnum(element.enumType);

            var enumElement = new ComboBox();

            enumElement.FormattingEnabled = true;
            enumElement.DisplayMember = "entryName";
            enumElement.DropDownWidth = 200;
            var largestName = 0;
            foreach (var entryId in enumSource.Keys)
            {
                var entryName = enumSource[entryId];
                if(entryName.Length > largestName)
                {
                    largestName = entryName.Length;    
                }
                var eObj = new EnumObject() { entryName = entryName, entryId = entryId };
                enumElement.Items.Add(eObj);
                if (entryId == value)
                {
                    enumElement.SelectedItem = eObj;
                }
            }

            //enumElement.Width = 50;
            if (largestName < 10)
            {
                enumElement.Width = 60;
            }
            else
            {
                enumElement.Width = (int)(largestName * 6);
            }

            if (enumElement.SelectedItem == null)
            {
                enumElement.SelectedItem = enumElement.Items[0];
            }

            enumElement.DropDownStyle = ComboBoxStyle.DropDownList;
            enumElement.TabIndex = tabIndex;
            
            //TODO: add change logic
            enumElement.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(element, ((EnumObject)enumElement.SelectedItem).entryId); });

            elementTable.Controls.Add(enumElement, column - 1, element.row - 1);
            elementTable.SetColumnSpan(enumElement, element.colspan);
        }

        private void CreateHexNumberElement(FormElement element, int tabIndex)
        {
            var column = element.column;
            for (int i = 0; i < element.colspan; i++)
            {
                if (elementTable.GetControlFromPosition(column - 1 + i, element.row - 1) != null)
                {
                    return;
                }
            }

            if (element.label.Length != 0)
            {
                if (elementTable.GetControlFromPosition(column - 1 + element.colspan, element.row - 1) != null)
                {
                    return;
                }

                CreateLabelElement(element, false);
                column += 1;
            }

            int value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength, currentListEntry);
            var hexNumberElement = new TextBox();
            hexNumberElement.TabIndex = tabIndex;
            hexNumberElement.Text = "0x" + value.Hex();
            hexNumberElement.LostFocus += new EventHandler((object o, EventArgs e) => { TextboxLostFocus(hexNumberElement, element); });
            hexNumberElement.KeyDown += EnterUnfocus;

            var size = 0;
            switch (element.valueType)
            {
                case ObjectValueType.BYTE:
                    size = 16;
                    break;
                case ObjectValueType.SHORT:
                    size = 32;
                    break;
                case ObjectValueType.TRI:
                    size = 48;
                    break;
                case ObjectValueType.TABLEPOINTER:
                case ObjectValueType.INT:
                    size = 64;
                    break;
                case ObjectValueType.PART:
                    size = 16 * (int)Math.Ceiling((double)element.valueLength / 8);
                    break;
                default:
                    break;
            }
            hexNumberElement.Size = new Size(16 + size, hexNumberElement.Size.Height);

            elementTable.Controls.Add(hexNumberElement, column - 1, element.row - 1);
            elementTable.SetColumnSpan(hexNumberElement, element.colspan);
        }

        private void CreateNumberElement(FormElement element, int tabIndex)
        {
            var column = element.column;
            for (int i = 0; i < element.colspan; i++)
            {
                if (elementTable.GetControlFromPosition(column - 1 + i, element.row - 1) != null)
                {
                    return;
                }
            }

            if (element.label.Length != 0)
            {
                if (elementTable.GetControlFromPosition(column - 1 + element.colspan, element.row - 1) != null)
                {
                    return;
                }

                CreateLabelElement(element, false);
                column += 1;
            }

            var value = GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength, currentListEntry);
            
            var numberElement = new TextBox();
            numberElement.TabIndex = tabIndex;
            numberElement.Text = "" + value;
            numberElement.LostFocus += new EventHandler((object o, EventArgs e) => { TextboxLostFocus(numberElement, element); });
            numberElement.KeyDown += EnterUnfocus;

            elementTable.Controls.Add(numberElement, column - 1, element.row - 1);
            elementTable.SetColumnSpan(numberElement, element.colspan);
        }

        private void TextboxLostFocus(TextBox textBoxElement, FormElement element)
        {
            var value = 0;
            var success = NumberUtil.TryParse(textBoxElement.Text, out value);
 
            if(success && element.valueType == ObjectValueType.PART && value > Math.Pow(2,element.valueLength)-1)
            {
                success = false;
            }

            if (!success)
            {
                textBoxElement.Text = "" + GetTargetValue(element.valueType, element.valuePos - 1, element.valueLength, currentListEntry);
                return;
            }
            ChangedHandler(element, value);
        }

        public Tuple<Object, Func<Object, Tuple<Point[], Brush>>> CreateCrossMarkerFunc(List<string> rawData)
        {
            
            if (rawData.Count < 6)
            {
                throw new ArgumentException("Not enough data given to create a cross marker, at least 6 entries needed (x datatype, x value position, x value length, y datatype, y value position, y value length)");
            }
            //x datatype
            //x position
            //x value length
            //y datatype
            //y position
            //y value length
            //brush
            //length
            //width
            CrossMarkerData data = new CrossMarkerData();
            ObjectValueType xType;
            ObjectValueType yType;
            int xpos;
            int ypos;
            int xlength;
            int ylength;
            if (!Enum.TryParse(rawData[0].ToUpper(), out xType))
            {
                throw new ParserException($"1st cross data value {rawData[0]} is not a valid x value type");
            }
            if(!int.TryParse(rawData[1], out xpos))
            {
                throw new ParserException($"2nd cross data value {rawData[1]} is not a valid x value position");
            }
            if (!int.TryParse(rawData[2], out xlength))
            {
                throw new ParserException($"3rd cross data value {rawData[2]} is not a valid x value length");
            }

            if (!Enum.TryParse(rawData[3].ToUpper(), out yType))
            {
                throw new ParserException($"4th cross data value {rawData[3]} is not a valid y value type");
            }
            if (!int.TryParse(rawData[4], out ypos))
            {
                throw new ParserException($"5th cross data value {rawData[4]} is not a valid y value position");
            }
            if (!int.TryParse(rawData[5], out ylength))
            {
                throw new ParserException($"6th cross data value {rawData[5]} is not a valid y value length");
            }

            // - 1 because its 0 based in the array but 1 based in the file
            Point markerPosition = new Point();
            markerPosition.X = GetTargetValue(xType, xpos - 1, xlength, currentListEntry);
            markerPosition.Y = GetTargetValue(yType, ypos - 1, ylength, currentListEntry);
            data.position = markerPosition;
            for (int i = 6; i < rawData.Count; i++)
            {
                if (i == 6)
                {
                    Color color = Color.FromName(rawData[i]);
                    if (color == null)
                    {
                        throw new ParserException($"7th cross data value {rawData[i]} is not a valid brush name");
                    }
                    //get brush
                    
                    data.brush = new SolidBrush(color);
                }
                else if (i == 7)
                {
                    int lineLength;
                    if (!int.TryParse(rawData[i], out lineLength))
                    {
                        throw new ParserException($"8th cross data value {rawData[i]} is not a valid length");
                    }
                    data.length = lineLength;
                }
                else if (i == 8)
                {
                    int lineWidth;
                    if (!int.TryParse(rawData[i], out lineWidth))
                    {
                        throw new ParserException($"9th cross data value {rawData[i]} is not a valid width");
                    }
                    data.width = lineWidth;
                }
            }
            
            return new Tuple<Object, Func<Object, Tuple<Point[],Brush>>>(data, CrossMarkerTemplate);

        }

        public Tuple<Point[], Brush> CrossMarkerTemplate(Object data)
        {
            //position is top left of the center 4 pixels
            //length is length from center in pixels
            //width is line width from line center 1 = 1 wide, 2 = 3 wide
            //brush is color
            CrossMarkerData cmData = (CrossMarkerData)data;
            //List<Point> pixelPositions = new List<Point>();
            Point[] pixelPositions = new Point[4 + (4 * cmData.length) + (cmData.length * (cmData.width - 1) * 8)];
            var pos = 0;
            for(int i = 0; i <= cmData.length; i++)
            {
                pixelPositions[pos++] = new Point(cmData.position.X - i    , cmData.position.Y - i);
                pixelPositions[pos++] = new Point(cmData.position.X + 1 + i, cmData.position.Y - i);
                pixelPositions[pos++] = new Point(cmData.position.X - i    , cmData.position.Y + 1 + i);
                pixelPositions[pos++] = new Point(cmData.position.X + 1 + i, cmData.position.Y + 1 + i);

                if (i == 0)
                {
                    continue;
                }

                for(int j = 1; j < cmData.width; j++)
                {
                    pixelPositions[pos++] = new Point(cmData.position.X - i + j    , cmData.position.Y - i);
                    pixelPositions[pos++] = new Point(cmData.position.X - i        , cmData.position.Y - i + j);

                    pixelPositions[pos++] = new Point(cmData.position.X + 1 + i - j, cmData.position.Y - i);
                    pixelPositions[pos++] = new Point(cmData.position.X + 1 + i    , cmData.position.Y - i + j);

                    pixelPositions[pos++] = new Point(cmData.position.X - i + j    , cmData.position.Y + 1 + i);
                    pixelPositions[pos++] = new Point(cmData.position.X - i        , cmData.position.Y + 1 + i - j);

                    pixelPositions[pos++] = new Point(cmData.position.X + 1 + i - j, cmData.position.Y + 1 + i);
                    pixelPositions[pos++] = new Point(cmData.position.X + 1 + i    , cmData.position.Y + 1 + i - j);
                }
            }

            return new Tuple<Point[], Brush> (pixelPositions, cmData.brush);
        }

        private void ListLinkBox_LostFocus(object sender, EventArgs e)
        {
            if (!shouldTrigger)
            {
                return;
            }
            int val = 0xFF;
            if (ListLinkBox.Text != "" && !NumberUtil.TryParse(ListLinkBox.Text, out val))
            {
                ListLinkBox.Text = currentRoom.MetaData.GetLinkFor(listKeys[listIndex]) + "";
            }
            currentRoom.MetaData.SetLinkFor(listKeys[listIndex], (byte)val);

            SetData();
        }
    }
}

public class EnumObject
{
    public string entryName { get; set; }
    public int entryId { get; set; }
}

public class CrossMarkerData
{
    public Brush brush = Brushes.Red;
    public Point position = new Point(0, 0);
    public int length = 8;
    public int width = 1;
}


