using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public class FileExportResult
    {
        public string RestServiceError
        {
            get;
            set;
        }

        public string DownloadUrl
        {
            get;
            set;
        }
    }
}
