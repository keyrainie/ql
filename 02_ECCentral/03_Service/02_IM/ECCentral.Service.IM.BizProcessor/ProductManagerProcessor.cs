//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		        PM管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.ComponentModel;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductManagerProcessor))]
    public class ProductManagerProcessor
    {
        private readonly IProductManagerDA _productManagerDA = ObjectFactory<IProductManagerDA>.Instance;

        #region PM业务方法
        /// <summary>
        /// 根据SysNo获取PM信息
        /// </summary>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        public virtual ProductManagerInfo GetProductManagerInfoBySysNo(int productManagerInfoSysNo)
        {
            CheckProductManagerProcessor.CheckProductManagerInfoSysNo(productManagerInfoSysNo);
            return _productManagerDA.GetProductManagerInfoBySysNo(productManagerInfoSysNo);
        }

        /// <summary>
        /// 创建PM信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductManagerInfo CreateProductManagerInfo(ProductManagerInfo entity)
        {
            CheckProductManagerProcessor.CheckProductManagerInfo(entity);
            return _productManagerDA.CreateProductManagerInfo(entity);
        }

        /// <summary>
        /// 修改品PM息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductManagerInfo UpdateProductManagerInfo(ProductManagerInfo entity)
        {
            if (entity != null)
            {
                CheckProductManagerProcessor.CheckProductManagerInfoSysNo(entity.SysNo);
            }
            CheckProductManagerProcessor.CheckProductManagerInfo(entity);
            if (entity.Status == PMStatus.DeActive)
            {
                ObjectFactory<ProductLineProcessor>.Instance.DeleteByPMUserSysNo(entity.UserInfo.SysNo.Value);
            }
            return _productManagerDA.UpdateProductManagerInfo(entity);
        }


        #endregion

        #region 检查品牌逻辑
        private static class CheckProductManagerProcessor
        {
            /// <summary>
            /// 检查PM实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductManagerInfo(ProductManagerInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManagerIsNull"));
                }
                if (entity.UserInfo == null || entity.UserInfo.SysNo < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManageruserSysNoIsNull"));
                }
                //获取UserID的SysNo
                UserInfo userEntity = GetUserInfoByUserID(entity.UserInfo.UserID);
                if (userEntity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "PMIDIsNull"));
                }
                entity.UserInfo.SysNo = userEntity.SysNo;
                CheckIsExistPMUserSysNo(userEntity.SysNo.Value);
                CheckPMUserSysNo(userEntity.SysNo.Value, entity.SysNo);
                if (entity.SysNo != null && entity.SysNo > 0 && entity.Status == PMStatus.DeActive)
                {
                    //CheckPMInUsing(userEntity.SysNo.Value, CheckPMType.Product);
                    //CheckPMInUsing(userEntity.SysNo.Value, CheckPMType.ProductLine);
                    bool result = ObjectFactory<ProductLineProcessor>.Instance.HasProductByPMUserSysNo(userEntity.SysNo.Value);
                    if (!result) 
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ExistProductManagerByProduct"));
                    }
                }
            }

            /// <summary>
            /// 检查PM编号
            /// </summary>
            /// <param name="productManagerInfoSysNo"></param>
            public static void CheckProductManagerInfoSysNo(int? productManagerInfoSysNo)
            {

                if (productManagerInfoSysNo == null || productManagerInfoSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManagerSysNOIsNull"));
                }
            }

            /// <summary>
            /// 是否存在相同PM
            /// </summary>
            /// <param name="userSysNo"></param>
            /// <param name="productManagerInfoSysNo"></param>
            /// <returns></returns>
            private static void CheckPMUserSysNo(int? userSysNo, int? productManagerInfoSysNo)
            {
                if (userSysNo == null || userSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManageruserSysNoIsNull"));
                }
                if (productManagerInfoSysNo == null)
                {
                    productManagerInfoSysNo = 0;
                }

                if (productManagerInfoSysNo.Value < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManagerSysNOIsNull"));
                }
                var productManagerDA = ObjectFactory<IProductManagerDA>.Instance;
                var isExist = productManagerInfoSysNo == 0 ? productManagerDA.IsExistPMUserSysNo(userSysNo.Value) : productManagerDA.IsExistPMUserSysNo(userSysNo.Value, productManagerInfoSysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ExistProductManageruserSysNo"));
                }
            }

            /// <summary>
            /// 检查被其他用户使用的PM编号
            /// </summary>
            /// <param name="userSysNo"></param>
            /// <param name="usingType"></param>
            /// <returns></returns>
            private static void CheckPMInUsing(int? userSysNo, CheckPMType usingType)
            {
                if (userSysNo == null || userSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManageruserSysNoIsNull"));
                }
                var productManagerDA = ObjectFactory<IProductManagerDA>.Instance;
                var result = false;
                var msg = "";
                switch (usingType)
                {
                    case CheckPMType.Product:
                        result = productManagerDA.IsPMInUsingByProduct(userSysNo.Value);
                        msg = ResouceManager.GetMessageString("IM.ProductManager", "ExistProductManagerByProduct");
                        break;
                    case CheckPMType.ProductLine:
                        result = ObjectFactory<IProductLineDA>.Instance.PMIsUsing(userSysNo.Value);
                        msg = ResouceManager.GetMessageString("IM.ProductManager", "ExistProductManagerByProductLine");
                        break;
                }

                if (result)
                {
                    throw new BizException(msg);
                }
            }


            /// <summary>
            /// 检查PM编号是否存在
            /// </summary>
            /// <param name="userSysNo"></param>
            public static void CheckIsExistPMUserSysNo(int? userSysNo)
            {

                if (userSysNo == null || userSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManageruserSysNoIsNull"));
                }
                var productManagerDA = ObjectFactory<IProductManagerDA>.Instance;
                var result = productManagerDA.IsExistUserSysNo(userSysNo.Value);
                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ExistUserSysNo"));
                }
            }

            /// <summary>
            /// 根据UserID获取UserInfo对象
            /// </summary>
            /// <param name="userID"></param>
            /// <returns></returns>
            public static UserInfo GetUserInfoByUserID(string userID)
            {
                UserInfo rtn = new UserInfo();
                IProductManagerDA productManagerDA = ObjectFactory<IProductManagerDA>.Instance;
                rtn = productManagerDA.GetUserInfoByUserID(userID);

                return rtn;
            }

            /// <summary>
            /// 检查PMID是否存在
            /// </summary>
            /// <param name="userID"></param>
            public static void CheckIsExistPMUserID(string userID)
            {

                if (string.IsNullOrEmpty(userID))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManageruserSysNoIsNull"));
                }
                var productManagerDA = ObjectFactory<IProductManagerDA>.Instance;
                var result = productManagerDA.IsExistUserID(userID);
                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ExistUserSysNo"));
                }
            }

        }

        /// <summary>
        /// PM被使用的类型
        /// </summary>
        private enum CheckPMType
        {
            [Description("商品")]
            Product,
            [Description("商品线")]
            ProductLine
        }
        #endregion

        /// <summary>
        /// 根据条件和权限，查询PM列表信息
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="loginName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<ProductManagerInfo> GetPMList(PMQueryType queryType, string loginName, string companyCode)
        {
            if (queryType != PMQueryType.None)
            {
                return ObjectFactory<IProductManagerDA>.Instance.GetPMListByType(queryType, loginName, companyCode);
            }
            return new List<ProductManagerInfo>();
        }

        public virtual List<ProductManagerInfo> GetPMLeaderList(string companyCode)
        {
            return ObjectFactory<IProductManagerDA>.Instance.GetPMLeaderList(companyCode);
        }
    }
}