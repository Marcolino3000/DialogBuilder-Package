using UnityEngine.UIElements;

namespace Editor.UIToolkit
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private UnityEditor.Editor _editor;
        
        public InspectorView()
        {
            
        }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = (DialogNodeInspector)  UnityEditor.Editor.CreateEditor(nodeView.Node, typeof(DialogNodeInspector));
            
            VisualElement inspectorGUI = _editor.CreateInspectorGUI();
            Add(inspectorGUI);
        }
    }
}