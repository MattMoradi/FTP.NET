using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;

namespace Client
{
	public class ClientWrapper
	{
		public FtpClient internalClient = new FtpClient();
		public bool IsAuthenticated()
		{
			return internalClient.IsAuthenticated;
		}

		public FtpStatus UploadFile(string localPath, string remotePath, FtpRemoteExists existsMode=FtpRemoteExists.Overwrite, 
			bool createRemoteDir=false, FtpVerify verifyOptions=FtpVerify.None, Action<FtpProgress>progress = null)
		{
			return internalClient.UploadFile(localPath, remotePath, existsMode, createRemoteDir, verifyOptions, progress);
		}

		public int UploadFiles(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode=FtpRemoteExists.Overwrite, 
			bool createRemoteDir=true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, 
			Action<FtpProgress> progress=null)
		{
			return internalClient.UploadFiles(localPaths, remoteDir, existsMode, createRemoteDir, verifyOptions, errorHandling, progress);
		}

		public async Task<bool> CreateDirectoryAsync(string path, CancellationToken token = default)
		{
			return await internalClient.CreateDirectoryAsync(path, token);
		}

		public void Connect()
		{
			internalClient.Connect();
		}

		public void Disconnect()
		{
			internalClient.Disconnect();
		}

		public bool FileExists(string path)
		{
			return internalClient.FileExists(path);
		}

		public bool DirectoryExists(string path)
		{
			return internalClient.DirectoryExists(path);
		}

		public FtpStatus DownloadFile(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Overwrite, 
			FtpVerify verifyOptions=FtpVerify.None, Action<FtpProgress>progress=null)
		{
			return internalClient.DownloadFile(localPath, remotePath, existsMode, verifyOptions, progress);
		}

	}
}
