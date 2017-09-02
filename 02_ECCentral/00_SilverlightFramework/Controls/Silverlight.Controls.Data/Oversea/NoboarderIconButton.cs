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
using System.Windows.Media.Imaging;

namespace Newegg.Oversea.Silverlight.Controls.Data.Oversea
{
    public class NoboarderIconButton : Button
    {
        #region Fields
        private BitmapImage m_RAMImage = null;
        private Image m_ImageColor = null;
        private Image m_ImageGrayscale = null;
        #endregion

        #region Constructor
        public NoboarderIconButton()
        {
            DefaultStyleKey = typeof(NoboarderIconButton);
        }
        #endregion

        #region Dependency Property
        public string IconSource
        {
            get { return (string)GetValue(IconSourceProperty); }
            set
            {
                SetValue(IconSourceProperty, value);
                BitmapImage bi = new BitmapImage(new Uri(value, UriKind.Relative));
                bi.CreateOptions = BitmapCreateOptions.None;
                m_RAMImage = bi;
            }
        }

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource",
            typeof(string),
            typeof(NoboarderIconButton),
            new PropertyMetadata(null));
        #endregion

        #region Override Functions
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_ImageColor = ((System.Windows.Controls.Image)(this.GetTemplateChild("IconImageColor")));
            m_ImageGrayscale = ((System.Windows.Controls.Image)(this.GetTemplateChild("IconImageGrayscale")));

            if (m_ImageColor != null && m_RAMImage != null && m_ImageGrayscale != null)
            {
                m_ImageColor.Source = m_RAMImage;

                m_ImageGrayscale.Source = m_RAMImage;
                m_ImageGrayscale.ImageOpened += new EventHandler<RoutedEventArgs>(m_ImageGrayscale_ImageOpened);
            }
        }
        #endregion

        #region Event Handle
        void m_ImageGrayscale_ImageOpened(object sender, RoutedEventArgs e)
        {
            SetGrayscaleImage();
        }
        #endregion

        #region Private Functions
        private void SetGrayscaleImage()
        {
            if (m_RAMImage == null || m_ImageGrayscale == null)
                return;

            WriteableBitmap bitmap = new WriteableBitmap(m_ImageGrayscale, null);

            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    int pixelLocation = bitmap.PixelWidth * y + x;
                    int pixel = bitmap.Pixels[pixelLocation];
                    byte[] pixBytes = BitConverter.GetBytes(pixel);

                    byte bnwPixel = (byte)(0.3 * pixBytes[2] + 0.59 * pixBytes[1] + 0.11 * pixBytes[0]);
                    pixBytes[0] = bnwPixel; // B
                    pixBytes[1] = bnwPixel; // G     
                    pixBytes[2] = bnwPixel; // R        

                    bitmap.Pixels[pixelLocation] = BitConverter.ToInt32(pixBytes, 0);
                }
            }

            m_ImageGrayscale.Source = bitmap;
        }
        #endregion
    }
}