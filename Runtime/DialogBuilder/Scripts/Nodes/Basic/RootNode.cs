using DialogBuilder.Scripts.Nodes.Decorator;

namespace DialogBuilder.Scripts.Nodes.Basic
{
    public class RootNode : Node
    {
        public Node Child;

        public override DialogOptionNode[] GetChildNodes()
        {
            return null;
        }

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }

}