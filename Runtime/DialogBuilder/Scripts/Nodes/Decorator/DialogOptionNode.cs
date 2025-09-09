using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogBuilder.Scripts.Nodes.Decorator
{
    public abstract class DialogOptionNode : CompositeNode
    {
        public virtual DialogOptionType OptionType => DialogOptionType.None;
        public List<Tuple<string, float>> Paragraphs { get; } = new();
        public float TotalDuration => Paragraphs.Sum(p => p.Item2);

        private void OnEnable()
        {
            CreateParagraphs();
        }

        private void CreateParagraphs()
        {
            foreach (var paragraph in DialogLine.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                 Paragraphs.Add(new Tuple<string, float>(
                     paragraph, 
                     paragraph.Length * 0.08f + 0.5f));
            }
        }
        public override DialogOptionNode[] GetChildNodes()
        {
            return Children.Cast<DialogOptionNode>().ToArray();
        }

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            return node;
        }
    }
    
    public enum DialogOptionType
    {
        None,
        Player,
        NPC,
        Both
    }
}