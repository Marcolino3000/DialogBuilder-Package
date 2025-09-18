using System;

namespace Core
{
    public interface IDialogStarter : IDialogInterface
    {
        event Action OnStartDialog;
        event Action OnStopDialog;
    }
}