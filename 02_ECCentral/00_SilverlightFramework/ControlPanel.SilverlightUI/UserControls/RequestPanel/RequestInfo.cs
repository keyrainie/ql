namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.RequestPanel
{
    public class RequestInfo
    {
        /// <summary>
        /// 设置或获取在Request中Tab显示的标题
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 设置或获取Xap包的Url地址(相对路径)
        /// </summary>
        public string XapUrl { get; set; }

        /// <summary>
        /// 设置或获取需要加载的程序集
        /// </summary>
        public string AssemblyName { get; set; }
        
        /// <summary>
        /// 设置或获取需要加载的用户控件的类名（命名控件+类名）
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 设置或获取授权Key
        /// </summary>
        public string AuthKey { get; set; }
    }
}
