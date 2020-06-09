using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace Core.Helpers
{
    public class FileHelpers
    {
        
        public void DeleteFile(string fileName)
        {
            FileInfo fileInf = new FileInfo(fileName);
            if (fileInf.Exists)
            {
                try
                {
                    fileInf.Delete();
                }
                catch (Exception ex)
                {
                    
                }
                
            }
        }
    }
}
