namespace ProjectRenaissance.VictoryEvaluators
{
    public sealed class InsuranceEvaluator : IVictoryEvaluator
    {
        public int CheckVictory(int poolTotal, int playerTotal, bool playerNatural, int dealerTotal, bool dealerNatural)
        {
            if (dealerNatural)
                return poolTotal;

            return -poolTotal;
        }
    }
}