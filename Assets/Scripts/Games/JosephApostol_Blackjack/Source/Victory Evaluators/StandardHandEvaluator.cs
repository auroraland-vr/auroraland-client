using UnityEngine;

namespace ProjectRenaissance.VictoryEvaluators
{
    public sealed class StandardHandEvaluator : IVictoryEvaluator
    {
        public int CheckVictory(int poolTotal, int playerTotal, bool playerNatural, int dealerTotal, bool dealerNatural)
        {
            if (playerTotal <= 21)
            {
                if (dealerNatural)
                {
                    if (playerNatural)
                        return 0;
                    else
                        return -poolTotal;
                }
                else if (playerNatural)
                    return Mathf.RoundToInt(poolTotal * 0.5f);
                else if (dealerTotal > 21)
                    return poolTotal;
                else
                {
                    if (playerTotal > dealerTotal)
                        return poolTotal;
                    else if (playerTotal < dealerTotal)
                        return -poolTotal;
                    else
                        return 0;
                }
            }

            return -poolTotal;
        }
    }
}