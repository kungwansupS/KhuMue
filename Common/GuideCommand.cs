using KhuMue.UI;

namespace KhuMue.Common;

/// <summary>"/khumue" in chat opens or closes the guide, so it can be reached before a key is bound.</summary>
public sealed class GuideCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "khumue";

    public override string Description => UI.Pages.PageKit.L("UI.CommandHelp");

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!Main.dedServ)
            ModContent.GetInstance<GuideUISystem>().Toggle();
    }
}
