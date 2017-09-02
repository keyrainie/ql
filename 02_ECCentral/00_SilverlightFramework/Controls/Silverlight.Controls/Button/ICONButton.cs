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

namespace Newegg.Oversea.Silverlight.Controls
{
    public class ICONButton : Button
    {
        //Control Parts
        private Border m_Background_Active { get; set; }
        private Border m_Background_General { get; set; }
        private Border m_Background_Secondary { get; set; }
        private ContentControl m_Content_Active { get; set; }
        private ContentControl m_Content_General { get; set; }
        private ContentControl m_Content_Secondary { get; set; }

        private bool m_typeInited = true;

        public ICONButton()
        {
            this.DefaultStyleKey = typeof(ICONButton);
        }


        public static readonly DependencyProperty ICONProperty = DependencyProperty.Register("ICON", typeof(string), typeof(ICONButton), null);
        public string ICON
        {
            get { return (string)GetValue(ICONProperty); }
            set { SetValue(ICONProperty, value); }
        }

        public static readonly DependencyProperty InnerSpaceProperty = DependencyProperty.Register("InnerSpace", typeof(double), typeof(ICONButton), null);
        public double InnerSpace
        {
            get { return (double)GetValue(InnerSpaceProperty); }
            set { SetValue(InnerSpaceProperty, value); }
        }


        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(ICONButtonType), typeof(ICONButton), new PropertyMetadata(ICONButtonType.General, new PropertyChangedCallback(Type_PropertyChanged)));
        public ICONButtonType Type
        {
            get { return (ICONButtonType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        static void Type_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ICONButton button = ((ICONButton)sender);
            if (button.m_Background_Active != null
                && button.m_Background_General != null
                && button.m_Background_Secondary != null)
            {
                SetVisualByButtonType(button, (ICONButtonType)args.NewValue);
            }
            else
            {
                button.m_typeInited = false;
            }
        }

        static void SetVisualByButtonType(ICONButton button, ICONButtonType buttonType)
        {
            if (buttonType == ICONButtonType.Active)
            {
                button.m_Background_Active.Visibility = Visibility.Visible;
                button.m_Background_General.Visibility = Visibility.Collapsed;
                button.m_Background_Secondary.Visibility = Visibility.Collapsed;
            }
            else if (buttonType == ICONButtonType.Secondary)
            {
                button.m_Background_Active.Visibility = Visibility.Collapsed;
                button.m_Background_General.Visibility = Visibility.Collapsed;
                button.m_Background_Secondary.Visibility = Visibility.Visible;
            }
            else
            {
                button.m_Background_Active.Visibility = Visibility.Collapsed;
                button.m_Background_General.Visibility = Visibility.Visible;
                button.m_Background_Secondary.Visibility = Visibility.Collapsed;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.m_Background_Active == null)
            {
                this.m_Background_Active = (Border)this.GetTemplateChild("Background_Active");
            }
            if (this.m_Background_General == null)
            {
                this.m_Background_General = (Border)this.GetTemplateChild("Background_General");
            }
            if (this.m_Background_Secondary == null)
            {
                this.m_Background_Secondary = (Border)this.GetTemplateChild("Background_Secondary");
            }
            if (this.m_Content_Active == null)
            {
                this.m_Content_Active = (ContentControl)this.GetTemplateChild("Content_Active");
            }
            if (this.m_Content_General == null)
            {
                this.m_Content_General = (ContentControl)this.GetTemplateChild("Content_General");
            }
            if (this.m_Content_Secondary == null)
            {
                this.m_Content_Secondary = (ContentControl)this.GetTemplateChild("Content_Secondary");
            }
            if (!this.m_typeInited)
            {
                SetVisualByButtonType(this, this.Type);
            }
            if (this.ICON != null && this.ICON.Trim() != string.Empty)
            {
                this.m_Content_Active.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
                this.m_Content_General.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
                this.m_Content_Secondary.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
            }
        }

    }

    public enum ICONButtonType
    {
        General = 0,
        Active = 1,
        Secondary = 2
    }
}
