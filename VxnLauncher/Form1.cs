using CefSharp;
using CefSharp.DevTools.DOM;
using CefSharp.WinForms;
using EZUnmanagedFunctions;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VxnNoBan;
using static VxnAuth.Class1;

namespace VxnLauncher
{
    public partial class Form1 : Form
    {
        public static string calderaTokenJWT { get; set; }
        public static string CalderaProvider { get; set; }

        public static bool IsLaunching { get; set; }

        public static bool hasLaunched { get; set; }

        public static bool isAuthing { get; set; }
        public static bool isLoadingDashboard { get; set; }


        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        public ChromiumWebBrowser browser = new ChromiumWebBrowser("https://vixenfn-launcher-1.conspiracylol.repl.co/");
        public static string VxnDllLaunchUtilPath = Directory.GetCurrentDirectory() + "/VxnLaunchUtils.dll";


        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        private LoadingForm loadingForm; // Declare the loading form
        private bool isBrowserLoaded = false; // Add a flag to track browser loading state

        public Form1()
        {

            this.Hide();
            // Show the loading screen


            // Initialize CefSharp settings
            CefSettings settings = new CefSettings();
            settings.PersistSessionCookies = false;
            //Cef.Initialize(settings);

            InitializeComponent();

            // Create and configure the ChromiumWebBrowser instance
            //browser.Load("https://vixenfn-launcher-1.conspiracylol.repl.co/");
            browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            browser.JavascriptObjectRepository.Register("vxnLauncher",
                new VxnLauncher(), false, options: BindingOptions.DefaultBinder);

            // Handle the LoadEnd event to close the loading screen when the page is loaded
            // Handle the LoadEnd event to close the loading screen when the page is loaded
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            loadingForm = new LoadingForm();
            loadingForm.Show();
            Controls.Add(browser);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (isBrowserLoaded)
            {
                browser.Visible = true;
                this.Show();

                FortniteUtil.FortnitePathFinder.GetFortPath().GetAwaiter().GetResult();
            }

        }

        public static bool CreateSuspendedProcess(string filePath, string arguments)
        {
            EZFuncs.STARTUPINFO attributes = new EZFuncs.STARTUPINFO();
            bool launchReturnValueV2 = EZFuncs.LaunchProcessW(filePath, arguments, false, EZFuncStartupAttributesValues.CREATE_NO_WINDOW | EZFuncStartupAttributesValues.CREATE_SUSPENDED | EZFuncStartupAttributesValues.DETACHED_PROCESS, attributes);
            return launchReturnValueV2;
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Console.WriteLine("Loading state changed"); // Debug output

            if (!e.IsLoading)
            {
                Console.WriteLine("Loading complete"); // Debug output

                if (loadingForm != null)
                {
                    loadingForm.Invoke((MethodInvoker)delegate
                    {
                        loadingForm.Close();
                        isBrowserLoaded = true; // Update the flag
                        this.Visible = true; // Show the form
                                             // browser.ShowDevTools();
                    });
                }
            }
        }

        public class VxnLauncher
        {
            public bool isAuthed = false;
            public string accountId = "";
            public string token = "";

            public void openDashboard()
            {

                if (isAuthed == false)
                {
                    isLoadingDashboard = true;
                    var devicecode = Auth.GetDeviceCode(Auth.GetDevicecodetoken());

                    var array = devicecode.Split(new char[] { ',' }, 2);
                    if (devicecode.Contains("error"))
                    {
                        MessageBox.Show("An error occured! Error: " + devicecode, "VixenFN V3.");
                        isLoadingDashboard = false;
                        return;
                    }
                    var tokenV2 = array[0];
                    accountId = VxnAuth.Class1.AccountInfo.AccountId;
                    token = tokenV2;
                    isAuthed = true;
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.UseShellExecute = true;
                    proc.CreateNoWindow = true;
                    proc.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Arguments = "/c start https://dashboard.vxn.lol?accountId=" + accountId;
                    proc.FileName = "cmd.exe";
                    Process.Start(proc);
                    isLoadingDashboard = false;
                    MessageBox.Show("Welcome " + VxnAuth.Class1.AccountInfo.DisplayName + "!", "VixenFN V3");
                    isLoadingDashboard = false;
                }
                else
                {
                    isLoadingDashboard = true;
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.UseShellExecute = true;
                    proc.CreateNoWindow = true;
                    proc.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Arguments = "/c start https://dashboard.vxn.lol?accountId=" + accountId;
                    proc.FileName = "cmd.exe";
                    Process.Start(proc);
                    isLoadingDashboard = false;
                }
            }

            public void launchVxn()
            {
                if (hasLaunched == true)
                {
                    if (Process.GetProcessesByName("FortniteClient-Win64-Shipping").Length != 0)
                    {
                        DialogResult dialogResult = MessageBox.Show("Fortnite is running, do you want to close Fortnite and relaunch VixenFN?", "VixenFN", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            foreach (Process launcherProclol in Process.GetProcessesByName("FortniteLauncher"))
                            {
                                launcherProclol.Kill();
                            }
                            foreach (Process shippingProcLol in Process.GetProcessesByName("FortniteClient-Win64-Shipping"))
                            {
                                shippingProcLol.Kill();
                            }
                            foreach (Process anticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_EAC"))
                            {
                                anticheatProcLOL.Kill();
                            }
                            foreach (Process EOSanticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_EAC_EOS"))
                            {
                                EOSanticheatProcLOL.Kill();
                            }
                            foreach (Process BEanticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_BE"))
                            {
                                BEanticheatProcLOL.Kill();
                            }
                            foreach (Process BEanticheatSrvcProcLOL in Process.GetProcessesByName("BEService_fn"))
                            {
                                BEanticheatSrvcProcLOL.Kill();
                            }
                            foreach (Process BEanticheatSrvcProclol in Process.GetProcessesByName("BEService"))
                            {
                                BEanticheatSrvcProclol.Kill();
                            }
                            foreach (Process BEanticheatSrvcProclolV2 in Process.GetProcessesByName("BEService_x64"))
                            {
                                BEanticheatSrvcProclolV2.Kill();
                            }
                            hasLaunched = false;
                            MessageBox.Show("Closed Fortnite, please click launch again!", "VixenFN");
                            return;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            return;
                        }

                        return;
                    }
                    else
                    {
                        if (Process.GetProcessesByName("FortniteClient-Win64-Shipping").Length == 0 && Process.GetProcessesByName("FortniteLauncher").Length != 0)
                        {
                            foreach (Process launcherProclol in Process.GetProcessesByName("FortniteLauncher"))
                            {
                                launcherProclol.Kill();
                            }
                            foreach (Process shippingProcLol in Process.GetProcessesByName("FortniteClient-Win64-Shipping"))
                            {
                                shippingProcLol.Kill();
                            }
                            foreach (Process anticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_EAC"))
                            {
                                anticheatProcLOL.Kill();
                            }
                            foreach (Process EOSanticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_EAC_EOS"))
                            {
                                EOSanticheatProcLOL.Kill();
                            }
                            foreach (Process BEanticheatProcLOL in Process.GetProcessesByName("FortniteClient-Win64-Shipping_BE"))
                            {
                                BEanticheatProcLOL.Kill();
                            }
                            foreach (Process BEanticheatSrvcProcLOL in Process.GetProcessesByName("BEService_fn"))
                            {
                                BEanticheatSrvcProcLOL.Kill();
                            }
                            foreach (Process BEanticheatSrvcProclol in Process.GetProcessesByName("BEService"))
                            {
                                BEanticheatSrvcProclol.Kill();
                            }
                            foreach (Process BEanticheatSrvcProclolV2 in Process.GetProcessesByName("BEService_x64"))
                            {
                                BEanticheatSrvcProclolV2.Kill();
                            }
                            hasLaunched = false;
                            return;
                        }

                    }

                }
                EZFuncs.AllocRedirectConsoleW();
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (isAuthed == false)
                {

                    var devicecode = Auth.GetDeviceCode(Auth.GetDevicecodetoken());

                    var array = devicecode.Split(new char[] { ',' }, 2);
                    if (devicecode.Contains("error"))
                    {
                        MessageBox.Show("An error occured! Error: " + devicecode, "VixenFN V3.");
                    }
                    var tokenV2 = array[0];
                    accountId = VxnAuth.Class1.AccountInfo.AccountId;
                    token = tokenV2;
                    isAuthed = true;

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Welcome " + VxnAuth.Class1.AccountInfo.DisplayName + "!\nPlease wait while we do some stuff.\n");
                    bool hasInitiatedAnticheat = VxnNoBan.NoBan.SimpleInitV1(FortniteUtil.FortnitePathInfo.FortniteInstallPath + "\\FortniteGame\\Binaries\\Win64\\");
                    if (hasInitiatedAnticheat == true)
                    {
                        Console.WriteLine("Anticheats have been modded successfully. Ban risk currently: Unknown. (EXPECTED: EXTREMELY EXTREMELY LOW)\nGetting authorization for user: " + VxnAuth.Class1.AccountInfo.DisplayName + ". Please wait..\n");
                        string exchangeCode = VxnAuth.Class1.Auth.GetExchange(token);
                        CalderaRequestBody requestBody = new CalderaRequestBody
                        {
                            accountId = VxnAuth.Class1.AccountInfo.AccountId,
                            exchangeCode = exchangeCode,
                            test_mode = false,
                            Epic_App = "Fortnite",
                            nvidia = true,
                            luna = false
                        };

                        string CalderaRequest = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                        Console.WriteLine();

                        var url = "https://caldera-service-prod.ecosec.on.epicgames.com/caldera/api/v1/launcher/racp";

                        var client = new RestClient(url);
                        var request = new RestRequest();
                        request.Method = Method.Post;

                        request.AddHeader("Content-Type", "application/json");

                        request.AddParameter("application/json", CalderaRequest, ParameterType.RequestBody);

                        RestResponse response = client.Execute(request);

                        Console.WriteLine("Caldera Response code: " + response.StatusCode);
                        Console.WriteLine("Caldera Response content: " + response.Content);

                        var res = JsonConvert.DeserializeObject<CalderaResponseBody>(response.Content.ToString());
                        calderaTokenJWT = res.jwt;
                        CalderaProvider = res.provider;
                        if (res.provider.ToString() != "EasyAntiCheatEOS")
                        {
                            MessageBox.Show("Caldera Provider returned non-easyanticheat_eos. Exiting for safety.. Provider: " + res.provider, "VixenFN");
                            return;
                        }

                        Console.WriteLine("We are going to try and download assets. Please wait...");
                        var sslPath = Path.GetTempPath() + "/VixenFN.dll";
                        Uri sslUrl = new Uri("https://cdn.vxn.lol/launcher/ssl");
                        WebClient sslClient = new WebClient();
                        sslClient.DownloadFileAsync(sslUrl, sslPath);
                        Console.Clear();
                        Console.WriteLine("Downloaded SSL. Initiating launcher process.....");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("generating arguments..");
                        string launchArgsFN = " -AUTH_LOGIN=unused -AUTH_PASSWORD=" + exchangeCode + "-AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -steamimportavailable -epicusername=" + VxnAuth.Class1.AccountInfo.DisplayName + " -epicuserid=" + VxnAuth.Class1.AccountInfo.AccountId + " -epiclocale=en -epicsandboxid=fn -nobe -noeac -fromfl=eaceos -caldera=" + res.jwt;
                        Console.WriteLine("Generated arguments. Arguments: " + launchArgsFN); 
                        try
                        {
                            Console.WriteLine("Trying to replace shipping...");
                            bool replaceShipping = NoBan.ReplaceShipping(FortniteUtil.FortnitePathInfo.FortniteInstallPath + "\\FortniteGame\\Binaries\\Win64\\");
                            Console.WriteLine("Shipping replace value: " + replaceShipping);
                            if(!replaceShipping == true)
                            {
                                MessageBox.Show("Failed to startup. Unable to replace shipping.", "VixenFN");
                                Process.GetCurrentProcess().Kill();
                            }

                        } catch(Exception ex)
                        {
                            Console.WriteLine("Replace shipping error: " + ex);
                            MessageBox.Show("Failed to startup. Unable to replace shipping. Error: " + ex, "VixenFN");
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                    else
                    {
                        MessageBox.Show("hasInitiatedAnticheat was not true with VxnLaunchUtils. We cannot start for your safety. See internal output log.\n Sorry.", "VixenFN");
                        Console.WriteLine("Fail init");
                    }
                }
                else
                {
                }
            }
        }
        public class CalderaResponseBody
        {
            public string jwt { get; set; }
            public string provider { get; set; }
        }
        public class CalderaRequestBody
        {
            public string accountId { get; set; }
            public string exchangeCode { get; set; }
            public bool test_mode = false;
            public string Epic_App = "Fortnite";
            public bool nvidia = true;
            public bool luna = false;
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            if (isBrowserLoaded == false)
            {
                // Hide the form
                if (InvokeRequired)
                {
                    Invoke(new Action(() => Hide()));
                }
                else
                {
                    Hide();
                }
            }
            else
            {
                // Show the form
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        Visible = true;
                        Show();
                    }));
                }
                else
                {
                    Visible = true;
                    Show();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(SettingsForm.isSettingsFormOpen == true)
            {
                return;
            }
            SettingsForm sttngs = new SettingsForm();
            sttngs.Show();
            sttngs.Visible = true;
        }
    }
}
