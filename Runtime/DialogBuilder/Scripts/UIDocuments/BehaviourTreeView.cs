using System;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Nodes;
using DialogBuilder.Scripts.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Node = DialogBuilder.Scripts.Nodes.Node;

namespace DialogBuilder.Scripts.UIDocuments
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public Action<NodeView> OnNodeSelected;

        private BehaviourTree _tree;
        
        public BehaviourTreeView()
        { 
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Package/Runtime/DialogBuilder/Scripts/UIDocuments/BehaviourTreeEditor.uss");
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
        
        public void PopulateView(BehaviourTree tree)
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

            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", a => CreateNode(type));
            }
            
            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", a => CreateNode(type));
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

        private void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
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