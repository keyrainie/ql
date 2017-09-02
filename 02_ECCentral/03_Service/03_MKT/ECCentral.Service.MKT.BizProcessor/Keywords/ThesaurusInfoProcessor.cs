using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using System.Text.RegularExpressions;


namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ThesaurusInfoProcessor))]
    public class ThesaurusInfoProcessor
    {
        private IThesaurusInfoDA keywordDA = ObjectFactory<IThesaurusInfoDA>.Instance;

        #region 同义词（ThesaurusInfo）
        /// <summary>
        ///添加同义词
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddThesaurusWords(ThesaurusInfo item)
        {
            if (keywordDA.CheckThesaurusWordsForInsert(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheThesaurusWords"));
            string message = CheckThesaurusKeywords(item.ThesaurusWords.Content,item.Type.Value);

            if(string.IsNullOrEmpty(message))
                    keywordDA.AddThesaurusWords(item);
            else
                throw new BizException(message);
        }

        /// <summary>
        ///根据编号列表批量更新同义词
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchUpdateThesaurusWords(List<ThesaurusInfo> items)
        {
            StringBuilder ex = new StringBuilder();
            foreach (ThesaurusInfo item in items)
            {
                if (keywordDA.CheckThesaurusWordsForUpdate(item))
                    ex.AppendLine(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheThesaurusWordsContent") + item.ThesaurusWords.Content);

                string message = CheckThesaurusKeywords(item.ThesaurusWords.Content,item.Type.Value);
                if (string.IsNullOrEmpty(message))
                    keywordDA.UpdateThesaurusInfo(item);
                else
                    //ex.AppendLine("编号:" + item.SysNo.Value + " 更新失败! 错误原因:"+ message);
                    ex.AppendLine(string.Format(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_UpdateError"), item.SysNo.Value, message));
            }
            if (!string.IsNullOrEmpty(ex.ToString()))
            {
                ex.AppendLine(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_OtherKeywordsUpdateSuccessful"));
                throw new BizException(ex.ToString());
            }
        }

        /// <summary>
        ///根据编号编辑同义词
        /// </summary>
        /// <param name="item"></param>
        //public virtual void EditThesaurusWords(ThesaurusInfo item)
        //{
        //    keywordDA.EditThesaurusWords(item);
        //}


        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <returns></returns>
        public virtual ThesaurusInfo LoadThesaurusWords(int sysNo)
        {
            return keywordDA.LoadThesaurusWords(sysNo);
        }

        /// <summary>
        /// 检查同义的格式
        /// </summary>
        /// <param name="WordContent"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        private string CheckThesaurusKeywords(string WordContent, ThesaurusWordsType Type)
        {
            string Errormsg = string.Empty;
            if (WordContent == null || string.IsNullOrEmpty(WordContent))
                //return "同义词不能为空";
                return ResouceManager.GetMessageString("MKT.Keywords", "Keywords_SynonymNotNull");

            if (WordContent.Length > 300)
                //return "同义词的长度不能超过300个字符";
                return ResouceManager.GetMessageString("MKT.Keywords", "Keywords_SynonymLengthError");

            switch (Type)
            {
                case ThesaurusWordsType.Monodirectional:
                    Errormsg = CheckedOldKeyword(WordContent);
                    break;
                case ThesaurusWordsType.Doubleaction:
                    Errormsg = CheckedDoubleKeyword(WordContent);
                    break;
            }
            return Errormsg;
        }

        /// <summary>
        /// 检查单向词的格式
        /// </summary>
        /// <param name="WordContent"></param>
        /// <returns></returns>
        private string CheckedOldKeyword(string WordContent)
        {
            //string OldErrorMsg = "格式错误,单向词：使用 '>' 来分割原词和转向词。例如 ：7900>Geforce7900";
            string OldErrorMsg = ResouceManager.GetMessageString("MKT.Keywords", "Keywords_FormatError");
            string Errormsg = string.Empty;
            Regex Odd = new Regex(@">");

            if (Odd.IsMatch(WordContent))
            {
                int index = WordContent.IndexOf('>');

                if ((index == WordContent.Length - 1) || (index == 0))
                    return OldErrorMsg;
            }
            else
                Errormsg = OldErrorMsg;

            return Errormsg;
        }

        /// <summary>
        /// 检查双向词的格式
        /// </summary>
        /// <param name="WordContent"></param>
        /// <returns></returns>
        private string CheckedDoubleKeyword(string WordContent)
        {
            string Errormsg = string.Empty;
            //string DoubleErrorMsg = "格式错误,双向词：使用 ':'来分割双向词中的多个单词 。例如6800:6800GS";
            string DoubleErrorMsg = ResouceManager.GetMessageString("MKT.Keywords", "Keywords_FormatError2");
            Regex Double = new Regex(@":");
            if (Double.IsMatch(WordContent))
            {
                int index = WordContent.IndexOf(':');

                if ((index == WordContent.Length - 1) || (index == 0))
                    return DoubleErrorMsg;
            }
            else
                Errormsg = DoubleErrorMsg;
            return Errormsg;
        }
        #endregion
    }
}
