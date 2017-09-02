using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess
{

    public interface IProductKeywordsInfoDA
    {
        #region 关键字对应商品（ProductKeywordsInfo）
        /// <summary>
        /// 添加关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        void AddProductKeywords(ProductKeywordsInfo item);

        /// <summary>
        /// 编辑关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        void EditProductKeywords(ProductKeywordsInfo item);

        /// <summary>
        /// 更新关键字对应商品状态
        /// </summary>
        /// <param name="item"></param>
        void ChangeProductKeywordsStatus(ProductKeywordsInfo item);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        void DeleteProductKeywords(int SysNo);
        #endregion
    }
}
