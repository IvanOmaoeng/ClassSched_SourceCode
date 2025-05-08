using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Class_Schedule_and_Room_Requesting
{
    public partial class SecondCustomControl : UserControl
    {
        // Initialize DataTable for DataGridView
        DataTable dt = new DataTable();
        IFirebaseClient client;

        public SecondCustomControl()
        {
            InitializeComponent();

            // Firebase Configuration
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

            // Setup DataTable Columns
            dt.Columns.Add("Name");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Instructors ID no");
            dt.Columns.Add("Email");
            dt.Columns.Add("Password");

            // Bind DataTable to DataGridView
            dataGridView1.DataSource = dt;
        }

        // Add Data to Firebase
        private async void button1_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Name = textBox1.Text,
                Lastname = textBox2.Text,
                instructorsIDno = textBox3.Text,
                Email = textBox4.Text,
                Password = textBox5.Text
            };

            if (client == null)
            {
                MessageBox.Show("Firebase client is not initialized.");
                return;
            }

            try
            {
                SetResponse response = await client.SetTaskAsync("instructors/" + textBox3.Text, data);
                MessageBox.Show("Data Successfully Inserted: " + response.ResultAs<Data>().Name);

                // Clear Input Fields
                ClearFields();

                // Reload Data
                await LoadDataFromFirebase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while adding data: " + ex.Message);
            }
        }

        // Load Data from Firebase
        private async Task LoadDataFromFirebase()
        {
            try
            {
                FirebaseResponse response = await client.GetTaskAsync("instructors/");
                Dictionary<string, Data> allData = response.ResultAs<Dictionary<string, Data>>();

                dt.Rows.Clear(); // Clear previous rows

                if (allData != null)
                {
                    foreach (var item in allData)
                    {
                        Data obj = item.Value;
                        dt.Rows.Add(obj.Name, obj.Lastname, obj.instructorsIDno, obj.Email, obj.Password);
                    }

                    MessageBox.Show("Data Loaded Successfully");
                }
                else
                {
                    MessageBox.Show("No data found in the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading data: " + ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FirebaseResponse response = await client.GetTaskAsync("instructors/" + textBox3.Text);
                Data obj = response.ResultAs<Data>();

                if (obj != null)
                {
                    textBox1.Text = obj.Name;
                    textBox2.Text = obj.Lastname;
                    textBox3.Text = obj.instructorsIDno;
                    textBox4.Text = obj.Email;
                    textBox5.Text = obj.Password;

                    MessageBox.Show("Data Retrieved Successfully");
                }
                else
                {
                    MessageBox.Show("No data found for the given ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving data: " + ex.Message);
            }
        }

        // Delete Specific Data from Firebase
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                FirebaseResponse response = await client.DeleteTaskAsync("instructors/" + textBox3.Text);
                MessageBox.Show($"Record with ID {textBox3.Text} Deleted Successfully");

                // Reload Data
                await LoadDataFromFirebase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting data: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Reload data from Firebase
                await LoadDataFromFirebase();
                MessageBox.Show("Data Reloaded Successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while reloading data: " + ex.Message);
            }
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            var data = new Data
            {
                Name = textBox1.Text,
                Lastname = textBox2.Text,
                instructorsIDno = textBox3.Text,
                Email = textBox4.Text,
                Password = textBox5.Text
            };

            try
            {
                FirebaseResponse response = await client.UpdateTaskAsync("instructors/" + textBox3.Text, data); 
                MessageBox.Show("Data Updated Successfully");


                ClearFields();


                await LoadDataFromFirebase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating data: " + ex.Message);
            }
        }
    }
}
