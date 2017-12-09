using System;

namespace Adevent2017.Utils
{
    static class Oh
    {
        private static void Swear(string profanity) { throw new Exception($"Oh {profanity}"); }

        public static void Bollocks() => Swear("bollock");
        public static void ForFucksSake() => Swear("for fucks sake!");
        public static void WhatTheFuck() => Swear("what the fuck?");
        public static void PissingNora() => Swear("pissing Nora");
        public static void ShttingHell() => Swear("shitting hell!");
    }
}
