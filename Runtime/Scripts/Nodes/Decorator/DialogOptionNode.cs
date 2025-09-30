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
        public bool WasSelected { get; set; }
        public List<DialogOptionNode> RequiredNodes;
        public List<DialogOptionNode> BlockerNodes;
        

        private void OnEnable()
        {
            CreateParagraphs();
        }

        private void CreateParagraphs()
        {
            if(string.IsNullOrWhiteSpace(DialogLine))
                return;
            
            var paragraphs = DialogLine.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            // if(paragraphs.Length == 0)
            //     return;
            
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