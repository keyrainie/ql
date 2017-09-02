using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using System.IO;
using System.Data;


namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductKeywordsInfoProcessor))]
    public class ProductKeywordsInfoProcessor
    {
        private IProductKeywordsInfoDA keywordDA = ObjectFactory<IProductKeywordsInfoDA>.Instance;


        #region 关键字对应商品（ProductKeywordsInfo）
        /// <summary>
        /// 添加自关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddProductKeywords(ProductKeywordsInfo item)
        {
            keywordDA.AddProductKeywords(item);
        }

        /// <summary>
        /// 编辑关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditProductKeywords(ProductKeywordsInfo item)
        {
            keywordDA.EditProductKeywords(item);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="item"></param>
        public void ChangeProductKeywordsStatus(List<ProductKeywordsInfo> list)
        {
            foreach (var item in list)
            {
                keywordDA.ChangeProductKeywordsStatus(item);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductKeywords(List<int> list)
        {
            foreach (var item in list)
            {
                keywordDA.DeleteProductKeywords(item);
            }
        }

        #endregion
    }
}
