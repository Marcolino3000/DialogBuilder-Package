using System.Collections.Generic;
using UnityEngine;

namespace DialogBuilder.Scripts.Nodes
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] 
        public List<Node> Children = new();
        
        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}