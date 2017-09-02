using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.SO.BizProcessor;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOInterceptAppService))]
    public class SOInterceptAppService
    {
        /// <summary>
        /// 添加订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="companyCode"></param> 
        public virtual void AddSOInterceptInfo(SOInterceptInfo info, string companyCode)
        {
            ObjectFactory<SOInterceptProcessor>.Instance.AddSOInterceptInfo(info, companyCode);
        }
       
        /// <summary>
        /// 批量修改订单拦截设置信息(修改对象可以为 CheckBox 选中的数据，也可能为页面查询条件查询出来的数据 需要Portal准备数据)
        /// </summary>
        /// <param name="info"></param>
        public virtual void BatchUpdateSOInterceptInfo(SOInterceptInfo info)
        {
            ObjectFactory<SOInterceptProcessor>.Instance.BatchUpdateSOInterceptInfo(info);
        }

        /// <summary>
        /// 删除订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        public virtual void DeleteSOInterceptInfo(SOInterceptInfo info)
        {
            ObjectFactory<SOInterceptProcessor>.Instance.DeleteSOInterceptInfo(info);         
        }

        /// <summary>
        /// 发送订单拦截邮件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="Language"></param>
        public virtual void SendSOOrderInterceptEmail(SOInfo info, string Language)
        {
            ObjectFactory<SOInterceptProcessor>.Instance.SendSOOrderInterceptEmail(info, Language);    
        }

        /// <summary>
        /// 发送增票拦截邮件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="Language"></param>
        public virtual void SendSOFinanceInterceptEmail(SOInfo info, string Language)
        {
            ObjectFactory<SOInterceptProcessor>.Instance.SendSOFinanceInterceptEmail(info, Language);
        }
    }
}
