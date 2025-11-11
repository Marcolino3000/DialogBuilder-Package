using System;
using Nodes.Decorator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class DialogOptionLabel : MonoBehaviour, IPointerClickHandler
    {
        public Action<PlayerDialogOption> Clicked;
        
        [SerializeField] private TMP_Text _textField;
        
        private PlayerDialogOption _node;
        private bool _isHidden;
        
        public void Setup(PlayerDialogOption node)
        {
            _node = node;
            _textField.text = node.DialogLine;

            switch (node.Type)
            {
                case AnswerType.DeepTalk:
                    _textField.color = Color.green;
                    break;
                case AnswerType.TrashTalk:
                    _textField.color = Color.red;
                    break;
                case AnswerType.SmallTalk:
                    _textField.color = Color.yellow;
                    break;
            }
        }

        public void ShowText()
        {
            _textField.alpha = 1f;
            _isHidden = false;
        }
        public void HideText()
        {
            _textField.alpha = 0f;
            _isHidden = true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_textField.text))
            {
                Debug.LogWarning("DialogBuilder: Text field is empty, but DialogOptionLabel is still clickable. Ignoring click.");
                return;
            }
            
            if (_isHidden) 
                return;
            
            Clicked?.Invoke(_node);
        }
    }
}