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
using IPP.EcommerceMgmt.MostUsefulComment.Entities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities.String;

namespace IPP.EcommerceMgmt.MostUsefulComment.DataAccess
{
    public static class MostUsefulCommentDA
    {
        public static List<CommentEntity> GetCommentList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCommentList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<CommentEntity>();
        }

        public static void UpdateProductReview(int productSysNo,string companyCode, string editUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductReview_DetailByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@EditUser", editUser);
            cmd.ExecuteNonQuery();
        }
    }
}