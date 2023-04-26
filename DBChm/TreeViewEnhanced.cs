using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBCHM
{
    /// <summary>
    /// 在使用treeview控件过程中会碰到，
    /// 当快速点击checkbox时，checkbox选中状态和实际状态不符，并且不会触发aftercheck事件，
    /// 造成此问题的原因是：快速点击识别为双击事件。
    /// </summary>
    public class TreeViewEnhanced : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203) { m.Result = IntPtr.Zero; }
            else base.WndProc(ref m);
        }
    }
}