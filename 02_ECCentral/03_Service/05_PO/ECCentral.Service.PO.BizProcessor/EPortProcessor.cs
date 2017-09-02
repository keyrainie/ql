using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.BizProcessor
{
     [VersionExport(typeof(EPortProcessor))]
    public class EPortProcessor
    {
        /// <summary>
        /// 创建新供应商
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity CreateEport(EPortEntity entity)
        {
            return ObjectFactory<IEPortDA>.Instance.CreateEPort(entity);
        }
        /// <summary>
        /// 保存电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity SaveEport(EPortEntity entity)
        {
            return ObjectFactory<IEPortDA>.Instance.SaveEPort(entity);
        }
        /// <summary>
        /// 删除电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int DeleteEport(int sysNO)
        {
            return ObjectFactory<IEPortDA>.Instance.DeleteEPort(sysNO);
        }
        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity GetEport(string sysNO)
        {
            return ObjectFactory<IEPortDA>.Instance.GetEPort(sysNO);
        }
        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<EPortEntity> GetAllEPort()
        {
            return ObjectFactory<IEPortDA>.Instance.GetEPort();
        }
    }
}
