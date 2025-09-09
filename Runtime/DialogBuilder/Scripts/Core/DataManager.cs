using System.Collections.Generic;
using DialogBuilder.Scripts.Nodes.Decorator;
using UnityEngine;

namespace DialogBuilder.Scripts.Core
{
    public class DataManager : MonoBehaviour
    {
        public string CurrentCharacterName;
        public int CurrentPopularity;

        public CharacterData CurrentCharacter => _currentCharacter;

        private Dictionary<CharacterData, RelationshipData> Characters;
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
            Characters = new Dictionary<CharacterData, RelationshipData>();
            CharacterData[] loadedCharacters = Resources.LoadAll<CharacterData>("ScriptableObjects");
        
            _currentCharacter = loadedCharacters[Random.Range(0, loadedCharacters.Length)];
            
            foreach (CharacterData character in loadedCharacters)
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
            Characters[_currentCharacter].CurrentPopularity += playerDialogChoiceEffects.PopularityModifier;
            
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