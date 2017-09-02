using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Transactions;
using System.Collections;
using ECommerce.Entity.Promotion;
using ECommerce.DataAccess.Promotion;
using ECommerce.Entity.Common;
using ECommerce.Utility;
using ECommerce.Enums.Promotion;
using ECommerce.Service.Product;
using ECommerce.WebFramework;


namespace ECommerce.Service.Promotion
{
    public class ComboService
    {
        private ComboDA _comboDA = new ComboDA();
        public QueryResult<ComboQueryResult> Query(ComboQueryFilter filter)
        {
            return _comboDA.Query(filter);
        }

        public ComboQueryResult Load(int id)
        {
            return _comboDA.Load(id);
        }

        public void Create(ComboInfo entity)
        {
            ValidateEntity(entity);
            using (var ts = new TransactionScope())
            {
                _comboDA.CreateMaster(entity);
                foreach (var item in entity.Items)
                {
                    item.ComboSysNo = entity.SysNo;
                    item.CreateTime = DateTime.Now;
                    _comboDA.AddComboItem(item);
                }
                ts.Complete();
            }
        }

        public void Update(ComboInfo entity)
        {
            ValidateEntity(entity);
            using (var ts = new TransactionScope())
            {
                _comboDA.UpdateMaster(entity);
                _comboDA.DeleteComboAllItem(entity.SysNo);
                foreach (var item in entity.Items)
                {
                    item.ComboSysNo = entity.SysNo;
                    item.CreateTime = DateTime.Now;
                    _comboDA.AddComboItem(item);
                }
                ts.Complete();
            }
        }

        public void Submit(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    var entity = _comboDA.Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    //验证记录是否可提交审核
                    if (entity.CanSubmit == false)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}的状态不是无效，不可执行提交审核操作！"), id));
                        continue;
                    }

                    _comboDA.UpdateStatus(id,(int)ComboStatus.WaitingAudit,opUserName);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException(LanguageHelper.GetText("操作已完成。<br/>") + sb.ToString());
                }
            }
        }

        public void Void(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    var entity = _comboDA.Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    //验证记录是否可作废
                    if (entity.CanVoid == false)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}的状态不是有效，不可执行作废操作！"), id));
                        continue;
                    }
                    _comboDA.UpdateStatus(id, (int)ComboStatus.Deactive, opUserName);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException(LanguageHelper.GetText("操作已完成。<br/>") + sb.ToString());
                }
            }
        }

        private void ValidateEntity(ComboInfo entity)
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
                if (item.Status != ComboStatus.Deactive)
                {
                    throw new BusinessException(LanguageHelper.GetText("此单据不是无效状态，无法执行编辑操作。"));
                }
            }
            if (string.IsNullOrWhiteSpace(entity.SaleRuleName))
            {
                throw new BusinessException(LanguageHelper.GetText("活动名称不可为空！"));
            }
            if (entity.Items == null || entity.Items.Count < 2)
            {
                throw new BusinessException(LanguageHelper.GetText("捆绑促销活动至少包含2个商品!"));
            }
            foreach (var item in entity.Items)
            {
                var product = ProductService.GetProductBySysNo(entity.SellerSysNo.Value, item.ProductSysNo);
                if (Math.Abs(item.Discount) > product.CurrentPrice)
                {
                    throw new BusinessException(LanguageHelper.GetText(string.Format("商品#{0}的折扣不能大于卖价!",product.SysNo)));
                }
            }
        }
    }
}
