using System;
using System.Collections.Generic;
using Nodes;
using Nodes.Basic;
using Nodes.Decorator;
using UnityEditor;
using UnityEngine;

namespace Tree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public DialogOptionNode[] RootNodes;
        public List<Node> nodes = new();
        public Blackboard Blackboard = new();

        public Node CreateNode(Type type)
        {
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();
            
            Undo.RecordObject(this, "Behaviour Tree (Create Node)");
            nodes.Add(node);

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);    
            
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
            
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
            nodes.Remove(node);
            
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            
            AssetDatabase.SaveAssets();
   
        }

        public void AddChild(Node parent, Node child)
        {
            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (Add Child)");
                rootNode.Child = child;
                EditorUtility.SetDirty(rootNode);
            }
            
            if(parent is DecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (Add Child)");
                decoratorNode.Child = child;
                EditorUtility.SetDirty(decoratorNode);
            }

            if (parent is CompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (Add Child)");
                compositeNode.Children.Add(child);
                EditorUtility.SetDirty(compositeNode);
            }
        }
        
        public void RemoveChild(Node parent, Node child)
        {
            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (Remove Child)");
                rootNode.Child = null;    
                EditorUtility.SetDirty(rootNode);
            }
            
            if(parent is DecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (Remove Child)");
                decoratorNode.Child = null;
                EditorUtility.SetDirty(decoratorNode);
            }

            if (parent is CompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (Remove Child)");
                compositeNode.Children.Remove(child);
                EditorUtility.SetDirty(compositeNode);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new();

            if (parent is RootNode rootNode)
            {
                if(rootNode.Child != null)
                    children.Add(rootNode.Child);
            }

            if (parent is DecoratorNode decoratorNode)
            {
                if (decoratorNode.Child != null)
                    children.Add(decoratorNode.Child);
            }

            if (parent is CompositeNode compositeNode)
            {
                return compositeNode.Children;
            }

            return children;
        }

        // public void Traverse(Node node, System.Action<Node> visitor)
        // {
        //     if (node)
        //     {
        //         visitor.Invoke(node);
        //         var children = GetChildren(node);
        //         children.ForEach(n => Traverse(n ,visitor));
        //     }
        // }
        
        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            // tree.RootNodes = tree.RootNodes.Clone();
            // tree.nodes = new List<Node>();
            //
            // Traverse(tree.RootNodes, n =>
            // {
            //     tree.nodes.Add(n); 
            // });
            
            return tree;
        }
        
        public DialogOptionNode[] GetStartingNodes()
        {
            if (RootNodes == null || RootNodes.Length == 0 || RootNodes[0] is not DialogOptionNode)
            {
                Debug.LogWarning("No fitting root node found. Returning empty array.");
                return Array.Empty<DialogOptionNode>();
            }
            
            return RootNodes;
        }
    }
}