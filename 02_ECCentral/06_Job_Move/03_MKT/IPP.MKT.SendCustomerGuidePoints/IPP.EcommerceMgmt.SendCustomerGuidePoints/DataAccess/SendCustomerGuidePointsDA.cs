using System;
/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Harry.H.Ning
 *  Date:    2010-3-31
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System.Collections.Generic;
using System.Configuration;
using IPP.EcommerceMgmt.SendCustomerGuidePoints.Entities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.EcommerceMgmt.SendCustomerGuidePoints.DataAccess
{
    public static class SendCustomerGuidePointsDA
    {
        public static List<CustomerGuideEntity> GetAllCustomerGuideList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCustomerGuideList");
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return cmd.ExecuteEntityList<CustomerGuideEntity>();
        }

        public static void UpdateObtainPoint(CustomerGuideEntity commentEntity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerGuideObtainPoint");
            cmd.SetParameterValue("@SysNo", commentEntity.SysNo);
            cmd.SetParameterValue("@ObtainPoint", commentEntity.CustomerPoint);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            cmd.ExecuteNonQuery();
        }
    }
}