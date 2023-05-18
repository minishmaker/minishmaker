using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using MinishMaker.Core;
using MinishMaker.Utilities.Rework;

namespace MinishMaker.Utilities
{
    public class ObjectDefinitionParser
    {
        private static Dictionary<string, Dictionary<int, string>> enums = new Dictionary<string, Dictionary<int, string>>();

        private static Filter baseFilter;
        public static Filter BaseFilter 
        { 
            get
            { 
                if(baseFilter == null)
                {
                    Setup();
                }
                return baseFilter; 
            }
        }

        private static Dictionary<string, Func<List<string>, Tuple<Object, Func<Object, Tuple<Point[], Brush>>>>> markerSetupFuncs = new Dictionary<string, Func<List<string>, Tuple<object, Func<object, Tuple<Point[], Brush>>>>>();

        public static void Setup()
        {
            baseFilter = null;
            enums.Clear();
            var filterTemplates = new Dictionary<string, FilterTemplate>();
            var filters = new Dictionary<string, Filter>();
            var elements = new Dictionary<string, FormElement>();
            var elementSets = new Dictionary<string, FormElement[]>();

            ReadAllXML(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6) + "/Resources/",
                        filterTemplates, filters,
                        elements, elementSets);

            if (Directory.Exists(Project.Instance.ProjectPath + "/UserObjectDefinitions"))
            {

                var userFilterTemplates = new Dictionary<string, FilterTemplate>();
                var userFilters = new Dictionary<string, Filter>();
                var userElements = new Dictionary<string, FormElement>();
                var userElementSets = new Dictionary<string, FormElement[]>();

                ReadAllXML(Project.Instance.ProjectPath + "/UserObjectDefinitions",
                           userFilterTemplates, userFilters,
                           userElements, userElementSets);

                TransferDictionaries(filterTemplates, userFilterTemplates);
                TransferDictionaries(filters, userFilters);
                TransferDictionaries(elements, userElements);
                TransferDictionaries(elementSets, userElementSets);
            }

            //setup extra childrenName, parent only set during search so it can bubble up the same way
            foreach (string name in filterTemplates.Keys)
            {
                var template = filterTemplates[name];
                if (template.parentNames.Length == 0)
                {
                    continue;
                }

                foreach (string parentName in template.parentNames)
                {
                    FilterTemplate parent;
                    if (!filterTemplates.TryGetValue(parentName, out parent))
                    {
                        throw new ParserException($"Unknown parent name {parentName} found in a filter with the name {name}");
                    }
                    parent.childrenNames.Add(name);
                }
            }

            ProcessValues(baseFilter, filterTemplates, filters, elements, elementSets, 0, false);
        }

        private static void ReadAllXML(string startPath, Dictionary<string, FilterTemplate> filterTemplates, Dictionary<string, Filter> filters, Dictionary<string, FormElement> elements, Dictionary<string, FormElement[]> elementSets)
        {
            var folders = new List<string>();
            folders.Add(startPath);
            var baseFilterAdded = false; //1 basefilter, and otherwise 1 overwritten basefilter

            while (folders.Count > 0)
            {
                var currentFolder = folders[0];
                folders.AddRange(Directory.GetDirectories(currentFolder));
                foreach (var file in Directory.GetFiles(currentFolder, "*.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(file);
                    }
                    catch (Exception ex)
                    {
                        throw new ParserException($"{file} is not valid XML file", ex);
                    }

                    var root = doc.GetElementsByTagName("root")[0];
                    if (!root.HasChildNodes)
                    {
                        continue;
                    }

                    foreach (XmlNode node in root)
                    {
                        if (node.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }

                        switch (node.Name.ToLower())
                        {
                            case "basefilter":
                                if (baseFilterAdded)
                                {
                                    throw new ParserException($"Attempt to define a second baseFilter in {file}");
                                }
                                baseFilter = ReadFilter(node, filterTemplates);
                                break;
                            case "filters":
                                ReadFilters(node, filterTemplates, filters);
                                break;
                            case "enums":
                                ReadEnums(node);
                                break;
                            case "elementsets":
                                ReadElementSets(node, elementSets);
                                break;
                            case "elements":
                                ReadElements(node, elements);
                                break;
                            default:
                                break;
                        }
                    }
                }
                folders.RemoveAt(0);
            }
        }

        private static void ProcessValues(Filter current, Dictionary<string, FilterTemplate> filterTemplates, Dictionary<string, Filter> filters, Dictionary<string, FormElement> elements, Dictionary<string, FormElement[]> elementSets, int depth, bool hasDataSize)
        {
            if (depth > 20) {
                throw new ParserException($"Exceeded maximum filter depth (20) currently inside ${current.name}");
            }


            hasDataSize |= current.dataSize != 0;

            if (current.children.Count() > 0 || current.elements.Length > 0)
            {
                if(!hasDataSize)
                {
                    CheckDataSize(current);
                }
                return; //already processed, must be a copy
            }

            var name = current.name;
            var template = filterTemplates[name];
            var defaultName = "";

            if (!hasDataSize && template.childrenNames.Count == 0 && template.childrenSetName == "")
            {
                throw new ParserException($"Filter chain ending with filter {current.name} is missing a dataSize");
            }

            if (current.targetType == ObjectValueType.PART)
            {
                throw new ParserException($"Part targetType found in filter {name}, this is currently only intended for elements and elementSets");
            }
            
            foreach (string childName in template.childrenNames)
            {
                string trimmedName = childName;
                Filter child;

                if (!filters.TryGetValue(childName, out child))
                {
                    throw new ParserException($"Unknown child filter name {childName} found in a filter with the name {name}");
                }

                ProcessValues(child, filterTemplates, filters, elements, elementSets, depth + 1, hasDataSize);

                current.children.Add(child);
                if (child.IsDefault())
                {
                    if (defaultName.Length > 0)
                    {
                        throw new ParserException($"Multiple default filters found, {defaultName} and {childName} ,in a filter with name {name}");
                    }
                    defaultName = childName;
                }

                if (current.defaultChildTargetType == ObjectValueType.BLANK && child.targetType == ObjectValueType.BLANK && !child.IsDefault())
                {
                    throw new ParserException($"No targetType found in child {childName} and also no defaultChildTargetType found in {name}, if this should be a default choice remove the targetValues");
                }

                if (current.defaultChildTargetType != ObjectValueType.LIST && current.defaultChildTargetPos == 0 && child.targetType != ObjectValueType.LIST && child.targetPos == 0 && !child.IsDefault())
                {
                    throw new ParserException($"No targetPos found in child {childName} and also no defaultChildTargetPos found in {name}, if this should be a default choice remove the targetValues");
                }

                if ((child.targetPos != 0 || child.targetType != ObjectValueType.BLANK) && child.IsDefault())
                {
                    throw new ParserException($"Missing targetValues in {childName} in {name}, if this should be a default choice remove the targetPos and targetType");
                }
            }

            var curPos = 0;
            if (template.elementSetName.Length != 0)
            {
                FormElement[] elementSet;
                if (!elementSets.TryGetValue(template.elementSetName, out elementSet))
                {
                    throw new ParserException($"Unknown elementSet name {template.elementSetName} found in a filter with the name {name}");
                }
                current.elements = new FormElement[elementSet.Length + template.elementNames.Length];
                foreach (FormElement element in elementSet)
                {
                    if (element.type == FormVisualType.ENUM && !enums.ContainsKey(element.enumType))
                    {
                        throw new ParserException($"Unknown enumType {element.enumType} found in an elementSet with the name {template.elementSetName} in an element named {element.name}");
                    }
                    current.elements[curPos] = element;
                    curPos++;
                }
            }
            else
            {
                current.elements = new FormElement[template.elementNames.Length];
            }

            foreach (string elementName in template.elementNames)
            {
                FormElement element;
                if (!elements.TryGetValue(elementName, out element))
                {
                    throw new ParserException($"Unknown element name {elementName} found in a filter with the name {name}");
                }

                if (element.type == FormVisualType.ENUM && !enums.ContainsKey(element.enumType))
                {
                    throw new ParserException($"Unknown enumType {element.enumType} found in an element named {element.name}");
                }
                current.elements[curPos] = element;
                curPos++;
            }
        }

        private static void CheckDataSize(Filter current)
        {
            if(current.dataSize != 0)
            {
                return;
            }

            if(current.children.Count == 0)
            {
                throw new ParserException($"Filter chain ending with {current.name} is missing a dataSize");
            }

            foreach (Filter child in current.children)
            {
                CheckDataSize(child);
            }
        }
     
        private static void ReadFilters(XmlNode filterNode, Dictionary<string, FilterTemplate> filterTemplates, Dictionary<string, Filter> filters)
        {
            if(!filterNode.HasChildNodes)
            {
                return;
            }

            foreach(XmlNode entryNode in filterNode)
            {
                if (entryNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                var filter = ReadFilter(entryNode, filterTemplates);
                filters.Add(entryNode.Name, filter);
            }
        }

        private static Filter ReadFilter(XmlNode entryNode, Dictionary<string, FilterTemplate> filterTemplates)
        {
            if (!entryNode.HasChildNodes)
            {
                throw new ParserException($"Filter with name {entryNode.Name} has no properties");
            }

            if(entryNode.Name.EndsWith("_"))
            {
                throw new ParserException($"Filter with name {entryNode.Name} ends with the reserved character _");
            }

            if (filterTemplates.ContainsKey(entryNode.Name))
            {
                throw new ParserException($"Filter with name {entryNode.Name} already exists");
            }

            FilterTemplate template = new FilterTemplate();
            Filter filter = new Filter();

            //defaultChildTargetPos - int
            XMLHelper.GetIntAttribute(entryNode, "defaultChildTargetPos", ref filter.defaultChildTargetPos, 1, "filter");
            //defaultChildTargetType - ValueType
            XMLHelper.GetEnumAttribute(entryNode, "defaultChildTargetType", ref filter.defaultChildTargetType, "filter", "value type");
            //targetPos - int
            XMLHelper.GetIntAttribute(entryNode, "targetPos", ref filter.targetPos, 1, "filter");
            //targetType - ValueType
            XMLHelper.GetEnumAttribute(entryNode, "targetType", ref filter.targetType, "filter", "value type");
            //targetValues - List<int>
            XMLHelper.GetIntArrayAttribute(entryNode, "targetValues", ref filter.targetValues, 0, "filter");
            //childrenSetName - String
            XMLHelper.GetStringAttribute(entryNode, "childrenSetName", ref template.childrenSetName, "filter");
            //childrenNames - List<string>
            XMLHelper.GetStringListAttribute(entryNode, "childrenNames", ref template.childrenNames, "filter");
            //elementSetName - string
            XMLHelper.GetStringAttribute(entryNode, "elementSetName", ref template.elementSetName, "filter");
            //elementNames - List<string>
            XMLHelper.GetStringArrayAttribute(entryNode, "elementNames", ref template.elementNames, "filter");
            //markers - Marker[]
            GetMarkerValues(entryNode, ref filter.markers, "filter");
            //dataSize - int
            XMLHelper.GetIntAttribute(entryNode, "dataSize", ref filter.dataSize, 1, "filter");
            //parentNames - List<string>
            XMLHelper.GetStringArrayAttribute(entryNode, "parentNames", ref template.parentNames, "filter");

            filterTemplates.Add(entryNode.Name, template);
            filter.name = entryNode.Name; //main use error handling during checks
            return filter;
        }

        private static void ReadEnums(XmlNode enumNode)
        {
            if (!enumNode.HasChildNodes)
            {
                return;
            }

            foreach (XmlNode entryNode in enumNode)
            {
                if (entryNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                Dictionary<int, string> enumEntry;
                if (enums.ContainsKey(entryNode.Name))
                {
                    enumEntry = enums[entryNode.Name];
                }
                else
                {
                    enumEntry = new Dictionary<int, string>();
                    enums.Add(entryNode.Name, enumEntry);
                }

                foreach (XmlNode entrySet in entryNode.ChildNodes)
                {
                    if (entrySet.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }

                    string entryName = "";
                    int entryValue = 0;
                    XMLHelper.GetStringAttribute(entrySet, "name", ref entryName, "enum", true);
                    XMLHelper.GetIntAttribute(entrySet, "value", ref entryValue, -1, "enum", true);
                    

                    if (enumEntry.ContainsKey(entryValue))
                    {
                        enumEntry[entryValue] = entryName;
                    }
                    else
                    {
                        enumEntry.Add(entryValue, entryName);
                    }
                }
            }
        }

        private static void ReadElements(XmlNode elementsNode, Dictionary<string, FormElement> elements)
        {
            if (!elementsNode.HasChildNodes)
            {
                return;
            }
            var elementsList = ReadElementsAsList(elementsNode);
            foreach(FormElement element in elementsList)
            {
                if(elements.ContainsKey(element.name))
                {
                    throw new ParserException($"Duplicate element name {element.name} found");
                }

                elements.Add(element.name, element);
            }
        }

        private static void ReadElementSets(XmlNode elementSetsNode, Dictionary<string, FormElement[]> elementSets)
        {
            if (!elementSetsNode.HasChildNodes)
            {
                return;
            }

            foreach (XmlNode elementSetNode in elementSetsNode)
            {
                if (elementSetNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                if (!elementSetNode.HasChildNodes)
                {
                    return;
                }

                if (elementSets.ContainsKey(elementSetNode.Name))
                {
                    throw new ParserException($"Duplicate element set name {elementSetNode.Name} found");
                }

                var elementsList = ReadElementsAsList(elementSetNode);
                elementSets.Add(elementSetNode.Name, elementsList.ToArray());
            }
        }

        private static List<FormElement> ReadElementsAsList(XmlNode elementsNode)
        {
            var elements = new List<FormElement>();
            foreach (XmlNode entryNode in elementsNode)
            {
                //entryNode.Name;
                if (entryNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                FormElement el = new FormElement();

                XMLHelper.GetEnumAttribute(entryNode, "type", ref el.type, "element", "type", true);

                if (el.type == FormVisualType.ENUM)
                {
                    XMLHelper.GetStringAttribute(entryNode, "enumType", ref el.enumType, "element", true);
                }

                XMLHelper.GetEnumAttribute(entryNode, "valueType", ref el.valueType, "element", "value type", el.type != FormVisualType.TEXT);
                XMLHelper.GetIntAttribute(entryNode, "valuePos", ref el.valuePos, 1, "element", el.type != FormVisualType.TEXT);
                XMLHelper.GetIntAttribute(entryNode, "valueLength", ref el.valueLength, 1, "element", el.valueType == ObjectValueType.PART);
                XMLHelper.GetStringAttribute(entryNode, "label", ref el.label, "element", el.type == FormVisualType.TEXT);
                XMLHelper.GetIntAttribute(entryNode, "colspan", ref el.colspan, 1, "element");
                XMLHelper.GetIntAttribute(entryNode, "column", ref el.column, 1, "element", true);
                XMLHelper.GetIntAttribute(entryNode, "row", ref el.row, 1, "element", true);

                el.name = entryNode.Name;
                elements.Add(el);
            }
            return elements;
        }
        private static void GetMarkerValues(XmlNode entryNode, ref Marker[] setArray, string context)
        {
            var entry = entryNode["markers"];
            var markers = new List<Marker>();
            if (entry != null)
            {
                foreach (XmlNode markerNode in entry.ChildNodes)
                {
                    if (markerNode.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }


                    var marker = new Marker();

                    marker.markerType = markerNode["type"].InnerText;
                    XMLHelper.GetStringListAttribute(markerNode, "data", ref marker.data, context);

                    markers.Add(marker);
                }
                setArray = markers.ToArray();
            }
        }

        public static Dictionary<int, string> GetEnum(string enumName)
        {
            return enums[enumName];
        }
        
        private static void TransferDictionaries<T>(Dictionary<string, T> to, Dictionary<string, T> from)
        {
            foreach (string key in from.Keys)
            {
                if (to.ContainsKey(key))
                {
                    to[key] = from[key];
                }
                else
                {
                    to.Add(key, from[key]);
                }
            }
        }

        public static int GetTargetValue(ObjectValueType targetType, int targetPos, int valueLength, List<byte> data )
        {
            var dataValue = 0;
            switch (targetType)
            {
                case ObjectValueType.BIT:
                    var bytePos = targetPos / 8;
                    var bitPos = targetPos & 0x7;
                    dataValue = (data[bytePos] >> bitPos) & 0x1;
                    break;
                case ObjectValueType.TABLEPOINTER:
                case ObjectValueType.INT:
                    dataValue += (data[targetPos + 3] << 24);
                    goto case ObjectValueType.TRI;
                case ObjectValueType.TRI:
                    dataValue += (data[targetPos + 2] << 16);
                    goto case ObjectValueType.SHORT;
                case ObjectValueType.SHORT:
                    dataValue += (data[targetPos + 1] << 8);
                    goto case ObjectValueType.BYTE;
                case ObjectValueType.BYTE:
                    dataValue += data[targetPos];
                    break;
                case ObjectValueType.PART:
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

        public static Filter FilterData(out int dataSize, List<byte> data, int currentList)
        {
            var currentFilter = BaseFilter;
            dataSize = -1;
            var depth = 1;
            while (currentFilter.children.Count != 0)
            {
                if (depth >= 20)
                {
                    throw new IndexOutOfRangeException($"Exceeded maximum depth while going through {currentFilter.name}");
                }
                var dctt = currentFilter.defaultChildTargetType;
                var dctp = currentFilter.defaultChildTargetPos - 1;

                Filter nextFilter = null;
                if (currentFilter.dataSize != -1)
                {
                    dataSize = currentFilter.dataSize;
                }

                int defaultDataValue = -1;
                if (dctt == ObjectValueType.LIST)
                {
                    defaultDataValue = currentList;
                }
                else if ( dctp != -1)
                {
                    defaultDataValue = GetTargetValue(dctt, dctp, 0, data);
                }

                foreach (var filter in currentFilter.children)
                {
                    if (filter.targetValues.Length == 0) //default
                    {
                        nextFilter = filter;
                        continue;
                    }

                    var tt = filter.targetType;
                    var tp = filter.targetPos - 1;

                    int dataValue = -1;
                    if (defaultDataValue != -1 && tt == ObjectValueType.BLANK && tp == -1)
                    {
                        dataValue = defaultDataValue;
                    }
                    else if (tt == ObjectValueType.LIST)
                    {
                        dataValue = currentList;
                    }
                    else if (tt == ObjectValueType.BLANK || tp == -1)
                    {
                        throw new MissingMemberException($"Missing either a defaultChildTargetType and pos in the parent filter or a targetType and pos the filter named {filter.name}");
                    }
                    else
                    {
                        dataValue = GetTargetValue(tt, tp, 0, data);
                    }

                    if (filter.HasTarget(dataValue))
                    {
                        nextFilter = filter;
                        break;
                    }
                }

                if (nextFilter == null)
                {
                    throw new MissingMemberException($"No matches were made and no default filter was found in the children of a filter with defaultTarget:{currentFilter.targetType} and defaultPos:{currentFilter.targetPos}");
                }

                nextFilter.parent = currentFilter; //for bubbling up
                currentFilter = nextFilter; //continue deeper
            }

            if (dataSize == -1)
            {
                throw new MissingMemberException($"Missing a dataSize in the filter chain ending on {currentFilter.name}");
            }
            return currentFilter;
        }

        public class FilterTemplate
        {
            public string childrenSetName = "";
            public List<string> childrenNames = new List<string>(); //added to later using parentNames

            //public bool overwriteElements = false;
            public string elementSetName = "";
            public string[] elementNames = new string[0];

            public string[] parentNames = new string[0];
        }

        public class Filter
        {
            public string name = "";

            public ObjectValueType defaultChildTargetType = ObjectValueType.BLANK;
            public int defaultChildTargetPos = 0;

            public ObjectValueType targetType = ObjectValueType.BLANK;
            public int targetPos = 0;

            public int[] targetValues = new int[0];

            public List<Filter> children = new List<Filter>();

            public FormElement[] elements = new FormElement[0];

            public Filter parent = null;
            public int dataSize = -1;
            public Marker[] markers = new Marker[0];

            public bool HasTarget(int value) 
            {
                if (IsDefault()) {
                    return false;
                }

                foreach (int targetValue in targetValues)
                {
                    if(value == targetValue)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool IsDefault()
            {
                return targetValues.Length == 0;
            }

            public Filter GetTarget()
            {
                return null;
            }
        }

        public class Marker
        {
            public string markerType = "";
            public List<string> data = new List<string>();
        }

        public static void AddSetupFunc(string name, Func<List<string>, Tuple<Object, Func<Object, Tuple<Point[], Brush>>>> setupFunc)
        {
            if (markerSetupFuncs.ContainsKey(name))
            {
                return;
            }
            markerSetupFuncs.Add(name, setupFunc);
        }


        public static Tuple<Object, Func<Object, Tuple<Point[], Brush>>> SetupMarker(string key, List<string> rawData)
        {
            Func<List<string>, Tuple<Object, Func<Object, Tuple<Point[], Brush>>>> setupFunc;
            if (!markerSetupFuncs.TryGetValue(key, out setupFunc))
            {
                throw new ArgumentException($"Marker type with the name {key} does not exist");
            }

            return setupFunc(rawData);
        }

        public enum ObjectValueType
        {
            BLANK = 0,
            BIT,
            BYTE,
            SHORT,
            TRI,
            INT,
            LIST,
            PART,
            TABLEPOINTER
        }

        public enum FormVisualType
        {
            BLANK = 0,
            ENUM,
            TEXT,
            BIT,
            NUMBER,
            HEXNUMBER
        }

        public class FormElement
        {
            public string name = "";
            public FormVisualType type = FormVisualType.BLANK;
            public string enumType = "";
            public ObjectValueType valueType = ObjectValueType.BLANK;
            public int valuePos = 0;
            public int valueLength = 0;
            public int colspan = 1;
            public string label = "";

            public int row = 0;
            public int column = 0;
        }

        public class ParserException : Exception
        {
            public ParserException() { }
            public ParserException(string message) : base(message) { }
            public ParserException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
