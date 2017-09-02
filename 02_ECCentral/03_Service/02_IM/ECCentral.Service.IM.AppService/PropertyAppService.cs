using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(PropertyAppService))]
    public class PropertyAppService
    {
        /// <summary>
        /// 创建Property
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyInfo CreateProperty(PropertyInfo entity)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.CreateProperty(entity);
            return result;
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyInfo UpdateProperty(PropertyInfo entity)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.UpdateProperty(entity);
            return result;
        }

        public virtual PropertyInfo GetPropertyByPropertyName(string propertyName)
        {

            return ObjectFactory<PropertyProcessor>.Instance.GetPropertyByPropertyName(propertyName);
        }

        public virtual List<PropertyInfo> GetPropertyListByPropertyName(string propertyName)
        {

            return ObjectFactory<PropertyProcessor>.Instance.GetPropertyListByPropertyName(propertyName);
        }

        /// <summary>
        /// 根据SysNO获取Property信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public virtual PropertyInfo GetPropertyBySysNo(int sysno)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.GetPropertyBySysNo(sysno);

            return result;
        }

        public virtual List<PropertyValueInfo> GetPropertyValueByPropertySysNo(int sysno)
        {

            return ObjectFactory<PropertyProcessor>.Instance.GetPropertyValueByPropertySysNo(sysno);
        }

        public virtual List<PropertyInfo> GetAllProperty()
        {
            return ObjectFactory<PropertyProcessor>.Instance.GetAllProperty();
        }
        public virtual List<PropertyValueInfo> GetPropertyValueByPropertyName(string propertyName)
        {

            return ObjectFactory<PropertyProcessor>.Instance.GetPropertyValueByPropertyName(propertyName);
        }

        /// <summary>
        /// 创建属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyValueInfo CreatePropertyValue(PropertyValueInfo entity)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.CreatePropertyValue(entity);
            return result;
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyValueInfo UpdatePropertyValue(PropertyValueInfo entity)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.UpdatePropertyValue(entity);
            return result;
        }

        /// <summary>
        /// 根据PropertySysNoList获取Property信息
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList)
        {
            var result = ObjectFactory<PropertyProcessor>.Instance.GetPropertyValueInfoByPropertySysNoList(sysNoList);
            return result;
        }

    }
}
