using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Tree;
using UnityEngine;

namespace Nodes
{
    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        public Blackboard Blackboard;
        public string DialogLine;
        [HideInInspector] public string TextPreview;

        public abstract List<DialogOptionNode> GetChildNodes();

        private Action<string> OnDialogChanged;

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }
}
