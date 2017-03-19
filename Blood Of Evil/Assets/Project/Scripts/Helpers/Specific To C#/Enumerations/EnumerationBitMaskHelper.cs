namespace BloodOfEvil.Helpers
{
    /// <summary>
    /// Facilite l'utilisation des énumérations de bitmask.
    /// </summary>
    public static class EnumerationBitMaskHelper
    {
        /// <summary>
        /// Renvoit si le le bit "bitMaskBit" est actif dans le bitMask "bitMask".
        /// </summary>
        public static bool DoesBitMaskBitIsSetted<TBitMaskType>(TBitMaskType bitMask, TBitMaskType bitMaskBit) where TBitMaskType : struct
        {
            return ((int)(object)bitMask & (int)(object)bitMaskBit) != 0;
        }

        /// <summary>
        /// Active le bit "bitMaskBit" au bitMask "bitMask".
        /// </summary>
        public static void AddBitMaskBit<TBitMaskType>(ref TBitMaskType bitMask, TBitMaskType bitMaskBit) where TBitMaskType : struct
        {
            bitMask = (TBitMaskType)(object)((int)(object)bitMask | (int)(object)bitMaskBit);
        }

        /// <summary>
        /// Désactive le bit "bitMaskBit" au bitMask "bitMask".
        /// </summary>
        public static void RemoveBitMaskBit<TBitMaskType>(ref TBitMaskType bitMask, TBitMaskType bitMaskBit) where TBitMaskType : struct
        {
            bitMask = (TBitMaskType)(object)((int)(object)bitMask & (~(int)(object)bitMaskBit));
        }
    }
}
