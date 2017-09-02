using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;

using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 加载一个组合信息
        /// </summary>
        [WebGet(UriTemplate = "/Combo/{sysNo}")]
        public virtual ComboInfo LoadCombo(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ActivitySysNoIsNotActive"));
            }
            return ObjectFactory<ComboAppService>.Instance.Load(id);
        }

        [WebInvoke(UriTemplate = "/Combo/CheckOptionalAccessoriesItemAndDiys", Method = "POST")]
        public virtual List<string> CheckOptionalAccessoriesItemAndDiys(List<int> sysNos)
        {
            return ObjectFactory<ComboAppService>.Instance.CheckOptionalAccessoriesItemAndDiys(sysNos);
        }



        [WebInvoke(UriTemplate = "/Combo/BatchCreate", Method = "POST")]
        public List<ComboInfo> BatchCreateCombo(ComboBatchReq info)
        {
            List<ComboInfo> list = new List<ComboInfo>();
            for (int i = 0; i < info.MasterItems.Count; i++)
            {
                ComboInfo combo = new ComboInfo();
                combo.IsShowName = info.IsShowName;
                combo.Priority = info.Priority;
                combo.Name = info.Name;
                combo.Status = info.Status;
                combo.SaleRuleType = info.SaleRuleType;
                combo.CompanyCode = info.CompanyCode;
                list.Add(combo);

                //主商品
                ComboItem comboItem = new ComboItem();
                comboItem.Discount = Math.Round(info.MDiscount.Value - 0.005m, 2);
                comboItem.Quantity = info.MQty;
                comboItem.ProductSysNo = info.MasterItems[i];
                comboItem.IsMasterItemB = true;

                combo.Items.Add(comboItem);

                //次商品
                if (info.Items != null)
                {
                    List<ProductInfo> products = null;
                    //如果只输入了折扣率，则需要计算Discount的值
                    if (info.Discount.HasValue)
                    {
                        products = ObjectFactory<ComboAppService>.Instance.GetProductInfoListByProductSysNoList(info.Items);
                    }
                    for (int j = 0; j < info.Items.Count; j++)
                    {
                        ComboItem saleRuleItemV2 = new ComboItem();
                        if (info.Discount.HasValue)
                        {
                            saleRuleItemV2.Discount = Math.Round(info.Discount.Value - 0.005m, 2);
                        }
                        else
                        {
                            if (products != null)
                            {
                                foreach (var product in products)
                                {
                                    if (product.SysNo == info.Items[j])
                                    {
                                        saleRuleItemV2.Discount = (-1) * product.ProductPriceInfo.CurrentPrice * info.DiscountRate.Value;
                                    }
                                }
                            }
                        }

                        saleRuleItemV2.Quantity = info.Qty;
                        saleRuleItemV2.ProductSysNo = info.Items[j];
                        saleRuleItemV2.IsMasterItemB = false;

                        combo.Items.Add(saleRuleItemV2);
                    }                    
                }
            }
            //判断【商品捆绑规则设定对应的商家类型是否相同】bug125
            foreach (ComboInfo entity in list)
            {
                List<string> errorMessage = CheckComboItemIsPass(entity);
                if (errorMessage != null && errorMessage.Count > 0)
                {
                    throw new BizException(errorMessage[0]);
                }
            }
            
            return ObjectFactory<ComboAppService>.Instance.BatchCreateCombo(list);
        }

        [WebInvoke(UriTemplate = "/Combo/BatchUpdate", Method = "PUT")]
        public List<ComboInfo> BatchUpdateCombo(ComboBatchReq info)
        {
            List<ComboInfo> list = new List<ComboInfo>();
            for (int i = 0; i < info.MasterItems.Count; i++)
            {
                ComboInfo combo = new ComboInfo();
                combo.Name = info.Name;
                combo.Status = info.Status;
                combo.SaleRuleType = info.SaleRuleType;
                list.Add(combo);

                //主商品
                ComboItem comboItem = new ComboItem();
                comboItem.Discount = Math.Round(info.MDiscount.Value - 0.005m, 2);
                comboItem.Quantity = info.MQty;
                comboItem.ProductSysNo = info.MasterItems[i];
                comboItem.IsMasterItemB = true;

                combo.Items.Add(comboItem);

                //次商品
                if (info.Items != null)
                {
                    List<ProductInfo> products = null;
                    //如果只输入了折扣率，则需要计算Discount的值
                    if (info.Discount.HasValue)
                    {
                        products = ObjectFactory<ComboAppService>.Instance.GetProductInfoListByProductSysNoList(info.Items);
                    }
                    for (int j = 0; j < info.Items.Count; j++)
                    {
                        ComboItem saleRuleItemV2 = new ComboItem();
                        if (info.Discount.HasValue)
                        {
                            saleRuleItemV2.Discount = Math.Round(info.Discount.Value - 0.005m, 2);
                        }
                        else
                        {
                            if (products != null)
                            {
                                foreach (var product in products)
                                {
                                    if (product.SysNo == info.Items[j])
                                    {
                                        saleRuleItemV2.Discount = (-1) * product.ProductPriceInfo.CurrentPrice * info.DiscountRate.Value;
                                    }
                                }
                            }
                        }

                        saleRuleItemV2.Quantity = info.Qty;
                        saleRuleItemV2.ProductSysNo = info.Items[j];
                        saleRuleItemV2.IsMasterItemB = false;

                        combo.Items.Add(saleRuleItemV2);
                    }
                }
            }

            return ObjectFactory<ComboAppService>.Instance.BatchUpdateCombo(list);
        }

        [WebInvoke(UriTemplate = "/Combo/Create", Method = "POST")]
        public int? CreateCombo(ComboInfo info)
        {
            return ObjectFactory<ComboAppService>.Instance.CreateCombo(info);
        }

        [WebInvoke(UriTemplate = "/Combo/Update", Method = "PUT")]
        public void UpdateCombo(ComboInfo info)
        {
            ObjectFactory<ComboAppService>.Instance.UpdateCombo(info);
        }

        [WebInvoke(UriTemplate = "/Combo/CheckComboItemIsPass", Method = "POST")]
        public List<string> CheckComboItemIsPass(ComboInfo info)
        {
            return ObjectFactory<ComboAppService>.Instance.CheckComboItemIsPass(info);
        }
 
        //设置产品的价格和+折扣<成本价格和  变为待审核状态
        //提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno，
        //然后检查商品对应捆绑规则是否有低于成本价的情况，有的就将其变为待审核(status=1)！
        /// <summary>
        /// 检查条件：Combo当前是有效状态, 检查内容：价格和+折扣和 小于 成本价格和 ，价格检查不通过
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Combo/CheckPriceIsPass/{comboSysNo}")]
        public string CheckPriceIsPass(string comboSysNo)
        {
            int sysNo = int.Parse(comboSysNo);
            return ObjectFactory<ComboAppService>.Instance.CheckPriceIsPass(sysNo) ? "Y" : "N";
        }

        /// <summary>
        /// 仅仅更新状态，不做任何检查，主要是为外部系统提供服务
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="targetStatus"></param>
        [WebInvoke(UriTemplate = "/Combo/UpdateStatus", Method = "PUT")]
        public void UpdateComboStatus(ComboUpdateStatusReq msg)
        {
            ObjectFactory<ComboAppService>.Instance.UpdateStatus(msg.SysNo.Value, msg.TargetStatus.Value);
        }

        [WebInvoke(UriTemplate = "/Combo/ApproveCombo", Method = "PUT")]
        public void ApproveCombo(ComboUpdateStatusReq msg)
        {
            ObjectFactory<ComboAppService>.Instance.ApproveCombo(msg.SysNo.Value, msg.TargetStatus.Value);
        }

        [WebInvoke(UriTemplate = "/Combo/Query", Method = "POST")]
        public QueryResult QueryCombo(ComboQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<IComboQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };            
        }

        [WebInvoke(UriTemplate = "/Combo/BatchDelete", Method = "PUT")]
        public void BatchDeleteCombo(List<int> sysNoList)
        {
            ObjectFactory<ComboAppService>.Instance.BatchDelete(sysNoList);
        }

        [WebInvoke(UriTemplate = "/Combo/CheckComboPriceAndSetStatus", Method = "POST")]
        public void CheckComboPriceAndSetStatus(int productSysNo)
        {
            ObjectFactory<ComboAppService>.Instance.CheckComboPriceAndSetStatus(productSysNo);
        }
    }
}