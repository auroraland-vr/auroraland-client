namespace ProjectRenaissance.VictoryEvaluators
{
    public interface IVictoryEvaluator
    {
        int CheckVictory(int poolTotal, int playerTotal, bool playerNatural, int dealerTotal, bool dealerNatural);
    }
}