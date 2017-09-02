//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		        PM组管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductManagerGroupAppService))]
    public class ProductManagerGroupAppService
    {

        private readonly ProductManagerGroupProcessor productmanagergroupProcessor = ObjectFactory<ProductManagerGroupProcessor>.Instance;
        /// <summary>
        /// 根据SysNO获取PM组信息
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo GetProductManagerGroupInfoBySysNo(int productManagerGroupInfoSysNo)
        {
            var result = ObjectFactory<ProductManagerGroupProcessor>.Instance.GetProductManagerGroupInfoBySysNo(productManagerGroupInfoSysNo);
            return result;
        }

        /// <summary>
        /// 根据userSysNo获取PM组信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo)
        {
            var result = ObjectFactory<ProductManagerGroupProcessor>.Instance.GetPMListByUserSysNo(userSysNo);
            return result;
        }

        /// <summary>
        /// 创建PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo CreateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            ProductManagerGroupInfo result = new ProductManagerGroupInfo();

            using (TransactionScope scope = new TransactionScope())
            {
                //创建组信息
                result = productmanagergroupProcessor.CreateProductManagerGroupInfo(entity);

                //创建组内PM信息
                if (entity.ProductManagerInfoList.Count > 0)
                {
                    foreach (ProductManagerInfo item in entity.ProductManagerInfoList)
                    {
                        productmanagergroupProcessor.UpdatePMMasterGroupSysNo(item.UserInfo.SysNo.Value, result.SysNo.Value);
                    }

                }

                scope.Complete();
            }

            return result;
        }

        /// <summary>
        /// 修改PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo UpdateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            ProductManagerGroupInfo result = new ProductManagerGroupInfo();

            using (TransactionScope scope = new TransactionScope())
            {
                //修改组信息
                result = ObjectFactory<ProductManagerGroupProcessor>.Instance.UpdateProductManagerGroupInfo(entity);

                //清空原来选择该组的PM
                productmanagergroupProcessor.ClearPMMasterGroupSysNo(entity.SysNo.Value);
                //创建组内PM信息
                if (entity.ProductManagerInfoList.Count > 0)
                {
                    foreach (ProductManagerInfo item in entity.ProductManagerInfoList)
                    {
                        productmanagergroupProcessor.UpdatePMMasterGroupSysNo(item.UserInfo.SysNo.Value, result.SysNo.Value);
                    }

                }
                scope.Complete();
            }

            return result;
        }

    }
}
