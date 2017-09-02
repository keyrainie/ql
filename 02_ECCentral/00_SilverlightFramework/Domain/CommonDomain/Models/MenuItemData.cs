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
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models
{
    /// <summary>
    /// 提供导入导出的实体
    /// </summary>
    public class MenuItemData
    {
        public string MenuId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LinkPath { get; set; }

        public string MenuType { get; set; }

        public string AuthKey { get; set; }

        public string ParentMenuId { get; set; }

        public string ApplicationName { get; set; }

        public bool? IsDisplay { get; set; }

        public string Status { get; set; }

        public List<LocalizedRes> LocalizedResList { get; set; }
    }
}
