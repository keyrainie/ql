/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:   Danish.G.Wang
 * Create Date:  2009-03-04
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;

using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentManagement.BusinessEntities
{
    [Serializable]
    public class BatchEntity<T> where T : EntityBase
    {
        public List<T> Entities
        {
            get;
            set;
        }
    }
}
