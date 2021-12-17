using Advent2019;
using System;
using System.Collections.Generic;
using Utils;
using Utils.VM;
using static Advent2019.IntCode;

namespace _2019_Day15
{
    class Program
    {
        static int x;
        static int y;
        static int o2x = -1;
        static int o2y = -1;
        static int lastAction = 0;
        static Queue<int> queudActions = new();

        static Executor<VmState, int, (long, long, long)> vm;

        static void Main(string[] args)
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            Console.Clear();
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.Write($"w={width}, h={height}");

            x = width / 2;
            y = 30;

            var prog = FileIterator.LoadCSV<int>("prog.txt");
            vm = IntCode.CreateVM(prog);
            vm.State.Input = Step;

            Console.SetCursorPosition(x, y);
            Console.Write('D');

            vm.Execute();
        }

        static long Step()
        {
            Console.SetCursorPosition(x, y);
            Console.Write('·');

            if (vm.State.OutputQueue.Count != 0)
            {
                var result = vm.State.OutputQueue.Dequeue();
                EvaluateResult((int)result);
            }

            Console.SetCursorPosition(x, y);
            Console.Write('D');

            if (o2x != -1)
            {
                Console.SetCursorPosition(o2x, o2y);
                Console.Write('X');
            }

            if (queudActions.Count != 0)
            {
                lastAction = queudActions.Dequeue();
                return lastAction;
            }

            lastAction = 0;
            do
            {
                var info = Console.ReadKey(true);
                switch (info.Key)
                {
                    case ConsoleKey.X:
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.Q:
                        SendAction(1);
                        queudActions.Enqueue(2);
                        queudActions.Enqueue(3);
                        queudActions.Enqueue(4);
                        break;

                    case ConsoleKey.UpArrow:
                        SendAction(1);
                        break;

                    case ConsoleKey.DownArrow:
                        SendAction(2);
                        break;

                    case ConsoleKey.LeftArrow:
                        SendAction(3);
                        break;

                    case ConsoleKey.RightArrow:
                        SendAction(4);
                        break;
                }
            }
            while (lastAction == 0);

            return lastAction;
        }

        static void EvaluateResult(int result)
        {
            int newX, newY;

            switch (lastAction)
            {
                case 0:
                    newX = x;
                    newY = y;
                    break;

                case 1:
                    newX = x;
                    newY = y - 1;
                    break;

                case 2:
                    newX = x;
                    newY = y + 1;
                    break;

                case 3:
                    newX = x - 1;
                    newY = y;
                    break;

                case 4:
                    newX = x + 1;
                    newY = y;
                    break;

                default:
                    throw new InvalidOperationException($"invalid action: {lastAction}");
            }

            if (result == 1 || result == 2)
            {
                x = newX;
                y = newY;

                if (result == 2)
                {
                    o2x = x;
                    o2y = y;
                }
            }
            else if (result == 0)
            {
                Console.SetCursorPosition(newX, newY);
                Console.Write('#');
            }
            else
            {
                throw new InvalidOperationException($"Invalid result: {result}");
            }
        }

        static void SendAction(int action)
        {
            lastAction = action;
        }
    }
}
