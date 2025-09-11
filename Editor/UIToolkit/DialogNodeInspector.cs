using System;
using System.Linq;
using Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.UIToolkit
{
    [CustomEditor(typeof(Node))]
    public class DialogNodeInspector : UnityEditor.Editor
    {
        public ushort maxPreviewLength = 16;
        
     public override VisualElement CreateInspectorGUI()
     {
         var root = new VisualElement();
         
         var el = new InspectorElement(serializedObject);
         root.Add(el);
         
         var DialogTextFieldLabel = new Label("Dialog Text:");
         var DialogText = new TextField()
         {
             multiline = true,
         };

         DialogText.style.whiteSpace = WhiteSpace.Normal;
         DialogText.bindingPath = "DialogLine";
         DialogText.Bind(serializedObject);
         DialogText.RegisterValueChangedCallback(HandleDialogTextChanged);
         
         root.Add(DialogTextFieldLabel);
         root.Add(DialogText);

         return root;
     }

     private void HandleDialogTextChanged(ChangeEvent<string> evt)
     {
         var previewText = CreatePreview(evt.newValue);
         serializedObject.FindProperty("TextPreview").stringValue = previewText;
         serializedObject.ApplyModifiedProperties();
     }

     private string CreatePreview(string newText)
     {
         if (string.IsNullOrWhiteSpace(newText))
             return string.Empty;
         
         var shortenedText = newText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).First(); 
         shortenedText = shortenedText.Substring(0, Math.Min(shortenedText.Length, maxPreviewLength));
         
         if (shortenedText.Length == maxPreviewLength)
             shortenedText += "...";
         
         return shortenedText;
     }
    }
}