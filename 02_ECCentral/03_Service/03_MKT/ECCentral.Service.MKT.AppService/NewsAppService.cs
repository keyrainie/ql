using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
//using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using System.Data;
using System.Transactions;


namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(NewsAppService))]
    public class NewsAppService
    {
        #region 公告及促销评论管理
        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchSetNewsAdvReplyShow(List<int> items)
        {
            ObjectFactory<NewsProcessor>.Instance.BatchSetNewsAdvReplyShow(items);
        }

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchSetNewsAdvReplyHide(List<int> items)
        {
            ObjectFactory<NewsProcessor>.Instance.BatchSetNewsAdvReplyHide(items);
        }

        /// <summary>
        /// 评论回复
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateNewsAdvReply(NewsAdvReply item)
        {
            ObjectFactory<NewsProcessor>.Instance.CreateNewsAdvReply(item);
        }

        /// <summary>
        /// 获取公告及促销评论回复创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道</param>
        /// <returns></returns>
        public virtual List<UserInfo> GetNewAdvReplyCreateUsers(string companyCode, string channelID)
        {
            return ObjectFactory<NewsProcessor>.Instance.GetNewAdvReplyCreateUsers(companyCode, channelID);
        }

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual NewsAdvReply LoadNewsAdvReply(int sysNo)
        {
            return ObjectFactory<NewsProcessor>.Instance.LoadNewsAdvReply(sysNo);
        }

        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateNewsAdvReply(NewsAdvReply item)
        {
            ObjectFactory<NewsProcessor>.Instance.UpdateNewsAdvReply(item);
        }
        #endregion

        public virtual int? CreateNewsInfo(NewsInfo entity)
        {
            int? result;
            //如果扩展生效的话，需要为该C3相关的前台类别插入相同的新闻
            if (entity.Extendflag.HasValue && entity.Extendflag.Value)
            {
                var list = ObjectFactory<ECCategoryProcessor>.Instance.GetRelatedECCategory3SysNo(entity.ReferenceSysNo.Value);
                list.Add(new ECCategory() { SysNo = entity.ReferenceSysNo.Value });
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var item in list)
                    {
                        entity.ReferenceSysNo = item.SysNo;
                        ObjectFactory<NewsProcessor>.Instance.CreateNewsInfo(entity);
                    }
                    scope.Complete();
                }
                result = null;
            }
            else
            {
                result = ObjectFactory<NewsProcessor>.Instance.CreateNewsInfo(entity);
            }
            ExternalDomainBroker.CreateOperationLog(
                String.Format("{0}{1}SysNo:{2}",
                DateTime.Now.ToString(), ResouceManager.GetMessageString("MKT.News", "News_CreateNewsAndNotice")
                , entity.SysNo)
                , BizEntity.Common.BizLogType.ComputerConfig_Add
                , entity.SysNo.Value, entity.CompanyCode);
            return result;
        }

        public virtual void UpdateNewsInfo(NewsInfo entity)
        {
            ObjectFactory<NewsProcessor>.Instance.UpdateNewsInfo(entity);
        }

        public virtual void CancelNewsInfo(List<int> newsID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (int i in newsID)
                {
                    ObjectFactory<NewsProcessor>.Instance.CancelNewsInfo(i);
                }
                scope.Complete();
            }
        }

        public virtual List<UserInfo> GetCreateUsers(string companyCode, string channelID)
        {
            return ObjectFactory<NewsProcessor>.Instance.GetCreateUsers(companyCode, channelID);
        }

        public NewsInfo LoadNewsInfo(int SysNo)
        {
            return ObjectFactory<NewsProcessor>.Instance.GetNewsInfo(SysNo);
        }

        /// <summary>
        /// 删除相关图片
        /// </summary>
        /// <param name="image"></param>
        public void DeleteNewsAdvReplyImage(string image)
        {
            ObjectFactory<NewsProcessor>.Instance.DeleteNewsAdvReplyImage(image);
        }
    }
}
