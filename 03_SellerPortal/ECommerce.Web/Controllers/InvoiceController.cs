using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Invoice;
using ECommerce.Enums;
using ECommerce.Service.Invoice;
using ECommerce.Utility;
using System.Data;
using System.Reflection;
using System.Collections;
using ECommerce.Web.Utility;
using System.IO;

namespace ECommerce.Web.Controllers
{
    public class InvoiceController : SSLControllerBase
    {
        //
        // GET: /Invoice/
        
        public ActionResult SettleQuery()
        {
            return View();
        }

        public ActionResult FinancialReportQuery()
        {
            return View();
        }
        
        public ActionResult GetSettleData()
        {
            SettleQueryFilter filter = BuildQueryFilterEntity<SettleQueryFilter>();
            filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = SettleService.SettleQuery(filter);
            return AjaxGridJson(result);
        }

        public ActionResult GetFinancialReportData()
        {
            SalesStatisticsReportQueryFilter filter = BuildQueryFilterEntity<SalesStatisticsReportQueryFilter>();

            filter.C1SysNo = filter.Category.C1SysNo == 0 ? null : filter.Category.C1SysNo;
            filter.C2SysNo = filter.Category.C2SysNo == 0 ? null : filter.Category.C2SysNo;
            filter.C3SysNo = filter.Category.C3SysNo == 0 ? null : filter.Category.C3SysNo;


            int MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            filter.VendorSysNoList = new List<int>();
            filter.VendorSysNoList.Add(MerchantSysNo);
            filter.SOStatusList = new List<int>();
            if (filter.SOStatus.HasValue)
            {
                filter.SOStatusList.Add(filter.SOStatus.Value);
            }
            SalesStatisticsReport result = FinancialReportService.SalesStatisticsReportQuery(filter);

            return AjaxGridJson(result.SalesStatisticsResult);
        }

        public ActionResult ViewCommissionItemDetail()
        {
            int sysno = 0;
            if (Request["sysno"] == null)
            {
                sysno = SettleService.GetNotSettleedSysNo(UserAuthHelper.GetCurrentUser().SellerSysNo);

            }
            else
            {
                sysno = int.Parse(Request["sysno"]);
            }

            decimal rentFee = 0;
            if (Request["RentFee"] != null)
            {
                rentFee = decimal.Parse(Request["RentFee"]);
            }

            List<CommissionItemInfo> commissionItems = null;

            commissionItems = SettleService.GetCommissionItem(sysno, UserAuthHelper.GetCurrentUser().SellerSysNo);

            //SAC:销售提成;SOC:订单提成;DEF:配送费用;
            List<CommissionItemInfo> commissionItems_SAC = new List<CommissionItemInfo>();
            List<CommissionItemInfo> commissionItems_SOC = new List<CommissionItemInfo>();
            List<CommissionItemInfo> commissionItems_DEF = new List<CommissionItemInfo>();



            foreach (CommissionItemInfo item in commissionItems)
            {
                switch (item.CommissionType)
                {
                    case CommissionType.SAC:
                        #region 销售规则
                        if (!string.IsNullOrEmpty(item.SalesRuleXml) && item.SalesRuleEntity != null)
                        {
                            item.SalesRuleStr = string.Format("保底金额：{0}<br/>", item.SalesRuleEntity.MinCommissionAmt.ToString("f2"));

                            for (int i = 0; i < item.SalesRuleEntity.Rules.Count; i++)
                            {
                                string startAmt = item.SalesRuleEntity.Rules[i].StartAmt.ToString("f2");
                                string endAmt = item.SalesRuleEntity.Rules[i].EndAmt.ToString("f2");
                                string percentage = (item.SalesRuleEntity.Rules[i].Percentage).ToString("f2") + "%";
                                if (item.SalesRuleEntity.Rules[i].StartAmt == 0.0m && item.SalesRuleEntity.Rules[i].EndAmt == 0.0m)
                                {
                                    item.SalesRuleStr += string.Format("按销售总额的 {0} 收取佣金<br/>", percentage);
                                    break;
                                }
                                else if (item.SalesRuleEntity.Rules[i].StartAmt == 0.0m)
                                    item.SalesRuleStr += string.Format("不超过 {0}元的部分，按销售总额的 {1} 收取佣金<br/>", endAmt, percentage);
                                else if (item.SalesRuleEntity.Rules[i].EndAmt == 0.0m)
                                    item.SalesRuleStr += string.Format("超过 {0}元的部分，按销售总额的 {1} 收取佣金<br/>", startAmt, percentage);
                                else
                                    item.SalesRuleStr += string.Format("超过 {0}元 至 {1}元的部分，按销售总额的 {2} 收取佣金<br/>", startAmt, endAmt, percentage);
                            }
                        }
                        #endregion
                        commissionItems_SAC.Add(item);
                        break;
                    case CommissionType.SOC:
                        item.SalesRuleStr = item.SalesRuleSOC.ToString("f2");
                        commissionItems_SOC.Add(item);
                        break;
                    case CommissionType.DEF:
                        item.SalesRuleStr = item.SalesRuleDEF.ToString("f2");
                        commissionItems_DEF.Add(item);
                        break;
                }
            }

            decimal SACSum = commissionItems_SAC.Sum(x => x.SalesCommissionFee);
            decimal SOCSum = commissionItems_SOC.Sum(x => x.OrderCommissionFee);
            decimal DEFSum = commissionItems_DEF.Sum(x => x.DeliveryFee);
            decimal SumAll = SACSum + SOCSum;// +DEFSum + rentFee;

            ViewBag.SAC = commissionItems_SAC;
            ViewBag.SOC = commissionItems_SOC;
            ViewBag.DEF = commissionItems_DEF;
            ViewBag.StatisticalInformation = string.Format("佣金信息汇总：销售提成 {0}元，订单提成 {1}元 总计：{2}元", SACSum.ToString("f2"), SOCSum.ToString("f2"), SumAll.ToString("f2"));
            ViewBag.CommissionMasterInfo = SettleService.GetCommissionMasterInfoBySysNo(sysno, UserAuthHelper.GetCurrentUser().SellerSysNo);
            ViewBag.VendorName = string.Format("{0} ({1})", UserAuthHelper.GetCurrentUser().SellerName, UserAuthHelper.GetCurrentUser().SellerSysNo);

            return View();
        }

        
        public ActionResult ViewCommissionLogDetail()
        {
            string type = string.Empty;
            if (Request["type"] != null)
            {
                type = Request["type"].ToString();
            }
            List<int> sysnoList = new List<int>();
            if (Request["Data"] != null && Request["Data"].ToString() != "")
            {
                foreach (var item in Request["Data"].ToString().Split(','))
                {
                    sysnoList.Add(int.Parse(item));
                }
            }

            string brandCategory = string.Empty;
            if (Request["brandCategory"] != null)
            {
                brandCategory = Request["brandCategory"].ToString();
            }


            List<CommissionItemLogDetailInfo> data = SettleService.QueryCommissionLogDetail(UserAuthHelper.GetCurrentUser().SellerSysNo, type, sysnoList);

            if (string.IsNullOrEmpty(type)
              || data == null)
            {
                return View();
            }

            int soCount = (from x in data
                           where x.ReferenceType.Trim().ToUpper() == OrdersType.SO
                           group x by x.ReferenceSysNo into g
                           select g).Count();
            int rmaCount = (from x in data
                            where x.ReferenceType.Trim().ToUpper() == OrdersType.RMA
                            group x by x.ReferenceSysNo into g
                            select g).Count();

            FillDataSource(data);
            switch ((CommissionType)Enum.Parse(typeof(CommissionType), type.ToUpper()))
            {
                case CommissionType.SAC:
                    {
                        ViewBag.SACData = data;
                        var totalAmtSO = data.FindAll(x => { return x.ReferenceType.Trim().ToUpper() == OrdersType.SO; }).Sum(x => { return x.Qty * x.Price; });
                        var totalAmtRMA = data.FindAll(x => { return x.ReferenceType.Trim().ToUpper() == OrdersType.RMA; }).Sum(x => { return x.Qty * x.Price; });
                        decimal totalPromotionDiscount = data.Where(x => x.PromotionDiscount.HasValue).Sum(x => x.PromotionDiscount.Value * x.Qty);
                        totalPromotionDiscount = Math.Round(totalPromotionDiscount, 2);
                        ViewBag.StatisticalInformation = string.Format("代理品类：{0}，销售金额总计：{1}元", brandCategory, (totalAmtSO + totalAmtRMA + totalPromotionDiscount).ToString("f2"));
                        break;
                    }
                case CommissionType.SOC:
                    {
                        ViewBag.SOCData = data;
                        ViewBag.StatisticalInformation = string.Format("代理品类：{0}，销售订单数：{1}；退货单数：{2}", brandCategory, soCount, rmaCount);
                        break;
                    }

                case CommissionType.DEF:
                    {
                        ViewBag.DEFData = data;
                        ViewBag.StatisticalInformation = string.Format("代理品类：{0}，销售订单数：{1}；", brandCategory, soCount);
                        break;
                    }
                default:
                    break;
            }

            return View();
        }

        private void FillDataSource(List<CommissionItemLogDetailInfo> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i].SN = i + 1;
            }

            //if (data != null)
            //{
            //    var oList = ResourceConfigHelper.ListConfig.GetListItems("CollectionSettleOrderType");

            //    data.ForEach(x =>
            //    {
            //        var item = oList.Find(y => x.ReferenceType == y.Value);
            //        if (item != null)
            //            x.ReferenceTypeDesc = item.Text;
            //    });
            //}
        }





        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FinancialReportDataText()
        {
            SalesStatisticsReportQueryFilter filter = SerializationUtility.JsonDeserialize2<SalesStatisticsReportQueryFilter>(Request.Form["data"]);
            filter.C1SysNo = filter.Category.C1SysNo == 0 ? null : filter.Category.C1SysNo;
            filter.C2SysNo = filter.Category.C2SysNo == 0 ? null : filter.Category.C2SysNo;
            filter.C3SysNo = filter.Category.C3SysNo == 0 ? null : filter.Category.C3SysNo;
            filter.SortFields = "B.ProductID DESC";

            int MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            filter.VendorSysNoList = new List<int>();
            filter.VendorSysNoList.Add(MerchantSysNo);
            filter.SOStatusList = new List<int>();
            if (filter.SOStatus.HasValue)
            {
                filter.SOStatusList.Add(filter.SOStatus.Value);
            }
            var result = FinancialReportService.SalesStatisticsReportQuery(filter);
            string text = "";
            if (result.SalesStatisticsResult.ResultList != null && result.SalesStatisticsResult.ResultList.Count > 0)
            {
                string page = string.Format("{0}： --- 销售成本：{1}  销售金额：{2}  优惠金额：{3}  实际销售金额：{4}  商品毛利：{5}",
                    "本页小计",
                    result.CostReportStatisticList[0].ProductCost,
                    result.CostReportStatisticList[0].ProductPriceAmount,
                    result.CostReportStatisticList[0].PromotionDiscountAmount,
                    result.CostReportStatisticList[0].ProductSaleAmount,
                    result.CostReportStatisticList[0].ProductGrossMargin);
                string Total = string.Format("{0}： --- 销售成本：{1}  销售金额：{2}  优惠金额：{3}  实际销售金额：{4}  商品毛利：{5}",
                    "全部总计",
                    result.CostReportStatisticList[1].ProductCost,
                    result.CostReportStatisticList[1].ProductPriceAmount,
                    result.CostReportStatisticList[1].PromotionDiscountAmount,
                    result.CostReportStatisticList[1].ProductSaleAmount,
                    result.CostReportStatisticList[1].ProductGrossMargin);
                text = "<div class='col-md-12' style='color:red;'>" + page + "</div><div class='col-md-12' style='color:red;'>" + Total + "</div>";
            }
            return Content(text);
        }

        /// <summary>
        /// 导出查询数据
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        [HttpPost]
        public FileResult AjaxFinancialExportData(string queryString)
        {
            //每页显示条数:
            SalesStatisticsReportQueryFilter getQueryEntity = SerializationUtility.JsonDeserialize2<SalesStatisticsReportQueryFilter>(queryString);
            getQueryEntity.C1SysNo = getQueryEntity.Category.C1SysNo == 0 ? null : getQueryEntity.Category.C1SysNo;
            getQueryEntity.C2SysNo = getQueryEntity.Category.C2SysNo == 0 ? null : getQueryEntity.Category.C2SysNo;
            getQueryEntity.C3SysNo = getQueryEntity.Category.C3SysNo == 0 ? null : getQueryEntity.Category.C3SysNo;
            getQueryEntity.SortFields = "B.ProductID DESC";

            int MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            getQueryEntity.VendorSysNoList = new List<int>();
            getQueryEntity.VendorSysNoList.Add(MerchantSysNo);
            getQueryEntity.SOStatusList = new List<int>();
            if (getQueryEntity.SOStatus.HasValue)
            {
                getQueryEntity.SOStatusList.Add(getQueryEntity.SOStatus.Value);
            }

            getQueryEntity.PageSize = 10000000;
            getQueryEntity.PageIndex = 0;

            
            SalesStatisticsReport result = FinancialReportService.SalesStatisticsReportQuery(getQueryEntity);


            List<DataTable> dataTableList = new List<DataTable>();
            if (result.SalesStatisticsResult.ResultList.Count <= 0)
            {
                throw new BusinessException("无数据可导出！！！");
            }


            DataTable newDt = ToDataTableTow(result.SalesStatisticsResult.ResultList);
            dataTableList.Add(newDt);

            List<ColumnData> columndatalist = new List<ColumnData>();
            ColumnData columndata = null;
            columndata = new ColumnData() { FieldName = "ProductID", Width = 20, Title = "商品ID" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductName", Width = 30, Title = "商品名称" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "C1Name", Width = 15, Title = "一级分类" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "C2Name", Width = 15, Title = "二级分类" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "C3Name", Width = 15, Title = "三级分类", ValueFormat = "" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "BeginDate", Width = 20, Title = "开始时间" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "EndDate", Width = 20, Title = "结束时间" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "BrandName", Width = 20, Title = "品牌" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "VendorName", Width = 20, Title = "供应商" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "StockName", Width = 20, Title = "仓库" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "BMCode", Width = 20, Title = "BM编号" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductProperty1", Width = 20, Title = "属性1" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductProperty2", Width = 20, Title = "属性2" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "Quantity", Width = 20, Title = "数量" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductCost", Width = 20, Title = "销售成本" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductPriceAmount", Width = 20, Title = "销售金额" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "PromotionDiscountAmount", Width = 20, Title = "优惠金额" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductSaleAmount", Width = 20, Title = "实际销售金额" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "PayTypeName", Width = 20, Title = "支付方式" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductGrossMargin", Width = 20, Title = "商品毛利" };
            columndatalist.Add(columndata);
            List<List<ColumnData>> columnList = new List<List<ColumnData>>();
            columnList.Add(columndatalist);
            //步骤5：调用生成Excel的公共方法，返回

            /*例如:
            IFileExport excelExport = new ExcelFileExporter();
            byte[] excelByte = excelExport.CreateFile(dataTableList, columnList, null, out fileName, "测试导出Excel");
            return File(new MemoryStream(excelByte), "application/ms-excel", fileName);
             */
            string fileName = "";
            IFileExport excelExport = new ExcelFileExporter();
            byte[] excelByte = excelExport.CreateFile(dataTableList, columnList, null, out fileName, "销售统计报表");
            return File(new MemoryStream(excelByte), "application/ms-excel", fileName);



        }


        #region   List<T>转换DataTable
        /// <summary>
        /// List<T>转换DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTableTow(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        #endregion
    }
}
