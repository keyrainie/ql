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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.ComponentModel;
using Newegg.Oversea.Silverlight.Controls.Containers;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class Shortcuts
    {
        internal static Dictionary<IPage, List<System.Windows.Controls.Button>> ShortcutKeyButtons = new Dictionary<IPage, List<System.Windows.Controls.Button>>();
        internal static Dictionary<IDialog, List<System.Windows.Controls.Button>> ShortcutKeyButtonsInDialog = new Dictionary<IDialog, List<System.Windows.Controls.Button>>();

        static Shortcuts()
        {
            Application.Current.RootVisual.KeyDown += new KeyEventHandler(RootVisual_KeyDown);
        }

        static void RootVisual_KeyDown(object sender, KeyEventArgs e)
        {
            if ((CPApplication.Current.CurrentPage != null 
                && CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.IsOpen)
                || PopupBox.PopupBoxes.Count > 0)
            {
                return;
            }
            ModifierKeys keys = Keyboard.Modifiers;
            if (CPApplication.Current.CurrentPage != null)
            {
                List<Button> buttonList = null;
                bool isExists = ShortcutKeyButtons.TryGetValue(CPApplication.Current.CurrentPage, out buttonList);
                if (isExists)
                {
                    foreach (var button in buttonList)
                    {
                        if (!GetEnable(button))
                        {
                            continue;
                        }

                        if (GetKey(button) == e.Key && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                        {
                            if (!button.IsAvailable())
                            {
                                continue;
                            }

                            ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
                            if (peer != null)
                            {
                                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                                if (invokeProv != null)
                                    invokeProv.Invoke();
                            }
                            return;
                        }
                    }
                }
            }
        }

        #region DependencyProperty

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(Key), typeof(Shortcuts), new PropertyMetadata(Key.Home, KeyPropertyChangedCallback));

        public static void SetKey(DependencyObject obj, Key value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static Key GetKey(DependencyObject obj)
        {
            return (Key)obj.GetValue(KeyProperty);
        }

        public static readonly DependencyProperty IsShowInDialogProperty = DependencyProperty.RegisterAttached("IsShowInDialog", typeof(bool), typeof(Shortcuts), new PropertyMetadata(false));

        public static void SetIsShowInDialog(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowInDialogProperty, value);
        }

        public static bool GetIsShowInDialog(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowInDialogProperty);
        }

        public static readonly DependencyProperty EnableProperty = DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(Shortcuts), new PropertyMetadata(true));

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        #endregion




        #region Private Event

        private static void KeyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.Controls.Button button = d as System.Windows.Controls.Button;
            if (button != null)
            {
                button.Loaded -= new RoutedEventHandler(button_Loaded);
                button.Loaded += new RoutedEventHandler(button_Loaded);
            }
        }

        static void button_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var button = sender as System.Windows.Controls.Button;

                bool isShowInDialog = GetIsShowInDialog(button);
                if (isShowInDialog)
                {
                    if (ShortcutKeyButtonsInDialog.ContainsKey(PopupBox.PopupBoxes.Peek()))
                    {
                        if (!ShortcutKeyButtonsInDialog[PopupBox.PopupBoxes.Peek()].Contains(button))
                        {
                            ShortcutKeyButtonsInDialog[PopupBox.PopupBoxes.Peek()].Add(button);
                        }
                    }
                    else
                    {
                        IDialog dialog = PopupBox.PopupBoxes.Peek();
                        if (dialog != null)
                        {
                            ShortcutKeyButtonsInDialog.Add(dialog, new List<System.Windows.Controls.Button>() { button });
                            dialog.Closed += new EventHandler(CurrentDialog_Closed);
                            (dialog as PopupBox).KeyDown += new KeyEventHandler(CurrentDialog_KeyDown);
                        }
                    }
                }
                else
                {
                    if (ShortcutKeyButtons.ContainsKey(CPApplication.Current.CurrentPage))
                    {
                        if (!ShortcutKeyButtons[CPApplication.Current.CurrentPage].Contains(button))
                        {
                            ShortcutKeyButtons[CPApplication.Current.CurrentPage].Add(button);
                        }
                    }
                    else
                    {
                        ShortcutKeyButtons.Add(CPApplication.Current.CurrentPage, new List<System.Windows.Controls.Button>() { button });
                        CPApplication.Current.CurrentPage.Context.OnClose += new EventHandler<ControlPanel.Core.Base.PageCloseEventArgs>(Context_OnClose);
                    }
                }
            }
        }

        static void CurrentDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (CPApplication.Current.CurrentPage != null 
                && CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.IsOpen)
            {
                return;
            }
            ModifierKeys keys = Keyboard.Modifiers;
            if (ShortcutKeyButtonsInDialog.ContainsKey(PopupBox.PopupBoxes.Peek()))
            {
                List<System.Windows.Controls.Button> buttons = ShortcutKeyButtonsInDialog[PopupBox.PopupBoxes.Peek()];
                foreach (var button in buttons)
                {
                    if (!GetEnable(button))
                    {
                        continue;
                    }

                    if (GetKey(button) == e.Key && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        if (!button.IsAvailable(PopupBox.PopupBoxes.Peek() as UIElement))
                        {
                            continue;
                        }

                        ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
                        if (peer != null)
                        {
                            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                            if (invokeProv != null)
                                invokeProv.Invoke();
                        }
                        return;
                    }
                }
            }
        }

        static void CurrentDialog_Closed(object sender, EventArgs e)
        {
            if (ShortcutKeyButtonsInDialog.ContainsKey(sender as IDialog))
            {
                ShortcutKeyButtonsInDialog.Remove(sender as IDialog);
            }
        }

        static void Context_OnClose(object sender, ControlPanel.Core.Base.PageCloseEventArgs e)
        {
            if (ShortcutKeyButtons.ContainsKey(sender as IPage))
            {
                ShortcutKeyButtons.Remove(sender as IPage);
            }
        }

        #endregion
    }
}
