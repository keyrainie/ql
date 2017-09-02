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

using Newegg.Oversea.Framework.ExceptionHandler;

using IPP.Oversea.CN.ContentMgmt.GoogleSearch.BizProcess;
using IPP.Oversea.CN.ContentMgmt.GoogleSearch.Entities;

namespace IPP.Oversea.CN.ContentMgmt.SecHandPrice
{
    class ProgramStart
    {
        //[ErrorHandling]
        static void Main(string[] args)
        {           
            try
            {
                Console.WriteLine("GoogleSearch Start...");
                new GoogleSearchBP().Starter();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常：" + ex.Message);
                //Console.ReadLine();
            }

            Console.WriteLine("GoogleSearch End!!!");
        }
    }
}