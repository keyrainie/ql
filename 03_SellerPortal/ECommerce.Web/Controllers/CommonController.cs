using ECommerce.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ECommerce.Utility;
using ECommerce.Entity.Common;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Entity;

namespace ECommerce.Web.Controllers
{
    public class CommonController : WWWControllerBase
    {
        /// <summary>
        /// 获取所有省份
        /// </summary>
        /// <returns></returns>
        public ActionResult Province(bool hasCountry = false, bool hasRegion = false)
        {
            var list = CommonService.GetAllProvince();
            if (hasCountry)
            {
                list.Insert(0, new AreaInfo() { SysNo = -1, ProvinceName = "中国" });
            }
            if (hasRegion)
            {
                list.Insert(1, new AreaInfo() { SysNo = -2, ProvinceName = "华东" });
                list.Insert(2, new AreaInfo() { SysNo = -3, ProvinceName = "华南" });
                list.Insert(3, new AreaInfo() { SysNo = -4, ProvinceName = "华北" });
                list.Insert(4, new AreaInfo() { SysNo = -5, ProvinceName = "华中" });
                list.Insert(5, new AreaInfo() { SysNo = -6, ProvinceName = "西南" });
                list.Insert(6, new AreaInfo() { SysNo = -7, ProvinceName = "西北" });
                list.Insert(7, new AreaInfo() { SysNo = -8, ProvinceName = "东北" });
            }
            var data = list.Select(t => new { Code = t.SysNo.ToString(), Name = t.ProvinceName });
            return Json(data);
        }
        /// <summary>
        /// 获取省份下面所有城市
        /// </summary>
        /// <param name="provinceSysNo"></param>
        /// <returns></returns>
        public ActionResult City(int provinceSysNo)
        {
            string strProvinceSysNo="";
            switch (provinceSysNo)
            {
                case -1:
                    strProvinceSysNo = ConstValue.regionCountry;
                    break;
                case -2:
                    strProvinceSysNo = ConstValue.regionHuadong;
                    break;
                case -3:
                    strProvinceSysNo = ConstValue.regionHuanan;
                    break;
                case -4:
                    strProvinceSysNo = ConstValue.regionHuabei;
                    break;
                case -5:
                    strProvinceSysNo = ConstValue.regionHuazhong;
                    break;
                case -6:
                    strProvinceSysNo = ConstValue.regionXinan;
                    break;
                case -7:
                    strProvinceSysNo = ConstValue.regionXibei;
                    break;
                case -8:
                    strProvinceSysNo = ConstValue.regionDongbei;
                    break;
                default:
                    strProvinceSysNo = provinceSysNo.ToString();
                    break;
            }
            List<AreaInfo> list = new List<AreaInfo>();
            if (provinceSysNo == -1)
            {
                list.Add(new AreaInfo() { SysNo = null, CityName = ConstValue.regionCountry });
            }
            else
            {
                list = CommonService.GetAllCity(strProvinceSysNo);
            } 
            var data = list.Select(t => new { Code = t.SysNo.ToString(), Name = t.CityName });
            return Json(data);
        }
        /// <summary>
        /// 获取城市下面所有区域
        /// </summary>
        /// <param name="citySysNo"></param>
        /// <returns></returns>
        public ActionResult District(int citySysNo)
        {
            var list = CommonService.GetAllDistrict(citySysNo);
            var data = list.Select(t => new { Code = t.SysNo.ToString(), Name = t.DistrictName == null ? "所有区" : t.DistrictName });
            return Json(data);
        }
        /// <summary>
        /// 查询区域
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryArea()
        {
            var queryFilter = SerializationUtility.JsonDeserialize2<AreaInfoQueryFilter>(this.Request["queryfilter[]"]);
            switch (queryFilter.ProvinceSysNo)
            {
                case "-1":
                    queryFilter.ProvinceSysNo = null;
                    break;
                case "-2":
                    queryFilter.ProvinceSysNo = ConstValue.regionHuadong;
                    break;
                case "-3":
                    queryFilter.ProvinceSysNo = ConstValue.regionHuanan;
                    break;
                case "-4":
                    queryFilter.ProvinceSysNo = ConstValue.regionHuabei;
                    break;
                case "-5":
                    queryFilter.ProvinceSysNo = ConstValue.regionHuazhong;
                    break;
                case "-6":
                    queryFilter.ProvinceSysNo = ConstValue.regionXinan;
                    break;
                case "-7":
                    queryFilter.ProvinceSysNo = ConstValue.regionXibei;
                    break;
                case "-8":
                    queryFilter.ProvinceSysNo = ConstValue.regionDongbei;
                    break;
            }
            var data = CommonService.QueryArea(queryFilter);
            return AjaxGridJson(data);
        }

        public PartialViewResult AjaxShowDialog()
        {
            var url = Request.QueryString["url"];
            return PartialView(url);
        }

        public ActionResult AjaxQueryBrandList()
        {
            BrandQueryFilter qFilter = BuildQueryFilterEntity<BrandQueryFilter>();
            var result = CommonService.QueryBrandList(qFilter);
            return AjaxGridJson(result);
        }

        public ActionResult AjaxQueryManufacturerList()
        {
            ManufacturerQueryFilter qFilter = BuildQueryFilterEntity<ManufacturerQueryFilter>();
            var result = CommonService.QueryManufacturerList(qFilter);
            return AjaxGridJson(result);
        }
    }
}
