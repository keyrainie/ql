//************************************************************************
// 用户名				泰隆优选
// 系统名				商品图片管理
// 子系统名		        商品图片管理业务实现
// 作成者				Tom
// 改版日				2012.6.02
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductResourceAppService))]
    public class ProductResourceAppService
    {
        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="productCommonInfoSysNo"></param>
        public void DeleteProductResource(IList<ProductResourceForNewegg> entity)
        {
            if (entity == null || entity.Count == 0) return;
            var oper = ObjectFactory<ProductResourceProcessor>.Instance;
            foreach (var item in entity)
            {
                oper.DeleteProductResource(item);
            }
        }

        /// <summary>
        /// 修改商品图片
        /// </summary>
        /// <param name="entity"></param>
        public void ModifyProductResources(IList<ProductResourceForNewegg> entity)
        {
            var oper = ObjectFactory<ProductResourceProcessor>.Instance;
            using (var tran = new TransactionScope())
            {
                if (entity == null || entity.Count == 0) return;
                foreach (var item in entity)
                {
                    oper.ModifyProductResources(item);
                }
                tran.Complete();
            }
        }

        /// <summary>
        /// 创建商品图片
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="productCommonInfoSysNo"></param>
        public List<ProductResourceForNewegg> CreateProductResource(List<ProductResourceForNewegg> entity)
        {
            var oper = ObjectFactory<ProductResourceProcessor>.Instance;
            oper.CreateProductResource(entity);
            return entity;
        }

        /// <summary>
        /// 创建商品图片
        /// </summary>
        /// <param name="fileName"></param>
        public List<string> IsExistFileName(List<string> fileName)
        {
            var oper = ObjectFactory<ProductResourceProcessor>.Instance;
            var resultList = new List<string>();
            foreach (var item in fileName)
            {
                var result = oper.IsExistFileName(item);
                if(result)
                {
                    resultList.Add(item);
                }
            }
            return resultList;
        }

    }

   
}
