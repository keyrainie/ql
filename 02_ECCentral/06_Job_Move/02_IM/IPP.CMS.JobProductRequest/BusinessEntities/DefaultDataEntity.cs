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
using System.Collections.Generic;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentManagement.BusinessEntities.Common
{
    [Serializable]
    public class DefaultDataEntity
    {
        private List<FaultEntity> faults;

        public List<FaultEntity> Faults
        {
            get
            {
                if (faults == null)
                {
                    faults = new List<FaultEntity>();
                }
                return faults;
            }
            set
            {
                faults = value;
            }
        }

        private EntityHeader header;

        public EntityHeader Header
        {

            get
            {
                if (header == null)
                {
                    header = new EntityHeader();
                }
                return header;
            }

            set
            {
                header = value;
            }
        }
    }
}
