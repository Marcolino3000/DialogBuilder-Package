using UnityEngine;

namespace Core
{
    [CreateAssetMenu]
    public class CharacterData : ScriptableObject
    {
        public Sprite Icon;
        
        [Header("Relationship Data")]
        
        [Range(-20, 20)]
        public int BasePopularity; //todo: durch enums ersetzen?
        [Range(0, 2)]
        public float Influenceability;
    }
}