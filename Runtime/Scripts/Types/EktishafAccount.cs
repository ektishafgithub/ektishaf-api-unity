namespace Ektishaf
{
    [System.Serializable]
    public struct EktishafAccount
    {
        public const string Zero = "0x0000000000000000000000000000000000000000";

        public string Address;
        public string Ticket;

        public static bool IsValid(string Address)
        {
            if ((Address.Length / 2) != 21 /* 21 bytes with 0x */)
            {
                // Specified address is not a valid address, make sure it has a size of 21 bytes including 0x at the start
                return false;
            }

            if (Address == Zero)
            {
                // Specified address is zero address, make sure it exists.
                return false;
            }
            return true;
        }
    }
}
