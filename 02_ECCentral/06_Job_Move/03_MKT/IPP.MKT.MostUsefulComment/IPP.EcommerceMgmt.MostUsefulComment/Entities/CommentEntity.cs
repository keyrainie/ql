using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.MostUsefulComment.Entities
{
    public class CommentEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }
    }
}
