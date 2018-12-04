using FluentAssertions;
using System;
using System.Globalization;
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
    }
}
