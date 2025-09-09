using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Core;
using DialogBuilder.Scripts.Nodes;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;

namespace DialogBuilder.Scripts.Tree
{
    public class DialogTreeRunner : MonoBehaviour
    {
        public BehaviourTree Tree;
        public DialogOptionNode[] CurrentNodes;

        [SerializeField] private DataManager _dataManager;

        private List<IDialogReceiver> _dialogReceivers;
        private List<IDialogOptionReceiver> _dialogPresenters;
        private DialogOptionType _currentOptionType;
        
        public void Setup(List<IDialogReceiver> receivers, List<IDialogOptionReceiver> presenters)
        {
            _dialogReceivers = receivers;
            _dialogPresenters = presenters;

            foreach (var presenter in presenters)
            {
                presenter.DialogOptionSelected += HandleOptionSelected;
            }

            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
            
            ExecuteCurrentNodes();
        }

        private void HandleOptionSelected(DialogOptionNode dialogOption)
        {
            //todo: movo to dataManager-module
            if (dialogOption is PlayerDialogOption playerDialogOption)
            {
                _dataManager.AddPlayerDialogChoiceEffects(
                    new PlayerDialogChoiceEffects
                    {
                        PopularityModifier = playerDialogOption.PopularityModifier
                    });    
            }
            
            StartCoroutine(DisplayDialog(dialogOption));
        }

        private IEnumerator DisplayDialog(DialogOptionNode dialogOption)
        {
            foreach (var paragraph in dialogOption.Paragraphs)
            {
                if (dialogOption is PlayerDialogOption)
                    yield return StartCoroutine(ShowParagraph("Marlene", paragraph));
                if (dialogOption is NpcDialogOption)
                    yield return StartCoroutine(ShowParagraph(_dataManager.CurrentCharacter.name, paragraph));
            }

            GetNextNode(dialogOption);
        }

        private IEnumerator ShowParagraph(string characterName, Tuple<string, float> paragraph)
        {
            foreach (var receiver in _dialogReceivers)
            {
                receiver.DisplayDialogLine(characterName, paragraph.Item1);
            }
            
            yield return new WaitForSeconds(paragraph.Item2);

            foreach (var receiver in _dialogReceivers)
            {
                receiver.HideDialogLine();
            }
        }

        private void GetNextNode(Node currentNode)
        {
            CurrentNodes = SetOptionType(currentNode.GetChildNodes());
            
            ExecuteCurrentNodes();
        }

        private DialogOptionNode[] SetOptionType(DialogOptionNode[] nodes)
        {
            if(nodes.All(n => n.OptionType == DialogOptionType.NPC))
                _currentOptionType = DialogOptionType.NPC;
            else if(nodes.All(n => n.OptionType == DialogOptionType.Player))
                _currentOptionType = DialogOptionType.Player;
            else
            {
                Debug.LogError("Next nodes have mixed types. This is not supported and should not happen.");
                return null;
            }

            return nodes;
        }

        private void ExecuteCurrentNodes()
        {
            if(CurrentNodes == null || CurrentNodes.Length == 0)
            {
                Debug.LogWarning("No more nodes to execute.");
                return;
            }
            
            //todo:            
            _dialogPresenters.
                FindAll(p => p.DialogOptionType == _currentOptionType)
                .ForEach(p => p.SetDialogOptions(CurrentNodes));
        }
    }
}