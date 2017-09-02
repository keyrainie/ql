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

using Newegg.Oversea.Framework.ExceptionHandler;

using IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.BizProcess;

namespace IPP.Oversea.CN.ContentMgmt.AutoPricingDisable
{
    class ProgramStart
    {
        //[ErrorHandling]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("AutoPricingDisable Start...");
                new AutoPricingDisableBP().Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常：" + ex.Message);
                //Console.WriteLine(ex.Message);
            }

            Console.WriteLine("AutoPricingDisable End!!!");
        }
    }
}