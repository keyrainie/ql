//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品信息管理
// 子系统名		        渠道商品信息管理业务实现
// 作成者				John
// 改版日				2012.11.7
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Linq;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(ProductChannelMemberInfoAppService))]
    public class ProductChannelMemberInfoAppService
    {
        #region Const
        private int errorCount = 0;
        private int successCount = 0;
        private IDictionary<Int32, String> _ChannelMemberInfoByChannelNo;
        #endregion

        #region ProductChannelMemberInfo
        // 获取渠道列表
        public List<ProductChannelMemberInfo> GetChannelMemberInfoList()
        {
            return ObjectFactory<ProductChannelMemberInfoProcessor>.Instance
                .GetProductChannelMemberInfoListBiz();
        }
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询指定外部渠道会员价格
        public ProductChannelMemberPriceInfo GetProductChannelMemberPriceBySysNo(Int32 SysNo)
        {
            return
            ObjectFactory<ProductChannelMemberInfoProcessor>
                .Instance.GetProductChannelMemberPriceByAll().Where(p => p.SysNo.Equals(SysNo))
                .SingleOrDefault();
        }
        // 插入渠道会员表
        public void InsertProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> entityList)
        {
            StringBuilder result = new StringBuilder();
            
            foreach (var entity in entityList)
                {
                    try
                    {
                        entity.ChannelName = ChannelMemberInfoByChannelNo[entity.ChannelSysNO];
                        ProductChannelMemberPriceLogInfo log = new ProductChannelMemberPriceLogInfo()
                        {
                            ProductSysNo = entity.ProductSysNo,
                            ChannelName = entity.ChannelName,
                            MemberPrice = entity.MemberPrice,
                            MemberPricePercent = entity.MemberPricePercent,
                            OperationType = "A",
                            InDate = DateTime.Now,
                            InUser = entity.InUser,
                            CompanyCode = entity.CompanyCode,
                            StoreCompanyCode = entity.StoreCompanyCode,
                            LanguageCode = entity.LanguageCode
                        };
                        //插入会员渠道价格表
                        ObjectFactory<ProductChannelMemberInfoProcessor>.Instance
                         .InsertProductChannelMemberPricesBiz(entity);
                        //插入会员渠道日志
                        InsertProductChannelMemberPriceLog(log);
                        successCount++;
                    }
                    catch (BizException ex)
                    {
                        string message = ResouceManager.GetMessageString("IM.Category", "ProductSysNo");
                        message += "：{0}，";
                        message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                        message += "：{1}";
                        result.AppendLine(string.Format(message
                            , entity.ProductSysNo, ex.Message));
                        errorCount++;
                    }
                }
            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "AddSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "AddFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage
                    , successCount, errorCount));
                _ChannelMemberInfoByChannelNo = null;
                ClearNum();
            throw new BizException(result.ToString());     
        }

        //成批删除渠道会员表
        public void DeleteProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> entityList)
        {
            StringBuilder result = new StringBuilder();
            foreach (var entity in entityList)
            {
                try
                {
                    DeleteProductChannelMemberPrice(int.Parse(entity.SysNo.ToString()));
                    entity.MemberPricePercent = entity.MemberPricePercent != null
                  ? entity.MemberPricePercent / 100 : entity.MemberPricePercent;
                    ProductChannelMemberPriceLogInfo log = new ProductChannelMemberPriceLogInfo()
                    {
                        ProductSysNo = entity.ProductSysNo,
                        ChannelName = entity.ChannelName,
                        MemberPrice = entity.MemberPrice,
                        MemberPricePercent = entity.MemberPricePercent,
                        OperationType = "D",
                        InDate = DateTime.Now,
                        InUser = entity.EditUser,
                        CompanyCode = entity.CompanyCode,
                        StoreCompanyCode = entity.StoreCompanyCode,
                        LanguageCode = entity.LanguageCode
                    };
                    InsertProductChannelMemberPriceLog(log);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.ProductChannelInfo", "SystemNo");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message
                        , entity.SysNo, ex.Message));
                    errorCount++;
                }
            }
            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "DeleteSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "DeleteFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage
                , successCount, errorCount));
            _ChannelMemberInfoByChannelNo = null;
            ClearNum();
            throw new BizException(result.ToString());     
        }
        //删除渠道会员
        public void DeleteProductChannelMemberPrice(Int32 SysNo)
        {
            ObjectFactory<ProductChannelMemberInfoProcessor>
                .Instance.DeleteProductChannelMemberPrice(SysNo);
        }

        //更新优惠价和优惠比例
        public void UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo entity)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                entity.MemberPricePercent = entity.MemberPricePercent != null
                    ? entity.MemberPricePercent / 100 : entity.MemberPricePercent;
                ProductChannelMemberPriceLogInfo log = new ProductChannelMemberPriceLogInfo()
                {
                    ProductSysNo = entity.ProductSysNo,
                    ChannelName = entity.ChannelName,
                    MemberPrice = entity.MemberPrice,
                    MemberPricePercent = entity.MemberPricePercent,
                    OperationType = "E",
                    InDate = DateTime.Now,
                    InUser = entity.EditUser,
                    CompanyCode = entity.CompanyCode,
                    StoreCompanyCode = entity.StoreCompanyCode,
                    LanguageCode = entity.LanguageCode
                };
                ObjectFactory<ProductChannelMemberInfoProcessor>.Instance
                             .UpdateProductChannelMemberPrice(entity);
                InsertProductChannelMemberPriceLog(log);
            }
            catch (BizException ex)
            {
                result.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Category", "FailReason") + "：{0}", ex.Message));
                throw new BizException(result.ToString());
            }
        }
        //成批更新优惠价和优惠比例
        public void UpdateProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> entitys)
        {
            StringBuilder result = new StringBuilder();
            foreach (var entity in entitys)
            {
                try
                {
                    UpdateProductChannelMemberPrice(entity);
                    ProductChannelMemberPriceLogInfo log = new ProductChannelMemberPriceLogInfo()
                    {
                        ProductSysNo = entity.ProductSysNo,
                        ChannelName = entity.ChannelName,
                        MemberPrice = entity.MemberPrice,
                        MemberPricePercent = entity.MemberPricePercent,
                        OperationType = "E",
                        InDate = DateTime.Now,
                        InUser = entity.EditUser,
                        CompanyCode = entity.CompanyCode,
                        StoreCompanyCode = entity.StoreCompanyCode,
                        LanguageCode = entity.LanguageCode
                    };
                    InsertProductChannelMemberPriceLog(log);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductSysNo");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message
                        , entity.ProductSysNo, ex.Message));
                    errorCount++;
                }
            }
            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "UpdateSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "UpdateFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage
                , successCount, errorCount));
            _ChannelMemberInfoByChannelNo = null;
            ClearNum();
            throw new BizException(result.ToString());     
        }

        
        #endregion
        #region ProductChannelMemberPriceLogInfo
        //插入日志
        public Int32 InsertProductChannelMemberPriceLog(ProductChannelMemberPriceLogInfo entity)
        {
            return ObjectFactory<ProductChannelMemberInfoProcessor>
                .Instance.InsertProductChannelMemberPriceLogBiz(entity);
        }
        #endregion

        #region Method 
        //获取渠道列表的名称
        public IDictionary<Int32, String> ChannelMemberInfoByChannelNo
        {
            get 
            {
                if (_ChannelMemberInfoByChannelNo == null)
                {
                    _ChannelMemberInfoByChannelNo =
                        GetChannelMemberInfoList().ToDictionary(
                        p => int.Parse(p.SysNo.ToString()), p => p.ChannelName);
                }
                return _ChannelMemberInfoByChannelNo;
            }
        }
        protected void ClearNum()
        {
            errorCount = 0;
            successCount = 0;
        }
        #endregion
    }
}
