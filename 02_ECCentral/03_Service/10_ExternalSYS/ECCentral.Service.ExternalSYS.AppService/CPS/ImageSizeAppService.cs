using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
    [VersionExport(typeof(ImageSizeAppService))]
     public class ImageSizeAppService
    {
        private ImageSizeProcessor processor = ObjectFactory<ImageSizeProcessor>.Instance;
        /// <summary>
        /// 创建新尺寸
        /// </summary>
        /// <param name="info"></param>
        public void CreateImageSize(ImageSizeInfo info)
        {
                 processor.CreateImageSize(info);
        }

        /// <summary>
        /// 删除时更新标识位 逻辑删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void UpdateImageSizeFlag(int SysNo)
        {
            processor.UpdateImageSizeFlag(SysNo);
        }
    }
}
