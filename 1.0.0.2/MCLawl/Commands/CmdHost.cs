using System;
using System.IO;
using System.Collections.Generic;

namespace MCSong
{
    public class CmdHost : Command
    {
        public override string name { get { return "host"; } }
        public override string shortcut { get { return "zall"; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            Player.SendMessage(p, "Host is currently &3" + Server.ZallState + ".");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/host - Shows the server's host state/console name");
        }
    }
}