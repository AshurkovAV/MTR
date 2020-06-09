using System;
using System.Collections.Generic;
using System.IO;
using Core.Extensions;
using ICSharpCode.SharpZipLib.Zip;

namespace Core.Helpers
{
    public class ZipHelpers
    {
        /*public List<string> UnpackFiles(string zipFIleName)
        {
            //fix unknow version of zip
            var stream = File.OpenRead(zipFIleName);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            memoryStream.Position = 4;
            memoryStream.WriteByte(0x14);
            memoryStream.WriteByte(0);
            memoryStream.Position = 0;

            var fileNames = new List<string>();
            var zip = new ZipInputStream(memoryStream);
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(entry.Name);
                fileNames.Add(string.Format("{0}\\{1}", Path.GetDirectoryName(zipFIleName), fileName));
                if (fileName != null && !fileName.Equals(""))
                {
                    var streamWriter = File.Create(string.Format("{0}\\{1}", Path.GetDirectoryName(zipFIleName), fileName));
                    while (true)
                    {
                        var dataRead = new byte[2048];

                        int size = zip.Read(dataRead, 0, dataRead.Length);
                        if (size != 0)
                            streamWriter.Write(dataRead, 0, size);
                        else break;
                    }
                    streamWriter.Close();
                }
            }
            zip.Close();
            return fileNames;
        }*/

        public List<string> UnpackFiles(string zipFIleName, string outPath = null)
        {
            //fix unknow version of zip
            var stream = File.OpenRead(zipFIleName);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            memoryStream.Position = 4;
            memoryStream.WriteByte(0x14);
            memoryStream.WriteByte(0);
            memoryStream.Position = 0;

            var fileNames = new List<string>();
            var zip = new ZipInputStream(memoryStream);
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(entry.Name);
                if (fileName!=null && fileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    fileNames.Add(string.Format("{0}\\{1}", outPath.IsNotNullOrEmpty() ? outPath : Path.GetDirectoryName(zipFIleName), fileName));
                    if (fileName != null && !fileName.Equals(""))
                    {
                        var streamWriter = File.Create(string.Format("{0}\\{1}", outPath.IsNotNullOrEmpty() ? outPath : Path.GetDirectoryName(zipFIleName), fileName));
                        while (true)
                        {
                            var dataRead = new byte[2048];

                            int size = zip.Read(dataRead, 0, dataRead.Length);
                            if (size != 0)
                                streamWriter.Write(dataRead, 0, size);
                            else break;
                        }
                        streamWriter.Close();
                    }
                }
                
            }
            zip.Close();
            stream.Close();
            return fileNames;
        }

        public void PackFiles(string archiveFileName,List<string> filenames,bool deleteFiles = true)
        {
            if (string.IsNullOrEmpty(archiveFileName))
                throw new Exception("Название архива не указано");
            if(filenames.Count == 0)
                throw new Exception("Пустой список файлов для архивации");

            using (var s = new ZipOutputStream(File.Create(archiveFileName)))
            {
                var buffer = new byte[4096];
                s.SetLevel(9);
                s.UseZip64 = UseZip64.Off;
                foreach (string file in filenames)
                {
                    if(File.Exists(file))
                    {
                        var entry = new ZipEntry(Path.GetFileName(file)) {DateTime = DateTime.Now};
                        s.PutNextEntry(entry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                        if (deleteFiles)
                            File.Delete(file);
                    }
                }
                s.Finish();
                s.Close();
            }
        }

    }
}
