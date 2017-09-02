using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility.DataAccess;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using ECCentral.BizEntity;
using ECCentral.Service.Utility.DataAccess.DbProvider;
using System.Text.RegularExpressions;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IGiftCardDA))]
    public class GiftCardDA : IGiftCardDA
    {
        public List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetGiftCardInfoBySOSysNo");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            dc.SetParameterValue("@InternalType", internalType);

            return dc.ExecuteEntityList<GiftCardInfo>();
        }

        public GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetGiftCardInfoByReferenceSOSysNo");
            dc.SetParameterValue("@ReferenceSOSysNo", soSysNo);
            dc.SetParameterValue("@CustomerSysNo", customerSysNo);
            dc.SetParameterValue("@InternalType", internalType);
            dc.SetParameterValue("@Type", type);

            GiftCardInfo result = dc.ExecuteEntity<GiftCardInfo>();
            return result;
        }

        public List<GiftCardInfo> GetGiftCardsByCodeList(List<string> codeList)
        {
            string condition = "";
            codeList.ForEach(item => condition += ", " + "'" + item + "'");
            if (condition.Length > 0)
            {
                condition = condition.TrimStart(',', ' ');
            }
            else
            {
                condition = "0";
            }

            DataCommand command = DataCommandManager.GetDataCommand("GetGiftCardsByCodeList");

            command.ReplaceParameterValue("#CodeList#", condition);


            return command.ExecuteEntityList<GiftCardInfo>();
        }

        public List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetGiftCardRedeemLogBySoSysNo");
            dc.SetParameterValue("@ActionSysNo", actionSysNo);
            dc.SetParameterValue("@ActionType", actionType);
            dc.SetParameterValue("@Status", "A");

            return dc.ExecuteEntityList<GiftCardRedeemLog>();
        }

        public string OperateGiftCard(string xmlMsg)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("OperateGiftCard");
            dc.SetParameterValue("@Msg", xmlMsg);

            try
            {
                dc.ExecuteNonQuery();
            }
            catch (DataAccessException e)
            {
                Regex reg = new Regex(@"(?<=((?<!Error[ ])Message:)).+(?=Error Severity)");
                throw new BizException(reg.Match(e.Message).Value);
            }

            object o = dc.GetParameterValue("@StatusCode");
            if (o != null && o != DBNull.Value)
            {
                return o.ToString().Trim();
            }
            return string.Empty;
        }

        #region bober add


        /// <summary>
        /// 加载礼品卡
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public GiftCardInfo LoadGiftCardInfo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardInfo");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            return DataMapper.GetEntity<ECCentral.BizEntity.IM.GiftCardInfo>(dt.Rows[0]);
        }

        /// <summary>
        /// 操作礼品卡的状态
        /// </summary>
        /// <param name="action"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public string OperateGiftCardStatus(string action, GiftCardInfo item)
        {
            StringBuilder messageBuilder = new StringBuilder();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = Encoding.Unicode;
            using (XmlWriter xmlWriter = XmlWriter.Create(messageBuilder, setting))
            {
                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteStartElement("Header");
                #region header
                xmlWriter.WriteElementString("Action", action);
                xmlWriter.WriteElementString("Version", "V1");
                xmlWriter.WriteElementString("From", "IPP.Content");
                xmlWriter.WriteElementString("CompanyCode", item.CompanyCode);
                xmlWriter.WriteElementString("StoreCompanyCode", item.CompanyCode);
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Body");
                xmlWriter.WriteElementString("EditUser", item.EditUser.UserDisplayName);//需要当前用户名

                #region giftcard list

                xmlWriter.WriteStartElement("GiftCard");
                xmlWriter.WriteElementString("Code", item.CardCode);//BarCode
                // update begindate and enddate  in sp don't know why setting here
                if (!action.Equals("ActiveGiftVoucher"))
                {
                    xmlWriter.WriteElementString("BeginDate", item.BeginDate.Value.ToString("yyyy-MM-dd"));
                    xmlWriter.WriteElementString("EndDate", item.EndDate.Value.ToString("yyyy-MM-dd"));
                }
                xmlWriter.WriteEndElement();

                if (action == "AdjustExpireDate")
                {
                    xmlWriter.WriteElementString("Memo", "从时间" + (item.PreEndDate.HasValue ? item.PreEndDate.Value.ToString("yyyy-MM-dd") : "") + "修改到时间" + item.EndDate.Value.ToString("yyyy-MM-dd"));

                }
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            DataCommand cmd = DataCommandManager.GetDataCommand("OperateGiftCard");
            cmd.SetParameterValue("Msg", messageBuilder.ToString());
            cmd.ExecuteNonQuery();
            return cmd.GetParameterValue("StatusCode").ToString();
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<GiftCardOperateLog> GetGiftCardOperateLogByCode(string code)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardOperateLogByCode");
            dc.SetParameterValue("@Code", code);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<GiftCardOperateLog, List<GiftCardOperateLog>>(reader);
            }
        }
        /// <summary>
        /// 获取详细的礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<GiftCardRedeemLog> GetGiftCardRedeemLogJoinSOMaster(string code)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardRedeemLogJoinSOMaster");
            dc.SetParameterValue("@Code", code);

            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("OrderStatus", typeof(ECCentral.BizEntity.SO.SOStatus));
            enumList.Add("RedeemStatus", typeof(ValidStatus));

            var dt = dc.ExecuteDataTable(enumList);
            if (dt != null && dt.Rows.Count > 0)
            {
                List<GiftCardRedeemLog> list = new List<GiftCardRedeemLog>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(DataMapper.GetEntity<GiftCardRedeemLog>(dr));
                }
                return list;
            }
            else
                return null;
            //using (IDataReader reader = dc.ExecuteDataReader())
            //{
            //    return DataMapper.GetEntityList<GiftCardOperateLog, List<GiftCardOperateLog>>(reader);
            //}
        }

        /// <summary>
        /// GetItemExchangeRate
        /// </summary>
        /// <param name="currencySysNo"></param>
        /// <returns></returns>
        public decimal GetItemExchangeRate(int currencySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetItemExchangeRate");
            dc.SetParameterValue("@CurrencySysNo", currencySysNo);
            DataTable dt = dc.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return decimal.Parse(dt.Rows[0]["ExchangeRate"].ToString());
            return 0;
        }

        /// <summary>
        /// GetItemPayTypeSysNo
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public int GetItemPayTypeSysNo(int vendorSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetItemPayTypeSysNo");
            dc.SetParameterValue("@VendorSysNo", vendorSysNo);
            DataTable dt = dc.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return int.Parse(dt.Rows[0]["PayPeriodType"].ToString());
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="exchangeRate"></param>
        /// <param name="iPayPeriodType"></param>
        /// <returns></returns>
        public List<GiftCardFabrication> GetGiftCardFabricationItem(int sysNo, decimal exchangeRate, int iPayPeriodType)
        {

             //DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardFabricationItem");
           DataCommand dc = DataCommandManager.GetDataCommand("GiftVoucherProduct_GetFabricationItem");
            dc.SetParameterValue("@GiftCardFabricationSysNo", sysNo);
            //dc.SetParameterValue("@C3SysNo", AppSettingManager.GetSetting("MKT", "C3SysNoForGiftCardFabrication"));
            //dc.SetParameterValue("@ManufacturerSysNo", AppSettingManager.GetSetting("MKT", "ManufacturerSysNoForGiftCardFabrication"));
            //dc.SetParameterValue("@ExchangeRate", exchangeRate);
            //dc.SetParameterValue("@PayPeriodType", iPayPeriodType);
            //dc.SetParameterValue("@CompanyCode", "8601");

            var dt = dc.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                List<GiftCardFabrication> list = new List<GiftCardFabrication>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(DataMapper.GetEntity<GiftCardFabrication>(dr));
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public DataTable GetGiftCardFabricationItemSum(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardFabricationItemSum");
            dc.SetParameterValue("@GiftCardFabricationSysNo", sysNo);
            dc.SetParameterValue("@C3SysNo", AppSettingManager.GetSetting("MKT", "C3SysNoForGiftCardFabrication"));
            dc.SetParameterValue("@ManufacturerSysNo", AppSettingManager.GetSetting("MKT", "ManufacturerSysNoForGiftCardFabrication"));
            dc.SetParameterValue("@CompanyCode", "8601");

            return dc.ExecuteDataTable();
        }


        /// <summary>
        /// 更新主体信息
        /// </summary>
        /// <param name="item"></param>
        public void UpdateGiftCardFabricationMaster(GiftCardFabricationMaster item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftCardFabricationMaster");
            cmd.SetParameterValue<GiftCardFabricationMaster>(item);
            cmd.ExecuteNonQuery();
        }

        #region GiftCardFabricationMaster

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        public void DeleteGiftCardFabrication(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_DeleteGiftCardFabrications");
            cmd.SetParameterValue("@SysNo", sysNo);


            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建,事实已经存在该GiftCardFabricationMaster对象 
        /// </summary>
        /// <param name="item"></param>
        public void CreatePOGiftCardFabrication(GiftCardFabricationMaster item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_CreatePOGiftCardFabrication");
            cmd.SetParameterValue<GiftCardFabricationMaster>(item);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="item"></param>
        public int InsertGiftCardFabricationMaster(GiftCardFabricationMaster item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_InsertGiftCardFabricationMaster");
            cmd.SetParameterValue<GiftCardFabricationMaster>(item);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@SysNo");
        }
        #endregion

        #region GiftCardFabrication
        /// <summary>
        /// 新建礼品卡子项，对应礼品卡具体金额
        /// </summary>
        /// <param name="item"></param>
        public void InsertGiftCardFabricationItem(GiftCardFabrication item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_InsertGiftCardFabricationItem");
            cmd.SetParameterValue<GiftCardFabrication>(item);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新礼品卡子项，对应礼品卡具体金额
        /// </summary>
        /// <param name="item"></param>
        public void UpdateGiftCardFabricationItem(GiftCardFabrication item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftCardFabricationItem");
            cmd.SetParameterValue<GiftCardFabrication>(item);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public decimal GetAddGiftCardInfoList(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetPassGiftCardInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            //cmd.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                return decimal.Parse(dt.Rows[0][0].ToString());
            return 0;
        }

        /// <summary>
        /// 导出制卡后改变礼品卡制作单的状态
        /// </summary>
        /// <param name="sysNO"></param>
        public void UpdateGiftCardInfoStatus(int sysNO)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftCardInfoStatus");
            cmd.SetParameterValue("@SysNo", sysNO);
            cmd.ExecuteNonQuery();
        }

        #endregion


        #endregion

        /// <summary>
        /// 获取当前生成的需要导出的礼品卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public DataTable GetGiftCardInfoByGiftCardFabricationSysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetGiftCardInfoByGiftCardFabricationSysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            //cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteDataTable();
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    List<GiftCardInfo> list = new List<GiftCardInfo>();
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        list.Add(DataMapper.GetEntity<GiftCardInfo>(dr));
            //    }
            //    return list;
            //}
            //return null;
        }


        #region GiftVoucherProduct

        public int SaveGiftVoucherProduct(GiftVoucherProduct item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_SaveGiftVoucherProductInfo");
            cmd.SetParameterValue<GiftVoucherProduct>(item);
            return cmd.ExecuteScalar<int>();
        }

        public void UpdateVoucherProduct(GiftVoucherProduct item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftVoucherProduct");
            cmd.SetParameterValue<GiftVoucherProduct>(item);
            cmd.ExecuteNonQuery();
        }

        public GiftVoucherProduct GetVoucherProductBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetGiftVoucherProductBySysNo");
            cmd.SetParameterValue(@"SysNo", sysNo);

            return cmd.ExecuteEntity<GiftVoucherProduct>();
        }

        public List<GiftVoucherProductRelation> GetVoucherProductRelationByVoucher(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetGiftVoucherProductRelationByVoucherSysNo");
            cmd.SetParameterValue(@"GiftVoucherSysNo", sysNo);

            return cmd.ExecuteEntityList<GiftVoucherProductRelation>();
        }

        public GiftVoucherProductRelation GetVoucherProductRelationByRequest(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetGiftVoucherProductRelationByReqeust");
            cmd.SetParameterValue(@"RequestSysNo", sysNo);

            return cmd.ExecuteEntity<GiftVoucherProductRelation>();
        }

        public List<GiftVoucherProductRelationRequest> GetGiftVoucherProductRelationRequestByRelation(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetVoucherRelationRequestByRelationSysNo");
            cmd.SetParameterValue(@"RelationSysNo", sysNo);

            return cmd.ExecuteEntityList<GiftVoucherProductRelationRequest>();
        }

        public int SaveGiftVoucherProductRelation(GiftVoucherProductRelation item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_SaveGiftVoucherRelationInfo");
            cmd.SetParameterValue<GiftVoucherProductRelation>(item);
            return cmd.ExecuteScalar<int>();
        }

        public void SaveGiftVoucherProductRelationRequest(GiftVoucherProductRelationRequest item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_SaveGiftVoucherRelationRequestInfo");
            cmd.SetParameterValue<GiftVoucherProductRelationRequest>(item);
            cmd.ExecuteNonQuery();
        }

        public void DeleteGiftVoucherProductRelationByRelation(GiftVoucherProductRelation item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_DeleteGiftVoucherRelationByRelation");
            cmd.SetParameterValue("@RelationSysNo", item.SysNo);
            cmd.ExecuteNonQuery();
        }

        public void DeleteGiftVoucherProductReleationBySysNo(GiftVoucherProductRelation item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_DeleteGiftVoucherRelationBySysNo");
            cmd.SetParameterValue("@SysNo", item.SysNo);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGiftVoucherProductRelationStatus(GiftVoucherProductRelation item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftVoucherRelationStatus");
            cmd.SetParameterValue("@SysNo", item.SysNo);
            cmd.SetParameterValue("@Status", item.Status);

            cmd.ExecuteNonQuery();
        }

        public void UpdateGiftVoucherProductRelationRequestStatus(GiftVoucherProductRelationRequest item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_UpdateGiftVoucherRelationRequestStatus");
            cmd.SetParameterValue("@SysNo", item.SysNo);
            cmd.SetParameterValue("@AuditStatus", item.AuditStatus);


            cmd.ExecuteNonQuery();

        }

        public bool IsExistsSameProduct(GiftVoucherProduct item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftVoucherProduct_IsExistSameProduct");
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);

            int i = cmd.ExecuteScalar<int>();

            return i > 0 ? true : false;
        }

        public bool IsExistSameGiftVoucherPrice(GiftVoucherProduct item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftVoucherProduct_IsExistSamePrice");
            cmd.SetParameterValue("@Price", item.Price);
            cmd.SetParameterValue("@SysNo", item.SysNo);

            int i = cmd.ExecuteScalar<int>();

            return i > 0 ? true : false;
        }

        public GiftVoucherProductRelationRequest GetGiftVoucherProductRelationRequestBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCard_GetVoucherRelationRequestBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<GiftVoucherProductRelationRequest>();
        }

        public void UpdateProductGiftVoucherType(int productSysNo, GiftVoucherType voucherType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductGiftVoucherType");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@GiftVoucherType", voucherType);

            cmd.ExecuteNonQuery();
        }

        public bool IsExistsSameRelationProduct(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsSameRelationProduct");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteScalar<int>() > 0 ? true : false;
        }
        #endregion
    }
}
