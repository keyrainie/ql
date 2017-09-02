using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CustomerRightProcessor))]
    public class CustomerRightProcessor
    {
        private IRightDA rightDA = ObjectFactory<IRightDA>.Instance;
        /// <summary>
        /// 更新用户权限
        /// </summary>
        /// <param name="right"></param>
        public virtual void UpdateCustomerRight(int CustomerSysNo, List<CustomerRight> right)
        {
            List<CustomerRight> oldRight = LoadAllCustomerRight(CustomerSysNo);
            var temp = right.Intersect(oldRight).ToList<CustomerRight>();
            //减去
            foreach (CustomerRight r in oldRight.Except(temp).ToList<CustomerRight>())
            {
                rightDA.UpdateCustomerRight(CustomerSysNo, r.Right.Value);
            }
            //新增
            foreach (CustomerRight r in right.Except(temp).ToList<CustomerRight>())
            {
                rightDA.CreateCustomerRight(CustomerSysNo, r.Right.Value);
            }
        }

        /// <summary>
        /// 获取用户所有的权限
        /// </summary>
        /// <param name="right"></param>
        public virtual List<CustomerRight> LoadAllCustomerRight(int customerSysNo)
        {
            return rightDA.LoadAllCustomerRight(customerSysNo);
        }
    }
}
