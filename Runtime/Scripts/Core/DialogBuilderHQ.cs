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
        public static float dialogTextSpeed = 1f;
        [SerializeField] public float _dialogTextSpeed = 1f;
        private float test;
        
        [Tooltip("Pick a random option when no option is selected within time span.")]
        [SerializeField] private bool randomPick;
        [SerializeField] private float timerUntilRandomPick = 5f;
        
        [SerializeField] private DialogOptionPresenter dialogOptionPresenter;
        // [SerializeField] private GameObject dialogOptionContainer;
        [SerializeField] private DecisionHandler decisionHandler;
        [SerializeField] private SubtitlePresenter subtitlePresenter;
        [SerializeField] private DialogTreeRunner treeRunner;
        
        
        private void Start()
        {
            FindClients();
            FindEventSystem();
        }
        
        private void FindClients()
        {
            var monos = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDialogInterface>().ToArray();
            
            var scriptables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<IDialogInterface>().ToArray();
            
            var clients = monos.Concat(scriptables).ToArray();
            
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
                // clients might implement multiple interfaces
                
                if (client is IDialogOptionReceiver presenter)
                {
                    presenters.Add(presenter);
                }
                if (client is IDialogReceiver textReceiver)
                {
                    receivers.Add(textReceiver);
                }
                if (client is IDialogStarter starter)
                {
                    starters.Add(starter);
                }
                if (client is IDialogTreeSetter treeSetter)
                {
                    treeSetters.Add(treeSetter);
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
            }
            
            treeRunner.SetOptionsForIdleRandomPick(randomPick, timerUntilRandomPick);
            treeRunner.StartIdleTimer();
            
            dialogTextSpeed = _dialogTextSpeed;
        }
        
        private void OnGUI()
         {
             dialogTextSpeed = GUILayout.HorizontalSlider(dialogTextSpeed, 0.2f, 5.0f);
             
             if (GUILayout.Button("Reset Dialog")) 
                 treeRunner.Reset();

             if(GUILayout.Button("Start Dialog"))
                 treeRunner.StartDialog();
         }
    }
}