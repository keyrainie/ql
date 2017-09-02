using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.AppService
{
    public class PriceChangePrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string sysNo = requestPostData["SysNo"];
            if (sysNo != null && sysNo.Trim() != String.Empty)
            {
                sysNo = System.Web.HttpUtility.UrlDecode(sysNo);
                int requestSysNo = int.TryParse(sysNo, out requestSysNo) ? requestSysNo : int.MinValue;

                if (requestSysNo > int.MinValue)
                {
                    PriceChangeMaster requestInfo = ObjectFactory<PriceChangeAppService>.Instance.GetPriceChangeBySysNo(requestSysNo);

                    if (requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.SalePrice)
                    {
                        variables.Add("PrintTitle", "销售变价单");
                    }
                    else if (requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.PurchasePrice)
                    {
                        variables.Add("PrintTitle", "采购变价单");
                    }
                    variables.Add("SysNo", requestInfo.SysNo.ToString().Trim().PadLeft(8, '0'));
                    variables.Add("RealBeginDate", requestInfo.RealBeginDate.HasValue ? requestInfo.RealBeginDate.Value.ToLongDateString().Trim() : string.Empty);
                    variables.Add("BeginDate", requestInfo.BeginDate.HasValue ? requestInfo.BeginDate.Value.ToLongDateString().Trim() : string.Empty);
                    variables.Add("EndDate", requestInfo.EndDate.HasValue ? requestInfo.EndDate.Value.ToLongDateString().Trim() : string.Empty);
                    variables.Add("BillInDate", DateTime.Now.ToLongDateString());
                    variables.Add("IsPurchasePrice", requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.PurchasePrice);
                    variables.Add("IsSalePrice", requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.SalePrice);

                    DataTable dtProduct = new DataTable();
                    dtProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("OldPrice"), 
                            new DataColumn("NewPrice"), 
                            new DataColumn("DiffPrice"), 

                            new DataColumn("OldShowPrice"), 
                            new DataColumn("NewShowPrice"), 

                            new DataColumn("OldInstockPrice"), 
                            new DataColumn("NewInstockPrice"),
                            new DataColumn("DiffInstockPrice"),

                            new DataColumn("IsPurchasePrice"),
                            new DataColumn("IsSalePrice")
                        });

                    if (requestInfo.ItemList != null)
                    {
                        requestInfo.ItemList.ForEach(p =>
                        {
                            DataRow drProduct = dtProduct.NewRow();
                            drProduct["ProductID"] = p.ProductID;
                            drProduct["ProductName"] = p.ProductName.Trim();
                            drProduct["OldPrice"] = p.OldPrice.ToString("N2");
                            drProduct["NewPrice"] = p.NewPrice == 0 ? "---" : p.NewPrice.ToString("N2");
                            drProduct["DiffPrice"] = (p.NewPrice - p.OldPrice).ToString("N2");

                            drProduct["OldShowPrice"] = p.OldShowPrice.ToString("N2");
                            drProduct["NewShowPrice"] = p.NewShowPrice == 0 ? "---" : p.NewShowPrice.ToString("N2");
                            drProduct["OldInstockPrice"] = p.OldInstockPrice.ToString("N2");
                            drProduct["NewInstockPrice"] = p.NewInstockPrice == 0 ? "---" : p.NewInstockPrice.ToString("N2");
                            drProduct["DiffInstockPrice"] = (p.NewInstockPrice - p.OldInstockPrice).ToString("N2");

                            drProduct["IsPurchasePrice"] = requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.PurchasePrice;
                            drProduct["IsSalePrice"] = requestInfo.PriceType == BizEntity.Invoice.RequestPriceType.SalePrice;

                            dtProduct.Rows.Add(drProduct);
                        });
                    }

                    tableVariables.Add("ProductList", dtProduct);

                }
            }

        }
    }
}
