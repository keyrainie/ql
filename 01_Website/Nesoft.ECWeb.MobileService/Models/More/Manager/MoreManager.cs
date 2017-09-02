using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Facade.Topic;
using Nesoft.ECWeb.Entity;
using Nesoft.Utility;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.MobileService.AppCode;
using Nesoft.ECWeb.MobileService.Models.Version;
using Nesoft.ECWeb.Facade;
using Nesoft.ECWeb.MobileService.Models.App;

namespace Nesoft.ECWeb.MobileService.Models.More
{
    public class MoreManager
    {
        /// <summary>
        /// 获取关于我们
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public NewsContentModel GetAboutUs()
        {
            var config = AppSettings.GetCachedConfig();
            return GetNewsContent(config.ArticleIDAboutUs);
        }

        /// <summary>
        /// 帮助中心分类列表
        /// </summary>
        /// <returns></returns>
        public List<HelpCategoryModel> GetHelpCategories()
        {
            List<HelpCategoryModel> result = new List<HelpCategoryModel>();
            var c1List = TopicFacade.GetHelperCenterCategory() ?? new List<NewsInfo>();
            foreach (var c1 in c1List)
            {
                if (c1.SysNo == 1)
                {
                    //排除编号为1的根结点
                    continue;
                }
                var categoryModel = MapHelpCategory(c1);

                var c2List = TopicFacade.GetTopHelperCenterList(c1.SysNo.ToString(), 1000) ?? new List<NewsInfo>();
                foreach (var c2 in c2List)
                {
                    var subCategoryModel = MapHelpCategory(c2);
                    categoryModel.SubCategories.Add(subCategoryModel);
                }

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// 根据系统编号获取文章详情
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public NewsContentModel GetNewsContent(int sysNo)
        {
            var news = TopicFacade.GetTopHelperCenterBySysNo(sysNo);
            if (news != null)
            {
                return MapHelpContent(news);
            }

            throw new BusinessException(string.Format("内容#{0}不存在。", sysNo.ToString()));
        }

        /// <summary>
        /// 插入用户留言
        /// </summary>
        /// <param name="leaveWordsModel"></param>
        /// <returns></returns>
        public bool InsertLeaveWords(UserLeaveWordsModel leaveWordsModel)
        {
            if (!leaveWordsModel.UserEmail.IsEmail())
            {
                throw new BusinessException("请输入有效的邮箱地址。");
            }
            if (string.IsNullOrWhiteSpace(leaveWordsModel.Content))
            {
                throw new BusinessException("请输入反馈内容。");
            }
            CustomerLeaveWords entity = new CustomerLeaveWords();
            entity.InDate = DateTime.Now;
            entity.Subject = HeaderHelper.GetClientType().ToString();
            entity.LeaveWords = leaveWordsModel.Content;
            entity.CustomerEmail = leaveWordsModel.UserEmail;
            entity.CompanyCode = ConstValue.CompanyCode;
            entity.LanguageCode = ConstValue.LanguageCode;
            entity.StoreCompanyCode = ConstValue.StoreCompanyCode;

            var loginedUser = UserMgr.ReadUserInfo();
            if (loginedUser != null)
            {
                entity.CustomerSysNo = loginedUser.UserSysNo;
                entity.CustomerName = loginedUser.UserID;
            }

            return CustomerFacade.InsertCustomerLeaveWords(entity);
        }

        private NewsContentModel MapHelpContent(NewsInfo newsInfo)
        {
            NewsContentModel categoryModel = new NewsContentModel();
            categoryModel.SysNo = newsInfo.SysNo;
            categoryModel.Title = newsInfo.Title;
            var config = AppSettings.GetCachedConfig();
            categoryModel.Content = config.NewsContentTemplate.Replace("${content}", newsInfo.Content);

            return categoryModel;
        }

        private HelpCategoryModel MapHelpCategory(NewsInfo newsInfo)
        {
            HelpCategoryModel categoryModel = new HelpCategoryModel();
            categoryModel.SysNo = newsInfo.SysNo;
            categoryModel.Title = newsInfo.Title;

            return categoryModel;
        }
        /// <summary>
        /// 添加崩溃报告
        /// </summary>
        /// <param name="content"></param>
        public void AddCrashLog(string content)
        {
            ClientType clientType = HeaderHelper.GetClientType();

            List<KeyValuePair<string, object>> extList = new List<KeyValuePair<string, object>>();
            extList.Add(new KeyValuePair<string, object>("X-OSVersion", HeaderHelper.GetOSVersion()));
            extList.Add(new KeyValuePair<string, object>("X-HighResolution", HeaderHelper.GetHighResolution()));
            Logger.WriteLog(content, clientType.ToString(), "", extList);

            MobileAppConfig config = AppSettings.GetCachedConfig();
            content = content.Replace("\r\n", "<br/>");
            content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br/>" + content;
            EmailFacade.SendEmail(clientType.ToString() + "-app-crash-log", content, config.CrashLogEmailTo, "mallsupport@zjtlcb.com");
        }
    }
}