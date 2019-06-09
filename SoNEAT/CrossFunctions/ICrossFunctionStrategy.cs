namespace SoNEAT.CrossFunctions
{
    public interface ICrossFunctionStrategy
    {
        Genome GenerateOffspring(Genome moreFitParent, Genome lessFitParent);
    }
}
