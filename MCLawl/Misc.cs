using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCSong
{
    class Report
    {
        private static string path = "logs/reports/" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        public static void addReport(Player reported, Player reporter, string reason)
        {
            if (!Directory.Exists("logs/reports/")) Directory.CreateDirectory("logs/reports/");
            if (!File.Exists(path)) File.Create(path);
            try
            {
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("(" + DateTime.Now.ToString("HH:mm:ss") + ")");
                sw.WriteLine(reported.name + "[" + reported.ip + "] reported by " + reporter.name + "[" + reporter.ip + "]");
                sw.WriteLine("Reason: " + reason);
                sw.WriteLine("-----------------------------------------------------");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception e) { Server.ErrorLog(e); return; }
            Server.s.Log(reported.name + "[" + reported.ip + "] reported by " + reporter.name + "[" + reporter.ip + "]: " + reason);
            Player.GlobalMessageOps(reported.color + reported.name + Server.DefaultColor + " reported by " + reporter.color + reporter.name + Server.DefaultColor + ": " + reason);
            Player.SendMessage(reporter, reported.color + reported.name + Server.DefaultColor + " has been reported to the server operators.");
            Player.SendMessage(reported, "You have been reported to the server operators for: " + reason);
        }
    }

    class ProfanityFilter
    {
        public struct bannedWord { public string word; public string replacement; }
        public static List<bannedWord> bannedWords = new List<bannedWord>();
        public string checkMessage(Player p, string message)
        {
            string filtered = "";
            string[] spl = message.Split(' ');
            try
            {
                foreach (string word in spl)
                {
                    bannedWords.ForEach(delegate(bannedWord bw)
                    {
                        if (word.ToLower() == bw.word.ToLower())
                        {
                            filtered = message.Replace(bw.word, (Server.swearColor + bw.replacement + Server.DefaultColor));
                        }
                    });
                }
                return filtered;
            }
            catch (Exception e) { Server.ErrorLog(e); return message; }
        }
        public static void loadWords(string path)
        {
            if (!File.Exists(path)) File.WriteAllText(path,
                "# MCSong Profanity Filter swearwords.txt" + Environment.NewLine +
                "# Add words to this list to have them filtered from the chat." + Environment.NewLine +
                "# Add words like this:\tword : replacement" + Environment.NewLine +
                "# Put each word on its own line. Blank lines and lines starting with '#' will be ignored." + Environment.NewLine +
                "# Some example words have already been added to the list." + Environment.NewLine + Environment.NewLine +
                "fuck : BEEP" + Environment.NewLine +
                "bitch : dogsftw" + Environment.NewLine +
                "shit : soap");

            try
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    if ((!line.StartsWith("#")) && (line != ""))
                    {
                        bannedWord bw = new bannedWord();
                        bw.word = line.Split(':')[0].Trim().ToLower();
                        bw.replacement = line.Split(':')[1].Trim().ToLower();
                        bannedWords.Add(bw);
                    }
                }
                Server.s.Log("Profanity Filter loaded");
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
    }
}
