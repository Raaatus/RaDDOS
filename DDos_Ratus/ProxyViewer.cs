using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDos_Ratus
{
    internal class ProxyViewer
    {
        public static void NewWindowsProxy(List<(string IP, int Port)> list)
        {
            Form proxyForm = new Form();
            proxyForm.Text = "Proxy List";
            proxyForm.StartPosition = FormStartPosition.CenterScreen;
            proxyForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            proxyForm.MaximizeBox = false;

            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Font = new System.Drawing.Font("Consolas", 12);
            richTextBox.ReadOnly = true;

            foreach (var proxy in list)
            {
                richTextBox.AppendText(proxy.IP + ":" + proxy.Port + Environment.NewLine);
            }

            proxyForm.Controls.Add(richTextBox);
            proxyForm.ClientSize = new Size(450, 350);

            proxyForm.ShowDialog();
        }
    }
}
