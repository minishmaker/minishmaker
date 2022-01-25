using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core
{
    public class LanguageManager
    {
        private static LanguageManager _Instance;
        private List<List<List<string>>> textEntries;
        public int LangCount {get; private set;}

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
            Load();
        }

        private void Load() {
            //read through
            textEntries = new List<List<List<string>>>();
            var languageStart = ROM.Instance.headers.languageTableLoc;
            var r = ROM.Instance.reader;

            bool validLBT;
            var langId = 0;
            while (true)
            {
                var languageBankTableLoc = r.ReadAddr(languageStart + (langId * 4));
                r.SetPosition(r.Position - 1);
                validLBT = r.ReadByte() == 8;

                if (!validLBT)
                {
                    break;
                }

                int firstBankOffset = 0;
                var bankId = 0;
                do
                {
                    var bankOffset = r.ReadInt(languageBankTableLoc + (bankId * 4));
                    var bankLoc = languageBankTableLoc + bankOffset;
                    if (firstBankOffset == 0)
                    {
                        firstBankOffset = bankOffset;
                    }

                    var firstEntryOffset = 0;
                    var entryId = 0;

                    var bankExists = textEntries.Count() > bankId;
                    var textEntryCount = bankExists ? textEntries[bankId].Count() : 0;
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
                        byte curChar;
                        do
                        {
                            curChar = r.ReadByte();
                            sb.Append((char)curChar);
                        }
                        while (curChar != 0x00);

                        if (!bankExists)
                        {
                            textEntries.Add(new List<List<string>>());
                            bankExists = true;
                        }

                        if (textEntryCount <= entryId)
                        {
                            textEntries[bankId].Add(new List<string>());
                        }

                        textEntries[bankId][entryId].Add(sb.ToString());
                        entryId++;
                    }
                    while (firstEntryOffset > entryId * 4);
                    bankId++;
                }
                while (firstBankOffset > bankId * 4);
                langId++;
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
            textEntries[bankNum][entryNum][langNum] = text;
        }

        public int GetBankCount()
        {
            return textEntries.Count();
        }

        public int GetEntryCount(int bankNum)
        {
            return textEntries[bankNum].Count;
        }

        public int[] GetAsData(out byte[] data)
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
                    var entryOffset = textStart;
                    byte[] entryOffsets = new byte[textStart];

                    StringBuilder sb = new StringBuilder();
                    for (int entry = 0; entry < textEntries[bank].Count; entry++)
                    {
                        entryOffsets[entry * 4] = (byte)(entryOffset & 0xff); //start of next string, might need to align 4 it
                        entryOffsets[entry * 4 + 1] = (byte)(entryOffset & 0xff00);
                        entryOffsets[entry * 4 + 2] = (byte)(entryOffset & 0xff0000);
                        entryOffsets[entry * 4 + 3] = (byte)(entryOffset & 0xff000000);
                        var text = textEntries[bank][entry][langNum];
                        //sanitize the string to be correctly encoded
                        var sanitized = text;
                        //TODO
                        sb.Append(sanitized);
                        entryOffset += sanitized.Length;
                    }
                    var entryText = (byte[])Encoding.ASCII.GetBytes(sb.ToString());
                    bankEntry = new byte[textStart + entryOffset];
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
