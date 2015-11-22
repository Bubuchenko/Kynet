using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KynetLib;
using System.IO;
using System.Threading;
using System.ServiceModel;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace KynetClient
{

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CallbackContractFunctions : ICallbackContract
    {
        public void Message(string message)
        {
            Diagnostics.WriteLog(message);
        }

        #region Uploading/Downloading of files
        public async void DownloadAsync(string FilePath)
        {
            FileTransfer fileTranferInfo = new FileTransfer();

            try
            {
                fileTranferInfo.Fingerprint = System.SystemInfo.Fingerprint;
                fileTranferInfo.transferType = FileTransfer.TransferType.Download;
                fileTranferInfo.Data = Stream.Null;
                string filename = Path.GetFileName(FilePath);
                fileTranferInfo.ClientFilePath = Path.GetFullPath(filename);
                fileTranferInfo.FileName = filename;

                using (Stream stream = File.OpenRead(FilePath))
                {
                    Reporting.ReportEvent(string.Format("Download of '{0}' has started.", FilePath), EventType.EventNotification);
                    fileTranferInfo.Data = stream;
                    fileTranferInfo.FileSize = stream.Length;
                    await Task.Run(() => Client.FileserviceClient.CreateChannel().DownloadAsync(fileTranferInfo));
                    Reporting.ReportEvent(string.Format("Download of '{0}' has finished.", FilePath), EventType.EventNotification);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error uploading file to '{0}', access to the path is denied.", FilePath), EventType.FileTransferError, null, fileTranferInfo);
            }
            catch (FileNotFoundException ex)
            {
                Reporting.ReportEvent(string.Format("Error downloading file '{0}', file not found at the specified path.", ex.FileName), EventType.FileTransferError, null, fileTranferInfo);
            }
            catch (DirectoryNotFoundException)
            {
                Reporting.ReportEvent(string.Format("Error downloading file '{0}', directory not found.", Path.GetDirectoryName(FilePath)), EventType.FileTransferError, null, fileTranferInfo);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An exception was thrown while trying to download the file '{0}'.", FilePath), EventType.FileTransferError, ex, fileTranferInfo);
            }
        }

        public async void UploadAsync(string clientFilePath, string serverFilePath)
        {
            FileTransfer fileTransferinfo = new FileTransfer();
            try
            {
                fileTransferinfo.Fingerprint = System.SystemInfo.Fingerprint;
                fileTransferinfo.FileName = Path.GetFileName(serverFilePath);
                fileTransferinfo.transferType = FileTransfer.TransferType.Upload;
                fileTransferinfo.Data = Stream.Null;
                fileTransferinfo.ClientFilePath = clientFilePath;
                fileTransferinfo.ServerFilePath = serverFilePath;

                FileTransfer fileTransferResponse = await Task.Run(() => Client.FileserviceClient.CreateChannel().UploadAsync(fileTransferinfo));

                if (fileTransferResponse.Error == null)
                {
                    using (Stream output = File.Create(fileTransferinfo.FileName))
                    {
                        Reporting.ReportEvent(string.Format("Upload of '{0}' has started.", clientFilePath), EventType.EventNotification);
                        byte[] buffer = new byte[4 * 1024];
                        int len;
                        while ((len = await Task.Run(() => fileTransferResponse.Data.ReadAsync(buffer, 0, buffer.Length))) > 0)
                        {
                            await Task.Run(() => output.WriteAsync(buffer, 0, len));
                        }
                        Reporting.ReportEvent(string.Format("Upload of '{0}' has finished.", clientFilePath), EventType.EventNotification);
                    }
                }
                else
                {
                    Reporting.ReportEvent(string.Format("The server reported an error before trying to upload '{0}'. The specified file was most likely not found on the server.", clientFilePath), EventType.FileTransferError, null, fileTransferinfo);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error uploading file to '{0}', access to the path is denied.", clientFilePath), EventType.FileTransferError, null, fileTransferinfo);
            }
            catch (DirectoryNotFoundException)
            {
                Reporting.ReportEvent(string.Format("Error uploading file '{0}', the directory you're trying to upload the file to doesn't exist.", Path.GetDirectoryName(clientFilePath)), EventType.FileTransferError, null, fileTransferinfo);
            }
            catch (IOException ex)
            {
                Reporting.ReportEvent(string.Format("Error uploading file to '{0}', the file is in use by another process.", clientFilePath), EventType.FileTransferError, ex, fileTransferinfo);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An exception was thrown while trying to upload the file '{0}'.", clientFilePath), EventType.FileTransferError, ex, fileTransferinfo);
            }
        }
        #endregion

        public async Task<DirectoryInformation> GetFolderStructure(string directoryPath)
        {
            DirectoryInformation directoryInfo = new DirectoryInformation();

            try
            {
                await Task.Delay(0); //Temporary solution, since an async task requires atleast one awaited function
                directoryInfo.Fingerprint = System.SystemInfo.Fingerprint;
                directoryInfo.Folder = directoryPath;
                directoryInfo.Folders = Directory.GetDirectories(directoryPath);
                directoryInfo.Files = new List<FileData>();

                string[] files = Directory.GetFiles(directoryPath);


                foreach (string file in files)
                {
                    FileData fileInfo = new FileData();
                    fileInfo.Filename = Path.GetFileName(file);
                    fileInfo.Filesize = new FileInfo(file).Length;
                    fileInfo.FileType = FileData.GetFileType(file);
                    fileInfo.DateCreated = File.GetCreationTimeUtc(file);
                    fileInfo.DateModified = File.GetLastWriteTimeUtc(file);
                    directoryInfo.Files.Add(fileInfo);
                }

            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error trying to access '{0}', access to the path is denied.", directoryPath), EventType.FileBrowsingError);
                directoryInfo.Failed = true;
            }
            catch (DirectoryNotFoundException)
            {
                Reporting.ReportEvent(string.Format("Error trying to access '{0}', the directory you're trying to access doesn't exist.", Path.GetDirectoryName(directoryPath)), EventType.FileBrowsingError);
                directoryInfo.Failed = true;
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An exception while trying to retrieve directory information from '{0}'.", directoryPath), EventType.FileBrowsingError, ex, null);
                directoryInfo.Failed = true;
            }
            return directoryInfo;
        }


        #region File Actions
        public void OpenFile(string filepath, string arguments = "")
        {
            try
            {
                Reporting.ReportEvent(string.Format("Opening file {0}.", filepath), EventType.EventNotification);
                Process.Start(filepath, arguments);
                Reporting.ReportEvent(string.Format("File {0} opened", filepath), EventType.EventNotification);
            }
            catch (InvalidOperationException)
            {
                Reporting.ReportEvent(string.Format("Error opening file '{0}', no file name was specified.", filepath), EventType.FileActionError);
            }
            catch (FileNotFoundException)
            {
                Reporting.ReportEvent(string.Format("Error opening file '{0}', the file could not be found.", filepath), EventType.FileActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to open the file {0}.", filepath), EventType.FileActionError, ex);
            }
        }

        public void RemoveFiles(string[] filepaths)
        {
            for (int i = 0; i < filepaths.Length; i++)
            {
                try
                {
                    Reporting.ReportEvent(string.Format("Deleting file {0}.", filepaths[i]), EventType.EventNotification);
                    if (File.Exists(filepaths[i]))
                    {
                        File.Delete(filepaths[i]);
                        Reporting.ReportEvent(string.Format("File removed: {0}.", filepaths[i]), EventType.EventNotification);
                    }
                    else
                        Reporting.ReportEvent(string.Format("Unable to delete {0}, the file does not exist.", filepaths[i]), EventType.FileActionError);

                }
                catch (IOException)
                {
                    Reporting.ReportEvent(string.Format("Error deleting file '{0}', the file is in use.", filepaths[i]), EventType.FileActionError);
                }
                catch (UnauthorizedAccessException)
                {
                    Reporting.ReportEvent(string.Format("Error deleting file '{0}', access denied.", filepaths[i]), EventType.FileActionError);
                }
                catch (Exception ex)
                {
                    Reporting.ReportEvent(string.Format("An unknown error occurred while trying to remove the file {0}.", filepaths[i]), EventType.FileActionError, ex);
                }
            }
        }

        public void RenameFile(string filepath, string newName)
        {
            try
            {
                Reporting.ReportEvent(string.Format("Renaming file {0}.", filepath), EventType.EventNotification);
                if (File.Exists(filepath))
                {
                    FileSystem.Rename(filepath, Path.GetDirectoryName(filepath) + @"\" + newName);
                    Reporting.ReportEvent(string.Format("File {0} successfully renamed to {1}.", filepath, newName), EventType.EventNotification);
                }
                else
                    Reporting.ReportEvent(string.Format("Error renaming file '{0}', the file was not found.", filepath), EventType.FileActionError);
            }
            catch (IOException)
            {
                Reporting.ReportEvent(string.Format("Error renaming file '{0}', a file with the same name already exists!", filepath), EventType.FileActionError);
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error renaming file '{0}', access denied.", filepath), EventType.FileActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to rename the file {0}.", filepath), EventType.FileActionError, ex);
            }
        }

        public void MoveFile(string filepath, string newPath)
        {
            try
            {
                Reporting.ReportEvent(string.Format("Moving file {0}.", filepath), EventType.EventNotification);
                if (File.Exists(filepath))
                {
                    File.Move(filepath, filepath);
                    Reporting.ReportEvent(string.Format("File {0} successfully moved to {1}.", filepath, newPath), EventType.EventNotification);
                }
                else
                    Reporting.ReportEvent(string.Format("Error moving file '{0}', the file was not found.", filepath), EventType.FileActionError);
            }
            catch (IOException)
            {
                Reporting.ReportEvent(string.Format("Error moving file '{0}', a file with the same name already exists!", filepath), EventType.FileActionError);
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error moving file '{0}', access denied.", filepath), EventType.FileActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to move the file {0}.", filepath), EventType.FileActionError, ex);
            }
        }
        #endregion
        #region Folder Actions
        public void OpenFolder(string Folderpath)
        {
            try
            {
                Reporting.ReportEvent(string.Format("Opening folder {0}.", Folderpath), EventType.EventNotification);
                Process.Start(Folderpath);
                Reporting.ReportEvent(string.Format("Folder {0} opened", Folderpath), EventType.EventNotification);
            }
            catch (InvalidOperationException)
            {
                Reporting.ReportEvent(string.Format("Error opening folder '{0}', no folder name was specified.", Folderpath), EventType.FolderActionError);
            }
            catch (FileNotFoundException)
            {
                Reporting.ReportEvent(string.Format("Error opening folder '{0}', the folder could not be found.", Folderpath), EventType.FolderActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to open the folder {0}.", Folderpath), EventType.FolderActionError, ex);
            }
        }

        public void RemoveFolders(string[] folderPaths)
        {
            for (int i = 0; i < folderPaths.Length; i++)
            {
                try
                {
                    Reporting.ReportEvent(string.Format("Deleting directory {0}.", folderPaths[i]), EventType.EventNotification);
                    if (Directory.Exists(folderPaths[i]))
                    {
                        Directory.Delete(folderPaths[i]);
                        Reporting.ReportEvent(string.Format("Directory removed: {0}.", folderPaths[i]), EventType.EventNotification);
                    }
                    else
                        Reporting.ReportEvent(string.Format("Unable to delete {0}, the directory does not exist.", folderPaths[i]), EventType.FolderActionError);

                }
                catch (IOException)
                {
                    Reporting.ReportEvent(string.Format("Error deleting directory '{0}', a file in the directory is in use.", folderPaths[i]), EventType.FolderActionError);
                }
                catch (UnauthorizedAccessException)
                {
                    Reporting.ReportEvent(string.Format("Error deleting directory '{0}', access denied.", folderPaths[i]), EventType.FolderActionError);
                }
                catch (Exception ex)
                {
                    Reporting.ReportEvent(string.Format("An unknown error occurred while trying to remove the directory {0}.", folderPaths[i]), EventType.FolderActionError, ex);
                }
            }
        }

        public void RenameFolder(string Folderpath, string newName)
        {
            try
            {
                Reporting.ReportEvent(string.Format("Renaming directory {0}.", Folderpath), EventType.EventNotification);
                if (Directory.Exists(Folderpath))
                {
                    Directory.Move(Folderpath, Path.GetDirectoryName(Folderpath) + @"\" + newName);
                    Reporting.ReportEvent(string.Format("Directory {0} successfully renamed to {1}.", Folderpath, newName), EventType.EventNotification);
                }
                else
                    Reporting.ReportEvent(string.Format("Error renaming directory '{0}', the path was not found.", Folderpath), EventType.FolderActionError);
            }
            catch (IOException)
            {
                Reporting.ReportEvent(string.Format("Error renaming directory '{0}', a directory with the same name already exists!", Folderpath), EventType.FolderActionError);
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error renaming directory '{0}', access denied.", Folderpath), EventType.FolderActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to rename the directory {0}.", Folderpath), EventType.FolderActionError, ex);
            }
        }

        public void MoveFolder(string Folderpath, string newPath)
        {
            try
            {
                Reporting.ReportEvent(string.Format("Moving directory {0}.", Folderpath), EventType.EventNotification);
                if (Directory.Exists(Folderpath))
                {
                    Directory.Move(Folderpath, newPath);
                    Reporting.ReportEvent(string.Format("Directory {0} successfully moved to {1}.", Folderpath, newPath), EventType.EventNotification);
                }
                else
                    Reporting.ReportEvent(string.Format("Error moving directory '{0}', the path was not found.", Folderpath), EventType.FolderActionError);
            }
            catch (IOException)
            {
                Reporting.ReportEvent(string.Format("Error moving directory '{0}', a directory with the same name already exists!", Folderpath), EventType.FolderActionError);
            }
            catch (UnauthorizedAccessException)
            {
                Reporting.ReportEvent(string.Format("Error moving directory '{0}', access denied.", Folderpath), EventType.FolderActionError);
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent(string.Format("An unknown error occurred while trying to move the directory {0}.", Folderpath), EventType.FolderActionError, ex);
            }
        }
        #endregion

        public async Task<List<string>> ExecuteRemoteCommand(string command)
        {
            List<string> list = new List<string>();
            try
            {
                Diagnostics.WriteLog("Command received");
                using (Process p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        WorkingDirectory = Directory.GetDirectoryRoot("cmd.exe"),
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    {
                        list.Add(e.Data);
                    };

                    p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                    {
                        list.Add(e.Data);
                    };

                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.EnableRaisingEvents = true;
                    await p.StandardInput.WriteAsync(command + " & exit" + p.StandardInput.NewLine);
                    p.WaitForExit(60000);
                }
            }
            catch (Exception ex)
            {
                Reporting.ReportEvent("An unknown error occurred, could not execute remote command.", EventType.GeneralError, ex);
            }

            return list;
        }

        public async Task<List<UserProcess>> GetProcesses()
        {
            List<UserProcess> ProcessList = new List<UserProcess>();
            UserProcess ProcessInfo = new UserProcess();

            {
                Process[] Processes = Process.GetProcesses();
                foreach (Process p in Processes)
                {
                    try
                    {
                        ProcessInfo.Name = p.ProcessName;
                        ProcessInfo.PID = p.Id.ToString();
                        ProcessInfo.Locaton = p.MainModule.FileName;
                        ProcessInfo.Description = p.MainModule.FileVersionInfo.FileDescription;
                        ProcessList.Add(ProcessInfo);
                    }
                    catch { }
                }
                return ProcessList;
            }
        }
    }
}
