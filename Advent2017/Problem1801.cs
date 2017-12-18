using Adevent2017.DataStructures;
using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    class VM
    {
        public long[] registers = new long[26];
        public long lastSound = 0;
        public long lastRecv = 0;
        public string[] prog;
        public int ip;
    }

    public class Problem1801
    {
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
            vm.lastSound = value;
        }

        void set(VM vm, string[] command)
        {
            var register = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[register] = value;
        }

        void add(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] += value;
        }

        void mul(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] *= value;
        }

        void mod(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] %= value;
        }

        void rcv(VM vm, string[] command)
        {
            var value = GetValue(vm, command[1]);
            if (value != 0)
            {
                vm.lastRecv = vm.lastSound;
                StopIteration.Now();
            }
        }

        void jgz(VM vm, string[] command)
        {
            var checkValue = GetValue(vm, command[1]);
            var jumpValue = GetValue(vm, command[2]) - 1;
            if (checkValue > 0)
                vm.ip += (int)jumpValue;
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

            vm.ip++;
        }

        VM Exec(string datafile)
        {
            var vm = new VM();
            vm.prog = FileIterator.LoadLines<string>(datafile);
            try
            {
                while (true)
                {
                    ExecInstruction(vm);
                    if (vm.ip < 0 || vm.ip >= vm.prog.Length)
                        break;
                }
            }
            catch (StopIteration) { }

            return vm;
        }

        [Theory]
        [InlineData("Data/1801-example.txt", 4)]
        [InlineData("Data/1801.txt", 3188)]
        public void Part1(string datafile, long answer)
        {
            var vm = Exec(datafile);
            vm.lastRecv.Should().Be(answer);
        }
    }
}
