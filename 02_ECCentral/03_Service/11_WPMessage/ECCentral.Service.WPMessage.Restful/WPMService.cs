using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using WPM = ECCentral.WPMessage.BizEntity;
using ECCentral.Service.WPMessage.AppService;
using ECCentral.WPMessage.BizEntity;
using ECCentral.Service.WPMessage.Restful.RequestMsg;
using System.Data;
using ECCentral.Service.WPMessage.IDataAccess;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.WPMessage.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class WPMService
    {

        [WebInvoke(UriTemplate = "Message/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryWPMessageByUserSysNo(ECCentral.WPMessage.QueryFilter.WPMessageQueryFilter request)
        {
            int dataCount;
            DataTable dt = ObjectFactory<IWPMessageDA>.Instance.QueryWPMessageByUserSysNo(request, out dataCount);
            return new QueryResult
            {
                Data = dt,
                TotalCount = dataCount
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/DoMessage/", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        public void DoMessage(ESBMessage msg)
        {
            ObjectFactory<WPMessageAppService>.Instance.DoESBMessage(msg);
        }


        #region WPMessage相关处理
        [WebInvoke(UriTemplate = "Message/Processing", Method = "PUT")]
        public void UpdateWPMessageToProcessing(int msgSysNo)
        {
            ObjectFactory<WPMessageAppService>.Instance.ProcessWPMessage(msgSysNo, Service.Utility.ServiceContext.Current.UserSysNo);
        }

        [WebInvoke(UriTemplate = "Message/HasMesssage/{userSysNo}", Method = "GET")]
        public bool HasWPMessageByUserSysNo(string userSysNo)
        {
            int no;
            if (int.TryParse(userSysNo, out no))
            {
                return ObjectFactory<WPMessageAppService>.Instance.HasWPMessageByUserSysNo(no);
            }
            return false;
        }

        #endregion

        #region WPMessageCategory相关处理

        [WebInvoke(UriTemplate = "Category/GetAll", Method = "GET")]
        public List<WPMessageCategory> GetAllCategory()
        {
            return ObjectFactory<WPMessageAppService>.Instance.GetAllCategory();
        }
        [WebInvoke(UriTemplate = "Category/Get/{UserSysNo}", Method = "GET")]
        public List<WPMessageCategory> GetCategoryByUserSysNo(string userSysNo)
        {
            int no;
            if (int.TryParse(userSysNo, out no))
            {
                return ObjectFactory<WPMessageAppService>.Instance.GetCategoryByUserSysNo(no);
            }
            return null;
        }

        [WebInvoke(UriTemplate = "Category/UpdateRole", Method = "PUT")]
        public void UpdateCategoryRoleByCategorySysNo(UpdateWPMessageCategoryRoleReq request)
        {
            if (request == null)
            {
                return;
            }
            ObjectFactory<WPMessageAppService>.Instance.UpdateCategoryRoleByCategorySysNo(request.CategorySysNo, request.RoleSysNoList);
        }

        [WebInvoke(UriTemplate = "Category/GetRole/{categorySysNo}", Method = "GET")]
        public List<int> GetRoleSysNoByCategorySysNo(string categorySysNo)
        {
            int no;
            if (int.TryParse(categorySysNo, out no))
            {
                return ObjectFactory<WPMessageAppService>.Instance.GetRoleSysNoByCategorySysNo(no);
            }
            return null;
        }

        [WebInvoke(UriTemplate = "Category/UpdateRoleCategory", Method = "PUT")]
        public void UpdateCategoryRoleByRoleSysNo(UpdateWPMessageCategoryRoleByRoleSysNoReq request)
        {
            if (request == null)
            {
                return;
            }
            ObjectFactory<WPMessageAppService>.Instance.UpdateCategoryRoleByRoleSysNo(request.RoleSysNo, request.CategorySysNoList);
        }

        [WebInvoke(UriTemplate = "Category/GetCategory/{roleSysNo}", Method = "GET")]
        public List<int> GetCategorySysNoByRoleSysNo(string roleSysNo)
        {
            int no;
            if (int.TryParse(roleSysNo, out no))
            {
                return ObjectFactory<WPMessageAppService>.Instance.GetCategorySysNoByRoleSysNo(no);
            }
            return null;
        }
        #endregion

    }
}
