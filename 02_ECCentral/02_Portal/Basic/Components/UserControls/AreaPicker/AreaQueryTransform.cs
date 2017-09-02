using System;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Common.Restful.ResponseMsg;

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    /// <summary>
    /// 将服务端返回的数据转换成AreaPicker需要的视图模型。
    /// </summary>
    public class AreaQueryTransform
    {
        public static AreaPickerVM Transform(AreaQueryResponse resp)
        {
            AreaPickerVM vm = new AreaPickerVM();
            if (resp.CurrentAreaInfo != null)
            {
                vm.CurrentArea = resp.CurrentAreaInfo.Convert<AreaInfo, AreaInfoVM>();
                vm.AllArea = resp.ProviceList.Convert<AreaInfo, AreaInfoVM>()
                    .Union(resp.CityList.Convert<AreaInfo, AreaInfoVM>())
                    .Union(resp.DistrictList.Convert<AreaInfo, AreaInfoVM>())
                    .ToList();
            }
            else
            {
                vm.CurrentArea = new AreaInfoVM();
                vm.AllArea = resp.ProviceList.Convert<AreaInfo, AreaInfoVM>()
                    .ToList();
            }

            return vm;
        }

        public static AreaVM_Old Transform_Old(AreaQueryResponse resp)
        {
            AreaVM_Old vm = new AreaVM_Old();
            if (resp.CurrentAreaInfo != null)
            {
                vm.CurrentArea = resp.CurrentAreaInfo.Convert<AreaInfo, AreaInfoVM_Old>();
                vm.AllArea = resp.ProviceList.Convert<AreaInfo, AreaInfoVM_Old>()
                    .Union(resp.CityList.Convert<AreaInfo, AreaInfoVM_Old>())
                    .Union(resp.DistrictList.Convert<AreaInfo, AreaInfoVM_Old>())
                    .ToList();
            }
            else
            {
                vm.CurrentArea = new AreaInfoVM_Old();
                vm.AllArea = resp.ProviceList.Convert<AreaInfo, AreaInfoVM_Old>()
                    .ToList();
            }

            return vm;
        }
    }
}