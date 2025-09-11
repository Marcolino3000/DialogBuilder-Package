using System;
using Nodes;
using Nodes.Basic;
using Nodes.Decorator;
using Nodes.Utility;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Nodes.Node;

namespace Editor.UIToolkit
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node Node;
        public CustomPort InputPort;
        public CustomPort OutputPort;
        public NodeView(Node node) : base("Packages/com.cod.dialog-builder/Editor/UIToolkit/NodeView.uxml")
        {
            Node = node;
            viewDataKey = node.Guid;

            style.left = node.Position.x;
            style.top = node.Position.y;
            style.scale = new Vector2(1.4f, 1.4f);
            style.width = 200;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            
            var textField = this.Q<Label>("title-label");
            textField.bindingPath = "TextPreview";
            textField.Bind(new SerializedObject(node));
        }

        private void SetupClasses()
        {
            if (Node is RootNode)
            {
                AddToClassList("root");
            }
            
            else if (Node is ActionNode)
            {
                AddToClassList("action");
            }
            
            else if (Node is CompositeNode)
            {
                AddToClassList("composite");
            }

            else if (Node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            
            if (Node is DialogOptionNode)
            {
                AddToClassList("textNode");
            }
        }

        private void CreateInputPorts()
        {
            if (Node is RootNode) { }
            
            else if (Node is ActionNode)
            {
                InputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            
            else if (Node is CompositeNode)
            {
                InputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }

            else if (Node is DecoratorNode)
            {
                InputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (InputPort != null)
            {
                InputPort.portName = "";
                InputPort.style.flexDirection = FlexDirection.Column;
                InputPort.style.paddingLeft = 40;
                InputPort.style.paddingRight = 40;
                InputPort.style.height = 15;
                inputContainer.Add(InputPort);
            }
        }

        private void CreateOutputPorts()
        {
            if (Node is RootNode)
            {
                OutputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
         
            }
            
            else if (Node is ActionNode) { }
            
            else if (Node is CompositeNode)
            {
                OutputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            else if (Node is DecoratorNode)
            {
                OutputPort = CustomPort.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (OutputPort != null)
            {
                OutputPort.portName = "";
                OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
                OutputPort.style.paddingLeft = 40;
                OutputPort.style.paddingRight = 40;
                OutputPort.style.height = 15;
                outputContainer.Add(OutputPort);
            }
        }

        public override void SetPosition(Rect newPosition)
        {
            base.SetPosition(newPosition);
            Undo.RecordObject(Node, "Behaviour Tree (Set Node Position) ");
            Node.Position.x = newPosition.xMin;
            Node.Position.y = newPosition.yMin;
            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public void SortChildren()
        {
            CompositeNode composite = Node as CompositeNode;
            if (composite)
            {
                composite.Children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            if (Node is not PlayerDialogOption playerDialogOption)
                return;

            RemoveFromClassList("neutral");
            RemoveFromClassList("negative");
            RemoveFromClassList("positive");
            
            switch (playerDialogOption.Vibe)
            {
                case AnswerVibe.Neutral:
                    AddToClassList("neutral");
                    break;
                case AnswerVibe.Negative:
                    AddToClassList("negative");
                    break;
                case AnswerVibe.Positive:
                    AddToClassList("positive");
                    break;
            }
        }
        
        
    }
}