using System;

namespace Advent2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var vm = IntCode.CreateVM("prog.txt");
            vm.State.Input = () =>
            {
                var c = Console.ReadKey().KeyChar;
                if (c == '\r') c = '\n';
                return c;
            };
            vm.State.Output = c => Console.Write((char)c);
            vm.Execute();
        }
    }
}
