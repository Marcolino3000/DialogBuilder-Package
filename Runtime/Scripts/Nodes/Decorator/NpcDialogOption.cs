namespace Nodes.Decorator
{
    public class NpcDialogOption : DialogOptionNode
    {
        public override DialogOptionType OptionType => DialogOptionType.NPC;
        public int PopularityValue;
    }
}