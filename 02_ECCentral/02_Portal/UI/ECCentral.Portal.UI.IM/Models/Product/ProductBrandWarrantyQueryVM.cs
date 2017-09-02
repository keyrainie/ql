using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Text.RegularExpressions;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductBrandWarrantyQueryVM : ModelBase
    {

        public ProductBrandWarrantyQueryVM()
        { 
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        /// 
        public string BrandName { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 一级分类名称
        /// </summary>
        public string C1Name { get; set; }

        /// <summary>
        /// 二级分类名称
        /// </summary>
        public string C2Name { get; set; }

        /// <summary>
        /// 三级分类名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 保修天数
        /// </summary>
        public string _warrantyDay;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^1$|^[1-9]\d{0,7}$", ErrorMessageResourceType=typeof(ResProductMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string WarrantyDay 
        {
            get { return _warrantyDay; }
            set { base.SetValue("WarrantyDay", ref _warrantyDay, value); }
        }

        /// <summary>
        /// 详细说明
        /// </summary>
        public string _warrantyDesc;
        [Validate(ValidateType.Required)]
        public string WarrantyDesc 
        {
            get { return _warrantyDesc; }
            set { base.SetValue("WarrantyDesc", ref _warrantyDesc, value); }
        }

        /// <summary>
        /// 新建日期
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 新建用户
        /// </summary>
        public String InUser { get; set; }

        /// <summary>
        /// 创建人/创建时间
        /// </summary>
        public String InUserAndInDate 
        {
            get 
            {
                return String.Format("{0}[{1}]", InUser, InDate);
            }
        }

        /// <summary>
        /// 编辑日期
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public String EditUser { get; set; }

        /// <summary>
        /// 编辑人/编辑时间
        /// </summary>
        public String EditUserAndEditDate
        {
            get
            {
                return 
                    EditDate == DateTime.MinValue 
                    ? String.Empty : String.Format("{0}[{1}]", EditUser, EditDate);
            }
        }

        /// <summary>
        /// 顯示品牌詳細信息
        /// </summary>
        public String DispWarrantyDesc
        {
            get
            {
                if (!String.IsNullOrEmpty(WarrantyDesc))
                {
                    String _dispWarrantyDesc = NoHTML(WarrantyDesc);
                    return _dispWarrantyDesc.Length > 20
                        ? _dispWarrantyDesc.Substring(0, 20)
                        : _dispWarrantyDesc;
                }
                return string.Empty;
            }
        }

        #region Private Method
        public static string NoHTML(string Htmlstring) //去除HTML标记   
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            return Htmlstring;
        }  
        #endregion
    }
}
