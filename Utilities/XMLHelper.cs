using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MinishMaker.Utilities.Rework
{
    public class XMLHelper
    {
        public static void GetIntAttribute(XmlNode entryNode, string attributeName, ref int setValue, int minNumber, string context, bool required = false)
        {
            var entry = GetEntry(entryNode, attributeName);

            if (entry != null)
            {
                int value;
                if (!NumberUtil.TryParse(entry.InnerText, out value))
                {
                    throw new XMLHelperException($"{attributeName} value {entry.InnerText} in {context} {entryNode.Name} is not a number");
                }
                if (value < minNumber)
                {
                    throw new XMLHelperException($"{attributeName} value {entry.InnerText} in {context} {entryNode.Name} is below the minimal number {minNumber}");
                }
                setValue = value;
                return;
            }

            if (required)
            {
                throw new XMLHelperException($"{attributeName} is required but missing in {context} {entryNode.Name}");
            }
        }

        public static void GetIntArrayAttribute(XmlNode entryNode, string attributeName, ref int[] setValue, int minNumber, string context, bool required = false)
        {
            var arrayEntry = entryNode[attributeName];
            if (arrayEntry != null)
            {
                var valueList = new List<int>();
                foreach (XmlNode entry in arrayEntry.ChildNodes)
                {
                    if (entryNode.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }

                    int value;
                    if (!NumberUtil.TryParse(entry.InnerText, out value))
                    {
                        throw new XMLHelperException($"{attributeName} array value {entry.InnerText} in {context} {entryNode.Name} is not a number");
                    }
                    if (value < minNumber)
                    {
                        throw new XMLHelperException($"{attributeName} array value {entry.InnerText} in {context} {entryNode.Name} is below the minimal number {minNumber}");
                    }
                    valueList.Add(value);
                }
                setValue = valueList.ToArray();
                return;
            }

            if (required)
            {
                throw new XMLHelperException($"{attributeName} is required but missing in {context} {entryNode.Name}");
            }
        }

        public static void GetEnumAttribute<T>(XmlNode entryNode, string attributeName, ref T setValue, string context, string enumContext, bool required = false) where T : struct, Enum
        {
            var entry = GetEntry(entryNode, attributeName);

            if (entry != null)
            {
                T value;
                if (!Enum.TryParse(entry.InnerText.ToUpper(), out value))
                {
                    throw new XMLHelperException($"{attributeName} value {entry.InnerText.ToUpper()} in {context} {entryNode.Name} is not a valid {enumContext}");
                }
                setValue = value;
                return;
            }

            if (required)
            {
                throw new XMLHelperException($"{attributeName} is required but missing in {context} {entryNode.Name}");
            }
        }

        public static void GetStringAttribute(XmlNode entryNode, string attributeName, ref string setValue, string context, bool required = false)
        {
            var entry = GetEntry(entryNode, attributeName);

            if (entry != null)
            {
                setValue = entry.InnerText;
                return;
            }

            if (required)
            {
                throw new XMLHelperException($"{attributeName} is required but missing in {context} {entryNode.Name}");
            }
        }

        public static void GetStringArrayAttribute(XmlNode entryNode, string attributeName, ref string[] setArray, string context, bool required = false)
        {
            var valueList = new List<string>();
            GetStringListAttribute(entryNode, attributeName, ref valueList, context, required);
            setArray = valueList.ToArray();
        }

        public static void GetStringListAttribute(XmlNode entryNode, string attributeName, ref List<string> setList, string context, bool required = false)
        {
            var entry = entryNode[attributeName];
            if (entry != null)
            {
                foreach (XmlNode elementName in entry.ChildNodes)
                {
                    if (elementName.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }

                    setList.Add(elementName.InnerText);
                }
                return;
            }

            if (required)
            {
                throw new XMLHelperException($"{attributeName} is required but missing in {context} {entryNode.Name}");
            }
        }

        private static XmlNode GetEntry(XmlNode entryNode, string attributeName)
        {
            XmlNode entry = null;
            if (entryNode.Attributes != null && entryNode.Attributes.Count > 0)
            {
                entry = entryNode.Attributes[attributeName];
            }

            if (entry == null)
            {
                entry = entryNode[attributeName];
            }
            return entry;
        }

        public class XMLHelperException : Exception
        {
            public XMLHelperException() { }
            public XMLHelperException(string message) : base(message) { }
            public XMLHelperException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
