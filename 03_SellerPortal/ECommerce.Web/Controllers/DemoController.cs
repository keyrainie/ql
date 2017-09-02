using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Product;
using ECommerce.Service.Product;
using ECommerce.Web.Utility;
using ECommerce.Utility;
using ECommerce.WebFramework.Mail;

namespace ECommerce.Web.Controllers
{
    public class DemoController : WWWControllerBase
    {
        public ActionResult Layout()
        {
            return View();
        }


        public ActionResult AjaxPostError()
        {
            return View();
        }

        public ActionResult AjaxLoading()
        {
            return View();
        }

        public ActionResult HtmlEditor()
        {
            return View();
        }

        public ActionResult ExportExcel()
        {
            return View();
        }

        public ActionResult AjaxGridPagedQuery()
        {
            return View();
        }

        public ActionResult AlertAndConfirm()
        {
            return View();
        }

        #region Ajax Methods
        [HttpPost]
        public ActionResult GetDemoData()
        {
            System.Threading.Thread.Sleep(3000);
            List<string> dataList = new List<string>();
            dataList.Add("123123123");
            dataList.Add("22");
            dataList.Add("33");
            return Json(dataList);
        }

        [HttpPost]
        public ActionResult GetDemoDataWithUnhandledException()
        {
            System.Threading.Thread.Sleep(3000);
            return Json(Convert.ToInt32("asdfasdf"));
        }

        [HttpPost]
        public ActionResult GetDemoDataWithBizException()
        {
            System.Threading.Thread.Sleep(3000);
            throw new BusinessException("这里是后台代码抛出的BizException");
        }

        [HttpPost]
        public JsonResult QueryListDemo()
        {
            //每页显示条数:
            ProductListQueryFilter qFilter = BuildQueryFilterEntity<ProductListQueryFilter>(s =>
            {
                if (!string.IsNullOrEmpty(s.Status))
                {
                    s.Status = s.Status.Contains("-") ? null : s.Status;
                }
            });

            var result = ProductService.QueryProductList(qFilter);

            return AjaxGridJson(result);
        }

        #endregion

        [HttpPost]
        public FileResult ExportData(string queryString)
        {

            //步骤1：接收postData,得到查询条件:
            /*例如:
            var getQueryEntity = SerializationUtility.JsonDeserialize<object>(queryString);
            或者:
            var getQueryEntity = SerializationUtility.JsonDeserialize<object>(Request["xxName"]);
             */

            //步骤2：传入查询条件,调用Service相关的query静态方法
            /*例如:
            XXXService.QueryList(getQueryEntity);
             */

            //步骤3：返回List<T>,转换为DataTable

            /*例如:
             * DataTable dataTable = ConvertToDataTable(List);
             */

            //步骤4：构建需要输出的列信息，以及excel名称
            /*例如:
            List<ColumnData> columndatalist = new List<ColumnData>();
            ColumnData columndata = null;
            columndata = new ColumnData() { FieldName = "SysNo", Width = 30, Title = "系统编号" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "Name", Width = 30, Title = "用户名称" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "Time", Width = 30, Title = "创建时间" };
            columndatalist.Add(columndata);
            List<List<ColumnData>> columnList = new List<List<ColumnData>>();
            columnList.Add(columndatalist);
             */
            //步骤5：调用生成Excel的公共方法，返回

            /*例如:
            IFileExport excelExport = new ExcelFileExporter();
            byte[] excelByte = excelExport.CreateFile(dataTableList, columnList, null, out fileName, "测试导出Excel");
            return File(new MemoryStream(excelByte), "application/ms-excel", fileName);
             */


            //*******************以下为一个导出excel的例子(省去调用Service的代码):***************************
            List<DataTable> dataTableList = new List<DataTable>();
            DataTable newDt = new DataTable();
            newDt.Columns.Add("SysNo");
            newDt.Columns.Add("Name");
            newDt.Columns.Add("Time");
            for (int i = 0; i < 5000; i++)
            {
                DataRow dr1 = newDt.NewRow();
                dr1[0] = i + 1;
                dr1[1] = string.Format("测试名称_{0}", i);
                dr1[2] = DateTime.Now.ToString();
                newDt.Rows.Add(dr1);
            }
            dataTableList.Add(newDt);

            //2.构建Column信息:
            List<ColumnData> columndatalist = new List<ColumnData>();
            ColumnData columndata = null;
            columndata = new ColumnData() { FieldName = "SysNo", Width = 30, Title = "系统编号" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "Name", Width = 30, Title = "用户名称" };
            columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "Time", Width = 30, Title = "创建时间" };
            columndatalist.Add(columndata);
            List<List<ColumnData>> columnList = new List<List<ColumnData>>();
            columnList.Add(columndatalist);

            string fileName = "";

            IFileExport excelExport = new ExcelFileExporter();
            byte[] excelByte = excelExport.CreateFile(dataTableList, columnList, null, out fileName, "测试导出Excel");
            return File(new MemoryStream(excelByte), "application/ms-excel", fileName);
        }

        public ActionResult Validator()
        {

            return View();
        }

        public ActionResult BuildFormEntity()
        {
            return View();
        }


        public ActionResult EmailTemplate()
        {
            return View();
        }

        public ActionResult GetEmailTemplate()
        {
            MailTemplate template = MailHelper.GetMailTemplateByID("PO_AutoCloseMail");
            return Content(template.Body);
        }

        public ActionResult BuildEmailTemplate()
        {
            KeyValueVariables keyValues = new KeyValueVariables();
            keyValues.Add("StockName", "香港九龙仓");
            keyValues.Add("ETATime", DateTime.Now.Date);
            keyValues.Add("CompanyName", "公司名称");
            keyValues.Add("CompanyAddress", "公司地址");
            keyValues.Add("CompanyTel", "123456789");
            keyValues.Add("CompanyWebSite", "http://www.xxx.com");
            keyValues.Add("StockAddress", "仓库地址");
            keyValues.Add("StockContact", "仓库联系人");
            keyValues.Add("StockTel", "仓库电话");
            keyValues.Add("displayNo", "");


            keyValues.Add("numberString", "1231312312312321");
            keyValues.Add("shipTypeString", "默认 配送方式");


            keyValues.Add("totalInPage", "5000.00");
            keyValues.Add("totalAmt", "18000.00");
            keyValues.Add("totalReturnPoint", "100");
            keyValues.Add("PMName", "产品经理姓名");
            keyValues.Add("CurrencyName", "港币");

            keyValues.Add("entity.POID", "123");
            keyValues.Add("DateTime.Now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            keyValues.Add("vendor.VendorName", "供应商名称");
            keyValues.Add("vendor.SysNo", "123");
            keyValues.Add("vendor.Address", "供应商联系地址");
            keyValues.Add("vendor.Contact", "供应商联系人");
            keyValues.Add("vendor.Fax", "0123445");
            keyValues.Add("entity.InTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            keyValues.Add("entity.PayTypeName", "默认支付方式");
            keyValues.Add("entity.Memo", "备注信息备注信息备注信息备注信息备注信息");
            keyValues.Add("entity.InStockMemo", "入库备注入库备注入库备注入库备注入库备注入库备注入库备注");
            keyValues.Add("SendTimeString", DateTime.Now.ToString());

            KeyTableVariables keyTables = new KeyTableVariables();
            DataTable productAccessoryList = new DataTable();
            productAccessoryList.Columns.Add("ProductID");
            productAccessoryList.Columns.Add("AccessoriesID");
            productAccessoryList.Columns.Add("AccessoriesIDAndName");
            productAccessoryList.Columns.Add("Qty");


            DataTable productItemList = new DataTable();
            productItemList.Columns.Add("item.ProductID");
            productItemList.Columns.Add("item.IsVirtualStockProduct");
            productItemList.Columns.Add("item.ProductMode");
            productItemList.Columns.Add("item.BriefName");
            productItemList.Columns.Add("item.CurrencySymbol");
            productItemList.Columns.Add("item.OrderPrice");
            productItemList.Columns.Add("item.PurchaseQty");
            productItemList.Columns.Add("item.Quantity");
            productItemList.Columns.Add("item.PurchaseQtyOrderPrice");
            productItemList.Columns.Add("item.QuantityOrderPrice");


            for (int i = 0; i < 5; i++)
            {
                DataRow dr = productItemList.NewRow();
                dr["item.ProductID"] ="ProductID-"+i.ToString();
                dr["item.IsVirtualStockProduct"] = "IsVirtualStockProduct-" + i;
                dr["item.ProductMode"] = "ProductMode-" + i;
                dr["item.BriefName"] = "BriefName-" + i;
                dr["item.CurrencySymbol"] = "港币";
                dr["item.OrderPrice"] = (i + 1) * 100;
                dr["item.PurchaseQty"] = (i + 1) * 5;
                dr["item.Quantity"] = (i + 1) * 5;
                dr["item.PurchaseQtyOrderPrice"] = ((i + 1) * 5 * 100).ToString("#########0.00");
                dr["item.QuantityOrderPrice"] = ((i + 1) * 5 * 100).ToString("#########0.00");
                productItemList.Rows.Add(dr);
            }

            for (int i = 0; i < 5; i++)
            {
                DataRow dr = productAccessoryList.NewRow();
                dr["ProductID"] = "ProductID-" + i;
                dr["AccessoriesID"] = "AccessoriesID-" + i;
                dr["AccessoriesIDAndName"] = "AccessoriesIDAndName-" + i;
                dr["Qty"] = (i + 1) * 5;
                productAccessoryList.Rows.Add(dr);
            }

            keyTables.Add("tblProductItemsList", productItemList);
            keyTables.Add("tblProductAccessoryList", productAccessoryList);

            MailTemplate template = MailHelper.BuildMailTemplate("PO_AutoCloseMail", keyValues, keyTables);

            return Content(template.Body);
        }

        public ActionResult Form()
        {
            return View();
        }

    }
}
