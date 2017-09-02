using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(AccessoryProcessor))]
    public class AccessoryProcessor
    {
        private readonly IAccessoryDA _accessoryDA = ObjectFactory<IAccessoryDA>.Instance;

        #region 配件业务处理方法

        /// <summary>
        /// 创建配件
        /// </summary>        
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        public virtual AccessoryInfo CreateAccessory(AccessoryInfo accessoryInfo)
        {
            var checkResult = PreCheckAccessoryInfoForCreate(accessoryInfo);
            if (String.IsNullOrEmpty(checkResult))
            {
                return _accessoryDA.Insert(accessoryInfo);
            }
            throw new BizException(checkResult);
        }

        /// <summary>
        /// 修改配件
        /// </summary>
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        public virtual AccessoryInfo UpdateAccessory(AccessoryInfo accessoryInfo)
        {
            var checkResult = PreCheckAccessoryInfoForUpdate(accessoryInfo);
            if (String.IsNullOrEmpty(checkResult))
            {
                return _accessoryDA.Update(accessoryInfo);
            }
            throw new BizException(checkResult);
        }


        /// <summary>
        /// 获取配件信息
        /// </summary>
        /// <returns></returns>
        public virtual IList<AccessoryInfo> GetAllAccessory()
        {
            return _accessoryDA.GetAll();
        }

        /// <summary>
        /// 根据SysNo获取配件信息
        /// </summary>
        /// <param name="accessorySysNo"></param>
        /// <returns></returns>
        public virtual AccessoryInfo GetAccessoryInfo(int accessorySysNo)
        {
            return _accessoryDA.GetBySysNo(accessorySysNo);
        }

        /// <summary>
        /// 根据名字获取配件信息
        /// </summary>
        /// <param name="accessoryName"></param>
        /// <returns></returns>
        public virtual IList<AccessoryInfo> GetAccessoryInfoByName(string accessoryName)
        {
            return _accessoryDA.GetList(accessoryName);
        }

        #endregion

        #region 配件信息检查逻辑
        private string PreCheckAccessoryInfoForCreate(AccessoryInfo accessoryInfo)
        {
            var result = new StringBuilder();
            result.Append(PreCheckAccessoryInfo(accessoryInfo));
            if (_accessoryDA.GetList(accessoryInfo.AccessoryName.Content).Count > 0)
            {
                result.Append(ResouceManager.GetMessageString("IM.CategoryAccessory", "ExistsAccessoryName"));
            }
            return result.ToString();
        }

        private string PreCheckAccessoryInfoForUpdate(AccessoryInfo accessoryInfo)
        {
            var result = new StringBuilder();
            result.Append(PreCheckAccessoryInfo(accessoryInfo));
            if (_accessoryDA.GetList(accessoryInfo.AccessoryName.Content).Any(accessory => accessory.SysNo != accessoryInfo.SysNo))
            {
                result.Append(ResouceManager.GetMessageString("IM.CategoryAccessory", "ExistsAccessoryName"));
            }
            if (_accessoryDA.GetListByID(accessoryInfo.AccessoryID).Any(accessory => accessory.SysNo != accessoryInfo.SysNo))
            {
                result.Append(ResouceManager.GetMessageString("IM.CategoryAccessory", "ExistsAccessoryID"));
            }
            return result.ToString();
        }

        private string PreCheckAccessoryInfo(AccessoryInfo accessoryInfo)
        {
            var result = new StringBuilder();
            if (String.IsNullOrEmpty((accessoryInfo.AccessoryName.ToString())))
            {
                result.Append(ResouceManager.GetMessageString("IM.CategoryAccessory", "AccessoryNameNotNull"));
            }
            return result.ToString();
        }

        #endregion
    }
}
