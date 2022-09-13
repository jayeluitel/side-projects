using WinSCP;

namespace FTP_Automation
{
    public class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine($"Enter (comma separated) SFTP information: hostName, username, password, portNumber, hostKeyFingerprint");
                string? readInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(readInput))
                {
                    string[]? sftpInfo = readInput?.Split(',');

                    SessionOptions sessionOptions = new()
                    {
                        Protocol = Protocol.Sftp,
                        HostName = sftpInfo[0],
                        UserName = sftpInfo[1],
                        Password = sftpInfo[2],
                        PortNumber = int.Parse(sftpInfo[3]),
                        SshHostKeyFingerprint = sftpInfo[4]
                    };

                    Console.WriteLine("Enter 1 to download a file from this SFTP server, or any other key to upload a file to this SFTP server");
                    var input = Console.ReadLine();

                    var fileTransaction = input == "1" ? DownloadSftpFile(sessionOptions) : UploadSftpFile(sessionOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled error in Program.Main: {ex.Message}");
            }
            Console.WriteLine("Program concluded");
            Console.ReadKey();
        }

        public static string DownloadSftpFile(SessionOptions sessionOptions)
        {
            Console.WriteLine($"Enter (comma separated) file information:  fileName, filePath, destinationPath");
            try
            {
                string? fileInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fileInput)) return $"Invalid file information: {fileInput}";
                else
                {
                    string[]? fileInfo = fileInput?.Split(',');

                    using Session session = new();
                    session.Open(sessionOptions);

                    TransferOptions transferOptions = new();
                    transferOptions.TransferMode = TransferMode.Binary;

                    return Downloader.DownloadFile(session, transferOptions, fileInfo[0], fileInfo[1], fileInfo[2]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Program.DownloadSftpFile error: {ex.Message}");
            }
        }

        public static string UploadSftpFile(SessionOptions sessionOptions)
        {
            Console.WriteLine($"Enter (comma separated) file information: sourcePath, destinationPath");
            try
            {
                string? fileInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fileInput)) return $"Invalid file information: {fileInput}";
                else
                {
                    string[]? fileInfo = fileInput?.Split(',');

                    using Session session = new();
                    session.Open(sessionOptions);

                    TransferOptions transferOptions = new();
                    transferOptions.TransferMode = TransferMode.Binary;

                    return Uploader.UploadFile(session, transferOptions, fileInfo[0], fileInfo[1]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Program.UploadSftpFile error: {ex.Message}");
            }
        }
    }
}