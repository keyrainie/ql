using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;

namespace IPP.Oversea.CN.ServiceMgmt.Job.CalculateRepeat
{
    public class CalculateRepeat : IJobAction
    {
        string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        string Opertor = ConfigurationManager.AppSettings["Operator"];


        private RepeaterHistoryData repeaterHistoryData = null;

        public void Run(JobContext context)
        {
            GetHistoryRunData();
            //只在系统第一次运行的时候计算一次历史数据
            if (IsFirstRun())
            {
                UpdateHistory();
                ChangeFirstRunStatus();
            }
            else
            {
                //当月第一天，把上上个月的数据放到历史记录里
                if (DateTime.Now.Day == 1)
                {
                    if (!TransformedMonth())
                    {
                        TransformMonth();
                        ChangeTransformMonthStatus();
                    }
                }
            }

            //计算当前月和上个月的复合单件数
            UpdateCurrent();
        }

        private void UpdateCurrent()
        {
            List<RepeaterEntity> toAddOrUpdateList = null;
            toAddOrUpdateList = CalculateRepeatCount(DateTime.Now.Date.AddMonths(-1).AddDays(-DateTime.Now.Day + 1), DateTime.Now);
            UpdateCurrentData(toAddOrUpdateList);
        }

        private void UpdateHistory()
        {
            List<RepeaterEntity> toAddOrUpdateList = null;
            toAddOrUpdateList = CalculateRepeatCount(repeaterHistoryData.HistoryDataBeginDate.Value, repeaterHistoryData.HistoryDataEndDate.Value);
            PersistHistory(toAddOrUpdateList);
        }

        private void TransformMonth()
        {
            DateTime lastMonth = DateTime.Now.Date.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
            DateTime lastLastMonth = lastMonth.AddMonths(-1);
            List<RepeaterEntity> toAddOrUpdateList = CalculateRepeatCount(lastLastMonth, lastMonth);
            TransformData(toAddOrUpdateList);
        }

        private List<RepeaterEntity> CalculateRepeatCount(DateTime from, DateTime to)
        {
            int fromRow = 0;
            int toRow = 1000;
            List<RepeaterEntity> registers = null;
            List<RepeaterEntity> toAddOrUpdateList = new List<RepeaterEntity>();
            RepeaterEntity toAddOrUpdate = null;
            RepeaterEntity oldRepeatEntity = new RepeaterEntity();

            registers = GetToCalculateData(fromRow, toRow, from, to);

            while (registers != null && registers.Count > 0)
            {
                foreach (RepeaterEntity entity in registers)
                {
                    if (oldRepeatEntity.SOSysNo == entity.SOSysNo && oldRepeatEntity.ProductSysNo == entity.ProductSysNo)
                    {
                        if (oldRepeatEntity.RequestSysNo != entity.RequestSysNo)
                        {
                            toAddOrUpdate.RepeatCount += 1;
                        }
                    }
                    else
                    {
                        toAddOrUpdate = new RepeaterEntity();
                        toAddOrUpdate.SOSysNo = entity.SOSysNo;
                        toAddOrUpdate.ProductSysNo = entity.ProductSysNo;
                        toAddOrUpdate.RepeaterSysNo = entity.RepeaterSysNo;
                        toAddOrUpdate.RepeatCount = 1;
                        toAddOrUpdate.RepeatProductCount = entity.RepeatProductCount;
                        toAddOrUpdateList.Add(toAddOrUpdate);
                    }

                    oldRepeatEntity = entity;
                }

                if (registers.Count < 1000)
                {
                    break;
                }
                else
                {
                    fromRow += 1000;
                    toRow += 1000;
                    registers = GetToCalculateData(fromRow, toRow, from, to);
                }
            }

            return toAddOrUpdateList;
        }

        private List<RepeaterEntity> GetToCalculateData(int fromRow, int toRow, DateTime createTimeFrom, DateTime createTimeTo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetToCalculateData");
            dc.SetParameterValue("@From", fromRow);
            dc.SetParameterValue("@To", toRow);
            dc.SetParameterValue("@CreateTimeFrom", createTimeFrom);
            dc.SetParameterValue("@CreateTimeTo", createTimeTo);
            dc.SetParameterValue("@CompanyCode", CompanyCode);
            return dc.ExecuteEntityList<RepeaterEntity>();
        }

        private void PersistHistory(List<RepeaterEntity> toAddOrUpdateList)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateHistoryData");
            foreach (RepeaterEntity entity in toAddOrUpdateList)
            {
                dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
                dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
                dc.SetParameterValue("@RepeatCount", entity.RepeatCount);
                dc.SetParameterValue("@RepeatProductCount", entity.RepeatProductCount);
                dc.SetParameterValue("@CompanyCode", CompanyCode);
                dc.SetParameterValue("@CreateUserSysNo", Opertor);
                dc.ExecuteNonQuery();
            }
        }

        private void TransformData(List<RepeaterEntity> toAddOrUpdateList)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TransformData");
            foreach (RepeaterEntity entity in toAddOrUpdateList)
            {
                dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
                dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
                dc.SetParameterValue("@RepeatCount", entity.RepeatCount);
                dc.SetParameterValue("@RepeatProductCount", entity.RepeatProductCount);
                dc.SetParameterValue("@CompanyCode", CompanyCode);
                dc.SetParameterValue("@CreateUserSysNo", Opertor);
                dc.ExecuteNonQuery();
            }
        }

        private void UpdateCurrentData(List<RepeaterEntity> toAddOrUpdateList)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateCurrentData");
            foreach (RepeaterEntity entity in toAddOrUpdateList)
            {
                dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
                dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
                dc.SetParameterValue("@RepeatCount", entity.RepeatCount);
                dc.SetParameterValue("@RepeatProductCount", entity.RepeatProductCount);
                dc.SetParameterValue("@CompanyCode", CompanyCode);
                dc.SetParameterValue("@CreateUserSysNo", Opertor);
                dc.ExecuteNonQuery();
            }
        }

        private bool IsFirstRun()
        {
            if (repeaterHistoryData == null)
            {
                repeaterHistoryData = new RepeaterHistoryData();
                repeaterHistoryData.HistoryDataBeginDate = DateTime.Parse("2000-1-1");
                repeaterHistoryData.HistoryDataEndDate = DateTime.Parse("2010-02-01");
                repeaterHistoryData.FirstRun = 1;
            }

            if (!repeaterHistoryData.FirstRun.HasValue || repeaterHistoryData.FirstRun == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeFirstRunStatus()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ChangeFirstRunStatus");
            dc.SetParameterValue("@From", DateTime.Parse("2000-1-1"));
            dc.SetParameterValue("@To", DateTime.Parse("2010-02-01"));

            dc.ExecuteNonQuery();
        }

        private bool TransformedMonth()
        {
            if (!repeaterHistoryData.LastTransformMonth.HasValue
                || repeaterHistoryData.LastTransformMonth.Value != DateTime.Now.AddMonths(-2).Month)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ChangeTransformMonthStatus()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ChangeTransformMonthStatus");
            dc.SetParameterValue("@Month", DateTime.Now.AddMonths(-2).Month);

            dc.ExecuteNonQuery();
        }

        private void GetHistoryRunData()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetHistoryRunData");
            repeaterHistoryData = dc.ExecuteEntity<RepeaterHistoryData>();
        }
    }
}