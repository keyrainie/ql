/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  james.J.Yu
 *  Date:    2010-11-02
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;

using Newegg.Oversea.Framework.ExceptionHandler;

using IPP.Oversea.CN.ContentMgmt.JobProductRequest.Entities;
using IPP.Oversea.CN.ContentMgmt.JobProductRequest.BizProcess;

namespace IPP.Oversea.CN.ContentMgmt.JobProductRequest
{
    class ProgramStart
    {
        //[ErrorHandling]
        static void Main(string[] args)
        {
            Console.WriteLine("Process Start...");
            try
            {
                (new JobProductRequestBiz()).Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Process End!");
        }
    }
}