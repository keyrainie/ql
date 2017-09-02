using System.Collections.Generic;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    public static class ExternalDomainBroker
    {
        #region IM

        private static IIMBizInteract _imBizInteract;
        private static IIMBizInteract IMBizInteract
        {
            get
            {
                _imBizInteract = _imBizInteract ?? ObjectFactory<IIMBizInteract>.Instance;
                return _imBizInteract;
            }
        }

        /// <summary>
        /// 获取3级分类信息
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public static CategoryInfo GetCategory3Info(int c3SysNo)
        {
            return IMBizInteract.GetCategory3Info(c3SysNo);
        }

        /// <summary>
        /// 根据三级类获取一级类信息
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public static CategoryInfo GetCategory1ByCategory3(int c3SysNo)
        {
            return IMBizInteract.GetC1ByC3(c3SysNo);
        }

        /// <summary>
        /// 获取所有生产商
        /// </summary>
        /// <returns>所有生产商表</returns>
        public static List<ManufacturerInfo> GetManufacturerList(string companyCode)
        {
            return IMBizInteract.GetManufacturerList(companyCode);
        }

        /// <summary>
        /// 获取所有品牌列表
        /// </summary>
        /// <returns>所有品牌列表</returns>
        public static List<BrandInfo> GetBrandList(string companyCode)
        {
            return IMBizInteract.GetBrandInfoList();
        }

        /// <summary>
        /// 获取有效地PM列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>PM列表</returns>
        public static List<ProductManagerInfo> GetAllValidPMList(string companyCode)
        {
            return IMBizInteract.GetPMListByType(PMQueryType.AllValid, null, companyCode);
        }

        /// <summary>
        /// 根据3级分类获取属性
        /// </summary>
        /// <param name="categorySysNo">3级分类</param>
        /// <returns>属性列表</returns>
        public static List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            return IMBizInteract.GetCategoryPropertyByCategorySysNo(categorySysNo);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="sysNoList">属性编号集合</param>
        /// <returns>属性集合</returns>
        public static Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList)
        {
            return IMBizInteract.GetPropertyValueInfoByPropertySysNoList(sysNoList);
        }

        #endregion

        #region Common Domain

        private static ICommonBizInteract _commonBizInteract;
        private static ICommonBizInteract CommonBizInteract
        {
            get
            {
                _commonBizInteract = _commonBizInteract ?? ObjectFactory<ICommonBizInteract>.Instance;
                return _commonBizInteract;
            }
        }

        internal static UserInfo GetUserBySysNo(int userSysNo)
        {
            return CommonBizInteract.GetUserInfoBySysNo(userSysNo);
        }

        #endregion

        #region PO Domain
        private static IPOBizInteract _poBizInteract;

        public static IPOBizInteract PoBizInteract
        {
            get
            {
                _poBizInteract = _poBizInteract ?? ObjectFactory<IPOBizInteract>.Instance;
                return ExternalDomainBroker._poBizInteract;
            }
        }

        /// <summary>
        /// 获取供应商代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public static List<VendorAgentInfo> GetVendorAgentInfoList(VendorInfo vendorInfo)
        {
            return PoBizInteract.GetVendorAgentInfo(vendorInfo);
        }

        #endregion

        #region SO Domain

        private static ISOBizInteract _soBizInteract;
        private static ISOBizInteract SOBizInteract
        {
            get
            {
                _soBizInteract = _soBizInteract ?? ObjectFactory<ISOBizInteract>.Instance;
                return _soBizInteract;
            }
        }

        public static SOInfo GetSOInfo(int soSysNo)
        {
            return SOBizInteract.GetSOInfo(soSysNo);
        }
        #endregion

        #region RMA Domain

        private static IRMABizInteract _rmaBizInteract;
        private static IRMABizInteract RMABizInteract
        {
            get
            {
                _rmaBizInteract = _rmaBizInteract ?? ObjectFactory<IRMABizInteract>.Instance;
                return _rmaBizInteract;
            }
        }

        public static RefundInfo GetRefund(int refundSysNo)
        {
            return RMABizInteract.GetRefundBySysNo(refundSysNo);
        }

        #endregion
    }
}
