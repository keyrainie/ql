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
    public class DefaultQueryEntity 
    {
        public EntityHeader Header
        {
            get;
            set;
        }

        //public PageInfo PagingInfo
        //{
        //    get;
        //    set;
        //}    
    }

    [Serializable]
    public enum SortType
    {       
        DESC    = 0,
        ASC     = 1,
    }
}
