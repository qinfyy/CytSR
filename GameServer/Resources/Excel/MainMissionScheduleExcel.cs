using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameServer.Resources.Excel
{
    [ResourceType("MainMissionSchedule.json")]
    public class MainMissionScheduleExcel : ExcelBase
    {
        public uint MainMissionID;

        public override uint GetId()
        {
            return MainMissionID;
        }
    }
}
