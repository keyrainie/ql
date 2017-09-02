/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Harry.H.Ning
 *  Date:    2010-5-10
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Framework.Utilities.String;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess
{
    public static class TopItemDA
    {
        public static List<TopItemEntity> GetAllTopItemList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllTopItemList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<TopItemEntity>();
        }

        public static List<AdminEntity> GetCategoryList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<AdminEntity>();
        }

        public static string GetEmailAddressByCategorySysNo(int categorySysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetEmailAddressByCategorySysNo");
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteScalar<string>();
        }
    }
}