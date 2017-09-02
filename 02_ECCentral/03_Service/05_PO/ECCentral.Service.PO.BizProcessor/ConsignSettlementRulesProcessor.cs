using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.BizEntity.PO;
using ECCentral.Service.EventMessage.PO;
namespace ECCentral.Service.PO.BizProcessor
{
    [VersionExport(typeof(ConsignSettlementRulesProcessor))]
    public class ConsignSettlementRulesProcessor
    {

        #region [Fields]
        private IConsignSettlementRulesDA m_ConsignSettlementRuleDA;

        public IConsignSettlementRulesDA ConsignSettlementRuleDA
        {
            get
            {
                if (null == m_ConsignSettlementRuleDA)
                {
                    m_ConsignSettlementRuleDA = ObjectFactory<IConsignSettlementRulesDA>.Instance;
                }
                return m_ConsignSettlementRuleDA;
            }
        }
        #endregion


        /// <summary>
        /// 创建代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConsignSettlementRulesInfo CreateConsignSettlementRule(ConsignSettlementRulesInfo entity)
        {
            //检测数据完整性
            CheckEntity(entity);

            //Code的重复性检测
            ConsignSettlementRulesInfo chkEntity = ConsignSettlementRuleDA.GetConsignSettleRuleByCode(entity.SettleRulesCode);
            if (chkEntity != null)
            {
                //规则编码({0})已存在，请重新设置
                throw new BizException(string.Format(GetExceptionString("ConsignRule_RuleExist"), entity.SettleRulesCode));
            }

            //检测商品的属性
            CheckProduct(entity.ProductSysNo.Value);

            //时间段重复性的检测
            CheckRuleDateRepeat(entity);

            entity.Status = ConsignSettleRuleStatus.Wait_Audit;
            SetDefaultValue(entity);

            int sysNo = ConsignSettlementRuleDA.CreateConsignSettlementRule(entity);
            entity.RuleSysNo = sysNo;

            //发送ESB消息
            EventPublisher.Publish<SettlementRuleCreateMessage>(new SettlementRuleCreateMessage()
            {
                CreateUserSysNo = ServiceContext.Current.UserSysNo,
                SettleRulesCode = entity.SettleRulesCode
            });

            //记录系统日志
            //WriteLog(entity, LogType.ConsignSettleRule_Create);

            ExternalDomainBroker.CreateLog(" Create ConsignSettle "
             , BizEntity.Common.BizLogType.ConsignSettleRule_Create
             , sysNo
             , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 更新代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConsignSettlementRulesInfo UpdateConsignSettlementRule(ConsignSettlementRulesInfo entity)
        {
            //检测数据完整性
            CheckEntity(entity);

            //查询原实体
            ConsignSettlementRulesInfo oldEntity = ConsignSettlementRuleDA.GetConsignSettleRuleByCode(entity.SettleRulesCode);
            if (oldEntity == null)
            {
                //规则({0})不存在，无法修改
                throw new BizException(string.Format(GetExceptionString("ConsignRule_RuleNotExist"), entity.SettleRulesCode));
            }
            if (oldEntity.Status != ConsignSettleRuleStatus.Wait_Audit)
            {
                //规则({0})不处于待审核状态，无法修改
                throw new BizException(string.Format(GetExceptionString("ConsignRule_WaitingAudit_Invalid"), entity.SettleRulesCode));
            }

            //检测商品的属性
            CheckProduct(entity.ProductSysNo.Value);

            //时间段重复性的检测
            CheckRuleDateRepeat(entity);

            SetDefaultValue(entity);

            entity.EditUser = ExternalDomainBroker.GetUserNameByUserSysNo(ServiceContext.Current.UserSysNo);

            entity = Modify(entity, ConsignSettleRuleActionType.Update);

            //记录系统日志
            //WriteLog(entity, LogType.ConsignSettleRule_Update);

            ExternalDomainBroker.CreateLog(" Updated ConsignSettleRule "
              , BizEntity.Common.BizLogType.ConsignSettleRule_Update
              , entity.RuleSysNo.Value
              , entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 审核代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConsignSettlementRulesInfo AuditConsignSettlementRule(ConsignSettlementRulesInfo info)
        {
            ConsignSettlementRulesInfo entity = ConsignSettlementRuleDA.GetConsignSettleRuleByCode(info.SettleRulesCode);
            if (entity == null)
            {
                //规则({0})不存在，无法审核
                throw new BizException(string.Format(GetExceptionString("ConsignRule_RuleNotExist_Audit"), info.SettleRulesCode));
            }

            if (entity.Status != ConsignSettleRuleStatus.Wait_Audit)
            {
                //规则({0})不是待审核状态，无法审核
                throw new BizException(string.Format(GetExceptionString("ConsignRule_WaitingAudit_Invalid_Audit"), info.SettleRulesCode));
            }

            ////审核人和创建人不能相同
            //if (info.EditUser == entity.CreateUser)
            //{
            //    throw new BizException(GetExceptionString("ConsignRule_CreateAndAuditUserNotTheSame"));
            //}

            //检测商品的属性
            CheckProduct(entity.ProductSysNo.Value);

            //时间段重复性的检测
            CheckRuleDateRepeat(entity);

            entity.Status = ConsignSettleRuleStatus.Available;
            entity.EditUser = ExternalDomainBroker.GetUserNameByUserSysNo(ServiceContext.Current.UserSysNo);
            entity = Modify(entity, ConsignSettleRuleActionType.Audit);

            //发送ESB消息
            EventPublisher.Publish<SettlementRuleAuditMessage>(new SettlementRuleAuditMessage()
            {
                AuditUserSysNo = ServiceContext.Current.UserSysNo,
                SettleRulesCode = entity.SettleRulesCode
            });

            //记录系统日志
            //WriteLog(entity, LogType.ConsignSettleRule_Audit);

            ExternalDomainBroker.CreateLog(" Audit ConsignSettleRule "
            , BizEntity.Common.BizLogType.ConsignSettleRule_Audit
            , entity.RuleSysNo.Value
            , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 终止代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConsignSettlementRulesInfo StopConsignSettlementRule(string settleRulesCode)
        {
            ConsignSettlementRulesInfo entity = ConsignSettlementRuleDA.GetConsignSettleRuleByCode(settleRulesCode);
            if (entity == null)
            {
                //规则({0})不存在，无法终止
                throw new BizException(string.Format(GetExceptionString("ConsignRule_RuleNotExist_Stop"), settleRulesCode));
            }

            if (entity.Status != ConsignSettleRuleStatus.Available
                && entity.Status != ConsignSettleRuleStatus.Enable)
            {
                //规则({0})不处于“未生效”和“已生效”状态，无法终止
                throw new BizException(string.Format(GetExceptionString("ConsignRule_Available_Invalid_Stop"), settleRulesCode));
            }

            entity.Status = ConsignSettleRuleStatus.Stop;
            entity.EditUser = ExternalDomainBroker.GetUserNameByUserSysNo(ServiceContext.Current.UserSysNo);
            entity = Modify(entity, ConsignSettleRuleActionType.Stop);

            //记录系统日志
            //WriteLog(entity, LogType.ConsignSettleRule_Stop);
            ExternalDomainBroker.CreateLog(" Stop ConsignSettleRule "
           , BizEntity.Common.BizLogType.ConsignSettleRule_Stop
           , entity.RuleSysNo.Value
           , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 作废代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConsignSettlementRulesInfo AbandonConsignSettlementRule(string settleRulesCode)
        {
            ConsignSettlementRulesInfo entity = ConsignSettlementRuleDA.GetConsignSettleRuleByCode(settleRulesCode);
            if (entity == null)
            {
                //规则({0})不存在，无法作废
                throw new BizException(string.Format(GetExceptionString("ConsignRule_RuleNotExist_Abandon"), settleRulesCode));
            }

            if (entity.Status != ConsignSettleRuleStatus.Wait_Audit)
            {
                //规则({0})不处于待审核状态，无法作废
                throw new BizException(string.Format(GetExceptionString("ConsignRule_WaitingAudit_Invalid_Abandon"), settleRulesCode));
            }

            entity.Status = ConsignSettleRuleStatus.Forbid;
            entity.EditUser = ExternalDomainBroker.GetUserNameByUserSysNo(ServiceContext.Current.UserSysNo);
            entity = Modify(entity, ConsignSettleRuleActionType.Abandon);

            //发送ESB消息
            EventPublisher.Publish<SettlementRuleAbandonMessage>(new SettlementRuleAbandonMessage()
            {
                AbandonUserSysNo = ServiceContext.Current.UserSysNo,
                SettleRulesCode = settleRulesCode
            });

            //记录系统日志
            // WriteLog(entity, LogType.ConsignSettleRule_Abadon);

            ExternalDomainBroker.CreateLog(" Abandon ConsignSettleRule "
          , BizEntity.Common.BizLogType.ConsignSettleRule_Abadon
          , entity.RuleSysNo.Value
          , entity.CompanyCode);
            return entity;
        }

        #region [Check Methods]
        private void CheckEntity(ConsignSettlementRulesInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (string.IsNullOrEmpty(entity.SettleRulesCode))
            {
                //规则代码不能为空
                throw new BizException(GetExceptionString("ConsignRule_CodeEmpty"));
            }
            if (string.IsNullOrEmpty(entity.SettleRulesName))
            {
                //规则名称不能为空
                throw new BizException(GetExceptionString("ConsignRule_NameEmpty"));
            }
            if (!entity.ProductSysNo.HasValue || entity.ProductSysNo <= 0)
            {
                //无效的商品({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_ProductInvalid"), entity.ProductSysNo));
            }
            if (!entity.VendorSysNo.HasValue || entity.VendorSysNo <= 0)
            {
                //无效的商家({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_VendorInvalid"), entity.VendorSysNo));
            }
            if (!entity.OldSettlePrice.HasValue || entity.OldSettlePrice < 0M)
            {
                //无效的原结算价格({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_OldSettlePriceInvalid"), entity.OldSettlePrice));
            }
            if (!entity.NewSettlePrice.HasValue || entity.NewSettlePrice < 0M)
            {
                //无效的现结算价格({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_NewSettlePriceInvalid"), entity.NewSettlePrice));
            }
            if (!entity.BeginDate.HasValue || entity.BeginDate <= DateTime.Parse("1900-1-1"))
            {
                //无效的开始时间({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_BeginDateInvalid"), entity.BeginDate));
            }
            if (!entity.EndDate.HasValue || entity.EndDate <= DateTime.Parse("1900-1-1"))
            {
                //无效的结束时间({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_EndDateInvalid"), entity.EndDate));
            }
            if (entity.SettleRulesQuantity.HasValue
                && entity.SettleRulesQuantity.Value <= 0)
            {
                //无效的结算数量({0})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_SettleQtyInvalid"), entity.SettleRulesQuantity));
            }
        }

        //规则的时间点重叠检测
        private void CheckRuleDateRepeat(ConsignSettlementRulesInfo entity)
        {
            ConsignSettlementRulesInfo item = ConsignSettlementRuleDA.GetSettleRuleByItemVender(entity);
            if (item != null)
            {
                //相同Item({0}),相同商家({1})在同一个时间点只能存在一个结算规则({2})
                throw new BizException(string.Format(GetExceptionString("ConsignRule_SettleRuleOnlyOne"), item.ProductID, item.VendorName, item.SettleRulesCode));
            }
        }

        //商品有效性的验证
        //1、必须为代销商品
        private void CheckProduct(int productSysNo)
        {
            int count = ConsignSettlementRuleDA.CheckConsignProduct(productSysNo);
            if (count <= 0)
            {
                //商品({0})不存在或为非代销商品
                throw new BizException(string.Format(GetExceptionString("ConsignRule_ProductNotExistOrNotConsign"), productSysNo));
            }
        }

        private static void SetDefaultValue(ConsignSettlementRulesInfo entity)
        {
            if (string.IsNullOrEmpty(entity.CurrencyCode))
            {
                entity.CurrencyCode = "CNY";
            }
        }
        #endregion
        //修改规则
        private ConsignSettlementRulesInfo Modify(ConsignSettlementRulesInfo entity, ConsignSettleRuleActionType actionType)
        {
            int rows = ConsignSettlementRuleDA.UpdateConsignSettlementRulesInfo(entity, actionType);
            if (rows <= 0)
            {
                //未能修改规则_{0}
                throw new BizException(string.Format(GetExceptionString("ConsignRule_CannotUpdateRule"), entity.SettleRulesCode));
            }

            return entity;
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetExceptionString(string key)
        {
            return ResouceManager.GetMessageString("PO.ConsignSettlement", key);
        }
    }
}
