using System;
using System.Collections.Generic;
using System.IO;
using Core.Helpers;

namespace Medical.DataCore.v31K1.DV
{
    public class LoaderD : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v31K1.Xsd.D1.xsd"; }
        }

        public override int FileCount
        {
            get { return 2; }
        }

        public override string FileExtension
        {
            get { return "xml"; }
        }

        public override List<string> Unpack<LoaderD>(string omsFileName)
        {
            try
            {
                Info = new LoaderD();
                Info.SetInfo(Path.GetFileNameWithoutExtension(omsFileName));
                if (!string.IsNullOrWhiteSpace(Info.Errors))
                {
                    Warnings.Add(string.Format("Неправильное имя файла {0}", Path.GetFileNameWithoutExtension(omsFileName)));
                }

                var zipHelper = new ZipHelpers();
                List<string> fileNames = zipHelper.UnpackFiles(omsFileName);

                if (fileNames.Count == 0 || fileNames.Count > FileCount)
                {
                    Errors.Add(
                        string.Format("Не найдены файлы в архиве или неверное количество файлов {0} (0 или >{1} )",
                                      fileNames.Count, FileCount));
                    return null;
                }

                foreach (string name in fileNames)
                {
                    string unpackFileName = Path.GetFileName(name);
                    if (string.IsNullOrWhiteSpace(unpackFileName))
                    {
                        Errors.Add(string.Format("Не найден распакованный файл {0}", name));
                        return null;
                    }
                }

                return fileNames;
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("При загрузке xml поизошло исключение {0}", exception));
            }
            return null;
        }
    }
}