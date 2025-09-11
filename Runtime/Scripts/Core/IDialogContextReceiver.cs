namespace Core
{
    public interface IDialogContextReceiver
    {
        void OnDialogOptionChosen(int popularityModifier);
    }
}