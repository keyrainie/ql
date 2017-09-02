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
using IPP.EcommerceMgmt.SendCommentPoints.Entities;
using Newegg.Oversea.Framework.DataAccess;
using System.Configuration;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace IPP.EcommerceMgmt.SendCommentPoints.DataAccess
{
    public static class SendCommentPointsDA
    {
        public static List<CommentEntity> GetCommentListByDate()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCommentListByDate");
            // cmd.SetParameterValue("@StartDate", DateTime.Now.Date.AddDays(-1));
            // cmd.SetParameterValue("@EndDate", DateTime.Now.Date);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return cmd.ExecuteEntityList<CommentEntity>();
        }

        public static List<CommentEntity> GetAllGroup()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllGroup");
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return cmd.ExecuteEntityList<CommentEntity>();
        }

        /// <summary>
        /// 以组为单位取top 5 的评论
        /// </summary>
        /// <param name="GroupSysNo"></param>
        /// <returns></returns>
        public static List<CommentEntity> GetTop5CommentByGroupSysNo(int GroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTop5CommentByGroupSysNo");
            cmd.SetParameterValue("@GroupSysNo", GroupSysNo);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return cmd.ExecuteEntityList<CommentEntity>();
        }

        public static List<CommentEntity> GetMostUsefulCommentList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetMostUsefulCommentList");
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return cmd.ExecuteEntityList<CommentEntity>();
        }

        public static void UpdateProductReview(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductReview_DetailByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            cmd.SetParameterValue("@EditUser", ConfigurationManager.AppSettings["EditUser"]);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateObtainPoint(CommentEntity commentEntity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductReviewObtainPoint");
            cmd.SetParameterValue("@SysNo", commentEntity.SysNo);
            cmd.SetParameterValue("@ObtainPoint", commentEntity.CustomerPoint);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            cmd.SetParameterValue("@EditUser", ConfigurationManager.AppSettings["EditUser"]);
            cmd.ExecuteNonQuery();
        }

        public static void InsertPointLog(CommentEntity item, string type)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertPointLog");
            cmd.SetParameterValue("@ReviewSysNo", item.SysNo);
            cmd.SetParameterValue("@Point", item.CustomerPoint);
            cmd.SetParameterValue("@Type", type);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            cmd.SetParameterValue("@EditUser", ConfigurationManager.AppSettings["EditUser"]);
            cmd.ExecuteNonQuery();
        }


        public static List<CommentEntity> GetMKTTopicPoint(string customerID1, string customerID2)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetMKTTopicPoint");
            cmd.SetParameterValue("@CustomerID1", customerID1);
            cmd.SetParameterValue("@CustomerID2", customerID2);
            cmd.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);

            return cmd.ExecuteEntityList<CommentEntity>();
        }

        public static void SendMailWhenPointNotEnough(string ID)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["PMEmailAddress"]);
            string MailSubject = Convert.ToString(ConfigurationManager.AppSettings["EmailSubject"]);
            string EmailBody = Convert.ToString(ConfigurationManager.AppSettings["EmailBody"]);
            string LanguageCode = Convert.ToString(ConfigurationManager.AppSettings["LanguageCode"]);
            MailSubject = string.Format(MailSubject, ID);
            EmailBody = string.Format(EmailBody, ID, DateTime.Now.ToShortDateString());

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", EmailBody);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@LanguageCode", LanguageCode);
            command.ExecuteNonQuery();
        }

        internal static int HadObtainPointCheck(CommentEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HadObtainPointCheck");
            command.SetParameterValue("@ReviewSysNo", entity.SysNo.ToString());
            command.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 调整顾客积分
        /// </summary>
        /// <param name="adujstInfo"></param>
        /// <returns></returns> 
        public static object Adjust(AdjustPointRequest adujstInfo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SendCommentPoints");
            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, adujstInfo.CustomerSysNo);
            cmd.AddInputParameter("@Point", DbType.Int32, adujstInfo.Point);
            cmd.AddInputParameter("@PointType", DbType.Int32, adujstInfo.PointType);
            cmd.AddInputParameter("@Source", DbType.String, adujstInfo.Source);
            cmd.AddInputParameter("@Memo", DbType.String, adujstInfo.Memo);
            cmd.AddInputParameter("@InUser", DbType.String, "JOBUser");
            //cmd.SetParameterValue("@InUser",123);
            cmd.AddInputParameter("@OperationType", DbType.Int32, adujstInfo.OperationType);
            cmd.AddInputParameter("@SoSysNo", DbType.Int32, adujstInfo.SOSysNo);
            cmd.AddInputParameter("@ExpireDate", DbType.DateTime, adujstInfo.PointExpiringDate);
            cmd.AddOutParameter("@returnCode", DbType.Int32, 0);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            return obj;
        }
    }
}