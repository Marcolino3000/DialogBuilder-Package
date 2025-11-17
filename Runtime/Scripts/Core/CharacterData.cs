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

        public List<PlayerDialogOption> OptionsToUnlockOnThreshold;

        [Header("Relationship Data")] 
        
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

        public void UnlockTrustDialogOptions()
        {
            foreach (var option in OptionsToUnlockOnThreshold)
            {
                option.IsAvailable = true;
            }
        }
    }
}