using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Resources.Excel
{
    [ResourceType("ContentPackageConfig.json")]
    public class ContentPackageConfigExcel : ExcelBase
    {
        public uint ContentID { get; set; }

        public override uint GetId()
        {
            return ContentID;
        }
    }
}
