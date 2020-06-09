using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Medical.CoreLayer.Validation.Algorithm
{
    public class CommonAlgorithms
    {
        public static bool IsLuhnValid(string s)
        {

            return s.Reverse().SelectMany((c, i) => ((c - '0') << (i & 1)).ToString()).Sum(c => (c - '0')) % 10 == 0;
        }

        public static bool IsInpValid(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            if (s.Length < 16)
                return false;

            return true;
            /*
            int control = Convert.ToInt32(s.Substring(s.Length -1,1));
            s = s.Substring(0, 15);
            //string s1 = s.Reverse().SelectMany((c, i) => ((c - '0') << (i & 1)).ToString()).ToString();
            //int summa = s.Reverse().SelectMany((c, i) => ((c - '0') << (i & 1)).ToString()).Sum(c => (c - '0'));

            //return (s.Reverse().SelectMany((c, i) => ((c - '0') << (i & 1)).ToString()).Sum(c => (c - '0')) + control) % 10 == 0;

            int sum = 0;
            for (int i = 0; i < s.Length - 1; i++)
            {
                int p = Convert.ToInt32(s[s.Length - i - 1].ToString());
                if (i % 2 == 0)
                {
                    p *= 2;
                    if (p > 9)
                    {
                        p -= 9;
                    }
                }
                sum += p;
            }
            sum = (10 - (sum%10)%10);
            if (control == sum)
                return true;
            return false;
             */
            //for i = 1 to N-1 do
            //  p = Num[N-i]    
            //  if (i mod 2 == 0) then
            //    p = 2*p
            //    if (p > 9) then 
            //      p = p - 9
            //    end if
            //  end if
            //  sum = sum + p
            //next i
            ////дополнение до 10 
            //sum = (10 - (sum mod 10) mod 10)
            //Num[N] = sum
        }

        public static bool IsSnilsValid(string s)
        {

            int control = Convert.ToInt32(s.Substring(s.Length - 2, 2));
            string snils = s.Substring(0, s.Length - 2);
            snils = snils.Replace("-", "").Trim();
            
            int snilsNumber = Convert.ToInt32(snils);
            //TODO don't check number less 001-001-998 Check what's means "less"
            //001-001-998
            if (snilsNumber < 001001998)
                return true;

            int result = snils.Reverse().Select((c, i) => ((c - '0') * (i + 1))).Sum(c => c) % 101 ;
            result = result == 101 || result == 100 ? 00 : result;
            return result == control;
          }

        public static int GetInpRegion(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            if (s.Length < 16)
                return 0;

            return Convert.ToInt32(s.Substring(0, 2));
        }

        public static bool IsNumber(string number)
        {
            var reNum = new Regex(@"^\d+$");
            return reNum.Match(number).Success;
        }



    }
}
