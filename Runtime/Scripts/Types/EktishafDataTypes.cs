using System;

namespace Ektishaf
{
    public enum ServOp
    {
        None,
        Register,
        Login,
        External,
        Reveal,
        Sign,
        Verify,
        Balance,
        ABI,
        Read,
        Write,
        Accounts,
        Send
    }

    [Serializable]
    public struct EktishafNetwork
    {
        public string NetworkName;
        public string Rpc;
        public string ChainId;
        public string CurrencySymbol;
        public string BlockExplorer;
    }

    [Serializable]
    public struct EktishafAccount
    {
        public string Address;
        public string Ticket;
    }

    [Serializable]
    public struct EktishafNft
    {
        public int Id;
        public int Amount;
        public string Uri;
    }
}