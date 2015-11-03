using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RealEstate.Classes
{
    class Validation
    {
        public bool IsTextNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return !reg.IsMatch(text);
        }

        public bool IsNumberInRange(int min, int max, string text)
        {
            try
            {
                int temp_value = int.Parse(text);

                return ((temp_value >= min && temp_value <= max) ? true : false);
            }
            catch
            {
                return false;
            }
        }

        public bool TextHasNumber(string text)
        {
            return text.Where(c => Char.IsDigit(c)).Any();
        }


        public bool TextHasSpecialChars(string text)
        {
            return text.Any(c => !Char.IsLetterOrDigit(c));
        }

        public bool TextIsLongerThan(string text, int length)
        {
            return (text.Length > length ? true : false);
        }
        public bool TextIsShorterThan(string text, int length)
        {
            return (text.Length < length ? true : false);
        }
        public bool TextContainsUpperCase(string text)
        {
            return text.Any(c => !Char.IsUpper(c));
        }
        public bool TextisEmail(string text)
        {
            return (text.Contains('@') && text.Contains('.'));
        }
        public bool DateTest(string text)
        {
            bool flag = false;
            string [] date =text.Split(new string[] { "-" }, StringSplitOptions.None);            
            if(text.Length == 10)
            {
                if (date.Count() == 3)
                {
                    string year = date[0];
                    string month = date[1];
                    string day = date[2];

                    if(IsTextNumeric(year) && IsTextNumeric(month) && IsTextNumeric(day))
                    {
                        if (year.Length == 4 && month.Length == 2 && day.Length == 2)
                            flag = true;
                    }
                }
            }
            return flag;
        }
        public bool TextContainsBlankSpaces(string text)
        {
            return text.Contains(' ');
        }
    }
}
