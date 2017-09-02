using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Text;

using System.Data;
using ECommerce.Entity.Promotion;
using ECommerce.DataAccess.Promotion;
using ECommerce.Utility;
using ECommerce.Entity.Common;
using ECommerce.WebFramework;
using ECommerce.Enums;
using ECommerce.Enums.Promotion;
using ECommerce.Service.Product;
using ECommerce.DataAccess.Inventory;

namespace ECommerce.Service.Promotion
{
    public class GroupBuyingService
    {
        private GroupBuyingDA m_GroupBuyingDA = new GroupBuyingDA();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GroupBuyingInfo Create(GroupBuyingInfo info)
        {
            // 验证团购信息
            ValidateEntity(info);

            var ts = new TransactionScope();
            using (ts)
            {
                // 创建团购信息
                info.SysNo = m_GroupBuyingDA.Create(info);

                // 创建价格信息
                m_GroupBuyingDA.CreateProductGroupBuyingPrice(info.SysNo, 1, info.GroupBuyPrice, 0, 0);


                ts.Complete();
            }

            return info;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public GroupBuyingInfo Update(GroupBuyingInfo info)
        {
            // 验证团购信息
            ValidateEntity(info);

            var ts = new TransactionScope();
            using (ts)
            {
                // 更新团购信息
                m_GroupBuyingDA.Update(info);

                //先删除该团购的阶梯价格
                m_GroupBuyingDA.DeleteProductGroupBuyingPrice(info.SysNo);
                // 创建价格信息
                m_GroupBuyingDA.CreateProductGroupBuyingPrice(info.SysNo, 1, info.GroupBuyPrice, 0, 0);

                ts.Complete();
            }

            return info;
        }


        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Submit(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    GroupBuyingQueryResult entity = Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    if (entity.CanSubmit == false)
                    {
                        var msg = LanguageHelper.GetText("团购[{0}]提交审核失败，只有初始态的团购才能提交审核！");
                        sb.AppendLine(string.Format(msg, id));
                        continue;
                    }

                    m_GroupBuyingDA.UpdataSatus(id, "W", opUserName);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNos"></param>
        public virtual void Void(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    GroupBuyingQueryResult entity = Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    if (entity.CanVoid == false)
                    {
                        var msg = LanguageHelper.GetText("团购[{0}]作废失败，只有初始态、待审核、就绪或审核未通过的团购允许作废！");
                        sb.AppendLine(string.Format(msg, id));
                        continue;
                    }

                    m_GroupBuyingDA.UpdataSatus(id, "D", opUserName);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Stop(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    GroupBuyingQueryResult entity = Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    if (entity.CanStop == false)
                    {
                        var msg = LanguageHelper.GetText("团购[{0}]终止失败，只有运行状态的团购才能终止！");
                        sb.AppendLine(string.Format(msg, id));
                        continue;
                    }

                    m_GroupBuyingDA.UpdateGroupBuyingEndDate(id, opUserName);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public GroupBuyingQueryResult Load(int? sysNo)
        {
            var result = m_GroupBuyingDA.Load(sysNo.Value);
            if (result == null)
            {
                throw new BusinessException(LanguageHelper.GetText("团购信息不存在!"));
            }

            return result;
        }

        //分页查询
        public QueryResult<GroupBuyingQueryResult> Query(GroupBuyingQueryFilter filter)
        {
            if (filter.BeginDateTo.HasValue)
            {
                filter.BeginDateTo = filter.BeginDateTo.Value.AddDays(1);
            }
            if (filter.EndDateTo.HasValue)
            {
                filter.EndDateTo = filter.EndDateTo.Value.AddDays(1);
            }
            return m_GroupBuyingDA.Query(filter);
        }

        /// <summary>
        /// 限时抢购排重：验证商品在一段时间内是否存在有效的团购活动
        /// </summary>
        public bool CheckGroupBuyConflict(int productSysNo, DateTime begindate, DateTime enddate)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(productSysNo);

            return m_GroupBuyingDA.CheckConflict(0, productSysNos, begindate, enddate);
        }


        #region 团购业务验证规则

        /// <summary>
        /// 团购业务规则
        /// </summary>
        /// <param name="entity"></param>
        private void ValidateEntity(GroupBuyingInfo entity)
        {
            if (entity.SysNo > 0)
            {
                var item = Load(entity.SysNo);
                if (item == null)
                {
                    throw new BusinessException(LanguageHelper.GetText("单据已不存在。"));
                }
                if (item.SellerSysNo != entity.SellerSysNo)
                {
                    throw new BusinessException(LanguageHelper.GetText("您无权操作此单据。"));
                }
                if (item.Status != GroupBuyingStatus.Init)
                {
                    throw new BusinessException(LanguageHelper.GetText("此单据的状态不是初始态，无法执行编辑或提交审核操作。"));
                }
            }
            // 验证商品是否存在
            var product = ProductService.GetProduct(entity.SellerSysNo.Value, entity.ProductID);
            if (product == null)
            {
                throw new BusinessException(LanguageHelper.GetText("活动商品不存在！"));
            }
            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessException(LanguageHelper.GetText("活动商品为未上架状态,不能设置活动！"));
            }
            entity.ProductSysNo = product.SysNo;
            if (product.CurrentPrice <= entity.GroupBuyPrice)
            {
                throw new BusinessException(LanguageHelper.GetText("团购价格不能大于等于商品卖价！"));
            }
            // 验证团购时间
            if (entity.BeginDate > entity.EndDate)
            {
                throw new BusinessException(LanguageHelper.GetText("团购开始时间晚于结束时间！"));
            }
            if (entity.EndDate <= DateTime.Now)
            {
                throw new BusinessException(LanguageHelper.GetText("团购结束时间不能小于当前时间！"));
            }
            // 验证商品是否存在相冲突时间段的团购中
            CheckGroupBuyConflict(entity);

            // 验证商品是否存在相冲突时间段的限时抢购中
            CheckGroupBuyAndCountDownConflict(entity);
        }

        /// <summary>
        /// 验证商品是否存在相冲突时间段的团购中
        /// </summary>
        private void CheckGroupBuyConflict(GroupBuyingInfo groupBuy)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(groupBuy.ProductSysNo);

            if (m_GroupBuyingDA.CheckConflict(groupBuy.SysNo, productSysNos, groupBuy.BeginDate, groupBuy.EndDate))
            {
                throw new BusinessException(LanguageHelper.GetText("该商品已经存在一个时间冲突的团购！"));
            }
        }

        /// <summary>
        /// 验证商品是否存在相冲突时间段的限时抢购中
        /// </summary>
        private void CheckGroupBuyAndCountDownConflict(GroupBuyingInfo groupBuy)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(groupBuy.ProductSysNo);

            CountdownService countdownService = new CountdownService();
            if (countdownService.CheckGroupBuyAndCountDownConflict(productSysNos, groupBuy.BeginDate, groupBuy.EndDate))
            {
                throw new BusinessException(LanguageHelper.GetText("该商品已经存在一个时间冲突的限时抢购！"));
            }
        }
        #endregion

        public int GetProductAvailableSaleQty(int productSysNo)
        {
            var result = InventoryDA.GetProductTotalInventoryInfo(productSysNo);
            if (result != null)
            {
                return result.AvailableQty + result.ConsignQty + result.VirtualQty - result.InvalidQty;
            }
            else
            {
                return 0;
            }
        }
    }
}
