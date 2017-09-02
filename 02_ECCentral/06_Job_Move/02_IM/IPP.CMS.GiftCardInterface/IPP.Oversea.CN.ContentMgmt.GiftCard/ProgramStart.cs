/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2010-05-05
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;

using Newegg.Oversea.Framework.ExceptionHandler;

using IPP.Oversea.CN.ContentMgmt.GiftCard.BizProcess;
using IPP.Oversea.CN.ContentMgmt.GiftCard.Entities;

namespace IPP.Oversea.CN.ContentMgmt.GiftCard
{
    class ProgramStart
    {
        //[ErrorHandling]
        static void Main(string[] args)
        {
            GiftCardBP.BizLogFile = "Log\\biz2.log";
            GiftCardBP.WriteLog("start-- Log.WriteLog");
            Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
         
            try
            {
                //Console.WriteLine("GiftCard Data Process Start...");
                new GiftCardBP().Starter();
            }
            catch (Exception ex)
            {
                string msg = "异常：" + ex.Message;
                GiftCardBP.WriteLog(msg);
            }

            Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
        }
    }
}