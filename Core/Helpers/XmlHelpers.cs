using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Core.Helpers
{
    public class XmlHelpers
    {
        public static XDocument RemoveEmptyOrNull(XDocument document)
        {
            if (document == null)
                return null;
            
            var memory = new MemoryStream();
            document.Save(memory);

            return XDocument.Load(RemoveEmptyOrNull(memory));
        }

        public static Stream RemoveEmptyOrNull(Stream stream)
        {
            if (stream == null)
                return null;
            stream.Position = 0;

            var document = XDocument.Load(stream);
            document.Descendants()
                    .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
                    .Remove();

            var memory = new MemoryStream();
            document.Save(memory);
            memory.Position = 0;
            return memory;
        }

        public static XDocument ToUpperCase(XDocument document)
        {
            if (document == null)
                return null;

            var memory = new MemoryStream();
            document.Save(memory);

            return XDocument.Load(ToUpperCase(memory));
        }

        public static Stream ToUpperCase(Stream stream)
        {
            if (stream == null)
                return null;
            stream.Position = 0;

            var document = XDocument.Load(stream);
            document.Descendants()
                .ToList()
                .ForEach(p => p.Name = p.Name.ToString().ToUpperInvariant());
            var memory = new MemoryStream();
            document.Save(memory);
            memory.Position = 0;
            return memory;
        }

        public static XDocument ToLowerCase(XDocument document)
        {
            if (document == null)
                return null;

            var memory = new MemoryStream();
            document.Save(memory);

            return XDocument.Load(ToLowerCase(memory));
        }

        public static Stream ToLowerCase(Stream stream)
        {
            if (stream == null)
                return null;
            stream.Position = 0;

            var document = XDocument.Load(stream);
            document.Descendants()
                .ToList()
                .ForEach(p => p.Name = p.Name.ToString().ToLowerInvariant());
            var memory = new MemoryStream();
            document.Save(memory);
            memory.Position = 0;
            return memory;
        }

        //public static XDocument ChangeCommaToDot(XDocument document)
        //{
        //    if (document == null)
        //        return null;

        //    var memory = new MemoryStream();
        //    document.Save(memory);

        //    return XDocument.Load(ChangeCommaToDot(memory));
        //}


        //public static Stream ChangeCommaToDot(Stream stream)
        //{
        //    if (stream == null)
        //        return null;
        //    stream.Position = 0;
        //    MemoryStream output = new MemoryStream();
        //    StreamWriter writer = new StreamWriter(output);

        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //        {
        //            while (!reader.EndOfStream)
        //            {
        //                string line = reader.ReadLine();
        //                line = line.Replace(",", ".");
        //                writer.WriteLine(line);
        //            }
        //            writer.Flush();
        //            //writer.Close();
        //            reader.Close();
        //        }
        //        output.Position = 0; // rewind
        //        return output;
            

        //}
    }
}
