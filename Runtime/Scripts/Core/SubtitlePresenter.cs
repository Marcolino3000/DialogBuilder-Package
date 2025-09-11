using TMPro;
using UnityEngine;

namespace Core
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
