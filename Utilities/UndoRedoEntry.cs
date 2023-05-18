using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Utilities
{
    public class UndoRedoEntry
    {
        public bool hasNext = false;

        public ActionEnum actionType;

        private Action<Object[]> undoFunc;
        private Action<Object[]> redoFunc;

        private Object[] redoData;
        private Object[] undoData;

        public UndoRedoEntry(Object[] undoData, Action<Object[]> undoFunc, object[] redoData, Action<Object[]> redoFunc, ActionEnum actionType) {
            this.undoData = undoData;
            this.redoData = redoData;
            this.undoFunc = undoFunc;
            this.redoFunc = redoFunc;
            this.actionType = actionType;
        }

        public enum ActionEnum {
            EDIT_BG,
            CHANGE_ROOM,
            CHANGE_ROOM_SIZE,
            CHANGE_METATILE,

            EDIT_LIST
        }

        public void Undo() 
        {
            undoFunc(undoData);
        }

        public void Redo()
        {
            redoFunc(redoData);
        }
    }
}
