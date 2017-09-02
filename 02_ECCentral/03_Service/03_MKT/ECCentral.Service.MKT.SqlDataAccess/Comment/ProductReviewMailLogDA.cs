using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess
{

    [VersionExport(typeof(IProductReviewMailLogDA))]
    public class ProductReviewMailLogDA : IProductReviewMailLogDA
    {

        #region 有关评论所使用到的邮件LOG

        /// <summary>
        /// 检查邮件是否存在,返回true则存在
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool CheckProductCommentMailLog(ProductReviewMailLog log)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductReviewMailLog_CheckProductCommentDetailMailLog");
            cmd.SetParameterValue<ProductReviewMailLog>(log);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 新建回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        public void CreateProductCommentMailLog(ProductReviewMailLog log)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReviewMailLog_CreateProductCommentDetailMailLog");
            dc.SetParameterValue<ProductReviewMailLog>(log);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        public void UpdateProductCommentMailLog(ProductReviewMailLog log)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReviewMailLog_UpdateProductCommentDetailMailLog");
            dc.SetParameterValue<ProductReviewMailLog>(log);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取关于咨询的邮件log
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public ProductReviewMailLog QueryProductCommentMailLog(int sysNo, string type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReviewMailLog_QueryMailLogByRefSysNo");
            dc.SetParameterValue("@RefSysNo", sysNo);
            dc.SetParameterValue("@Type", type);//==C

            DataTable dt = dc.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return DataMapper.GetEntity<ProductReviewMailLog>(dt.Rows[0]);
            return null;
        }
        #endregion 
    }
}
