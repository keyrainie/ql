using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Facade;
using Nesoft.Utility;
using Nesoft.ECWeb.Entity.Payment;
using Nesoft.ECWeb.MobileService.Models.Payment;
using Nesoft.ECWeb.MobileService.AppCode;
using Nesoft.ECWeb.Facade.Member;

namespace Nesoft.ECWeb.MobileService.Models.Common
{
    public class CommonManager
    {
        public static List<AreaViewModel> GetAllProvinces()
        {
            var list = CommonFacade.GetAllProvince();
            return EntityConverter<List<Area>, List<AreaViewModel>>.Convert(list);
        }

        public static List<AreaViewModel> GetCitiesByProvinceSysNo(int provinceSysNo)
        {
            var list = CommonFacade.GetAllCity(provinceSysNo);
            return EntityConverter<List<Area>, List<AreaViewModel>>.Convert(list);
        }

        public static List<AreaViewModel> GetDistrictsByCitySysNo(int citySysNo)
        {
            var list = CommonFacade.GetAllDistrict(citySysNo);
            return EntityConverter<List<Area>, List<AreaViewModel>>.Convert(list);
        }

        public static PaymentSetting GetPaymentSettingInfo(int payTypeID)
        {
            var entity = CommonFacade.GetPaymentSettingInfoByID(payTypeID);
            return entity;
        }

        public static MobilePaySectionItem GetPaymentSectionInfo(int payTypeID)
        {
            var mobilePaySectionInfo = AppSettings.GetMobilePaySections();
            if (null != mobilePaySectionInfo && mobilePaySectionInfo.PaymentItems.Count > 0)
            {
                return mobilePaySectionInfo.PaymentItems.FirstOrDefault(x => x.PayTypeIdList.Contains(payTypeID));
            }
            return null;
        }

        /// <summary>
        /// 获得售后请求
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePairModel> GetRequests()
        {
           List<CodeNamePair> RequestList = CustomerFacade.GetRequests();
           return TransformCodeNamePair(RequestList);
        }

        /// <summary>
        /// 获得寄回方式
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePairModel> GetShipTypes()
        {
            List<CodeNamePair> ShipTypeList = CustomerFacade.GetShipTypes();
            return TransformCodeNamePair(ShipTypeList);
        }

        /// <summary>
        /// 获得申请理由
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePairModel> GetRMAReasons()
        {
            List<CodeNamePair> RMAReasonList = CustomerFacade.GetRMAReasons();
            return TransformCodeNamePair(RMAReasonList);
        }

        private static List<CodeNamePairModel> TransformCodeNamePair(List<CodeNamePair> rmaList)
        {
            rmaList = rmaList ?? new List<CodeNamePair>();
            List<CodeNamePairModel> result = new List<CodeNamePairModel>();
            foreach (var item in rmaList)
            {
                result.Add(new CodeNamePairModel { Code = item.Code, Name = item.Name });
            }

            return result;
        }
    }
}