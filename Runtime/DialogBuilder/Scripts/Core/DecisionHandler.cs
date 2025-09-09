using System;
using System.Collections.Generic;
using System.Linq;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;

namespace DialogBuilder.Scripts.Core
{
    public class DecisionHandler : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;
        public void SetDialogOptions(DialogOptionNode[] options)
        {
            if (options.Any(option => option is not NpcDialogOption))
            {
                Debug.LogWarning("Some options are not NpcDialogOption type. Ignoring SetDialogOptions call.");
                return;
            }
            
            int popularity = 0; //todo!! get from data manager
            
            var selectedOption = HandleDecision(popularity, options);
            
            DialogOptionSelected?.Invoke(selectedOption);
        }

        public DialogOptionType DialogOptionType => DialogOptionType.NPC;


        private NpcDialogOption HandleDecision(int popularity, DialogOptionNode[] options)
        {
            int smallestDifference = int.MaxValue;
            List<NpcDialogOption> closestAnswers = new();

            foreach (var option in options)
            {
                if(option is not NpcDialogOption npcOption) continue;
                
                int difference = Math.Abs(popularity - npcOption.PopularityValue);
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    closestAnswers.Clear();
                    closestAnswers.Add(npcOption);
                }
                else if (difference == smallestDifference)
                {
                    closestAnswers.Add(npcOption);
                }
            }

            // Pick a random answer among the closest ones
            var randomIndex = UnityEngine.Random.Range(0, closestAnswers.Count);
            
            return closestAnswers[randomIndex];
        }
    }
        
    [Serializable]
    public class CharacterAnswer
    {
        public string Text;
        public int Popularity;

        public CharacterAnswer()
        {
            Text = "Default Answer";
            Popularity = 0;
        }
    }
}