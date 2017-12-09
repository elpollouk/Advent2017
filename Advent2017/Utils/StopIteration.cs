using System;

namespace Adevent2017.Utils
{
    class StopIteration :  Exception
    {
        public static void Now()
        {
            throw new StopIteration();
        }
    }
}
