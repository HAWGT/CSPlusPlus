using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace CSGOPlusPlus.Helpers
{
    class BindExecuter
    {
        public static void ExecuteBind(VirtualKeyCode vkey)
        {
            KBMHelper.PressKey(vkey);
        }
    }
}
