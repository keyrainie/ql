using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IUnifiedImageDA
    {
        /// <summary>
        /// 添加图片信息
        /// </summary>
        /// <param name="entity"></param>
        void CreateUnifiedImage(UnifiedImage entity);

     
    }
}
