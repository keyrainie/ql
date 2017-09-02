using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Utilities
{
    /// <summary>
    /// 获取CodeNamePairList 配置信息类
    /// </summary>
    public class CodeNamePairHelper
    {
        private static readonly string relativeUrl = "/CodeNamePair/GetCodeNamePairByKey";
        private static readonly string relativeUrlBatch = "/CodeNamePair/GetCodeNamePairs";

        /// <summary>
        /// 获取CodeNamePairList 配置信息
        /// </summary>
        /// <param name="domainName">
        /// domain名称 :  [Customer,IM,Inventory,Invoice,MKT,PO,RMA,SO,Common]
        /// </param>
        /// <param name="key">List的key值</param>
        /// <param name="callBack">回调函数</param>
        public static void GetList(string domainName, string key, EventHandler<RestClientEventArgs<List<CodeNamePair>>> callBack)
        {
            GetList(domainName, key, CodeNamePairAppendItemType.None, callBack);
        }

        /// <summary>
        /// 获取CodeNamePairList 配置信息(带默认选项.)
        /// </summary>
        /// <param name="domainName">
        /// domain名称 :  [Customer,IM,Inventory,Invoice,MKT,PO,RMA,SO,Common]
        /// </param>
        /// <param name="key">List的key值</param>
        /// <param name="appendItemType">默认的选项</param>
        /// <param name="callBack">回调函数</param>
        public static void GetList(string domainName, string key, CodeNamePairAppendItemType appendItemType, EventHandler<RestClientEventArgs<List<CodeNamePair>>> callBack)
        {
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");
            CodeNamePairQueryFilter filter = new CodeNamePairQueryFilter
            {
                DomainName = domainName,
                Keys = new[] { key },
                AppendItemType = appendItemType
            };
            restClient.Query<List<CodeNamePair>>(relativeUrl, filter, (obj1, args1) =>
            {
                if (args1.FaultsHandle())
                {
                    return;
                }
                callBack(obj1, args1);
            });
        }


        /// <summary>
        /// 获取CodeNamePairList 配置信息(批量)
        /// </summary>
        /// <param name="domainName">
        /// domain名称 :  [Customer,IM,Inventory,Invoice,MKT,PO,RMA,SO,Common]
        /// </param>
        /// <param name="keys">List的keys</param>
        /// <param name="callBack">回调函数</param>
        public static void GetList(string domainName, string[] keys, EventHandler<RestClientEventArgs<BatchCodeNamePairList>> callBack)
        {
            GetList(domainName, keys, CodeNamePairAppendItemType.None, callBack);
        }

        /// <summary>
        /// 获取CodeNamePairList 配置信息(批量)
        /// </summary>
        /// <param name="domainName">
        /// domain名称 :  [Customer,IM,Inventory,Invoice,MKT,PO,RMA,SO,Common]
        /// </param>
        /// <param name="keys">List的keys</param>
        /// <param name="appendItemType">默认的选项</param>
        /// <param name="callBack">回调函数</param>
        public static void GetList(string domainName, string[] keys, CodeNamePairAppendItemType appendItemType, EventHandler<RestClientEventArgs<BatchCodeNamePairList>> callBack)
        {
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");
            CodeNamePairQueryFilter filter = new CodeNamePairQueryFilter()
            {
                DomainName = domainName,
                Keys = keys,
                AppendItemType = appendItemType
            };
            restClient.Query<BatchCodeNamePairList>(relativeUrlBatch, filter, (obj1, args1) =>
            {
                if (args1.FaultsHandle())
                {
                    return;
                }
                callBack(obj1, args1);
            });
        }
    }
}
