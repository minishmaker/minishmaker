using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Utilities.Rework
{
    public class UndoRedoManager
    {
        private int actionIndex = -1;
        private List<UndoRedoEntry> entries = new List<UndoRedoEntry>();
        private static UndoRedoManager instance;

        private UndoRedoManager() { }

        public static UndoRedoManager Get()
        {
            if (instance == null)
            {
                instance = new UndoRedoManager();
            }

            return instance;
        }

        public void AddEntry(UndoRedoEntry entry)
        {
            actionIndex++;
            if (actionIndex == entries.Count)
            {
                entries.Add(entry);
            }
            else
            {
                entries[actionIndex] = entry;
            }

            if (actionIndex != 0)
            {
                entries[actionIndex - 1].hasNext = true;
            }
        }

        public void Redo()
        {
            if (actionIndex == -1 && entries.Count == 0)
                return;
            if (actionIndex !=-1 && !entries[actionIndex].hasNext)
                return;

            //do redo action of actionList[actionIndex+1]
            entries[actionIndex + 1].Redo();
            actionIndex++;
        }

        public void Undo()
        {
            if (actionIndex == -1)
                return;

            //do undo action of actionList[actionIndex]
            entries[actionIndex].Undo();
            actionIndex--;
        }
    }
}
