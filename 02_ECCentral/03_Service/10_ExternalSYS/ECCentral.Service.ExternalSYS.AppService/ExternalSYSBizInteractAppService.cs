using System.ComponentModel.Composition;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.ExternalSYS.AppService
{
    [Export(typeof(ExternalSYSBizInteractAppService))]
    [VersionExport(typeof(IExternalSYSBizInteract))]
    public class ExternalSYSBizInteractAppService : IExternalSYSBizInteract
    {
        #region EIMS

        #endregion

        #region VendorPortal

        #endregion
    }
}
