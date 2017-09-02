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

using IPP.Oversea.CN.ContentMgmt.GoogleSearch.Entities;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.GoogleSearch.DataAccess
{
    public static class GoogleSearchDA
    {
        public static DataSet GetGoogleSearchProductList(string companyCode)
        {
            DataSet dsResult = null;
            try
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetGoogleSearchProductList");
                cmd.SetParameterValue("@CompanyCode", companyCode);
                dsResult = cmd.ExecuteDataSet();
            }
            catch (Exception ex) 
            { 
                throw ex;
            }
            return dsResult;
        }

        public static DataSet GetGoogleSearchScore(string sysNo,string companyCode)
        {
            DataSet dsResult = null;
            try
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetGoogleSearchScore");
                cmd.SetParameterValue("@SysNo", sysNo);
                cmd.SetParameterValue("@CompanyCode", companyCode);
                dsResult = cmd.ExecuteDataSet();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsResult;
        }
    }
}