using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Core;
using DialogBuilder.Scripts.Nodes;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;
using UnityEngine.Android;

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
        
        private bool _activateRandomPickWhenIdle;
        private float _timerUntilRandomPick = 2f;
        private Coroutine _currentIdleTimer;

        public void StartIdleTimer()
        {
            if (!isActiveAndEnabled) return;
            
            if (_currentIdleTimer != null)
                StopCoroutine(_currentIdleTimer);

            if (!_activateRandomPickWhenIdle)
                return;
            
            _currentIdleTimer = StartCoroutine(AwaitRandomPickTimer());
        }

        public void Setup(List<IDialogReceiver> receivers, List<IDialogOptionReceiver> presenters)
        {
            _dialogReceivers = receivers;
            _dialogPresenters = presenters;

            foreach (var presenter in presenters) 
                presenter.DialogOptionSelected += HandleOptionSelected;

            foreach (var receiver in receivers) 
                receiver.HideDialogLine();

            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
            
            ExecuteCurrentNodes();
        }

        public void Reset()
        {
            StopAllCoroutines();
            
            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
            
            ExecuteCurrentNodes();

            foreach (var receiver in _dialogReceivers)
                receiver.HideDialogLine();
        }

        public void SetOptionsForIdleRandomPick(bool activate, float time)
        {
            _activateRandomPickWhenIdle = activate;
            _timerUntilRandomPick = time;
        }

        private IEnumerator AwaitRandomPickTimer()
        {
            yield return new WaitForSeconds(_timerUntilRandomPick);
         
            foreach (var x in _dialogPresenters) 
                x.TriggerIdleReaction();
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
    
            StopAllCoroutines();
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
                .ForEach(p => p.ShowDialogOptions(CurrentNodes));
            
            if(_currentOptionType == DialogOptionType.Player && _activateRandomPickWhenIdle)
                StartIdleTimer();
        }
    }
}