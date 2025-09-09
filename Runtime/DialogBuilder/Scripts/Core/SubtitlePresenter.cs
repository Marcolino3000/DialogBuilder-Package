using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace DialogBuilder.Scripts.Core
{
    public class SubtitlePresenter : MonoBehaviour, IDialogReceiver
    {
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private TMP_Text SubtitleText;

        private void HideSubtitle()
        {
            characterNameText.text = "";
            SubtitleText.text = "";
        }

        // public IEnumerator ShowParagraph(string characterName, Tuple<string, float> paragraph)
        // {
        //     yield return new WaitForSeconds(paragraph.Item2);
        // }

        public void DisplayDialogLine(string characterName, string text)
        {
            characterNameText.text = characterName;
            SubtitleText.text = text;
        }

        public void HideDialogLine()
        {
            HideSubtitle();
        }
    }
}
