using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Class_Schedule_and_Room_Requesting
{
    public partial class FirstCustomControl : UserControl
    {
        private IFirebaseClient client;

        public FirstCustomControl()
        {
            InitializeComponent();
            timer1.Start();

            FirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "x84FJrIkTkAT9upbZates0a9Qi1tTaHu0CelfCZP",
                BasePath = "https://class-s-database-default-rtdb.firebaseio.com/"
            };

            client = new FireSharp.FirebaseClient(config);

            if (client == null)
            {
                MessageBox.Show("Failed to connect to Firebase.");
                return;
            }

            // Load user data
            LoadUserData();
        }

        // Method to load user data from Firebase
        private void LoadUserData()
        {
            try
            {
                // Retrieve student data
                FirebaseResponse studentResponse = client.Get("students");
                if (studentResponse.Body != "null")
                {
                    var studentData = studentResponse.ResultAs<Dictionary<string, object>>();
                    lblStudentTotal.Text = studentData?.Count.ToString() ?? "0";
                }
                else
                {
                    lblStudentTotal.Text = "0";
                }

                // Retrieve instructor data
                FirebaseResponse instructorResponse = client.Get("instructors");
                if (instructorResponse.Body != "null")
                {
                    var instructorData = instructorResponse.ResultAs<Dictionary<string, object>>();
                    lblInstructorTotal.Text = instructorData?.Count.ToString() ?? "0";
                }
                else
                {
                    lblInstructorTotal.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            {
                daylbl.Text = DateTime.Now.ToString("dddd");
                Timelbl.Text = DateTime.Now.ToString("HH:MM");
                pmlbl.Text = DateTime.Now.ToString("tt");
                Yearlbl.Text = DateTime.Now.ToString("yyyy");
                Secolabel.Text = DateTime.Now.ToString("ss");
                Datelbl.Text = DateTime.Now.ToString("MMM:dd");
            }
        }
        private void FirstCustomControl_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
