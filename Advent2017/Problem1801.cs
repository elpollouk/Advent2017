using Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem1801
    {
        class VM
        {
            public long[] registers = new long[26];
            public Queue<long> send;
            public Queue<long> recv;
            public string[] prog;
            public int ip;
            public bool waiting = false;
            public long sendCount = 0;

            public bool IsDone => ip < 0 || ip >= prog.Length;
            public bool IsWaitingForInput => recv.Count == 0 && waiting;
            public void setReg(char r, long v) => registers[r - 'a'] = v;
        }

        VM currentVM;
        VM vm0;
        VM vm1;

        int ParseRegister(string value)
        {
            if (value.Length == 1 && 'a' <= value[0] && value[0] <= 'z')
                return value[0] - 'a';
            return -1;
        }

        long GetValue(VM vm, string value)
        {
            var register = ParseRegister(value);
            if (register != -1)
                return vm.registers[register];
            return long.Parse(value);
        }

        void snd(VM vm, string[] command)
        {
            var value = GetValue(vm, command[1]);
            vm.send.Enqueue(value);
            vm.sendCount++;
            vm.ip++;
        }

        void set(VM vm, string[] command)
        {
            var register = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[register] = value;
            vm.ip++;
        }

        void add(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] += value;
            vm.ip++;
        }

        void mul(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] *= value;
            vm.ip++;
        }

        void mod(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] %= value;
            vm.ip++;
        }

        void rcv(VM vm, string[] command)
        {
            if (vm.recv.Count == 0)
            {
                vm.waiting = true;
                currentVM = vm == vm0 ? vm1 : vm0;
                return;
            }
            vm.waiting = false;
            var register = ParseRegister(command[1]);
            vm.registers[register] = vm.recv.Dequeue();
            vm.ip++;
        }

        void jgz(VM vm, string[] command)
        {
            var checkValue = GetValue(vm, command[1]);
            var jumpValue = GetValue(vm, command[2]);
            if (checkValue > 0)
                vm.ip += (int)jumpValue;
            else
                vm.ip++;
        }

        void ExecInstruction(VM vm)
        {
            var command = vm.prog[vm.ip].Split(' ');
            switch (command[0])
            {
                case "snd":
                    snd(vm, command);
                    break;

                case "set":
                    set(vm, command);
                    break;

                case "add":
                    add(vm, command);
                    break;

                case "mul":
                    mul(vm, command);
                    break;

                case "mod":
                    mod(vm, command);
                    break;

                case "rcv":
                    rcv(vm, command);
                    break;

                case "jgz":
                    jgz(vm, command);
                    break;

                default:
                    Oh.WhatTheFuck();
                    break;
            }
        }

        VM Exec(string datafile)
        {
            vm0 = new VM();
            vm1 = new VM();
            var q0to1 = new Queue<long>();
            var q1to0 = new Queue<long>();
            vm0.send = q0to1;
            vm0.recv = q1to0;
            vm1.send = q1to0;
            vm1.recv = q0to1;

            vm0.setReg('p', 0);
            vm1.setReg('p', 1);

            vm0.prog = vm1.prog = FileIterator.LoadLines<string>(datafile);
            currentVM = vm1;

            while (true)
            {
                ExecInstruction(currentVM);
                if (currentVM.IsDone)
                    break;
                if (vm0.IsWaitingForInput && vm1.IsWaitingForInput)
                    break;
            }

            return vm1;
        }

        [Theory]
        //[InlineData("Data/1801-example.txt", 3)]
        [InlineData("Data/1801.txt", 7112)]
        public void Part1(string datafile, long answer)
        {
            var vm = Exec(datafile);
            vm.sendCount.Should().Be(answer);
        }
    }
}
