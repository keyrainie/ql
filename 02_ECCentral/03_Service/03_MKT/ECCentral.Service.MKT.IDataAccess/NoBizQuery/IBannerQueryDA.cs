using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IBannerQueryDA
    {
        DataSet Query(BannerQueryFilter filter, out int totalCount);

        List<BannerDimension> GetBannerDimensions(BannerDimensionQueryFilter filter);

        List<BannerFrame> GetBannerFrame(BannerFrameQueryFilter filter);
    }
}
