/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:           Phoebe Zhang(Phoebe.F.Zhang@newegg.com)
 * Create Date:  2009-03-06
 * Usage:  
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;

namespace IPP.Oversea.CN.ContentManagement.BusinessEntities.Common
{
    [Serializable]
    public class QueryResultEntity<T> where T : class
    {
        public T ResultEntityList { get; set; }

        public int TotalCount { get; set; }
        
    }
}
