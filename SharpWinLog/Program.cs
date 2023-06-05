using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SharpWinLog
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("SharpWinLog");
            System.Console.WriteLine("Author: Unknown");
            System.Console.WriteLine("Github: https://github.com/UnknownU0/SharpWinLog");
            System.Console.WriteLine("");
            if (args.Length == 0)
            {
                System.Console.WriteLine("Usage: SharpWinLog.exe -4624");
                System.Console.WriteLine("       SharpWinLog.exe -4625");
                System.Console.WriteLine("       SharpWinLog.exe -rdp");
                System.Console.WriteLine("       SharpWinLog.exe -all");
            }
            if (args.Length == 1 && (args[0] == "-4624"))
            {
                EventLog_4624();
            }
            if (args.Length == 1 && (args[0] == "-4625"))
            {
                EventLog_4625();
            }
            if (args.Length == 1 && (args[0] == "-rdp"))
            {
                RDPLog();
            }
            if (args.Length == 1 && (args[0] == "-all"))
            {
                RDPLog();
                EventLog_4624();
                EventLog_4625();
            }
        }

        public static void EventLog_4624()
        {
            EventLog log = new EventLog("Security");
            Console.WriteLine("\r\n+++=====--EventLog -> 4624--====+++\r\n");
            var entries = log.Entries.Cast<EventLogEntry>().Where(x => x.InstanceId == 4624);
            entries.Select(x => new
            {
                x.MachineName,
                x.Site,
                x.Source,
                x.Message,
                x.TimeGenerated
            }).ToList();
            foreach (EventLogEntry log1 in entries)
            {
                string text = log1.Message;
                string ipaddress = MidStrEx(text, "	Source Network Address:	", "	Source Port:");
                string username = MidStrEx(text, "New Logon:", "Process Information:");
                username = MidStrEx(username, "	Account Name:		", "	Account Domain:		");
                DateTime Time = log1.TimeGenerated;
                if (ipaddress.Length >= 7)
                {
                    Console.WriteLine("\r\n-----------------------------------");
                    Console.WriteLine("Time: " + Time);
                    Console.WriteLine("Status: True");
                    Console.WriteLine("Username: " + username.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
                    Console.WriteLine("Remote ip: " + ipaddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
                }
            }
            Console.WriteLine("\r\n========--EventLog -> 4624--=======\r\n");
        }

        public static void EventLog_4625()
        {
            EventLog log = new EventLog("Security");
            Console.WriteLine("\r\n+++=====--EventLog -> 4625--====+++\r\n");
            var entries = log.Entries.Cast<EventLogEntry>().Where(x => x.InstanceId == 4625);
            entries.Select(x => new
            {
                x.MachineName,
                x.Site,
                x.Source,
                x.Message,
                x.TimeGenerated
            }).ToList();
            foreach (EventLogEntry log1 in entries)
            {
                string text = log1.Message;
                string ipaddress = MidStrEx(text, "	Source Network Address:	", "	Source Port:");
                string username = MidStrEx(text, "New Logon:", "Process Information:");
                username = MidStrEx(username, "	Account Name:		", "	Account Domain:		");
                DateTime Time = log1.TimeGenerated;
                if (ipaddress.Length >= 7)
                {
                    Console.WriteLine("\r\n-----------------------------------");
                    Console.WriteLine("Time: " + Time);
                    Console.WriteLine("Status: Flase");
                    Console.WriteLine("Username: " + username.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
                    Console.WriteLine("Remote ip: " + ipaddress.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
                }
            }
            Console.WriteLine("\r\n========--EventLog -> 4625--=======\r\n");
        }

        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1)
                return result;
            string tmpstr = sourse.Substring(startindex + startstr.Length);
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1)
                return result;
            result = tmpstr.Remove(endindex);

            return result;
        }

        public static void RDPPort()
        {
            System.Console.WriteLine("\r\n+++======--RDP PORT--===========+++");
            try
            {
                RegistryKey rsg = null;
                rsg = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\Wds\rdpwd\Tds\tcp", true);
                if (rsg.GetValue("PortNumber") != null)
                {
                    string RDPPort = rsg.GetValue("PortNumber").ToString();
                    System.Console.WriteLine("RDPPort: " + RDPPort);
                }
                else
                    System.Console.WriteLine("Cant find RDP PORT.");
                rsg.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unknown Error:" + ex.Message.ToString());
            }
            System.Console.WriteLine("=========--RDP PORT--==============\r\n");
        }

        public static void mstscCache()
        {
            System.Console.WriteLine("\r\n+++======--Mstsc Cache--========+++");
            try
            {
                string[] sKeyNameColl;
                RegistryKey rsg = null;
                rsg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Terminal Server Client\Servers", true);
                sKeyNameColl = rsg.GetSubKeyNames();
                foreach (string sIP in sKeyNameColl)
                {
                    try
                    {
                        RegistryKey rsgnow = null;
                        rsgnow = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Terminal Server Client\Servers\" + sIP, true);
                        if (rsgnow.GetValue("UsernameHint") != null)
                        {
                            string ConnectUsername = rsgnow.GetValue("UsernameHint").ToString();
                            System.Console.WriteLine("RemoteIP: " + sIP + ", Username:" + ConnectUsername);
                        }
                        else
                            System.Console.WriteLine("RemoteIP: " + sIP + ", Username:<UnknownUsername>");
                        rsgnow.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("RemoteIP: " + sIP + ", Username:<UnknownUsername>");
                    }
                }
                rsg.Close();
            }
            catch (Exception ex)
            {
                string exToStr = ex.Message.ToString();
                if (exToStr == "Object reference not set to an instance of an object.")
                    System.Console.WriteLine("Cant Find Mstsc Cache.");
                else
                    System.Console.WriteLine("Unknown Error:" + exToStr);
            }
            System.Console.WriteLine("=========--Mstsc Cache--===========\r\n");
        }

        public static void cmdKeyCache()
        {
            System.Console.WriteLine("\r\n+++======--Cmdkey Cache--=======+++");
            try
            {
                using (Process pc = new Process())
                {
                    pc.StartInfo.FileName = "cmd.exe";
                    pc.StartInfo.CreateNoWindow = true;
                    pc.StartInfo.RedirectStandardError = true;
                    pc.StartInfo.RedirectStandardInput = true;
                    pc.StartInfo.RedirectStandardOutput = true;
                    pc.StartInfo.UseShellExecute = false;
                    pc.Start();
                    pc.StandardInput.WriteLine("@echo off");
                    pc.StandardInput.WriteLine("cmdkey /list");
                    pc.StandardInput.WriteLine("exit");
                    pc.StandardInput.AutoFlush = true;
                    string outContext = pc.StandardOutput.ReadToEnd();
                    pc.WaitForExit();
                    pc.Close();
                    System.Console.WriteLine(MidStrEx(outContext, "cmdkey /list", "exit"));
                }
            }
            catch (Exception ex)
            {
                string exToStr = ex.Message.ToString();
                System.Console.WriteLine("Cmdkey Cache Catch Error.");
            }
            System.Console.WriteLine("\r\n=========--Cmdkey Cache--==========");
        }

        public static void RDPLog()
        {
            RDPPort();
            mstscCache();
            cmdKeyCache();
        }

    }
}
