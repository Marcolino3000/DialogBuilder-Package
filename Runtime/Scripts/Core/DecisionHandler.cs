using System;
using System.Collections.Generic;
using System.Linq;
using Nodes.Decorator;
using UnityEngine;

namespace Core
{
    public class DecisionHandler : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;
        public void ShowDialogOptions(DialogOptionNode[] options)
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

        public void HideDialogOptions()
        {
            // throw new NotImplementedException();
        }

        public void TriggerIdleReaction()
        {
            // throw new NotImplementedException();
        }


        public DialogOptionType DialogOptionType => DialogOptionType.NPC;


        private NpcDialogOption HandleDecision(int popularity, DialogOptionNode[] options)
        {
            int smallestDifference = int.MaxValue;
            List<NpcDialogOption> closestAnswers = new();

            foreach (var option in options.Where(option => option is NpcDialogOption))
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