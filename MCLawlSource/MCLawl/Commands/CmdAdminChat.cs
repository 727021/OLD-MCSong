﻿using System;

namespace MCSong
{
    public class CmdAdminChat : Command
    {
        public override string name { get { return "adminchat"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdAdminChat() { }

        public override void Use(Player p, string message)
        {
            p.adminchat = !p.adminchat;
            if (p.adminchat) Player.SendMessage(p, "All messages will now be sent to Admins only");
            else Player.SendMessage(p, "Admin chat turned off");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/adminchat - Makes all messages sent go to Admins by default");
        }
    }
}