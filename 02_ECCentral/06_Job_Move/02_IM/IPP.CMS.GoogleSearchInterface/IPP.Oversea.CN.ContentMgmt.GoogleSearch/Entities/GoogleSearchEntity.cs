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
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.Entity;

using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.GoogleSearch.Entities
{
    public class GoogleSearchEntity : DefaultQueryEntity
    {
       
    }

    public class MailEntity
    {
        public string BCC { get; set; }

        public string Body { get; set; }

        public string CC { get; set; }

        public string From { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }
    }

}