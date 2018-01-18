using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCHM
{
    public class ProgressArg
    {
        public ProgressArg(Action execAct, int maxNum)
        {
            this.ExecAct = execAct;
            this.MaxNum = maxNum;
        }
        public Action ExecAct { get; set; }

        public int MaxNum { get; set; }

    }
}
