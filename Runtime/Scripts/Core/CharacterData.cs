using System;
using System.Collections.Generic;
using Nodes.Decorator;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu]
    public class CharacterData : ScriptableObject
    {
        public Sprite Icon;
        
        [Header("Runtime Data")]
        public int CurrentPopularity;
        
        [Header("Relationship Data")] 
        
        public List<PlayerDialogOption> OptionsToUnlockOnThreshold;
        public int TrustThreshold;
        [Range(-20, 20)]
        public int BasePopularity; //todo: durch enums ersetzen?
        [Range(0, 2)]
        public float Influenceability;


        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode)
                {
                    foreach (var option in OptionsToUnlockOnThreshold)
                    {
                        option.IsAvailable = false;
                    }
                }
            };
        }
        
        public void ApplyPopularityModifier(int modifier)
        {
            CurrentPopularity += modifier;

            if (!(CurrentPopularity > TrustThreshold))
                return;
            
            foreach (var option in OptionsToUnlockOnThreshold)
            {
                option.IsAvailable = true;
            }
        }
    }
}