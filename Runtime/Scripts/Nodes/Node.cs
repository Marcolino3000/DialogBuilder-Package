using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Tree;
using UnityEngine;

namespace Nodes
{
    public enum Language
    {
        De,
        En
    }

    public abstract class Node : ScriptableObject
    {
        private static Language currentLanguage = Language.De;

        /// <summary>
        /// The language of every subtitle and dialog option. Picked on the start screen before the
        /// visitor sees the first scene, so it cannot change in the middle of a dialog.
        ///
        /// Static because the exe keeps running at the kiosk: the value survives scene loads and
        /// the reset after the inactivity timeout, and the nodes stay loaded along with it.
        /// Assigning therefore rebuilds the paragraphs of every loaded node — otherwise the next
        /// visitor would still be shown the text of the previous one.
        /// </summary>
        public static Language CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                currentLanguage = value;

                // Deliberately without an equality guard: the start screen assigns before every
                // play-through, the same value included, and that assignment is what builds the
                // paragraphs for the run.
                foreach (var node in Resources.FindObjectsOfTypeAll<DialogOptionNode>())
                    node.CreateParagraphs();
            }
        }

        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public bool customPreview;
        public Blackboard Blackboard;
        public string DialogLine;
        [Tooltip("English translation. Empty means the German line above is shown instead.")]
        public string DialogLineEn;
        public string TextPreview;
        public AudioClip AudioClip;
        public float ClipVolume = 1f;
        public float PauseAfter = 0.5f;

        /// <summary>
        /// The line in the current language. Falls back to German while no translation has been
        /// entered — at the kiosk an empty subtitle must never make it onto the screen.
        /// </summary>
        public string LocalizedLine =>
            CurrentLanguage == Language.En && !string.IsNullOrWhiteSpace(DialogLineEn)
                ? DialogLineEn
                : DialogLine;

        public abstract List<DialogOptionNode> GetChildNodes();

        private Action<string> OnDialogChanged;

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }
}
