using System;

namespace Utils
{
    public class StopIteration :  Exception
    {
        public static void Now()
        {
            throw new StopIteration();
        }
    }
}
