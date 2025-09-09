using System;
using DialogBuilder.Scripts.Nodes.Decorator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogBuilder.Scripts.Core
{
    public class DialogOptionLabel : MonoBehaviour, IPointerClickHandler
    {
        public Action<PlayerDialogOption> Clicked;
        
        [SerializeField] private TMP_Text _textField;
        private PlayerDialogOption _node;
        
        public void Setup(PlayerDialogOption node)
        {
            _node = node;
            _textField.text = node.DialogLine;

            switch (node.Vibe)
            {
                case AnswerVibe.Positive:
                    _textField.color = Color.green;
                    break;
                case AnswerVibe.Negative:
                    _textField.color = Color.red;
                    break;
            }
        }

        public void ShowText()
        {
            _textField.alpha = 1f;
        }
        public void HideText()
        {
            _textField.alpha = 0f;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(_node);
        }
    }
}