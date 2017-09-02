using ECCentral.BizEntity.MKT;
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

namespace ECCentral.Portal.UI.MKT.Facades.RequestMsg.ECCategory
{
    public class ECCategoryMange
    {
        public int? Category1SysNo
        {
            get;
            set;
        }
        public int? Category2SysNo
        {
            get;
            set;
        }
        public ECCCategoryManagerType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 类别ID
        /// </summary>
        public string CategoryID
        {
            get;
            set;
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        public ECCCategoryManagerStatus? Status { get; set; }

        public int? SysNo
        {
            get;
            set;
        }
    }
}
