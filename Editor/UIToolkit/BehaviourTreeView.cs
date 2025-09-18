using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Nodes;
using Nodes.Basic;
using Nodes.Decorator;
using Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Nodes.Node;
using Vector2 = UnityEngine.Vector2;

namespace Editor.UIToolkit
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public Action<NodeView> OnNodeSelected;

        private DialogTree _tree;

        private Vector2 GetLocalMousePosition(Vector2 worldMousePosition)
        {
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            
            return localMousePosition;
        }
        
        public BehaviourTreeView()
        { 
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"Packages/com.cod.dialog-builder/Editor/UIToolkit/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);
            
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            PopulateView(_tree);
            AssetDatabase.SaveAssets();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }
        
        public void PopulateView(DialogTree tree)
        {
            _tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // if (_tree.RootNode == null)
            // {
            //     _tree.RootNode = _tree.CreateNode(typeof(RootNode)) as RootNode;
            //     EditorUtility.SetDirty(tree);
            //     AssetDatabase.SaveAssets();
            // }
            
            //creates node view
            tree.nodes.ForEach(n => CreateNodeView(n));
            
            //creates edges
            tree.nodes.ForEach(parent =>
            {
                var children = tree.GetChildren(parent);
                children.ForEach(child =>
                {
                    NodeView parentView = FindNodeView(parent);
                    NodeView childView = FindNodeView(child);
                    childView.Focus();
                    Edge edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                    AddElement(edge);
                });
            });
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(e =>
                {
                    NodeView nodeView = e as NodeView;
                    if (nodeView != null)
                    {
                        _tree.DeleteNode(nodeView.Node);
                    }
                    
                    Edge edge = e as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        _tree.RemoveChild(parentView.Node, childView.Node);
                    }
                });
            }
            
            if (graphviewchange.edgesToCreate != null)
            {
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.AddChild(parentView.Node, childView.Node);
                });
            }

            if (graphviewchange.movedElements != null)
            {
                nodes.ForEach(n =>
                {
                    NodeView view = n as NodeView;
                    view?.SortChildren();
                });
            }
            
            return graphviewchange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);

            NodeView target = evt.target as NodeView;
            
            evt.menu.AppendAction("Add as start Node", a =>
            {
                if (target != null)
                {
                    _tree.AddStartNode(target.Node as DialogOptionNode);
                }
            }, a => target != null && target.Node is DialogOptionNode ? 
                DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            
            var mousePosition = GetLocalMousePosition(evt.mousePosition) + new Vector2(150f, 100f);
            
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", a => CreateNode(type));
            }
            
            types = TypeCache.GetTypesDerivedFrom<DialogOptionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", a => CreateNode(type, mousePosition));
            }
            
            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", a => CreateNode(type));
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => 
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node
                ).ToList();
        }

        private void CreateNode(Type type)
        {
            Node node = _tree.CreateNode(type);
            CreateNodeView(node);
        }
        
        private void CreateNode(Type type, Vector2 mousePosition)
        {
            Node node = _tree.CreateNode(type);
            CreateNodeView(node, mousePosition);
        }

        private void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }
        
        private void CreateNodeView(Node node, Vector2 mousePosition)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.SetPosition(new Rect(mousePosition, Vector2.zero));
            AddElement(nodeView);
            Debug.Log("create node view at " + mousePosition);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView nodeView = n as NodeView;
                nodeView.UpdateState();
            });
        }
    }
}