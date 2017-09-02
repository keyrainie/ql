using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Linq;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(PropertyProcessor))]
    public class PropertyProcessor
    {

        private readonly IPropertyDA propertyDA = ObjectFactory<IPropertyDA>.Instance;

        /// <summary>
        /// 创建Property
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyInfo CreateProperty(PropertyInfo entity)
        {

            var localName = entity.PropertyName.Content;
            if (string.IsNullOrEmpty(localName))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "PropertyNameIsNull"));
            }
            PropertyInfo propertyInfo = propertyDA.GetPropertyByPropertyName(localName);
            if (propertyInfo != null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "ExistsPropertyName"));
            }

            propertyDA.AddProperty(entity);
            return entity;
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyInfo UpdateProperty(PropertyInfo entity)
        {
            //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
            string localName = entity.PropertyName.Content.Trim();
            if (string.IsNullOrEmpty(localName))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "PropertyNameIsNull"));
            }
            PropertyInfo propertyInfo = propertyDA.GetPropertyInfoByEntity(entity);
            if (propertyInfo != null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "ExistsPropertyName"));
            }

            propertyDA.UpdateProperty(entity);
            return entity;
        }

        public virtual PropertyInfo GetPropertyByPropertyName(string propertyName)
        {

            PropertyInfo entity = propertyDA.GetPropertyByPropertyName(propertyName);

            return entity;
        }

        public virtual List<PropertyInfo> GetPropertyListByPropertyName(string propertyName)
        {

            List<PropertyInfo> entity = propertyDA.GetPropertyListByPropertyName(propertyName);

            return entity;
        }

        /// <summary>
        /// 根据SysNO获取Property信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public virtual PropertyInfo GetPropertyBySysNo(int sysno)
        {
            if (sysno < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "InValidSysNo"));
            }

            PropertyInfo entity = propertyDA.GetPropertyBySysNo(sysno);

            return entity;
        }


        public virtual List<PropertyValueInfo> GetPropertyValueByPropertySysNo(int sysno)
        {
            if (sysno < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "InValidSysNo"));
            }
            List<PropertyValueInfo> entityList = propertyDA.GetPropertyValueByPropertySysNo(sysno);

            return entityList;
        }

        public virtual List<PropertyInfo> GetAllProperty()
        {
            var entityList = propertyDA.GetAllProperty();

            return entityList;
        }

        public virtual List<PropertyValueInfo> GetPropertyValueByPropertyName(string propertyName)
        {
            List<PropertyValueInfo> entityList = propertyDA.GetPropertyValueByPropertyName(propertyName);

            return entityList;
        }

        /// <summary>
        /// 创建属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyValueInfo CreatePropertyValue(PropertyValueInfo entity)
        {
            if (string.IsNullOrEmpty(entity.ValueDescription.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "PropertyValueName"));
            }
            if (entity.Priority < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "InValidPriority"));
            }
            if (IsExistsPropertyValueByPropertySysNo(entity.SysNo.Value, entity.PropertyInfo.SysNo.Value, entity.ValueDescription.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "ExistsPropertyValueName"));
            }

            return propertyDA.CreatePropertyValue(entity);
        }

        /// <summary>
        /// 修改属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PropertyValueInfo UpdatePropertyValue(PropertyValueInfo entity)
        {
            if (string.IsNullOrEmpty(entity.ValueDescription.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "PropertyValueName"));
            }
            if (entity.Priority < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "InValidPriority"));
            }
            //判断该属性下是否存在相同的属性值
            if (IsExistsPropertyValueByPropertySysNo(entity.SysNo.Value, entity.PropertyInfo.SysNo.Value, entity.ValueDescription.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "ExistsPropertyValueName"));
            }

            PropertyValueInfo oldEntity = new PropertyValueInfo();
            oldEntity = propertyDA.GetPropertyValueByPropertyValueSysNo(entity.SysNo.Value);

            if (entity.ValueStatus == PropertyStatus.DeActive && oldEntity.ValueStatus == PropertyStatus.Active)
            {
                if (IsExistsProductPropertyValue(entity))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Property", "PropertyValueChangeInvalid"));
                }
            }

            return propertyDA.UpdatePropertyValue(entity);
        }

        /// <summary>
        /// 根据PropertySysNoList获取Property信息
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList)
        {
            Dictionary<int, List<PropertyValueInfo>> result = new Dictionary<int, List<PropertyValueInfo>>();
            string sysStr = sysNoList.Join(",");

            PropertyValueInfo valueEntity = new PropertyValueInfo();
            List<PropertyValueInfo> valueList = propertyDA.GetPropertyValueInfoListByPropertySysNo(sysStr);
            if (valueList == null || valueList.Count == 0) return new Dictionary<int, List<PropertyValueInfo>>();
            List<int?> propertySysNoList = valueList.Select(p => p.PropertyInfo.SysNo).Distinct().ToList();
            foreach (int item in propertySysNoList)
            {
                List<PropertyValueInfo> lists = valueList.Where(p => p.PropertyInfo.SysNo == item).ToList();
                result.Add(item,lists);
            }
            return result;
        }

        /// <summary>
        /// 判断该属性下是否存在相同的属性值
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool IsExistsPropertyValueByPropertySysNo(int sysNo, int propertySysNo, string propertyValue)
        {
            bool rtn = false;
            rtn = propertyDA.IsExistsPropertyValueByPropertySysNo(sysNo, propertySysNo, propertyValue);

            return rtn;
        }

        /// <summary>
        /// 判断该属性值，是否已经有产品使用
        /// 不能修改为无效，属性值已生成CommonSKU!
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExistsProductPropertyValue(PropertyValueInfo entity)
        {
            bool rtn = false;
            rtn = propertyDA.IsExistsProductPropertyValue(entity);

            return rtn;
        }
    }
}
