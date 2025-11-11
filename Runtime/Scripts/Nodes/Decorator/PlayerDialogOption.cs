using System;
using UnityEngine;

namespace Nodes.Decorator
{
    public class PlayerDialogOption : DialogOptionNode
    {
        public override DialogOptionType OptionType => DialogOptionType.Player;
        public int PopularityModifier => _popularityModifier;
        public AnswerType Type => answerType;

        [SerializeField] private AnswerType answerType = AnswerType.SmallTalk;
        [SerializeField] private int _popularityModifier;

        private void OnValidate()
        {
            switch (answerType)
            {
                case AnswerType.SmallTalk:
                    _popularityModifier = 0;
                    break;
                case AnswerType.DeepTalk:
                    _popularityModifier = 5;
                    break;
                case AnswerType.TrashTalk:
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

    public enum AnswerType
    {
        SmallTalk,
        DeepTalk,
        TrashTalk,
        BusinessTalk
    }
}