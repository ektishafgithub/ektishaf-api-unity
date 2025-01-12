namespace Ektishaf
{
    [System.Serializable]
    public struct EktishafNetwork
    {
        public string NetworkName;
        public string Rpc;
        public string ChainId;
        public string CurrencySymbol;
        public string BlockExplorer;

        public static bool IsValid(EktishafNetwork Network)
        {
            if (string.IsNullOrEmpty(Network.Rpc.Trim()) || string.IsNullOrEmpty(Network.ChainId.Trim())  || string.IsNullOrEmpty(Network.CurrencySymbol.Trim()))
            {
                // Specified network is not a valid network, make sure it has at least Rpc, ChainId and CurrencySymbol information
                return false;
            }
            return true;
        }
    }
}
