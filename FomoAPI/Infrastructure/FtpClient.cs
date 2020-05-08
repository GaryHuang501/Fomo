using System.IO;
using System.Net;

namespace FomoAPI.Infrastructure
{
    public class FtpClient: IFtpClient
    {
        public Stream DownloadFile(string url, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            return responseStream;
        }
    }
}
