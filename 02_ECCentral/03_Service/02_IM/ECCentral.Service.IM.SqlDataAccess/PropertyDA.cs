using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IPropertyDA))]
    public class PropertyDA : IPropertyDA
    {
        /// <summary>
        /// 创建Property
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyInfo AddProperty(PropertyInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AddProperty");

            dc.SetParameterValue("@PropertyDescription", entity.PropertyName.Content.Trim());
            dc.SetParameterValue("@IsActive", entity.Status);
            dc.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));

            return entity;
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyInfo UpdateProperty(PropertyInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProperty");

            dc.SetParameterValue("@SysNo", entity.SysNo.Value);
            dc.SetParameterValue("@PropertyDescription", entity.PropertyName.Content.Trim());
            dc.SetParameterValue("@IsActive", entity.Status);
            dc.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);

            dc.ExecuteNonQuery();

            return entity;
        }

        /// <summary>
        /// 根据属性名称获取Entity
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyByPropertyName(string propertyName)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyByPropertyName");

            dc.SetParameterValue("@PropertyName", propertyName);

            return dc.ExecuteEntity<PropertyInfo>();

        }

        /// <summary>
        /// 类别属性使用
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public List<PropertyInfo> GetPropertyListByPropertyName(string propertyName)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyListByPropertyName");

            dc.SetParameterValue("@PropertyName", propertyName);

            return dc.ExecuteEntityList<PropertyInfo>();

        }

        /// <summary>
        /// 判断属性名称是否重复
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfoByEntity(PropertyInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyInfoByEntity");

            dc.SetParameterValue("@PropertySysNo", entity.SysNo.Value);
            dc.SetParameterValue("@PropertyName", entity.PropertyName.Content);

            return dc.ExecuteEntity<PropertyInfo>();
        }

        /// <summary>
        /// 根据SysNO获取Property信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyBySysNo(int sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyBySysNo");

            dc.SetParameterValue("@SysNo", sysno);

            return dc.ExecuteEntity<PropertyInfo>();
        }

        public List<PropertyValueInfo> GetPropertyValueByPropertySysNo(int sysno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据商品编号获取商品属性信息
        /// </summary>
        /// <param name="productsysNo"></param>
        /// <returns></returns>
        public List<PropertyValueInfo> GetPropertyValueByProductSysNo(int productsysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPropertyValueByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productsysNo);
            var sourceEntity = cmd.ExecuteEntityList<PropertyValueInfo>() ??
                              new List<PropertyValueInfo>();
            return sourceEntity;
        }

        public List<PropertyValueInfo> GetPropertyValueByPropertyName(string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 增加属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyValueInfo CreatePropertyValue(PropertyValueInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreatePropertyValue");

            dc.SetParameterValue("@PropertySysNo", entity.PropertyInfo.SysNo);
            dc.SetParameterValue("@ValueDescription", entity.ValueDescription.Content.Trim());
            dc.SetParameterValue("@Priority", entity.Priority);
            dc.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@IsActive", entity.ValueStatus);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));

            return entity;
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyValueInfo UpdatePropertyValue(PropertyValueInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdatePropertyValue");

            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@PropertySysNo", entity.PropertyInfo.SysNo);
            dc.SetParameterValue("@ValueDescription", entity.ValueDescription.Content.Trim());
            dc.SetParameterValue("@Priority", entity.Priority);
            dc.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@IsActive", entity.ValueStatus);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();

            return entity;
        }

        /// <summary>
        /// 根据属性值查询属性信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public PropertyValueInfo GetPropertyValueByPropertyValueSysNo(int sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyValueByPropertyValueSysNo");

            dc.SetParameterValue("@SysNo", sysno);

            return dc.ExecuteEntity<PropertyValueInfo>();
        }

        /// <summary>
        /// 判断该属性下是否存在相同的属性值
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool IsExistsPropertyValueByPropertySysNo(int sysNo, int propertySysNo, string propertyValue)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IsExistsPropertyValueByPropertySysNo");

            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@PropertySysNo", propertySysNo);
            dc.SetParameterValue("@ValueDescription", propertyValue.Trim());

            if ((int)dc.ExecuteScalar() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断该属性值，是否已经有产品使用
        /// 不能修改为无效，属性值已生成CommonSKU!
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExistsProductPropertyValue(PropertyValueInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IsExistsProductPropertyValue");

            dc.SetParameterValue("@SysNo", entity.SysNo.Value);

            if ((int)dc.ExecuteScalar() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有属性
        /// </summary>
        /// <returns></returns>
        public List<PropertyInfo> GetAllProperty()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllProperty");
            var sourceEntity = cmd.ExecuteEntityList<PropertyInfo>() ??
                              new List<PropertyInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 根据PropertySysNo 字符串数组查询属性值信息
        /// </summary>
        /// <param name="sysnoStr"></param>
        /// <returns></returns>
        public List<PropertyValueInfo> GetPropertyValueInfoListByPropertySysNo(string sysnoStr)
        {
            if (string.IsNullOrEmpty(sysnoStr))
            {
                return new List<PropertyValueInfo>();
            }

            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyValueInfoListByPropertySysNo");

            dc.SetParameterValue("@SysNoSTR", sysnoStr);

            return dc.ExecuteEntityList<PropertyValueInfo>();
        }

        public int GetPropertyGroupSysNoByPropertySysNo(int? categorySysNo, int propertySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyGroupSysNoByPropertySysNo");
            dc.SetParameterValue("@CategorySysNo", categorySysNo);
            dc.SetParameterValue("@PropertySysNo", propertySysNo);
            return (int)dc.ExecuteScalar();
        }

        /// <summary>
        /// 获取属性组名称
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public PropertyGroupInfo GetPropertyGroupBySysNo(int? sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyGroupBySysNo");
            dc.SetParameterValue("@SysNo", sysNo);
            return dc.ExecuteEntity<PropertyGroupInfo>();
        }

    }
}