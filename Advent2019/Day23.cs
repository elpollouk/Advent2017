using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day23
    {
        class Network
        {
            readonly IntCode.VM[] vms = new IntCode.VM[50];
            readonly Dictionary<IntCode.VM, bool> Interrupted = new();
            readonly bool natActive;
            readonly HashSet<long> ySeen = new();

            public (IntCode.VM sender, long dest, long x, long y) FinalMessage { get; private set; }
            
            public long FirstDoubleY { get; private set; }
            bool _wasActive;

            public Network(string filename, bool natActive)
            {
                this.natActive = natActive;

                var prog = FileIterator.LoadCSV<long>(filename);
                for (var i = 0; i < vms.Length; i++)
                {
                    var vm = IntCode.CreateVM(prog);
                    vm.Write(i);
                    vm.State.Input = () =>
                    {
                        if (!vm.HasInput)
                        {
                            Interrupted[vm] = true;
                            return -1;
                        }
                        return vm.State.InputQueue.Dequeue();
                    };
                    vm.State.Output = v =>
                    {
                        _wasActive = true;
                        if (vm.State.OutputQueue.Count == 2)
                        {
                            Route(vm, vm.Read(), vm.Read(), v);
                            return;
                        }
                        vm.State.OutputQueue.Enqueue(v);
                    };

                    vms[i] = vm;
                }
            }

            void Route(IntCode.VM sender, long dest, long x, long y)
            {
                if (dest == 255)
                {
                    FinalMessage = (sender, dest, x, y);
                    Interrupted[sender] = true;
                    return;
                }
                vms[dest].Write(x);
                vms[dest].Write(y);
            }

            void Execute(IntCode.VM vm)
            {
                Interrupted[vm] = false;
                vm.Execute(s => s.HasHalted || Interrupted[vm]);
            }

            bool ExecutionSweep(int fromVm)
            {
                _wasActive = false;

                for (var i = 0; i < vms.Length; i++)
                    Execute(vms[(fromVm + i) % vms.Length]);

                return _wasActive;
            }

            public void Execute()
            {
                FinalMessage = (null, -1, -1, -1);

                int vm = 0;
                while (true)
                {
                    Execute(vms[vm]);
                    vm = ++vm % vms.Length;

                    if (FinalMessage.sender != null)
                    {
                        if (!natActive) break;

                        var wasActive = ExecutionSweep(vm);
                        if (!wasActive)
                        {
                            if (ySeen.Contains(FinalMessage.y))
                            {
                                FirstDoubleY = FinalMessage.y;
                                return;
                            }
                            else
                            {
                                ySeen.Add(FinalMessage.y);
                            }
                            vms[0].Write(FinalMessage.x);
                            vms[0].Write(FinalMessage.y);
                            FinalMessage = (null, -1, -1, -1);
                        }
                    }
                }
            }
        }

        [Theory]
        [InlineData("Data/Day23.txt", 17541)]
        public void Part1(string filename, long expectedAnswer)
        {
            Network net = new(filename, false);
            net.Execute();
            net.FinalMessage.y.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day23.txt", 12415)]
        public void Part2(string filename, long expectedAnswer)
        {
            Network net = new(filename, true);
            net.Execute();
            net.FirstDoubleY.Should().Be(expectedAnswer);
        }
    }
}
