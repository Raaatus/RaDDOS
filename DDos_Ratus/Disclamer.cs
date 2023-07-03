using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDos_Ratus
{
    public partial class Disclamer : Form
    {

        public Disclamer()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            // Fermer le formulaire
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        
    }
}
