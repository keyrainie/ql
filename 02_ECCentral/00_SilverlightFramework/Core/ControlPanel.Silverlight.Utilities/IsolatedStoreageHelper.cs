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
using System.IO.IsolatedStorage;
using System.IO;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Text;

namespace Newegg.Oversea.Silverlight.Utilities
{
    /// <summary>
    /// IsolatedStoreage for persist cache data
    /// </summary>
    public static class IsolatedStoreageHelper
    {
        public static object m_synch = new object();

        /// <summary>
        /// Reads the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static object Read(string fileName)
        {
            System.Diagnostics.Debug.WriteLine("IsolatedStoreageHelper Read " + fileName);
            object result = null;

            if (IsolatedStorageFile.IsEnabled)
            {
                lock (m_synch)
                {
                    IsolatedStorageFile isoStore = null;
                    try
                    {
                        isoStore = IsolatedStorageFile.GetUserStoreForApplication();

                        if (isoStore.FileExists(fileName))
                        {
                            byte[] buffer = null;
                            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, isoStore))
                            {
                                buffer = new byte[isoStream.Length];
                                if (isoStream.CanRead)
                                {
                                    isoStream.Read(buffer, 0, buffer.Length);
                                }
                                isoStream.Close();
                            }

                            if (buffer != null)
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    var decompressedBuffer = UtilityHelper.DeflateUnCompress(buffer);

                                    stream.Write(decompressedBuffer, 0, decompressedBuffer.Length);
                                    stream.Position = 0;

                                    result = UtilityHelper.BinaryDeserialize(stream);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string[] fileNames = IsolatedStorageFile.GetUserStoreForApplication().GetFileNames();

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("IsolatedStoreageHelper Read {0}", fileName));
                            sb.AppendLine(string.Format("Used Size: {0}kb", Math.Round(Convert.ToDouble(IsolatedStorageFile.GetUserStoreForApplication().UsedSize) / 1024d, 2)));
                            if (fileNames != null)
                            {
                                foreach (var filePath in fileNames)
                                {
                                    IsolatedStorageFileStream isoStream = null;
                                    try
                                    {
                                        isoStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(filePath, FileMode.Open, FileAccess.ReadWrite);
                                        sb.AppendLine(string.Format("{0}: {1}kb", filePath, Math.Round(Convert.ToDouble(isoStream.Length) / 1024d, 2)));
                                    }
                                    finally
                                    {
                                        if (isoStream != null)
                                        {
                                            isoStream.Dispose();
                                        }
                                    }

                                }
                            }
                            ComponentFactory.Logger.LogError(ex, sb.ToString(), null);
                        }
                        catch (Exception ex1)
                        {
                            ComponentFactory.Logger.LogError(ex1, "Delete IsolatedStorage File error on Read.", null);
                        }
                    }
                    finally
                    {
                        if (isoStore != null)
                        {
                            isoStore.Dispose();
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Writes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data.</param>
        public static void Write(string fileName, object data)
        {
            System.Diagnostics.Debug.WriteLine("IsolatedStoreageHelper Write " + fileName);

            if (IsolatedStorageFile.IsEnabled)
            {
                lock (m_synch)
                {
                    IsolatedStorageFile isoStore = null;
                    try
                    {

                        using (MemoryStream stream = new MemoryStream())
                        {
                            UtilityHelper.BinarySerialize(data, stream);

                            stream.Position = 0;

                            isoStore = IsolatedStorageFile.GetUserStoreForApplication();


                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            var compressedBuffer = UtilityHelper.DeflateCompress(buffer);

                            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, isoStore))
                            {
                                if (isoStream.CanWrite)
                                {
                                    isoStream.Write(compressedBuffer, 0, compressedBuffer.Length);
                                }
                                isoStream.Close();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string[] fileNames = IsolatedStorageFile.GetUserStoreForApplication().GetFileNames();
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("IsolatedStoreageHelper Write {0}", fileName));
                            sb.AppendLine(string.Format("Used Size: {0}kb", Math.Round(Convert.ToDouble(IsolatedStorageFile.GetUserStoreForApplication().UsedSize) / 1024d, 2)));
                            sb.AppendLine("For Application:");
                            if (fileNames != null)
                            {
                                foreach (var filePath in fileNames)
                                {
                                    IsolatedStorageFileStream isoStream = null;
                                    try
                                    {
                                        isoStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(filePath, FileMode.Open, FileAccess.ReadWrite);
                                        sb.AppendLine(string.Format("{0}: {1}kb", filePath, Math.Round(Convert.ToDouble(isoStream.Length) / 1024d, 2)));
                                    }
                                    finally
                                    {
                                        if (isoStream != null)
                                        {
                                            isoStream.Dispose();
                                        }
                                    }

                                }
                            }

                            fileNames = IsolatedStorageFile.GetUserStoreForSite().GetFileNames();
                            sb.AppendLine("For Site:");
                            if (fileNames != null)
                            {
                                foreach (var filePath in fileNames)
                                {
                                    IsolatedStorageFileStream isoStream = null;
                                    try
                                    {
                                        isoStream = IsolatedStorageFile.GetUserStoreForSite().OpenFile(filePath, FileMode.Open, FileAccess.ReadWrite);
                                        sb.AppendLine(string.Format("{0}: {1}kb", filePath, Math.Round(Convert.ToDouble(isoStream.Length) / 1024d, 2)));
                                    }
                                    finally
                                    {
                                        if (isoStream != null)
                                        {
                                            isoStream.Dispose();
                                        }
                                    }

                                }
                            }

                            ComponentFactory.Logger.LogError(ex, sb.ToString(), null);

                            if (ex is IsolatedStorageException && isoStore != null && ex.StackTrace.Contains("IsolatedStorageFile.Reserve"))
                            {
                                isoStore.Remove();
                            }
                            isoStore = IsolatedStorageFile.GetUserStoreForSite();
                            if (ex is IsolatedStorageException && isoStore != null && ex.StackTrace.Contains("IsolatedStorageFile.Reserve"))
                            {
                                isoStore.Remove();
                            }
                        }
                        catch (Exception ex1)
                        {
                            ComponentFactory.Logger.LogError(ex1, "IsolatedStoreageHelper Write failed.", null);
                        }
                    }
                    finally
                    {
                        if (isoStore != null)
                        {
                            isoStore.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inits the quota.
        /// </summary>
        /// <param name="size">The size(MB).</param>
        public static void InitQuota(int size)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            long newQuota = size * 1024 * 1024;
            if (isoStore.Quota < newQuota)
            {
                isoStore.IncreaseQuotaTo(newQuota);
            }
        }
    }
}
