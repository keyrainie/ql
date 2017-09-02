using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IAccessoryDA
    {
        /// <summary>
        /// 获取单个配件
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        AccessoryInfo GetBySysNo(int sysNo);

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        AccessoryInfo Insert(AccessoryInfo accessoryInfo);

        /// <summary>
        /// 修改配件
        /// </summary>
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        AccessoryInfo Update(AccessoryInfo accessoryInfo);

        /// <summary>
        /// 根据配件名称获取配件列表
        /// </summary>
        /// <param name="accessoryName"></param>
        /// <returns></returns>
        IList<AccessoryInfo> GetList(string accessoryName);

        /// <summary>
        /// 获取全部配件
        /// </summary>
        /// <returns></returns>
        IList<AccessoryInfo> GetAll();

        /// <summary>
        /// 根据配件ID获取配件列表
        /// </summary>
        /// <param name="accessoryID"></param>
        /// <returns></returns>
        IList<AccessoryInfo> GetListByID(string accessoryID);
    }
}
