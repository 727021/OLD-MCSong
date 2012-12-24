/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCSong) Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;
using System.IO;

namespace MCSong
{
    public class CmdTp : Command
    {
        public override string name { get { return "tp"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Command.all.Find("spawn");
                return;
            }
            string[] split = message.Split(' ');
            if (split.Length <= 1)
            {
                Player who = Player.Find(message);
                if (who == null || (who.hidden && p.group.Permission < LevelPermission.Admin)) { Player.SendMessage(p, "There is no player \"" + message + "\"!"); return; }
                if (p.level != who.level)
                {
                    if (who.level.name.Contains("cMuseum"))
                    {
                        Player.SendMessage(p, "Player \"" + message + "\" is in a museum!");
                        return;
                    }
                    if (who.level.locked)
                    {
                        Player.SendMessage(p, "Player \"" + message + "\" is in a locked level!");
                        return;
                    }
                    else
                    {
                        Command.all.Find("goto").Use(p, who.level.name);
                    }
                }
                if (p.level == who.level)
                {
                    if (who.Loading)
                    {
                        Player.SendMessage(p, "Waiting for " + who.color + who.name + Server.DefaultColor + " to spawn...");
                        while (who.Loading) { }
                    }
                    while (p.Loading) { }  //Wait for player to spawn in new map
                    unchecked { p.SendPos((byte)-1, who.pos[0], who.pos[1], who.pos[2], who.rot[0], 0); }
                }
                return;
            }
            if (split.Length <= 2)
            {
                ushort x;
                ushort y;
                ushort z;
                try // X (width)
                {
                    x = Convert.ToUInt16(split[0]);
                }
                catch { Player.SendMessage(p, "Invalid coordinates!"); return; }
                try // Y (height)
                {
                    y = Convert.ToUInt16(split[1]);
                }
                catch { Player.SendMessage(p, "Invalid coordinates!"); return; }
                try // Z (depth)
                {
                    z = Convert.ToUInt16(split[2]);
                }
                catch { Player.SendMessage(p, "Invalid coordinates!"); return; }
                if ((x > p.level.width) || (y > p.level.height) || (z > p.level.depth)) { Player.SendMessage(p, "Invalid coordinates!"); return; }
                unchecked { p.SendPos((byte)-1, x, y, z, p.rot[0], 0); }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tp <x> <y> <z> - Teleports you to the coordinates x,y,z.");
            Player.SendMessage(p, "/tp <player> - Teleports yourself to a player.");
            Player.SendMessage(p, "If <player> is blank, /spawn is used.");
        }
    }
}