using System;
using System.Linq;

namespace Medical.CoreLayer.Validation.Algorithm
{
    public class SrzAlgorithms
    {
        public static double Age(DateTime dr)
        {
            DateTime dt = DateTime.Today;
            double age = Math.Floor((dt - dr).Days / 365.25);
            return age;
        }

        public static bool IsSexValid(int? sex)
        {
            return sex == 1 || sex == 2;
        }

        public static bool IsBirthdayValid(DateTime? birthday)
        {
            if (birthday == null)
                return false;
            if (birthday > DateTime.Today)
                return false;
            if (birthday < new DateTime(1880, 01, 01)) 
                return false;

            return true;
        }

        public static bool IsSnils(string snils)
        {
            if (string.IsNullOrEmpty(snils))
                return false;
            
            /*
              declare @S int
              declare @i int
              declare @k int
              declare @C varchar(14)
              if (@SS not like '___-___-___ __')
                return 0
              else begin
                set @C=substring(@SS,1,3) + substring(@SS,5,3) + substring(@SS,9,3)
                if dbo.sverify(@C+substring(@SS,13,2),'0123456789')=0 return 0
                if charindex('000',@C)>0 return 0
                if charindex('111',@C)>0 return 0
                if charindex('222',@C)>0 return 0
                if charindex('333',@C)>0 return 0
                if charindex('444',@C)>0 return 0
                if charindex('555',@C)>0 return 0
                if charindex('666',@C)>0 return 0
                if charindex('777',@C)>0 return 0
                if charindex('888',@C)>0 return 0
                if charindex('999',@C)>0 return 0
                if @C < '001001998' return 1
                else begin
                   set @s=0
                   set @i=9
                   set @k=1
                   while @i>0 begin
                     if @k=4 set @k=5
                     if @k=8 set @k=9
                     set @S = @S+cast(substring(@SS,  @k, 1) as int)*@i
                     set @k=@k+1
                     set @i=@i-1
                   end
                   set @S = @S % 101
                   if @S=100 set @S=0
                   if cast(right(@SS,2) as int)=@S return 1
                 end
              end
            return 0
             */
            return true;
        }

        public static bool IsDeathDateValid(DateTime? birthday, DateTime? deathhday)
        {
            if (birthday == null || deathhday == null)
                return false;

            return birthday <= deathhday;
        }

        public static bool IsDocumentDateValid(DateTime? birthday, DateTime? document)
        {
            if (birthday == null || document == null)
                return false;

            return birthday < document;
        }

        public static bool IsDocTypeValid(int document)
        {
            int[] docType = { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18 };
            return docType.Contains(document);
        }

        //public static bool IsOkatoValid(string okato)
        //{
        //    if (string.IsNullOrEmpty(okato))
        //        return false;
                
        //    string pattern = @"(?<ter>\d{2})(?<kod1>\d{3})(?<kod2>\d{3})(?<kod3>\d{3})";
        //    Regex rx = new Regex(pattern,RegexOptions.Compiled | RegexOptions.IgnoreCase); 
                
        //    MatchCollection matches = rx.Matches(okato);
        //    if(matches.Count == 0)
        //    {
        //        return false;
        //    }
        //    GroupCollection groups = matches[0].Groups;
        //    try
        //    {
        //        using (var db = new Database())
        //        {
        //            if(ConnectionState.Open != db.Connection.State)
        //            {
        //                return false;
        //            }
        //            return db.O002.Where(b =>
        //                                        b.TER == groups["ter"].Value &&
        //                                        b.KOD1 == groups["kod1"].Value &&
        //                                        b.KOD2 == groups["kod2"].Value &&
        //                                        b.KOD3 == groups["kod3"].Value).Count() == 1;

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
            
        //}

        public static bool IsCitizenshipValid(int citizenship)
        {
            return false;
        }
  
        public static bool IsEnpK(string enp)
        {
            if (!CommonAlgorithms.IsNumber(enp) || enp.Length != 16)
            {
                return false;
            }

            int i;
            int n;
            string c;
            string s = new string(enp.Substring(0, 15).Reverse().ToArray());
            string s1 = string.Empty;
            string s2 = string.Empty;
            i = 0;
            while (i <= s.Length - 1)
            {
                c = s.Substring(i, 1);
                if(i%2 == 0)
                {
                    s2 += c;
                }else
                {
                    s1 += c;
                }
                i++;
            }
            n = Convert.ToInt32(s2)*2;
            s2 = n.ToString();
            s = s1 + s2;
            n = s.Sum(p => Convert.ToInt32(p.ToString()));
            n = n%10;
            if (n > 0)
                n = 10 - n;
            return enp.Equals(enp.Substring(0, 15) + n);
        }

        public static bool IsEnpDateSex(DateTime dateTime, int sex, string Enp)
        {
            string enpDay = "";
            string enpMonth = "";
            string enpYear = "";

            int day = dateTime.Day;
            int month = dateTime.Month;
            int year = dateTime.Year;

            if ((day > 0) && (day < 32) && (month > 0) && (month < 13) && (year > 0))
            {
                //convert month
                if (year <= 1950) enpMonth = (month + 20).ToString();
                else if (year <= 2000) enpMonth = (month + 40).ToString();
                else enpMonth = month.ToString();

                string enpMonthConvert = "";
                for (int i = 0; i < enpMonth.Length; i++)
                {
                    enpMonthConvert = enpMonthConvert + (9 - Convert.ToInt32(enpMonth[i].ToString())).ToString();
                }

                while (enpMonthConvert.Length != 2)
                {
                    enpMonthConvert = "9" + enpMonthConvert;
                }


                if (sex == 1)
                    enpDay = (day + 50).ToString();
                else
                    enpDay = day.ToString();

                string enpDayConvert = "";
                for (int i = 0; i < enpDay.Length; i++)
                {
                    enpDayConvert = enpDayConvert + (9 - Convert.ToInt32(enpDay[i].ToString())).ToString();
                }

                while (enpDayConvert.Length != 2)
                {
                    enpDayConvert = "9" + enpDayConvert;
                }

                enpYear = year.ToString();
                string enpYearConvert = "";
                for (int i = 0; i < enpYear.Length; i++)
                {
                    enpYearConvert = (9 - (Convert.ToInt32(enpYear[i].ToString()))).ToString() + enpYearConvert;
                }
                return (enpMonthConvert + enpYearConvert + enpDayConvert).Equals(Enp.Substring(2, 8));
            }
            return false;
        }

        public static string EnpByDateSex(DateTime dateTime, int sex)
        {
            string enpDay = "";
            string enpMonth = "";
            string enpYear = "";

            int day = dateTime.Day;
            int month = dateTime.Month;
            int year = dateTime.Year;

            if ((day > 0) && (day < 32) && (month > 0) && (month < 13) && (year > 0))
            {
                //convert month
                if (year <= 1950) enpMonth = (month + 20).ToString();
                else if (year <= 2000) enpMonth = (month + 40).ToString();
                else enpMonth = month.ToString();

                string enpMonthConvert = "";
                for (int i = 0; i < enpMonth.Length; i++)
                {
                    enpMonthConvert = enpMonthConvert + (9 - Convert.ToInt32(enpMonth[i].ToString())).ToString();
                }

                while (enpMonthConvert.Length != 2)
                {
                    enpMonthConvert = "9" + enpMonthConvert;
                }


                if (sex == 1)
                    enpDay = (day + 50).ToString();
                else
                    enpDay = day.ToString();

                string enpDayConvert = "";
                for (int i = 0; i < enpDay.Length; i++)
                {
                    enpDayConvert = enpDayConvert + (9 - Convert.ToInt32(enpDay[i].ToString())).ToString();
                }

                while (enpDayConvert.Length != 2)
                {
                    enpDayConvert = "9" + enpDayConvert;
                }

                enpYear = year.ToString();
                string enpYearConvert = "";
                for (int i = 0; i < enpYear.Length; i++)
                {
                    enpYearConvert = (9 - (Convert.ToInt32(enpYear[i].ToString()))).ToString() + enpYearConvert;
                }
                return (enpMonthConvert + enpYearConvert + enpDayConvert);
            }

            return null;
        }

        public static string GenerateInp(int territory, DateTime birthDate, int sex)
        {
            var enp = territory.ToString("D2") + EnpByDateSex(birthDate, sex);
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 99999; j++)
                {
                    var result = string.Format("{0}{1}{2}", enp, j.ToString("D5"), i);
                    if (IsEnpK(result))
                    {
                        return result;
                    }
                }

            }

            return null;
        }
    }
}
