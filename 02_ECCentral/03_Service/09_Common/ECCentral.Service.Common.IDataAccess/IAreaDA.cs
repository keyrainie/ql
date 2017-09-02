using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IAreaDA
    {
        AreaInfo Create(AreaInfo areaInfo);

        AreaInfo Update(AreaInfo areaInfo);

        AreaInfo Load(int areaSysNo);

        DataTable QueryAreaList(AreaQueryFilter filter, out int totalCount);

        DataTable QueryAreaNoDistrictList(AreaQueryFilter filter, out int totalCount);

        List<AreaInfo> QueryProvinceAreaList();

        List<AreaInfo> QueryCityAreaListByProvinceSysNo(int provinceSysNo);

        List<AreaInfo> QueryDistrictAreaListByCitySysNo(int citySysNo);

        void QueryCurrentAreaStructure(int sysNo, out AreaInfo currentAreaInfo, out List<AreaInfo> parentProviceList, out List<AreaInfo> parentCityList, out  List<AreaInfo> SiblingAreaList);
        void QueryCurrentAreaStructure_Old(int sysNo, out AreaInfo currentAreaInfo, out List<AreaInfo> parentProviceList, out List<AreaInfo> parentCityList, out  List<AreaInfo> SiblingAreaList);
    }
}