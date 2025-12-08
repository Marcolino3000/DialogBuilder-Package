using System;
using Tree;

namespace Core
{
    public interface IDialogTreeSetter : IDialogInterface
    {
        event Action<DialogTree, Action<bool>> OnSetDialogTree;
    }
}