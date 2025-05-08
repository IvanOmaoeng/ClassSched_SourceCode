using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response; 

namespace Class_Schedule_and_Room_Requesting
{
    public partial class Homepage : Form
    {
        FirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "x84FJrIkTkAT9upbZates0a9Qi1tTaHu0CelfCZP",
            BasePath = "https://class-s-database-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        public Homepage()
        {
            InitializeComponent();
            Sidepanel1.Height = button8.Height;
            Sidepanel1.Top = button8.Height;
            firstCustomControl1.BringToFront();

        }

        private void Homepage_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Successfully Connection Established");
            }

        }
   

        private void button7_Click(object sender, EventArgs e)
        {
            Sidepanel1.Height = button7.Height;
            Sidepanel1.Top = button7.Top;
            secondCustomControl1.BringToFront(); 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Sidepanel1.Height = button6.Height;
            Sidepanel1.Top = button6.Top;
            thirdCustomControl1.BringToFront(); 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Sidepanel1.Height = button5.Height;
            Sidepanel1.Top = button5.Top;
            fourtCustomControl1.BringToFront(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sidepanel1.Height = button1.Height;
            Sidepanel1.Top = button1.Top;
            fifthCustomControl1.BringToFront();        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("are you sure you want to log out?", "confirm log out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);  
            
            if (result == DialogResult.Yes)
            {
                Form1 LoginForm = new Form1();
                LoginForm.Show();
                
                this.Close(); 
            }
            else
            {

            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            Sidepanel1.Height = button8.Height;
            Sidepanel1.Top = button8.Top;
            firstCustomControl1.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Sidepanel1.Height = button2.Height;
            Sidepanel1.Top = button2.Top;
            sixCustomControl1.BringToFront(); 
            
        }

        private void sixCustomControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
