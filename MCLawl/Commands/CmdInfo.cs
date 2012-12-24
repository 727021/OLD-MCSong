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

namespace MCSong
{
    public class CmdInfo : Command
    {
        public override string name { get { return "info"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message)
        {
            if (message != "") 
            { 
                Help(p); 
            }
            else
            {
                Player.SendMessage(p, "This server runs on" + c.teal + " MCSong" + Server.DefaultColor + ", an MCLawl based software started by 727021.");
                Player.SendMessage(p, "This server's name: " + c.aqua + Server.name);
                Player.SendMessage(p, "This server's version: " + c.teal + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
               // Player.SendMessage(p, "This server's owner: " + c.aqua + Server.owner);
                Player.SendMessage(p, "There are currently " + c.aqua + Player.players.Count.ToString() + Server.DefaultColor + " players on " + c.aqua + Server.levels.Count.ToString() + Server.DefaultColor + " worlds.");
                if (Server.autorestart) Player.SendMessage(p, "This server is scheduled to restart at " + c.teal + Server.restarttime.ToString("HH:mm:ss"));

                TimeSpan up = DateTime.Now - Server.timeOnline;
                string upTime = "Time online: " + c.aqua;
                if (up.Days == 1) upTime += up.Days + " day, ";
                else if (up.Days > 0) upTime += up.Days + " days, ";
                if (up.Hours == 1) upTime += up.Hours + " hour, ";
                else if (up.Days > 0 || up.Hours > 0) upTime += up.Hours + " hours, ";
                if (up.Minutes == 1) upTime += up.Minutes + " minute and ";
                else if (up.Hours > 0 || up.Days > 0 || up.Minutes > 0) upTime += up.Minutes + " minutes and ";
                if (up.Seconds == 1) upTime += up.Seconds + " second";
                else upTime += up.Seconds + " seconds";
                Player.SendMessage(p, upTime);

                if (Server.updateTimer.Interval > 1000) Player.SendMessage(p, "Server is currently in " + c.purple + "Low Lag" + Server.DefaultColor + " mode.");
                if (Server.maintenanceMode) Player.SendMessage(p, "Server is currently in " + c.red + "Maintenance" + Server.DefaultColor + " mode.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/info - Displays the server information.");
        }
    }
}
