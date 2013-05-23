using System;

namespace MCSong
{
    public class CmdClearchat : Command
    {
        public override string name { get { return "clearchat"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message)
        {
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/clearchat - Clears all messages from your chat");
        }
    }
}
