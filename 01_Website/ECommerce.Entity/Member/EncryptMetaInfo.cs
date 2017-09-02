using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class EncryptMetaInfo
    {
        public string PasswordSalt { get; set; }
        public EncryptMode EncryptMode { get; set; }

        public EncryptMetaInfo()
        {
        }

        public EncryptMetaInfo(string passwordSalt)
        {
            PasswordSalt = passwordSalt;
            EncryptMode = EncryptMode.SHA1;
        }

        public EncryptMetaInfo(string passwordSalt, EncryptMode encryptMode)
        {
            PasswordSalt = passwordSalt;
            EncryptMode = encryptMode;
        }
    }
}
