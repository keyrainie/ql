using System;
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
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAccessoriesVM : ModelBase
    {

        public ProductAccessoriesVM()
        {
            StatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.None);
        }

        public int SysNo { get; set; }

        /// <summary>
        /// 查询功能名称 
        /// </summary>
        private string accessoriesQueryName;
        [Validate(ValidateType.Required)]     
        public string AccessoriesQueryName 
        {
            get { return accessoriesQueryName; }
            set { SetValue("AccessoriesQueryName",ref accessoriesQueryName, value); }
        }

        /// <summary>
        /// 背景图片Url
        /// </summary>
        private string backPictureBigUrl;
         [Validate(ValidateType.URL)]
         [Validate(ValidateType.Required)]   
        public string BackPictureBigUrl 
        {
            get { return backPictureBigUrl; }
            set { SetValue("BackPictureBigUrl", ref backPictureBigUrl, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private ValidStatus status = ValidStatus.DeActive;
        public ValidStatus Status 
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 是否树形结构
        /// </summary>
        private bool isTreeQuery = true;
        public bool IsTreeQuery 
        {
            get { return isTreeQuery; }
            set { isTreeQuery = value; }
        }

        public List<KeyValuePair<ValidStatus?, string>> StatusList { get; set; }

    }
}
