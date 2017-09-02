using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(HelpCenterProcessor))]
    public class HelpCenterProcessor
    {
        private IHelpCenterDA _da = ObjectFactory<IHelpCenterDA>.Instance;

        /// <summary>
        /// 创建帮助主题
        /// </summary>
        public virtual void Create(HelpTopic item)
        {
            if(_da.CheckTitleExists(item.SysNo??0,item.Title,item.CompanyCode,""))
            {
                //throw new BizException("已经存在此帮助标题，请更换后重试。");
                throw new BizException(ResouceManager.GetMessageString("MKT.HelpCenter", "HelpCenter_ExistsSameTitle"));
            }
            _da.Create(item);
        }

        /// <summary>
        /// 更新帮助主题
        /// </summary>
        public virtual void Update(HelpTopic item)
        {
            if (_da.CheckTitleExists(item.SysNo.Value, item.Title, item.CompanyCode,""))
            {
                //throw new BizException("已经存在此帮助标题，请更换后重试。");
                throw new BizException(ResouceManager.GetMessageString("MKT.HelpCenter", "HelpCenter_ExistsSameTitle"));
            }
            _da.Update(item);
        }

        /// <summary>
        /// 加载一个帮助主题
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>帮助主题</returns>
        public virtual HelpTopic Load(int sysNo)
        {
            return _da.Load(sysNo);
        }
    }
}
