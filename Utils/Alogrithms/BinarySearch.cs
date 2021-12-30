using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Alogrithms
{
    public class BinarySearch
    {
        /*
         * checkNumber return:
         *     0 = Number matches
         *     + = Number is too low
         *     - = Number is too high
         */
        public static long Search(long min, long max, Func<long, int> checkNumber)
        {
            long mid = (min + max) / 2;

            while (true)
            {
                var result = checkNumber(mid);
                if (result == 0) break;
                if (result < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
                mid = (min + max) / 2;
            }

            return mid;
        }
    }
}
