using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using MinishMaker.Core.Rework;

namespace MinishMaker.Utilities.Rework
{
    public class ObjectDefinitionParser
    {
        private static Dictionary<string, Dictionary<int, string>> enums = new Dictionary<string, Dictionary<int, string>>();
        public static Filter topFilter;
        private static ListSettings baseSettings;

        public static void Setup()
        {
            baseSettings = new ListSettings();


            List<string> folders = new List<string>();
            ReadAllXML((Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "//Resources//");
            if (Directory.Exists(Project.Instance.ProjectPath + "\\ObjectDefinitions"))
            {
                ReadAllXML(Project.Instance.ProjectPath + "\\ObjectDefinitions");
            }

        }

        private static void ConvertEnums()
        {
            foreach (var typeKey in baseSettings.enums.Keys)
            {
                var dict = new Dictionary<int, string>();
                foreach (var key in baseSettings.enums[typeKey].Keys)
                {
                    int intKey = 0;
                    var success = NumberUtil.ParseInt(key, ref intKey);
                    var value = baseSettings.enums[typeKey][key];
                    dict.Add(intKey, value);
                }
                enums.Add(typeKey, dict);
            }
        }

        private static void ReadAllXML(string startPath)
        {
            var folders = new List<string>();
            folders.Add(startPath);
            var filterDeserialiser = new XmlSerializer(typeof(Filter));
            var enumDeserialiser = new XmlSerializer(typeof(EnumEntry));
            while (folders.Count > 0)
            {
                var currentFolder = folders[0];
                folders.AddRange(Directory.GetDirectories(currentFolder));
                foreach (var file in Directory.GetFiles(currentFolder, "*.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    var nodes = doc.SelectNodes("/filters");
                    if (nodes.Count != 0)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            using (XmlNodeReader reader = new XmlNodeReader(node))
                            {
                                Filter filter = (Filter)filterDeserialiser.Deserialize(reader);
                                if (filter.name == "baseFilter")
                                {
                                    baseSettings.baseFilter = filter;
                                }
                                else
                                {
                                    if (baseSettings.filters.ContainsKey(filter.name))
                                    {
                                        baseSettings.filters[filter.name] = filter;
                                    }
                                    else
                                    {
                                        baseSettings.filters.Add(filter.name, filter);
                                    }   
                                }
                            }
                        }
                    }

                    nodes = doc.SelectNodes("/enums");
                    if (nodes.Count != 0)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            var enumNodes = node.SelectNodes("/*");
                            var enumSetName = node.Name;
                            if(!enums.ContainsKey(enumSetName)) {
                                enums.Add(enumSetName, new Dictionary<int, string>());
                            }
                            var enumSet = enums[enumSetName];

                            foreach(XmlNode enumNode in enumNodes)
                            {
                                using (XmlNodeReader reader = new XmlNodeReader(node))
                                {
                                    EnumEntry enumObject = (EnumEntry)enumDeserialiser.Deserialize(reader);
                                    if(enumSet.ContainsKey(enumObject.value)) {
                                        enumSet[enumObject.value] = enumObject.name;
                                    } else {
                                        enumSet.Add(enumObject.value, enumObject.name);
                                    }
                                }
                            }
                        }
                    }


                }
                folders.RemoveAt(0);
            }
        }

        private static void ReadNodeSet<T>(Dictionary<string, Nameable> list, XmlSerializer deserializer, string nodeName, XmlDocument doc) where T : Nameable
        {
            var nodes = doc.SelectNodes("/filter");
            if (nodes.Count != 0)
            {
                foreach (XmlNode node in nodes)
                {
                    using (XmlNodeReader reader = new XmlNodeReader(node))
                    {
                        T typeObject = (T)deserializer.Deserialize(reader);

                        if (list.ContainsKey(typeObject.name))
                        {
                            list[typeObject.name] = typeObject;
                        }
                        else
                        {
                            list.Add(typeObject.name, typeObject);
                        }
                    }
                }
            }
        }

        private static void ParseFilter(Filter filter)
        {
            //children
            var childTotal = filter.children.Count();
            var chId = childTotal;
            childTotal += filter.childrenNames.Count();
            if (baseSettings.filterSets.ContainsKey(filter.childrenSetName))
            {
                childTotal = baseSettings.filterSets[filter.childrenSetName].Count();
            }

            var newFilterArr = new Filter[childTotal];
            filter.children.CopyTo(newFilterArr, 0);

            foreach (string childString in filter.childrenNames)
            {
                if (!baseSettings.filters.ContainsKey(childString))
                {
                    throw new ArgumentException($"{filter.name} : No filter with the name {childString} exists within filters");
                }

                newFilterArr[chId] = baseSettings.filters[childString];
                chId++;
            }

            if (filter.childrenSetName != "")
            {
                if (!baseSettings.filterSets.ContainsKey(filter.childrenSetName))
                {
                    throw new ArgumentException($"{filter.name} : No filter set with the name {filter.childrenSetName} exists within filterSets");
                }

                var filters = baseSettings.filterSets[filter.childrenSetName];
                foreach (var setFilter in filters)
                {
                    newFilterArr[chId] = setFilter;
                    chId++;
                }
            }

            filter.children = newFilterArr;

            foreach (Filter childFilter in filter.children)
            {
                if (childFilter.dataSize == -1 && filter.dataSize != -1)
                {
                    childFilter.dataSize = filter.dataSize;
                }

                childFilter.parent = filter;
                ParseFilter(childFilter);
            }

            if (filter.dataSize == -1 && filter.children.Count() == 0)
            {
                string name = baseSettings.filters.FirstOrDefault(x => x.Value == filter).Key;
                throw new ArgumentException($"{name} : dataSize is not set and no dataSize was inherited from a parent filter.");
            }



            //defaultChildTargetType
            if (filter.defaultChildTargetType.Length != 0)
            {
                if (!filter.defaultChildTargetType.Equals("list", StringComparison.InvariantCultureIgnoreCase) &&  //current list value
                    !filter.defaultChildTargetType.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&   // 1 bit
                    !filter.defaultChildTargetType.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&  // 1 byte
                    !filter.defaultChildTargetType.Equals("short", StringComparison.InvariantCultureIgnoreCase) && // 2 bytes
                    !filter.defaultChildTargetType.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&   // 3 bytes
                    !filter.defaultChildTargetType.Equals("int", StringComparison.InvariantCultureIgnoreCase))     // 4 bytes
                {
                    throw new FormatException($"{filter.name} : {filter.defaultChildTargetType} is not a default filter target type, use list, bit, byte, short, tri or int.");
                }

                if (filter.defaultChildTargetPos <= 0 && !filter.defaultChildTargetType.Equals("list", StringComparison.InvariantCultureIgnoreCase)) //pos not needed if list value
                {
                    throw new FormatException($"{filter.name} : Default target type is set but target position is not.");
                }
            }

            //targetValues
            if (filter.targetValues.Length != 0)
            {
                filter.parsedTargets = new int[filter.targetValues.Length];
                var x = 0;
                try
                {
                    while (x < filter.targetValues.Length)
                    {
                        var success = NumberUtil.ParseInt(filter.targetValues[x], ref filter.parsedTargets[x]);
                        x++;
                    }
                }
                catch
                {
                    throw new FormatException($"{filter.name} : Could not convert target value {filter.targetValues[x]} to a number.");
                }
            }

            //targetType
            if (filter.parsedTargets.Length != 0) //if empty act as default after matching fails
            {
                if (filter.targetType.Length != 0)
                {
                    if (!filter.targetType.Equals("list", StringComparison.InvariantCultureIgnoreCase) &&
                        !filter.targetType.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                        !filter.targetType.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&
                        !filter.targetType.Equals("short", StringComparison.InvariantCultureIgnoreCase) &&
                        !filter.targetType.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&
                        !filter.targetType.Equals("int", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{filter.name} : {filter.targetType} is not a filter target type, use list, bit, byte, short, tri or int.");
                    }

                    if (filter.parent.defaultChildTargetPos <= 0 && !filter.targetType.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{filter.name} : Target type is set but target position is not.");
                    }
                }
                else //if target is empty there must be a default on the entry above
                {
                    var parentDTT = filter.parent.defaultChildTargetType;
                    if (parentDTT.Length == 0)
                    {
                        throw new ArgumentNullException($"{filter.name} : Missing either a target type or a default target type in the parent");
                    }

                    if (!parentDTT.Equals("list", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("short", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("int", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{filter.name} : {parentDTT} is not a type, use list, bit, byte, short, tri or int");
                    }

                    if (filter.parent.defaultChildTargetPos <= 0 && !parentDTT.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{filter.name} : Default target type is set but default target position is not");
                    }
                }
            }

            //elements
            var elementTotal = filter.elements.Count();
            var elId = elementTotal;
            elementTotal += filter.elementNames.Count();
            if (baseSettings.elementSets.ContainsKey(filter.elementSetName))
            {
                elementTotal += baseSettings.elementSets[filter.elementSetName].Count();
            }

            var newElementArr = new FormElement[elementTotal];
            filter.elements.CopyTo(newElementArr, 0);

            foreach (string elementString in filter.elementNames)
            {
                if (!baseSettings.elements.ContainsKey(elementString))
                {
                    throw new ArgumentException($"{filter.name} : No element with the name {elementString} exists within elements");
                }

                newElementArr[elId] = baseSettings.elements[elementString];
                elId++;
            }

            if (filter.elementSetName != "")
            {
                if (!baseSettings.elementSets.ContainsKey(filter.elementSetName))
                {
                    throw new ArgumentException($"{filter.name} : No element set with the name {filter.elementSetName} exists within elementSets");
                }

                var elements = baseSettings.elementSets[filter.elementSetName];
                foreach (var setElement in elements)
                {
                    newElementArr[elId] = setElement;
                    elId++;
                }
            }

            filter.elements = newElementArr;

            foreach (FormElement element in filter.elements)
            {
                if (element.column <= 0)
                {
                    throw new ArgumentException($"{filter.name} : The column for a formElement has to be specified at a value of 1 or higher.");
                }

                if (element.row <= 0)
                {
                    throw new ArgumentException($"{filter.name} : The row for a formElement has to be specified at a value of 1 or higher.");
                }

                if (!element.type.Equals("enum", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("number", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("hexnumber", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new FormatException($"{filter.name} : {element.type} is not an element type, use enum, text, bit, number or hexnumber");
                }

                if (!element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase)) //text doesnt require a value, just a label
                {
                    if (!element.type.Equals("bit", StringComparison.InvariantCultureIgnoreCase) && //ignore valueType on bit element, pos is still needed
                        !element.valueType.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                        !element.valueType.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&
                        !element.valueType.Equals("short", StringComparison.InvariantCultureIgnoreCase) &&
                        !element.valueType.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&
                        !element.valueType.Equals("int", StringComparison.InvariantCultureIgnoreCase) &&
                        !element.valueType.Equals("part", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{filter.name} : {element.valueType} is not an element value type, use bit, byte, short, tri, int or part");
                    }

                    if (element.valuePos <= 0)
                    {
                        throw new FormatException($"{filter.name} : Element value position must be 1 or higher");
                    }

                    if (element.valueType.Equals("part", StringComparison.InvariantCultureIgnoreCase) && element.valueLength <= 1) //why not use a bit when its only 1 long?
                    {
                        throw new FormatException($"{filter.name} : Element part value length must be 2 or higher");
                    }
                }

                if (element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (element.label == "")
                    {
                        throw new ArgumentException($"{filter.name} : A text element needs a label with text");
                    }
                }

                if (element.type.Equals("enum", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (element.enumType == "")
                    {
                        throw new FormatException($"{filter.name} : An enum form element requires an enumType");
                    }

                    if (!enums.ContainsKey(element.enumType))
                    {
                        throw new ArgumentException($"{filter.name} : {element.enumType} was not found in the enum file");
                    }
                }

                if (!element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase) && element.valueType.Equals("part", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (element.valueLength <= 0)
                    {
                        throw new ArgumentException($"{filter.name} : A valueLength is required for part values");
                    }
                }
            }
        }

        public static Dictionary<int, string> GetEnum(string enumName)
        {
            return enums[enumName];
        }

        public class ListSettings
        {
            public Filter baseFilter = null;
            public Dictionary<string, Filter> filters;
            public Dictionary<string, Filter[]> filterSets;
            public Dictionary<string, FormElement> elements;
            public Dictionary<string, FormElement[]> elementSets;
            public Dictionary<string, Dictionary<string, string>> enums;
        }

        public static Tuple<List<Filter>, int> FilterData(byte[] dataOriginal, int startIndex, int listIndex)
        {
            List<Filter> filters = new List<Filter>();
            var data = dataOriginal.Skip(startIndex).Take(0x10).ToArray();
            if (ListFileParser.topFilter == null)
            {
                ListFileParser.Setup();
            }

            var currentFilter = ObjectDefinitionParser.topFilter;
            filters.Add(currentFilter);

            while (currentFilter.children.Length != 0)
            {
                var dtt = currentFilter.defaultChildTargetType;
                var dtp = currentFilter.defaultChildTargetPos - 1;
                Filter nextFilter = null;

                int defaultDataValue = -1;
                if (dtt.ToLower() == "list" || dtp != -1)
                {
                    defaultDataValue = GetTargetValue(data, listIndex, dtt, dtp, 0);
                }

                foreach (var filter in currentFilter.children)
                {
                    var tt = filter.targetType;
                    var tp = filter.targetPos - 1;

                    if (filter.targetValues.Length == 0 && nextFilter == null)//default if nothing else is found
                    {
                        nextFilter = filter;
                        continue;
                    }
                    int dataValue = 0;
                    if (defaultDataValue != -1 && tt.ToLower() != "list" && tp == -1) //no type and pos set so use default target type and pos
                    {
                        dataValue = defaultDataValue;
                    }
                    else
                    {
                        dataValue = GetTargetValue(data, listIndex, tt, tp, 0);
                    }

                    bool found = false;
                    foreach (var targetVal in filter.targetValues)
                    {
                        var outValue = 0;
                        var success = NumberUtil.ParseInt(targetVal, ref outValue);
                        if (!success)
                        {
                            throw new FormatException("This should never happen as it is pre-validated.");
                        }

                        if (outValue == dataValue)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        nextFilter = filter;
                        break;
                    }

                }

                if (nextFilter == null)
                {
                    throw new MissingMemberException($"No matches were made and no default filter was found in the children of a filter with defaultTarget:{currentFilter.targetType} and defaultPos:{currentFilter.targetPos}");
                }

                currentFilter = nextFilter;
                filters.Add(nextFilter);
            }

            return new Tuple<List<Filter>, int>(filters, currentFilter.dataSize);
        }

        public static int GetTargetValue(byte[] data, int currentListNumber, string targetType, int targetPos, int valueLength)
        {
            var dataValue = 0;
            switch (targetType.ToLower())
            {
                case "bit":
                    var bytePos = targetPos / 8;
                    var bitPos = targetPos & 0x7;
                    dataValue = (data[bytePos] >> bitPos) & 0x1;
                    break;
                case "list":
                    dataValue = currentListNumber;
                    break;
                case "int":
                    dataValue += (data[targetPos + 3] << 24);
                    goto case "tri";
                case "tri":
                    dataValue += (data[targetPos + 2] << 16);
                    goto case "short";
                case "short":
                    dataValue += (data[targetPos + 1] << 8);
                    goto case "byte";
                case "byte":
                    dataValue += data[targetPos];
                    break;
                case "part":
                    int startBytePos = targetPos / 8;
                    var startBitPos = targetPos & 0x7; //0-7
                    //                                  0-7           V + 1 so its 1-8 per byte instead of 0-7
                    int byteCount = (int)Math.Ceiling((startBitPos + valueLength + 1) / 8d); //ceil(0/8) = 0, ceil(1/8) =1 ceil(8/8)=1

                    for (int i = 0; i < byteCount; i++)
                    {
                        var val = data[i + startBytePos] << (i * 8);
                        dataValue += val;
                    }

                    var mask = 0;
                    for (int i = 0; i < valueLength; i++)
                    { //create a mask for valueLength - 1 bits
                        mask += (int)Math.Pow(2, i);
                    }

                    dataValue = (dataValue >> startBitPos);
                    dataValue = dataValue & mask; //shave off the first 0-7 bits, then apply mask to get the value

                    break;
                default:
                    throw new ArgumentException($"Unknown targetType ({targetType}). How did you get here? this should have been checked already.");
            }
            return dataValue;
        }

        public class Filter:Nameable
        {
            public string defaultChildTargetType = "";
            public int defaultChildTargetPos = 0;

            public string targetType = "";
            public int targetPos = 0;

            public string[] targetValues = new string[0];
            public int[] parsedTargets = new int[0];

            public Filter[] children = new Filter[0];
            public string childrenSetName = "";
            public string[] childrenNames = new string[0];

            //public bool overwriteElements = false;
            public FormElement[] elements = new FormElement[0];
            public string elementSetName = "";
            public string[] elementNames = new string[0];

            public string parentName = "";
            public Filter parent = null;
            public int dataSize = -1;
        }


        public class FormElement:Nameable
        {
            public string type;
            public string enumType = "";
            public string valueType;
            public int valuePos = 0;
            public int valueLength = 0;
            public string label = "";

            public int row = 0;
            public int column = 0;
        }

        public class EnumEntry
        {
            public string name = "";
            public int value = -1;
        }

        public class Nameable
        {
            public string name = "";
        }
    }
}
