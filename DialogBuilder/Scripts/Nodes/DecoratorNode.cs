using UnityEngine;

namespace DialogBuilder.Scripts.Nodes
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] 
        public Node Child;
        
        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}