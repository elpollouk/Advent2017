namespace Utils
{
    public static class BitFlags
    {
        public static void SetFlag(ref this long value, int flag)
        {
            value |= 1L << flag;
        }

        public static void SetFlag(ref this long value, int flag, bool state)
        {
            if (state)
                SetFlag(ref value, flag);
            else
                ClearFlag(ref value, flag);
        }

        public static void ClearFlag(ref this long value, int flag)
        {
            value &= (1L << flag) ^ -1;
        }

        public static bool CheckFlag(this long value, int flag)
        {
            return (value & (1L << flag)) != 0;
        }
    }
}
