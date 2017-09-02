using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoClose.Model;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using IPP.POASNMgmt.AutoClose;
using Newegg.Oversea.Framework.DataAccess;

namespace AutoClose.DAL
{
    public static class EmailDA
    {
        public static PO QueryPOEntity(int poSysNo)
        {
            PO entity = new PO();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPOBySysNo");

            cmd.SetParameterValue("@SysNo", poSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            entity = cmd.ExecuteEntity <PO>();

            if (entity != null)
            {
                entity.PayTypeName = ((VendorPayType)entity.PayTypeSysNo).GetDesc();
                entity.SettlementCompanyName = ((SettlementCompany)entity.SettlementCompany).GetDesc();
                
                    if (entity.UsingReturnPoint < 0)
                    {
                        entity.UsingReturnPoint = 0;
                    }
                    else
                    {
                       // entity.UsingReturnPoint = Convert.ToDecimal(entity.UsingReturnPoint).ToString("#########0.00");
                    }
               

                entity.StatusDes = ((POStatus)entity.Status).GetDesc();
               // entity.ETP = entity.ETP != null && entity.ETP != DateTime.Parse("1900-1-1").ToString() ? Convert.ToDateTime(entity.ETP).ToShortDateString() : "";
               // entity.ETATime = entity.ETATime != null && entity.ETATime.ToString() != DateTime.Parse("1900-1-1").ToString() ? Convert.ToDateTime(entity.ETATime).ToShortDateString() : "";
                entity.VendorStatusDes = ((VendorStatus)entity.VendorStatus).GetDesc();
                entity.POTypeName = ((POType)entity.POType).GetDesc();
                entity.TaxRateDes = decimal.Round((entity.TaxRate * 100), 0).ToString() + "%";
                entity.IsConsignName = ((POIsConsign)(int)entity.IsConsign).GetDesc();
                if (entity.CurrencySymbol == null && entity.CurrencySymbol == "")
                {
                    entity.CurrencySymbol = entity.LocalCurrencySymbol;
                }
               // entity.TotalAmt = entity.CurrencySymbol + Convert.ToDecimal(entity.TotalAmt).ToString("#########0.00");
                //if (entity.ComfirmUserSysNo.HasValue)
                //{
                //    entity.ComfirmTimeShow = QueryProviderFactory.GetQueryProvider<IQueryCommon>().GetDisplayName(entity.ComfirmUserSysNo.Value) + "[" + entity.ComfirmTime + "]";
                //}
                entity.CarriageCost = Convert.ToDecimal(entity.CarriageCost.ToString("#########0.00"));

                if (entity.StockSysNo == 50)
                {
                    string resultName;
                    int resultSysNo;
                    if (entity.ITStockSysNo.HasValue)
                    {
                        switch (entity.ITStockSysNo.Value)
                        {
                            case 52:
                                resultName = "经中转到北京仓";
                                resultSysNo = 5052;
                                break;
                            case 53:
                                resultName = "经中转到广州仓";
                                resultSysNo = 5053;
                                break;
                            case 54:
                                resultName = "经中转到成都仓";
                                resultSysNo = 5054;
                                break;
                            case 55:
                                resultName = "经中转到武汉仓";
                                resultSysNo = 5055;
                                break;
                            case 56:
                                resultName = "经中转到西安仓";
                                resultSysNo = 5056;
                                break;
                            case 57:
                                resultName = "经中转到济南仓";
                                resultSysNo = 5057;
                                break;
                            default:
                                resultName = "经中转到南京仓";
                                resultSysNo = 5058;
                                break;
                        }
                    }
                    else
                    {
                        resultName = "中转仓";
                        resultSysNo = 50;
                    }
                    entity.StockName = resultName;
                    entity.StockSysNo = resultSysNo;
                }

            }
            return entity;
        }
        public static PO GetReturnPoint(PO entity)
        {
            if (entity.PM_ReturnPointSysNo != null)
            {
                if (Convert.ToInt32(entity.PM_ReturnPointSysNo) > 0)
                {
                    ReturnPointList returnPoint = GetReturnPointNameBySysNo(Convert.ToInt32(entity.PM_ReturnPointSysNo));
                    if (returnPoint != null)
                    {
                        entity.ReturnPointName = returnPoint.ReturnPointName;
                        entity.RemnantReturnPoint = returnPoint.RemnantReturnPoint.ToString("#########0.00");
                    }
                }
                else
                {
                    entity.PM_ReturnPointSysNo = 0;
                }
            }

            return entity;

        }
        /// <summary>
        /// 根据SysNo获得货币实体信息
        /// </summary>
        /// <returns></returns>
        public static AutoClose.Model.CurrencyInfo QueryCurrencyInfoBySysNo(int SysNo)
        {
            CurrencyInfo result = new AutoClose.Model.CurrencyInfo();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCurrencyBySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            //DataCommand selectCommand = DataCommandManager.GetDataCommand("GetCurrencyBySysNo");
            //ProviderHelper.SetCommonParams(selectCommand);
            //selectCommand.SetParameterValue("@SysNo", SysNo);

            List<CurrencyInfo> list = cmd.ExecuteEntityList<CurrencyInfo>();
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        public static string GetPMNameBySysNo(int pmSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPMNameBySysNo");
            cmd.SetParameterValue("@UserSysNo", pmSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            object o = cmd.ExecuteScalar();

            return o.ToString();
        }
        private static ReturnPointList GetReturnPointNameBySysNo(int? p)
        {
            return null;
        }

        /// <summary>
        ///  获得键值对的Value,获得PO的流水账号,在打印的时候使用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetSysConfiguration(string key)
        {

            DataCommand cmd = DataCommandManager.GetDataCommand("GetSysConfiguration");
            Dictionary<int, string> kv = new Dictionary<int, string>();
            cmd.SetParameterValue("@Key", key);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            using (DataTable das = cmd.ExecuteDataSet().Tables[0])
            {
                foreach (DataRow dr in das.Rows)
                {
                    kv.Add((int)dr["SysNo"], dr["Value"] as string);
                }
            }
            return kv;
        }
        public static string GetWHReceiptSN(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetWHReceiptSNBySysNo");
            cmd.SetParameterValue("@SysNo", poSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            object o = cmd.ExecuteScalar();
            if (o == null)
            {
                return "";
            }
            else
            {
                return o.ToString();
            }
        }
        public static Vendor GetVendorBySysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryVendorbySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            List<Vendor> vendors = cmd.ExecuteEntityList<Vendor>();
            if (vendors.Count > 0)
            {
                return vendors[0];
            }
            else
            {
                return null;
            }
        }
        //获得送货方式
        public static string GetShipTypeList(int shipTypeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShipTypeName");
            cmd.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return Convert.ToString(cmd.ExecuteScalar());
        }
    }
    

    /// <summary>
    /// PO类型
    /// </summary>
    public enum POType
    {
        [Description("正常")]
        Normal = 0,
        [Description("负采购")]
        Return = 1,
        [Description("历史负采购")]
        HistoryReturn = 2,
        [Description("调价单")]
        AdjustPrice = 3,

    }

    public enum POIsConsign
    {
        UnConsign = 0,
        IsConsign = 1,
        TempConsign = 2

    }
    public enum VendorPayType
    {
        [Description("款到发货（预付）")]
        MoneyInItemOut = 1,
        [Description("货到付款")]
        ItemInMoneyIn = 2,
        [Description("货到后2天(*)")]
        ItemInAfter2 = 3,
        [Description("货到后每周一(*)")]
        ItemInEveryMonday = 4,
        [Description("货到当天，且票到")]
        ItemInPayIn = 5,
        [Description("货到后3天，且票到")]
        ItemInAfter3PayIn = 6,
        [Description("货到后7天，且票到")]
        ItemInAfter7PayIn = 7,
        [Description("货到后14天，且票到")]
        ItemInAfter14PayIn = 8,
        [Description("货到后30天，且票到")]
        ItemInAfter30PayIn = 9,
        [Description("每月25日，且票到(*)")]
        ItemInEvery25PayIn = 10,
        [Description("每月10日及25日，且票到(*)")]
        ItemInEvery1525PayIn = 11,
        [Description("代销")]
        Consign = 12,
        [Description("货与票都到后15天(*)")]
        ItemInPayInAfter15 = 13,
        [Description("货与票都到后30天(*)")]
        ItemInPayInAfter30 = 14,
        [Description("货与票都到后20天(*)")]
        ItemInPayInAfter20 = 15,
        [Description("货与票都到后7天(*)")]
        ItemInPayInAfter7 = 16,
        [Description("货与票都到后45天(*)")]
        ItemInPayInAfter45 = 17,
        [Description("货与票都到后当天(*)")]
        ItemInPayInAfter0 = 18,
        [Description("月结")]
        ItemInPayByMonth = 19,
        [Description("货到后18天，且票到")]
        ItemInAfter18PayIn = 20,
        [Description("货到后20天，且票到")]
        ItemInAfter20PayIn = 21,
        [Description("货到后45天，且票到")]
        ItemInAfter45PayIn = 22,
        [Description("货到后14天")]
        ItemInAfter14 = 23,
        [Description("货到后20天")]
        ItemInAfter20 = 24,
        [Description("货到后3天")]
        ItemInAfter3 = 25,
        [Description("货到后7天")]
        ItemInAfter7 = 26,
        [Description("货到后10天，且票到")]
        ItemInAfter10PayIn = 27,
        [Description("货到后25天，且票到")]
        ItemInAfter25PayIn = 28,
    }
    /// <summary>
    /// PO的付款结算公司
    /// </summary>
    public enum SettlementCompany
    {
        [Description("上海分公司")]
        ShanghaiFiliale = 3201,

        [Description("北京分公司")]
        BeijingFiliale = 3205,

        [Description("广州分公司")]
        GuangzhouFiliale = 3206
    }
    #region PO状态
    public enum POStatus
    {
        /// <summary>
        /// 自动作废
        /// </summary>
        [Description("自动作废")]
        AutoAbandon = -1,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Abandon = 0,

        /// <summary>
        /// 已创建
        /// </summary>
        Origin = 1,

        /// <summary>
        /// 等待分摊
        /// </summary>
        [Description("等待分摊")]
        WaitingApportion = 2,

        /// <summary>
        /// 等待入库
        /// </summary>
        [Description("等待入库")]
        WaitingInStock = 3,

        /// <summary>
        /// 在仓库中
        /// </summary>
        [Description("已入库")]
        InStock = 4,

        /// <summary>
        /// 
        /// </summary>
        [Description("待审核")]
        WaitCheck = 5,
        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Return = -2,

        #region 16526
        /// <summary>
        /// 部分入库
        /// </summary>
        [Description("部分入库")]
        PartlyInStock = 6,

        /// <summary>
        /// 部分收货并且供应商不再送货
        /// </summary>
        [Description("手动关闭")]
        CloseByHand = 7,

        /// <summary>
        /// 后一次Task打印时间起超过30天未继续到货的“部分入库”状态PO，系统则自动关闭该PO
        /// </summary>
        [Description("系统关闭")]
        CloseBySystem = 8
        #endregion
    }
    public enum VendorStatus
    {
        Valid = 0,
        InValid = -1
    }
    #endregion
    public static class VendorPayTypeExtension
    {
        public static string GetDesc(this VendorPayType payTypeExtension)
        {
            switch (payTypeExtension)
            {
                case VendorPayType.MoneyInItemOut: return QueryModelResource.VendorPayType_MoneyInItemOut;

                case VendorPayType.ItemInMoneyIn: return QueryModelResource.VendorPayType_ItemInMoneyIn;

                case VendorPayType.ItemInAfter2: return QueryModelResource.VendorPayType_ItemInAfter2;

                case VendorPayType.ItemInEveryMonday: return QueryModelResource.VendorPayType_ItemInEveryMonday;

                case VendorPayType.ItemInPayIn: return QueryModelResource.VendorPayType_ItemInPayIn;

                case VendorPayType.ItemInAfter3PayIn: return QueryModelResource.VendorPayType_ItemInAfter3PayIn;

                case VendorPayType.ItemInAfter7PayIn: return QueryModelResource.VendorPayType_ItemInAfter7PayIn;

                case VendorPayType.ItemInAfter10PayIn: return QueryModelResource.VendorPayType_ItemInAfter10PayIn;
                case VendorPayType.ItemInAfter25PayIn: return QueryModelResource.VendorPayType_ItemInAfter25PayIn;
                case VendorPayType.ItemInAfter14PayIn: return QueryModelResource.VendorPayType_ItemInAfter14PayIn;

                case VendorPayType.ItemInAfter30PayIn: return QueryModelResource.VendorPayType_ItemInAfter30PayIn;

                case VendorPayType.ItemInEvery25PayIn: return QueryModelResource.VendorPayType_ItemInEvery25PayIn;

                case VendorPayType.ItemInEvery1525PayIn: return QueryModelResource.VendorPayType_ItemInEvery1525PayIn;

                case VendorPayType.Consign: return QueryModelResource.VendorPayType_Consign;

                case VendorPayType.ItemInPayInAfter15: return QueryModelResource.VendorPayType_ItemInPayInAfter15;

                case VendorPayType.ItemInPayInAfter30: return QueryModelResource.VendorPayType_ItemInPayInAfter30;

                case VendorPayType.ItemInPayInAfter20: return QueryModelResource.VendorPayType_ItemInPayInAfter20;

                case VendorPayType.ItemInPayInAfter7: return QueryModelResource.VendorPayType_ItemInPayInAfter7;

                case VendorPayType.ItemInPayInAfter45: return QueryModelResource.VendorPayType_ItemInPayInAfter45;

                case VendorPayType.ItemInPayInAfter0: return QueryModelResource.VendorPayType_ItemInPayInAfter0;

                case VendorPayType.ItemInPayByMonth: return QueryModelResource.VendorPayType_ItemInPayByMonth;

                case VendorPayType.ItemInAfter18PayIn: return QueryModelResource.VendorPayType_ItemInAfter18PayIn;

                case VendorPayType.ItemInAfter20PayIn: return QueryModelResource.VendorPayType_ItemInAfter20PayIn;

                case VendorPayType.ItemInAfter45PayIn: return QueryModelResource.VendorPayType_ItemInAfter45PayIn;

                case VendorPayType.ItemInAfter14: return QueryModelResource.VendorPayType_ItemInAfter14;

                case VendorPayType.ItemInAfter20: return QueryModelResource.VendorPayType_ItemInAfter20;

                case VendorPayType.ItemInAfter3: return QueryModelResource.VendorPayType_ItemInAfter3;

                case VendorPayType.ItemInAfter7: return QueryModelResource.VendorPayType_ItemInAfter7;

                default: return "";
            }

        }

        public static Dictionary<string, string> GetValueAndKey()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] names = System.Enum.GetNames(typeof(VendorPayType));
            Array values = System.Enum.GetValues(typeof(VendorPayType));
            foreach (int value in values)
            {
                dic.Add(value.ToString(), ((VendorPayType)value).GetDesc());
            }
            return dic;
        }
    }
    public static class SettlementCompanyExtension
    {
        public static string GetDesc(this SettlementCompany settlementCompany)
        {
            switch (settlementCompany)
            {
                case SettlementCompany.ShanghaiFiliale: return QueryModelResource.SettlementCompany_ShanghaiFiliale;
                case SettlementCompany.BeijingFiliale: return QueryModelResource.SettlementCompany_BeijingFiliale;
                case SettlementCompany.GuangzhouFiliale: return QueryModelResource.SettlementCompany_GuangzhouFiliale;
                default: return "";
            }
        }
    }
    public static class POStatusExtension
    {
        public static string GetDesc(this POStatus status)
        {
            switch (status)
            {
                case POStatus.AutoAbandon: return QueryModelResource.POStatus_AutoAbandon;
                case POStatus.Abandon: return QueryModelResource.POStatus_Abandon;
                case POStatus.InStock: return QueryModelResource.POStatus_InStock;
                case POStatus.Origin: return "已创建";
                case POStatus.WaitingApportion: return QueryModelResource.POStatus_WaitingApportion;
                case POStatus.WaitingInStock: return QueryModelResource.POStatus_WaitingInStock;
                case POStatus.WaitCheck: return QueryModelResource.POStatus_WaitCheck;
                case POStatus.Return: return QueryModelResource.POStatus_Return;
                case POStatus.PartlyInStock: return QueryModelResource.POStatus_PartlyInStock;
                case POStatus.CloseByHand: return QueryModelResource.POStatus_CloseByHand;
                case POStatus.CloseBySystem: return QueryModelResource.POStatus_CloseBySystem;
                default: return "";
            }
        }
    }
    public static class POIsConsignExtension
    {
        public static string GetDesc(this POIsConsign status)
        {
            switch (status)
            {
                case POIsConsign.UnConsign: return QueryModelResource.POIsConsign_UnConsign;
                case POIsConsign.IsConsign: return QueryModelResource.POIsConsign_IsConsign;
                default: return "";
            }
        }
    }
    public static class VendorStatusExtension
    {
        public static string GetDesc(this VendorStatus item)
        {
            switch (item)
            {
                case VendorStatus.Valid: return QueryModelResource.VendorStatus_Valid;
                case VendorStatus.InValid: return QueryModelResource.VendorStatus_InValid;
                default: return "";
            }
        }
    }
    public static class POTypeStatusExtension
    {
        public static string GetDesc(this POType Type)
        {
            switch (Type)
            {
                case POType.Normal: return QueryModelResource.POType_Normal;
                case POType.Return: return QueryModelResource.POType_Return;
                case POType.HistoryReturn: return QueryModelResource.POType_HistoryReturn;
                case POType.AdjustPrice: return QueryModelResource.POType_AdjustPrice;
                default: return "";
            }
        }
    }
}

