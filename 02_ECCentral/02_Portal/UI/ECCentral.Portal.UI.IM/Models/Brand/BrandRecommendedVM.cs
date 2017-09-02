
//using System.Collections.Generic;
//using ECCentral.BizEntity.IM;
//using ECCentral.Portal.Basic.Utilities;
//using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
//using Newegg.Oversea.Silverlight.Utilities.Validation;


//namespace ECCentral.Portal.UI.IM.Models
//{
//    public class BrandRecommendedVM:ModelBase
//    {
//        public int Sysno { get; set; }

      
//        private string brandRank;
//        [Validate(ValidateType.Required)]
//        [Validate(ValidateType.Regex, @"^((\d+[,，]){0,10000})\d+$", ErrorMessage = "输入数字以，隔开")]
//        public string BrandRank {
//            get { return brandRank; }
//            set { base.SetValue("BrandRank", ref brandRank, value); }
//          }
//    }
//}
