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
using System.Windows.Interactivity;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace ECCentral.Portal.Basic.Behaviors
{
    /// <summary>
    /// Panel容器下所有的TextBox控件回车执行指定按钮的单击事件
    /// </summary>
    public class EnterSearchBehavior : Behavior<Panel>
    {        
        /// <summary>
        /// 要触发click事件的按钮名字
        /// </summary>
        public string ButtonName { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += OnPanelKeyUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= OnPanelKeyUp;
        }

        private void OnPanelKeyUp(object sender, KeyEventArgs e)
        {
            var txtSource = e.OriginalSource as TextBox;
            if ((e.Key == Key.Enter) && (txtSource !=null))
            {
                Button button = AssociatedObject.FindName(this.ButtonName) as Button;
                if (button != null)
                {
                    //调用Button之前，先更新ViewModel
                    var b = txtSource.GetBindingExpression(TextBox.TextProperty);
                    if (b != null)
                    {
                        b.UpdateSource();
                    }

                    ButtonAutomationPeer buttonPeer = new ButtonAutomationPeer(button);
                    IInvokeProvider invokeProvider = buttonPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    
                    invokeProvider.Invoke();
                }
            }
        }
    }
}
