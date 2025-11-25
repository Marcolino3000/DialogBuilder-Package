using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        
        public int TrustThreshold;
        [Range(-20, 20)]
        public int BasePopularity; //todo: durch enums ersetzen?
        [Range(0, 2)]
        public float Influenceability;
        
    #if UNITY_EDITOR
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                CurrentPopularity = BasePopularity;
                BondedWithPlayer = false;
            };
        }
    #endif
        
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