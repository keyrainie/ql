using System;
using System.Collections.Generic;
using System.IO;

namespace AutoSendMail.ThemeProviders
{
    public class ThemeProvider
    {
        private static readonly string themePath = "./Themes/";

        private static ThemeProvider instance;

        private static object synRoot = new object();

        private static Dictionary<string, Theme> themes = new Dictionary<string, Theme>();

        private ThemeProvider()
        {
        }

        public static ThemeProvider GetInstance()
        {
            if (instance == null)
            {
                lock (synRoot)
                {
                    if (instance == null)
                    {
                        instance = new ThemeProvider();
                    }
                }
            }

            return instance;
        }

        public Theme GetTheme(string themeName)
        {
            var relative = themePath + themeName + "/style.txt";
            var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relative);

            if (!themes.ContainsKey(themeName))
            {
                if (!File.Exists(absolutePath))
                {
                    return null;
                }

                using (StreamReader sr = new StreamReader(absolutePath))
                {
                    var style = sr.ReadToEnd();

                    var theme = new Theme
                    {
                        Style = style
                    };

                    themes.Add(themeName, theme);
                }
            }

            return themes[themeName];
        }
    }

    public class Theme
    {
        public string Style { get; set; }
    }
}