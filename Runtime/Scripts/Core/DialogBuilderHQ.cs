using System.Collections.Generic;
using System.Linq;
using Tree;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
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
        
        [HideInInspector] [SerializeField] private DialogOptionPresenter dialogOptionPresenter;
        [HideInInspector] [SerializeField] private GameObject dialogOptionContainer;
        [HideInInspector] [SerializeField] private DecisionHandler decisionHandler;
        [HideInInspector] [SerializeField] private SubtitlePresenter subtitlePresenter;
        [HideInInspector] [SerializeField] private DialogTreeRunner treeRunner;
        
        
        private void Start()
        {
            FindClients();
            FindEventSystem();
        }
        
        private void FindClients()
        {
            var clients = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDialogInterface>().ToArray();
            
            List<IDialogReceiver> receivers = new();
            List<IDialogOptionReceiver> presenters = new();
            List<IDialogStarter> starters = new();
            List<IDialogTreeSetter> treeSetters = new();

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
                    
                    case IDialogStarter starter:
                        starters.Add(starter);
                        break;
                    
                    case IDialogTreeSetter treeSetter:
                        treeSetters.Add(treeSetter);
                        break;
                    
                    default: Debug.LogWarning("Interface not implemented: " + client.GetType().Name); 
                        break;
                }
            }
            
            treeRunner.Setup(receivers, presenters, starters, treeSetters);
        }

        private void FindEventSystem()
        {
            if (Application.isPlaying)
            {
                if (EventSystem.current != null) 
                    return;
            
                var eventSystem = new GameObject("[TEMP] EventSystem - added by DialogBuilder");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                
                Debug.LogWarning("Added EventSystem because none was found in the scene.");
            }
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
                // dialogOptionContainer.SetActive(showOptions);
            }
            
            treeRunner.SetOptionsForIdleRandomPick(randomPick, timerUntilRandomPick);
            treeRunner.StartIdleTimer();
        }
        
        private void OnGUI()
         {
             if (GUILayout.Button("Reset Dialog")) 
                 treeRunner.Reset();
         }
    }
}