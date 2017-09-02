using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using System.Data;
using System.Data.OleDb;
using System.IO;
namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ICountdownDA))]
    public class CountdownDA : ICountdownDA
    {
        #region ICountdownDA Members

        public List<CountdownInfo> GetCountDownByProductSysNo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadCountdownByProductSysNo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            List<CountdownInfo> result = command.ExecuteEntityList<CountdownInfo>();
            return result;
        }

        public CountdownInfo Load(int? sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadCountdownInfo");
            command.SetParameterValue("@SysNo", sysNo);
            CountdownInfo result = command.ExecuteEntity<CountdownInfo>();
            return result;
        }

        public bool HasDuplicateProduct(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckDuplicate");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            List<CountdownInfo> duplicateProducts = command.ExecuteEntityList<CountdownInfo>();

            return (duplicateProducts != null && duplicateProducts.Count > 0) ? true : false;
        }

        public void CreateCountdown(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateCountdown");


            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice ?? 0);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate ?? 0);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint ?? 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty ?? 0);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice ?? 0);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate ?? 0);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint ?? 0);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty ?? 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.IsSecondKill.HasValue && entity.IsSecondKill.Value ? "DC" : string.Empty);
            command.SetParameterValue("@IsLimitedQty", entity.IsLimitedQty);
            command.SetParameterValue("@IsReservedQty", entity.IsReservedQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsPromotionSchedule", entity.IsPromotionSchedule);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", entity.BaseLine);
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@IsHomePageShow", entity.IsHomePageShow);

            command.SetParameterValue("@IsC1Show", entity.IsC1Show.HasValue && entity.IsC1Show.Value ? "Y" : "N");
            command.SetParameterValue("@IsC2Show", entity.IsC2Show.HasValue && entity.IsC2Show.Value ? "Y" : "N");
            command.SetParameterValue("@IsTodaySpecials", entity.IsTodaySpecials.HasValue && entity.IsTodaySpecials.Value ? "Y" : "N");
            command.SetParameterValue("@Is24hNotice", entity.Is24hNotice.HasValue && entity.Is24hNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsShowPriceInNotice", entity.IsShowPriceInNotice.HasValue && entity.IsShowPriceInNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsEndIfNoQty", entity.IsEndIfNoQty.HasValue && entity.IsEndIfNoQty.Value ? "Y" : "N");
            command.SetParameterValue("@IsGroupOn", entity.IsGroupOn.HasValue ? entity.IsGroupOn.Value ? "Y" : "N" : null);

            command.SetParameterValue("@AreaShowPriority", entity.AreaShowPriority);
            command.SetParameterValue("@HomePagePriority", entity.HomePagePriority);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValueAsCurrentUserAcct("@CreateUserName");
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            //return entity;
        }

        public void CreatePromotionSchedule(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePromotionSchedule");


            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice ?? 0);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate ?? 0);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint ?? 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty ?? 0);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice ?? 0);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate ?? 0);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint ?? 0);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty ?? 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.IsSecondKill.HasValue && entity.IsSecondKill.Value ? "DC" : string.Empty);
            command.SetParameterValue("@IsLimitedQty", entity.IsLimitedQty);
            command.SetParameterValue("@IsReservedQty", entity.IsReservedQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsPromotionSchedule", entity.IsPromotionSchedule);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", entity.BaseLine);
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@IsHomePageShow", entity.IsHomePageShow);
            command.SetParameterValue("@IsTodaySpecials", entity.IsTodaySpecials.HasValue && entity.IsTodaySpecials.Value ? "Y" : "N");
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValueAsCurrentUserAcct("@CreateUserName");
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            //return entity;
        }

        public bool CheckRunningLimitedItem(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckRunningLimitedItem");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            int n = command.ExecuteScalar<int>();
            return n > 0 ? true : false;
        }

        public void MaintainPromotionSchedule(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainPromotionSchedule");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice ?? 0);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate ?? 0);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint ?? 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty ?? 0);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice ?? 0);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate ?? 0);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint ?? 0);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty ?? 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.IsSecondKill.HasValue && entity.IsSecondKill.Value ? "DC" : string.Empty);
            command.SetParameterValue("@IsLimitedQty", entity.IsLimitedQty);
            command.SetParameterValue("@IsReservedQty", entity.IsReservedQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            command.SetParameterValue("@IsPromotionSchedule", entity.IsPromotionSchedule);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", entity.BaseLine);
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@IsHomePageShow", entity.IsHomePageShow);

            command.SetParameterValue("@IsTodaySpecials", entity.IsTodaySpecials.HasValue && entity.IsTodaySpecials.Value ? "Y" : "N");

            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }

        public void MaintainCountdown(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdown");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice ?? 0);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate ?? 0);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint ?? 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty ?? 0);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice ?? 0);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate ?? 0);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint ?? 0);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty ?? 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.IsSecondKill.HasValue && entity.IsSecondKill.Value ? "DC" : string.Empty);
            command.SetParameterValue("@IsLimitedQty", entity.IsLimitedQty);
            command.SetParameterValue("@IsReservedQty", entity.IsReservedQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsPromotionSchedule", entity.IsPromotionSchedule);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", entity.BaseLine);
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@IsHomePageShow", entity.IsHomePageShow);
            command.SetParameterValue("@IsC1Show", entity.IsC1Show.HasValue && entity.IsC1Show.Value ? "Y" : "N");
            command.SetParameterValue("@IsC2Show", entity.IsC2Show.HasValue && entity.IsC2Show.Value ? "Y" : "N");
            command.SetParameterValue("@IsTodaySpecials", entity.IsTodaySpecials.HasValue && entity.IsTodaySpecials.Value ? "Y" : "N");
            command.SetParameterValue("@Is24hNotice", entity.Is24hNotice.HasValue && entity.Is24hNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsShowPriceInNotice", entity.IsShowPriceInNotice.HasValue && entity.IsShowPriceInNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsEndIfNoQty", entity.IsEndIfNoQty.HasValue && entity.IsEndIfNoQty.Value ? "Y" : "N");
            command.SetParameterValue("@IsGroupOn", entity.IsGroupOn.HasValue && entity.IsGroupOn.Value ? "Y" : "N");
            command.SetParameterValue("@AreaShowPriority", entity.AreaShowPriority);
            command.SetParameterValue("@HomePagePriority", entity.HomePagePriority);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.ExecuteNonQuery();
        }

        public void MaintainCountdownStatus(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdownStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.ExecuteNonQuery();
        }

        public List<CountdownInfo> CountItemHasReserveQtyNotRunning(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountItemHasReserveQtyNotRunning");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            List<CountdownInfo> result = command.ExecuteEntityList<CountdownInfo>();

            return result;
        }

        public void VerifyCountdown(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("VerifyCountdown");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@VerifyMemo", entity.VerifyMemo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValueAsCurrentUserAcct("@AuditUser");
            command.ExecuteNonQuery();
        }

        public void VerifyPromotionSchedule(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("VerifyPromotionSchedule");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.IsSecondKill.HasValue && entity.IsSecondKill.Value ? "DC" : string.Empty);
            command.SetParameterValue("@IsLimitedQty", entity.IsLimitedQty);
            command.SetParameterValue("@IsReservedQty", entity.IsReservedQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            command.SetParameterValue("@IsPromotionSchedule", entity.IsPromotionSchedule);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", entity.BaseLine);
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@IsHomePageShow", entity.IsHomePageShow);

            command.SetParameterValue("@IsTodaySpecials", entity.IsTodaySpecials.HasValue && entity.IsTodaySpecials.Value ? "Y" : "N");
            command.SetParameterValue("@Is24hNotice", entity.Is24hNotice.HasValue && entity.Is24hNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsShowPriceInNotice", entity.IsShowPriceInNotice.HasValue && entity.IsShowPriceInNotice.Value ? "Y" : "N");
            command.SetParameterValue("@IsEndIfNoQty", entity.IsEndIfNoQty.HasValue && entity.IsEndIfNoQty.Value ? "Y" : "N");
            command.SetParameterValue("@IsGroupOn", entity.IsGroupOn.HasValue && entity.IsGroupOn.Value ? "Y" : "N");
            command.SetParameterValue("@VerifyMemo", entity.VerifyMemo);
            command.SetParameterValueAsCurrentUserAcct("@AuditUser");
            command.ExecuteNonQuery();
        }


        #endregion

        public bool CheckConflict(int excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            if (productSysNos == null || productSysNos.Count == 0)
                throw new ArgumentException("productSysNos");
            string inProductSysNos = productSysNos[0].ToString();
            for (int i = 1; i < productSysNos.Count; i++)
            {
                inProductSysNos += "," + productSysNos[i].ToString();
            }
            DataCommand command = DataCommandManager.GetDataCommand("CheckProductInSCByDateTime");
            command.SetParameterValue("@SysNo", excludeSysNo);
            command.ReplaceParameterValue("#ProductSysNos#", inProductSysNos);
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);

            return command.ExecuteScalar<int>() > 0;
        }

        public bool CheckGroupBuyConflict(List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            if (productSysNos == null || productSysNos.Count == 0)
                throw new ArgumentException("productSysNos");
            string inProductSysNos = productSysNos[0].ToString();
            for (int i = 1; i < productSysNos.Count; i++)
            {
                inProductSysNos += "," + productSysNos[i].ToString();
            }
            DataCommand command = DataCommandManager.GetDataCommand("CheckProductInGroupBuyDateTime");
            command.ReplaceParameterValue("#ProductSysNos#", inProductSysNos);
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);

            return command.ExecuteScalar<int>() > 0;
        }




        #region ICountdownDA Members


        public List<BizEntity.Common.UserInfo> GetAllCountdownCreateUser(string channleID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllCountdownCreateUser");
            command.SetParameterValue("@ChannleID", channleID);
            return command.ExecuteEntityList<BizEntity.Common.UserInfo>();
        }

        public void SyncCountdownStatus(int requestSysNo, int status, string reason)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SyncCountdownStatus");

            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@Reason", reason);

            command.ExecuteScalar();
        }
        public List<ECCentral.BizEntity.PO.VendorInfo> GetCountdownVendorList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCountdownVendorList");
            List<ECCentral.BizEntity.PO.VendorInfo> list = command.ExecuteEntityList<ECCentral.BizEntity.PO.VendorInfo>();
            return list;
        }
        public void MainTainCountdownEndTime(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdownEndTime");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");


            command.ExecuteNonQuery();
        }
        #endregion

        /// <summary>
        /// 检查限时抢购价格不能低于捆绑销售折扣价格，请先处理对应Valid状态的捆绑销售折扣价格！
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CheckSaleRuleDiscount(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckSaleRuleDiscount");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice);
            int n = command.ExecuteScalar<Int32>();
            return n > 0;
        }




        /// <summary>
        /// 检查审核人和创建人是否相同 true:相同 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool CheckUser(int sysNo, int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CountdownCheckUser");
            cmd.SetParameterValue("@UserSysNo", userSysNo);
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }

        /// <summary>
        /// 读取上传的Execl转换DataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataTable ReadExcelFileToDataTable(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {

                    string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";

                    DataTable dt = new DataTable();
                    using (OleDbConnection Conn = new OleDbConnection(string.Format(strConn, fileName)))
                    {
                        string SQL = "select * from [Sheet1$] ";
                        Conn.Open();
                        OleDbDataAdapter da = new OleDbDataAdapter(SQL, string.Format(strConn, fileName));
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
            catch (Exception)
            {

                return null; 
            }
           
            return null;
        }

        /// <summary>
        /// 判断限时抢购创建权限
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool CheckCreatePermissions(int productSysNo, int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckCreatePermissions");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@UserSysNo", userSysNo);
            command.ExecuteNonQuery();
            var count = (int)command.GetParameterValue("@PMCount");
            return count > 0;
        }
    }
}
