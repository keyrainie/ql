using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(ImageSizeProcessor))]
    public class ImageSizeProcessor
    {
        private IImageSizeDA imageSizeDA = ObjectFactory<IImageSizeDA>.Instance;

        /// <summary>
        /// 创建新尺寸
        /// </summary>
        /// <param name="info"></param>
        public void CreateImageSize(ImageSizeInfo info)
        {
            if (!imageSizeDA.IsExistsImageSize(info))
            {
                imageSizeDA.CreateImageSzie(info);
            }
            else
            {
                throw new BizException("该尺寸已存在!");
            }

        }

        /// <summary>
        /// 删除时更新标识位 逻辑删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void UpdateImageSizeFlag(int SysNo)
        {
            imageSizeDA.UpdateImageSizeFlag(SysNo);
        }

    }
}
