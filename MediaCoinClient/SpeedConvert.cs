using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCoinClient
{
    class SpeedConvert
    {
        
        public string ConvertFromBytes(int bits)
        {
            var names = new List<string> { "b/s", "Kbit/s", "Mbit/s", "Gbit/s"};
            float check = bits * 8;
            if (check < 1024)
            {
                return Math.Round(check, 2).ToString()+" " + names[0];
            }

            int counter = 1;
            //int tmp_check = check;
            while (true)
            {
                if (check > 1024)
                {
                    counter++;
                    check /= 1024;
                }
                else
                {
                    counter--;
                    break;
                }
            }

            return Math.Round(check,2).ToString()+ " " + names[counter];
            
        }

        public string ConvertFromBytesFs(long bits)
        {
            var names = new List<string> { "B", "KB", "MB", "GB" , "TB", "PB"};
            float check = bits;
            if (check < 1024)
            {
                return Math.Round(check, 2).ToString() + " " + names[0];
            }

            int counter = 1;
            //int tmp_check = check;
            while (true)
            {
                if (check > 1024)
                {
                    counter++;
                    check /= 1024;
                }
                else
                {
                    counter--;
                    break;
                }
            }

            return Math.Round(check, 2).ToString() + " " + names[counter];

        }
    }
}
