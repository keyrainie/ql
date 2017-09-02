//************************************************************************
// 用户名				泰隆优选
// 系统名				 OpenAPI管理
// 子系统名		         OpenAPI管理Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class InernetKeywordFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/MKTService/InternetKeyword/CreateKeyword";
        const string ModifyKeywordStatusoUrl = "/MKTService/InternetKeyword/ModifyOpenAPIStatus";

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public InernetKeywordFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public InernetKeywordFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换OpenAPI视图和OpenAPI实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<InternetKeywordInfo> CovertVMtoEntity(InternetKeywordVM data)
        {
            if (data == null) return new List<InternetKeywordInfo>();
            data.OperateUser = new UserInfo
            {
                SysNo = CPApplication.Current.LoginUser.UserSysNo,
                UserDisplayName = CPApplication.Current.LoginUser.DisplayName
            };
            data.OperateDate = DateTime.Now;
            var dataSource = data.Spilt();
            return dataSource;
        }

        /// <summary>
        /// 创建OpenAPI
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateKeyword(InternetKeywordVM data, EventHandler<RestClientEventArgs<OpenAPIMasterInfo>> callback)
        {
            var tempdata = CovertVMtoEntity(data);
            _restClient.Create(CreateRelativeUrl, tempdata, callback);
        }

        /// <summary>
        /// 批量设置OpenAPI状态
        /// </summary>
        /// <param name="internetKeywordInfoList"></param>
        /// <param name="callback"></param>
        public void ModifyKeywordStatus(List<InternetKeywordInfo> internetKeywordInfoList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (internetKeywordInfoList == null || internetKeywordInfoList.Count == 0) return;
            _restClient.Update(ModifyKeywordStatusoUrl, internetKeywordInfoList, callback);
        }


        #endregion


    }

    public static class InernetKeywordConvert
    {
        public static List<InternetKeywordInfo> Spilt(this InternetKeywordVM vm)
        {
            if (vm == null || String.IsNullOrWhiteSpace(vm.SearchKeyword)) return new List<InternetKeywordInfo>();
            var arrayList = vm.SearchKeyword.Split('\r');
            var _vm = new List<InternetKeywordInfo>();
            for (var i = 0; i < arrayList.Count(); i++)
            {
                var entity = new InternetKeywordInfo
                                 {
                                     OperateDate = vm.OperateDate,
                                     OperateUser = vm.OperateUser,
                                     Searchkeyword = arrayList[i],
                                     Status = vm.Status
                                 };
                _vm.Add(entity);
            }
            return _vm;
        }
    }
}
