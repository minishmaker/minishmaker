using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MinishMaker.Utilities;
using MinishMaker.Utilities.Rework;

namespace MinishMaker.Core
{
    public class LanguageManager
    {
        private static LanguageManager _Instance;

        private List<List<List<string>>> textEntries;
        public int LangCount {get; private set;}

        private static Dictionary<SetKey, List<SetData>> dataToStringSets = new Dictionary<SetKey, List<SetData>>();
        private static Dictionary<string, byte[]> stringToDataSets = new Dictionary<string, byte[]>();
        private static Dictionary<string, Tuple<byte, Dictionary<string, byte>>> commandToDataSets = new Dictionary<string, Tuple<byte, Dictionary<string, byte>>>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<string, Dictionary<string, byte[]>> modeToDataSets = new Dictionary<string, Dictionary<string, byte[]>>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<RegionVersion, Dictionary<string, byte[]>> regionToDataSets = new Dictionary<RegionVersion, Dictionary<string, byte[]>>();
        private static Dictionary<short, short> dataRedirects = new Dictionary<short, short>();

        private static int bankCount = 0;
        private static Dictionary<int, Dictionary<int, List<string>>> changedEntries = new Dictionary<int, Dictionary<int, List<string>>>();

        private static Dictionary<int, int> bankLengths = new Dictionary<int, int>();
        public enum TranslationType {
            ANY,
            PHONETIC,
            KATAKANA,
            HIRAGANA
        }

        private struct SetKey
        {
            public TranslationType config;
            public byte setValue;
            
            public SetKey(TranslationType config, byte setValue)
            {
                this.setValue = setValue;
                this.config = config;
            }
        }

        private struct SetData
        {
            public string mode;
            public string command;
            public int length;
            public RegionVersion region;
            public Dictionary<byte, string> entries;

            public SetData(string mode, string command, int length, RegionVersion region)
            {
                this.mode = mode;
                this.command = command;
                this.length = length;
                this.region = region;
                this.entries = new Dictionary<byte, string>();
            }
        }

        public static LanguageManager Get()
        {
            if (_Instance == null)
            {
                _Instance = new LanguageManager();
            }
            return _Instance;
        }

        private LanguageManager()
        {
            Reset();
        }

        public void Reset()
        {
            dataToStringSets.Clear();
            stringToDataSets.Clear();
            commandToDataSets.Clear();
            changedEntries.Clear();
            LoadConversions();
            textEntries = new List<List<List<string>>>();

            var languageStart = ROM.Instance.headers.languageTableLoc;
            var r = ROM.Instance.reader;
            var languageBankTableLoc = r.ReadAddr(languageStart);
            bankCount = r.ReadInt(languageBankTableLoc) / 4;

            var langId = 0;
            var validLBT = true;
            while (true)
            {
                var langTable = r.ReadAddr(languageStart + (langId * 4));
                r.SetPosition(r.Position - 1);
                validLBT = r.ReadByte() == 8;

                if (!validLBT)
                {
                    LangCount = langId;
                    break;
                }
                langId++;
            }

        }

        private void LoadConversions() {
            //if empty, load from xml
            textEntries = new List<List<List<string>>>();
            if (dataToStringSets.Count == 0)
            {
                XmlDocument doc = new XmlDocument();
                var file = Project.Instance.ProjectPath + "\\TextConvert.xml";
                try
                {
                    doc.Load(file);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"{file} is not valid XML file", ex);
                }
                var root = doc.GetElementsByTagName("root")[0];
                foreach (XmlNode set in root)
                {
                    if (set.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }

                    int setValue = 0;
                    XMLHelper.GetIntAttribute(set, "value", ref setValue, 0, "set", true);

                    var command = ""; //used when writing to game
                    var mode = ""; //used when writing to game
                    var region = RegionVersion.None;    //used for seprate Japanese and other 0x00 table.

                    var length = 0; //used when writing to and reading from game
                    var config = TranslationType.ANY; //used when reading from game

                    XMLHelper.GetEnumAttribute(set, "config", ref config, $"set {setValue}", "TranslationType");
                    XMLHelper.GetStringAttribute(set, "command", ref command, $"{config} set {setValue}");
                    XMLHelper.GetStringAttribute(set, "mode", ref mode, $"{config} set {setValue}");
                    XMLHelper.GetEnumAttribute(set, "region", ref region, $"set {setValue}", "RegionVersion");
                    XMLHelper.GetIntAttribute(set, "length", ref length, 1, $"{config} set {setValue}");
                    var setKey = new SetKey(config, (byte)setValue);

                    List<SetData> targetSetList;
                    if (dataToStringSets.ContainsKey(setKey)) {
                        targetSetList = dataToStringSets[setKey];
                    }
                    else
                    {
                        targetSetList = new List<SetData>();
                    }
                    var setData = new SetData(mode, command, length, region);
                    targetSetList.Add(setData);

                    if (command != "" && !commandToDataSets.ContainsKey(command))
                    {
                        commandToDataSets.Add(command, new Tuple<byte, Dictionary<string, byte>>((byte)setValue, new Dictionary<string, byte>()));
                    }

                    if (mode != "" && !modeToDataSets.ContainsKey(mode))
                    {
                        modeToDataSets.Add(mode, new Dictionary<string, byte[]>());
                    }

                    if (region != RegionVersion.None && !regionToDataSets.ContainsKey(region))
                    {
                        regionToDataSets.Add(region, new Dictionary<string, byte[]>());
                    }


                    foreach (XmlNode value in set)
                    {
                        if (value.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        int dataValue = 0;
                        string replacement = "";
                        int redirect = -1;
                        XMLHelper.GetIntAttribute(value, "data", ref dataValue, 0, $"{config} {setValue.Hex(2)} entry", true);
                        XMLHelper.GetStringAttribute(value, "value", ref replacement, $"{config} {setValue.Hex(2)} {dataValue} entry", true);
                        XMLHelper.GetIntAttribute(value, "redirect", ref redirect, 0, $"{config} {setValue.Hex(2)} {dataValue} entry");
                        if (redirect != -1) //specifically added because of duplicate ♪
                        {
                            /*TODO: This is a hack, should use a better way to handle redirects.*/
                            if (dataRedirects.ContainsKey((short)(setValue * 0x100 + dataValue)))
                                continue;
                            dataRedirects.Add((short)(setValue * 0x100 + dataValue), (short)redirect);
                            continue;
                        }
                        if (replacement.Length == 0 && command == "") //ignore empty values if no command is set
                        {
                            continue;
                        }
                        setData.entries.Add((byte)dataValue, replacement);
                        if (command != "")
                        {
                            var dict = commandToDataSets[command].Item2;
                            if (dict.ContainsKey(replacement) && dict[replacement] != (byte)dataValue)
                            {
                                throw new ArgumentException($"{replacement} already exists in commandset {command} with the value 0x{dict[replacement]}, trying to add 0x{dataValue.Hex(2)}");
                            }
                            if (!dict.ContainsKey(replacement))
                            {
                                commandToDataSets[command].Item2.Add(replacement, (byte)dataValue);
                            }
                        }
                        else if (mode != "")
                        {
                            var dict = modeToDataSets[mode];
                            if (dict.ContainsKey(replacement) && dict[replacement][1] != (byte)dataValue)
                            {
                                throw new ArgumentException($"{replacement} already exists in modeset {mode} with the value 0x{dict[replacement][1].Hex(2)}, trying to add 0x{dataValue.Hex(2)}");
                            }
                            if (!dict.ContainsKey(replacement))
                            {
                                modeToDataSets[mode].Add(replacement, new byte[] { (byte)setValue, (byte)dataValue });
                            }
                        }
                        else if (region != RegionVersion.None)
                        {
                            var dict = regionToDataSets[region];
                            if (dict.ContainsKey(replacement) && dict[replacement][1] != (byte)dataValue)
                            {
                                throw new ArgumentException($"{replacement} already exists in modeset {region} with the value 0x{dict[replacement][1].Hex(2)}, trying to add 0x{dataValue.Hex(2)}");
                            }
                            if (!dict.ContainsKey(replacement))
                            {
                                regionToDataSets[region].Add(replacement, new byte[] { (byte)setValue, (byte)dataValue });
                            }
                        }
                        else
                        {
                            stringToDataSets.Add(replacement, new byte[] { (byte)setValue, (byte)dataValue });
                        }
                    }
                    dataToStringSets[setKey] = targetSetList;
                }
            }
        }

        public void LoadEntry(int bankNum, int entryId)
        {
            if (textEntries.Count > bankNum && textEntries[bankNum].Count > entryId && textEntries[bankNum][entryId].Count != 0)
            {
                return;
            }

            //if we at some point allow you to skip over entries or banks, dont break
            while (textEntries.Count() <= bankNum)
            {
                textEntries.Add(new List<List<string>>());
            }

            if (textEntries[bankNum].Count() == 0)
            {
                LoadFromJSON(bankNum);
            }

            while (textEntries[bankNum].Count() <= entryId)
            {
                textEntries[bankNum].Add(new List<string>());
            }

            if (changedEntries.ContainsKey(bankNum) && changedEntries[bankNum].ContainsKey(entryId))
            {
                textEntries[bankNum][entryId] = changedEntries[bankNum][entryId];
                return;
            }

            textEntries[bankNum][entryId] = ChangeToReadable( bankNum, entryId);
        }


        private List<string> ChangeToReadable(int bankNum, int entryId)
        {
            StringBuilder sbDebug = new StringBuilder();
            List<string> readable = new List<string>();
            var languageStart = ROM.Instance.headers.languageTableLoc;
            var r = ROM.Instance.reader;

            SetKey anyKey = new SetKey(TranslationType.ANY, 0x00);
            SetKey configKey = new SetKey(TranslationType.PHONETIC, 0x00); //TODO: read from project file


            var readAll = entryId == -1;
            entryId = entryId == -1 ? 0 : entryId;

            for (int langId = 0; langId < LangCount; langId++)
            {
                var languageBankTableLoc = r.ReadAddr(languageStart + (langId * 4));

                var bankOffset = r.ReadInt(languageBankTableLoc + (bankNum * 4));

                var bankLoc = languageBankTableLoc + bankOffset;

                var firstEntryOffset = 0;

                var firstDebug = true;

                do
                {
                    var entryOffset = r.ReadInt(bankLoc + (entryId * 4));
                    var entryLoc = bankLoc + entryOffset;
                    if (firstEntryOffset == 0)
                    {
                        firstEntryOffset = entryOffset;
                    }

                    r.SetPosition(entryLoc);
                    StringBuilder sb = new StringBuilder();
                    byte curValue = r.ReadByte();
                    string mode = "";
                    var endReached = curValue == 0x00;

                    while (!endReached)
                    {
                        if (curValue == 0x0A)
                        {
                            sb.Append(Environment.NewLine);
                        }
                        else
                        {
                            if (curValue >= 0x10)
                            {
                                r.SetPosition(r.Position - 1);
                                configKey.setValue = 0x00;
                                anyKey.setValue = 0x00;
                                curValue = 0x00;
                            }
                            else
                            {
                                configKey.setValue = curValue;
                                anyKey.setValue = curValue;
                            }

                            var nextValue = r.ReadByte();
                            if (dataRedirects.ContainsKey((short)(curValue * 0x100 + nextValue)))
                            {
                                var newVal = dataRedirects[(short)(curValue * 0x100 + nextValue)];
                                var highByte = (byte)(newVal >> 8);
                                configKey.setValue = highByte;
                                anyKey.setValue = highByte;
                                nextValue = (byte)(newVal & 0xFF);
                            }

                            List<SetData> configSets = null;
                            List<SetData> anySets = null;

                            if (dataToStringSets.ContainsKey(configKey))
                            {
                                configSets = dataToStringSets[configKey];
                            }
                            if (dataToStringSets.ContainsKey(anyKey))
                            {
                                anySets = dataToStringSets[anyKey];
                            }

                            var found = false;
                            if (configSets != null)
                            {
                                found = CheckSets(configSets, nextValue, r, mode, ref sb);
                            }

                            if (anySets != null && !found)
                            {
                                found = CheckSets(anySets, nextValue, r, mode, ref sb);
                            }

                            if (!found)
                            {
                                if (firstDebug)
                                {
                                    sbDebug.Append($"Language {langId} - Bank 0x{bankNum.Hex(2)} - Entry 0x{entryId.Hex(2)} |");
                                    firstDebug = false;
                                }
                                if (curValue == 0x00)
                                {
                                    sbDebug.Append($"{sb.Length.ToString().PadLeft(3, '0')}: 0x{nextValue.Hex(2)} |");
                                    sb.Append($"{{Raw:{nextValue.Hex(2)}}}");

                                }
                                else
                                {
                                    sb.Append($"{{Raw:{curValue.Hex(2)}:{nextValue.Hex(2)}}}");
                                    sbDebug.Append($"{sb.Length}: 0x{curValue.Hex(2)} - 0x{nextValue.Hex(2)} |");
                                }
                            }
                        }
                        if (curValue == 0x07)
                        {
                            endReached = true;
                        }

                        curValue = r.ReadByte();

                        if (curValue == 0x00)
                        {
                            endReached = true;
                        }
                    }

                    readable.Add(sb.ToString());
                    if (readAll)
                    {
                        entryId++;
                    }
                }
                while (readAll && firstEntryOffset > entryId * 4);
            }
            if (sbDebug.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine(sbDebug.ToString());
            }
            return readable;
        }

        private bool CheckSets(List<SetData> sets, byte value, Reader r, string curMode, ref StringBuilder sb)
        {
            SetData? defaultSet = null;

            foreach (SetData set in sets)
            {
                if (set.region != RegionVersion.None && set.region != ROM.Instance.version)
                    continue;
                if (set.length != 0)
                {
                    defaultSet = set;
                }

                if (set.entries.ContainsKey(value))
                {
                    if (set.command != "")
                    {
                        var val = set.entries[value];
                        if(val == "")
                        {
                            sb.Append($"{{{set.command}}}");
                        }
                        else
                        {
                            sb.Append($"{{{set.command}:{set.entries[value]}}}");
                        }
                        return true;
                    }

                    if (set.mode != "" && curMode != set.mode)
                    {
                        sb.Append($"{{{set.mode}}}");
                        curMode = set.mode;
                    }

                    if (curMode != "" && curMode != set.mode)
                    {
                        sb.Append($"{{Normal}}");
                        curMode = "";
                    }
                    sb.Append(set.entries[value]);
                    return true;
                }
            }

            if (defaultSet.HasValue)
            {
                r.SetPosition(r.Position - 1);
                var set = defaultSet.Value;
                var length = set.length;
                sb.Append($"{{{set.command}");
                for (int i = 0; i < length; i++)
                {
                    sb.Append($":{r.ReadByte().Hex(2)}");
                }
                sb.Append("}");
                return true;
            }
            return false;
        }

        public void Save()
        { 
        }


        public List<byte> Sanitize2(int bankNum, int entryId, int langId, string text)
        {
            var data = new List<byte>();
            var matchingSet = stringToDataSets;
            var setLength = 0;

            if(entryId == 0x35 && bankNum == 0x0B)
            {
                Debug.WriteLine(text);
            }
            for (var pos = 0; pos < text.Length; pos++)
            {
                var val = text[pos] + "";
                if (val == "{") //command or mode
                {
                    var end = text.IndexOf('}', pos);
                    if (end != -1)
                    {
                        var sub = text.Substring(pos+1, end - pos - 1);

                        var parts = sub.Split(':');
                        if (parts.Length == 1) //mode or char
                        {
                            if (modeToDataSets.ContainsKey(parts[0]))
                            {
                                matchingSet = modeToDataSets[parts[0]];
                                pos = end;
                            }
                            else if (parts[0].Equals("{normal}", StringComparison.InvariantCultureIgnoreCase))
                            {
                                matchingSet = stringToDataSets;
                                pos += 8;
                            }
                        }
                        else //command or char
                        {
                            if (commandToDataSets.ContainsKey(parts[0]))
                            {
                                var command = commandToDataSets[parts[0]];
                                var sets = dataToStringSets[new SetKey(TranslationType.ANY, command.Item1)]; //dont know why command would ever need to have a hiragana/katakana/phonetic version
                                setLength = 0;
                                foreach (var set in sets)
                                {
                                    if (set.length != 0)
                                    {
                                        setLength = set.length;
                                        break;
                                    }
                                }

                                if (setLength != 0 && parts.Length > setLength + 1)
                                {
                                    throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\n{sub} exceeds max part size:{setLength}");
                                }

                                data.Add(command.Item1);
                                var commandSet = command.Item2;
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    int value;
                                    if (commandSet.ContainsKey(parts[1]))
                                    {
                                        data.Add(commandSet[parts[1]]);
                                    }
                                    else if (NumberUtil.TryParse("0x"+parts[i], out value))
                                    {
                                        data.Add((byte)value);
                                    }
                                    else 
                                    {
                                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find command value: {parts[1]} in command: {parts[0]}");
                                    }
                                    
                                }
                                pos = end;
                                setLength = 0;
                                continue;
                            }
                            if (parts[0].Equals("raw", StringComparison.InvariantCultureIgnoreCase))
                            {
                                int value;
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    if (!NumberUtil.TryParse(parts[i], out value))
                                    {
                                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to convert {parts[i]} into a number");
                                    }
                                    data.Add((byte)value);
                                }
                                pos = end;
                                continue;
                            }
                        }
                    }
                }

                if (stringToDataSets.ContainsKey(val))
                {
                    if (stringToDataSets[val][0] == 0)
                    {
                        data.Add(stringToDataSets[val][1]);
                    }
                    else
                    {
                        data.AddRange(stringToDataSets[val]);
                    }
                }
                else if(Environment.NewLine.StartsWith(val))
                {
                    data.Add(0x0A);
                    pos++;
                }
                else
                {
                    /*if (setLength == 0)
                    {
                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                    }*/

                    var searchDepth = 1;
                    val += text[pos + searchDepth];

                    var partials = matchingSet.Where(d => d.Key.StartsWith(val)).ToDictionary(d => d.Key, d => d.Value);
                    while (partials.Count != 0)
                    {
                        if (partials.ContainsKey(val))
                        {
                            data.AddRange(partials[val]);
                            break;
                        }
                        searchDepth++;
                        if (partials.Count == 0 || (pos + searchDepth) >= text.Length )
                        {
                            throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                        }
                        val += text[pos + searchDepth];
                        partials = partials.Where(d => d.Key.StartsWith(val)).ToDictionary(d => d.Key, d => d.Value);
                    }
                    if (partials.Count == 0)
                    {
                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankNum.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                    }
                    pos += searchDepth;
                }
            }

            if (data.Count >= 3 && data[data.Count - 3] != 0x07) //textjump
            {
                data.Add(0);
            }

            return data;
        }

        public void Sanitize(int bankId, int entryId, int langId, ref List<byte> data)
        {
            var text = textEntries[bankId][entryId][langId];
            var matchingSet = stringToDataSets;
            var setLength = 0;
            for (var pos = 0; pos < text.Length; pos++)
            {
                var val = text[pos] + "";
                if(val == "{") //command or mode
                {
                    var end = val.IndexOf('}',pos);
                    if (end != -1)
                    {
                        var sub = text.Substring(pos, end-pos);
                        var parts = sub.Split(' ');
                        if (parts.Length == 1) //mode or char
                        {
                            if (modeToDataSets.ContainsKey(parts[0]))
                            {
                                matchingSet = modeToDataSets[parts[0]];
                                pos += end - pos;
                                continue;
                            }

                            if(parts[0].Equals("{normal}", StringComparison.InvariantCultureIgnoreCase))
                            {
                                matchingSet = stringToDataSets;
                                pos += 8;
                            }
                        }
                        else //command or char
                        {
                            if (commandToDataSets.ContainsKey(parts[0]))
                            {
                                var command = commandToDataSets[parts[0]];
                                var sets = dataToStringSets[new SetKey(TranslationType.ANY, command.Item1)]; //dont know why command would ever need to have a hiragana/katakana/phonetic version
                                setLength = 0;
                                foreach (var set in sets)
                                {
                                    if(set.length != 0)
                                    {
                                        setLength = set.length;
                                        break;
                                    }
                                }

                                if(setLength != 0 && parts.Length > setLength + 1)
                                {
                                    throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\n{sub} exceeds max part size:{setLength}");
                                }

                                data.Add(command.Item1);
                                var commandSet = command.Item2;
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    int value;
                                    if (!commandSet.ContainsKey(parts[1]))
                                    {
                                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find command value: {parts[1]} in command: {parts[0]}");
                                    }
                                    if (!NumberUtil.TryParse(parts[i], out value))
                                    {
                                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to convert {parts[i]} into a number");
                                    }
                                    data.Add((byte)value);
                                }

                                continue;
                            }
                            if(parts[0].Equals("raw", StringComparison.InvariantCultureIgnoreCase))
                            {
                                int value;
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    if (!NumberUtil.TryParse(parts[i], out value))
                                    {
                                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to convert {parts[i]} into a number");
                                    }
                                    data.Add((byte)value);
                                }
                            }
                        }
                    }
                }

                if (stringToDataSets.ContainsKey(val))
                {
                    data.AddRange(stringToDataSets[val]);
                }
                else
                {
                    if (setLength == 0)
                    {
                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                    }

                    var searchDepth = 1;
                    val += text[pos + searchDepth];

                    var partials = matchingSet.Where(d => d.Key.StartsWith(val)).ToDictionary(d => d.Key, d => d.Value);
                    while (partials.Count != 0)
                    {
                        if (partials.ContainsKey(val))
                        {
                            data.AddRange(partials[val]);
                            break;
                        }
                        searchDepth++;
                        if(searchDepth > setLength)
                        {
                            throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                        }
                        val += text[pos + searchDepth];
                        partials = partials.Where(d => d.Key.StartsWith(val)).ToDictionary(d => d.Key, d => d.Value);
                    }
                    if (partials.Count == 0)
                    {
                        throw new ArgumentException($"Language: {langId}, Bank: 0x{bankId.Hex(2)}, Entry: 0x{entryId.Hex(2)}\nUnable to find a conversion for: {val}");
                    }
                    pos += searchDepth;
                }

                if(data[data.Count-3] != 0x07) //textjump
                {
                    data.Add(0);
                }
            }
        }

        public List<string> GetEntries(int bankNum, int entryNum)
        {
            return textEntries[bankNum][entryNum];
        }

        public void SetEntry(int bankNum, int entryNum, int langNum, string text)
        {
            if(textEntries[bankNum][entryNum][langNum] == text) 
            {
                return;
            }

            if(!changedEntries.ContainsKey(bankNum))
            {
                changedEntries.Add(bankNum, new Dictionary<int, List<string>>());
            }

            if(!changedEntries[bankNum].ContainsKey(entryNum))
            {
                changedEntries[bankNum].Add(entryNum, new List<string>());
            }

            textEntries[bankNum][entryNum][langNum] = text;

            for(int i = 0; i < LangCount; i++) //making sure that we dont go out of bounds
            {
                if(changedEntries[bankNum][entryNum].Count <= i)
                {
                    changedEntries[bankNum][entryNum].Add(textEntries[bankNum][entryNum][i]);
                }
            }
            var a = changedEntries[bankNum];
            var b = a[entryNum];
            var c = b[langNum];
            c = text;
            changedEntries[bankNum][entryNum][langNum] = text;
        }

        public int GetBankCount()
        {
            return bankCount;
        }

        public int GetEntryCount(int bankNum)
        {

            if(!bankLengths.ContainsKey(bankNum))
            {
                
                var languageStart = ROM.Instance.headers.languageTableLoc;
                var r = ROM.Instance.reader;
                var languageBankTableLoc = r.ReadAddr(languageStart);
                var bankTableLoc = languageBankTableLoc + r.ReadInt(languageBankTableLoc + (bankNum * 4));
                var size = r.ReadInt(bankTableLoc) / 4;
                if (changedEntries.ContainsKey(bankNum))
                {
                    var highest = changedEntries.Keys.Max();
                    if(size < highest)
                    {
                        size = highest;
                    }
                }
                bankLengths.Add(bankNum, size);
            }

            return bankLengths[bankNum];
        }

        public string GetAsJSON(int bankNum)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(changedEntries[bankNum]);
        }

        public void LoadFromJSON(int bankNum)
        {
            var path = $"{Project.Instance.ProjectPath}/Global/{DataType.textData}/{DataType.textData}{bankNum.Hex(2)}.json";
            if (File.Exists(path))
            {
                if (!changedEntries.ContainsKey(bankNum))
                {
                    changedEntries.Add(bankNum, new Dictionary<int, List<string>>());
                }

                changedEntries[bankNum] = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(File.ReadAllText(path));
                while (textEntries.Count() <= bankNum)
                {
                    textEntries.Add(new List<List<string>>());
                }
                while (textEntries[bankNum].Count() <= changedEntries[bankNum].Last().Key)
                {
                    textEntries[bankNum].Add(new List<string>());
                }
            }
        }

        public struct LangBankData
        {
            public byte[] zeroEntry;
            public int bankStart;
            public int[] entryIds;
            public int[] entryOffsets;
            public byte[] bankText;
        }


        public LangBankData[] GetAsData2(int bankNum)
        {
            //create all entry tables for all languages of a single bank
            // language=>banklist=>entrylist=>entry
            var languageStart = ROM.Instance.headers.languageTableLoc;
            var r = ROM.Instance.reader;
            var lang0Loc = r.ReadAddr(languageStart);
            var firstBankEntryOffset = r.ReadInt(lang0Loc + (bankNum * 4));
            var originalEntryCount = r.ReadInt(lang0Loc + firstBankEntryOffset) / 4;
            var changedBankEntries = changedEntries[bankNum];
            var offset = 0;


            var textData = new LangBankData[LangCount];

            List<int> originalEntrySizes = new List<int>();
            List<List<byte>> sanitizedZeroEntries = new List<List<byte>>();
            if(changedBankEntries.ContainsKey(0))
            {
                var originalEntries = ChangeToReadable(bankNum, 0);
                for (int langId = 0; langId < LangCount; langId++)
                {
                    sanitizedZeroEntries.Add(Sanitize2(bankNum, 0, langId, changedBankEntries[0][langId]));
                    originalEntrySizes.Add(Sanitize2(bankNum, 0, langId, originalEntries[langId]).Count()); //regrab data from rom to get the vanilla text;
                }
            }
            else
            {
                for (int langId = 0; langId < LangCount; langId++)
                {
                    var sanitizedZeroEntry = Sanitize2(bankNum, 0, langId, textEntries[bankNum][0][langId]);
                    sanitizedZeroEntries.Add(sanitizedZeroEntry);
                    originalEntrySizes.Add(sanitizedZeroEntry.Count);
                }
            }

            var extraEntries = textEntries.Count - originalEntryCount;
            for (int langId = 0; langId < LangCount; langId++)
            {
                var entryIds = changedBankEntries.Keys.ToList();
                var lbd = new LangBankData();
                var langLoc = r.ReadAddr(languageStart + (langId *4));
                lbd.bankStart = langLoc + r.ReadAddr(langLoc + (bankNum * 4));

                //pre-check for 0th entry space
                var missingSpace = sanitizedZeroEntries[langId].Count - originalEntrySizes[langId] + (extraEntries * 4);

                if (missingSpace > 0)
                {
                    List<byte> zeroEntry = sanitizedZeroEntries[langId];

                    var extraSlots = 0;
                    lbd.zeroEntry = zeroEntry.ToArray();

                    while(missingSpace > 0)
                    {
                        var originalText = ChangeToReadable(bankNum, extraSlots + 1)[langId];
                        missingSpace -= Sanitize2(bankNum, extraSlots + 1, langId, originalText).Count;
                        extraSlots++;
                        if (!entryIds.Contains(extraSlots))
                        {
                            entryIds.Add(extraSlots); //also move this one around if it isnt in the list yet
                        }
                    }
                }
                entryIds.Sort();
                lbd.entryIds = entryIds.ToArray();

                var offsets = new List<int>();
                var textBytes = new List<byte>();
                foreach(var entryId in entryIds)
                {
                    if (entryId == 0)
                    {
                        lbd.zeroEntry = sanitizedZeroEntries[langId].ToArray();
                        offsets.Add(originalEntryCount * 4);
                    }
                    else
                    {
                        var entryData = Sanitize2(bankNum, entryId, langId, changedBankEntries[entryId][langId]).ToArray();
                        textBytes.AddRange(entryData);
                        offsets.Add(offset);
                        offset += entryData.Length;
                    }
                }
                lbd.bankText = textBytes.ToArray();
                lbd.entryOffsets = offsets.ToArray();
                textData[langId] = lbd;
            }
            return textData;
        }

        public int[] GetAsData(out byte[] data, int bankId)
        {
            //create all entry tables for 1 language
            //create bank table for above entry tables
            //store offset to start of bank table
            //repeat until languages are done
            // language=>banklist=>entrylist=>entry

            List<byte[]> languages = new List<byte[]>();
            var totalLength = 0;
            var languageOffsets = new int[LangCount];
            for (int langNum = 0; langNum < LangCount; langNum++)
            {
                List<byte[]> banks = new List<byte[]>();
                int languageLength = 0;
                languageOffsets[langNum] = totalLength;
                for (int bank = 0; bank < textEntries.Count; bank++)
                {
                    byte[] bankEntry;
                    var textStart = textEntries[bank].Count * 4; //start of first entry
                    byte[] entryOffsets = new byte[textStart];

                    List<byte> textData = new List<byte>();
                    for (int entry = 0; entry < textEntries[bank].Count; entry++)
                    {
                        entryOffsets[entry * 4] = (byte)((textStart + textData.Count) & 0xff); //start of next string, might need to align 4 it
                        entryOffsets[entry * 4 + 1] = (byte)((textStart + textData.Count) & 0xff00);
                        entryOffsets[entry * 4 + 2] = (byte)((textStart + textData.Count) & 0xff0000);
                        entryOffsets[entry * 4 + 3] = (byte)((textStart + textData.Count) & 0xff000000);

                        //transform the entry into a list of bytes
                        Sanitize(bank, entry, langNum, ref textData);
                    }
                    var entryText = textData.ToArray();
                    bankEntry = new byte[textStart + textData.Count];
                    entryOffsets.CopyTo(bankEntry, 0);
                    entryText.CopyTo(bankEntry, textStart);
                    banks.Add(bankEntry);
                    languageLength += bankEntry.Length;
                    //entrylist + entries
                }

                byte[] languageEntry = new byte[banks.Count*4 + languageLength];
                var languageProgress = banks.Count * 4; //start at the end of the offsets
                for (int i = 0; i < banks.Count; i++)
                {
                    banks[i].CopyTo(languageEntry, languageProgress);
                    languageEntry[i * 4] = (byte)(languageProgress & 0xff);
                    languageEntry[i * 4+1] = (byte)(languageProgress & 0xff00);
                    languageEntry[i * 4+2] = (byte)(languageProgress & 0xff0000);
                    languageEntry[i * 4+3] = (byte)(languageProgress & 0xff000000);
                    languageProgress += banks[i].Length;
                }
                languages.Add(languageEntry);
                totalLength += languageEntry.Length;
            }
            byte[] languageData = new byte[totalLength];
            var totalProgress = 0;
            for(int i = 0; i < languages.Count; i++)
            {
                languages[i].CopyTo(languageData, totalProgress);
                totalProgress += languages[i].Length;
            }

            data = languageData;
            return languageOffsets;
        }
    }
}
