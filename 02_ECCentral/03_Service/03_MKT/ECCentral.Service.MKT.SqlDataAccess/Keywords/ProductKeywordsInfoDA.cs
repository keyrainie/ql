using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
using System.IO;
using System.Data.OleDb;
namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductKeywordsInfoDA))]
    public class ProductKeywordsInfoDA : IProductKeywordsInfoDA
    {
        #region 关键字对应商品（ProductKeywordsInfo）
        /// <summary>
        /// 添加自关键字对应商品  
        /// </summary>
        /// <param name="item"></param>
        public void AddProductKeywords(ProductKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertKeywordsForProduct");
            dc.SetParameterValue<ProductKeywordsInfo>(item);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 编辑关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        public void EditProductKeywords(ProductKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateKeywordsForProduct");
            dc.SetParameterValue<ProductKeywordsInfo>(item);
            dc.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="item"></param>
        public void ChangeProductKeywordsStatus(ProductKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_ChangeProductKeywordsStatus");
            dc.SetParameterValue("@SysNo", item.SysNo);
            dc.SetParameterValue("@Status", item.Status);
            dc.SetParameterValue("@EditUser", item.User.UserName);
            dc.ExecuteNonQuery();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductKeywords(int SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_DeleteProductKeywords");
            dc.SetParameterValue("@SysNo", SysNo);
            dc.ExecuteNonQuery();

        }
        #endregion


        
    }
}
