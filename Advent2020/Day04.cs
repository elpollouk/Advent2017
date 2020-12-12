using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

using Passport = System.Collections.Generic.Dictionary<string, string>;

namespace Advent2020
{
    public class Day04
    {
        static void StoreField(Passport passport, string rawField)
        {
            var split = rawField.Split(':');
            passport[split[0]] = split[1];
        }

        static List<Passport> LoadPassports(string input)
        {
            var passports = new List<Passport>();
            var currentPassport = new Passport();
            passports.Add(currentPassport);

            FileIterator.ForEachLine(input, (string details) =>
            {
                if (details == "")
                {
                    currentPassport = new();
                    passports.Add(currentPassport);
                    return;
                }

                var fields = details.Split(' ');
                foreach (var field in fields)
                    StoreField(currentPassport, field);
            });

            return passports;
        }

        bool IsValidPart1(Passport passport)
        {
            int expectedFieldCount = passport.ContainsKey("cid") ? 8 : 7;
            return passport.Count == expectedFieldCount;
        }

        static readonly HashSet<string> ValidHairColours = new(){ "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };

        bool IsValidPart2(Passport passport)
        {
            var byr = int.Parse(passport.GetOrDefault("byr", "-1"));
            if (byr < 1920 || byr > 2002) return false;

            var iyr = int.Parse(passport.GetOrDefault("iyr", "-1"));
            if (iyr < 2010 || iyr > 2020) return false;

            var eyr = int.Parse(passport.GetOrDefault("eyr", "-1"));
            if (eyr < 2020 || eyr > 2030) return false;

            var hgt = passport.GetOrDefault("hgt", "-1cm");
            var value = int.Parse(hgt.Substring(0, hgt.Length - 2));
            if (hgt.EndsWith("cm"))
            {
                if (value < 150 || value > 193) return false;
            }
            else if (hgt.EndsWith("in"))
            {
                if (value < 59 || value > 76) return false;
            }
            else return false;

            if (!passport.GetOrDefault("hcl", "").IsMatch("^#[a-f0-9][a-f0-9][a-f0-9][a-f0-9][a-f0-9][a-f0-9]$")) return false;

            if (!ValidHairColours.Contains(passport.GetOrDefault("ecl", ""))) return false;

            if (!passport.GetOrDefault("pid", "").IsMatch("^\\d\\d\\d\\d\\d\\d\\d\\d\\d$")) return false;

            return true;
        }

        [Theory]
        [InlineData("Data/Day04_test.txt", 2)]
        [InlineData("Data/Day04.txt", 254)]
        public void Part1(string input, int expected)
        {
            LoadPassports(input)
                .Where(IsValidPart1)
                .Count()
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day04_test_allvalid.txt", 4)]
        [InlineData("Data/Day04_test_allinvalid.txt", 0)]
        [InlineData("Data/Day04_test_mixed.txt", 4)]
        [InlineData("Data/Day04.txt", 184)]
        public void Part2(string input, int expected)
        {
            LoadPassports(input)
                .Where(IsValidPart2)
                .Count()
                .Should().Be(expected);
        }
    }
}
