using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ECCentral.Service.Utility
{
	/// <summary>
	/// Summary description for FileUtility.
	/// </summary>
	public class FileUtility
	{
		/// <summary>
		/// Read all Bytes from file 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>byte [] </returns>
		public static byte[] ReadAllBytes(string filePath)
		{
			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				byte[] buffer = new byte[stream.Length];
				int count = 0;
				int readCount = 0;
				while ((count = stream.Read(buffer, readCount, buffer.Length - readCount)) != 0)
				{
					readCount += count;
				}
				return buffer;
			}
		}

		public static void CopyStream(Stream sourceStream,Stream destStream)
		{
			if (sourceStream == null)
			{
				throw new Exception("Source Stream can't be empty.");
			}

			if (destStream == null)
			{
				throw new Exception("destination Stream can't be empty.");
			}

			byte[] buffer = new byte[1024*8];
			int count = 0;

			while ((count = sourceStream.Read(buffer,0,buffer.Length)) > 0)
			{
				destStream.Write(buffer,0,count);
			}			
		}
	}
}
