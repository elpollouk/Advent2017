using Advent2019;
using System;
using System.Collections.Generic;
using Utils;
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
        static readonly Queue<int> queuedActions = new();
        static readonly int[] path = new int[] { 1, 1, 3, 3, 3, 3, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 1, 1, 1, 1, 1, 1, 4, 4, 2, 2, 4, 4, 2, 2, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 4, 4, 2, 2, 3, 3, 3, 3, 3, 3, 2, 2, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 4, 4, 1, 1, 4, 4, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 1, 1, 4, 4, 2, 2, 4, 4, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 3, 3, 2, 2, 3, 3, 3, 3, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 3, 3, 2, 2, 2, 2, 3, 3, 1, 1, 3, 3, 1, 1, 1, 1, 1, 1, 4, 4, 2, 2, 4, 4, 4, 4, 4, 4, 1, 1, 4, 4, 1, 1, 4, 4, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 1, 1, 4, 4, 2, 2, 4, 4, 2, 2, 2, 2, 4, 4, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 4, 4, 4, 4, 1, 1, 3, 3, 3, 3, 3, 3, 1, 1, 3, 3, 2, 2, 2, 2, 2, 2, 4, 4, 2, 2, 4, 4, 4, 4, 4, 4, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 4, 4, 2, 2, 2, 2, 4, 4, 2, 2, 4, 4, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 4, 4, 4, 4 };

        static VM vm;

        static void Main()
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

            if (queuedActions.Count != 0)
            {
                lastAction = queuedActions.Dequeue();
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
                        queuedActions.Enqueue(2);
                        queuedActions.Enqueue(3);
                        queuedActions.Enqueue(4);
                        break;

                    case ConsoleKey.Spacebar:
                        SendAction(path[0]);
                        for (int i = 1; i < path.Length; i++)
                            queuedActions.Enqueue(path[i]);
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
