namespace SoNEAT.Mutations
{
    public interface IMutation
    {
        bool Mutate(ref Genome genome);
    }
}
