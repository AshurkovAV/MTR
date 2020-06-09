using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Infrastructure;

namespace Medical.AppLayer.Services
{
    public interface IApplyUpdateBaseService
    {
        bool Run(string sqlScript);
        string UnZipFileToSqlScript(byte[] data);
        OperationResult<List<string>> UnZipFileToFile(byte[] data);
    }
}
