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
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Utilities
{
    public class OperationControlStatusHelper
    {
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }

                childList.AddRange(GetChildObjects<T>(child, ""));
            }
            return childList;
        }

        public static void SetControlsStatus(DependencyObject container,bool isOnlyView)
        {
            List<RadioButton> rdoList = new List<RadioButton>();
            rdoList.AddRange(GetChildObjects<RadioButton>(container, ""));
            foreach (RadioButton rdo in rdoList)
            {
                rdo.IsEnabled = !isOnlyView;
            }

            List<CheckBox> chkList = new List<CheckBox>();
            chkList.AddRange(GetChildObjects<CheckBox>(container, ""));
            foreach(CheckBox chk in chkList)
            {
                chk.IsEnabled=!isOnlyView;
            }

            List<TextBox> tbList = new List<TextBox> ();
            tbList.AddRange(GetChildObjects<TextBox>(container,""));
            foreach(TextBox tb in tbList)
            {
                tb.IsReadOnly=isOnlyView;
            }
            
            List<Button> btnList=new List<Button>();
            btnList.AddRange(GetChildObjects<Button>(container, ""));
            foreach(Button btn in btnList)
            {
                btn.Visibility = isOnlyView ? Visibility.Collapsed : Visibility.Visible;
            }

            List<HyperlinkButton> btnLinkList = new List<HyperlinkButton>();
            btnLinkList.AddRange(GetChildObjects<HyperlinkButton>(container, ""));
            foreach (var btn in btnLinkList)
            {
                btn.IsEnabled = !isOnlyView;
            }

            List<Combox> cmbList = new List<Combox>();
            cmbList.AddRange(GetChildObjects<Combox>(container, ""));
            foreach (Combox cmb in cmbList)
            {
                cmb.IsEnabled = !isOnlyView;
            }
        }

        public static void SetAllButtonNotEnabled(DependencyObject container)
        {
            List<Button> btnList = new List<Button>();
            btnList.AddRange(GetChildObjects<Button>(container, ""));
            btnList.ForEach(p => p.IsEnabled = false);
        }
    }
}
