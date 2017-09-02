using System.Data;
using Newegg.Oversea.Framework.Entity;
namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
   public class PMMPISysNoEntity
    {
        [DataMapping("PMSysNumber", DbType.String)]
        public string PMSysNumber { get; set; } 
        [DataMapping("PMLoginName", DbType.String)]
        public string PMLoginName { get; set; } 
        [DataMapping("PMDisplayName", DbType.String)]
        public string PMDisplayName { get; set; } 
        [DataMapping("Category1Name", DbType.String)]
        public string Category1Name { get; set; } 
        [DataMapping("PMStatus", DbType.Int32)]
        public int? PMStatus { get; set; }
        [DataMapping("PMGroupSysNo", DbType.Int32)]
        public int? PMGroupSysNo { get; set; }

    }
}
