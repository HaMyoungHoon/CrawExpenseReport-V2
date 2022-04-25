using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Data
{
    public static class ClipboardHelper
    {
        public static List<string[]> ParseClipboardData(string rawDataStr)
        {
            List<string[]> clipboardData = null;

            string[] rows = rawDataStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (rows != null)
            {
                clipboardData = new List<string[]>();
                foreach (string row in rows)
                {
                    clipboardData.Add(ParseTextFormat(row));
                }
            }
            return clipboardData;
        }

        public static string[] ParseTextFormat(string value)
        {
            List<string> outputList = new List<string>();

            char separator = '\t';
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch == separator)
                {
                    outputList.Add(value.Substring(startIndex, endIndex - startIndex));

                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                }
                else if (i + 1 == value.Length)
                {
                    // add the last value
                    outputList.Add(value.Substring(startIndex));
                    break;
                }
                else
                {
                    endIndex++;
                }
            }
            return outputList.ToArray();
        }
    }
}
