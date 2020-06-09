using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Services
{
    [Flags]
    public enum FileType
    {
        Text = 0,
        Binary = 1,
    }
    public class FileService : IFileService
    {
        private readonly IMessageService _messageService;

        private Dictionary<FileType, Func<string, object, OperationResult>> _fileSaveHandlers;

        public FileService(IMessageService messageService)
        {
            _messageService = messageService;

            Init();
        }

        private void Init()
        {
            _fileSaveHandlers = new Dictionary<FileType, Func<string, object,OperationResult>>()
            {
                { FileType.Text, SaveTextFile },
                { FileType.Binary, SaveBinaryFile }
            }; 
        }

        public OperationResult<string> SelectFolder(string description, Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer)
        {
            var result = new OperationResult<string>();
            try
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = description;
                    dialog.ShowNewFolderButton = true;
                    dialog.RootFolder = rootFolder;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        result.Data = dialog.SelectedPath;
                    }
                    else
                    {
                        result.IsCanceled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }


            return result;
        }

        public OperationResult<string> SelectFile(string filter, string defaultExtension, Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer)
        {
            var result = new OperationResult<string>();
            try
            {
                var dlg = new OpenFileDialog
                {
                    DefaultExt = defaultExtension,
                    Filter = filter,
                    InitialDirectory = Environment.GetFolderPath(rootFolder)
                };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    result.Data = dlg.FileName;
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> ShowSaveFileDialog(string defaultExt, string filter, string fileName = null)
        {
            var result = new OperationResult<string>();
            try
            {
                var dlg = new SaveFileDialog { DefaultExt = defaultExt, Filter = filter, FileName = fileName};
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    result.Data = dlg.FileName;
                }
                else
                {
                    result.IsCanceled = true;
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult SaveTextFile(string filename, object data)
        {
            var result = new OperationResult();
            try
            {
                using (var writer = new StreamWriter(filename))
                {
                    writer.Write(data);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult SaveTextFileWithEncoding(string filename, object data, Encoding encoding)
        {
            var result = new OperationResult();
            try
            {
                using (var writer = new StreamWriter(filename, false, encoding))
                {
                    writer.Write(data);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }


        public OperationResult SaveBinaryFile(string filename, byte[] data)
        {
            var result = new OperationResult();
            try
            {
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult SaveBinaryFile(string filename, object data)
        {
            var result = new OperationResult();
            try
            {
                var bytes = data.ToByteArray();
                return SaveBinaryFile(filename, bytes);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult SaveFileWithDialog<T>(string defaultExt, string filter, T data, FileType type, string fileName = null)
        {
            var result = new OperationResult();
            try
            {
                var showDialogResult = ShowSaveFileDialog(defaultExt, filter, fileName);
                if (showDialogResult.Success)
                {
                    if (!_fileSaveHandlers.ContainsKey(type))
                    {
                        result.AddError(new Exception("Неверный тип файла для записи."));
                    }
                    
                    var operation = _fileSaveHandlers[type];
                    var saveResult = operation(showDialogResult.Data, data);

                    if (!saveResult.Success)
                    {
                        result.AddError(saveResult.LastError);
                    }
                }
                else if (showDialogResult.IsCanceled)
                {
                    result.IsCanceled = true;
                }
                else
                {
                    result.AddError(showDialogResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult SaveExcelWithDialog(string defaultExt, string filter, GridControl data, FileType type, string fileName = null)
        {
            var result = new OperationResult();
            try
            {
                var showDialogResult = ShowSaveFileDialog(defaultExt, filter, fileName);
                if (showDialogResult.Success)
                {
                    if (!_fileSaveHandlers.ContainsKey(type))
                    {
                        result.AddError(new Exception("Неверный тип файла для записи."));
                    }

                    data.View.ExportToXls(showDialogResult.Data);
                }
                else if (showDialogResult.IsCanceled)
                {
                    result.IsCanceled = true;
                }
                else
                {
                    result.AddError(showDialogResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult Pack(string archiveName, List<string> files, bool delete)
        {
            var result = new OperationResult();
            try
            {
                var zipHelper = new ZipHelpers();
                zipHelper.PackFiles(archiveName, files, delete);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<IEnumerable<string>> Unpack(string archiveName, string outPath = null)
        {
            var result = new OperationResult<IEnumerable<string>>();
            try
            {
                var zipHelper = new ZipHelpers();
                result.Data = zipHelper.UnpackFiles(archiveName, outPath);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public void ShowFileInExplorer(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                string argument = @"/select, " + fileName;
                Process.Start("explorer.exe", argument);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при попытке открыть файл в windows expolorer", typeof(FileService));
            }
        }

        public OperationResult<string> LoadTextFile(string filename, Encoding encoding)
        {
            var result = new OperationResult<string>();
            try
            {
                using (var stream = new StreamReader (filename, encoding))
                {
                    result.Data = stream.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult DeleteFileWithAsk(string filePath)
        {
            var result = new OperationResult();
            try
            {
                var fileName = Path.GetFileName(filePath);
                if (_messageService.AskQuestion("Вы действительно хотите удалить файл {0}".F(fileName), "Внимание!"))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult MoveFile(string src, string dest)
        {
            var result = new OperationResult();
            try
            {
                File.Move(src, dest);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }
    }
}
