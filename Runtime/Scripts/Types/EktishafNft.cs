using System.Collections.Generic;
using System.Numerics;

namespace Ektishaf
{
    [System.Serializable]
    public struct EktishafNft
    {
        public string Owner;
        public int Id;
        public int Amount;
        public string Metadata;
        public bool ListForSale;
        public BigInteger ListPrice;
        public int ListAmount;

        public static EktishafNft FromResponse(List<string> nft)
        {
            return new EktishafNft()
            {
                Owner = nft[0],
                Id = int.Parse(nft[1]),
                Amount = int.Parse(nft[2]),
                Metadata = nft[3],
                ListForSale = bool.Parse(nft[4]),
                ListPrice = BigInteger.Parse(nft[5]),
                ListAmount = int.Parse(nft[6])
            };
        }

        public static bool HasZeroTokens(List<string> nft)
        {
            return int.Parse(nft[2]) == 0;
        }

        public static bool IsListForSale(List<string> nft)
        {
            return bool.Parse(nft[4]) == true;
        }
    }
}
