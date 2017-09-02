/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2009-12-08
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities.String;

using IPP.Oversea.CN.ContentMgmt.GiftCard.Entities;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.GiftCard.DataAccess
{
    public static class GiftCardDA
    {
        public static DataSet GetGiftCardFabricationMaster(string companyCode)
        {
            DataSet dsResult = new DataSet();        
            
            DataCommand cmd = DataCommandManager.GetDataCommand("GetGiftCardFabricationMaster");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            dsResult = cmd.ExecuteDataSet();           
            return dsResult;
        }

        public static DataSet GetGiftCardFabricationItem(string GiftCardFabricationSysNo, string companyCode, string gcC3Number, int iManufacturerSysNo)
        {
            DataSet dsResult = new DataSet();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetGiftCardFabricationItem");

            cmd.SetParameterValue("@GiftCardFabricationSysNo", GiftCardFabricationSysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@C3SysNo", gcC3Number);
            cmd.SetParameterValue("@ManufacturerSysNo", iManufacturerSysNo);
            dsResult = cmd.ExecuteDataSet();
            return dsResult;
        }


        public static void GiftCardFabricationECOperateGiftCard(string messageBuilder)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GiftCardFabrication_ECOperateGiftCard");
            cmd.SetParameterValue("Msg", messageBuilder);
            cmd.ExecuteNonQuery();
        }

        public static DataSet CheckGiftCardInfo()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckGiftCardInfo");
            return cmd.ExecuteDataSet();
        }

        public static DataSet GetPassGiftCardInfo(string GiftCardFabricationSysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPassGiftCardInfo");
            cmd.SetParameterValue("@GiftCardFabricationSysNo", GiftCardFabricationSysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteDataSet();
        }
    }
}