using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading;

namespace VxnNoBan
{
    public class NoBan
    {
        public static bool checkFileExists(string filename, string fileDirectory)
        {
            if(File.Exists(fileDirectory.Replace("/", "\\") + "\\" + filename))
            {
                return true;
            }
            return false;
        }
     
        public static bool ReplaceShipping(string fortPath)
        {
            try
            {
                var execCommand = "cd /d \"" + fortPath + "\" && ren FortniteClient-Win64-Shipping_EAC_EOS.exe FortniteClient-Win64-Shipping_EAC_EOS.exe_VXNBACKUP && attrib +r FortniteClient-Win64-Shipping_EAC_EOS.exe";
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = execCommand;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to replace shipping! error: " + ex);
                return false;
            }
        }
        public static bool restoreNormal()
        {
            var programFilesX84PathDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var beFolder = programFilesX84PathDir + "\\Common Files\\BattlEye";
            var eacFolder = programFilesX84PathDir + "\\EasyAntiCheat_EOS";
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice.exe_VXNBACKUP"))
            {
                try
                {
                   if(RunCommandInBeFolder("attrib -r beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"beservice.exe\" at battleye dir");
                        return false;
                    }
                    if(RunCommandInBeFolder("del beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"beservice.exe\" at battleye dir");
                        return false;
                    }
                    return true;
                } catch(Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"beservice.exe\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice_fn.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInBeFolder("attrib -r beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"beservice_fn.exe\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("del beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"beservice_fn.exe\" at battleye dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"beservice_fn.exe\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "bedaisy.sys_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInBeFolder("attrib -r bedaisy.sys") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"bedaisy.sys\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("del bedaisy.sys") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"bedaisy.sys\" at battleye dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"bedaisy.sys\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            Console.WriteLine("Battleye Restore Part 1/2 complete. Attempting Easy anticheat restore 1/2.");
            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("del EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.sys_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("del EasyAntiCheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            Console.WriteLine("Easy anticheat Restore Part 1/2 complete. Attempting Battleye restore part 2/2.");

            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("ren beservice.exe_VXNBACKUP beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"beservice.exe_VXNBACKUP\" at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"beservice.exe_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice_fn.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("ren beservice_fn.exe_VXNBACKUP beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"beservice_fn.exe_VXNBACKUP\"  at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"beservice_fn.exe_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "BEDAISY.SYS_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("ren BEDAISY.sys_VXNBACKUP BEDAISY.sys") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"BEDAISY.SYS_VXNBACKUP\"  at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"BEDAISY.SYS_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }
            Console.WriteLine("Battleye backup/mod Part 2/4 complete. Attempting Easy anticheat backup/mod part 2/4.");

            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.exe_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("ren EasyAntiCheat_EOS.exe_VXNBACKUP EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.sys_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.sys_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for restore) command for file: \"EasyAntiCheat_EOS.sys_VXNBACKUP\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("ren EasyAntiCheat_EOS.sys_VXNBACKUP EasyAntiCheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run delete (for restore) command for file: \"EasyAntiCheat_EOS.sys_VXNBACKUP\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run restore command for file: \"EasyAntiCheat_EOS.sys_VXNBACKUP\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            return true;
        }

        public static bool SimpleInitV1(string FortPath)
        {
            var vxnBackupDir = Directory.GetCurrentDirectory() + "\\vxn\\vxn.exe";
            var programFilesX84PathDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var beFolder = programFilesX84PathDir + "\\Common Files\\BattlEye";
            var eacFolder = programFilesX84PathDir + "\\EasyAntiCheat_EOS";
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInBeFolder("attrib -r beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for backup/mod) command for file: \"beservice.exe\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("copy beservice.exe beservice.exe_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"beservice.exe\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("del beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for backup/mod) command for file: \"beservice.exe\" at battleye dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"beservice.exe\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice_fn.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInBeFolder("attrib -r beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for backup/mod) command for file: \"beservice_fn.exe\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("copy beservice_fn.exe beservice_fn.exe_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"beservice_fn.exe\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("del beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for backup/mod) command for file: \"beservice_fn.exe\" at battleye dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"beservice_fn.exe\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "bedaisy.sys_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInBeFolder("attrib -r bedaisy.sys") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for backup/mod) command for file: \"bedaisy.sys\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("copy bedaisy.sys bedaisy.sys_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"bedaisy.sys\" at battleye dir");
                        return false;
                    }
                    if (RunCommandInBeFolder("del bedaisy.sys") != true)
                    {
                        Console.WriteLine("Unable to run delete (for backup/mod) command for file: \"bedaisy.sys\" at battleye dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"bedaisy.sys\" at battleye dir, error: " + ex);
                    return false;
                }
            }
            Console.WriteLine("Battleye backup/mod Part 1/4 complete. Attempting Easy anticheat backup/mod 1/4.");
            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for backup/mod) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("copy EasyAntiCheat_EOS.exe EasyAntiCheat_EOS.exe_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("del EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run delete (for backup/mod) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.sys_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("attrib -r EasyAntiCheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run attribute set to not read only (for backup/mod) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("copy EasyAnticheat_EOS.sys EasyAntiCheat_EOS.sys_VXNBACKUP") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    if (RunCommandInEacFolder("del EasyAnticheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            Console.WriteLine("Easy anticheat backup/mod Part 1/4 complete. Attempting Battleye backup/mod part 2/4.");

            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("copy \" " + vxnBackupDir + " \" beservice.exe") != true)
                    {
                        Console.WriteLine("Unable to run copy set to read only (for backup/mod) command for file: \"beservice.exe_VXNBACKUP\" at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"beservice.exe_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }
            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "beservice_fn.exe_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("copy \" " + vxnBackupDir + " \" beservice_fn.exe") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"beservice_fn.exe_VXNBACKUP\"  at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run copy  (for backup/mod) command for file: \"beservice_fn.exe_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(beFolder.Replace("/", "\\") + "\\" + "BEDAISY.SYS_VXNBACKUP"))
            {
                try
                {
                    if (RunCommandInEacFolder("copy \" " + vxnBackupDir + " \" BEDAISY.sys") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"BEDAISY.SYS_VXNBACKUP\"  at BE dir");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run copy command (for backup/mod) for file: \"BEDAISY.SYS_VXNBACKUP\" at BE dir, error: " + ex);
                    return false;
                }
            }

            Console.WriteLine("Battleye backup/mod Part 2/4 complete. Attempting Easyanticheat backup/mod part 2/4.");
            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.exe_VXNBACKUP"))
            {
                try
                {
                    
                    if (RunCommandInEacFolder("ren EasyAntiCheat_EOS.exe_VXNBACKUP EasyAntiCheat_EOS.exe") != true)
                    {
                        Console.WriteLine("Unable to run copy (for backup/mod) command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir");
                        return false;
                    }
                  
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run backup/mod command for file: \"EasyAntiCheat_EOS.exe\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }

            if (File.Exists(eacFolder.Replace("/", "\\") + "\\" + "EasyAntiCheat_EOS.sys_VXNBACKUP"))
            {
                try
                {
             
                    if (RunCommandInEacFolder("ren EasyAnticheat_EOS.sys_VXNBACKUP EasyAntiCheat_EOS.sys") != true)
                    {
                        Console.WriteLine("Unable to run ren (for backup/mod) command for file: \"EasyAntiCheat_EOS.sys\" at easyanticheat dir");
                        return false;
                    }
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to run ren command for file: \"EasyAntiCheat_EOS.sys_VXNBACKUP\" at easyanticheat dir, error: " + ex);
                    return false;
                }
            }
            return true;
        }
        public static bool RestoreShipping(string FortPath)
        {
            try
            {


                var execCommand = "cd /d \"" + FortPath + "\" && attrib -r FortniteClient-Win64-Shipping_EAC_EOS.exe && del FortniteClient-Win64-Shipping_EAC_EOS.exe";
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = execCommand;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();

                var execCommandV2 = "cd /d \"" + FortPath + "\" && attrib -r FortniteClient-Win64-Shipping_EAC_EOS.exe_VXNBACKUP && ren FortniteClient-Win64-Shipping_EAC_EOS.exe_VXNBACKUP FortniteClient-Win64-Shipping_EAC_EOS.exe";
                Process procV2 = new Process();
                procV2.StartInfo.FileName = "cmd.exe";
                procV2.StartInfo.Arguments = execCommandV2;
                procV2.StartInfo.UseShellExecute = true;
                procV2.StartInfo.Verb = "runas";
                procV2.StartInfo.CreateNoWindow = true;
                procV2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                procV2.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to restore shipping! error: " + ex);
                return false;
            }
        }
        private static bool RunCommandInBeFolder(string command)
        {
            try
            {
            var programFilesX84PathDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var beFolder = programFilesX84PathDir + "\\Common Files\\BattlEye";
            var execCommand = "cd /d \"" + beFolder + "\" && " + command;
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = execCommand;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            return true;
            } 
            catch(Exception ex) {
                Console.WriteLine("Unable to run command at be dir, command: " + command + " error: " + ex);
                return false;
            }
            
        }
        private static bool RunCommandAtFolder(string command, string executeDirectory)
        {
            try
            {
                var execCommand = "cd /d \"" + executeDirectory + "\" && " + command;
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = execCommand;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to run command at dir: " + executeDirectory + ", command: " + command + " error: " + ex);
                return false;
            }
        }
        private static bool RunCommandInEacFolder(string command)
        {
            try
            {
                var programFilesX84PathDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var beFolder = programFilesX84PathDir + "\\EasyAntiCheat_EOS";
                var execCommand = "cd /d \"" + beFolder + "\" && " + command;
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = execCommand;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                return true;
            } catch(Exception ex) {
                Console.WriteLine("Unable to run command at eac dir, command: " + command + " error: " + ex);
                return false;
            }
        }
        
    }
}
