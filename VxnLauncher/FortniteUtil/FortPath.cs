using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VxnLauncher.FortniteUtil
{

    public class FortnitePathInfo
    {
        public static string FortniteInstallPath { get; set; }
    }
    internal class FortnitePathFinder
    {
        public static async Task<string> GetFortPath()
        {
            try
            {
                var path1 = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic\\UnrealEngineLauncher\\LauncherInstalled.dat"));
                dynamic json = JsonConvert.DeserializeObject(path1);

                foreach (var installation in json.InstallationList)
                {
                    if (installation.AppName == "Fortnite")
                    {
                        var path = installation.InstallLocation.ToString() + "";
                        FortnitePathInfo.FortniteInstallPath = path.Replace('/', '\\');
                    }
                }

                if (string.IsNullOrEmpty(FortnitePathInfo.FortniteInstallPath))
                {
                    MessageBox.Show("We were unable to get your fortnite install location. Please verify your game.", "VixenFN");
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("We were unable to get your fortnite install location. Please verify your game. Error: " + ex.Message, "VixenFN");
                Process.GetCurrentProcess().Kill();
            }
            await Task.Delay(1);
            return FortnitePathInfo.FortniteInstallPath;
        }
    }
}
