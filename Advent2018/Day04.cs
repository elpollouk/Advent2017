using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day04
    {
        public enum GuardEventType
        {
            BeginShift,
            Asleep,
            Awake
        }

        class GuardEvent
        {
            private static Regex s_TimeEventRegex = new Regex(@"\[(.+)\] (.+)");
            private static Regex s_GuardIdRegex = new Regex(@"#(\d+)");

            public readonly GuardEventType Type;
            public readonly DateTime Time;
            public readonly int Id;

            public GuardEvent(string details)
            {
                var matches = s_TimeEventRegex.Match(details);
                if (matches.Groups.Count != 3) Oh.Bugger();

                Time = DateTime.ParseExact(matches.Groups[1].Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                var desription = matches.Groups[2].Value;

                if (desription == "falls asleep")
                {
                    Type = GuardEventType.Asleep;
                    Id = -1;
                }
                else if (desription == "wakes up")
                {
                    Type = GuardEventType.Awake;
                    Id = -1;
                }
                else
                {
                    Type = GuardEventType.BeginShift;
                    Id = int.Parse(s_GuardIdRegex.Match(desription).Groups[1].Value);
                }
            }
        }

        [Theory]
        [InlineData("[1518-11-01 00:00] Guard #10 begins shift", GuardEventType.BeginShift, "1518-11-01T00:00:00Z", 10)]
        [InlineData("[1518-11-03 00:24] falls asleep", GuardEventType.Asleep, "1518-11-03T00:24:00Z", -1)]
        [InlineData("[1518-11-05 00:55] wakes up", GuardEventType.Awake, "1518-11-05T00:55:00Z", -1)]
        public void Test_EventParsing(string details, GuardEventType expectedType, string expectedTime, int expectedId)
        {
            var time = DateTime.Parse(expectedTime);
            var ev = new GuardEvent(details);
            ev.Type.Should().Be(expectedType);
            ev.Time.Should().Be(time);
            ev.Id.Should().Be(expectedId);
        }

        [Theory]
        [InlineData("Day04-example.txt", 240, 4455)]
        [InlineData("Day04.txt", 94542, 50966)]
        public void Test_Problem1(string file, int answer1, int answer2)
        {
            var events = FileIterator.ForEachLine(file)
                .Select(l => new GuardEvent(l))
                .OrderBy(e => e.Time);

            var guardTimes = new Dictionary<int, int[]>();
            var currentGuard = -1;
            var asleepMinute = -1;
            foreach (var evnt in events)
            {
                switch (evnt.Type)
                {
                    case GuardEventType.BeginShift:
                        currentGuard = evnt.Id;
                        break;

                    case GuardEventType.Asleep:
                        if (asleepMinute != -1) Oh.Bollocks();
                        asleepMinute = evnt.Time.Minute;
                        break;

                    case GuardEventType.Awake:
                        if (asleepMinute == -1) Oh.PissingNora();
                        var minutes = guardTimes.GetOrCreate(currentGuard, () => new int[60]);
                        for (var i = asleepMinute; i < evnt.Time.Minute; i++)
                            minutes[i]++;

                        asleepMinute = -1;
                        break;
                }
            }

            var maxGuard = 0;
            var maxAsleep = 0;
            foreach (var pair in guardTimes)
            {
                var totalAsleep = pair.Value.Sum();
                if (maxAsleep < totalAsleep)
                {
                    maxGuard = pair.Key;
                    maxAsleep = totalAsleep;
                }
            }

            var maxMinute = guardTimes[maxGuard].ArgMax();

            (maxGuard * maxMinute).Should().Be(answer1);

            maxGuard = 0;
            maxAsleep = 0;
            foreach (var pair in guardTimes)
            {
                var max = pair.Value.Max();
                if (maxAsleep < max)
                {
                    maxGuard = pair.Key;
                    maxAsleep = max;
                }
            }

            maxMinute = guardTimes[maxGuard].ArgMax();

            (maxGuard * maxMinute).Should().Be(answer2);
        }
    }
}
