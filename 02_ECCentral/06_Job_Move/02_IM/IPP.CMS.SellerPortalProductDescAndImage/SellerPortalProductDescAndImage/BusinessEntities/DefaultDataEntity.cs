using Newegg.Oversea.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
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

    [Serializable]
    public class CustomDataEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }
    }
}
