using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure
{
    public interface IFtpClient
    {
        /// <summary>
        /// Downloadfile form FTP
        /// </summary>
        /// <param name="url">ftp url</param>
        /// <param name="username">user login</param>
        /// <param name="password">user password</param>
        /// <returns>Returns stream of downloaded file</returns>
        Stream DownloadFile(string url, string username, string password);
    }
}
