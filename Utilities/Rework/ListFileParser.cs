using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Hjson;

namespace MinishMaker.Utilities.Rework
{
    public class ListFileParser
    {
        private static Dictionary<string, Dictionary<int, string>> enums = new Dictionary<string, Dictionary<int, string>>();
        public static Filter topFilter;

        public static void Setup()
        {
            string filterJson = Hjson.HjsonValue.Load("testObjFile").ToString();
            JavaScriptSerializer jsonthingy = new JavaScriptSerializer();
            topFilter = jsonthingy.Deserialize<Filter>(filterJson);
            enums = jsonthingy.Deserialize <Dictionary<string, Dictionary<int, string>>>(File.ReadAllText("testEnumFile"));
            ParseFilter(topFilter);
        }

        private static void ParseFilter(Filter filter)
        {
            //children
            foreach (Filter childFilter in filter.children)
            {
                ParseFilter(childFilter);
            }

            //defaultTargetType
            if (filter.defaultTargetType.Length != 0)
            {
                if (!filter.defaultTargetType.Equals("list", StringComparison.InvariantCultureIgnoreCase) &&  //current list value
                    !filter.defaultTargetType.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&   // 1 bit
                    !filter.defaultTargetType.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&  // 1 byte
                    !filter.defaultTargetType.Equals("short", StringComparison.InvariantCultureIgnoreCase) && // 2 bytes
                    !filter.defaultTargetType.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&   // 3 bytes
                    !filter.defaultTargetType.Equals("int", StringComparison.InvariantCultureIgnoreCase))     // 4 bytes
                {
                    throw new FormatException($"{filter.defaultTargetType} is not a default filter target type, use list, bit, byte, short, tri or int");
                }

                if (filter.defaultTargetPos <= 0 && !filter.defaultTargetType.Equals("list", StringComparison.InvariantCultureIgnoreCase)) //pos not needed if list value
                {
                    throw new FormatException($"default target type is set but target position is not");
                }
            }

            //targetValues
            if (filter.targetValues.Length != 0)
            {
                var x = 0;
                try
                {
                    while (x < filter.targetValues.Length)
                    {
                        filter.parsedTargets[x] = ParseInt(filter.targetValues[x]);
                        x++;
                    }
                }
                catch
                {
                    throw new FormatException($"Could not convert target value {filter.targetValues[x]} to a number");
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
                        throw new FormatException($"{filter.targetType} is not a filter target type, use list, bit, byte, short, tri or int");
                    }

                    if (filter.parent.defaultTargetPos <= 0 && !filter.targetType.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"target type is set but target position is not");
                    }
                }
                else //if target is empty there must be a default on the entry above
                {
                    var parentDTT = filter.parent.defaultTargetType;
                    if (parentDTT.Length == 0)
                    {
                        throw new ArgumentNullException("Missing either a target type or a default target type in the parent");
                    }

                    if (!parentDTT.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("byte", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("short", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("tri", StringComparison.InvariantCultureIgnoreCase) &&
                        !parentDTT.Equals("int", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new FormatException($"{parentDTT} is not a type, use bit, byte, short, tri or int");
                    }

                    if (filter.parent.defaultTargetPos <= 0)
                    {
                        throw new FormatException($"default target type is set but default target position is not");
                    }
                }
            }

            //elements
            foreach (FormElement element in filter.elements)
            {
                if (element.collumn <= 0)
                {
                    throw new ArgumentException("The collumn for a formElement has to be specified at a value of 1 or higher.");
                }

                if (element.row <= 0)
                {
                    throw new ArgumentException("The row for a formElement has to be specified at a value of 1 or higher.");
                }

                if (!element.type.Equals("enum", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("bit", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("number", StringComparison.InvariantCultureIgnoreCase) &&
                    !element.type.Equals("hexnumber", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new FormatException($"{filter.targetType} is not a type, use bit, byte, short, tri or int");
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
                        throw new FormatException($"{element.valueType} is not a element value type, use bit, byte, short, tri, int or part");
                    }

                    if (element.valuePos <= 0)
                    {
                        throw new FormatException($"Element value position must be 1 or higher");
                    }

                    if (element.valueType.Equals("part", StringComparison.InvariantCultureIgnoreCase) && element.valueLength <= 1) //why not use a bit when its only 1 long?
                    {
                        throw new FormatException($"Element part value length must be 2 or higher");
                    }
                }

                if (element.type.Equals("text", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(element.label == "")
                    {
                        throw new ArgumentException("A text element needs a label with text");
                    }
                }

                if (element.type.Equals("enum", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (element.enumType == "")
                    {
                        throw new FormatException("An enum form element requires an enumType");
                    }

                    if (!enums.ContainsKey(element.enumType))
                    {
                        throw new ArgumentException($"{element.enumType} was not found in the enum file");
                    }
                }

                if (element.valueType.Equals("part", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(element.valueLength <= 0)
                    {
                        throw new ArgumentException("A valueLength is required for part values");
                    }
                }
            }
        }

        public static Dictionary<int,string> GetEnum(string enumName)
        {
            return enums[enumName];
        }

        private static int ParseInt(string numberString)
        {
            if (numberString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                return int.Parse(numberString.Substring(2), NumberStyles.HexNumber);
            }
            else
            {
                return int.Parse(numberString);
            }
        }

        public class Filter
        {
            public string defaultTargetType = "";
            public int defaultTargetPos = 0;

            public string targetType = "";
            public int targetPos = 0;

            public string[] targetValues = new string[0];
            public int[] parsedTargets = new int[0];

            public Filter[] children =  new Filter[0];
            public FormElement[] elements = new FormElement[0];

            public Filter parent = null;
        }

        public class FormElement
        {
            public string type;
            public string enumType = "";
            public string valueType;
            public int valuePos = 0;
            public int valueLength = 0;
            public string label = "";

            public int row = 0;
            public int collumn = 0;
        }
    }
}
