using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{

    [VersionExport(typeof(ProductKeywordsInfoAppService))]
    public class ProductKeywordsInfoAppService
    {

        #region 关键字对应商品（ProductKeywordsInfo）
        /// <summary>
        /// 添加自关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddProductKeywords(ProductKeywordsInfo item)
        {
            ObjectFactory<ProductKeywordsInfoProcessor>.Instance.AddProductKeywords(item);
        }

        /// <summary>
        /// 编辑关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditProductKeywords(ProductKeywordsInfo item)
        {
            ObjectFactory<ProductKeywordsInfoProcessor>.Instance.EditProductKeywords(item);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="item"></param>
        public void ChangeProductKeywordsStatus(List<ProductKeywordsInfo> list)
        {
            ObjectFactory<ProductKeywordsInfoProcessor>.Instance.ChangeProductKeywordsStatus(list);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductKeywords(List<int> list)
        {
            ObjectFactory<ProductKeywordsInfoProcessor>.Instance.DeleteProductKeywords(list);
        }

        #endregion
    }
}
