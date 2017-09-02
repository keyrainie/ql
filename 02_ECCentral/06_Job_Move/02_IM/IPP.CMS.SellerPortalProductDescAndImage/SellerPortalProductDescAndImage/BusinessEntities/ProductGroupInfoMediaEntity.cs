using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    public class ProductGroupInfoMediaEntity : DefaultDataEntity
    {
        public int ProductGroupSysno { get; set; }

        public string FileType { get; set; }

        public Guid FileGuid { get; set; }          //*

        public string FileName { get; set; }

        public bool IsFirstChunck { get; set; }     //*

        public bool IsLastChunck { get; set; }      //*

        public bool IsNeedWatermark { get; set; }   //*

        public byte[] ChunckData { get; set; }      //*

        public long FileLenght { get; set; }

        public long ChunckSize { get; set; }

        public int ChunckIndex { get; set; }

        public int ChunckCount { get; set; }

        public string FileWebPath { get; set; }
    }
}
