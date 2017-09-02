using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost.Utilities
{
    public static class AttachmentHelper
    {
        public static List<MailAttachment> GetFileAttachment(HttpFileCollection files)
        {
            List<MailAttachment> attachments = null;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                if (!string.IsNullOrWhiteSpace(file.FileName) && file.ContentLength > 0)
                {
                    if (attachments == null)
                        attachments = new List<MailAttachment>();

                    var fileInfo = new FileInfo(file.FileName);

                    var att = new MailAttachment();
                    att.FileName = fileInfo.Name;
                    att.FileContent = GetFileContent(file.InputStream);
                    att.MediaType = GetMediaType(file.ContentType);

                    attachments.Add(att);
                }
            }

            return attachments;
        }

        private static MediaType GetMediaType(string contentType)
        {
            return MediaType.Other;
        }

        private static byte[] GetFileContent(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}