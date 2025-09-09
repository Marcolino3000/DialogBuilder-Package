using System;
using DialogBuilder.Scripts.Nodes.Decorator;

namespace DialogBuilder.Scripts.Core
{
    public interface IDialogOptionReceiver : IDialogInterface
    {
        event Action<DialogOptionNode> DialogOptionSelected;
        void SetDialogOptions(DialogOptionNode[] options);
        DialogOptionType DialogOptionType { get; }
    }
}