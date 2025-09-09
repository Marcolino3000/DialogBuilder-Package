using System;
using UnityEngine;

namespace DialogBuilder.Scripts.Nodes.Decorator
{
    public class PlayerDialogOption : DialogOptionNode
    {
        public override DialogOptionType OptionType => DialogOptionType.Player;
        public int PopularityModifier => _popularityModifier;
        public AnswerVibe Vibe => answerVibe;


        [SerializeField] private AnswerVibe answerVibe = AnswerVibe.Neutral;
        [SerializeField] private int _popularityModifier;

        private void OnValidate()
        {
            switch (answerVibe)
            {
                case AnswerVibe.Neutral:
                    _popularityModifier = 0;
                    break;
                case AnswerVibe.Positive:
                    _popularityModifier = 5;
                    break;
                case AnswerVibe.Negative:
                    _popularityModifier = -5;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public record PlayerDialogChoiceEffects
    {
        public int PopularityModifier;
        //List<Hint> Hints;
    }

    public enum AnswerVibe
    {
        Neutral,
        Positive,
        Negative
    }
}