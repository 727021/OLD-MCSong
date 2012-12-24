using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSong
{
    class CmdReport : Command
    {
        public override string name { get { return "report"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "moderation"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command is not usable frrom console"); return; }
            string[] spl = message.Split(' ');
            Player pl = Player.Find(spl[0]);
            string reason = message.Substring(message.IndexOf(spl[1]));
            Report.addReport(pl, p, reason);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/report <player> <message> - Report <player> to the server operators");
        }
    }
}
