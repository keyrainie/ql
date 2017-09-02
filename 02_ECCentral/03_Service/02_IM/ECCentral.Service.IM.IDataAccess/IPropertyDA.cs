using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IPropertyDA
    {
        /// <summary>
        /// 增加属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PropertyInfo AddProperty(PropertyInfo entity);

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <param name="entity"></param>
        PropertyInfo UpdateProperty(PropertyInfo entity);

        /// <summary>
        /// 根据属性名称获取属性信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        PropertyInfo GetPropertyByPropertyName(string propertyName);

        /// <summary>
        /// 类别属性使用
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        List<PropertyInfo> GetPropertyListByPropertyName(string propertyName);

        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        PropertyInfo GetPropertyInfoByEntity(PropertyInfo entity);

        /// <summary>
        /// 根据属性SysNo获取属性信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        PropertyInfo GetPropertyBySysNo(int sysno);

        /// <summary>
        /// 根据属性名称获取属性值列表
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        List<PropertyValueInfo> GetPropertyValueByPropertySysNo(int sysno);

        /// <summary>
        /// 根据属性名称获取属性值列表
        /// </summary>
        /// <param name="productsysNo"></param>
        /// <returns></returns>
        List<PropertyValueInfo> GetPropertyValueByProductSysNo(int productsysNo);

        /// <summary>
        /// 根据属性SysNo获取属性值列表
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        List<PropertyValueInfo> GetPropertyValueByPropertyName(string propertyName);

        /// <summary>
        /// 增加属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PropertyValueInfo CreatePropertyValue(PropertyValueInfo entity);

        /// <summary>
        /// 修改属性值
        /// </summary>
        /// <param name="entity"></param>
        PropertyValueInfo UpdatePropertyValue(PropertyValueInfo entity);


        /// <summary>
        /// 根据属性值名称获取属性值信息
        /// </summary>
        /// <param name="propertyValueName"></param>
        /// <returns></returns>
        //PropertyValueInfo GetPropertyValueByPropertyValueName(string propertyValueName);

        /// <summary>
        /// 根据属性值SysNo获取属性值信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        PropertyValueInfo GetPropertyValueByPropertyValueSysNo(int sysno);

        /// <summary>
        /// 根据属性SysNo和属性值，判断数据库中是否存在重复记录
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        bool IsExistsPropertyValueByPropertySysNo(int sysNo, int propertySysNo, string propertyValue);

        /// <summary>
        /// 判断该属性值，是否已经有产品使用
        /// 不能修改为无效，属性值已生成CommonSKU!
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsExistsProductPropertyValue(PropertyValueInfo entity);

        /// <summary>
        /// 根据获取所有有效属性
        /// </summary>
        /// <returns></returns>
        List<PropertyInfo> GetAllProperty();

        /// <summary>
        /// 根据PropertySysNo 字符串数组查询属性值信息
        /// </summary>
        /// <param name="sysnoStr"></param>
        /// <returns></returns>
        List<PropertyValueInfo> GetPropertyValueInfoListByPropertySysNo(string sysnoStr);

        /// <summary>
        /// 根据类别和属性SysNo获取该属性在该类别属性模板中的属性组SysNo
        /// </summary>
        /// <returns></returns>
        int GetPropertyGroupSysNoByPropertySysNo(int? categorySysNo, int propertySysNo);

        /// <summary>
        /// 根据编号获取属性组信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        PropertyGroupInfo GetPropertyGroupBySysNo(int? sysNo);
    }
}
