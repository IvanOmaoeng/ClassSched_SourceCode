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
using FireSharp.Response;
using FireSharp.Interfaces; 

namespace Class_Schedule_and_Room_Requesting
{
    public partial class FifthCustomControl : UserControl
    {
        DataTable dt = new DataTable();
        IFirebaseClient client;
        public FifthCustomControl()
        {
            InitializeComponent();
             FirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "x84FJrIkTkAT9upbZates0a9Qi1tTaHu0CelfCZP",
                BasePath = "https://class-s-database-default-rtdb.firebaseio.com/"
            };

            client = new FireSharp.FirebaseClient(config);

            if (client == null)
            {
                MessageBox.Show("Failed to connect to Firebase.");
            }

            dt.Columns.Add("ID No.");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Email");
  
            dataGridView1.DataSource = dt;

        }

        private void FifthCustomControl_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
