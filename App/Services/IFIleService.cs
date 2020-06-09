using System;
using System.Collections.Generic;
using System.Text;
using Core.Infrastructure;
using DevExpress.Xpf.Grid;

namespace Medical.AppLayer.Services
{
    public interface IFileService
    {
        OperationResult<string> SelectFolder(string description, Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer);
        OperationResult<string> SelectFile(string filter, string defaultExtension, Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer);
        OperationResult SaveTextFile(string filename, object data);
        OperationResult SaveTextFileWithEncoding(string filename, object data, Encoding encoding);
        OperationResult SaveBinaryFile(string filename, object data);
        OperationResult SaveBinaryFile(string filename, byte[] data);
        OperationResult<string> ShowSaveFileDialog(string defaultExt, string filter, string fileName = null);
        OperationResult SaveFileWithDialog<T>(string defaultExt, string filter, T data, FileType type, string fileName = null);
        OperationResult SaveExcelWithDialog(string defaultExt, string filter, GridControl data, FileType type, string fileName = null);
        OperationResult Pack(string archiveName, List<string> files, bool delete);
        OperationResult<IEnumerable<string>> Unpack(string archiveName, string outPath = null);
        void ShowFileInExplorer(string filename);
        OperationResult<string> LoadTextFile(string filename, Encoding encoding);
        OperationResult DeleteFileWithAsk(string filePath);
        OperationResult MoveFile(string src, string dest);
    }
}