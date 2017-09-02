using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(AreaProcessor))]
    public class AreaProcessor
    {
        private IAreaDA m_DataAccess = ObjectFactory<IAreaDA>.Instance;

        public virtual AreaInfo Create(AreaInfo entity)
        {
            return m_DataAccess.Create(entity);
        }

        public virtual AreaInfo Update(AreaInfo entity)
        {
            return m_DataAccess.Update(entity);
            
        }

        public virtual AreaInfo Load(int sysNo)
        {
            var result = m_DataAccess.Load(sysNo);
            //如果地区名称有值，证明读取的数据为地区
            if (!string.IsNullOrEmpty(result.DistrictName))
            {
                result.DistrictSysNo = sysNo;   
            }
            return result;
        }
    }
} 