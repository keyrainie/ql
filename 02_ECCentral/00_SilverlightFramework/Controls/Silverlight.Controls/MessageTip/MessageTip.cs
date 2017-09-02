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
using Newegg.Oversea.Silverlight.Controls.Resources;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class MessageTip : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(MessageTip), null);
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(MessageTipType), typeof(MessageTip), new PropertyMetadata(MessageTipType.Help, new PropertyChangedCallback(Type_PropertyChanged)));
        public MessageTipType Type
        {
            get { return (MessageTipType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        static void Type_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MessageTip tip = (MessageTip)sender;
            SetVisualByType((MessageTipType)args.NewValue, tip);
        }

        private static void SetVisualByType(MessageTipType type, MessageTip tip)
        {
            if (tip.m_GridError != null)
            {
                tip.m_GridError.Visibility = Visibility.Collapsed;
            }
            if (tip.m_GridHelp != null)
            {
                tip.m_GridHelp.Visibility = Visibility.Collapsed;
            }
            if (tip.m_GridSucceed != null)
            {
                tip.m_GridSucceed.Visibility = Visibility.Collapsed;
            }
            if (type == MessageTipType.Error && tip.m_GridError != null)
            {
                tip.m_GridError.Visibility = Visibility.Visible;
            }
            else if (type == MessageTipType.Succeed && tip.m_GridSucceed != null)
            {
                tip.m_GridSucceed.Visibility = Visibility.Visible;
            }
            else if (type == MessageTipType.Help && tip.m_GridHelp != null)
            {
                tip.m_GridHelp.Visibility = Visibility.Visible;
            }
        }

        private Grid m_GridHelp;
        private Grid m_GridError;
        private Grid m_GridSucceed;

        private Grid m_GridToggleGreen;
        private Border m_BorderGreenContent;

        private Grid m_GridToggleRed;
        private Border m_BorderRedContent;

        private Grid m_GridToggleBlue;
        private Border m_BorderBlueContent;

        public MessageTipType MessageType { get; set; }


        public MessageTip()
        {
            DefaultStyleKey = typeof(MessageTip);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.m_GridError == null)
            {
                this.m_GridError = (Grid)this.GetTemplateChild("GridError");
            }
            if (this.m_GridHelp == null)
            {
                this.m_GridHelp = (Grid)this.GetTemplateChild("GridHelp");
            }
            if (this.m_GridSucceed == null)
            {
                this.m_GridSucceed = (Grid)this.GetTemplateChild("GridSucceed");
            }

            if (this.m_GridToggleGreen == null)
            {
                this.m_GridToggleGreen = (Grid)this.GetTemplateChild("GridToggleGreen");
                this.m_GridToggleGreen.MouseEnter += new MouseEventHandler(m_GridToggleGreen_MouseEnter);
                this.m_GridToggleGreen.MouseLeave += new MouseEventHandler(m_GridToggleGreen_MouseLeave);
                this.m_GridToggleGreen.MouseLeftButtonUp += new MouseButtonEventHandler(m_GridToggleGreen_MouseLeftButtonUp);
            }
            if (this.m_BorderGreenContent == null)
            {
                this.m_BorderGreenContent = (Border)this.GetTemplateChild("BorderGreenContent");
            }

            if (this.m_GridToggleRed == null)
            {
                this.m_GridToggleRed = (Grid)this.GetTemplateChild("GridToggleRed");
                this.m_GridToggleRed.MouseEnter += new MouseEventHandler(m_GridToggleGreen_MouseEnter);
                this.m_GridToggleRed.MouseLeave += new MouseEventHandler(m_GridToggleGreen_MouseLeave);
                this.m_GridToggleRed.MouseLeftButtonUp += new MouseButtonEventHandler(m_GridToggleGreen_MouseLeftButtonUp);
            }
            if (this.m_BorderRedContent == null)
            {
                this.m_BorderRedContent = (Border)this.GetTemplateChild("BorderRedContent");
            }

            if (this.m_GridToggleBlue == null)
            {
                this.m_GridToggleBlue = (Grid)this.GetTemplateChild("GridToggleBlue");
                this.m_GridToggleBlue.MouseEnter += new MouseEventHandler(m_GridToggleGreen_MouseEnter);
                this.m_GridToggleBlue.MouseLeave += new MouseEventHandler(m_GridToggleGreen_MouseLeave);
                this.m_GridToggleBlue.MouseLeftButtonUp += new MouseButtonEventHandler(m_GridToggleGreen_MouseLeftButtonUp);
            }
            if (this.m_BorderBlueContent == null)
            {
                this.m_BorderBlueContent = (Border)this.GetTemplateChild("BorderBlueContent");
            }

            SetVisualByType(this.Type, this);
        }

        void m_GridToggleGreen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.m_BorderGreenContent != null)
            {
                if (this.m_BorderGreenContent.Visibility == Visibility.Collapsed)
                {
                    VisualStateManager.GoToState(this, "ToggleExpanded", false);
                    this.m_BorderGreenContent.Visibility = Visibility.Visible;
                    this.m_BorderRedContent.Visibility = Visibility.Visible;
                    this.m_BorderBlueContent.Visibility = Visibility.Visible;
                }
                else
                {
                    VisualStateManager.GoToState(this, "ToggleCollapsed", false);
                    this.m_BorderGreenContent.Visibility = Visibility.Collapsed;
                    this.m_BorderRedContent.Visibility = Visibility.Collapsed;
                    this.m_BorderBlueContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        void m_GridToggleGreen_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToggleMouseOut", false);
        }

        void m_GridToggleGreen_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToggleMouseOver", false);
        }


    }

    public enum MessageTipType
    {
        Help,
        Succeed,
        Error
    }
}
