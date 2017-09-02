/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2009-11-11
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities.String;

using IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.Entities;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.DataAccess
{
    public static class AutoPricingDisableDA
    {
        public static int UpdateAutoPricingDisableData(string SysNo, string CompanyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateItemPriceAcceptData");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            try
            {
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetProductItemNoList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductItemNoList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteDataSet();
        }

        public static SalesMailEntity GetPMMails(string productSysNo, string pmUserSysNo, string CompanyCode)
        {
            SalesMailEntity result = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductPmEmail");
            cmd.SetParameterValue("@UserSysNo", pmUserSysNo);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            result = cmd.ExecuteEntity<SalesMailEntity>();

            DataSet dsResult = GetSalesMails(productSysNo, CompanyCode);

            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                result = (result == null) ? new SalesMailEntity() : result;
                result.toEmail = dsResult.Tables[0].Rows[0]["toEmail"].ToString();
            }
            return result;
        }

        public static DataSet GetSalesMails(string productSysNo, string CompanyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductSlEmail");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            return cmd.ExecuteDataSet();
        }

    }
}