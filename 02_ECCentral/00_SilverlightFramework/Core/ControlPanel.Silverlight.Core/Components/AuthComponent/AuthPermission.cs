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

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class AuthPermission
    {
        public string AuthKey { get; set; }

        public string LinkPath { get; set; }

        public AuthPermissionType PermissionType { get; set; }
    }

    public enum AuthPermissionType
    {
        View,
        Edit,
    }
}
