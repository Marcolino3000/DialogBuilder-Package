using UnityEditor;
using UnityEngine.iOS;
using UnityEngine.UIElements;

namespace DialogBuilder.Scripts.UIDocuments
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private Editor _editor;
        
        public InspectorView()
        {
            
        }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = (DialogNodeInspector)  Editor.CreateEditor(nodeView.Node, typeof(DialogNodeInspector));
            // _editor = Editor.CreateEditor(nodeView.Node);
            
            // IMGUIContainer container = new IMGUIContainer(() =>
            // {
            //     if(_editor.target)
            //         _editor.OnInspectorGUI();
            // });
            // Add(container);
            
            VisualElement inspectorGUI = _editor.CreateInspectorGUI();
            Add(inspectorGUI);
        }
    }
}