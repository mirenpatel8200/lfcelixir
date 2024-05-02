using System.IO;

namespace Elixir.Contracts.Interfaces.Core
{
    public class SitemapInfo
    {
        public SitemapInfo(string serverRootPath, string fileName)
        {
            FileName = fileName;
            ServerRootPath = serverRootPath;

            FileSystemPath = Path.Combine(ServerRootPath, FileName);
            FileUrlPath = Path.Combine("/", FileName).Replace("\\", "/");
        }

        public SitemapInfo(string serverRootPath, string subfolderPath, string fileName): this(serverRootPath, fileName)
        {
            SubfolderPath = subfolderPath;
            FileSystemPath = Path.Combine(ServerRootPath, SubfolderPath.Replace("/", "\\"), FileName);

            FileUrlPath = Path.Combine("/", SubfolderPath, FileName).Replace("\\", "/");
        }

        public string SubfolderPath { get; }
        public string ServerRootPath { get; }
        public string FileName { get; }

        public string FileSystemPath { get; }
        public string FileUrlPath { get; }
        public bool Exists => File.Exists(FileSystemPath);
    }
}
