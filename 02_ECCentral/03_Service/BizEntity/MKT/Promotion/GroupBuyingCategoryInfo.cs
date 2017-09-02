using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 团购分类信息
    /// </summary>
    public class GroupBuyingCategoryInfo : IIdentity, ICompany
    {
        public int? SysNo { get; set; }
        public GroupBuyingCategoryType? CategoryType { get; set; }
        public string Name { get; set; }
        public GroupBuyingCategoryStatus? Status { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string EditUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string CompanyCode { get; set; }
        public bool IsHotKey { get; set; }
    }
}
