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
    public partial class ThirdCustomControl : UserControl
    {
        DataTable dt = new DataTable();
        IFirebaseClient client;

        public ThirdCustomControl()
        {
            InitializeComponent();
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "x84FJrIkTkAT9upbZates0a9Qi1tTaHu0CelfCZP",
                BasePath = "https://class-s-database-default-rtdb.firebaseio.com/"
            };
            client = new FireSharp.FirebaseClient(config);

            if (client == null)
            {
                MessageBox.Show("Failed to connect to Firebase.");
            }

            dt.Columns.Add("Section");
            dt.Columns.Add("Subject Code");
            dt.Columns.Add("Subject Description");
            dt.Columns.Add("Semester");
            dt.Columns.Add("Credits");
            dt.Columns.Add("Time");
            dt.Columns.Add("Day");
            dt.Columns.Add("Room");
            dt.Columns.Add("Faculty");

            dataGridView1.DataSource = dt;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Section = textBox1.Text,
                SubjectCode = textBox2.Text,
                SubjectDescription = textBox3.Text,
                Semester = textBox4.Text,
                Credits = textBox5.Text,
                Time = textBox6.Text,
                Day = textBox7.Text,
                Room = textBox8.Text,
                Faculty = textBox9.Text
            };
            if (client == null)
            {
                MessageBox.Show("Firebase client is not initialized.");
                return;
            }

            SetResponse respo = await client.SetTaskAsync("Schedule_Information/" + textBox1.Text, data);
            Data result = respo.ResultAs<Data>();
            MessageBox.Show("Data Successfully Inserted: " + result.Section);



            ClearTextBoxes();

            await LoadDataFromFirebase();
        }

        private async Task LoadDataFromFirebase()
        {
            try
            {
                FirebaseResponse response = await client.GetTaskAsync("Schedule_Information/");


                Console.WriteLine(response.Body);

                if (string.IsNullOrEmpty(response.Body))
                {
                    MessageBox.Show("No data found in the database.");
                    return;
                }

                Dictionary<string, Data> allData = response.ResultAs<Dictionary<string, Data>>();

                dt.Rows.Clear();

                if (allData != null)
                {
                    foreach (var item in allData)
                    {
                        Data obj = item.Value;
                        dt.Rows.Add(
                            obj.Section,
                            obj.SubjectCode,
                            obj.SubjectDescription,
                            obj.Semester,
                            obj.Credits,
                            obj.Time,
                            obj.Day,
                            obj.Room,
                            obj.Faculty
                        );
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
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Section = textBox1.Text,
                SubjectCode = textBox2.Text,
                SubjectDescription = textBox3.Text,
                Semester = textBox4.Text,
                Credits = textBox5.Text,
                Time = textBox6.Text,
                Day = textBox7.Text,
                Room = textBox8.Text,
                Faculty = textBox9.Text
            };

            FirebaseResponse response = await client.UpdateTaskAsync("Schedule_Information/" + textBox1.Text, data);
            Data result = response.ResultAs<Data>();

            if (result != null)
            {
                MessageBox.Show("Data Updated Successfully.");
                await LoadDataFromFirebase();
            }
            else
            {
                MessageBox.Show("Failed to update data. Please check if the record exists.");
            }
             ClearTextBoxes();

            await LoadDataFromFirebase();
        }
               
        private async void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter a valid section to delete.");
                return;
            }


            FirebaseResponse response = await client.GetTaskAsync("Schedule_Information/" + textBox1.Text);
            Data existingData = response.ResultAs<Data>();

            if (existingData == null)
            {
                MessageBox.Show("No data found for the given section. Delete failed.");
                return;
            }

            try
            {

                FirebaseResponse deleteResponse = await client.DeleteTaskAsync("Schedule_Information/" + textBox1.Text);

                if (deleteResponse != null)
                {
                    MessageBox.Show(textBox1.Text + " Deleted Record Successfully");
                    await LoadDataFromFirebase();
                }
                else
                {
                    MessageBox.Show("Failed to delete data. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting the data: " + ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetTaskAsync("Schedule_Information/" + textBox1.Text);
            Data obj = response.ResultAs<Data>();

            if (obj != null)
            {
                textBox1.Text = obj.Section;
                textBox2.Text = obj.SubjectCode;
                textBox3.Text = obj.SubjectDescription;
                textBox4.Text = obj.Semester;
                textBox5.Text = obj.Credits;
                textBox6.Text = obj.Time;
                textBox7.Text = obj.Day;
                textBox8.Text = obj.Room;
                textBox9.Text = obj.Faculty;

                MessageBox.Show("Data Retrieved Successfully");
            }
            else
            {
                MessageBox.Show("No data found for the given ID.");
            }
           
        }
        private void ClearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
        }
    }
    
}
