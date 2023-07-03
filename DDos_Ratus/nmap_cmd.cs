using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace DDos_Ratus
{
    public partial class nmap_cmd : Form
    {
        private string ip = "[IP-ADDRESSE]";
        private string pt = "-p";
        private string sv = "";
        private string sc = "";
        private string tn = "T0";

        public nmap_cmd()
        {
            InitializeComponent();
            this.textBox2.TextChanged += TextBox2_TextChanged;
            this.trackBar1.ValueChanged += TrackBar_Change;
            this.button3.Visible = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Rendre la fenêtre non redimensionnable
            this.MaximizeBox = false; // Désactiver le bouton d'agrandissement

        }

        private void nmap_cmd_Load(object sender, EventArgs e)
        {

        }
        private void TrackBar_Change(object sender, EventArgs e)
        {
            if (trackBar1.Value != 0)
            {
                tn = "T"+trackBar1.Value;
                updTextBox1();
            }
            else
            {
                tn = "T0";
                updTextBox1();
            }
        }

        private void updTextBox1()
        {
            if(button3.Visible == false)
            {
                button3.Visible = true;
            }
            this.textBox1.Text = "sudo nmap " + ip + " " + sv + " " + sc + " " + tn + " "+ pt + " -Pn";
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            string ipAddress = textBox2.Text.Trim();
            if (IsValidIpAddress(ipAddress))
            {
                ip = ipAddress;
                updTextBox1();
            }
        }

        private bool IsValidIpAddress(string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress))
            {
                if (parsedIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return true;
                }
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateTable();
        }
        private void GenerateTable()
        {
            string[,] tableData = new string[,]
            {
                {"Level", "T0", "T1", "T2", "T3", "T4", "T5"},
                {"Name", "Paranoid", "Sneaky", "Polite", "Normal", "Aggressive", "Insane"},
                {"min-rtt-timeout", "100 ms", "100 ms", "100 ms", "100 ms", "100 ms", "50 ms"},
                {"max-rtt-timeout", "5 minutes", "15 seconds", "10 seconds", "10 seconds", "1250 ms", "300 ms"},
                {"initial-rtt-timeout", "5 minutes", "15 seconds", "1 second", "1 second", "500 ms", "250 ms"},
                {"max-retries", "10", "10", "10", "10", "6", "2"},
                {"Initial (and minimum) scan delay (--scan-delay)", "5 minutes", "15 seconds", "400 ms", "0", "0", "0"},
                {"Maximum TCP scan delay", "5 minutes", "15,000", "1 second", "1 second", "10 ms", "5 ms"},
                {"Maximum UDP scan delay", "5 minutes", "15 seconds", "1 second", "1 second", "1 second", "1 second"},
                {"host-timeout", "0", "0", "0", "0", "0", "15 minutes"},
                {"script-timeout", "0", "0", "0", "0", "0", "10 minutes"},
                {"max-parallelism", "1", "1", "1", "Dynamic", "Dynamic", "Dynamic"},
            };

            int rowCount = tableData.GetLength(0);
            int colCount = tableData.GetLength(1);

            DataTable dataTable = new DataTable();

            // Ajouter les colonnes au DataTable
            for (int col = 0; col < colCount; col++)
            {
                dataTable.Columns.Add(tableData[0, col]);
            }

            // Ajouter les lignes au DataTable
            for (int row = 1; row < rowCount; row++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int col = 0; col < colCount; col++)
                {
                    dataRow[col] = tableData[row, col];
                }
                dataTable.Rows.Add(dataRow);
            }

            // Créer un contrôle DataGridView
            DataGridView dataGridView = new DataGridView();
            dataGridView.DataSource = dataTable;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView.RowHeadersVisible = false;
            dataGridView.DefaultCellStyle.Font = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Regular);

            // Créer une nouvelle fenêtre de formulaire pour afficher le tableau
            Form tableForm = new Form();
            tableForm.Text = "Informations Requêtes par secondes";
            tableForm.Padding = new Padding(10); // Ajouter un padding de 10 pixels
            tableForm.StartPosition = FormStartPosition.CenterScreen; // Centrer la fenêtre sur l'écran
            tableForm.FormBorderStyle = FormBorderStyle.FixedSingle; // Rendre la fenêtre non redimensionnable
            tableForm.MaximizeBox = false; // Désactiver le bouton d'agrandissement

            dataGridView.Dock = DockStyle.Fill;
            tableForm.Controls.Add(dataGridView);

            // Redimensionner la fenêtre en fonction du contenu du DataGridView
            tableForm.ClientSize = new Size(600 + tableForm.Padding.Horizontal,
                                            270 + tableForm.Padding.Vertical);

            // Afficher la fenêtre du tableau
            tableForm.ShowDialog();
        }  

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                pt = "-p-";
                updTextBox1();
            }
            else
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                upport();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                sv = "-sV";
                updTextBox1();
            }
            else
            {
                sv = "";
                updTextBox1();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                sc = "-sC";
                updTextBox1();
            }
            else
            {
                sc = "";
                updTextBox1();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void upport()
        {
            int portA = (int)numericUpDown1.Value;
            int portB = (int)numericUpDown2.Value;
            if (portA == portB && portA == 0)
            {
                pt = "";
            }
            else if (portA == portB && portA != 0)
            {
                pt = "-p"+ portA;
            }
            else if (portA != portB && portA != 0)
            {
                pt = "-p " + portA+"-"+ portB;
            }
            updTextBox1();

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            upport();

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            upport();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Si les 2 Ports = 0 alors faires les ports les plus connus.\nSi les 2 ports = le même alors nmap que ce port spécifique\nEntre Range Alors nmap tout les ports compris.\n\"-p-\" Activé alors tout les ports sont nmap", "Information du port range", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void label17_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            trackBar1.Value = 4;
            pt = "-p-";
            sc = "";
            sv = "-sV";
            tn = "T4";
            updTextBox1();
        }
    }
}
