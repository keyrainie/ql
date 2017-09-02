using System.Data;
using Newegg.Oversea.Framework.Entity;
namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
   public class CategorySysNoEntity
    {
       [DataMapping("Category1SysNo", DbType.String)]
       public string Category1SysNo { get; set; } 
    }
}
