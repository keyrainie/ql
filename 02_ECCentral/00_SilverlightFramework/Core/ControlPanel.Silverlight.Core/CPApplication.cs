using System;
using System.Collections.Generic;
using System.Windows.Browser;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Windows.Controls.Primitives;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core
{
    public class CPApplication
    {
        #region Fields

        private static object s_synObj = new object();

        private static CPApplication m_application;

        #endregion

        private CPApplication()
        {

        }
        /// <summary>
        /// 获取当前应用程序的CPAppliaction实例
        /// </summary>
        public static CPApplication Current
        {
            get
            {
                if (m_application == null)
                {
                    lock (s_synObj)
                    {
                        if (m_application == null)
                        {
                            m_application = new CPApplication();
                        }
                    }
                }
                return m_application;
            }
        }

        #region Properties

        /// <summary>
        /// 获取当前登录用户的相关信息
        /// </summary>
        public AuthUser LoginUser { get; internal set; }

        /// <summary>
        /// 获取当前应用程序的相关信息
        /// </summary>
        public AuthApplication Application { get; internal set; }

        /// <summary>
        /// 获取Keystone配置内的相关的Application信息
        /// (注意：由于该框架提供一个SL的Application支持集成多个Keystone的系统权限配置，所以这里了List作为参数类型)
        /// </summary>
        public List<KS_Application> KS_Applications { get; internal set; }

        /// <summary>
        /// 获取当前登录系统的CompanyCode
        /// </summary>
        public string CompanyCode
        {
            internal set;
            get;
        }

        /// <summary>
        /// 获取当前登录系统的CompanyName
        /// </summary>
        public string CompanyName
        {
            internal set;
            get;
        }

        public ReadOnlyCollection<UICompany> CompanyList
        {
            internal set;
            get;
        }

        public ReadOnlyCollection<UIWebChannel> CurrentWebChannelList
        {
            internal set;
            get;
        }


        /// <summary>
        /// 获取当前登录系统的语言Code值
        /// </summary>
        public string LanguageCode { get; internal set; }


        /// <summary>
        /// 获取当前登录系统使用的主题Code
        /// </summary>
        public string ThemeCode { get; internal set; }

        /// <summary>
        /// 获取Server端的IP
        /// </summary>
        public string ServerIPAddress { get; internal set; }

        /// <summary>
        /// 获取Client端的IP
        /// </summary>
        public string ClientIPAddress { get; internal set; }
        /// <summary>
        /// 获取Client端的机器名
        /// </summary>
        public string ClientComputerName { get; internal set; }
        /// <summary>
        /// 获取Server端的计算机名
        /// </summary>
        public string ServerComputerName { get; internal set; }


        public string GlobalRegionName { get; set; }

        internal string LocalRegionName { get; set; }

        public IDictionary<string, string> InitParams 
        { 
            get; 
            internal set;
        }


        /// <summary>
        /// 获取网站的基地址
        /// </summary>
        public string PortalBaseAddress
        {

            get;
            set;
        }

        /// <summary>
        /// 获取当前登录系统的国家名称
        /// </summary>
        public string CountryCode { get { return "CHN"; } }

        /// <summary>
        /// 获取当前框架包的XAP文件名字
        /// </summary>
        public string FrameworkXapName { get; internal set; }

        public IPageBrowser Browser { get; internal set; }

        /// <summary>
        /// 获取当前选中的页面
        /// </summary>
        public IPage CurrentPage
        {
            get
            {
                if (Browser != null)
                {
                    return Browser.SelectedPage;
                }

                return null;
            }
        }

        /// <summary>
        /// 当前Dialog
        /// </summary>
        public IDialog CurrentDialog { get; set; }

        /// <summary>
        /// 获取PageBrowser的Home页面的Url
        /// </summary>
        public string DefaultPage { get; internal set; }


        public string ErrorPage
        {
            get
            {
                return "/PageBrowser/Error";
            }
        }

        /// <summary>
        /// 获取或设置基于application级别的自定义，存储在客户端浏览器内存中的键值对；
        /// </summary>
        public Dictionary<string, object> CommonData { get; set; }

        internal Popup RequestPanel { get; set; }

        public void CloseRequestPanel()
        {
            if (RequestPanel != null)
            {
                RequestPanel.IsOpen = false;
            }
        }

        internal StackPanel spFlashRequest;
        internal StackPanel spStopRequest;

        public void SetRequestIcon(bool isFlash)
        {
            if ((this.spStopRequest != null) && (this.spFlashRequest != null))
            {
                if (isFlash)
                {
                    this.spStopRequest.Visibility = System.Windows.Visibility.Collapsed;
                    this.spFlashRequest.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.spStopRequest.Visibility = System.Windows.Visibility.Visible;
                    this.spFlashRequest.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 销售渠道
    /// </summary>
    public class UIWebChannel
    {
        public string ChannelID { get; set; }

        public string ChannelName { get; set; }

        public UIWebChannelType ChannelType { get; set; }
    }

    /// <summary>
    /// 渠道类型
    /// </summary>
    public enum UIWebChannelType
    {
        /// <summary>
        /// NESOFT提供的电子商务网站
        /// </summary>
        Sales = 1,
        /// <summary>
        /// 外部第三方网站
        /// </summary>
        Publicity = 2
    }

    public class UICompany
    {
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }
    }

    /// <summary>
    /// 只读的表示键和值（字典）的集合。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">字典中的值的类型。</typeparam>
    internal class ReadOnlyDirectionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public static ReadOnlyDirectionary<TKey, TValue> Empty
        {
            get
            {
                return new ReadOnlyDirectionary<TKey, TValue>(0);
            }
        }
        /// <summary>
        /// 初始化只读字典类的新实例，该实例包装了一个普通的字典实例。
        /// </summary>
        /// <param name="innerDirectionary">要包装的普通字典实例。</param>
        /// <remarks>
        /// <para>只读字典不会从包装的普通字典中复制元素，而是直接暴露普通字典中具有只读特征的成员。
        /// 具有只写特征的成员将被隐藏，如果强制调用，则会抛出<see cref="NotSupportedException"/>异常。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <see cref="P:innerDirectionary"/>为null。
        /// </exception>
        public ReadOnlyDirectionary(IDictionary<TKey, TValue> innerDirectionary)
        {
            if (innerDirectionary == null)
            {
                throw new ArgumentNullException();
            }

            _dictionary = innerDirectionary;
        }

        private ReadOnlyDirectionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        #region IDictionary<TKey,TValue>
        /// <summary>
        /// 在字典中添加一个带有所提供的键和值的元素。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="value">要添加的元素的值。</param>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 确定字典中是否包含指定的键。
        /// </summary>
        /// <param name="key">要在字典中定位的键。</param>
        /// <returns>如果字典中包含具有指定键的元素，则为true；否则为false。</returns>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 获取包含字典中的键的集合。
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        /// <summary>
        /// 从字典中移除所指定的键的值。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        /// <returns>如果成功找到并移除该元素，则为 true；否则为 false。
        /// 如果在字典中没有找到 key，此方法则返回 false。</returns>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="value">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；
        /// 否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递。</param>
        /// <returns>如果字典包含具有指定键的元素，则为 true；否则为 false。</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取包含字典中的值的集合。
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <returns>与指定的键相关联的值。如果找不到指定的键，
        /// get操作便会引发<see cref="KeyNotFoundException"/>异常。</returns>
        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// 调用set操作总是会引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <returns>与指定的键相关联的值。如果找不到指定的键，
        /// get操作便会引发<see cref="KeyNotFoundException"/>异常。
        /// 调用set操作总是会引发<see cref="NotSupportedException"/>异常。</returns>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this[key]; }
            set { throw new NotSupportedException(); }
        }
        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>>
        /// <summary>
        /// 在字典中添加一个带有所提供的键和值的元素。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="item">要添加的元素的键和值。</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 从字典中移除所有项。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 确定集合中是否包含特定值。
        /// </summary>
        /// <param name="item">要在集合中定位的对象。</param>
        /// <returns>如果在集合中找到<see cref="P:item"/>，则为true；否则为false。</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        /// <summary>
        /// 从特定的数组索引开始，将集合中的元素复制到一个数组中。
        /// </summary>
        /// <param name="array">作为从集合复制的元素的目标位置的一维数组。
        /// 该数组必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><see cref="P:array"/>中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示集合是否为只读。
        /// 此实现总是返回true。
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 从集合中移除特定对象的第一个匹配项。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="item">要从集合中移除的对象。</param>
        /// <returns>如果已从集合中成功移除<see cref="P:item"/>，则为true；否则为false。
        /// 如果在原始集合中没有找到<see cref="P:item"/>，该方法也会返回 false。</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>
        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>可用于循环访问集合的枚举数。</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>可用于循环访问集合的枚举数。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }
        #endregion

        /// <summary>
        /// 用于保存被包装的普通字典实例。
        /// </summary>
        private IDictionary<TKey, TValue> _dictionary;
    }
}
