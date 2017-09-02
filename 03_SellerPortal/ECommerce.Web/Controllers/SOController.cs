using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Web.Mvc;
using ECommerce.Entity.Common;
using ECommerce.Entity.Invoice;
using ECommerce.Entity.SO;
using ECommerce.Service.SO;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System.Data.OleDb;
using System.Data;
using System.Collections;

namespace ECommerce.Web.Controllers
{
    public class SOController : SSLControllerBase
    {
        #region ajax urls

        [HttpPost]
        public ActionResult AjaxSOQuery()
        {
            //每页显示条数:
            var qFilter = BuildQueryFilterEntity<SOQueryFilter>();
            qFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            QueryResult<SOInfo> result = SOService.SOQuery(qFilter);

            return AjaxGridJson(result);
        }

        /// <summary>
        /// SO批量操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSOOpreate()
        {
            var soSysNos = SerializationUtility.JsonDeserialize2<int[]>(Request.Form["data"]);
            var opreateName = Request.Form["opreateName"];
            switch (opreateName)
            {
                case "AuditAccept":
                    SOService.AuditAccept(soSysNos);
                    break;

                case "BatchVoid":
                    LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
                    SOService.BatchVoidSO(soSysNos, user: user);
                    break;
                case "BatchOutStock":
                    SOService.BatchOutStock(soSysNos);
                    break;
                case "BatcReported":
                    SOService.BatcReported(soSysNos);
                    break;
                case "BatchCustomsPass":
                    SOService.BatchCustomsPass(soSysNos);
                    break;

                default:
                    throw new BusinessException(LanguageHelper.GetText("无效的操作类型"));
            }
            
            return new JsonResult();
        }
        
        [HttpGet]
        public ActionResult AjaxIsNetPayed()
        {
            var soInfo = SOService.GetSOInfo(int.Parse(Request.QueryString["soSysNo"]));
            var result = new JsonResult
            {
                Data = soInfo.IsNetPayed == 1, 
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }

        /// <summary>
        /// 出库等待申报
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSOOutStock()
        {
            SOOutStockWaitReportRequest outStockWaitReportRequest = SerializationUtility.JsonDeserialize2<SOOutStockWaitReportRequest>(Request.Form["data"]);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            outStockWaitReportRequest.User = user;
            SOService.OutStockWaitReport(outStockWaitReportRequest);
            return new JsonResult();
        }
        
        /// <summary>
        /// 作废订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSOVoid()
        {
            SOIncomeRefundInfo refundInfo = SerializationUtility.JsonDeserialize2<SOIncomeRefundInfo>(Request.Form["data"]);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            SOService.VoidSO(refundInfo.SOSysNo.Value, refundInfo, loginUser: user);
            return new JsonResult();
        }

        [HttpPost]
        public ActionResult AjaxMaintainUpdate()
        {
            var soUpdateInfo = SerializationUtility.JsonDeserialize2<SOUpdateInfo>(Request.Form["data"]);
            SOService.SOUpdate(soUpdateInfo);
            return new JsonResult();
        }

        [HttpPost]
        public ActionResult AjaxMaintainPreview()
        {
            var soUpdateInfo = SerializationUtility.JsonDeserialize2<SOUpdateInfo>(Request.Form["data"]);
            SOService.SOUpdatePreview(soUpdateInfo);
            return new JsonResult { Data = soUpdateInfo };
        }

        /// <summary>
        /// 快递单号批量导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSaveExcel()
        {
            var data = SerializationUtility.JsonDeserialize2<List<ExcelInfo>>(Request.Form["data"]);
            if (!string.IsNullOrEmpty(data[0].Url))
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(".");
                System.Text.RegularExpressions.Match m = r.Match(data[0].Url);
                if (m.Success)
                {
                    string suffix = data[0].Url.Substring(data[0].Url.LastIndexOf(".") + 1);
                    if (suffix != "xls" && suffix != "xlsx" && suffix != "xlsm")
                    {
                        throw new BusinessException(LanguageHelper.GetText("只能导入Excel格式，请参考模板！！！"));
                    }
                }
            }
            string Url = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/UploadFiles/{0}", data[0].Url));
            DataTable dt = FileSvr.GetExcelDatatable(Url, data[0].Name);
            if (dt.Rows.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<SOTrackingInfos>");
                string UserID = UserAuthHelper.GetCurrentUser().UserID;
                int SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                int[] soSysNos = new int[dt.Rows.Count];
                ArrayList TrackingNumbers = new ArrayList();
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    SOOutStockWaitReportRequest outStockWaitReportRequest = new Entity.SO.SOOutStockWaitReportRequest();
                    outStockWaitReportRequest.Logistics = new LogisticsInfo();
                    //如果订单号为空，SOSysNo=0
                    if (!string.IsNullOrEmpty(dr[0].ToString()))
                    {
                        try
                        {
                            outStockWaitReportRequest.SOSysNo = int.Parse(dr[0].ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new BusinessException(LanguageHelper.GetText("订单号只能是正整数"));
                        }
                    }
                    else 
                    {
                        outStockWaitReportRequest.SOSysNo = 0;
                    }
                    outStockWaitReportRequest.Logistics.TrackingNumber = string.IsNullOrEmpty(dr[1].ToString()) ? "" : dr[1].ToString();
                    soSysNos.SetValue(outStockWaitReportRequest.SOSysNo, i++);
                    TrackingNumbers.Add(outStockWaitReportRequest.Logistics.TrackingNumber);
                    stringBuilder.Append("<SOTrackingInfo><SONumber>" + outStockWaitReportRequest.SOSysNo + "</SONumber><TrackingNumber>" + outStockWaitReportRequest.Logistics.TrackingNumber + "</TrackingNumber></SOTrackingInfo>");
                }
                stringBuilder.Append("</SOTrackingInfos>");
                SOService.CreatMoreSOSysNoAndTrackingNumber(stringBuilder.ToString(), UserID, SellerSysNo, soSysNos, TrackingNumbers);
                return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功") });
            }
            else
            {
                return Json(new { Error = true, Message = LanguageHelper.GetText("Excel没有数据") });
            }

        }

        #endregion

        public ActionResult BulkImport()
        {
            return View();
        }
        

        public ActionResult Query()
        {
            return View();
        }
        
        public ActionResult Detail()
        {
            return View();
        }

        
        public ActionResult Maintain()
        {
            return View();
        }

        #region 订单作废退款单
        
        public ActionResult AOQuery()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AOQuery")]
        public ActionResult AjaxAOQuery()
        {
            //每页显示条数:
            var qFilter = BuildQueryFilterEntity<AOQueryFilter>();
            qFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            QueryResult result = SOService.AOQuery(qFilter);

            return AjaxGridJson(result);
        }

        public ActionResult SORefundDialog()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Refund")]
        public JsonResult AjaxRefund(string SOSysNo)
        {
            int soSysNo;
            if (string.IsNullOrWhiteSpace(SOSysNo) || !int.TryParse(SOSysNo, out soSysNo) || soSysNo <= 0)
            {
                throw new BusinessException(ECommerce.WebFramework.LanguageHelper.GetText("订单编号不正确，退款失败"));
            }
            SOService.AORefund(soSysNo, UserAuthHelper.GetCurrentUser().SellerSysNo
                                        , UserAuthHelper.GetCurrentUser().UserSysNo);
            return Json(new { SOSysNo = soSysNo });
        }

        public ActionResult AORefundDialog()
        {
            return PartialView();
        }
        #endregion

        #region Excel批量导入
        public static class FileSvr
        {
            /// <summary>
            /// Excel第一个sheet数据导入Datable
            /// </summary>
            /// <param name="fileUrl">Excel文件绝对路径</param>
            /// <param name="table">Excel名称</param>
            /// <returns></returns>
            public static System.Data.DataTable GetExcelDatatable(string fileUrl, string excelName)
            {
                //office2007之前 仅支持.xls
                //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
                //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
                const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

                System.Data.DataTable dt = null;
                //建立连接
                OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
                try
                {
                    //打开连接
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    
                    
                    System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    //获取Excel的第一个Sheet名称
                    string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                    //查询sheet中的数据
                    string strSql = "select * from [" + sheetName + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, excelName);
                    dt = ds.Tables[0];
                    
                    return dt;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
        }
        #endregion
    }
}