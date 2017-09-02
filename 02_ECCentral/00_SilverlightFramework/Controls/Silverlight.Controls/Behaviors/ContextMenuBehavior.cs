using System.Collections.ObjectModel;
using System.Diagnostics;

/********************************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 *
 * Author: Roger Yan(Roger.y.yan@newegg.com)
 * Create Date: 12/03/2009 2:24:52 PM
 * Description:
 *
 * Revision History:
 *      Date         Author               Description
 *
*********************************************************************************/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interactivity;
using Newegg.Oversea.Silverlight.Controls;
using System;
using System.Windows.Input;


namespace Newegg.Oversea.Silverlight.Behaviors
{
    public class ContextMenuBehavior : Behavior<System.Windows.FrameworkElement>
    {
        #region DependencyProperty

        public ContextMenu ContextMenu
        {
            get { return GetValue(ContextMenuProperty) as ContextMenu; }
            set { SetValue(ContextMenuProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register("ContextMenuProperty", typeof(ContextMenu), typeof(ContextMenuBehavior), new PropertyMetadata(null));

        #endregion DependencyProperty

        #region Property Method

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            AssociatedObject.Unloaded += new RoutedEventHandler(AssociatedObject_Unloaded);
        }
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            ContextMenu = null;
            System.Windows.Controls.ContextMenuService.SetContextMenu(AssociatedObject, null);
        }

        #endregion Property Method

        #region Event

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu = ContextMenu ?? new NeweggContextMenu();
            System.Windows.Controls.ContextMenuService.SetContextMenu(AssociatedObject, ContextMenu);
        }

        //由于Toolkit的ContextMenu有内存泄露问题，所以这里，在unload的时候需要，注销这些事件，以及取消AssociatedObject的ContextMenu的关联；
        void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= new RoutedEventHandler(AssociatedObject_Unloaded);
            AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            if (ContextMenu != null)
            {
                ContextMenu.Tag = null;
            }
            ContextMenu = null;
            System.Windows.Controls.ContextMenuService.SetContextMenu(AssociatedObject, null);
        }

        private void AssociatedObject_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ContextMenu.Tag = e.OriginalSource;
            FrameworkElement fm = e.OriginalSource as FrameworkElement;
            if (fm != null)
            {
                fm.Unloaded += new RoutedEventHandler(fm_Unloaded);
            }

            //阻止ContextMenu原始的MouseRightButtonDown时间,解决重复出现child的问题;
            if (fm != null && ContextMenu != null)
            {
                if (ContextMenu.IsOpen)
                {
                    ContextMenu.IsOpen = false;
                    e.Handled = true;
                }
            }
        }
        void fm_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement fm = sender as FrameworkElement;
            if (fm != null)
            {
                fm.Unloaded -= new RoutedEventHandler(fm_Unloaded);
            }
            if (ContextMenu != null)
            {
                ContextMenu.Tag = null;
            }
        }

        #endregion Event
    }
}