using System;
using WinSCP;

namespace FTP_Automation
{
    public static class Uploader
    {
        public static string UploadFile(Session session, TransferOptions transferOptions, string sourcePath, string destinationPath)
        {
            try
            {
                TransferOperationResult transferOperationResult = session.PutFiles(sourcePath, destinationPath, false, transferOptions);

                transferOperationResult.Check();
                if (transferOperationResult.Transfers.Count == 0)
                {
                    return $"No files were uploaded to {destinationPath}";
                }
                else
                {
                    foreach (TransferEventArgs transferEvent in transferOperationResult.Transfers)
                    {
                        Console.WriteLine($"{transferEvent.FileName} successfully uploaded to {destinationPath}");
                    }
                    return $"{transferOperationResult.Transfers.Count} file(s) have been uploaded to {destinationPath}";
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

