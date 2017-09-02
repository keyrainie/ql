using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity
{
    /// <summary>
    /// 多语言内容
    /// </summary>
    [Serializable]
    [DataContract]
    public class LanguageContent : ILanguage
    {
        /// <summary>
        /// 构造函数，可默认添加一条语言对应的内容
        /// </summary>
        /// <param name="languageCode">语言编码</param>
        /// <param name="content">内容</param>
        public LanguageContent(string languageCode, string content)
        {
            _languageCode = languageCode;
            _content = content;
        }

        /// <summary>
        /// 构造函数，可默认添加一条语言对应的内容
        /// </summary>
        /// <param name="content">内容</param>
        public LanguageContent(string content) 
        {
            _content = content;
        }

        public LanguageContent() { }

        private string _languageCode = "zh-CN";
        [DataMember]
        public string LanguageCode
        {
            get
            {
                return _languageCode;
            }
            set
            {
                _languageCode = value;
            }
        }

        private string _content = string.Empty;
        [DataMember]
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// 默认返回语言对应的内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _content;
        }

        ///// <summary>
        ///// 所有的语言数据列表
        ///// </summary>
        //public List<LanguageString> ContentList = new List<LanguageString>();

        //public LanguageContent()
        //{

        //}

        ///// <summary>
        ///// 构造函数，可默认添加一条语言对应的内容
        ///// </summary>
        ///// <param name="languageCode">语言编码</param>
        ///// <param name="content">内容</param>
        //public LanguageContent(string content)
        //    : this(Thread.CurrentThread.CurrentCulture.Name, content)
        //{
        //    //为Fix Portal端Error方便，临时加上。
        //}

        ///// <summary>
        ///// 构造函数，可默认添加一条语言对应的内容
        ///// </summary>
        ///// <param name="languageCode">语言编码</param>
        ///// <param name="content">内容</param>
        //public LanguageContent(string languageCode, string content)
        //{
        //    Set(languageCode, content);
        //}

        ///// <summary>
        ///// 设置一条语言对应的内容；如果已经有该语言的内容，则将Update该内容
        ///// </summary>
        ///// <param name="languageCode">语言编码</param>
        ///// <param name="content">内容</param>
        //public void Set(string languageCode, string content)
        //{
        //    languageCode = languageCode.ToUpper();
        //    foreach (LanguageString ls in ContentList)
        //    {
        //        if (ls.LanguageCode.ToUpper() == languageCode)
        //        {
        //            ls.Text = content;
        //            return;
        //        }
        //    }
        //    ContentList.Add(new LanguageString(languageCode, content));
        //}



        ///// <summary>
        ///// 所有包含的语言编码列表
        ///// </summary>
        //public List<string> GetLanguageList()
        //{
        //    List<string> list = new List<string>(ContentList.Count);
        //    ContentList.ForEach(f => list.Add(f.LanguageCode));
        //    return list;
        //}

        ///// <summary>
        ///// 判断是否包含有指定语言编码的文本信息
        ///// </summary>
        ///// <param name="languageCode">指定的语言编码</param>
        ///// <returns>是否包含</returns>
        //public bool CheckLanguageExists(string languageCode)
        //{
        //    languageCode = languageCode.ToUpper();
        //    foreach (LanguageString ls in ContentList)
        //    {
        //        if (ls.LanguageCode.ToUpper() == languageCode)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 按照语言编码返回对应的内容
        ///// </summary>
        ///// <param name="languageCode">语言编码</param>
        ///// <returns>内容</returns>
        //public string GetText(string languageCode)
        //{
        //    languageCode = languageCode.ToUpper();
        //    foreach (var item in ContentList)
        //    {
        //        if (item.LanguageCode.ToLower() == languageCode)
        //        {
        //            return item.Text;
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 默认返回第一种语言对应的内容
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    if (ContentList.Count == 0)
        //    {
        //        return null;
        //    }
        //    return ContentList[0].Text;
        //}

        ///// <summary>
        ///// 判断当前多语言内容对象是否已经赋值，如果已赋值，则返回false
        ///// </summary>
        //public bool IsEmpty
        //{
        //    get
        //    {
        //        return ContentList.Count == 0;
        //    }
        //}

        //public static implicit operator string(LanguageContent content)
        //{
        //    return content.ToString(); 
        //}

        //public static implicit operator LanguageContent(string str)
        //{
        //    return new LanguageContent("zh-CN",str);
        //}
    }

    ///// <summary>
    ///// 基于语言接口的数据内容
    ///// </summary>
    //public class LanguageString : ILanguage
    //{
    //    public LanguageString() { }

    //    public LanguageString(string languageCode, string text)
    //    {
    //        _languageCode = languageCode;
    //        _text = text;
    //    }

    //    private string _languageCode = "zh-CN";
    //    public string LanguageCode
    //    {
    //        get
    //        {
    //            return _languageCode;
    //        }
    //        set
    //        {
    //            _languageCode = value;
    //        }
    //    }

    //    private string _text = string.Empty;
    //    public string Text
    //    {
    //        get
    //        {
    //            return _text;
    //        }
    //        set
    //        {
    //            _text = value;
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return _text;
    //    }

    //}



}
