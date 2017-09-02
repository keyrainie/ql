using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.AppService
{

    [VersionExport(typeof(EPortService))]
    public class EPortService
    {
        private EPortProcessor m_EPortProcessor;
        public EPortProcessor M_EPortProcessor
        {
            get
            {
                if (null == m_EPortProcessor)
                {
                    m_EPortProcessor = ObjectFactory<EPortProcessor>.Instance;
                }
                return m_EPortProcessor;
            }
        }

        public EPortEntity CreateEPort(EPortEntity newEPortInfo)
        {
            return M_EPortProcessor.CreateEport(newEPortInfo);
        }

        /// <summary>
        /// 保存电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity SaveEport(EPortEntity entity)
        {
            return M_EPortProcessor.SaveEport(entity);
        }
        /// <summary>
        /// 删除电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int DeleteEport(int sysNO)
        {
            return M_EPortProcessor.DeleteEport(sysNO);
        }
        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity GetEport(string sysNO)
        {
            return M_EPortProcessor.GetEport(sysNO);
        }
        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<EPortEntity> GetAllEPort()
        {
            return M_EPortProcessor.GetAllEPort();
        }
    }
}
