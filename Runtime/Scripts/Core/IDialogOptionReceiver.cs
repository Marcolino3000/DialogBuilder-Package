using System;
using Nodes.Decorator;

namespace Core
{
    public interface IDialogOptionReceiver : IDialogInterface
    {
        event Action<DialogOptionNode> DialogOptionSelected;
        void ShowDialogOptions(DialogOptionNode[] options);
        public void HideDialogOptions();
        void TriggerIdleReaction();
        DialogOptionType DialogOptionType { get; }
    }
}