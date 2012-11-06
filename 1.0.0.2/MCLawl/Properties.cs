/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCSong) (Modified for use with MCZall/MCSong) Licensed under the
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
using System.Security.Cryptography;
using System.Text;

namespace MCSong
{
    public static class Properties
    {
        public static void Load(string givenPath, bool skipsalt = false)
        {
            if (!skipsalt)
            {
                RandomNumberGenerator prng = RandomNumberGenerator.Create();
                StringBuilder sb = new StringBuilder();
                byte[] oneChar = new byte[1];
                while (sb.Length < 16)
                {
                    prng.GetBytes(oneChar);
                    if (Char.IsLetterOrDigit((char)oneChar[0]))
                    {
                        sb.Append((char)oneChar[0]);
                    }
                }
                Server.salt = sb.ToString();
            }

            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        string color = "";

                        switch (key.ToLower())
                        {
                            case "server-name":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                {
                                    Server.name = value;
                                }
                                else { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "motd":
                                if (ValidString(value, "![]&:.,{}~-+()?_/\\ "))
                                {
                                    Server.motd = value;
                                }
                                else { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "port":
                                try { Server.port = Convert.ToInt32(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "upnp":
                                Server.upnp = (value.ToLower() == "true") ? true : false;
                                break;
                            case "verify-names":
                                Server.verify = (value.ToLower() == "true") ? true : false;
                                break;
                            case "public":
                                Server.pub = (value.ToLower() == "true") ? true : false;
                                break;
                            case "world-chat":
                                Server.worldChat = (value.ToLower() == "true") ? true : false;
                                break;
                            case "guest-goto":
                                Server.guestGoto = (value.ToLower() == "true") ? true : false;
                                break;
                            case "max-players":
                                try
                                {
                                    if (Convert.ToByte(value) > 128)
                                    {
                                        value = "128"; Server.s.Log("Max players has been lowered to 128.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.s.Log("Max players has been increased to 1.");
                                    }
                                    Server.players = Convert.ToByte(value);
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "max-guests":
                                try
                                {
                                    if (Convert.ToByte(value) > 128)
                                    {
                                        value = "128"; Server.s.Log("Max guests has been lowered to 128.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.s.Log("Max guests increased to 1.");
                                    }
                                    Server.guests = Convert.ToByte(value);
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "max-maps":
                                try
                                {
                                    if (Convert.ToByte(value) > 100)
                                    {
                                        value = "100";
                                        Server.s.Log("Max maps has been lowered to 100.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                        Server.s.Log("Max maps has been increased to 1.");
                                    }
                                    Server.maps = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Server.s.Log("Invalid " + key + ". Using default.");
                                }
                                break;
                            case "irc":
                                Server.irc = (value.ToLower() == "true") ? true : false;
                                break;
                            case "irc-server":
                                Server.ircServer = value;
                                break;
                            case "irc-nick":
                                Server.ircNick = value;
                                break;
                            case "irc-channel":
                                Server.ircChannel = value;
                                break;
                            case "irc-opchannel":
                                Server.ircOpChannel = value;
                                break;
                            case "irc-port":
                                try
                                {
                                    Server.ircPort = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Server.s.Log("Invalid " + key + ". Using default.");
                                }
                                break;
                            case "irc-identify":
                                try
                                {
                                    Server.ircIdentify = Convert.ToBoolean((value.ToLower() == "true") ? true : false);
                                   // Server.ircIdentify = Convert.ToBoolean(value);
                                }
                                catch
                                {
                                    Server.s.Log("Invalid " + key + ". Using default.");
                                }
                                break;
                            case "irc-password":
                                Server.ircPassword = value;
                                break;

                            // Anti Tunneling
                            case "anti-tunnels":
                                Server.antiTunnel = (value.ToLower() == "true") ? true : false;
                                break;
                            case "max-depth":
                                try
                                {
                                    Server.maxDepth = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Server.s.Log("Invalid " + key + ". Using default.");
                                }
                                break;
                            case "tunnel-rank":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.tunnelRank = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
                                break;
                            // Chat Spam Filter
                            case "chat-spam":
                                Server.chatSpam = (value.ToLower() == "true") ? true : false;
                                break;
                            case "chat-spam-count":
                                Server.chatSpamCount = Convert.ToInt32(value);
                                break;
                            case "chat-spam-seconds":
                                Server.chatSpamSeconds = Convert.ToInt32(value);
                                break;
                            case "chat-spam-consequence":
                                Server.chatSpamCon = value.ToLower();
                                break;
                            case "chat-spam-rank":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.chatSpamRank = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            // Block Spam Filter
                            case "block-spam":
                                Server.blockSpam = (value.ToLower() == "true") ? true : false;
                                break;
                            case "block-spam-count":
                                Server.blockSpamCount = Convert.ToInt32(value);
                                break;
                            case "block-spam-seconds":
                                Server.blockSpamSeconds = Convert.ToInt32(value);
                                break;
                            case "block-spam-consequence":
                                Server.blockSpamCon = value.ToLower();
                                break;
                            case "block-spam-rank":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.blockSpamRank = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;

                            case "rplimit":
                                try { Server.rpLimit = Convert.ToInt16(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "rplimit-norm":
                                try { Server.rpNormLimit = Convert.ToInt16(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;


                            case "report-back":
                                Server.reportBack = (value.ToLower() == "true") ? true : false;
                                break;

                                // Home Maps
                            case "homes":
                                Server.homes = (value.ToLower() == "true") ? true : false;
                                break;
                            case "home-x":
                                try { Server.homeX = Convert.ToInt32(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "home-y":
                                try { Server.homeY = Convert.ToInt32(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "home-z":
                                try { Server.homeZ = Convert.ToInt32(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "home-type":
                                Server.homeType = value.ToString().ToLower();
                                break;
                            case "home-physics":
                                try { Server.homePhys = Convert.ToInt16(value).ToString(); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "home-rank":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.homeRank = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;

                            case "backup-time":
                                if (Convert.ToInt32(value) > 1) { Server.backupInterval = Convert.ToInt32(value); }
                                break;
                            case "backup-location":
                                if (!value.Contains("System.Windows.Forms.TextBox, Text:"))
                                    Server.backupLocation = value;
                                break;

                            case "console-only":
                                Server.console = (value.ToLower() == "true") ? true : false;
                                break;

                            case "physicsrestart":
                                Server.physicsRestart = (value.ToLower() == "true") ? true : false;
                                break;
                            case "deathcount":
                                Server.deathcount = (value.ToLower() == "true") ? true : false;
                                break;

                            case "usemysql":
                                Server.useMySQL = (value.ToLower() == "true") ? true : false;
                                break;
                            case "host":
                                Server.MySQLHost = value;
                                break;
                            case "sqlport":
                                Server.MySQLPort = value;
                                break;
                            case "username":
                                Server.MySQLUsername = value;
                                break;
                            case "password":
                                Server.MySQLPassword = value;
                                break;
                            case "databasename":
                                Server.MySQLDatabaseName = value;
                                break;
                            case "pooling":
                                try { Server.MySQLPooling = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "defaultcolor":
                                color = c.Parse(value);
                                if (color == "")
                                {
                                    color = c.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.DefaultColor = color;
                                break;
                            case "irc-color":
                                color = c.Parse(value);
                                if (color == "")
                                {
                                    color = c.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.IRCColour = color;
                                break;
                            case "old-help":
                                try { Server.oldHelp = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "opchat-perm":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.opchatperm = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
                                break;
                            case "adminchat-perm":
                                try
                                {
                                    sbyte parsed = sbyte.Parse(value);
                                    if (parsed < -50 || parsed > 120)
                                    {
                                        throw new FormatException();
                                    }
                                    Server.adminchatperm = (LevelPermission)parsed;
                                }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "log-heartbeat":
                                try { Server.logbeat = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
                                break;
                            case "force-cuboid":
                                try { Server.forceCuboid = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
                                break;
                            case "cheapmessage":
                                try { Server.cheapMessage = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "cheap-message-given":
                                if (value != "") Server.cheapMessageGiven = value;
                                break;
                            case "custom-ban":
                                try { Server.customBan = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "custom-ban-message":
                                if (value != "") Server.customBanMessage = value;
                                break;
                            case "custom-shutdown":
                                try { Server.customShutdown = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "custom-shutdown-message":
                                if (value != "") Server.customShutdownMessage = value;
                                break;
                            case "rank-super":
                                try { Server.rankSuper = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "default-rank":
                                try { Server.defaultRank = value.ToLower(); }
                                catch { }
                                break;
                            case "afk-minutes":
                                try
                                {
                                    Server.afkminutes = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Server.s.Log("Invalid " + key + ". Using default.");
                                }
                                break;
                            case "afk-kick":
                                try { Server.afkkick = Convert.ToInt32(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "check-updates":
                                try { Server.checkUpdates = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "autoload":
                                try { Server.AutoLoad = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "auto-restart":
                                try { Server.autorestart = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "restarttime":
                                try { Server.restarttime = DateTime.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using defualt."); break; }
                                break;
                            case "parse-emotes":
                                try { Server.parseSmiley = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "use-whitelist":
                                Server.useWhitelist = (value.ToLower() == "true") ? true : false;
                                break;
                            case "main-name":
                                if (Player.ValidName(value)) Server.level = value;
                                else Server.s.Log("Invalid " + key + ". Using default.");
                                break;
                            case "dollar-before-dollar":
                                try { Server.dollardollardollar = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "money-name":
                                if (value != "") Server.moneys = value;
                                break;
                            case "mono":
                                try { Server.mono = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "restart-on-error":
                                try { Server.restartOnError = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "repeat-messages":
                                try { Server.repeatMessage = bool.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); }
                                break;
                            case "host-state":
                                if (value != "")
                                    Server.ZallState = value;
                                break;
                        }
                    }
                }
                Server.s.SettingsUpdate();
                Save(givenPath);
            }
            else Save(givenPath);
        }
        public static bool ValidString(string str, string allowed)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach (char ch in str)
            {
                if (allowedchars.IndexOf(ch) == -1)
                {
                    return false;
                }
            } return true;
        }

        static void Save(string givenPath)
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create(givenPath));
                if (givenPath.IndexOf("server") != -1)
                {
                    w.WriteLine("# Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
                    w.WriteLine("#   server-name\t=\tThe name which displays on minecraft.net");
                    w.WriteLine("#   motd\t=\tThe message which displays when a player connects");
                    w.WriteLine("#   port\t=\tThe port to operate from");
                    w.WriteLine("#   console-only\t=\tRun without a GUI (useful for Linux servers with mono)");
                    w.WriteLine("#   verify-names\t=\tVerify the validity of names");
                    w.WriteLine("#   public\t=\tSet to true to appear in the public server list");
                    w.WriteLine("#   max-players\t=\tThe maximum number of connections");
                    w.WriteLine("#   max-maps\t=\tThe maximum number of maps loaded at once");
                    w.WriteLine("#   world-chat\t=\tSet to true to enable world chat");
                    w.WriteLine("#   guest-goto\t=\tSet to true to give guests goto and levels commands");
                    w.WriteLine("#   irc\t=\tSet to true to enable the IRC bot");
                    w.WriteLine("#   irc-nick\t=\tThe name of the IRC bot");
                    w.WriteLine("#   irc-server\t=\tThe server to connect to");
                    w.WriteLine("#   irc-channel\t=\tThe channel to join");
                    w.WriteLine("#   irc-opchannel\t=\tThe channel to join (posts OpChat)");
                    w.WriteLine("#   irc-port\t=\tThe port to use to connect");
                    w.WriteLine("#   irc-identify\t=(true/false)\tDo you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
                    w.WriteLine("#   irc-password\t=\tThe password you want to use if you're identifying with nickserv");
                    w.WriteLine("#   anti-tunnels\t=\tStops people digging below max-depth");
                    w.WriteLine("#   max-depth\t=\tThe maximum allowed depth to dig down");
                    w.WriteLine("#   backup-time\t=\tThe number of seconds between automatic backups");
                    w.WriteLine("#   overload\t=\tThe higher this is, the longer the physics is allowed to lag. Default 1500");
                    w.WriteLine("#   use-whitelist\t=\tSwitch to allow use of a whitelist to override IP bans for certain players.  Default false.");
                    w.WriteLine("#   force-cuboid\t=\tRun cuboid until the limit is hit, instead of canceling the whole operation.  Default false.");
                    w.WriteLine();
                    w.WriteLine("#   Host\t=\tThe host name for the database (usually 127.0.0.1)");
                    w.WriteLine("#   SQLPort\t=\tPort number to be used for MySQL.  Unless you manually changed the port, leave this alone.  Default 3306.");
                    w.WriteLine("#   Username\t=\tThe username you used to create the database (usually root)");
                    w.WriteLine("#   Password\t=\tThe password set while making the database");
                    w.WriteLine("#   DatabaseName\t=\tThe name of the database stored (Default = MCZall)");
                    w.WriteLine();
                    w.WriteLine("#   defaultColor\t=\tThe color code of the default messages (Default = &e)");
                    w.WriteLine();
                    w.WriteLine("#   Super-limit\t=\tThe limit for building commands for SuperOPs");
                    w.WriteLine("#   Op-limit\t=\tThe limit for building commands for Operators");
                    w.WriteLine("#   Adv-limit\t=\tThe limit for building commands for AdvBuilders");
                    w.WriteLine("#   Builder-limit\t=\tThe limit for building commands for Builders");
                    w.WriteLine();
                    w.WriteLine();
                    w.WriteLine("# Server options");
                    w.WriteLine("server-name = " + Server.name);
                    w.WriteLine("motd = " + Server.motd);
                    w.WriteLine("port = " + Server.port.ToString());
                    w.WriteLine("upnp = " + Server.upnp.ToString().ToLower());
                    w.WriteLine("verify-names = " + Server.verify.ToString().ToLower());
                    w.WriteLine("public = " + Server.pub.ToString().ToLower());
                    w.WriteLine("max-players = " + Server.players.ToString());
                    w.WriteLine("max-guests = " + Server.guests.ToString());
                    w.WriteLine("max-maps = " + Server.maps.ToString());
                    w.WriteLine("world-chat = " + Server.worldChat.ToString().ToLower());
                    w.WriteLine("check-updates = " + Server.checkUpdates.ToString().ToLower());
                    w.WriteLine("autoload = " + Server.AutoLoad.ToString().ToLower());
                    w.WriteLine("auto-restart = " + Server.autorestart.ToString().ToLower());
                    w.WriteLine("restarttime = " + Server.restarttime.ToShortTimeString());
                    w.WriteLine("restart-on-error = " + Server.restartOnError);
                    w.WriteLine("main-name = " + Server.level);
                    w.WriteLine();
                    w.WriteLine("# irc bot options");
                    w.WriteLine("irc = " + Server.irc.ToString().ToLower());
                    w.WriteLine("irc-nick = " + Server.ircNick);
                    w.WriteLine("irc-server = " + Server.ircServer);
                    w.WriteLine("irc-channel = " + Server.ircChannel);
                    w.WriteLine("irc-opchannel = " + Server.ircOpChannel);
                    w.WriteLine("irc-port = " + Server.ircPort.ToString());
                    w.WriteLine("irc-identify = " + Server.ircIdentify.ToString().ToLower());
                    w.WriteLine("irc-password = " + Server.ircPassword);
                    w.WriteLine();
                    w.WriteLine("# other options");
                    w.WriteLine("anti-tunnels = " + Server.antiTunnel.ToString().ToLower());
                    w.WriteLine("max-depth = " + Server.maxDepth.ToString().ToLower());
                    w.WriteLine("tunnel-rank = " + ((sbyte)Server.tunnelRank).ToString());
                    w.WriteLine("chat-spam = " + Server.chatSpam.ToString().ToLower());
                    w.WriteLine("chat-spam-count = " + Server.chatSpamCount.ToString());
                    w.WriteLine("chat-spam-seconds = " + Server.chatSpamSeconds.ToString());
                    w.WriteLine("chat-spam-consequence = " + Server.chatSpamCon.ToLower());
                    w.WriteLine("chat-spam-rank = " + ((sbyte)Server.chatSpamRank).ToString());
                    w.WriteLine("block-spam = " + Server.blockSpam.ToString().ToLower());
                    w.WriteLine("block-spam-count = " + Server.blockSpamCount.ToString());
                    w.WriteLine("block-spam-seconds = " + Server.blockSpamSeconds.ToString());
                    w.WriteLine("block-spam-consequence = " + Server.blockSpamCon.ToLower());
                    w.WriteLine("block-spam-rank = " + ((sbyte)Server.blockSpamRank).ToString());
                    w.WriteLine("rplimit = " + Server.rpLimit.ToString().ToLower());
                    w.WriteLine("rplimit-norm = " + Server.rpNormLimit.ToString().ToLower());
                    w.WriteLine("physicsrestart = " + Server.physicsRestart.ToString().ToLower());
                    w.WriteLine("old-help = " + Server.oldHelp.ToString().ToLower());
                    w.WriteLine("deathcount = " + Server.deathcount.ToString().ToLower());
                    w.WriteLine("afk-minutes = " + Server.afkminutes.ToString());
                    w.WriteLine("afk-kick = " + Server.afkkick.ToString());
                    w.WriteLine("parse-emotes = " + Server.parseSmiley.ToString().ToLower());
                    w.WriteLine("dollar-before-dollar = " + Server.dollardollardollar.ToString().ToLower());
                    w.WriteLine("use-whitelist = " + Server.useWhitelist.ToString().ToLower());
                    w.WriteLine("money-name = " + Server.moneys);
                    w.WriteLine("opchat-perm = " + ((sbyte)Server.opchatperm).ToString());
                    w.WriteLine("adminchat-perm = " + ((sbyte)Server.adminchatperm).ToString());
                    w.WriteLine("log-heartbeat = " + Server.logbeat.ToString());
                    w.WriteLine("force-cuboid = " + Server.forceCuboid.ToString());
                    w.WriteLine("repeat-messages = " + Server.repeatMessage.ToString());
                    w.WriteLine("host-state = " + Server.ZallState.ToString());
                    w.WriteLine();
                    w.WriteLine("# Homes");
                    w.WriteLine("homes = " + Server.homes.ToString().ToLower());
                    w.WriteLine("home-x = " + Server.homeX.ToString());
                    w.WriteLine("home-y = " + Server.homeY.ToString());
                    w.WriteLine("home-z = " + Server.homeZ.ToString());
                    w.WriteLine("home-type = " + Server.homeType);
                    w.WriteLine("home-physics = " + Server.homePhys);
                    w.WriteLine("home-rank = " + ((sbyte)Server.homeRank).ToString());
                    w.WriteLine();
                    w.WriteLine("# backup options");
                    w.WriteLine("backup-time = " + Server.backupInterval.ToString());
                    w.WriteLine("backup-location = " + Server.backupLocation);
                    w.WriteLine();
                    w.WriteLine("# Error logging");
                    w.WriteLine("report-back = " + Server.reportBack.ToString().ToLower());
                    w.WriteLine();
                    w.WriteLine("# MySQL information");
                    w.WriteLine("UseMySQL = " + Server.useMySQL);
                    w.WriteLine("Host = " + Server.MySQLHost);
                    w.WriteLine("SQLPort = " + Server.MySQLPort);
                    w.WriteLine("Username = " + Server.MySQLUsername);
                    w.WriteLine("Password = " + Server.MySQLPassword);
                    w.WriteLine("DatabaseName = " + Server.MySQLDatabaseName);
                    w.WriteLine("Pooling = " + Server.MySQLPooling);
                    w.WriteLine();
                    w.WriteLine("# Colors");
                    w.WriteLine("defaultColor = " + Server.DefaultColor);
                    w.WriteLine("irc-color = " + Server.IRCColour);
                    w.WriteLine();
                    w.WriteLine("# Running on mono?");
                    w.WriteLine("mono = " + Server.mono);
                    w.WriteLine();
                    w.WriteLine("# Custom Messages");
                    w.WriteLine("custom-ban = " + Server.customBan.ToString().ToLower());
                    w.WriteLine("custom-ban-message = " + Server.customBanMessage);
                    w.WriteLine("custom-shutdown = " + Server.customShutdown.ToString().ToLower());
                    w.WriteLine("custom-shutdown-message = " + Server.customShutdownMessage);
                    w.WriteLine();
                    w.WriteLine("cheapmessage = " + Server.cheapMessage.ToString().ToLower());
                    w.WriteLine("cheap-message-given = " + Server.cheapMessageGiven);
                    w.WriteLine("rank-super = " + Server.rankSuper.ToString().ToLower());
                    try { w.WriteLine("default-rank = " + Server.defaultRank); }
                    catch { w.WriteLine("default-rank = guest"); }
                }
                w.Flush();
                w.Close();
                w.Dispose();
            }
            catch
            {
                Server.s.Log("SAVE FAILED! " + givenPath);
            }
        }
    }
}
