using System.Collections.Generic;
using Nodes.Decorator;

namespace Nodes.Basic
{
    public class RootNode : Node
    {
        public Node Child;

        public override List<DialogOptionNode> GetChildNodes()
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