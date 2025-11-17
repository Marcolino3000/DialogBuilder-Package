using System.Collections.Generic;
using Nodes.Decorator;
using UnityEngine;

namespace Core
{
    public class CharacterDataManager : MonoBehaviour
    {
        public string CurrentCharacterName;
        public int CurrentPopularity;
        public CharacterData CurrentCharacter => _currentCharacter;
        public List<CharacterData> CharactersTemp;
        
        private Dictionary<CharacterData, RelationshipData> Characters = new();
        private CharacterData _currentCharacter;
        
        private void Awake()
        {
            LoadCharacters();
            UpdateCharacterNameAndValue();
        }

        public RelationshipData GetCurrentRelationship()
        {
            return Characters[_currentCharacter];
        }

        private void LoadCharacters()
        {
            // CharacterData[] loadedCharacters = Resources.LoadAll<CharacterData>("ScriptableObjects");
        
            _currentCharacter = CharactersTemp[Random.Range(0, CharactersTemp.Count)];
            
            foreach (CharacterData character in CharactersTemp)
            {
                RelationshipData relationshipData = new RelationshipData
                {
                    CurrentPopularity = character.BasePopularity
                };
                Characters.Add(character, relationshipData);
            }
        }

        public void AddPlayerDialogChoiceEffects(PlayerDialogChoiceEffects playerDialogChoiceEffects)
        {
            var relationshipData = Characters[_currentCharacter];
            relationshipData.CurrentPopularity += playerDialogChoiceEffects.PopularityModifier;

            if (relationshipData.CurrentPopularity >= _currentCharacter.TrustThreshold)
            {
                _currentCharacter.UnlockTrustDialogOptions();
            }
            
            UpdateCharacterNameAndValue();
        }

        private void UpdateCharacterNameAndValue()
        {
            CurrentCharacterName = _currentCharacter.name;
            CurrentPopularity = Characters[_currentCharacter].CurrentPopularity;
        }
    }

    public class RelationshipData
    {
        public int CurrentPopularity;
    }
    
}