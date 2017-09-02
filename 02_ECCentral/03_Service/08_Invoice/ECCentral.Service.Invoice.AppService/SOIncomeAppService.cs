using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 销售收款单应用层服务
    /// </summary>
    [VersionExport(typeof(SOIncomeAppService))]
    public class SOIncomeAppService
    {
        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity">待创建的销售收款单</param>
        /// <returns>创建后的销售收款单</returns>
        public virtual SOIncomeInfo Create(SOIncomeInfo entity)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 批量确认收款单
        /// </summary>
        /// <param name="batchActionData"></param>
        /// <returns></returns>
        public virtual BatchActionResult<SOIncomeInfo> BatchConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<SOIncomeInfo>> request = sysNoList.Select(x => new BatchActionItem<SOIncomeInfo>()
            {
                ID = x.ToString(),
                Data = new SOIncomeInfo()
                {
                    SysNo = x
                }
            }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, data =>
            {
                soIncomeBL.Confirm(data);
            });
            return result;
        }

        public string BatchForcesConfirm(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                soIncomeBL.ForcesConfirm(sysNo);
            });
            return result.PromptMessage;
        }

        #region 自动确认收款单

        /// <summary>
        /// 自动确认收款单
        /// </summary>
        /// <param name="fileIdentity">上传文件标识符</param>
        /// <param name="soOutFromDate">订单出库起始时间</param>
        /// <param name="soOutToDate">订单出库结束时间</param>
        /// <param name="successSysNoList">成功确认的订单系统编号列表</param>
        /// <param name="failedSysNoList">确认失败的订单系统编号列表，包括匹配失败的订单编号</param>
        /// <param name="submitConfirmCount">成功提交审核的订单数（仅仅是成功提交，不保证能审核成功）</param>
        /// <param name="failedMessage">失败信息</param>
        public virtual void AutoConfirm(string fileIdentity, DateTime? soOutFromDate, DateTime? soOutToDate,
            out List<int> successSysNoList, out List<int> failedSysNoList, out int submitConfirmCount, out string failedMessage)
        {
            FileStream fs = FileUploadManager.OpenFile(fileIdentity);
            DataTable dataTable = ProcessUploadFile(fs);
            if (dataTable.Rows.Count == 0)
            {
                // throw new ECCentral.BizEntity.BizException("上传记录格式不正确，请重新上传");
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_FormatError"));
            }
            if (dataTable.Rows.Count > 1000)
            {
                //throw new ECCentral.BizEntity.BizException("上传记录大于1000条，会造成系统不稳定，请减少记录重试");
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_UploadCountLimitError"));
            }

            List<int> soSysNoList = new List<int>();
            dataTable.AsEnumerable().Where(r => r["OrderSysNo"] != null && !string.IsNullOrEmpty(r["OrderSysNo"].ToString()))
            .ForEach(row =>
            {
                int soSysNo;
                if (int.TryParse(row["OrderSysNo"].ToString(), out soSysNo))
                {
                    soSysNoList.Add(soSysNo);
                }
            });

            DateTime? fromDateTime = null;
            DateTime? toDateTime = null;
            if (!soOutFromDate.HasValue)
            {
                fromDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else
            {
                fromDateTime = soOutFromDate.Value;
            }
            if (soOutToDate.HasValue)
            {
                toDateTime = soOutToDate.Value.AddDays(1).AddSeconds(-1);
            }

            var normalSaleIncomeList = new List<SOIncomeInfo>();
            var masterSaleIncomeList = new List<SOIncomeInfo>();
            var errorMessage = new StringBuilder();

            GetNeedConfirmSysNoList(soSysNoList, dataTable, fromDateTime, toDateTime, normalSaleIncomeList, masterSaleIncomeList, errorMessage);
            submitConfirmCount = normalSaleIncomeList.Count + masterSaleIncomeList.Count;
            ProcessAutoConfirm(normalSaleIncomeList, masterSaleIncomeList, dataTable, out successSysNoList, out failedSysNoList, errorMessage);

            failedMessage = errorMessage.ToString();
        }

        private void GetNeedConfirmSysNoList(List<int> soSysNoList, DataTable dataTable, DateTime? fromDateTime, DateTime? toDateTime
            , List<SOIncomeInfo> normalSaleIncomeList, List<SOIncomeInfo> masterSaleIncomeList, StringBuilder errorMessage)
        {
            var soIncomeInfoList = ObjectFactory<SOIncomeProcessor>.Instance.GetListBySOSysNoList(soSysNoList);
            var soInfoList = ExternalDomainBroker.GetSimpleSOInfoList(soSysNoList);

            #region 取得需要执行确认操作的收款单编号列表

            foreach (DataRow row in dataTable.Rows)
            {
                int orderSysNo = -1;
                decimal orderAmt = 0;

                if ((row["OrderSysNo"].ToString() != string.Empty && row["OrderAmt"].ToString() != string.Empty)
                        && (int.TryParse(row["OrderSysNo"].ToString(), out orderSysNo) && decimal.TryParse(row["OrderAmt"].ToString(), out orderAmt)))
                {
                    var getSOIncome = soIncomeInfoList.FirstOrDefault(w => w.OrderSysNo == orderSysNo);
                    if (getSOIncome == null)
                    {
                        //收款单不存在不能被确认
                        //errorMessage.AppendLine(string.Format("{0}:收款单不存在不能被确认", orderSysNo));
                        errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_NotExsistBill"), orderSysNo));
                        continue;
                    }

                    var getSO = soInfoList.FirstOrDefault(w => w.BaseInfo.SysNo == getSOIncome.OrderSysNo);
                    if (getSO == null)
                    {
                        //收款单对应的订单不存在不能被确认
                        //errorMessage.AppendLine(string.Format("{0}:收款单对应的订单不存在不能被确认", orderSysNo));
                        errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_NotExsistOrderForTheBill"), orderSysNo));
                        continue;
                    }

                    if (orderAmt != getSOIncome.IncomeAmt)
                    {
                        //单据金额与实收金额不相等的单据不能被确认
                        errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_BillAmountNotEqualTureth"), orderSysNo));
                        continue;
                    }
                    if (getSOIncome.Status != SOIncomeStatus.Origin && getSOIncome.Status != SOIncomeStatus.Splited)
                    {
                        //不是待确认状态的单据不能被确认
                        errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_OrderStatusIsNotToBeConfirm"), orderSysNo));
                        continue;
                    }

                    if (getSOIncome.Status == SOIncomeStatus.Splited)
                    {
                        // 验证是否有子单，确认母子单今额是否相等IncomeAmt。
                        // 如果相等，把子单加入normalIncomeData确认集合，母单放入另外集合最终把状态改为以处理，如果不相等加到errorMessage消息中。
                        CheckHasSubIncome(normalSaleIncomeList, masterSaleIncomeList, soIncomeInfoList, soInfoList, errorMessage, orderSysNo, orderAmt, getSOIncome, fromDateTime, toDateTime);
                    }
                    else
                    {
                        if (getSO.ShippingInfo.OutTime.HasValue)
                        {
                            //检查时间出库时间是否在指定范围如果在放入确认列表
                            if (IsOrginIncome(getSO, fromDateTime, toDateTime))
                            {
                                normalSaleIncomeList.Add(getSOIncome);
                            }
                            else
                            {
                                //errorMessage.AppendLine(string.Format("{0}:不在限定时间段内", orderSysNo));
                                errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_TimeNotInLimit"), orderSysNo));
                            }
                        }
                        else
                        {
                            //errorMessage.AppendLine(string.Format("{0}:无出库时间", orderSysNo));
                            errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_NoTimeForStock"), orderSysNo));
                        }
                    }
                }
            }

            #endregion 取得需要执行确认操作的收款单编号列表
        }

        private void ProcessAutoConfirm(List<SOIncomeInfo> normalSaleIncomeList, List<SOIncomeInfo> masterSaleIncomeList, DataTable dataTable,
            out List<int> successSysNoList, out List<int> failedSysNoList, StringBuilder errorMessage)
        {
            var BL = ObjectFactory<SOIncomeProcessor>.Instance;
            BatchActionResult<SOIncomeInfo> batchResult = new BatchActionResult<SOIncomeInfo>(0);
            if (normalSaleIncomeList.Count > 0)
            {
                var request = normalSaleIncomeList
                    .Select(x => new BatchActionItem<SOIncomeInfo>
                    {
                        ID = x.SysNo.ToString(),
                        Data = x
                    }).ToList();
                batchResult = BatchActionManager.DoBatchAction(request.ToList(), data => BL.Confirm(data));
            }
            if (masterSaleIncomeList.Count > 0)
            {
                BL.UpdateToProcessedStatus(masterSaleIncomeList);
            }
            if (batchResult.TotalCount != 0)
            {
                errorMessage.AppendLine("======== 收款单确认信息 ========");
                errorMessage.AppendLine(batchResult.PromptMessage);
            }

            //获取成功或失败的列表
            successSysNoList = (batchResult.SuccessList.Select(s => s.Data.OrderSysNo.Value)).ToList();
            if (masterSaleIncomeList != null && masterSaleIncomeList.Count > 0)
            {
                successSysNoList.Union(masterSaleIncomeList.Select(s => s.OrderSysNo.Value)).ToList();
            }
            int orderSysNo;
            failedSysNoList = dataTable.AsEnumerable().Where(r => int.TryParse(r["OrderSysNo"].ToString(), out orderSysNo))
               .Select(r => int.Parse(r["OrderSysNo"].ToString()))
               .Except(successSysNoList)
               .ToList();
        }

        private DataTable ProcessUploadFile(FileStream fs)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("OrderSysNo", typeof(int));
            dataTable.Columns.Add("OrderAmt", typeof(decimal));

            if (fs == null)
            {
                //throw new ECCentral.BizEntity.BizException("未指定上传的文件");
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_FileNotExsist"));
            }

            using (StreamReader reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    string veryLine = reader.ReadLine();
                    String[] dtArr = veryLine.Split('\t');
                    int orderSysNo;
                    decimal orderAmt;
                    if (int.TryParse(dtArr[0], out orderSysNo))
                    {
                        decimal.TryParse(dtArr[1], out orderAmt);
                        DataRow dr = dataTable.NewRow();
                        dr[0] = orderSysNo;
                        dr[1] = orderAmt;
                        dataTable.Rows.Add(dr);
                    }
                }
            }
            return dataTable;
        }

        private static void CheckHasSubIncome(List<SOIncomeInfo> normalSaleIncomeList, List<SOIncomeInfo> masterSaleIncomeList
            , List<SOIncomeInfo> soIncomeInfoList, List<SOInfo> soInfoList, StringBuilder errorMessage
            , int orderSysNo, decimal orderAmt, SOIncomeInfo masterSaleIncome, DateTime? fromDateTime, DateTime? toDateTime)
        {
            var resultData = soIncomeInfoList.Where(item =>
            {
                bool restult = false;
                SOInfo getSoData = soInfoList.FirstOrDefault(s => s.BaseInfo.SysNo == item.OrderSysNo);
                restult = getSoData.BaseInfo.Status == SOStatus.OutStock && item.Status == SOIncomeStatus.Origin && item.MasterSoSysNo == orderSysNo && getSoData.ShippingInfo.OutTime >= fromDateTime;
                if (toDateTime.HasValue)
                {
                    restult = getSoData.ShippingInfo.OutTime <= toDateTime;
                }
                return restult;
            });
            if (resultData.Count() > 0)
            {
                var sumInccomeAmt = resultData.Sum(p => p.IncomeAmt.Value);
                if (sumInccomeAmt == orderAmt)
                {
                    normalSaleIncomeList.AddRange(resultData.Select(s => s));
                    masterSaleIncomeList.Add(masterSaleIncome);
                }
                else
                {
                    //errorMessage.AppendLine(string.Format("{0}:母单金额与子单总金额不相等", orderSysNo));
                    errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_OrderAmountNotEqual"), orderSysNo));
                }
            }
            else
            {
                //errorMessage.AppendLine(string.Format("{0}:未找到匹配子单", orderSysNo)); 
                errorMessage.AppendLine(string.Format(ResouceManager.GetMessageString("Invoice.SOIncome", "SOIncome_NotMatchOrder"), orderSysNo));
            }
        }

        private bool IsOrginIncome(SOInfo item, DateTime? fromDateTime, DateTime? toDateTime)
        {
            if (!item.ShippingInfo.OutTime.HasValue)
            {
                return false;
            }
            if (item.ShippingInfo.OutTime.Value >= fromDateTime)
            {
                if (toDateTime.HasValue)
                {
                    if (item.ShippingInfo.OutTime.Value <= toDateTime.Value)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion 自动确认收款单

        /// <summary>
        /// 取消确认销售收款单
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        public virtual void CancelConfirm(int sysNo)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.CancelConfirm(sysNo);
        }

        /// <summary>
        /// 批量取消确认收款单
        /// </summary>
        /// <param name="batchData"></param>
        /// <returns></returns>
        public virtual string BatchCancelConfirm(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
                {
                    ID = x.ToString(),
                    Data = x
                }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                soIncomeBL.CancelConfirm(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 作废销售收款单
        /// </summary>
        /// <param name="entity">待作废的销售收款单系统编号</param>
        public virtual void Abandon(int sysNo)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.Abandon(sysNo);
        }

        /// <summary>
        /// 批量作废销售收款单
        /// </summary>
        /// <param name="sysNoList">收款单系统编号列表</param>
        /// <returns></returns>
        public virtual string BatchAbandon(List<int> sysNoList)
        {
            List<BatchActionItem<int>> Request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(Request, sysNo =>
            {
                soIncomeBL.Abandon(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 根据系统编号获取销售收款单
        /// </summary>
        /// <param name="sysNo">销售收款单系统编号</param>
        /// <returns>销售收款单</returns>
        public virtual SOIncomeInfo GetBySysNo(int sysNo)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 设置收款单凭证号
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="referenceID"></param>
        public void SetReferenceID(int sysNo, string referenceID)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.SetReferenceID(sysNo, referenceID);
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="referenceID">凭证号</param>
        public virtual string BatchSetReferenceID(List<int> sysNoList, string referenceID)
        {
            List<BatchActionItem<int>> Request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(Request, sysNo =>
            {
                soIncomeBL.SetReferenceID(sysNo, referenceID);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="incomeAmt">收款单实收金额</param>
        public virtual void SetIncomeAmt(int sysNo, decimal incomeAmt)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.SetIncomeAmount(sysNo, incomeAmt);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        public virtual SOIncomeInfo GetValid(int orderSysNo, SOIncomeOrderType orderType)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.GetValid(orderSysNo, orderType);
        }

        /// <summary>
        /// 手动网关退款
        /// </summary>
        /// <param name="sysNo">单据编号</param>
        public void ManualBankRefund(string sysNo)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.ManualBankRefund(sysNo);
        }

        /// <summary>
        /// 批量手动网关退款
        /// </summary>
        /// <param name="batchActionData"></param>
        /// <returns></returns>
        public string BatchManualRefund(List<int> sysNoList)
        {
            List<BatchActionItem<int>> request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, data =>
            {
                soIncomeBL.ManualBankRefund(data.ToString());
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量确认运费收入
        /// </summary>
        /// <param name="netpaySysNoList">系统编号列表</param>
        public string BatchSOFreightConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            SOIncomeProcessor soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (info) =>
            {
                soIncomeBL.SOFreightConfirm(info);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量确认运费支出
        /// </summary>
        /// <param name="netpaySysNoList">系统编号列表</param>
        public string BatchRealFreightConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            SOIncomeProcessor soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (info) =>
            {
                soIncomeBL.RealFreightConfirm(info);
            });
            return result.PromptMessage;
        }
        public virtual List<int> GetSysNoListByRefund()
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.GetSysNoListByRefund();
        }
        public virtual void QueryRefund(int sysNo)
        {
            (new TenPayUtils()).QueryRefund(sysNo);
        }
    }
}