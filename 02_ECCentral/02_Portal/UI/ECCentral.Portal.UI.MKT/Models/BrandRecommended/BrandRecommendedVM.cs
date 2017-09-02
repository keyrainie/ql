
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.MKT.Models
{
    public class BrandRecommendedVM : ModelBase
    {
        public int Sysno { get; set; }


        //private string brandRank;
        //[Validate(ValidateType.Required)]
        //[Validate(ValidateType.Regex, @"^((\d+[,，]){0,10000})\d+$", ErrorMessage = "输入数字以，隔开")]
        //public string BrandRank
        //{
        //    get { return brandRank; }
        //    set { base.SetValue("BrandRank", ref brandRank, value); }
        //}

        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 0=首页，1=一级栏目推荐，2=二级栏目推荐
        /// </summary>
        public int Level_No { get; set; }

        /// <summary>
        /// Level_No=1时，存一级ECCategorySysNo；=2时，存放2级ECCategorySysNo；=0时，存0
        /// </summary>
        public int Level_Code { get; set; }

        public string Level_Name { get; set; }
    }
}
