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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    public class LayoutMask
    {
        #region properties

        private readonly Grid m_Mask = new Grid();

        //在RootVisual级别Show Dialog的个数；
        public static int RootVisualContainerCount = 0;
        public static Panel OldMessageBoxParent = null;
        public static Panel MessageBoxPanel = null;

        public Grid MaskPanel
        {
            get
            {
                return m_Mask;
            }
        }

        protected IEnumerable<PopupBox> Boxes
        {
            get
            {
                return m_Mask.Children.OfType<PopupBox>();
            }
        }

        public bool IsRendered
        {
            get
            {
                return (MaskPanel.Parent != null);
            }
        }

        public int MaxZIndex
        {
            get
            {
                int zIndex = 0;
                if (m_Mask.Children.Count > 0)
                {
                    zIndex = m_Mask.Children.Select(element => Canvas.GetZIndex(element)).Max();
                }
                if (zIndex >= 30000)
                {
                    ReorderZIndex();
                }
                return zIndex;
            }
        }

        protected bool IsModal
        {
            get
            {
                if (MaskPanel.Children.Count == 0)
                {
                    return false;
                }

                return Boxes.Any(box => box.IsModal);
            }
        }

        private readonly Panel m_layOutRoot;

        protected Panel LayOutRoot
        {
            get
            {
                return m_layOutRoot;
            }
        }

        #endregion


        public LayoutMask(Panel owner)
        {
            m_layOutRoot = owner;
            m_Mask.Name = System.Guid.NewGuid().ToString("N");
        }

        #region method

        public void AddBox(PopupBox box)
        {
            var container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            if (this.LayOutRoot == container)
            {
                RootVisualContainerCount++;
            }

            if (!MaskPanel.Children.Contains(box))
            {
                MaskPanel.Children.Add(box);
            }

            CheckModal();

            if (IsRendered == false)
            {
                RenderMask();
            }

            PopupBox.PopupBoxes.Push(box);
        }

        public void RemoveBox(PopupBox box)
        {
            var container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            if (this.LayOutRoot == container)
            {
                RootVisualContainerCount--;
                if (RootVisualContainerCount == 0 && OldMessageBoxParent != null && MessageBoxPanel != null)
                {
                    if (container.Children.Contains(MessageBoxPanel))
                    {
                        container.Children.Remove(MessageBoxPanel);
                        OldMessageBoxParent.Children.Add(MessageBoxPanel);
                        MessageBoxPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }

            MaskPanel.Children.Remove(box);

            CheckModal();

            if (MaskPanel.Children.Count == 0)
            {
                LayOutRoot.Children.Remove(MaskPanel);
                box.LayoutMask = null;
            }

            if (PopupBox.PopupBoxes.Count > 0)
            {
                PopupBox.PopupBoxes.Pop();
            }
        }

        protected virtual void RenderMask()
        {
            CheckModal();
            LayOutRoot.UpdateLayout();
            MaskPanel.UpdateLayout();
            LayOutRoot.Children.Add(MaskPanel);
            Canvas.SetZIndex(MaskPanel, MaxZIndex +3000);
            //Canvas.SetZIndex(MaskPanel, MaxZIndex);
        }

        protected virtual void RemoveMask()
        {
            LayOutRoot.Children.Remove(MaskPanel);
        }

        protected virtual void CheckModal()
        {
            if (IsModal)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                MaskPanel.Background = brush;
            }
            else
            {
                MaskPanel.Background = null;
            }
        }

        protected virtual void ReorderZIndex()
        {
            IEnumerable<UIElement> elements =
                m_Mask.Children.OrderBy(element => Canvas.GetZIndex(element));
            int currentZIndex = 1;
            int zIndexOfLastElement = 0;
            foreach (UIElement element in elements)
            {
                if (zIndexOfLastElement != Canvas.GetZIndex(element))
                {
                    currentZIndex++;
                }
                Canvas.SetZIndex(element, currentZIndex);
            }
        }

        #endregion
    }
}
