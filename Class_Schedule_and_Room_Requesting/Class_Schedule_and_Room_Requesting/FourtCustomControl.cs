using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using System.Threading.Tasks;

namespace Class_Schedule_and_Room_Requesting
{
    public partial class FourtCustomControl : UserControl
    {
        DataTable dt = new DataTable();
        IFirebaseClient client;

        public FourtCustomControl()
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

            dt.Columns.Add("Room No.");
            dt.Columns.Add("Date");
            dt.Columns.Add("Time");
            dt.Columns.Add("Section");
            dt.Columns.Add("Instructor");
            dt.Columns.Add("Email");
            dt.Columns.Add("Building");

            dataGridView1.DataSource = dt;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SaveData();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await UpdateData();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await DeleteData();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await RetrieveData();
        }

        private async Task SaveData()
        {
            if (!ValidateBuildingInput(out string databasePath)) return;

            var data = CreateDataFromInputs();

            try
            {
                SetResponse response = await client.SetTaskAsync(databasePath + textBox1.Text, data);
                Data result = response.ResultAs<Data>();
                MessageBox.Show($"Data Successfully Inserted into {databasePath}: " + result.Section);

                ClearTextBoxes();
                await LoadDataFromFirebase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving data: " + ex.Message);
            }
        }

        private async Task UpdateData()
        {
            if (!ValidateBuildingInput(out string databasePath)) return;

            var data = CreateDataFromInputs();

            try
            {
                FirebaseResponse response = await client.UpdateTaskAsync(databasePath + textBox1.Text, data);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating data: " + ex.Message);
            }
        }

        private async Task DeleteData()
        {
            if (!ValidateBuildingInput(out string databasePath)) return;

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter a valid Room No to delete.");
                return;
            }

            try
            {
                FirebaseResponse deleteResponse = await client.DeleteTaskAsync(databasePath + textBox1.Text);

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

        private async Task RetrieveData()
        {
            if (!ValidateBuildingInput(out string databasePath)) return;

            try
            {
                FirebaseResponse response = await client.GetTaskAsync(databasePath + textBox1.Text);
                Data obj = response.ResultAs<Data>();

                if (obj != null)
                {
                    PopulateInputsFromData(obj);
                    MessageBox.Show("Data Retrieved Successfully");
                }
                else
                {
                    MessageBox.Show("No data found for the given Room No.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving data: " + ex.Message);
            }
        }

        private async Task LoadDataFromFirebase()
        {
            dt.Rows.Clear();

            string[] databasePaths = { "OB_Rooms_Information/", "NB_Rooms_Information/" };

            try
            {
                foreach (string databasePath in databasePaths)
                {
                    FirebaseResponse response = await client.GetTaskAsync(databasePath);

                    if (string.IsNullOrEmpty(response.Body))
                    {
                        continue; // Skip if no data
                    }

                    Dictionary<string, Data> allData = response.ResultAs<Dictionary<string, Data>>();

                    if (allData != null)
                    {
                        foreach (var item in allData)
                        {
                            Data obj = item.Value;
                            dt.Rows.Add(
                                obj.RoomNo,
                                obj.Date,
                                obj.Time,
                                obj.Section,
                                obj.Intructor,
                                obj.Email,
                                obj.Building
                            );
                        }
                    }
                }

                MessageBox.Show("Data Loaded Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }
        }

        private Data CreateDataFromInputs()
        {
            return new Data
            {
                RoomNo = textBox1.Text,
                Date = textBox2.Text,
                Time = textBox3.Text,
                Section = textBox4.Text,
                Intructor = textBox5.Text,
                Email = textBox7.Text,
                Building = textBox6.Text
            };
        }

        private void PopulateInputsFromData(Data obj)
        {
            textBox1.Text = obj.RoomNo;
            textBox2.Text = obj.Date;
            textBox3.Text = obj.Time;
            textBox4.Text = obj.Section;
            textBox5.Text = obj.Intructor;
            textBox7.Text = obj.Email;
            textBox6.Text = obj.Building;
        }

        private bool ValidateBuildingInput(out string databasePath)
        {
            databasePath = "";
            if (textBox6.Text.Equals("Old", StringComparison.OrdinalIgnoreCase))
            {
                databasePath = "OB_Rooms_Information/";
            }
            else if (textBox6.Text.Equals("New", StringComparison.OrdinalIgnoreCase))
            {
                databasePath = "NB_Rooms_Information/";
            }
            else
            {
                MessageBox.Show("Invalid Building input. Please enter 'Old' or 'New'.");
                return false;
            }
            return true;
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
        }

        private async void button4_Click(object sender, EventArgs e)
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
    }

}
