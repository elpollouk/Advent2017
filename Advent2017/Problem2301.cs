using Utils;
using FluentAssertions;
using Xunit;

namespace Advent2017
{
    public class Problem2301
    {
        class VM
        {
            public long[] registers = new long[26];
            public string[] prog;
            public int ip = 0;
            public long mulCount = 0;

            public bool IsDone => ip < 0 || ip >= prog.Length;
            public void setReg(char r, long v) => registers[r - 'a'] = v;
        }

        VM currentVM;

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

        void set(VM vm, string[] command)
        {
            var register = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[register] = value;
            vm.ip++;
        }

        void sub(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] -= value;
            vm.ip++;
        }

        void mul(VM vm, string[] command)
        {
            var target = ParseRegister(command[1]);
            var value = GetValue(vm, command[2]);
            vm.registers[target] *= value;
            vm.ip++;

            vm.mulCount++;
        }

        void jnz(VM vm, string[] command)
        {
            var checkValue = GetValue(vm, command[1]);
            var jumpValue = GetValue(vm, command[2]);
            if (checkValue != 0)
                vm.ip += (int)jumpValue;
            else
                vm.ip++;
        }

        void ExecInstruction(VM vm)
        {
            var command = vm.prog[vm.ip].Split(' ');
            switch (command[0])
            {
                case "set":
                    set(vm, command);
                    break;

                case "sub":
                    sub(vm, command);
                    break;

                case "mul":
                    mul(vm, command);
                    break;

                case "jnz":
                    jnz(vm, command);
                    break;

                default:
                    Oh.WhatTheFuck();
                    break;
            }
        }

        VM Exec(string datafile)
        {
            currentVM = new VM();
            currentVM.prog = FileIterator.LoadLines<string>(datafile);

            while (!currentVM.IsDone)
            {
                ExecInstruction(currentVM);
            }

            return currentVM;
        }

        [Theory]
        [InlineData("Data/2301.txt", 3025)]
        public void Part1(string datafile, long answer)
        {
            var vm = Exec(datafile);
            vm.mulCount.Should().Be(answer);
        }

        public static bool IsPrime(long value)
        {
            if (value == 1) return false;
            if (value < 4) return true;
            if (value % 2 == 0) return false;
            if (value < 9) return true;
            if (value % 3 == 0) return false;

            for (var i = 5; i * i <= value; i += 6)
            {
                if (value % i == 0) return false;
                if (value % (i + 2) == 0) return false;
            }

            return true;
        }

        [Fact]
        public void Part2()
        {
            long a = 1;
            long b = 0;
            long c = 0;
            long total = 0;

            b = 57;
            c = b;
            if (a != 0)
            {
                b *= 100;
                b += 100000;
                c = b;
                c += 17000;
            }

            do
            {
                if (!IsPrime(b))
                    total += 1;

                b += 17;
            }
            while (b <= c);

            total.Should().Be(915);
        }
    }
}
