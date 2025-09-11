namespace DialogBuilder.Scripts.Core
{
    public interface IDialogReceiver : IDialogInterface
    {
        void DisplayDialogLine(string characterName, string text);

        void HideDialogLine();
    }
}