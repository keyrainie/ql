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
using System.Collections.Generic;
using System.Collections;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public static class Extensions
    {
        public static string GetColumnName(this DataGridColumn column)
        {
            string name = string.Empty;
            if (column is DataGridTextColumn)
            {
                name = (column as DataGridTextColumn).Name;
            }
            else if (column is DataGridTemplateColumn)
            {
                name = (column as DataGridTemplateColumn).Name;
            }

            return name;
        }

        public static string GetBindingPath(this DataGridColumn column)
        {
            var name = string.Empty;
            if (column is DataGridTextColumn)
            {
                DataGridTextColumn boundColumn = (DataGridTextColumn)column;
                name = boundColumn.Name;
                if (string.IsNullOrEmpty(name))
                {
                    var binding = boundColumn.Binding;
                    if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                    {
                        name = binding.Path.Path;
                    }
                    else if (binding != null && binding.ConverterParameter != null)
                    {
                        name = binding.ConverterParameter.ToString();
                    }
                }
            }
            return name;
        }

        public static int GetCount(this IEnumerable collection)
        {
            if (collection == null)
            {
                return 0;
            }

            var num = 0;

            var enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                num++;
            }

            return num;
        }
    }
}
