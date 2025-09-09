using System;
using DialogBuilder.Scripts.Core;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;

namespace DialogBuilder.Scripts
{
    public class MOCKNpcDialogCaller : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;
        public DialogOptionNode npcDialogOption;
        
        private void OnGUI()
        {
            if (GUILayout.Button("select npc-option"))
            {
                Debug.Log("npc selected option");
                DialogOptionSelected?.Invoke(npcDialogOption);
            }
        }
        public void SetDialogOptions(DialogOptionNode[] options)
        {
            // throw new NotImplementedException();
        }

        public DialogOptionType DialogOptionType { get; }
    }
}