using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class StoreBrandFilingVM
    {
        /// <summary>
        ///商检号
        /// </summary>
        public string InspectionNo
        {
            get;
            set;
        }

        public StoreBrandFilingStatus? Staus
        {
            get;
            set;
        }
    }
}
