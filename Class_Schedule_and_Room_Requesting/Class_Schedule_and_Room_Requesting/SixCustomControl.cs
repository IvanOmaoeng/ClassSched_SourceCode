using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace Class_Schedule_and_Room_Requesting
{
    public partial class SixCustomControl : UserControl
    {
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();
        IFirebaseClient client;

        public SixCustomControl()
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
                return;
            }

            InitializeDataTable(dt1);
            InitializeDataTable(dt2);

            dataGridView1.DataSource = dt1;
            dataGridView2.DataSource = dt2;

            AddButtonsToDataGridView(dataGridView1);
            AddButtonsToDataGridView(dataGridView2);

            RefreshData();
        }

        private void InitializeDataTable(DataTable dt)
        {
            dt.Columns.Add("RoomNumber");
            dt.Columns.Add("Email");
            dt.Columns.Add("Instructor");
            dt.Columns.Add("Section");
            dt.Columns.Add("Status");
            dt.Columns.Add("Time");
            dt.Columns.Add("Timestamp");
            dt.Columns.Add("Date");
        }

        private async void LoadDataFromFirebase(string node, DataTable dt)
        {
            try
            {
                // Clear DataTable before loading data to avoid duplication
                dt.Rows.Clear();

                // Fetch data from Firebase for the given node
                FirebaseResponse response = await client.GetTaskAsync(node);
                if (response.Body != "null")
                {
                    // Deserialize the Firebase JSON response
                    var roomData = response.ResultAs<Dictionary<string, Dictionary<string, RoomInfo>>>();

                    // Traverse nested data structure
                    foreach (var room in roomData)
                    {
                        foreach (var details in room.Value.Values)
                        {
                            dt.Rows.Add(
                                details.RoomNumber,
                                details.Email,
                                details.Instructor,
                                details.Section,
                                details.Status,
                                details.Time,
                                details.Timestamp,
                                details.Date
                            );
                        }
                    }

                    // Refresh the DataGridView to display the new data
                    dataGridView1.Refresh();
                    dataGridView2.Refresh();
                }
                else
                {
                    MessageBox.Show($"No data found in the '{node}' node.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data from '{node}': {ex.Message}");
            }
        }


        private void AddButtonsToDataGridView(DataGridView dataGridView)
        {
            DataGridViewButtonColumn approvedButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Approved",
                Text = "Approve",
                UseColumnTextForButtonValue = true
            };
            dataGridView.Columns.Add(approvedButtonColumn);

            DataGridViewButtonColumn declineButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Decline",
                Text = "Decline",
                UseColumnTextForButtonValue = true
            };
            dataGridView.Columns.Add(declineButtonColumn);

            dataGridView.CellContentClick += DataGridView_CellContentClick;
        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dataGridView.Columns[e.ColumnIndex].Name;

                if (columnName == "Approved")
                {
                    DataRow row = (dataGridView.DataSource as DataTable)?.Rows[e.RowIndex];
                    if (row != null)
                    {
                        string nodeToRemove = dataGridView == dataGridView1 ? "NB_Requesting_Classroom" : "OB_Requesting_Classroom";
                        string nodeToAdd = dataGridView == dataGridView1 ? "NB_Rooms_Information" : "OB_Rooms_Information";

                        ApproveRoomRequest(nodeToRemove, nodeToAdd, row);
                    }
                }
                else if (columnName == "Decline")
                {
                    string roomNumber = dataGridView.Rows[e.RowIndex].Cells["RoomNumber"].Value.ToString();
                    string requestingNode = dataGridView == dataGridView1 ? "NB_Requesting_Classroom" : "OB_Requesting_Classroom";

                    DeclineRoomRequest(requestingNode, roomNumber);
                }
            }
        }

        private async void ApproveRoomRequest(string requestingNode, string approvedNode, DataRow row)
        {
            try
            {
                // Create a RoomInfo object from the DataRow
                var roomInfo = new RoomInfo
                {
                    RoomNumber = row["RoomNumber"].ToString(),
                    Email = row["Email"].ToString(),
                    Instructor = row["Instructor"].ToString(),
                    Section = row["Section"].ToString(),
                    Status = "Approved",
                    Time = row["Time"].ToString(),
                    Timestamp = row["Timestamp"].ToString(),
                    Date = row["Date"].ToString()
                };

                // Add the approved request to the approved node in Firebase
                await client.SetTaskAsync($"{approvedNode}/{roomInfo.RoomNumber}", roomInfo);

                // Remove the request from the requesting node in Firebase
                await client.DeleteTaskAsync($"{requestingNode}/{roomInfo.RoomNumber}");

                // Remove the row from the DataTable
                row.Delete();

                MessageBox.Show($"Room {roomInfo.RoomNumber} approved successfully.");

                // Refresh the DataTable to reflect changes
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving room request: {ex.Message}");
            }
        }

        private async void DeclineRoomRequest(string requestingNode, string roomNumber)
        {
            try
            {
                // Remove the room request from Firebase
                await client.DeleteTaskAsync($"{requestingNode}/{roomNumber}");

                // Refresh the DataTable
                RefreshData();

                MessageBox.Show($"Room {roomNumber} declined successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error declining room request: {ex.Message}");
            }
        }

        private void RefreshData()
        {
            // Reload data from Firebase for both tables
            LoadDataFromFirebase("NB_Requesting_Classroom", dt1);
            LoadDataFromFirebase("OB_Requesting_Classroom", dt2);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            LoadDataFromFirebase("NB_Requesting_Classroom", dt1);
            LoadDataFromFirebase("OB_Requesting_Classroom", dt2);
        }

        public class RoomInfo
        {
            public string RoomNumber { get; set; }
            public string Email { get; set; }
            public string Instructor { get; set; }
            public string Section { get; set; }
            public string Status { get; set; }
            public string Time { get; set; }
            public string Timestamp { get; set; }
            public string Date { get; set; }
        }
    }
}
