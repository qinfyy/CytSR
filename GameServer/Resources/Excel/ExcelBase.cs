using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Resources.Excel
{
    public abstract class ExcelBase
    {
        public abstract uint GetId();

        public virtual void OnLoad() { }

        public virtual void Finalized() { }

        public virtual void AfterAllDone() { }
    }
}
