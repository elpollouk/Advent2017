using System;

namespace Adevent2017.Utils
{
    public class Expletive : Exception
    {
        public Expletive(string profanity) : base(profanity) { }
    }

    static class Oh
    {
        private static void Swear(string profanity) { throw new Expletive($"Oh {profanity}"); }

        public static void Bollocks() => Swear("bollock");
        public static void ForFucksSake() => Swear("for fucks sake!");
        public static void WhatTheFuck() => Swear("what the fuck?");
        public static void PissingNora() => Swear("pissing Nora");
        public static void ShttingHell() => Swear("shitting hell!");
        public static void Bugger() => Swear("bugger");
    }
}
