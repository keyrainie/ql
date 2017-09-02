//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		        PM组管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductManagerGroupProcessor))]
    public class ProductManagerGroupProcessor
    {
        private readonly IProductManagerGroupDA _productManagerGroupDA = ObjectFactory<IProductManagerGroupDA>.Instance;

        #region PM组业务方法
        /// <summary>
        /// 根据SysNo获取PM组信息
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        public virtual ProductManagerGroupInfo GetProductManagerGroupInfoBySysNo(int productManagerGroupInfoSysNo)
        {
            CheckProductManagerProcessor.CheckProductManagerrGroupInfoSysNo(productManagerGroupInfoSysNo);
            return _productManagerGroupDA.GetProductManagerGroupInfoBySysNo(productManagerGroupInfoSysNo);
        }

        /// <summary>
        /// 根据userSysNo获取PM组信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public virtual ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo)
        {
            CheckProductManagerProcessor.CheckUserSysNo(userSysNo);
            return _productManagerGroupDA.GetPMListByUserSysNo(userSysNo);
        }

        /// <summary>
        /// 创建PM组信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductManagerGroupInfo CreateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            CheckProductManagerProcessor.CheckProductManagerrGroupInfo(entity);
            return _productManagerGroupDA.CreateProductManagerGroupInfo(entity);
        }

        /// <summary>
        /// 更新PM隶属于哪个PM组
        /// </summary>
        /// <param name="pmUserSysNo"></param>
        /// <param name="pmGroupSysNo"></param>
        public virtual void UpdatePMMasterGroupSysNo(int pmUserSysNo, int pmGroupSysNo)
        {
            _productManagerGroupDA.UpdatePMMasterGroupSysNo(pmUserSysNo, pmGroupSysNo);
        }

        /// <summary>
        /// 清空PM对应的PM组
        /// </summary>
        /// <param name="pmGroupSysNo"></param>
        public virtual void ClearPMMasterGroupSysNo(int pmGroupSysNo)
        {
            _productManagerGroupDA.ClearPMMasterGroupSysNo(pmGroupSysNo);
        }

        /// <summary>
        /// 修改品PM组信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductManagerGroupInfo UpdateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            if (entity != null)
            {
                CheckProductManagerProcessor.CheckProductManagerrGroupInfoSysNo(entity.SysNo);
            }
            CheckProductManagerProcessor.CheckProductManagerrGroupInfo(entity);
            return _productManagerGroupDA.UpdateProductManagerGroupInfo(entity);
        }


        #endregion

        #region 检查PM组逻辑
        private static class CheckProductManagerProcessor
        {
            /// <summary>
            /// 检查PM实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductManagerrGroupInfo(ProductManagerGroupInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupIsNull"));
                }
                if (entity.UserInfo == null || entity.UserInfo.SysNo < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupuserSysNoIsNull"));
                }
                //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
                var localName = entity.PMGroupName.Content;
                if (String.IsNullOrEmpty(localName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupNameIsNull"));
                }

                ChecPMGroupName(localName, entity.SysNo);

            }

            /// <summary>
            /// 检查PM编号
            /// </summary>
            /// <param name="productManagerrGroupInfoSysNo"></param>
            public static void CheckProductManagerrGroupInfoSysNo(int? productManagerrGroupInfoSysNo)
            {

                if (productManagerrGroupInfoSysNo == null || productManagerrGroupInfoSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupSysNoIsNull"));
                }
            }

            /// <summary>
            /// 检查用户编号
            /// </summary>
            /// <param name="userSysNo"></param>
            public static void CheckUserSysNo(int? userSysNo)
            {

                if (userSysNo == null || userSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "UserSysNoIsNull"));
                }
            }

            /// <summary>
            /// 是否存在某个同名的PM
            /// </summary>
            /// <param name="groupName"></param>
            /// <param name="productManagerrGroupInfoSysNo"></param>
            /// <returns></returns>
            private static void ChecPMGroupName(string groupName, int? productManagerrGroupInfoSysNo)
            {
                if (String.IsNullOrEmpty(groupName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupNameIsNull"));
                }
                if (productManagerrGroupInfoSysNo == null)
                {
                    productManagerrGroupInfoSysNo = 0;
                }

                if (productManagerrGroupInfoSysNo.Value < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupSysNoIsNull"));
                }
                var productManagerGroupDA = ObjectFactory<IProductManagerGroupDA>.Instance;
                var isExist = productManagerrGroupInfoSysNo == 0 ? productManagerGroupDA.IsExistPMGroupName(groupName) : productManagerGroupDA.IsExistPMGroupName(groupName, productManagerrGroupInfoSysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ExistProductManagerGroupuserSysNo"));
                }
            }
        }
    }
        #endregion
}

