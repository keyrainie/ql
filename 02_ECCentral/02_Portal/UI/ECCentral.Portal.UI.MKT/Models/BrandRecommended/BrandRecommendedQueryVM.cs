
using System;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class BrandRecommendedQueryVM : ModelBase
    {
        public int? BrandType { get; set; }
        public int? LevelCode { get; set; }
        public int? LevelCodeParent { get; set; }
    }
}
