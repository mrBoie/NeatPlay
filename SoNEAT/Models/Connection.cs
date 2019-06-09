namespace SoNEAT
{
    public class Connection
    {
        public int InNodeId { get; set; }
        public int OutNodeId { get; set; }
        public double Weight { get; set; }
        public bool IsEnabled { get; set; }
        public int InnovationPoint { get; set; }

        public Connection Copy(IInnovationPointGenerator innovationGenerator)
        {
            var copy = Copy();
            copy.InnovationPoint = innovationGenerator.GetNextInnovation();
            return copy;
        }

        public Connection Copy()
        {
            return new Connection
            {
                InNodeId = InNodeId,
                OutNodeId = OutNodeId,
                InnovationPoint = InnovationPoint,
                IsEnabled = IsEnabled,
                Weight = Weight
            };
        }
    }
}
