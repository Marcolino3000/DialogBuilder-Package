using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Nodes.Decorator
{
    public abstract class DialogOptionNode : CompositeNode              
    {
        public virtual DialogOptionType OptionType => DialogOptionType.None;
        public List<Tuple<string, float>> Paragraphs { get; } = new();         
        public float TotalDuration => Paragraphs.Sum(p => p.Item2);
        public bool FallThrough;
        public bool WasSelected { get; set; }
        public bool IsAvailable = true;
        public bool IsTrustOption = false;
        public bool IsBondingOption = false;
        public List<DialogOptionNode> RequiredNodes;
        public List<DialogOptionNode> BlockerNodes;
        

        private void OnEnable()
        {
            // Nodes can be unloaded and reloaded between scenes, so one that arrives after the
            // language was picked still has to build its own paragraphs.
            CreateParagraphs();
        }

        /// <summary>
        /// Builds the paragraphs from the line in the current language. Called on load, and again
        /// for every loaded node whenever <see cref="Node.CurrentLanguage"/> is assigned.
        /// </summary>
        public void CreateParagraphs()
        {
            // The list is reused instead of replaced so callers holding a reference keep seeing the
            // current text. Clearing first also keeps a second OnEnable from doubling every paragraph.
            Paragraphs.Clear();

            var line = LocalizedLine;

            if(string.IsNullOrWhiteSpace(line))
                return;

            var paragraphs = line.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var paragraph in paragraphs)
            {
                 Paragraphs.Add(new Tuple<string, float>(
                     paragraph,
                     paragraph.Length * 0.06f + 0.8f));
            }
        }
        public override List<DialogOptionNode> GetChildNodes()
        {
            return Children.Cast<DialogOptionNode>().ToList();
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
        NPC
    }
}