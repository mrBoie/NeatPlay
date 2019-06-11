using SoNEAT.InnovationPointGenerator;

namespace SoNEAT.Models
{
    public class Connection : IGene
    {
        public int InNodeId { get; set; }
        public int OutNodeId { get; set; }
        public double Weight { get; set; }
        public bool IsEnabled { get; set; }
        public int Id { get; set; }

        public Connection Copy(IInnovationPointGenerator innovationGenerator)
        {
            var copy = Copy();
            copy.Id = innovationGenerator.GetNextInnovation();
            return copy;
        }

        public Connection Copy()
        {
            return new Connection
            {
                InNodeId = InNodeId,
                OutNodeId = OutNodeId,
                Id = Id,
                IsEnabled = IsEnabled,
                Weight = Weight
            };
        }
    }
}
