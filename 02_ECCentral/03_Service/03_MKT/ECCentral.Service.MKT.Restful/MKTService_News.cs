using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private NewsAppService newsAppService = ObjectFactory<NewsAppService>.Instance;

        #region 公告及促销评论管理

        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/NewsInfo/BatchSetNewsAdvReplyShow", Method = "PUT")]
        public virtual void BatchSetNewsAdvReplyShow(List<int> items)
        {
            newsAppService.BatchSetNewsAdvReplyShow(items);
        }

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/NewsInfo/BatchSetNewsAdvReplyHide", Method = "PUT")]
        public virtual void BatchSetNewsAdvReplyHide(List<int> items)
        {
            newsAppService.BatchSetNewsAdvReplyHide(items);
        }

        /// <summary>
        ///评论回复 
        /// </summary>
        [WebInvoke(UriTemplate = "/NewsInfo/CreateNewsAdvReply", Method = "POST")]
        public virtual void CreateNewsAdvReply(NewsAdvReply item)
        {
            newsAppService.CreateNewsAdvReply(item);
        }

        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/NewsInfo/UpdateNewsAdvReply", Method = "PUT")]
        public void UpdateNewsAdvReply(NewsAdvReply item)
        {
            newsAppService.UpdateNewsAdvReply(item);
        }

        /// <summary>
        /// 获取公告及促销评论回复创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NewsInfo/GetNewAdvReplyCreateUsers", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<UserInfo> GetNewAdvReplyCreateUsers(string companyCode)
        {
            string channelID = "0";
            return newsAppService.GetNewAdvReplyCreateUsers(companyCode, channelID);
        }

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NewsInfo/LoadNewsAdvReply", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public NewsAdvReply LoadNewsAdvReply(int sysNo)
        {
            return newsAppService.LoadNewsAdvReply(sysNo);
        }

        /// <summary>
        /// 公告及促销评论查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NewsInfo/QueryNewsAdvReply", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryNewsAdvReply(NewsAdvReplyQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<INewsQueryDA>.Instance.QueryNewsAdvReply(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion

        /// <summary>
        /// 创建新闻公告
        /// </summary>
        [WebInvoke(UriTemplate = "/NewsInfo/CreateNewsInfo", Method = "POST")]
        public int? CreateNewsInfo(NewsInfo msg)
        {
            return newsAppService.CreateNewsInfo(msg);
        }

        /// <summary>
        /// 更新新闻公告
        /// </summary>
        [WebInvoke(UriTemplate = "/NewsInfo/UpdateNewsInfo", Method = "PUT")]
        public void UpdateNewsInfo(NewsInfo msg)
        {
            newsAppService.UpdateNewsInfo(msg);
        }


        /// <summary>
        /// GET新闻公告
        /// </summary>
        [WebInvoke(UriTemplate = "/LoadNewsInfo/{SysNo}", Method = "GET")]
        public NewsInfo LoadNewsInfo(string SysNo)
        {
            return newsAppService.LoadNewsInfo(int.Parse(SysNo));
        }

        /// <summary>
        /// 屏蔽新闻公告
        /// </summary>
        [WebInvoke(UriTemplate = "/NewsInfo/CancelNewsInfo", Method = "PUT")]
        public void CancelNewsInfo(List<int> newsID)
        {
            newsAppService.CancelNewsInfo(newsID);
        }

        /// <summary>
        /// 新闻公告查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NewsInfo/QueryNews", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult Query(NewsInfoQueryFilter msg)
        {
            int totalCount;
            DataSet ds = ObjectFactory<INewsQueryDA>.Instance.QueryNews(msg, out totalCount);
            DataTable dtNews = ds.Tables[0];
            DataTable dtBannerArea = ds.Tables[1];
            foreach (DataRow drBanner in dtNews.Rows)
            {
                string areaInfo = "";
                foreach (DataRow drArea in dtBannerArea.Rows)
                {
                    if (drBanner["SysNo"].ToString() == drArea["RefSysNo"].ToString())
                    {
                        //用逗号分隔主要投放区域
                        areaInfo += drArea["AreaSysNo"] + ",";
                    }
                }
                drBanner["AreaShow"] = areaInfo.TrimEnd(',');
            }
            return new QueryResult()
            {
                Data = dtNews,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取所有创建人列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NewsInfo/GetCreateUsers", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        //[DataTableSerializeOperationBehavior]
        public virtual List<UserInfo> GetCreateUsers(string companyCode)
        {
            string channelID = "0";
            return newsAppService.GetCreateUsers(companyCode, channelID);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="image"></param>
        [WebInvoke(UriTemplate = "/NewsInfo/DeleteNewsAdvReplyImage", Method = "POST")]
        public void DeleteNewsAdvReplyImage(string image)
        {
            newsAppService.DeleteNewsAdvReplyImage(image);
        }
    }
}
