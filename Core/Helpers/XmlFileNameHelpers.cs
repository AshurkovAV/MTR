using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Core.Extensions;

namespace Core.Helpers
{
    public class XmlFileNameHelpers
    {
        
    }

    public interface IRegisterInfo
    {
        string Errors { get; set; }
        string FileName{ get; }
        int NumberXXXX { get; set; }
        void SetInfo(string filename);
    }

    public abstract class RegisterInfo : IRegisterInfo
    {
        public abstract string Errors { get; set; }
        public abstract string FileName { get; }
        public abstract int NumberXXXX { get; set; }
        public abstract void SetInfo(string filename);
    }

    public class RegisterDInfo : RegisterInfo
    {
        /*
         *  H – константа, обозначающая передаваемые данные.
            Pi – Параметр, определяющий организацию-источник:
                    T – ТФОМС;
                    S – СМО;
                    M – МО.
            Ni – Номер источника (двузначный код ТФОМС или реестровый номер СМО или МО).
            Pp – Параметр, определяющий организацию -получателя:
                    T – ТФОМС;
                    S – СМО;
                    M – МО.
            Np – Номер получателя (двузначный код ТФОМС или реестровый номер СМО или МО).
            YY – две последние цифры порядкового номера года отчетного периода.
            MM – порядковый номер месяца отчетного периода:
            N – порядковый номер пакета. Присваивается в порядке возрастания, начиная со значения   
         * */
        public override string Errors { get; set; }
        public string Const { get; set; }
        public string SourceType { get; set; }
        public int SourceTerritory { get; set; }
        public string DestinationType { get; set; }
        public int DestinationTerritory { get; set; }
        public int YearXXXX { get; set; }
        public int MonthXX { get; set; }
        public override int NumberXXXX { get; set; }

        public override string FileName
        {
            get { return string.Format("{0}{1}{2}{3}{4}{5}{6:D2}{7:D4}", Const, SourceType, SourceTerritory, DestinationType, DestinationTerritory, YearXXXX, MonthXX, NumberXXXX); }
        }

        public string FileNameXml
        {
            get { return string.Format("{0}{1}{2}{3}{4}{5}{6:D2}{7:D4}.xml", Const, SourceType, SourceTerritory, DestinationType, DestinationTerritory, YearXXXX, MonthXX, NumberXXXX); }
        }

        public string FileNameH
        {
            get { return string.Format("H{0}{1}{2}{3}{4}{5:D2}{6:D4}", SourceType, SourceTerritory, DestinationType, DestinationTerritory, YearXXXX, MonthXX, NumberXXXX); }
        }
        public string FileNameHXml
        {
            get { return string.Format("{0}.xml", FileNameH); }
        }

        public string FileNameHOms
        {
            get { return string.Format("{0}.oms", FileNameH); }
        }

        public string FileNameL
        {
            get { return string.Format("L{0}{1}{2}{3}{4}{5:D2}{6:D4}", SourceType, SourceTerritory, DestinationType, DestinationTerritory, YearXXXX, MonthXX, NumberXXXX); }
        }

        public string FileNameLXml
        {
            get { return string.Format("{0}.xml", FileNameL); }
        }

        public string FileNameLOms
        {
            get { return string.Format("{0}.oms", FileNameL); }
        }

        public string FileNameP
        {
            get { return string.Format("P{0}{1}{2}{3}{4}{5:D2}{6:D4}", SourceType, SourceTerritory, DestinationType, DestinationTerritory, YearXXXX, MonthXX, NumberXXXX); }
        }

        public string FileNamePXml
        {
            get { return string.Format("{0}.xml", FileNameP); }
        }

        public string FileNamePOms
        {
            get { return string.Format("{0}.oms", FileNameP); }
        }

        public override void SetInfo(string filename)
        {

            const string pattern = @"(?<const>L|H|P|C|l|h|p|c{1})(?<sourcetype>T|S|M{1})(?<source>\d{2,6})(?<destinationtype>T|S|M|t|s|m{1})(?<destination>\d{2})(?<year>\d{4})(?<month>\d{2})(?<number>\d{4})";
            var rx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(filename);
            if (matches.Count == 0)
            {
                Errors = "Неправильное имя файла реестра {0}".F(filename);
                return;
            }

            foreach (Match match in matches)
            {
                Const = match.Groups["const"].Value;
                SourceType = match.Groups["sourcetype"].Value;
                SourceTerritory = Convert.ToInt32(match.Groups["source"].Value, CultureInfo.InvariantCulture);
                DestinationType = match.Groups["destinationtype"].Value;
                DestinationTerritory = Convert.ToInt32(match.Groups["destination"].Value, CultureInfo.InvariantCulture);
                YearXXXX = Convert.ToInt32(match.Groups["year"].Value, CultureInfo.InvariantCulture);
                MonthXX = Convert.ToInt32(match.Groups["month"].Value, CultureInfo.InvariantCulture);
                NumberXXXX = Convert.ToInt32(match.Groups["number"].Value, CultureInfo.InvariantCulture);
            }
        }
    }

    public class RegisterEInfo : RegisterInfo
    {
        /*
         * R7846110001
         * Основная часть
         * R + 
         * код территориального фонда обязательного медицинского страхования, выставившего счет + 
         * код территориального фонда обязательного медицинского страхования, которому предъявлен счет + 
         * две последние цифры года + 
         * четырехзначный поряд-ковый номер представления основной части в текущем году
         * 
         * D7846110001
         * Исправленная часть
         * D + 
         * код территориального фонда обязательного медицинского страхования, выставившего счет + 
         * код территориального фонда обязательного медицинского страхования, которому предъявлен счет + 
         * две последние цифры года + 
         * четырехзначный порядковый номер представления исправленной части в текущем году
         * 
         * A4678110001
         * Протокол
         * А + 
         * код территориального фонда обязательного медицинского страхования, которому предъявлен счет + 
         * код территориального фонда обязательного медицинского страхования, выставившего счет + 
         * две последние цифры года + 
         * четырехзначный порядковый номер представления протокола обработки реестра счета в текущем году
         * 
         * Y7846140001
         * Журнал ФЛК
         * Y +
         * код территориального фонда обязательного медицинского страхова-ния, выставившего счет + 
         * код территориального фонда обязательного ме-дицинского страхования, которому предъявлен счет +
         * две последние цифры года + 
         * четырехзначный порядковый номер представления основной или исправленной части реестра счета в текущем году
         * 
         */
        private int _yearXX;

        public override string Errors { get; set; }
        public string Type { get; set; }
        public int SourceTerritory { get; set; }
        public int DestinationTerritory { get; set; }
        public override int NumberXXXX { get; set; }

        public int YearXX
        {
            get { return _yearXX; }
            set
            {
                if (value.ToString().Length > 2 )
                {
                    var tmp = value.ToString();
                    _yearXX = SafeConvert.ToInt32(tmp.Substring(tmp.Length-2, 2)) ?? 0;
                }
                else
                {
                    _yearXX = value;
                }
                
            }
        }

        public override string FileName
        {
            get { return string.Format("{0}{1:D2}{2:D2}{3:D2}{4:D4}", Type, SourceTerritory, DestinationTerritory, YearXX, NumberXXXX); }
        }

        public string FileNameXml
        {
            get { return string.Format("{0}{1:D2}{2:D2}{3:D2}{4:D4}.xml", Type, SourceTerritory, DestinationTerritory, YearXX, NumberXXXX); }
        }

        public string FileNameOms
        {
            get { return string.Format("{0}{1:D2}{2:D2}{3:D2}{4:D4}.oms", Type, SourceTerritory, DestinationTerritory, YearXX, NumberXXXX); }
        }

        public override void SetInfo(string filename)
        {

            const string pattern = @"(?<type>R|D|A|Y|r|d|a|y{1})(?<source>\d{2})(?<destination>\d{2})(?<year>\d{2})(?<number>\d{4})";
            var rx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(filename);
            if (matches.Count == 0)
            {
                Errors = "Неправильное имя файла реестра {0}".F(filename);
                return;
            }

            foreach (Match match in matches)
            {
                Type = match.Groups["type"].Value;
                SourceTerritory = Convert.ToInt32(match.Groups["source"].Value, CultureInfo.InvariantCulture);
                DestinationTerritory = Convert.ToInt32(match.Groups["destination"].Value, CultureInfo.InvariantCulture);
                YearXX = Convert.ToInt32(match.Groups["year"].Value, CultureInfo.InvariantCulture);
                NumberXXXX = Convert.ToInt32(match.Groups["number"].Value, CultureInfo.InvariantCulture);
            }
        }
    }
}
