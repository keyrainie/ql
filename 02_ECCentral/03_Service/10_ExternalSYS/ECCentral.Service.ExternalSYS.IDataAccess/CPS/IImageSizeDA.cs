using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
  public  interface IImageSizeDA
    {
        /// <summary>
        /// 得到所有图片尺寸
        /// </summary>
        /// <returns></returns>
        DataTable GetAllImageSize(ImageSizeQueryFilter query,out int totalCount);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        void CreateImageSzie(ImageSizeInfo info);

        /// <summary>
        /// 图片尺寸是否存在
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool IsExistsImageSize(ImageSizeInfo info);

        /// <summary>
        /// 删除时更新标识位 逻辑删除
        /// </summary>
        /// <param name="SysNo"></param>
        void UpdateImageSizeFlag(int SysNo);


     
    }
}
