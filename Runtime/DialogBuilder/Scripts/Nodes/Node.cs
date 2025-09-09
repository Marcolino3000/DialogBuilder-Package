using DialogBuilder.Scripts.Nodes.Decorator;
using DialogBuilder.Scripts.Tree;
using UnityEngine;

namespace DialogBuilder.Scripts.Nodes
{
    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public Blackboard Blackboard;
        [TextArea] public string DialogLine;

        public abstract DialogOptionNode[] GetChildNodes();

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }
}
