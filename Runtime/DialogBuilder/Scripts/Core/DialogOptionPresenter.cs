using System;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogBuilder.Scripts.Core
{
    public class DialogOptionPresenter : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;
        public DialogOptionType DialogOptionType => DialogOptionType.Player;
        
        [SerializeField] private GameObject _dialogOptionContainer;
        [SerializeField] private PlayerDialogOption[] _currentNodes;

        private bool _activateRandomPickWhenIdle;
        private float _timerUntilRandomPick;
        private float _currentTime;
        private bool _displayTimerStarted;
        
        private List<DialogOptionLabel> _optionLabels = new();

        public void SetRandomPickOptions(bool activate, float time)
        {
            _activateRandomPickWhenIdle = activate;
            _timerUntilRandomPick = time;
        }
        
        public void StartIdleTimer()
        {
            StopTimer();
            _displayTimerStarted = true;
        }

        private void Awake()
        {
            _optionLabels = _dialogOptionContainer.GetComponentsInChildren<DialogOptionLabel>().ToList();

            foreach (var label in _optionLabels)
            {
                label.Clicked += OnOptionSelected;
            }
        }

        private void Update()
        {
            UpdateIdleTimer();
        }

        private void UpdateIdleTimer()
        {
            if(!_displayTimerStarted) return;
            
            if (_currentTime < _timerUntilRandomPick)
            {
                _currentTime += Time.deltaTime;
            }

            else
            {
                if (_activateRandomPickWhenIdle)
                {
                    OnOptionSelected(_currentNodes[Random.Range(0, _optionLabels.Count)]);
                    HideDialogOptions();
                }
                
                StopTimer();
            }
        }

        private void StopTimer()
        {
            _displayTimerStarted = false;
            _currentTime = 0f;
        }

        public void SetDialogOptions(DialogOptionNode[] options)
        {
            if (!options.All(option => option is PlayerDialogOption))
            {
                Debug.LogWarning("Some options are not PlayerDialogOption type. Ignoring SetDialogOptions call.");
                return;
            }

            _currentNodes = options.Cast<PlayerDialogOption>().ToArray();

            StopTimer();
            HideDialogOptions();

            var labels = _optionLabels.GetEnumerator();
            var opts = _currentNodes.ToList().GetEnumerator();

            while (labels.MoveNext() && opts.MoveNext())
            {
                SetupDialogOption(labels.Current, opts.Current);
            }

            _displayTimerStarted = true;
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
            StopTimer();
            HideDialogOptions();
        }
    }
}