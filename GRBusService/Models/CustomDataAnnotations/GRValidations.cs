/* File name: GRValidations.cs
 * Description: Class to perform various validations.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRBusService.Models
{
    public class GRValidations
    {
        // Capitalises the first character of a string's words
        public static string Capitalise(string value)
        {
            if (value != null)
            {
                string temp = "";
                value = RemoveSpaces(value);
                for (int i = 0; i < value.Length; i++)
                {
                    if (i == 0 || value[i - 1] == ' ')
                    {
                        temp += value.ElementAt(i).ToString().ToUpper();
                    }
                    else
                    {
                        temp += value.ElementAt(i).ToString().ToLower();
                    }
                }
                return temp;
            }
            return value;
        }

        // Trims a string and removes spaces if they are more than one
        public static string RemoveSpaces(string value)
        {
            return string.Join(" ", value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        // Formats a string to a valid canadian phone pattern with dashes
        public static string FormatCanadianPhone(string value)
        {
            string temp = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (value.ElementAt(i) > 47 && value.ElementAt(i) < 58)
                {
                    temp += value.ElementAt(i);
                }
            }
            temp = temp.Insert(3, "-");
            temp = temp.Insert(7, "-");
            return temp;
        }
    }
}
