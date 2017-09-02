using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace AutoClose.Model
{
    public class PoSysNoItem
    {
        [DataMapping("PoSysNo", DbType.Int32)]
        public int PoSysNo { get; set; }

        [DataMapping("PoStatus", DbType.Int32)]
        public int PoStatus { get; set; }
    }
}
