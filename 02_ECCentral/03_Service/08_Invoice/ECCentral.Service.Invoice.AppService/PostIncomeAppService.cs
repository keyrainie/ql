using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 电汇邮局收款单应用层服务
    /// </summary>
    [VersionExport(typeof(PostIncomeAppService))]
    public class PostIncomeAppService
    {
        /// <summary>
        /// 创建电汇邮局收款单
        /// </summary>
        /// <param name="input">待创建的电汇邮局收款单</param>
        /// <returns>创建后的电汇邮局收款单</returns>
        public virtual PostIncomeInfo Create(PostIncomeInfo entity)
        {
            //批量创建时不需要验证SO#和OrderType
            return ObjectFactory<PostIncomeProcessor>.Instance.Create(entity, true);
        }

        /// <summary>
        /// 确认电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待确认的电汇邮局收款单系统编号</param>
        public virtual void Confirm(int sysNo)
        {
            ObjectFactory<PostIncomeProcessor>.Instance.Confirm(sysNo);
        }

        /// <summary>
        /// 批量确认电汇邮局收款单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            PostIncomeProcessor postIncomeBL = ObjectFactory<PostIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (sysNo) =>
            {
                postIncomeBL.Confirm(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 更新电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="confirmedSOSysNo">CS确认的订单号,多个订单号之间用逗号隔开</param>
        /// <returns></returns>
        public virtual void Update(PostIncomeInfo entity, string confirmedSOSysNo)
        {
            ObjectFactory<PostIncomeProcessor>.Instance.Update(entity, confirmedSOSysNo);
        }

        /// <summary>
        /// 取消确认电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待取消确认的电汇邮局收款单系统编号</param>
        public virtual void CancelConfirm(int sysNo)
        {
            ObjectFactory<PostIncomeProcessor>.Instance.CancelConfirm(sysNo);
        }

        /// <summary>
        /// 作废电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待作废的电汇邮局收款单系统编号</param>
        public virtual void Abandon(int sysNo)
        {
            ObjectFactory<PostIncomeProcessor>.Instance.Abandon(sysNo);
        }

        /// <summary>
        /// 取消作废电汇邮局收款单
        /// </summary>
        /// <param name="entity">待取消作废的电汇邮局收款单系统编号</param>
        public virtual void CancelAbandon(int sysNo)
        {
            ObjectFactory<PostIncomeProcessor>.Instance.CancelAbandon(sysNo);
        }

        /// <summary>
        /// 根据系统编号获取电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">电汇邮局收款单系统编号</param>
        /// <returns>电汇邮局收款单</returns>
        public virtual PostIncomeInfo GetBySysNo(int sysNo)
        {
            return ObjectFactory<PostIncomeProcessor>.Instance.LoadBySysNo(sysNo);
        }

        public void ImportPostIncome(string fileIdentity,string companyCode, ref List<PostIncomeInfo> successList, ref List<ImportPostIncome> faultList, ref string message)
        {
            if (FileUploadManager.FileExists(fileIdentity))
            {
                string destinationPath = string.Empty;
                MoveFile(fileIdentity, ref destinationPath);

                DataSet ds = ConvertExcel2DataSet(destinationPath);

                BatachCreate(ds.Tables[0],companyCode, ref successList, ref faultList, ref  message);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, "PostIncome_NoFile"));
            }
        }

        private void BatachCreate(DataTable dataTable, string companyCode, ref List<PostIncomeInfo> successList, ref List<ImportPostIncome> faultList, ref string message)
        {
            List<PostIncomeInfo> list = new List<PostIncomeInfo>();
            List<PostIncomeInfo> _successList = new List<PostIncomeInfo>();
            List<ImportPostIncome> _faultList = new List<ImportPostIncome>();

            dataTable.Columns.Add("SysNo");
            int index = 1;
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    if (string.IsNullOrEmpty(row["SOSysNo"].ToString())
                        && string.IsNullOrEmpty(row["IncomeAmt"].ToString())
                        && string.IsNullOrEmpty(row["PayUser"].ToString())
                        && string.IsNullOrEmpty(row["IncomeDate"].ToString())
                        && string.IsNullOrEmpty(row["PayBank"].ToString())
                        && string.IsNullOrEmpty(row["IncomeBank"].ToString())
                        && string.IsNullOrEmpty(row["BankNo"].ToString())
                        && string.IsNullOrEmpty(row["Notes"].ToString()))
                    {
                        break;
                    }
                    PostIncomeInfo entity = new PostIncomeInfo();
                    entity.SysNo = index;
                    entity.SOSysNo = string.IsNullOrEmpty(row["SOSysNo"].ToString()) ? default(int?) : int.Parse(row["SOSysNo"].ToString());
                    entity.IncomeAmt = decimal.Parse(row["IncomeAmt"].ToString());
                    entity.PayUser = row["PayUser"].ToString();
                    entity.IncomeDate = DateTime.Parse(row["IncomeDate"].ToString());
                    entity.PayBank = row["PayBank"].ToString();
                    entity.IncomeBank = row["IncomeBank"].ToString();
                    entity.BankNo = row["BankNo"].ToString();
                    entity.Notes = row["Notes"].ToString();
                    entity.CompanyCode = companyCode;
                    index++;

                    list.Add(entity);
                }
                catch (Exception)
                {
                    ImportPostIncome model = new ImportPostIncome();
                    model.SysNo = index++;
                    model.SOSysNo = row["SOSysNo"].ToString();
                    model.IncomeAmtString = row["IncomeAmt"].ToString();
                    model.PayUser = row["PayUser"].ToString();
                    model.IncomeDateString = row["IncomeDate"].ToString();
                    model.PayBank = row["PayBank"].ToString();
                    model.IncomeBank = row["IncomeBank"].ToString();
                    model.BankNo = row["BankNo"].ToString();
                    model.Notes = row["Notes"].ToString();
                    _faultList.Add(model);
                }
            }

            List<BatchActionItem<PostIncomeInfo>> items = list.Select(x => new BatchActionItem<PostIncomeInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            var result = BatchActionManager.DoBatchAction(items, (PostIncomeInfo) =>
            {
                ObjectFactory<PostIncomeProcessor>.Instance.Create(PostIncomeInfo, false);
            });

            if (result.SuccessList != null)
            {
                result.SuccessList.ForEach(p =>
                {
                    p.Data.IncomeAmt = p.Data.IncomeAmt ?? 0;
                    _successList.Add(p.Data);
                });
            }
            if (result.FaultList != null)
            {
                result.FaultList.ForEach(p =>
                {
                    ImportPostIncome failed = new ImportPostIncome();
                    failed.SysNo = p.FaultItem.Data.SysNo;
                    failed.SOSysNo = p.FaultItem.Data.SOSysNo.ToString();
                    failed.IncomeAmtString = p.FaultItem.Data.IncomeAmt.ToString();
                    failed.PayUser = p.FaultItem.Data.PayUser;
                    failed.IncomeDateString = p.FaultItem.Data.IncomeDate.ToString();
                    failed.PayBank = p.FaultItem.Data.PayBank;
                    failed.IncomeBank = p.FaultItem.Data.IncomeBank;
                    failed.BankNo = p.FaultItem.Data.BankNo;
                    failed.Notes = p.FaultItem.Data.Notes;
                    _faultList.Add(failed);
                });

                faultList.Sort(new ImportPostIncome());
            }

            successList = _successList;
            faultList = _faultList;
            message = string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, "PostIncome_ImportExcel"), dataTable.Rows.Count, successList.Count, faultList.Count);
        }

        private DataSet ConvertExcel2DataSet(string destinationPath)
        {
            string strConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + destinationPath + ";Extended Properties=Excel 12.0;";
            DataSet ds = new DataSet();
            using (OleDbConnection Conn = new OleDbConnection(strConn))
            {
                string SQL = "select * from [sheet1$]";
                Conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(SQL, strConn);
                da.Fill(ds);
            }

            return ValidExcelData(ds);
        }

        private DataSet ValidExcelData(DataSet ds)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, "PostIncome_DataIsEmpty"));
            }
            if (ds.Tables[0].Rows.Count > 1000)
            {
                throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, "PostIncome_DataLimit"));
            }
            string[] columnNameArray = new string[] { "SOSysNo", "IncomeAmt", "PayUser", "IncomeDate", "PayBank", "IncomeBank", "BankNo", "Notes" };
            for (int i = 0; i < columnNameArray.Length; i++)
            {
                if (!ds.Tables[0].Columns.Contains(columnNameArray[i]))
                {
                    throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, "PostIncome_ExcelDataInvalid"));
                }
            }
            return ds;
        }

        private void MoveFile(string fileIdentity, ref string destinationPath)
        {
            string configPath = AppSettingManager.GetSetting("Invoice", "PostIncomeFilesPath");
            if (!Path.IsPathRooted(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
            }
            destinationPath = Path.Combine(configPath, fileIdentity);
            string folder = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            FileUploadManager.MoveFile(fileIdentity, destinationPath);
        }
    }
}