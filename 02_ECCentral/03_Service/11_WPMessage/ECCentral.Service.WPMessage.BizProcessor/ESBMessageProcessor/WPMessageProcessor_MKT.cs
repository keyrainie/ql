using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    #region 促销_优惠券活动审核

    /// <summary>
    /// 提交审核消息
    /// </summary>
    public class CouponSubmitMessageCreator : WPMessageCreator<CouponSubmitMessage>
    {
        protected override bool NeedProcess(CouponSubmitMessage msg)
        {
            throw new NotImplementedException();
            //CouponsInfo info = ObjectFactory<IMKTBizInteract>.Instance.GetCouponInfoByCouponSysNo(msg.CouponSysNo);
            //if (null == info)
            //    return false;
            //if (info.Status == CouponsStatus.WaitingAudit)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override int GetCategorySysNo()
        {
            return 412;
        }

        protected override string GetBizSysNo(CouponSubmitMessage msg)
        {
            return msg.CouponSysNo.ToString();
        }

        protected override string GetUrlParameter(CouponSubmitMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.CouponSysNo.ToString());
        }

        protected override string GetMemo(CouponSubmitMessage msg)
        {
            return "创建审核优惠券活动待办事项";
        }

        protected override int GetCurrentUserSysNo(CouponSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class CouponAuditMessageCompleter : WPMessageCompleter<CouponAuditMessage>
    {
        protected override bool NeedProcess(CouponAuditMessage msg)
        {
            return msg.CouponSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 412;
        }

        protected override string GetBizSysNo(CouponAuditMessage msg)
        {
            return msg.CouponSysNo.ToString();
        }

        protected override string GetUrlParameter(CouponAuditMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.CouponSysNo.ToString());
        }

        protected override string GetMemo(CouponAuditMessage msg)
        {
            return "优惠券活动待办事项关闭";
        }

        protected override int GetCurrentUserSysNo(CouponAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核取消
    /// </summary>
    public class CouponAuditCancelMessageCompleter : WPMessageCompleter<CouponAuditCancelMessage>
    {
        protected override bool NeedProcess(CouponAuditCancelMessage msg)
        {
            return msg.CouponSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 412;
        }

        protected override string GetBizSysNo(CouponAuditCancelMessage msg)
        {
            return msg.CouponSysNo.ToString();
        }

        protected override string GetUrlParameter(CouponAuditCancelMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.CouponSysNo.ToString());
        }

        protected override string GetMemo(CouponAuditCancelMessage msg)
        {
            return "优惠券活动待办事项关闭";
        }

        protected override int GetCurrentUserSysNo(CouponAuditCancelMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public class CouponAuditRejectMessageCompleter : WPMessageCompleter<CouponAuditRejectMessage>
    {
        protected override bool NeedProcess(CouponAuditRejectMessage msg)
        {
            return msg.CouponSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 412;
        }

        protected override string GetBizSysNo(CouponAuditRejectMessage msg)
        {
            return msg.CouponSysNo.ToString();
        }

        protected override string GetUrlParameter(CouponAuditRejectMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.CouponSysNo.ToString());
        }

        protected override string GetMemo(CouponAuditRejectMessage msg)
        {
            return "优惠券活动待办事项关闭";
        }

        protected override int GetCurrentUserSysNo(CouponAuditRejectMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class CouponVoidMessageCompleter : WPMessageCompleter<CouponVoidMessage>
    {
        protected override bool NeedProcess(CouponVoidMessage msg)
        {
            return msg.CouponSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 412;
        }

        protected override string GetBizSysNo(CouponVoidMessage msg)
        {
            return msg.CouponSysNo.ToString();
        }

        protected override string GetUrlParameter(CouponVoidMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.CouponSysNo.ToString());
        }

        protected override string GetMemo(CouponVoidMessage msg)
        {
            return "作废关闭优惠券活动待办事项";
        }

        protected override int GetCurrentUserSysNo(CouponVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    #endregion

    #region 促销_赠品活动审核

    /// <summary>
    /// 提交审核消息
    /// </summary>
    public class SaleGiftSubmitMessageCreator : WPMessageCreator<SaleGiftSubmitMessage>
    {
        protected override bool NeedProcess(SaleGiftSubmitMessage msg)
        {
            throw new NotImplementedException();
            //SaleGiftInfo info = ObjectFactory<IMKTBizInteract>.Instance.GetSaleGiftInfoById(msg.SaleGiftSysNo);
            //if (null == info)
            //{
            //    return false;
            //}

            //if (info.Status == SaleGiftStatus.WaitingAudit)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override int GetCategorySysNo()
        {
            return 411;
        }

        protected override string GetBizSysNo(SaleGiftSubmitMessage msg)
        {
            return msg.SaleGiftSysNo.ToString();
        }

        protected override string GetUrlParameter(SaleGiftSubmitMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.SaleGiftSysNo.ToString());
        }

        protected override string GetMemo(SaleGiftSubmitMessage msg)
        {
            return "创建审核赠品活动待办事项";
        }

        protected override int GetCurrentUserSysNo(SaleGiftSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class SaleGiftAuditMessageCompleter : WPMessageCompleter<SaleGiftAuditMessage>
    {
        protected override bool NeedProcess(SaleGiftAuditMessage msg)
        {
            return msg.SaleGiftSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 411;
        }

        protected override string GetBizSysNo(SaleGiftAuditMessage msg)
        {
            return msg.SaleGiftSysNo.ToString();
        }

        protected override string GetUrlParameter(SaleGiftAuditMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.SaleGiftSysNo.ToString());
        }

        protected override string GetMemo(SaleGiftAuditMessage msg)
        {
            return "赠品活动待办事项关闭";
        }

        protected override int GetCurrentUserSysNo(SaleGiftAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核取消
    /// </summary>
    public class SaleGiftAuditCancelMessageCompleter : WPMessageCompleter<SaleGiftAuditCancelMessage>
    {
        protected override bool NeedProcess(SaleGiftAuditCancelMessage msg)
        {
            return msg.SaleGiftSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 411;
        }

        protected override string GetBizSysNo(SaleGiftAuditCancelMessage msg)
        {
            return msg.SaleGiftSysNo.ToString();
        }

        protected override string GetUrlParameter(SaleGiftAuditCancelMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.SaleGiftSysNo.ToString());
        }

        protected override string GetMemo(SaleGiftAuditCancelMessage msg)
        {
            return "取消审核关闭赠品活动待办事项";
        }

        protected override int GetCurrentUserSysNo(SaleGiftAuditCancelMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public class SaleGiftAuditRejectMessageCompleter : WPMessageCompleter<SaleGiftAuditRejectMessage>
    {
        protected override bool NeedProcess(SaleGiftAuditRejectMessage msg)
        {
            return msg.SaleGiftSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 411;
        }

        protected override string GetBizSysNo(SaleGiftAuditRejectMessage msg)
        {
            return msg.SaleGiftSysNo.ToString();
        }

        protected override string GetUrlParameter(SaleGiftAuditRejectMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.SaleGiftSysNo.ToString());
        }

        protected override string GetMemo(SaleGiftAuditRejectMessage msg)
        {
            return "审核拒绝关闭赠品活动待办事项";
        }

        protected override int GetCurrentUserSysNo(SaleGiftAuditRejectMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class SaleGiftVoidMessageCompleter : WPMessageCompleter<SaleGiftVoidMessage>
    {
        protected override bool NeedProcess(SaleGiftVoidMessage msg)
        {
            return msg.SaleGiftSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 411;
        }

        protected override string GetBizSysNo(SaleGiftVoidMessage msg)
        {
            return msg.SaleGiftSysNo.ToString();
        }

        protected override string GetUrlParameter(SaleGiftVoidMessage msg)
        {
            return string.Format("?sysno={0}&operation=mgt", msg.SaleGiftSysNo.ToString());
        }

        protected override string GetMemo(SaleGiftVoidMessage msg)
        {
            return "作废关闭赠品活动待办事项";
        }

        protected override int GetCurrentUserSysNo(SaleGiftVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 促销_团购审核

    /// <summary>
    /// 保存消息
    /// </summary>
    public class GroupBuySaveMessageCreator : WPMessageCreator<GroupBuySaveMessage>
    {
        protected override bool NeedProcess(GroupBuySaveMessage msg)
        {
            GroupBuyingInfo groupBuy = ObjectFactory<IMKTBizInteract>.Instance.GetGroupBuyInfoBySysNo(msg.GroupBuySysNo);
            if (null == groupBuy)
                return false;
            if (groupBuy.Status == GroupBuyingStatus.Init)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override int GetCategorySysNo()
        {
            return 410;
        }

        protected override string GetBizSysNo(GroupBuySaveMessage msg)
        {
            return msg.GroupBuySysNo.ToString();
        }

        protected override string GetUrlParameter(GroupBuySaveMessage msg)
        {
            return string.Format("?sysNo={0}", msg.GroupBuySysNo.ToString());
        }

        protected override string GetMemo(GroupBuySaveMessage msg)
        {
            return "创建团购审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GroupBuySaveMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    /// <summary>
    /// 更新消息
    /// </summary>
    public class GroupBuyUpdateMessageCreator : WPMessageCreator<GroupBuyUpdateMessage>
    {
        protected override bool NeedProcess(GroupBuyUpdateMessage msg)
        {
            GroupBuyingInfo groupBuy = ObjectFactory<IMKTBizInteract>.Instance.GetGroupBuyInfoBySysNo(msg.GroupBuySysNo);
            if (null == groupBuy)
                return false;
            if (groupBuy.Status == GroupBuyingStatus.Init)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override int GetCategorySysNo()
        {
            return 410;
        }

        protected override string GetBizSysNo(GroupBuyUpdateMessage msg)
        {
            return msg.GroupBuySysNo.ToString();
        }

        protected override string GetUrlParameter(GroupBuyUpdateMessage msg)
        {
            return string.Format("?sysNo={0}", msg.GroupBuySysNo.ToString());
        }

        protected override string GetMemo(GroupBuyUpdateMessage msg)
        {
            return "创建团购审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GroupBuyUpdateMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class GroupBuyAuditMessageCompleter : WPMessageCompleter<GroupBuyAuditMessage>
    {
        protected override bool NeedProcess(GroupBuyAuditMessage msg)
        {
            return msg.GroupBuySysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 410;
        }

        protected override string GetBizSysNo(GroupBuyAuditMessage msg)
        {
            return msg.GroupBuySysNo.ToString();
        }

        protected override string GetUrlParameter(GroupBuyAuditMessage msg)
        {
            return string.Format("?sysNo={0}", msg.GroupBuySysNo.ToString());
        }

        protected override string GetMemo(GroupBuyAuditMessage msg)
        {
            return "关闭团购审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GroupBuyAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class GroupBuyVoidMessageCompleter : WPMessageCompleter<GroupBuyVoidMessage>
    {
        protected override bool NeedProcess(GroupBuyVoidMessage msg)
        {
            return msg.GroupBuySysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 410;
        }

        protected override string GetBizSysNo(GroupBuyVoidMessage msg)
        {
            return msg.GroupBuySysNo.ToString();
        }

        protected override string GetUrlParameter(GroupBuyVoidMessage msg)
        {
            return string.Format("?sysNo={0}", msg.GroupBuySysNo.ToString());
        }

        protected override string GetMemo(GroupBuyVoidMessage msg)
        {
            return "关闭团购审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GroupBuyVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 促销_套餐审核

    /// <summary>
    /// 提交审核消息
    /// </summary>
    public class ComboSaleSubmitMessageCreator : WPMessageCreator<ComboSaleSubmitMessage>
    {
        protected override bool NeedProcess(ComboSaleSubmitMessage msg)
        {
            ComboInfo comboInfo = ObjectFactory<IMKTBizInteract>.Instance.GetComboList(new List<int>{msg.ComboSaleSysNo}).FirstOrDefault();
            if (null == comboInfo)
                return false;
            if (comboInfo.Status == ComboStatus.WaitingAudit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override int GetCategorySysNo()
        {
            return 409;
        }

        protected override string GetBizSysNo(ComboSaleSubmitMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetUrlParameter(ComboSaleSubmitMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetMemo(ComboSaleSubmitMessage msg)
        {
            return "审核套餐待办事项";
        }

        protected override int GetCurrentUserSysNo(ComboSaleSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class ComboSaleAuditMessageCompleter : WPMessageCompleter<ComboSaleAuditMessage>
    {
        protected override bool NeedProcess(ComboSaleAuditMessage msg)
        {
            return msg.ComboSaleSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 409;
        }

        protected override string GetBizSysNo(ComboSaleAuditMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetUrlParameter(ComboSaleAuditMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetMemo(ComboSaleAuditMessage msg)
        {
            return "关闭审核套餐待办事项";
        }

        protected override int GetCurrentUserSysNo(ComboSaleAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 生效
    /// </summary>
    public class ComboSaleActiveMessageCompleter : WPMessageCompleter<ComboSaleActiveMessage>
    {
        protected override bool NeedProcess(ComboSaleActiveMessage msg)
        {
            return msg.ComboSaleSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 409;
        }

        protected override string GetBizSysNo(ComboSaleActiveMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetUrlParameter(ComboSaleActiveMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetMemo(ComboSaleActiveMessage msg)
        {
            return "关闭审核套餐待办事项";
        }

        protected override int GetCurrentUserSysNo(ComboSaleActiveMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public class ComboSaleAuditRefuseMessageCompleter : WPMessageCompleter<ComboSaleAuditRefuseMessage>
    {
        protected override bool NeedProcess(ComboSaleAuditRefuseMessage msg)
        {
            return msg.ComboSaleSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 409;
        }

        protected override string GetBizSysNo(ComboSaleAuditRefuseMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetUrlParameter(ComboSaleAuditRefuseMessage msg)
        {
            return msg.ComboSaleSysNo.ToString();
        }

        protected override string GetMemo(ComboSaleAuditRefuseMessage msg)
        {
            return "关闭审核套餐待办事项";
        }

        protected override int GetCurrentUserSysNo(ComboSaleAuditRefuseMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 促销_限时促销审核

    /// <summary>
    /// 提交审核消息
    /// </summary>
    public class CountDownSubmitMessageCreator : WPMessageCreator<CountDownSubmitMessage>
    {
        protected override bool NeedProcess(CountDownSubmitMessage msg)
        {
            throw new NotImplementedException();
            //CountdownInfo info = ObjectFactory<IMKTBizInteract>.Instance.GetCountdownById(msg.CountdownSysNo);
            //if (null == info)
            //    return false;
            //if (info.Status == CountdownStatus.WaitForVerify)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override int GetCategorySysNo()
        {
            return 408;
        }

        protected override string GetBizSysNo(CountDownSubmitMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetUrlParameter(CountDownSubmitMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetMemo(CountDownSubmitMessage msg)
        {
            return "审核限时促销待办事项";
        }

        protected override int GetCurrentUserSysNo(CountDownSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class CountDownAuditMessageCompleter : WPMessageCompleter<CountDownAuditMessage>
    {
        protected override bool NeedProcess(CountDownAuditMessage msg)
        {
            return msg.CountdownSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 408;
        }

        protected override string GetBizSysNo(CountDownAuditMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetUrlParameter(CountDownAuditMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetMemo(CountDownAuditMessage msg)
        {
            return "关闭限时促销待办事项";
        }

        protected override int GetCurrentUserSysNo(CountDownAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核不通过
    /// </summary>
    public class CountDownAuditRejectMessageCompleter : WPMessageCompleter<CountDownAuditRejectMessage>
    {
        protected override bool NeedProcess(CountDownAuditRejectMessage msg)
        {
            return msg.CountdownSysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 408;
        }

        protected override string GetBizSysNo(CountDownAuditRejectMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetUrlParameter(CountDownAuditRejectMessage msg)
        {
            return msg.CountdownSysNo.ToString();
        }

        protected override string GetMemo(CountDownAuditRejectMessage msg)
        {
            return "关闭限时促销待办事项";
        }

        protected override int GetCurrentUserSysNo(CountDownAuditRejectMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 评论

    /// <summary>
    /// ESB 创建评论
    /// </summary>
    public class ProductReviewCreatedMessageCreator : WPMessageCreator<ProductReviewCreatedMessage>
    {
        protected override bool NeedProcess(ProductReviewCreatedMessage msg)
        {
            throw new NotImplementedException();
            //ProductReview info = ObjectFactory<IMKTBizInteract>.Instance.GetProductReviewById(msg.SysNo);
            //// 初始化状态
            //if (info.Status.Equals("O"))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override int GetCategorySysNo()
        {
            return 406;
        }

        protected override string GetBizSysNo(ProductReviewCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewCreatedMessage msg)
        {
            return "商品发表评论后发送待审核待办消息";
        }

        protected override int GetCurrentUserSysNo(ProductReviewCreatedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class ProductReviewAuditMessageCompleter : WPMessageCompleter<ProductReviewAuditMessage>
    {
        protected override bool NeedProcess(ProductReviewAuditMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 406;
        }

        protected override string GetBizSysNo(ProductReviewAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewAuditMessage msg)
        {
            return "关闭审核评论待办事项";
        }

        protected override int GetCurrentUserSysNo(ProductReviewAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class ProductReviewVoidMessageCompleter : WPMessageCompleter<ProductReviewVoidMessage>
    {
        protected override bool NeedProcess(ProductReviewVoidMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 406;
        }

        protected override string GetBizSysNo(ProductReviewVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewVoidMessage msg)
        {
            return "关闭审核评论待办事项";
        }

        protected override int GetCurrentUserSysNo(ProductReviewVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 评论回复

    public class ProductReviewReplyCreatedMessageCreator : WPMessageCreator<ProductReviewReplyCreatedMessage>
    {
        protected override bool NeedProcess(ProductReviewReplyCreatedMessage msg)
        {
            throw new NotImplementedException();
            //ProductReview info = ObjectFactory<IMKTBizInteract>.Instance.GetProductReviewById(msg.ReviewSysNo);

            //ProductReviewReply reply = info.ProductReviewReplyList.Where(c => c.SysNo == msg.SysNo).FirstOrDefault();

            //if (reply.StatusValue == "O" || reply.StatusValue == "E")
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            
        }

        protected override int GetCategorySysNo()
        {
            return 407;
        }

        protected override string GetBizSysNo(ProductReviewReplyCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewReplyCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewReplyCreatedMessage msg)
        {
            return "商品发表评论回复后发送待审核待办消息";
        }

        protected override int GetCurrentUserSysNo(ProductReviewReplyCreatedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public class ProductReviewReplyAuditMessageCompleter : WPMessageCompleter<ProductReviewReplyAuditMessage>
    {
        protected override bool NeedProcess(ProductReviewReplyAuditMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 407;
        }

        protected override string GetBizSysNo(ProductReviewReplyAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewReplyAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewReplyAuditMessage msg)
        {
            return "关闭审核评论回复待办事项";
        }

        protected override int GetCurrentUserSysNo(ProductReviewReplyAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class ProductReviewReplyVoidMessageCompleter : WPMessageCompleter<ProductReviewReplyVoidMessage>
    {
        protected override bool NeedProcess(ProductReviewReplyVoidMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 407;
        }

        protected override string GetBizSysNo(ProductReviewReplyVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductReviewReplyVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductReviewReplyVoidMessage msg)
        {
            return "关闭审核评论回复待办事项";
        }

        protected override int GetCurrentUserSysNo(ProductReviewReplyVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 创建咨询及咨询回复
    
    /// <summary>
    /// 处理前台创建的咨询
    /// </summary>
    public class ConsultCreateMessageCompleter : WPMessageCreator<ProductConsultCreatedMessage>
    {
        protected override bool NeedProcess(ProductConsultCreatedMessage msg)
        {
            throw new NotImplementedException();
            //ProductConsult item = ObjectFactory<IMKTBizInteract>.Instance.LoadProductConsult(msg.SysNo);
            //if (item == null)
            //    return false;
            //if (item.Status == "O")
            //    return true;
            //return false;
        }

        protected override int GetCategorySysNo()
        {
            return 404;
        }

        protected override string GetBizSysNo(ProductConsultCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductConsultCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductConsultCreatedMessage msg)
        {
            return "商品发表咨询后发送待审核待办消息";
        }

        protected override int GetCurrentUserSysNo(ProductConsultCreatedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 处理前台创建的咨询回复
    /// </summary>
    public class ConsultReplyCreateMessageCompleter : WPMessageCreator<ProductConsultReplyCreatedMessage>
    {
        protected override bool NeedProcess(ProductConsultReplyCreatedMessage msg)
        {
            throw new NotImplementedException();
            //ProductConsultReply item = ObjectFactory<IMKTBizInteract>.Instance.LoadProductConsultReply(msg.SysNo);
            //if (item == null)
            //    return false;
            //if (item.Status == "O")
            //    return true;
            //return false;
        }

        protected override int GetCategorySysNo()
        {
            return 405;
        }

        protected override string GetBizSysNo(ProductConsultReplyCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ProductConsultReplyCreatedMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ProductConsultReplyCreatedMessage msg)
        {
            return "商品发表咨询回复后发送待审核待办消息";
        }

        protected override int GetCurrentUserSysNo(ProductConsultReplyCreatedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    #endregion

    #region 咨询审核

    /// <summary>
    /// 审核
    /// </summary>
    public class ConsultAuditMessageCompleter : WPMessageCompleter<ConsultAuditMessage>
    {
        protected override bool NeedProcess(ConsultAuditMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 404;
        }

        protected override string GetBizSysNo(ConsultAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsultAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsultAuditMessage msg)
        {
            return "关闭审核咨询待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsultAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class ConsultVoidMessageCompleter : WPMessageCompleter<ConsultVoidMessage>
    {
        protected override bool NeedProcess(ConsultVoidMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 404;
        }

        protected override string GetBizSysNo(ConsultVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsultVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsultVoidMessage msg)
        {
            return "关闭审核咨询待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsultVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    

    #endregion

    #region 咨询回复审核

    /// <summary>
    /// 审核
    /// </summary>
    public class ConsultReplyAuditMessageCompleter : WPMessageCompleter<ConsultReplyAuditMessage>
    {
        protected override bool NeedProcess(ConsultReplyAuditMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 405;
        }

        protected override string GetBizSysNo(ConsultReplyAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsultReplyAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsultReplyAuditMessage msg)
        {
            return "关闭审核咨询待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsultReplyAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废
    /// </summary>
    public class ConsultReplyVoidMessageCompleter : WPMessageCompleter<ConsultReplyVoidMessage>
    {
        protected override bool NeedProcess(ConsultReplyVoidMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 405;
        }

        protected override string GetBizSysNo(ConsultReplyVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsultReplyVoidMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsultReplyVoidMessage msg)
        {
            return "关闭审核咨询待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsultReplyVoidMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 拒绝
    /// </summary>
    public class ConsultReplyAuditRefuseMessageCompleter : WPMessageCompleter<ConsultReplyAuditRefuseMessage>
    {
        protected override bool NeedProcess(ConsultReplyAuditRefuseMessage msg)
        {
            return msg.SysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            return 405;
        }

        protected override string GetBizSysNo(ConsultReplyAuditRefuseMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsultReplyAuditRefuseMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsultReplyAuditRefuseMessage msg)
        {
            return "关闭审核咨询待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsultReplyAuditRefuseMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }



    #endregion
}
