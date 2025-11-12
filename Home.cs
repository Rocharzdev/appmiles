using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsmilhasApp.Views
{
    public partial class Home : Form
    {
        public Home()
        {
            
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Home));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(58, 61);
            label1.Name = "label1";
            label1.Size = new Size(484, 88);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(58, 176);
            label2.Name = "label2";
            label2.Size = new Size(484, 90);
            label2.TabIndex = 1;
            label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(59, 290);
            label3.Name = "label3";
            label3.Size = new Size(483, 43);
            label3.TabIndex = 2;
            label3.Text = "Educação que funciona: cursos, conteúdos e workshops para você aprender, no seu ritmo, como acumular, multiplicar e usar milhas com inteligência.";
            // 
            // Home
            // 
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1425, 726);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Home";
            ResumeLayout(false);

        }
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
