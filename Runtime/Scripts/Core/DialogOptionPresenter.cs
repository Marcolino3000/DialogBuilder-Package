using System;
using System.Collections.Generic;
using System.Linq;
using Nodes.Decorator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class DialogOptionPresenter : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;

        public DialogOptionType DialogOptionType => DialogOptionType.Player;

        [SerializeField] private GameObject _dialogOptionContainer;
        [SerializeField] private PlayerDialogOption[] _currentNodes;

        private List<DialogOptionLabel> _optionLabels = new();

        public void TriggerIdleReaction()
        {
            OnOptionSelected(_currentNodes[Random.Range(0, _currentNodes.Length)]);
            HideDialogOptions();
        }

        private void Awake()
        {
            _optionLabels = _dialogOptionContainer.GetComponentsInChildren<DialogOptionLabel>().ToList();

            foreach (var label in _optionLabels)
            {
                label.Clicked += OnOptionSelected;
            }
        }

        public void ShowDialogOptions(DialogOptionNode[] options)
        {
            if (!options.All(option => option is PlayerDialogOption))
            {
                Debug.LogWarning("Some options are not PlayerDialogOption type. Ignoring SetDialogOptions call.");
                return;
            }

            _currentNodes = options.Cast<PlayerDialogOption>().ToArray();

            HideDialogOptions();

            var labels = _optionLabels.GetEnumerator();
            var opts = _currentNodes.ToList().GetEnumerator();

            while (labels.MoveNext() && opts.MoveNext())
            {
                SetupDialogOption(labels.Current, opts.Current);
            }
        }

        private void SetupDialogOption(DialogOptionLabel labelsCurrent, PlayerDialogOption optsCurrent)
        {
            labelsCurrent.ShowText();
            labelsCurrent.Setup(optsCurrent);
        }

        private void HideDialogOptions()
        {
            foreach (var label in _optionLabels) 
                label.HideText();
        }

        private void OnOptionSelected(PlayerDialogOption option)
        {
            DialogOptionSelected?.Invoke(option);
            HideDialogOptions();
        }
    }
}