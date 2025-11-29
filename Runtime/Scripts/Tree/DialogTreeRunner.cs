using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Nodes;
using Nodes.Decorator;
using UnityEngine;

namespace Tree
{
    public class DialogTreeRunner : MonoBehaviour
    {
        public static event Action<bool, DialogTree> OnDialogRunningStatusChanged;
        public event Action<DialogOptionNode> DialogNodeSelected;

        public bool IsDialogRunning;
        
        public DialogTree Tree;
        public List<DialogOptionNode> CurrentNodes;

        [SerializeField] private CharacterDataManager characterDataManager;

        private List<IDialogReceiver> _dialogReceivers;
        private List<IDialogOptionReceiver> _dialogPresenters;
        private List<IDialogStarter> _dialogStarters;
        private List<IDialogTreeSetter> _dialogTreeSetters;
        
        private DialogOptionType _currentOptionType; //dopplung entfernen -> durch type-field ersetzen
        private Type _currentType;
        private List<DialogOptionNode> _fallThroughNodes;
        
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

        public void Setup(List<IDialogReceiver> receivers, List<IDialogOptionReceiver> presenters, 
            List<IDialogStarter> starters, List<IDialogTreeSetter> treeSetters)
        {
            _dialogReceivers = receivers;
            _dialogPresenters = presenters;
            _dialogStarters = starters;
            _dialogTreeSetters = treeSetters;
            
            
            foreach (var presenter in presenters)
            {
                presenter.DialogOptionSelected += HandleOptionSelected;
                presenter.HideDialogOptions();
            }

            foreach (var receiver in receivers) 
                receiver.HideDialogLine();

            foreach (var starter in starters)
            {
                starter.OnStartDialog += ExecuteCurrentNodes;
                starter.OnStopDialog += Reset;
            }

            foreach (var setter in treeSetters)
            {
                setter.OnSetDialogTree += SetDialogTree;
            }

            _fallThroughNodes = new List<DialogOptionNode>();
            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
        }

        public void StartDialog()
        {
            ExecuteCurrentNodes();
        }

        public void Reset()
        {
            IsDialogRunning = false;
            OnDialogRunningStatusChanged?.Invoke(false, Tree);
            StopAllCoroutines();

            foreach (var receiver in _dialogReceivers)
                receiver.HideDialogLine();

            foreach (var presenter in _dialogPresenters) 
                presenter.HideDialogOptions();
            
            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
            // ExecuteCurrentNodes();
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
            if (dialogOption is PlayerDialogOption playerDialogOption)
            {
                if(Tree.Blackboard.CharacterData != null)
                    Tree.Blackboard.CharacterData.ApplyPopularityModifier(playerDialogOption.PopularityModifier);

                if (playerDialogOption.IsBondingOption)
                {
                    Tree.Blackboard.CharacterData.BondedWithPlayer = true;
                }

                else
                    Debug.LogWarning("CharacterData was null");
            }
            
    
            dialogOption.WasSelected = true;
            _fallThroughNodes.Remove(dialogOption);

            DialogNodeSelected?.Invoke(dialogOption);
            
            SaveFallThroughNodes();
            StopAllCoroutines();
            StartCoroutine(DisplayDialog(dialogOption));
        }

        private void SaveFallThroughNodes()
        {
            _fallThroughNodes ??= new List<DialogOptionNode>();

            foreach (var node in CurrentNodes)
            {
                if(node.FallThrough && !node.WasSelected)
                {
                    _fallThroughNodes.Add(node);
                }
            }
        }

        private IEnumerator DisplayDialog(DialogOptionNode dialogOption)
        {
            foreach (var paragraph in dialogOption.Paragraphs)
            {
                if (dialogOption is PlayerDialogOption)
                    yield return StartCoroutine(ShowParagraph("Marlene", paragraph));
                if (dialogOption is NpcDialogOption)
                {
                    yield return StartCoroutine(ShowParagraph(Tree.Blackboard.CharacterData.name, paragraph));
                }
            }

            GetNextNode(dialogOption);
        }

        private IEnumerator ShowParagraph(string characterName, Tuple<string, float> paragraph)
        {
            foreach (var receiver in _dialogReceivers)
            {
                receiver.DisplayDialogLine(characterName, paragraph.Item1);
            }
            
            yield return new WaitForSeconds(paragraph.Item2 * 1 / DialogBuilderHQ.dialogTextSpeed);

            foreach (var receiver in _dialogReceivers)
            {
                receiver.HideDialogLine();
            }
        }

        private void GetNextNode(Node currentNode)
        {
            CurrentNodes = SetOptionType(currentNode.GetChildNodes());
            
            FilterForConditions();
            
            ExecuteCurrentNodes();
        }

        private void FilterForConditions()
        {
            foreach (var node in CurrentNodes)
            {
                node.WasSelected = false;
            }
            
            CurrentNodes = CurrentNodes
                .Where(node => CheckConditions(node.RequiredNodes, node.BlockerNodes))
                .Where(CheckTrustLevel)
                .Where(CheckForBondingOption)
                .ToList();
        }

        private bool CheckForBondingOption(DialogOptionNode node)
        {
            if (!node.IsBondingOption)
                return true;
            
            if (Tree.Blackboard.CharacterData == null)
            {
                Debug.LogWarning("CharacterData was not Set in Blackboard. Bonding-option was not checked");
                return true;
            }

            return Tree.Blackboard.CharacterData.TrustsPlayer;

        }

        private bool CheckTrustLevel(DialogOptionNode node)
        {
            if (!node.IsTrustOption)
                return true;

            if (Tree.Blackboard.CharacterData == null)
            {
                Debug.LogWarning("CharacterData was not Set in Blackboard. Trust-level was not checked");
                return true;
            }

            return Tree.Blackboard.CharacterData.TrustsPlayer;
        }

        private bool CheckConditions(List<DialogOptionNode> requiredNodes, List<DialogOptionNode> blockerNodes)
        {
            bool requiredFulfilled = false;
            bool blockerFulfilled = false;
            
            if (blockerNodes.IsNullOrEmpty() && requiredNodes.IsNullOrEmpty())
                return true;
            
            if(requiredNodes.IsNullOrEmpty())
                requiredFulfilled = true;

            else
            {
                if (requiredNodes.TrueForAll(n => n.WasSelected))
                    requiredFulfilled = true;
            }

            if(blockerNodes.IsNullOrEmpty())
                blockerFulfilled = true;
            
            else
            {
                if (!blockerNodes.Any(n => n.WasSelected))
                    blockerFulfilled = true;
            }
            
            return requiredFulfilled && blockerFulfilled;
        }

        private List<DialogOptionNode> SetOptionType(List<DialogOptionNode> nodes)
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
            
            if(!nodes.IsNullOrEmpty())
                _currentType = nodes.First().GetType();
            
            return nodes;
        }

        private void SetDialogTree(DialogTree tree)
        {
            Tree = tree;
            CurrentNodes = SetOptionType(Tree.GetStartingNodes());
            _fallThroughNodes = new List<DialogOptionNode>();
        }
        
        private void ExecuteCurrentNodes()
        {
            if(CurrentNodes == null || CurrentNodes.Count == 0)
            {
                Debug.LogWarning("No more nodes to execute.");
                OnDialogRunningStatusChanged?.Invoke(false, Tree);
                IsDialogRunning = false;
                return;
            }
            
            OnDialogRunningStatusChanged?.Invoke(true, Tree);
            IsDialogRunning = true;

            var options = GetCurrentAndFallThroughOptions();
            
            if(options.Length == 0)
            {
                Debug.LogWarning("No available dialog options.");
                return;
            }
            
            var nodesOfCurrentType = _dialogPresenters.FindAll(p => p.DialogOptionType == _currentOptionType);
            
            foreach (var node in nodesOfCurrentType)
            {
                node.ShowDialogOptions(options);
            }
            
                // .ForEach(p => p.ShowDialogOptions(options));
            
            if(_currentOptionType == DialogOptionType.Player && _activateRandomPickWhenIdle)
                StartIdleTimer();
        }

        private DialogOptionNode[] GetCurrentAndFallThroughOptions()
        {
            var fallThrough = _fallThroughNodes.Where(n => n.GetType() == _currentType).ToList();
            var allOptions = CurrentNodes.Concat(fallThrough);
            
            return allOptions.Where(n => n.IsAvailable).ToArray();
        }
    }
    
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}