using System;

namespace MCSong
{
    public class CmdTitle : Command
    {
        public override string name { get { return "title"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message)
        {
            if (!Server.useMySQL) { p.SendMessage("MySQL has not been configured! Please configure MySQL to use Titles!"); return; }
            if (message == "") { Help(p); return; }

            int pos = message.IndexOf(' ');
            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { Player.SendMessage(p, "Could not find player."); return; }

            string query;
            string newTitle = "";
            if (message.Split(' ').Length > 1) newTitle = message.Substring(pos + 1);
            else
            {
                who.title = "";
                who.SetPrefix();
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " had their title removed.", false);
                query = "UPDATE Players SET Title = '' WHERE Name = '" + who.name + "'";
                MySQL.executeQuery(query);
                return;
            }

            if (newTitle != "")
            {
                newTitle = newTitle.ToString().Trim().Replace("[", "");
                newTitle = newTitle.Replace("]", "");
                /* if (newTitle[0].ToString() != "[") newTitle = "[" + newTitle;
                if (newTitle.Trim()[newTitle.Trim().Length - 1].ToString() != "]") newTitle = newTitle.Trim() + "]";
                if (newTitle[newTitle.Length - 1].ToString() != " ") newTitle = newTitle + " "; */
            }

            if (newTitle.Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }
            if (!Server.devs.Contains(p.name.ToLower()))
            {
                if (Server.devs.Contains(who.name.ToLower()) || newTitle.ToLower() == "dev" || newTitle.ToLower() == "developer") { Player.SendMessage(p, "You're not a dev!"); return; }
            }

            if (newTitle != "")
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " was given the title of &b[" + newTitle + "]", false);
            else Player.GlobalChat(null, who.color + who.prefix + who.name + Server.DefaultColor + " had their title removed.", false);

            if (newTitle == "")
            {
                query = "UPDATE Players SET Title = '' WHERE Name = '" + who.name + "'";
            }
            else
            {
                query = "UPDATE Players SET Title = '" + newTitle.Replace("'", "\'") + "' WHERE Name = '" + who.name + "'";
            }
            MySQL.executeQuery(query);
            who.title = newTitle;
            who.SetPrefix();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/title <player> [title] - Gives <player> the [title].");
            Player.SendMessage(p, "If no [title] is given, the player's title is removed.");
        }
    }
}
