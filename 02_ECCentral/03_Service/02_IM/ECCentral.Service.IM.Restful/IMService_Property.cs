using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{

    public partial class IMService
    {
        /// <summary>
        /// 查询属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/QueryProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProperty(PropertyQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "RequestIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IPropertyQueryDA>.Instance.QueryPropertyList(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据属性名获取属性信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/GetPropertyByPropertyName", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public PropertyInfo GetPropertyByPropertyName(string propertyName)
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.GetPropertyByPropertyName(propertyName);
            return entity;
        }

        /// <summary>
        /// 根据属性名获取属性信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/GetPropertyListByPropertyName", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<PropertyInfo> GetPropertyListByPropertyName(string propertyName)
        {

            if (string.IsNullOrEmpty(propertyName))
            {
                var list = GetAllProperty();
                return list;
            }
            //var source = new List<PropertyInfo>();
            var entity = ObjectFactory<PropertyAppService>.Instance.GetPropertyListByPropertyName(propertyName);
            //if (entity == null) return source;
            //source.Add(entity);
            return entity;
        }

        /// <summary>
        /// 创建Property
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/CreateProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public PropertyInfo CreateProperty(PropertyInfo request)
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.CreateProperty(request);
            return entity;
        }

        /// <summary>
        /// 根据SysNO获取Property信息
        /// </summary>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/LoadPropertyBySysNo/{sysNo}", Method = "GET")]
        public PropertyInfo GetPropertyBySysNo(string sysNo)
        {
            int sysno = int.Parse(sysNo);
            return ObjectFactory<PropertyAppService>.Instance.GetPropertyBySysNo(sysno);
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/UpdateProperty", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public PropertyInfo UpdateProperty(PropertyInfo request)
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.UpdateProperty(request);
            return entity;
        }

        /// <summary>
        /// 根据PropertySysNo获取属性值列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/QueryPropertyValueListByPropertySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPropertyValueListByPropertySysNo(PropertyValueQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Property", "RequestIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IPropertyQueryDA>.Instance.QueryPropertyValueListByPropertySysNo(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 创建属性值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/CreatePropertyValue", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public PropertyValueInfo CreatePropertyValue(PropertyValueInfo request)
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.CreatePropertyValue(request);
            return entity;
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/UpdatePropertyValue", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public PropertyValueInfo UpdatePropertyValue(PropertyValueInfo request)
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.UpdatePropertyValue(request);
            return entity;
        }

        /// <summary>
        /// 获取所有属性
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/GetAllProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<PropertyInfo> GetAllProperty()
        {
            var entity = ObjectFactory<PropertyAppService>.Instance.GetAllProperty();
            return entity;
        }


        /// <summary>
        /// 根据PropertySysNoList获取Property信息
        /// </summary>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Property/GetPropertyValueInfoByPropertySysNoList", Method = "POST")]
        public Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList)
        {            
            return ObjectFactory<PropertyAppService>.Instance.GetPropertyValueInfoByPropertySysNoList(sysNoList);
        }
    }
}
