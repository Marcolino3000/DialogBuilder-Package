namespace DialogBuilder.Scripts.Core
{
    public interface IDialogContextReceiver
    {
        void OnDialogOptionChosen(int popularityModifier);
    }
}