using FomoAPI.Infrastructure;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;

namespace FomoAPIIntegrationTests.Infrastructure.Exchanges
{
    public class StubNasdaqFtpClient : IFtpClient
    {
        private readonly string _fileUrl;
        public StubNasdaqFtpClient(string fileUrl)
        {
            _fileUrl = fileUrl;
        }

        public Stream DownloadFile(string url, string username, string password)
        {
            var mockDbOptions = new Mock<IOptionsMonitor<DbOptions>>();

            mockDbOptions.Setup(x => x.CurrentValue).Returns(new DbOptions
            {
                ConnectionString = TestAppSettings.Instance.TestDBConnectionString
            });

            var syncRepo = new ExchangeSyncRepository(mockDbOptions.Object);
            Stream symbolsDownload = GetFile(_fileUrl);

            return symbolsDownload;
        }

        private Stream GetFile(string fileUrl)
        {
            FileInfo fileInfo = null;
            string fileName = Path.GetFileName(fileUrl);

            if (File.Exists(fileName))
            {
                fileInfo = new FileInfo(fileName);
            }

            bool fileMissingOrStale = fileInfo == null || (fileInfo != null && fileInfo.CreationTime < DateTime.Today);

            if (fileMissingOrStale)
            {

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                var ftpClient = new FtpClient();
                using (var downloadStream = ftpClient.DownloadFile(fileUrl, string.Empty, string.Empty))
                {
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        downloadStream.CopyTo(fileStream);
                    }
                }
            }

            return new MemoryStream(File.ReadAllBytes(fileName));
        }
    }
}
