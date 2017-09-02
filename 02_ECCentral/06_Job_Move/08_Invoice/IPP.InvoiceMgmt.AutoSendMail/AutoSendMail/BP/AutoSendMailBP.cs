using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using AutoSendMail.DAL;
using AutoSendMail.ThemeProviders;
using System;

namespace AutoSendMail.BP
{
    internal static class AutoSendMailBP
    {
        public static void SendMail(RootMailConfiguration item)
        {
            var queryResult = AutoSendMailDA.ExecuteResult(item.QueryCommand);

            var style = GetStyle(item.Template.Theme);
            var subject = BuildMailHeaderOrFooter(item.Template.Subject, queryResult.OutputParams);
            var mailHeader = BuildMailHeaderOrFooter(item.Template.Header, queryResult.OutputParams);
            var mailBody = BuildMailBody(item.Template.Body, queryResult.ResultTable);
            var mailFooter = BuildMailHeaderOrFooter(item.Template.Footer, queryResult.OutputParams);

            if (mailBody.Length == 0)
            {
                return;
            }

            if (!string.IsNullOrEmpty(mailHeader))
            {
                mailBody = mailBody.Insert(0, mailHeader);
            }

            if (!string.IsNullOrEmpty(style))
            {
                mailBody.Insert(0, style);
            }

            if (!string.IsNullOrEmpty(mailFooter))
            {
                mailBody.Append(mailFooter);
            }

            AutoSendMailDA.SendEmail(item.Address, subject, mailBody.ToString(), item.CompanyCode, item.StoreCompanyCode, item.LanguageCode);

            if (!string.IsNullOrEmpty(item.UpdateCommandName))
            {
                AutoSendMailDA.ExecuteNonQuery(item.UpdateCommandName);
            }
        }

        private static string BuildMailHeaderOrFooter(string template, Dictionary<string, object> param)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var templateBuilder = new StringBuilder(template);

            Dictionary<string, ReplacementInfo> fieldMapping = GetFieldMapping(template);

            foreach (var pair in fieldMapping)
            {
                var value = param[pair.Key];

                AppendFormattedString(templateBuilder, value, pair);
            }

            return templateBuilder.ToString();
        }

        private static void AppendFormattedString(StringBuilder template, object value, KeyValuePair<string, ReplacementInfo> pair)
        {
            if (value is DateTime)
            {
                DateTime dtValue = (DateTime)value;

                template = template.Replace(pair.Value.OriginalString, dtValue.ToString(pair.Value.FormatString));
            }
            else if (value is int)
            {
                int intValue = (int)value;

                template = template.Replace(pair.Value.OriginalString, intValue.ToString(pair.Value.FormatString));
            }
            else if (value is decimal)
            {
                decimal decimalValue = (decimal)value;

                template = template.Replace(pair.Value.OriginalString, decimalValue.ToString(pair.Value.FormatString));
            }
            else if (value is double)
            {
                double doubleValue = (double)value;

                template = template.Replace(pair.Value.OriginalString, doubleValue.ToString(pair.Value.FormatString));
            }
            else if (value is float)
            {
                float floatValue = (float)value;

                template = template.Replace(pair.Value.OriginalString, floatValue.ToString(pair.Value.FormatString));
            }
            else
            {
                template = template.Replace(pair.Value.OriginalString, value.ToString());
            }
        }

        private static StringBuilder BuildMailBody(string bodyTemplate, DataTable resultTable)
        {
            StringBuilder body = new StringBuilder();

            Dictionary<string, ReplacementInfo> fieldMapping = GetFieldMapping(bodyTemplate);

            foreach (DataRow row in resultTable.Rows)
            {
                StringBuilder bodyFragment = new StringBuilder(bodyTemplate);

                foreach (var pair in fieldMapping)
                {
                    var value = row[pair.Key];

                    AppendFormattedString(bodyFragment, value, pair);
                }

                body.AppendLine(bodyFragment.ToString());
            }

            return body;
        }

        private static string GetStyle(string themeName)
        {
            var theme = ThemeProvider.GetInstance().GetTheme(themeName);

            if (theme == null)
            {
                return string.Empty;
            }

            return theme.Style;
        }

        private static Dictionary<string, ReplacementInfo> GetFieldMapping(string templateString)
        {
            Dictionary<string, ReplacementInfo> fieldMapping = new Dictionary<string, ReplacementInfo>();

            Match match = Regex.Match(templateString, @"{(?<columns>\w+):(?<format>.+)}|{(?<columns>\w+)}", RegexOptions.Compiled);
            while (match.Success)
            {
                var key = match.Groups["columns"].Value;
                var replacementInfo = new ReplacementInfo
                {
                    OriginalString = match.Value,
                    FormatString = match.Groups["format"].Value
                };

                if (!fieldMapping.ContainsKey(key))
                {
                    fieldMapping.Add(key, replacementInfo);
                }

                match = match.NextMatch();
            }

            return fieldMapping;
        }
    }
}
