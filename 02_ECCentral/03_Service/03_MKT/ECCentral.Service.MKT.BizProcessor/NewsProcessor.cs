using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using System.Data;
using ECCentral.BizEntity.Common;
using System.Transactions;
using System.Text.RegularExpressions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(NewsProcessor))]
    public class NewsProcessor
    {
        #region 公告及促销评论管理

        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchSetNewsAdvReplyShow(List<int> items)
        {
            newsDA.BatchSetNewsAdvReplyShow(items);
        }

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchSetNewsAdvReplyHide(List<int> items)
        {
            newsDA.BatchSetNewsAdvReplyHide(items);
        }

        /// <summary>
        /// 评论回复
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateNewsAdvReply(NewsAdvReply item)
        {
            newsDA.CreateNewsAdvReply(item);
        }

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual NewsAdvReply LoadNewsAdvReply(int sysNo)
        {
            return newsDA.LoadNewsAdvReply(sysNo);
        }

        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateNewsAdvReply(NewsAdvReply item)
        {
            newsDA.UpdateNewsAdvReply(item);
        }

        /// <summary>
        /// 获取公告及促销评论回复创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道</param>
        /// <returns></returns>
        public virtual List<UserInfo> GetNewAdvReplyCreateUsers(string companyCode, string channelID)
        {
            return newsDA.GetNewAdvReplyCreateUsers(companyCode, channelID);
        }
        #endregion

        private INewsDA newsDA = ObjectFactory<INewsDA>.Instance;

        public virtual int? CreateNewsInfo(NewsInfo entity)
        {
            ValidateEntity(entity);
            //处理飘红
            if (entity.IsRed.HasValue && entity.IsRed.Value)
            {
                entity.Title.Content = GetRedTitle(entity.Title.Content);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                //1.插入新闻公告
                entity = newsDA.CreateNewsInfo(entity);
                //2.插入新闻地区关系
                if (entity.AreaShow != null && entity.AreaShow.Count() > 0)
                {
                    foreach (var area in entity.AreaShow)
                    {
                        ObjectFactory<IAreaRelationDA>.Instance.Create(area, entity.SysNo.Value, AreaRelationType.News);
                    }
                }
                scope.Complete();

                return entity.SysNo;
            }

            return null;
        }

        public virtual void UpdateNewsInfo(NewsInfo entity)
        {
            ValidateEntity(entity);
            //处理飘红
            if (entity.IsRed.HasValue && entity.IsRed.Value)
            {
                entity.Title.Content = GetRedTitle(entity.Title.Content);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                //1.更新新闻地区关系
                if (entity.AreaShow != null && entity.AreaShow.Count() > 0)
                {
                    NewsInfo orginal = GetNewsInfo(entity.SysNo.Value);
                    var temp = entity.AreaShow.Intersect(orginal.AreaShow).ToList<int>();
                    //删除
                    foreach (int r in orginal.AreaShow.Except(temp).ToList<int>())
                    {
                        ObjectFactory<IAreaRelationDA>.Instance.Delete(r, entity.SysNo.Value, AreaRelationType.News);
                    }
                    //新增
                    foreach (int r in entity.AreaShow.Except(temp).ToList<int>())
                    {
                        ObjectFactory<IAreaRelationDA>.Instance.Create(r, entity.SysNo.Value, AreaRelationType.News);
                    }
                }
                //2.更新新闻公告
                newsDA.UpdateNewsInfo(entity);
                string Areas=string.Empty;
                entity.AreaShow.ForEach(p => { Areas += String.Format("{0}、",p.ToString());});
                ExternalDomainBroker.CreateOperationLog(
                               String.Format(@"{0}{1}SysNo:{2}|所属渠道:{3}|标题:{4}|正文链接{5}|正文内容{6}|主要投放区域{7}|新闻类型{8}|过期时间{9}|是否置顶{10}
                                |是否飘红{11}|是否允许评论{12}|顾客可评论级别{13}|状态{14}",
                               DateTime.Now.ToString(), "更新新闻公告"
                               , entity.SysNo, entity.WebChannel==null?"":entity.WebChannel.ChannelID, new Regex("<(.[^>]*)>", RegexOptions.IgnoreCase)
                                .Replace(entity.Title.ToString().Trim(), ""), entity.LinkUrl, entity.Content
                               , Areas, entity.NewsType, entity.ExpireDate, entity.TopMost, entity.IsRed
                               , entity.EnableComment, entity.EnableReplyRank, entity.Status)
                               , BizEntity.Common.BizLogType.ComputerConfig_Add
                               , entity.SysNo.Value, entity.CompanyCode);

                scope.Complete();
            }
        }

        public virtual NewsInfo GetNewsInfo(int sysNo)
        {
            NewsInfo entity = newsDA.Load(sysNo);
            entity.AreaShow = ObjectFactory<IAreaRelationDA>.Instance.GetSelectedArea(entity.SysNo.Value, AreaRelationType.News);
            return entity;
        }

        public virtual void CancelNewsInfo(int newsID)
        {
            ObjectFactory<INewsDA>.Instance.UpdateStatus(newsID, NewsStatus.Deactive);
        }

        public virtual List<UserInfo> GetCreateUsers(string companyCode, string channelID)
        {
            return newsDA.GetCreateUsers(companyCode, channelID);
        }

        private string GetRedTitle(string title)
        {
            return "<font color='#ff6600'>" + title + "</font>";
        }

        public virtual void ValidateEntity(NewsInfo entity)
        {            
            //如果选择飘红，后台增加html代码
            if (entity.IsRed.HasValue && entity.IsRed.Value)
            {
                string redTitle = GetRedTitle(entity.Title.Content);
                if (redTitle.Length > 100)
                {
                    //throw new BizException("标题字数加上飘红代码不能超过100！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "News_TitleLengthError100"));
                }
            }
            else
            {
                if (entity.Title.Content.Length > 100)
                {
                    //throw new BizException("标题字数不能超过100！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "News_TitleCharLengthError100"));
                }

            }


            if (string.IsNullOrEmpty(entity.LinkUrl) && (string.IsNullOrEmpty(entity.Content.Content)))
            {
                //throw new BizException("正文链接和正文内容不可都为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "News_ContentIsNotNull"));
            }

            if (entity.Title.Content.IndexOf("<") >= 0 || entity.Title.Content.IndexOf(">") >= 0)
            {
                //throw new BizException("标题不能包含Html脚本！");
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "News_TitleCanntContainhtml"));
            }

            if (entity.ExpireDate < DateTime.Now)
            {
                //throw new BizException("过期时间不能小于当前时间！");
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "News_ExpireDateMoreThanCurrentDate"));
            }
        }

        /// <summary>
        /// 删除相关图片
        /// </summary>
        /// <param name="image"></param>
        public void DeleteNewsAdvReplyImage(string image)
        {
            newsDA.DeleteNewsAdvReplyImage(image);
        }
    }
}
