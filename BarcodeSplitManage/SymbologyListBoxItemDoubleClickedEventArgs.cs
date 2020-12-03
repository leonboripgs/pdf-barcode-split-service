using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeSplitManage
{
   public class SymbologyListBoxItemDoubleClickedEventArgs : EventArgs
   {
      private SymbologyListBoxItemDoubleClickedEventArgs()
      {
      }

      public SymbologyListBoxItemDoubleClickedEventArgs(int index)
      {
         _index = index;
      }

      private int _index;
      public int Index
      {
         get { return _index; }
      }
   }
}
