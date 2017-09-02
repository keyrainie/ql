using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(HelpCenterAppService))]
    public class HelpCenterAppService
    {
        private HelpCenterProcessor _processor = ObjectFactory<HelpCenterProcessor>.Instance;

        /// <summary>
        /// 创建帮助主题
        /// </summary>
        public virtual void Create(HelpTopic item)
        {
            _processor.Create(item);
        }

        /// <summary>
        /// 更新帮助主题
        /// </summary>
        public virtual void Update(HelpTopic item)
        {
            _processor.Update(item);
        }

        /// <summary>
        /// 加载一个帮助主题
        /// </summary>
        public virtual HelpTopic Load(int sysNo)
        {
            return _processor.Load(sysNo);
        }
    }
}
