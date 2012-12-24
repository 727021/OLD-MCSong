using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSong
{
    class CmdWarn : Command
    {
        public override string name { get { return "warn"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "moderation"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override void Use(Player p, string message)
        {
            Player pl = Player.Find(message);
            string msg = message.Substring(message.IndexOf(' ') + 1);
            if (pl == null) { Player.SendMessage(p, "Player \"" + pl.name + "\" not found!"); return; }
            if (p == null)
            {
                if (pl.warned)
                {
                    if (msg.Trim() == "" || msg == null)
                    {
                        pl.Kick("Maximum warnings exceeded!");
                    }
                    else
                    {
                        pl.Kick(msg);
                    }
                }
                else
                {
                    pl.warned = true;
                    if (msg.Trim() == "" || msg == null)
                    {
                        Player.SendMessage(pl, "Warned by &aConsole" + Server.DefaultColor + ": " + c.red + "One more warning is an automatic kick!");
                        Player.GlobalMessageOps("To Ops" + c.white + " - " + pl.color + pl.name + c.white + " was warned by &aConsole");
                        Server.s.Log(pl.name + " was warned by Console");
                    }
                    else
                    {
                        Player.SendMessage(pl, "Warned by &aConsole" + Server.DefaultColor + ": &c" + msg);
                        Player.GlobalMessageOps("To Ops" + c.white + " - " + pl.color + pl.name + c.white + " was warned by &aConsole" + c.white +  ": " + c.red + msg);
                        Server.s.Log(pl.name + " was warned by Console: " + msg);
                    }
                }
            }
            if (pl.group.Permission >= p.group.Permission)
            {
                Player.SendMessage(p, "You cannot warn a player of an equal or higher rank!");
                return;
            }
            if (p.warned)
            {
                Player.SendMessage(p, "You cannot warn a player if you are warned!");
                return;
            }
            if (pl.warned)
            {
                if (msg.Trim() == "" || msg == null)
                {
                    pl.Kick("Maximum warnings exceeded!");
                }
                else
                {
                    pl.Kick(msg);
                }
            }
            else
            {
                pl.warned = true;
                if (msg.Trim() == "" || msg == null)
                {
                    Player.SendMessage(pl, "Warned by " + p.color + p.name + ": " + c.red + "One more warning is an automatic kick!");
                    Player.GlobalMessageOps("To Ops" + c.white + " - " + pl.color + pl.name + c.white + " was warned by " + p.color + p.name);
                    Server.s.Log(pl.name + " was warned by " + p.name);
                    if (p.group.Permission < Server.opchatperm)
                    {
                        Player.SendMessage(p, pl.color + pl.name + c.white  + "was warned by " + p.color + p.name);
                    }
                }
                else
                {
                    Player.SendMessage(pl, "Warned by " + p.color + p.name + Server.DefaultColor + ": &c" + msg);
                    Player.GlobalMessageOps("To Ops" + c.white + " - " + pl.color + pl.name + c.white + " was warned by " + p.color + p.name + c.white + ": " + c.red + msg);
                    Server.s.Log(pl.name + " was warned by " + p.name + ": " + msg);
                    if (p.group.Permission < Server.opchatperm)
                    {
                        Player.SendMessage(p, pl.color + pl.name + c.white  +  "was warned by " + p.color + p.name + c.white + ": " + c.red + msg);
                    }
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/warn <player> [message] - Give <player> a warning about [message]");
            Player.SendMessage(p, "Players are automatically kicked after 2 warnings");
        }
    }
}
