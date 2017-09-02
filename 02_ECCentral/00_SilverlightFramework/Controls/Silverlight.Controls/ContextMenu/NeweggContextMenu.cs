using System;

/********************************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 *
 * Author: Roger Yan(Roger.y.yan@newegg.com)
 * Create Date: 04/16/2010 2:24:52 PM
 * Description:
 *
 * Revision History:
 *      Date         Author               Description
 *
*********************************************************************************/

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.Controls.Resources;
using System.Security;
using System.Threading;


namespace Newegg.Oversea.Silverlight.Controls
{
    public class NeweggContextMenu : ContextMenu
    {
        #region Consturct

        private bool isReallyClose;

        public NeweggContextMenu()
        {
            ObservableCollection<MenuItem> menuList = new ObservableCollection<MenuItem>();

            #region cutMenuItem
            MenuItem cutMenuItem = new MenuItem
            {
                Icon = new Image { Source = new BitmapImage(new Uri("/Themes/Default/Images/ContextMenu/Cut.png", UriKind.Relative)) },
                Header = MessageResource.ContextMenu_Cut,
                Cursor = Cursors.Hand
            };
            cutMenuItem.Click += (s, e) =>
            {
                ExecuteCutCommand();
            };
            menuList.Add(cutMenuItem);
            #endregion 

            #region copyMenuItem
            MenuItem copyMenuItem = new MenuItem
            {
                Icon = new Image { Source = new BitmapImage(new Uri("/Themes/Default/Images/ContextMenu/Copy.png", UriKind.Relative)) },
                Header = MessageResource.ContextMenu_Copy,
                Cursor = Cursors.Hand
            };
            copyMenuItem.Click += (s, e) =>
            {
                ExecuteCopyCommand();
            };
            menuList.Add(copyMenuItem);
            #endregion

            #region pasteMenuItem
            MenuItem pasteMenuItem = new MenuItem
            {
                Icon = new Image { Source = new BitmapImage(new Uri("/Themes/Default/Images/ContextMenu/Paste.png", UriKind.Relative)) },
                Header = MessageResource.ContextMenu_Paste,
                Cursor = Cursors.Hand
            };
            pasteMenuItem.Click += (s, e) =>
            {
                ExecutePasteCommand();
            };
            menuList.Add(pasteMenuItem);
            #endregion 

            #region allMenuItem
            MenuItem allMenuItem = new MenuItem { Header = MessageResource.ContextMenu_SelectAll, Cursor = Cursors.Hand };
            allMenuItem.Click += (s, e) =>
            {
                ExecuteSelectAllCommand();
            };
            menuList.Add(allMenuItem);
            #endregion

            this.ItemsSource = menuList;
            this.Closed += new RoutedEventHandler(NeweggContextMenu_Closed);
        }

        void NeweggContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if (!isReallyClose)
            {
                isReallyClose = true;
                this.IsOpen = true;
                new Thread(() =>
                {
                    Thread.Sleep(160);
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.IsOpen = false;
                    });
                }).Start();
            }
            else
            {
                isReallyClose = false;
            }
        }


        #endregion Consturct

        #region Command

        private void ExecuteCutCommand()
        {
            try
            {
                TextBox textBox = GetTextBox();
                if (textBox != null)
                {
                    bool isSelected = string.IsNullOrEmpty(textBox.SelectedText);
                    Clipboard.SetText(isSelected ? textBox.Text : textBox.SelectedText);
                    textBox.Text = isSelected ? string.Empty : textBox.Text.Replace(textBox.SelectedText, string.Empty);
                }
            }
            catch (Exception ex)
            {
                //Don't need to do nothing
            }
        }

        private void ExecuteCopyCommand()
        {
            try
            {
                TextBlock textBlock = Tag as TextBlock;
                if (Tag is TextBlock)
                    Clipboard.SetText((Tag as TextBlock).Text);
                else
                {
                    TextBox textBox = GetTextBox();
                    if (textBox != null)
                    {
                        Clipboard.SetText(string.IsNullOrEmpty(textBox.SelectedText) ? textBox.Text : textBox.SelectedText);
                        textBox.Focus();
                    }
                }
            }
            catch(Exception ex)
            {
                //Don't need to do nothing
            }
        }

        private void ExecutePasteCommand()
        {
            try
            {
                TextBox textBox = GetTextBox();
                if (textBox != null && Clipboard.ContainsText())
                {
                    string setValue = Clipboard.GetText();
                    int newCursorPosition = 0;
                    if (!string.IsNullOrEmpty(textBox.SelectedText))
                    {
                        int i = textBox.SelectionStart;
                        string preStr = textBox.Text.Substring(0, i);
                        string postStr = textBox.Text.Substring(i + textBox.SelectedText.Length);
                        newCursorPosition = (preStr + setValue).Length;
                        setValue = preStr + setValue + postStr;
                    }
                    else
                    {
                        newCursorPosition = textBox.SelectionStart + setValue.Length;
                        setValue = textBox.Text.Insert(textBox.SelectionStart, setValue);
                    }
                    if (textBox.MaxLength > 0)
                    {
                        textBox.Text = setValue.Substring(0, textBox.MaxLength >= setValue.Length ? setValue.Length : textBox.MaxLength);
                    }
                    else
                    {
                        textBox.Text = setValue;
                    }
                    textBox.SelectionStart = newCursorPosition;
                }
                textBox.Focus();
            }
            catch (Exception ex)
            {
                //Don't need to do nothing
            }
        }

        private void ExecuteSelectAllCommand()
        {
            TextBox textBox = GetTextBox();

            if (textBox != null)
            {
                this.Dispatcher.BeginInvoke(() => 
                {
                    textBox.Focus();
                    textBox.SelectAll();
                });
            }
        }

        #endregion Command

        #region Protected Method

        protected override void OnOpened(RoutedEventArgs e)
        {
            TextBlock textBlock = Tag as TextBlock;

            ObservableCollection<MenuItem> items = this.ItemsSource as ObservableCollection<MenuItem>;

            if (Tag is TextBlock)
            {
                MenuItemEnabled(items[0], false);
                MenuItemEnabled(items[1], string.IsNullOrEmpty((Tag as TextBlock).Text) == false);
                MenuItemEnabled(items[2], false);
                MenuItemEnabled(items[3], false);
            }
            else
            {
                TextBox textBox = GetTextBox();
                if (textBox != null)
                {
                    textBox.Focus();
                    bool isEnable = string.IsNullOrEmpty(textBox.Text) == false;
                    MenuItemEnabled(items[0], isEnable && textBox.IsReadOnly == false && "CanEdit == False".Equals(textBox.Tag) == false);
                    MenuItemEnabled(items[1], isEnable);
                    MenuItemEnabled(items[2], Clipboard.ContainsText() && textBox.IsReadOnly == false && "CanEdit == False".Equals(textBox.Tag) == false);
                    MenuItemEnabled(items[3], isEnable);
                }
                else
                {
                    MenuItemEnabled(items[0], false);
                    MenuItemEnabled(items[1], false);
                    MenuItemEnabled(items[2], false);
                    MenuItemEnabled(items[3], false);
                }
            }
        }

        #endregion Protected Method

        #region Private Method

        private TextBox GetTextBox()
        {
            TextBox textBox = null;
            DependencyObject obj = Tag as DependencyObject;
            while (obj != null && textBox == null)
            {
                obj = VisualTreeHelper.GetParent(obj);
                textBox = obj as TextBox;
            }

            return textBox;
        }

        private void MenuItemEnabled(MenuItem item, bool isEnabled)
        {
            item.IsEnabled = isEnabled;
            item.Opacity = isEnabled ? 1 : 0.2;
        }

        #endregion Private Method
    }
}