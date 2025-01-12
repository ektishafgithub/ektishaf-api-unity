using System.Numerics;
using UnityEngine;

namespace Ektishaf
{
    public class EktishafMathHelper : MonoBehaviour
    {
        public static BigInteger ParseEther(string ether)
        {
            if (decimal.TryParse(ether, out decimal value))
            {
                decimal wei = value * 1_000_000_000_000_000_000M; // 10^18
                return new BigInteger(wei);
            }
            return BigInteger.Zero;
        }

        public static string ParseWei(BigInteger wei)
        {
            decimal value = (decimal)wei;
            decimal ether = value / 1_000_000_000_000_000_000M; // 10^18
            return ether.ToString("F18");
        }
    }
}
