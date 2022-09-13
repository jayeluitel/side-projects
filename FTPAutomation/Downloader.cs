using System;
using WinSCP;

namespace FTP_Automation
{
    public static class Downloader
    {
        public static string DownloadFile(Session session, TransferOptions transferOptions, string fileName, string sourcePath, string destinationPath)
        {
            try
            {
                //downloads to a temp location in case fileName contains a wildcard
                var filePath = Path.GetTempFileName();
                TransferOperationResult transferOperationResult =
                    session.GetFiles($"{sourcePath}{fileName}", filePath, false,
                    transferOptions);

                transferOperationResult.Check();
                if (transferOperationResult.Transfers.Count == 0)
                {
                    return $"No files found on {sourcePath} for {fileName}";
                }
                else
                {
                    //checks for wildcard and records actual fileName
                    if (fileName.Contains('*'))
                    {
                        fileName = transferOperationResult.Transfers[0].FileName;
                        fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                    }

                    foreach (TransferEventArgs transferEvent in transferOperationResult.Transfers)
                    {
                        Console.WriteLine($"Download of {transferEvent.FileName} succeeded.");
                    }

                    File.Copy(filePath, destinationPath, overwrite: false);
                    return $"{destinationPath}{fileName}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Downloader.DownloadFile error: {ex.Message}");
                throw;
            }
        }
    }
}

