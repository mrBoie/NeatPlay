namespace SoNEAT
{
    public class Node
    {
        public int Id { get; set; }
        public NodeType NodeType { get; set; }
        public Node Copy()
        {
            return new Node()
            {
                Id = Id,
                NodeType = NodeType
            };
        }
    }
}
