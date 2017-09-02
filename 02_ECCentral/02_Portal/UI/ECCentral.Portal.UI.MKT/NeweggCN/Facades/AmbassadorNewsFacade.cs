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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Facades
{
    public class AmbassadorNewsFacade
    {
          private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        private NewsFacade CurrentNewsFacade;

        public AmbassadorNewsFacade(IPage page)
        {
            CurrentNewsFacade = new NewsFacade(page);
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }


        /// <summary>
        /// 查询泰隆优选大使公告。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void Query(AmbassadorNewsQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<AmbassadorNewsQueryVM, AmbassadorNewsQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/AmbassadorNews/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        public void GetAmbassadorNewsBySysNo(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            PagingInfo p = new PagingInfo
            {
                PageIndex = 0,
                PageSize = 1,
                SortBy = ""
            };
            AmbassadorNewsQueryFilter filter = new AmbassadorNewsQueryFilter();
            filter.SysNo = sysNo;
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = p;

            string relativeUrl = "/MKTService/AmbassadorNews/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);

 
        }

        /// <summary>
        /// 保存泰隆优选大使公告。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SaveAmbassadorNews(AmbassadorNewsVM data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            NewsInfoMaintainVM news = new NewsInfoMaintainVM();
            

            if (data != null)
            {
                news.SysNo = data.SysNo;
                news.NewsType = 12;
                news.Title = data.Title;
                news.Content = data.Content;
                news.Status = (NewsStatus)data.Status;

                ///处理大区信息。
                if (data.ReferenceSysNo < 0)
                {
                    news.ReferenceSysNo = 0;
                }
                else
                {
                    news.ReferenceSysNo = data.ReferenceSysNo;
                }
            }

            bool isCreate = true;
            if (data != null)
            {
                if (data.SysNo > 0)
                {
                    isCreate = false;
                }
                else
                {
                    news.Status = NewsStatus.Deactive;
                }
            }
            NewsInfo entity = news.ConvertVM<NewsInfoMaintainVM, NewsInfo>();
            entity.Title.Content = news.Title;
            entity.Content.Content = news.Content;
            if (isCreate)
            {
                if (CurrentNewsFacade != null)
                {
                    CurrentNewsFacade.Create(entity, callback);
                }
            }
            else
            {
                if (CurrentNewsFacade != null)
                {
                    CurrentNewsFacade.Update(entity, callback);
                }
            }
 
        }

        /// <summary>
        /// 撤销泰隆优选大使公告。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void UndoAmbassadorNews(AmbassadorNewsVM data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            NewsInfoMaintainVM news = new NewsInfoMaintainVM();

            if (data != null)
            {
                news.SysNo = data.SysNo;
                news.NewsType = 12;
                news.Status = NewsStatus.Deactive;
                news.Title = data.Title;
                news.Content = data.Content;
                ///处理大区信息。
                if (data.ReferenceSysNo < 0)
                {
                    news.ReferenceSysNo = 0;
                }
                else
                {
                    news.ReferenceSysNo = data.ReferenceSysNo;
                }

                NewsInfo entity = news.ConvertVM<NewsInfoMaintainVM, NewsInfo>();
                entity.Title.Content = news.Title;
                entity.Content.Content = news.Content;
                if (CurrentNewsFacade != null)
                {


                    CurrentNewsFacade.Update(entity, callback);
                }
            }

            
        }



        /// <summary>
        /// 提交泰隆优选大使公告。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SubmitAmbassadorNews(AmbassadorNewsVM data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            NewsInfoMaintainVM news = new NewsInfoMaintainVM();
            if (data != null)
            {
                news.SysNo = data.SysNo;
                news.NewsType = 12;
                news.Status = NewsStatus.Active;
                news.Title = data.Title;
                news.Content = data.Content;

                ///处理大区信息。
                if (data.ReferenceSysNo < 0)
                {
                    news.ReferenceSysNo = 0;
                }
                else
                {
                    news.ReferenceSysNo = data.ReferenceSysNo;
                }
            }

            bool isCreate = true;
            if (data != null)
            {
                if (data.SysNo > 0)
                {
                    isCreate = false;
                }
            }


            NewsInfo entity = news.ConvertVM<NewsInfoMaintainVM, NewsInfo>();
            entity.Title.Content = news.Title;
            entity.Content.Content = news.Content;
            if (isCreate)
            {
                if (CurrentNewsFacade != null)
                {
                    CurrentNewsFacade.Create(entity, callback);
                }
            }
            else
            {
                if (CurrentNewsFacade != null)
                {
                    CurrentNewsFacade.Update(entity, callback);
                }
            }

        }

        /// <summary>
        /// 批量修改泰隆优选大使公告的状态。
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="status"></param>
        /// <param name="callback"></param>
        public void BatchUpdateAmbassadorNewsStatus(List<int> sysNos, AmbassadorNewsStatus status, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            AmbassadorNewsBatchInfo batchInfo=new AmbassadorNewsBatchInfo();
            batchInfo.AmbassadorNewsSysNos=sysNos;
            batchInfo.Status = status;
            batchInfo.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/MKTService/AmbassadorNews/BatchUpdateAmbassadorNewsStatus";
            restClient.Update(relativeUrl, batchInfo, callback);

        }


    }
}
