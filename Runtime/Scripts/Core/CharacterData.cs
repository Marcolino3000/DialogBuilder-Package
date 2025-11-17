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
        public event Action OnPopularityThresholdReached;
        
        public Sprite Icon;
        
        [Header("Runtime Data")]
        public int CurrentPopularity;
        public bool TrustsPlayer => CurrentPopularity >= TrustThreshold;
        public bool BondedWithPlayer;
        
        [Header("Relationship Data")] 
        
        // public List<PlayerDialogOption> OptionsToUnlockOnThreshold;
        public int TrustThreshold;
        [Range(-20, 20)]
        public int BasePopularity; //todo: durch enums ersetzen?
        [Range(0, 2)]
        public float Influenceability;


        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                CurrentPopularity = BasePopularity;
            };
        }
        
        public void ApplyPopularityModifier(int modifier)
        {
            CurrentPopularity += modifier;
            
            if (CurrentPopularity >= TrustThreshold)
            {
                OnPopularityThresholdReached?.Invoke();
            }
        }
    }
}