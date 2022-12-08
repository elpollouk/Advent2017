using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day07
    {
        class Entry
        {
            public string Name;
            public long Size;
            public Dictionary<string, Entry> Children = new();
            public Entry Parent;
            public bool IsDir;

            public Entry(string name) { Name = name; }
        }

        void ls(Entry pwd, string[] log, ref int logIndex)
        {
            while (logIndex < log.Length && log[logIndex][0] != '$')
            {
                long size = 0;
                bool isDir = false;
                var parts = log[logIndex++].Split(' ');
                if (parts[0] != "dir")
                {
                    size = long.Parse(parts[0]);
                }
                else
                {
                    isDir = true;
                }

                Entry child = new(parts[1])
                {
                    Size = size,
                    IsDir = isDir,
                    Parent = pwd,
                };
                pwd.Children[parts[1]] = child;
            }
        }

        long CalcSize(Entry entry)
        {
            if (entry.Size == 0)
            {
                foreach (var child in entry.Children.Values)
                {
                    entry.Size += CalcSize(child);
                }
            }

            return entry.Size;
        }

        Entry Parse(string[] log)
        {
            Entry root = new("/");
            Entry pwd = root;

            int logIndex = 0;
            while (logIndex < log.Length)
            {
                var cmd = log[logIndex++];
                if (cmd == "$ ls")
                {
                    ls(pwd, log, ref logIndex);
                }
                else if (cmd == "$ cd /")
                {
                    pwd = root;
                }
                else if (cmd == "$ cd ..")
                {
                    pwd = pwd.Parent;
                }
                else
                {
                    var groups = cmd.Groups(@"\$ cd (.+)");
                    pwd = pwd.Children[groups[0]];
                }
            }

            CalcSize(root);

            return root;
        }

        void Walk(Entry entry, Action<Entry> action)
        {
            action(entry);
            foreach (var child in entry.Children.Values)
            {
                Walk(child, action);
            }
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 95437)]
        [InlineData("Data/Day07.txt", 1749646)]
        public void Part1(string filename, long expectedAnswer)
        {
            var log = FileIterator.LoadLines<string>(filename);
            var fs = Parse(log);

            long sum = 0;

            Walk(fs, entry => {
                if (entry.IsDir && entry.Size <= 100000)
                {
                    sum += entry.Size;
                }
            });

            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 24933642)]
        [InlineData("Data/Day07.txt", 1498966)]
        public void Part2(string filename, long expectedAnswer)
        {
            var log = FileIterator.LoadLines<string>(filename);
            var fs = Parse(log);

            long shrinkTo = 70000000 - 30000000;
            long deleteSize = fs.Size - shrinkTo;
            long smallest = long.MaxValue;

            Walk(fs, entry => {
                if (entry.IsDir && entry.Size >= deleteSize && entry.Size < smallest)
                {
                    smallest = entry.Size;
                }
            });

            smallest.Should().Be(expectedAnswer);
        }
    }
}
