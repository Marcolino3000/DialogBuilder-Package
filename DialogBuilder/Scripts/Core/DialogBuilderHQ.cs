using System;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Tree;
using UnityEngine;

namespace DialogBuilder.Scripts.Core
{
    public class DialogBuilderHQ : MonoBehaviour
    {
        [Header("Characters")]
        [SerializeField] private List<CharacterData> characters;

        [Header("Settings")] 
        [SerializeField] private bool showSubtitles;
        [SerializeField] private bool showOptions;
        [Tooltip("Pick a random option when no option is selected within time span.")]
        [SerializeField] private bool randomPick;
        [SerializeField] private float timerUntilRandomPick = 5f;
        
        [Header("Internal References")]
        [SerializeField] private DialogOptionPresenter dialogOptionPresenter;
        [SerializeField] private GameObject dialogOptionContainer;
        [SerializeField] private DecisionHandler decisionHandler;
        [SerializeField] private SubtitlePresenter subtitlePresenter;
        [SerializeField] private DialogTreeRunner treeRunner;

        private void Start()
        {
            FindObjectsImplementingDialogInterfaces();
        }

        private void FindObjectsImplementingDialogInterfaces()
        {
            var clients = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDialogInterface>().ToArray();
            
            List<IDialogReceiver> receivers = new();
            List<IDialogOptionReceiver> presenters = new();

            if (clients.Length == 0)
            {
                Debug.Log("No objects found implementing a dialog interface.");
                return;
            }

            foreach (var client in clients)
            {
                switch (client) 
                {
                    case IDialogOptionReceiver presenter:
                        presenters.Add(presenter);
                        break;
                    
                    case IDialogReceiver textReceiver:
                        receivers.Add(textReceiver);
                        break;
                    
                    default: Debug.LogWarning("Interface not implemented: " + client.GetType().Name); 
                        break;
                }
            }
            
            treeRunner.Setup(receivers, presenters);
        }
        
        private void OnValidate()
        {
            if(subtitlePresenter != null)
            {
                subtitlePresenter.gameObject.SetActive(showSubtitles);
            }
            
            if(dialogOptionPresenter != null)
            {
                dialogOptionPresenter.gameObject.SetActive(showOptions);
                dialogOptionContainer.SetActive(showOptions);
            }
            
            dialogOptionPresenter.SetRandomPickOptions(randomPick, timerUntilRandomPick);
            dialogOptionPresenter.StartIdleTimer();
        }
    }
}