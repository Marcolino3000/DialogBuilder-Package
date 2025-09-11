using System;
using Nodes.Decorator;
using Tree;
using UnityEngine;

namespace Nodes
{
    public abstract class Node : ScriptableObject
    {
        // public event Action<string> DialogLineChanged;
        
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public Blackboard Blackboard;
        [HideInInspector] public string DialogLine;
        [HideInInspector] public string TextPreview;

        public abstract DialogOptionNode[] GetChildNodes();

        private Action<string> OnDialogChanged;

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        // private void OnValidate()
        // {
        //     DialogLineChanged?.Invoke(DialogLine);
        //     OnDialogChanged?.Invoke(DialogLine);
        //     Debug.Log("OnValidate: " + DialogLine);
        // }

        public void OnCustomValidate()
        {
            // Debug.Log("OnCustomValidate: " + DialogLine);
        }

        public void RegisterCallback(Action<string> handleDialogLineChanged)
        {
            OnDialogChanged = handleDialogLineChanged;
        }
    }
}
