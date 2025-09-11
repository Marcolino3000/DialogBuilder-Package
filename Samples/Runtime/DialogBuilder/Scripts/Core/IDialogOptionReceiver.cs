using System;
using DialogBuilder.Scripts.Nodes.Decorator;

namespace DialogBuilder.Scripts.Core
{
    public interface IDialogOptionReceiver : IDialogInterface
    {
        event Action<DialogOptionNode> DialogOptionSelected;
        void ShowDialogOptions(DialogOptionNode[] options);
        void TriggerIdleReaction();
        DialogOptionType DialogOptionType { get; }
    }
}