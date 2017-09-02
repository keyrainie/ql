using System;
using System.IO;
using System.Net;
using System.Web;

namespace ECCentral.Service.Utility
{
	public enum UploadMethod
	{
		Add,
		Update
	}
	/// <summary>
	/// Summary description for HttpUploader.
	/// </summary>
	public class HttpUploader
	{
		public static void UploadFile(string uploadURL, string fileGroup, string fileType,
			string filePath, string specialPath,UploadMethod method)
		{
			HttpUploader.UploadFile(uploadURL, fileGroup, fileType,
				filePath, specialPath, Environment.UserName, method);
		}


		public static void UploadFile(string uploadURL, string fileGroup, string fileType,
			string filePath, string specialPath,string userName, UploadMethod method)
		{
			string uploadType = GetUploadType(method);
			HttpUploader.UploadInternal(uploadURL, fileGroup, 
				fileType, filePath, specialPath,userName,uploadType);
		}

		public static void UploadFile(string uploadURL, string fileGroup, string fileType,
			string filePath,string newFileName,string specialPath,string userName, UploadMethod method)
		{
			string uploadType = GetUploadType(method);
			HttpUploader.UploadInternalNewFileName(uploadURL, fileGroup, 
				fileType, filePath, newFileName, specialPath,userName,uploadType);
		}
		
		public static void UploadFile(Stream postedStream,string fileName,
			string uploadURL,string fileGroup, string fileType,
			string specialPath,UploadMethod method)
		{
			HttpUploader.UploadFile(postedStream,fileName,uploadURL,fileGroup,
				fileType, specialPath, Environment.UserName, method);			
		}

		public static void UploadFile(Stream postedStream,string fileName,
			string uploadURL,string fileGroup, string fileType,
			string specialPath,string userName ,UploadMethod method)
		{
			string uploadType = GetUploadType(method);

			HttpUploader.UploadFile(postedStream,fileName,uploadURL,fileGroup,
				fileType,specialPath,userName,uploadType);		
		}


		private static string GetUploadType(UploadMethod method)
		{
			string type = "ADD";
			switch (method)
			{
				case UploadMethod.Add:
					type = "ADD";
					break;
				case UploadMethod.Update:
					type = "UPDATE";
					break;
			}
			return type;
		}

		private static void UploadFile(Stream postedStream,string fileName,
			string uploadURL,string fileGroup, string fileType,
			string specialPath,string userName, string uploadType)
		{
			if (IsNullOrEmpty(uploadURL))
			{
				throw new ApplicationException("Upload Web URL Is Empty.");
			}

			if (IsNullOrEmpty(fileName))
			{
				throw new ApplicationException("FileName Is Empty.");				
			}

			if (postedStream == null || postedStream.Length == 0)
			{
				throw new ApplicationException("Posted Stream can't be Empty.");				
			}

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uploadURL);

			//Our method is post, otherwise the buffer (postvars) would be useless
			webRequest.Method = HttpMethods.Post;
			// Proxy setting
			WebProxy proxy = new WebProxy();
			webRequest.Proxy = proxy;
			//We use form contentType, for the postvars.
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = postedStream.Length;

			webRequest.Headers.Add("FileGroup", fileGroup);
			webRequest.Headers.Add("FileType", fileType);
			webRequest.Headers.Add("UploadType", uploadType);
			webRequest.Headers.Add("FileName",fileName);
			webRequest.Headers.Add("UserName", userName);
			webRequest.Headers.Add("SpecialFolderPath", specialPath);

			using (Stream requestStream = webRequest.GetRequestStream())
			{
				FileUtility.CopyStream(postedStream,requestStream);
			}

			try
			{
				webRequest.GetResponse().Close();
			}
			catch(WebException ex)
			{
				HttpWebResponse response = ex.Response as HttpWebResponse;
				if (response != null)
				{
					throw new WebException(response.StatusDescription, ex);
				}
			    throw;
			}
		}

		private static void UploadInternal(string uploadURL,string fileGroup,
			string fileType, string filePath,
			string specialPath,string userName, string uploadType)
		{
			string fileName = Path.GetFileName(filePath);
			if (IsNullOrEmpty(fileName))
			{
				throw new ApplicationException("File Name Is Empty.");				
			}

			using (Stream requestStream = new FileStream(filePath,FileMode.Open,
					   FileAccess.Read,FileShare.Read))
			{
				UploadFile(requestStream,fileName,uploadURL,fileGroup,
					fileType,specialPath,userName,uploadType);	
			}
		}

		private static void UploadInternalNewFileName(string uploadURL,string fileGroup,
			string fileType, string filePath,string newFileName,
			string specialPath,string userName, string uploadType)
		{
			if (IsNullOrEmpty(newFileName))
			{
				throw new ApplicationException("New File Name Is Empty.");				
			}
			if (!File.Exists(filePath))
			{
				throw new ApplicationException("File not exists.");				
			}

			using (Stream requestStream = new FileStream(filePath,FileMode.Open,
					   FileAccess.Read,FileShare.Read))
			{
				UploadFile(requestStream,newFileName,uploadURL,fileGroup,
					fileType,specialPath,userName,uploadType);	
			}
		}

		private static bool IsNullOrEmpty(string text)
		{
			if (text != null && text.Length > 0)
			{
				return false;
			}
			
			return true;
		}
	}
}
