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

using IPP.Oversea.CN.ContentMgmt.JobProductRequest.Entities;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;


namespace IPP.Oversea.CN.ContentMgmt.JobProductRequest.DataAccess
{
    public static class JobProductRequestDA
    {
        public static DataSet GetJobProductRequest(JobProductRequestEntity entity)
        {
            DataSet dsResult = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetJobProductRequest");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@ProductName", entity.ProductName);
            cmd.SetParameterValue("@ProductID", entity.ProductID);
            cmd.SetParameterValue("@ProductLink", entity.ProductLink);
            cmd.SetParameterValue("@HostWarrantyDay", entity.HostWarrantyDay);
            cmd.SetParameterValue("@PartWarrantyDay", entity.PartWarrantyDay);
            cmd.SetParameterValue("@Warranty", entity.Warranty);
            cmd.SetParameterValue("@ServicePhone", entity.ServicePhone);
            cmd.SetParameterValue("@ServiceInfo", entity.ServiceInfo);
            cmd.SetParameterValue("@Note", entity.Note);
            cmd.SetParameterValue("@Weight", entity.Weight);
            cmd.SetParameterValue("@IsLarge", entity.IsLarge);
            cmd.SetParameterValue("@Length", entity.Length);
            cmd.SetParameterValue("@Width", entity.Width);
            cmd.SetParameterValue("@Height", entity.Height);
            cmd.SetParameterValue("@MinPackNumber", entity.MinPackNumber);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@Type", entity.Type);
            cmd.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            cmd.SetParameterValue("@Auditor", entity.Auditor);
            cmd.SetParameterValue("@AuditDate", entity.AuditDate);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", entity.StoreCompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.SetParameterValue("@InDate", entity.InDate);
            cmd.SetParameterValue("@InUser", entity.InUser);
            cmd.SetParameterValue("@EditDate", entity.EditDate);
            cmd.SetParameterValue("@EditUser", entity.EditUser);

            dsResult = cmd.ExecuteDataSet();
            return dsResult;
        }

    }
}