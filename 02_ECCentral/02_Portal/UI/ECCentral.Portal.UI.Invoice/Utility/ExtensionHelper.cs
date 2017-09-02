using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Utility
{
    public static class ExtensionHelper
    {
        /// <summary>
        /// 判断集合List<T>中是否存在满足条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">需要检查的集合</param>
        /// <param name="match">需要满足的条件</param>
        /// <returns>存在则为true，否则为false</returns>
        public static bool Exists<T>(this List<T> collection, Predicate<T> match)
        {
            if (collection == null || collection.Count == 0)
            {
                return false;
            }
            bool bExist = false;
            foreach (var item in collection)
            {
                if (match(item))
                {
                    bExist = true;
                    break;
                }
            }
            return bExist;
        }

        /// <summary>
        /// 将集合List<T>中的每个元素通过转换器转换成另外一种类型，然后返回转换成的新类型的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="collection"></param>
        /// <param name="converter">类型转换器</param>
        /// <returns></returns>
        public static List<TOutput> ConvertAll<T, TOutput>(this List<T> collection, Converter<T, TOutput> converter)
        {
            if (collection != null)
            {
                return collection.Select(s => converter(s)).ToList();
            }
            return null;
        }

        /// <summary>
        /// 如果集合为NULL，则返回一个元素个数为0的空集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<TSource> DefaultIfNull<TSource>(this List<TSource> collection)
        {
            if (collection == null)
            {
                collection = new List<TSource>();
            }
            return collection;
        }

        /// <summary>
        /// 如果字符串是NULL，则返回一个Empty的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EmptyIfNull(this System.String str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str;
        }

        /// <summary>
        /// 剪切字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex">剪切点位置</param>
        /// <param name="length">剪切长度</param>
        /// <returns>剪切后的字符串</returns>
        public static string Cut(this System.String str, int startIndex, int length)
        {
            str = str.EmptyIfNull();
            if (str.Length <= startIndex + 1)
            {
                return str;
            }
            if (str.Length <= startIndex + length)
            {
                return str.Substring(startIndex);
            }
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 弹出确认对话框
        /// </summary>
        /// <param name="window"></param>
        /// <param name="content">对话框内容</param>
        /// <param name="OKHandler">点击确定按钮的回调</param>
        public static void Confirm(this Newegg.Oversea.Silverlight.Controls.IWindow window, string content, Action OKHandler)
        {
            window.Confirm(ResCommon.Message_ConfirmDlgDefaultTitle, content, _ => OKHandler(), null);
        }

        /// <summary>
        /// 弹出确认对话框
        /// </summary>
        /// <param name="window"></param>
        /// <param name="content">对话框内容</param>
        /// <param name="OKHandler">点击确定按钮的回调</param>
        public static void Confirm(this Newegg.Oversea.Silverlight.Controls.IWindow window, string content, Action<object> OKHandler)
        {
            window.Confirm(ResCommon.Message_ConfirmDlgDefaultTitle, content, OKHandler, null);
        }

        /// <summary>
        /// 弹出确认对话框
        /// </summary>
        /// <param name="window"></param>
        /// <param name="content">对话框内容</param>
        /// <param name="OKHandler">点击确定按钮的回调</param>
        /// <param name="CancelHandler">点击取消按钮的回调</param>
        public static void Confirm(this Newegg.Oversea.Silverlight.Controls.IWindow window, string content, Action<object> OKHandler, Action<object> CancelHandler)
        {
            window.Confirm(ResCommon.Message_ConfirmDlgDefaultTitle, content, OKHandler, CancelHandler);
        }

        /// <summary>
        /// 弹出确认对话框
        /// </summary>
        /// <param name="window"></param>
        /// <param name="title">弹出窗标题</param>
        /// <param name="content">对话框内容</param>
        /// <param name="OKHandler">点击确定按钮的回调</param>
        /// <param name="CancelHandler">点击取消按钮的回调</param>
        public static void Confirm(this Newegg.Oversea.Silverlight.Controls.IWindow window, string title, string content, Action<object> OKHandler, Action<object> CancelHandler)
        {
            window.Confirm(title, content, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    var OKhandler = OKHandler;
                    if (OKhandler != null)
                    {
                        OKhandler(args.Data);
                    }
                }
                else
                {
                    var cancelHandler = CancelHandler;
                    if (cancelHandler != null)
                    {
                        cancelHandler(args.Data);
                    }
                }
            });
        }

        /// <summary>
        /// 弹出一般的消息对话框
        /// </summary>
        /// <param name="window"></param>
        /// <param name="content">对话框内容</param>
        /// <param name="callback">关闭对话框时的回调</param>
        public static void Alert(this Newegg.Oversea.Silverlight.Controls.IWindow window, string content, Action callback)
        {
            window.Alert(ResCommon.Message_AlterDlgDefaultTitle, content,
                Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
            {
                callback();
            });
        }

        /// <summary>
        /// 设置容器下所有的子控件的可用性
        /// </summary>
        /// <param name="container">容器控件</param>
        /// <param name="isAvailably">是否可用，true为可用，否则为false</param>
        public static System.Windows.Controls.Panel SetChildControlAvailably(this System.Windows.Controls.Panel container, bool isAvailably)
        {
            return container.SetChildControlAvailably(isAvailably, null);
        }

        /// <summary>
        /// 设置容器下除排除列表外的所有的子控件的可用性，排除列表中的子控件可用性与之相反
        /// </summary>
        /// <param name="container">容器控件</param>
        /// <param name="isAvailably">是否可用，true为可用，否则为false</param>
        /// <param name="excepts">需要排除的控件列表</param>
        public static System.Windows.Controls.Panel SetChildControlAvailably(this System.Windows.Controls.Panel container, bool isAvailably, IList<System.Windows.UIElement> excepts)
        {
            if (container != null)
            {
                if (excepts == null)
                {
                    excepts = new List<System.Windows.UIElement>();
                }

                var iterator = container.Children.GetEnumerator();
                while (iterator.MoveNext())
                {
                    var element = iterator.Current;
                    if (!excepts.Contains(element))
                    {
                        SetControlAvailably(element, isAvailably);
                    }
                    else
                    {
                        SetControlAvailably(element, !isAvailably);
                    }
                }
            }
            return container;
        }

        private static void SetControlAvailably(System.Windows.UIElement element, bool isAvailably)
        {
            if (element is System.Windows.Controls.TextBox)
            {
                ((System.Windows.Controls.TextBox)element).IsReadOnly = !isAvailably;
            }
            else if (element is System.Windows.Controls.RichTextBox)
            {
                ((System.Windows.Controls.RichTextBox)element).IsReadOnly = !isAvailably;
            }
            else if (element is System.Windows.Controls.Control)
            {
                ((System.Windows.Controls.Control)element).IsEnabled = isAvailably;
            }
        }
    }
}