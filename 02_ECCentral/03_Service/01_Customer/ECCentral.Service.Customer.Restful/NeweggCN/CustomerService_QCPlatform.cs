using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NeweggCN.NoBizQuery;
using ECCentral.Service.Customer.AppService;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        /// <summary>
        /// 获取顾客经验值历史
        /// </summary>
        [WebInvoke(UriTemplate = "/QCSubject/GetQCSubjectTree", Method = "POST")]
        public virtual QCSubject GetQCSubjectTree(QCSubjectFilter filter)
        {
            var list = ObjectFactory<IQCSubjectQueryDA>.Instance.GetAllQCSubject(filter);
            QCSubject root = new QCSubject();
            root.Name = "Root";
            root.SysNo = 0;
            BuildTree(root, list);
            return root;
        }

        private void BuildTree(QCSubject parent, List<QCSubject> list)
        {
            parent.ChildrenList = list.Where(item => (item.ParentSysNo ?? 0) == parent.SysNo).ToList();
            foreach (var c in parent.ChildrenList)
            {
                BuildTree(c, list);
            }
        }

        [WebInvoke(UriTemplate = "/QCSubject/Create", Method = "POST")]
        public virtual void CreateQCSubject(QCSubject entity)
        {
            ObjectFactory<QCSubjectAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/QCSubject/Update", Method = "PUT")]
        public virtual void UpdateQCSubject(QCSubject entity)
        {
            ObjectFactory<QCSubjectAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/QCSubject/GetParent", Method = "POST")]
        public virtual List<QCSubject> GetParents(QCSubject entity)
        {
            return ObjectFactory<QCSubjectAppService>.Instance.GetParentsQCSubject(entity);
        }
    }
}
