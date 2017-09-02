using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class MiniButton : Button
    {
        private Image m_image;
        private bool m_isPinButton;


        public MiniButton()
        {
            this.DefaultStyleKey = typeof (MiniButton);
        }

        private static readonly DependencyProperty s_ImageProperty = DependencyProperty.Register("Image", typeof(string), typeof(MiniButton), null);
        public string Image
        {
            get { return (string) GetValue(s_ImageProperty); }
            set { SetValue(s_ImageProperty, value); }
        }

        private static readonly DependencyProperty s_PinnedImageProperty = DependencyProperty.Register("PinnedImage", typeof(string), typeof(MiniButton), null);
        public string PinnedImage
        {
            get { return (string)GetValue(s_PinnedImageProperty); }
            set { SetValue(s_PinnedImageProperty, value); }
        }

        private static readonly DependencyProperty s_isPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(MiniButton), new PropertyMetadata(true, OnIsPinnedPropertyChanged));
        public bool IsPinned
        {
            get { return (bool) GetValue(s_isPinnedProperty); }
            set { SetValue(s_isPinnedProperty, value); }
        }

        private static void OnIsPinnedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as MiniButton;

            RiseIsPinnedChanged(button);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_image = GetTemplateChild("MiniButton_Image") as Image;

            if (!PinnedImage.IsNullOrEmpty())
                m_isPinButton = true;

            RiseIsPinnedChanged(this);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (this.m_isPinButton)
            {
                this.IsPinned = !this.IsPinned;
            }
        }

        private static void RiseIsPinnedChanged(MiniButton button)
        {
            if (button.m_isPinButton)
            {
                VisualStateManager.GoToState(button, button.IsPinned ? "Pressed" : "Normal", true);

                if (button.IsPinned)
                {
                    button.m_image.Source = new BitmapImage(new Uri(button.PinnedImage, UriKind.Relative));
                }
                else
                {
                    button.m_image.Source = new BitmapImage(new Uri(button.Image, UriKind.Relative));
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (m_isPinButton) 
            {
                VisualStateManager.GoToState(this, IsPinned ? "Pressed" : "Normal", true);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (m_isPinButton)
            {
                VisualStateManager.GoToState(this, IsPinned ? "Pressed_MouseOver" : "MouseOver", true);
            }
        }

    }
}
