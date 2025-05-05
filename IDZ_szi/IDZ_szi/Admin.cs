// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IDZ_szi
{
    public partial class Admin : Form
    {
        private SqlConnection connection;
        private string connectionString = "Data Source=LAPTOP-JSVR0I7M;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";

        public Admin()
        {
            InitializeComponent();
        }
        private void Admin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Input input = new Input();
            input.Show();
            Hide();
        }
        private void Admin_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            FillPositionsComboBox(); FillDepartmentsComboBox(); FillEmployeesComboBox(); FillWorkComboBox();
            FillComplexComboBox(); FillSZIComboBox(); PopulateClassifierComboBox(); FillRequestDatesComboBox();
            FillRequestDatesComplexComboBox(); PopulateProviderComboBox();
        }
        private void LoadEmployees()
        {
            string query = "SELECT Employee.Surname_Employee, Employee.Name_Employee, Employee.Patronymic_Employee, " +
                           "Post.Title_Post, Department.Title_Department, Employee.Email_Employee, Employee.Telephone_Employee, Employee.Address_number " +
                           "FROM Employee " +
                           "INNER JOIN Post ON Employee.PostID = Post.ID_Post " +
                           "INNER JOIN [Department and employee] ON Employee.ID_Employee = [Department and employee].EmployeeID " +
                           "INNER JOIN Department ON [Department and employee].DepartmentID = Department.ID_Department";

            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataTable.Load(reader);

            dataTable.Columns[0].ColumnName = "фамилия";
            dataTable.Columns[1].ColumnName = "имя";
            dataTable.Columns[2].ColumnName = "отчество";
            dataTable.Columns[3].ColumnName = "должность";
            dataTable.Columns[4].ColumnName = "отдел";
            dataTable.Columns[5].ColumnName = "email";
            dataTable.Columns[6].ColumnName = "телефон";
            dataTable.Columns[7].ColumnName = "адрес";

            dataGridView1.DataSource = dataTable;
            reader.Close();
        }
        private void ClearFields()
        {
            textBox13.Text = ""; textBox14.Text = ""; textBox15.Text = ""; textBox16.Text = "";
            textBox17.Text = ""; textBox18.Text = ""; textBox19.Text = "";
            comboBox14.SelectedItem = null; comboBox15.SelectedItem = null;
        }
        private void FillPositionsComboBox()
        {
            string query = "SELECT Title_Post FROM Post";
            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string position = reader.GetString(0);
                    comboBox14.Items.Add(position);
                }
            }
        }
        private void FillDepartmentsComboBox()
        {
            string query = "SELECT Title_Department FROM Department";
            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string department = reader.GetString(0);
                    comboBox15.Items.Add(department);
                }
            }
        }
        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Title_Work, Price_Work, Period " +
                               "FROM Work";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns[0].ColumnName = "название";
                dataTable.Columns[1].ColumnName = "цена";
                dataTable.Columns[2].ColumnName = "период";

                dataGridView6.DataSource = dataTable;
            }
        }
        private void FillEmployeesComboBox()
        {
            comboBox16.Items.Clear();
            comboBox5.Items.Clear();
            string query = "SELECT CONCAT(Surname_Employee, ' ', Name_Employee, ' ', Patronymic_Employee) FROM Employee";

            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string employeeName = reader.GetString(0);
                    comboBox16.Items.Add(employeeName);
                    comboBox5.Items.Add(employeeName);
                }
            }
        }
        private void FillComplexComboBox()
        {
            string query = "SELECT ID_Complex, Title_Complex FROM [Complex solution]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string complexName = reader.GetString(1);
                        comboBox12.Items.Add(complexName);
                        comboBox13.Items.Add(complexName);
                        comboBox11.Items.Add(complexName);
                    }
                }
            }
        }
        private void FillWorkComboBox()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Title_Work FROM Work";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string title = reader.GetString(0);
                    comboBox10.Items.Add(title);
                }
            }
        }
        private void FillSZIComboBox()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID_Szi, Title_szi FROM Szi";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string title = reader.GetString(1);
                    comboBox6.Items.Add(title);
                    comboBox4.Items.Add(title);
                    comboBox1.Items.Add(title);
                }
            }
        }
        private void PopulateProviderComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Totle_Provider FROM Provider";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    comboBox8.Items.Clear();
                    while (reader.Read())
                    {
                        string providerName = reader.GetString(0);
                        comboBox8.Items.Add(providerName);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PopulateClassifierComboBox()
        {
            comboBox7.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Title_Classifier FROM Classifier";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    comboBox7.Items.Clear();

                    while (reader.Read())
                    {
                        string classifierTitle = reader.GetString(0);
                        comboBox7.Items.Add(classifierTitle);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string complexTitle = textBox8.Text;
            string employeeName = comboBox5.SelectedItem.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string EmployeeIdQuery = "SELECT ID_Employee FROM Employee WHERE CONCAT(Surname_Employee, ' ', Name_Employee, ' ', Patronymic_Employee) = @EmployeeName";
                    using (SqlCommand EmployeeIdCommand = new SqlCommand(EmployeeIdQuery, connection))
                    {
                        EmployeeIdCommand.Parameters.AddWithValue("@EmployeeName", employeeName);

                        connection.Open();
                        int employeeID = (int)EmployeeIdCommand.ExecuteScalar();
                        string ComplexQuery = "INSERT INTO [Complex solution] (Title_Complex, EmployeeID) VALUES (@ComplexTitle, @EmployeeID)";

                        using (SqlCommand insertComplex = new SqlCommand(ComplexQuery, connection))
                        {
                            insertComplex.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                            insertComplex.Parameters.AddWithValue("@EmployeeID", employeeID);
                            insertComplex.ExecuteNonQuery();
                        }
                        UpdateComplexSolutionsTable(connection);
                    }
                    MessageBox.Show("Комплексное решение успешно добавлено в базу данных!", "Успешно", MessageBoxButtons.OK);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            SZI szi = new SZI();
            szi.Show();
            Hide();
            FillRequestDatesComboBox();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            Complex complex = new Complex();
            complex.Show();
            Hide();
            FillRequestDatesComboBox();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            string title = textBox20.Text;
            decimal price = decimal.Parse(textBox21.Text);
            int period = int.Parse(textBox22.Text);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Work (Title_Work, Price_Work, Period) VALUES (@Title, @Price, @Period)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Period", period);
                connection.Open();
                command.ExecuteNonQuery();
            }
            comboBox10.Items.Clear();
            FillWorkComboBox();
            MessageBox.Show("Работа была успешно добавлена!", "Успешно", MessageBoxButtons.OK);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox10.SelectedItem != null)
            {
                string selectedWork = comboBox10.SelectedItem.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string Query = "SELECT ID_Work FROM Work WHERE Title_Work = @WorkTitle";
                    SqlCommand workIDCommand = new SqlCommand(Query, connection);
                    workIDCommand.Parameters.AddWithValue("@WorkTitle", selectedWork);
                    int workID = (int)workIDCommand.ExecuteScalar();

                    string WorkReq = "DELETE FROM [Work on the request] WHERE WorkID = @WorkID";
                    SqlCommand WorkReqCommand = new SqlCommand(WorkReq, connection);
                    WorkReqCommand.Parameters.AddWithValue("@WorkID", workID);
                    WorkReqCommand.ExecuteNonQuery();

                    string ComplexReq = "DELETE FROM [Complex and Request] WHERE ComplexxID IN (SELECT ID_ComplexSZI FROM [Complex solution and SZI] WHERE SziID = @WorkID)";
                    SqlCommand ComplexReqCommand = new SqlCommand(ComplexReq, connection);
                    ComplexReqCommand.Parameters.AddWithValue("@WorkID", workID);
                    ComplexReqCommand.ExecuteNonQuery();

                    string deleteWork = "DELETE FROM Work WHERE ID_Work = @WorkID";
                    SqlCommand WorkCommand = new SqlCommand(deleteWork, connection);
                    WorkCommand.Parameters.AddWithValue("@WorkID", workID);
                    WorkCommand.ExecuteNonQuery();
                }
                comboBox10.Items.Clear();
                FillWorkComboBox();

                MessageBox.Show("Работа успешно удалена!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Для удаления необходимо выбрать работу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            string complexTitle = comboBox12.SelectedItem?.ToString();
            string sziTitle = comboBox6.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(complexTitle) || string.IsNullOrEmpty(sziTitle))
            {
                MessageBox.Show("Пожалуйста, выберите комплексное решение и СЗИ");
                return; 
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string ComplexIDQuery = $"SELECT ID_Complex FROM [Complex solution] WHERE Title_Complex = @ComplexTitle";
                    using (SqlCommand ComplexIDCommand = new SqlCommand(ComplexIDQuery, connection))
                    {
                        ComplexIDCommand.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                        int complexID = (int)ComplexIDCommand.ExecuteScalar();

                        if (complexID == 0)
                        {
                            MessageBox.Show("Комплексного решения с указанным названием не существует");
                            return; 
                        }
                        string SziIDQuery = $"SELECT ID_Szi FROM Szi WHERE Title_szi = @SziTitle";
                        using (SqlCommand SziIDCommand = new SqlCommand(SziIDQuery, connection))
                        {
                            SziIDCommand.Parameters.AddWithValue("@SziTitle", sziTitle);
                            int sziID = (int)SziIDCommand.ExecuteScalar();

                            if (sziID == 0)
                            {
                                MessageBox.Show("СЗИ с указанным названием не существует");
                                return; 
                            }
                            string ComplexSziQuery = $"INSERT INTO [Complex solution and SZI] (ComplexID, SziID) VALUES (@ComplexID, @SziID)";
                            using (SqlCommand ComplexSziCommand = new SqlCommand(ComplexSziQuery, connection))
                            {
                                ComplexSziCommand.Parameters.AddWithValue("@ComplexID", complexID);
                                ComplexSziCommand.Parameters.AddWithValue("@SziID", sziID);
                                ComplexSziCommand.ExecuteNonQuery();
                            }
                        }
                        UpdateComplexSolutionsTable(connection);
                        MessageBox.Show("Комплексное решение успешно собрано!", "Успешно", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string complexTitle = comboBox13.SelectedItem.ToString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string ComplexIDQuery = "SELECT ID_Complex FROM [Complex solution] WHERE Title_Complex = @ComplexTitle";
                    using (SqlCommand ComplexIDCommand = new SqlCommand(ComplexIDQuery, connection))
                    {
                        ComplexIDCommand.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                        int complexID = (int)ComplexIDCommand.ExecuteScalar();

                        if (complexID == 0)
                        {
                            MessageBox.Show("Комплексного решения с выбранным названием не существует");
                            return;
                        }
                        string ComplexSziQuery = "DELETE FROM [Complex solution and SZI] WHERE ComplexID = @ComplexID";
                        using (SqlCommand ComplexSziCommand = new SqlCommand(ComplexSziQuery, connection))
                        {
                            ComplexSziCommand.Parameters.AddWithValue("@ComplexID", complexID);
                            ComplexSziCommand.ExecuteNonQuery();
                        }
                        string ComplexQuery = "DELETE FROM [Complex solution] WHERE ID_Complex = @ComplexID";
                        using (SqlCommand ComplexCommand = new SqlCommand(ComplexQuery, connection))
                        {
                            ComplexCommand.Parameters.AddWithValue("@ComplexID", complexID);
                            ComplexCommand.ExecuteNonQuery();
                        }
                        UpdateComplexSolutionsTable(connection);

                        comboBox11.Items.Clear(); comboBox12.Items.Clear();comboBox13.Items.Clear();
                        FillComplexComboBox();
                        UpdateComplexSolutionsTable(connection);
                        MessageBox.Show("Комплексное решение успешно удалено!", "Успешно", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT cs.Title_Complex, e.Surname_Employee, e.Name_Employee, e.Patronymic_Employee " +
                        "FROM [Complex solution] cs " +
                        "LEFT JOIN Employee e ON cs.EmployeeID = e.ID_Employee";

                    SqlCommand command = new SqlCommand(selectQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataTable.Columns[0].ColumnName = "название";
                    dataTable.Columns[1].ColumnName = "фамилия";
                    dataTable.Columns[2].ColumnName = "имя";
                    dataTable.Columns[3].ColumnName = "отчество";

                    dataGridView5.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateComplexSolutionsTable(SqlConnection connection)
        {
            try
            {
                string selectQuery = "SELECT cs.Title_Complex, e.Surname_Employee, e.Name_Employee, e.Patronymic_Employee " +
                    "FROM [Complex solution] cs " +
                    "LEFT JOIN Employee e ON cs.EmployeeID = e.ID_Employee " +
                    "GROUP BY cs.ID_Complex, cs.Title_Complex, e.Surname_Employee, e.Name_Employee, e.Patronymic_Employee";
                SqlCommand command = new SqlCommand(selectQuery, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns[0].ColumnName = "название";
                dataTable.Columns[1].ColumnName = "фамилия";
                dataTable.Columns[2].ColumnName = "имя";
                dataTable.Columns[3].ColumnName = "отчество";

                dataGridView5.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (comboBox11.SelectedItem != null)
            {
                string selectedComplexTitle = comboBox11.SelectedItem.ToString();
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string selectQuery = "SELECT s.Title_szi, s.Description_szi " +
                            "FROM Szi s " +
                            "JOIN [Complex solution and SZI] cszi ON s.ID_Szi = cszi.SziID " +
                            "JOIN [Complex solution] cs ON cszi.ComplexID = cs.ID_Complex " +
                            "WHERE cs.Title_Complex = @ComplexTitle";

                        SqlCommand command = new SqlCommand(selectQuery, connection);
                        command.Parameters.AddWithValue("@ComplexTitle", selectedComplexTitle);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        adapter.Fill(dataTable);

                        dataTable.Columns[0].ColumnName = "СЗИ";
                        dataTable.Columns[1].ColumnName = "описание";

                        dataGridView4.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT CONCAT(b.Surname_Buyer, ' ', b.Name_Buyer, ' ', b.Patronymic_Buyer) AS 'ФИО покупателя', " +
                        "SUM(COALESCE(w.Price_Work, 0) + (w2.Wholesale_price * c.Ratio) * rs.Сount_SZI) AS 'Стоимость заказа' " +
                        "FROM Request r " +
                        "JOIN Buyer b ON r.BuyerID = b.ID_Buyer " +
                        "LEFT JOIN [Request and SZI] rs ON r.ID_Request = rs.RequestID " +
                        "LEFT JOIN Szi s ON rs.SziID = s.ID_Szi " +
                        "LEFT JOIN [Classifier and SZI] cs ON s.ID_Szi = cs.SziID " +
                        "LEFT JOIN Classifier c ON cs.ClassifierID = c.ID_Classifier " +
                        "LEFT JOIN [Work on the request] wr ON r.ID_Request = wr.RequestID " +
                        "LEFT JOIN Work w ON wr.WorkID = w.ID_Work " +
                        "LEFT JOIN Warehouse w2 ON s.ID_Szi = w2.SziID " +
                        "WHERE r.ID_Request NOT IN (SELECT RequestID FROM Payment) AND r.Date_Request > '2023-05-24' " +
                        "AND s.ID_Szi IS NOT NULL " +
                        "GROUP BY r.ID_Request, b.Surname_Buyer, b.Name_Buyer, b.Patronymic_Buyer";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);
                    dataGridView3.Columns.Clear();

                    dataGridView3.Columns.Add("ФИО покупателя", "ФИО покупателя");
                    dataGridView3.Columns.Add("Стоимость заказа", "стоимость заказа");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string buyerName = row["ФИО покупателя"].ToString();
                        string orderCost = row["Стоимость заказа"].ToString();

                        dataGridView3.Rows.Add(buyerName, orderCost);
                    }
                    dataGridView3.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string selectedClassifier = comboBox7.SelectedItem.ToString();

                    string SziQuery = "INSERT INTO Szi (Title_szi, Description_szi) VALUES (@Title, @Description); SELECT SCOPE_IDENTITY();";
                    SqlCommand SziCommand = new SqlCommand(SziQuery, connection);
                    SziCommand.Parameters.AddWithValue("@Title", textBox9.Text);
                    SziCommand.Parameters.AddWithValue("@Description", textBox10.Text);
                    int sziId = Convert.ToInt32(SziCommand.ExecuteScalar());

                    string ClassifierIdQuery = "SELECT ID_Classifier FROM Classifier WHERE Title_Classifier = @Title";
                    SqlCommand ClassifierIdCommand = new SqlCommand(ClassifierIdQuery, connection);
                    ClassifierIdCommand.Parameters.AddWithValue("@Title", selectedClassifier);
                    int classifierId = Convert.ToInt32(ClassifierIdCommand.ExecuteScalar());

                    string ClassifierSziQuery = "INSERT INTO [Classifier and SZI] (SziID, ClassifierID) VALUES (@SziID, @ClassifierID)";
                    SqlCommand ClassifierSziCommand = new SqlCommand(ClassifierSziQuery, connection);
                    ClassifierSziCommand.Parameters.AddWithValue("@SziID", sziId);
                    ClassifierSziCommand.Parameters.AddWithValue("@ClassifierID", classifierId);
                    ClassifierSziCommand.ExecuteNonQuery();

                    PopulateClassifierComboBox();
                    MessageBox.Show("СЗИ успешно добавлено!", "Успешно", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT CONCAT(B.Surname_Buyer, ' ', B.Name_Buyer, ' ', B.Patronymic_Buyer) AS 'ФИО покупателя', " +
                                   "SUM((COALESCE(W.Price_Work, 0) + WS.Wholesale_price * C.Ratio)) AS 'Стоимость заказа' " +
                                   "FROM Request R " +
                                   "INNER JOIN Buyer B ON R.BuyerID = B.ID_Buyer " +
                                   "INNER JOIN [Complex and Request] CR ON R.ID_Request = CR.RequestID " +
                                   "INNER JOIN [Complex solution and SZI] CSZI ON CR.ComplexxID = CSZI.ComplexID " +
                                   "INNER JOIN Szi SZ ON CSZI.SziID = SZ.ID_Szi " +
                                   "INNER JOIN [Classifier and SZI] CS ON SZ.ID_Szi = CS.SziID " +
                                   "INNER JOIN Classifier C ON CS.ClassifierID = C.ID_Classifier " +
                                   "LEFT JOIN [Work on the request] WR ON R.ID_Request = WR.RequestID " +
                                   "LEFT JOIN Work W ON WR.WorkID = W.ID_Work " +
                                   "LEFT JOIN [Warehouse] WS ON SZ.ID_Szi = WS.SziID " +
                                   "WHERE R.ID_Request NOT IN (SELECT RequestID FROM Payment) " +
                                   "GROUP BY R.ID_Request, B.Surname_Buyer, B.Name_Buyer, B.Patronymic_Buyer;";

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataTable.Columns[0].ColumnName = "ФИО покупателя";
                    dataTable.Columns[1].ColumnName = "cтоимость заказа";


                    dataGridView8.DataSource = dataTable;
                    dataGridView8.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                int sziCount = Convert.ToInt32(textBox11.Text);
                decimal wholesalePrice = Convert.ToDecimal(textBox12.Text);
                string sziTitle = comboBox4.Text;
                DateTime deliveryDate = dateTimePicker1.Value;
                string providerName = comboBox8.Text;

                int sziID = -1;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Szi FROM Szi WHERE Title_szi = @Title";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Title", sziTitle);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        sziID = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                int providerID = -1;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Provider FROM Provider WHERE Totle_Provider = @ProviderName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProviderName", providerName);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        providerID = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Warehouse (SziID, Delivery_date, [Сount], Wholesale_price, ProviderID) " +
                                   "VALUES (@SziID, @DeliveryDate, @Count, @WholesalePrice, @ProviderID)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                    command.Parameters.AddWithValue("@Count", sziCount);
                    command.Parameters.AddWithValue("@WholesalePrice", wholesalePrice);
                    command.Parameters.AddWithValue("@ProviderID", providerID);
                    command.ExecuteNonQuery();
                }
                PopulateClassifierComboBox();
                MessageBox.Show("Запись успешно добавлена!", "Успешно", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                string sziTitle = comboBox1.Text;

                int sziID = -1;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_Szi FROM Szi WHERE Title_szi = @Title";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Title", sziTitle);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        sziID = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [Classifier and SZI] WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [Request and SZI] WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Warehouse WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Szi WHERE ID_Szi = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }
                PopulateClassifierComboBox();
                MessageBox.Show("СЗИ успешно удалено из базы данных!", "Успешно", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();

                dataGridView2.Columns.Add("ColumnSzi", "название");
                dataGridView2.Columns.Add("ColumnDescription", "описание");
                dataGridView2.Columns.Add("ColumnCount", "количество");
                dataGridView2.Columns.Add("ColumnProvider", "поставщик");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Szi.Title_szi, Szi.Description_szi, Warehouse.Сount, Provider.Totle_Provider " +
                                   "FROM Szi " +
                                   "INNER JOIN Warehouse ON Szi.ID_Szi = Warehouse.SziID " +
                                   "INNER JOIN Provider ON Warehouse.ProviderID = Provider.ID_Provider " +
                                   "WHERE Warehouse.Сount > 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    /*dataTable.Columns[0].ColumnName = "название";
                    dataTable.Columns[1].ColumnName = "цена";
                    dataTable.Columns[2].ColumnName = "период";*/

                    while (reader.Read())
                    {
                        string sziTitle = reader.GetString(0);
                        string sziDescription = reader.GetString(1);
                        int sziCount = reader.GetInt32(2);
                        string providerName = reader.GetString(3);

                        dataGridView2.Rows.Add(sziTitle, sziDescription, sziCount, providerName);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                string address = textBox13.Text;
                string telephone = textBox14.Text;
                string password = textBox15.Text;
                string email = textBox16.Text;
                string patronymic = textBox17.Text;
                string name = textBox18.Text;
                string surname = textBox19.Text;
                string selectedPost = comboBox14.SelectedItem?.ToString();
                string selectedDepartment = comboBox15.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedPost) || string.IsNullOrEmpty(selectedDepartment))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                    return;
                }
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Пожалуйста, введите корректный email.");
                    return;
                }
                /*if (!IsValidPhone(telephone))
                {
                    MessageBox.Show("Неверный формат телефона. Формат должен быть таким: +7 (___) ___-__-__");
                    return;
                }*/

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkUserQuery = "SELECT COUNT(*) FROM Employee WHERE Email_Employee = @UserMail";
                    using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@UserMail", email);
                        int userCount = (int)checkUserCommand.ExecuteScalar();
                        if (userCount > 0)
                        {
                            MessageBox.Show("Пользователь с таким email уже зарегистрирован", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    string hashedPassword = HashPassword(password);

                    string EmployeeQuery = "INSERT INTO Employee (Surname_Employee, Name_Employee, Patronymic_Employee, PostID, Email_Employee, Password_Employee, Telephone_Employee, Address_number) " +
                        "VALUES (@Surname, @Name, @Patronymic, @PostID, @Email, @Password, @Telephone, @Address)";
                    SqlCommand EmployeeCommand = new SqlCommand(EmployeeQuery, connection);
                    EmployeeCommand.Parameters.AddWithValue("@Surname", surname);
                    EmployeeCommand.Parameters.AddWithValue("@Name", name);
                    EmployeeCommand.Parameters.AddWithValue("@Patronymic", patronymic);

                    string PostIDQuery = "SELECT ID_Post FROM Post WHERE Title_Post = @PostTitle";
                    SqlCommand PostIDCommand = new SqlCommand(PostIDQuery, connection);
                    PostIDCommand.Parameters.AddWithValue("@PostTitle", selectedPost);
                    int postID = Convert.ToInt32(PostIDCommand.ExecuteScalar());

                    EmployeeCommand.Parameters.AddWithValue("@PostID", postID);
                    EmployeeCommand.Parameters.AddWithValue("@Email", email);
                    EmployeeCommand.Parameters.AddWithValue("@Password", hashedPassword);
                    EmployeeCommand.Parameters.AddWithValue("@Telephone", telephone);
                    EmployeeCommand.Parameters.AddWithValue("@Address", address);
                    EmployeeCommand.ExecuteNonQuery();

                    string LastEmployeeIDQuery = "SELECT IDENT_CURRENT('Employee')";
                    SqlCommand LastEmployeeIDCommand = new SqlCommand(LastEmployeeIDQuery, connection);
                    int lastEmployeeID = Convert.ToInt32(LastEmployeeIDCommand.ExecuteScalar());

                    string DepartmentIDQuery = "SELECT ID_Department FROM Department WHERE Title_Department = @DepartmentTitle";
                    SqlCommand DepartmentIDCommand = new SqlCommand(DepartmentIDQuery, connection);
                    DepartmentIDCommand.Parameters.AddWithValue("@DepartmentTitle", selectedDepartment);
                    int departmentID = Convert.ToInt32(DepartmentIDCommand.ExecuteScalar());

                    string DepartmentEmployeeQuery = "INSERT INTO [Department and employee] (EmployeeID, DepartmentID) " +
                                                           "VALUES (@EmployeeID, @DepartmentID)";
                    SqlCommand DepartmentEmployeeCommand = new SqlCommand(DepartmentEmployeeQuery, connection);
                    DepartmentEmployeeCommand.Parameters.AddWithValue("@EmployeeID", lastEmployeeID);
                    DepartmentEmployeeCommand.Parameters.AddWithValue("@DepartmentID", departmentID);
                    DepartmentEmployeeCommand.ExecuteNonQuery();

                    FillEmployeesComboBox();

                    MessageBox.Show("Данные сотрудника успешно добавлены в базу данных!", "Успешно", MessageBoxButtons.OK);
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false;}
        }
        private void FillRequestDatesComboBox()
        {
            comboBox2.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Request.ID_Request, Request.Date_Request, Buyer.Surname_Buyer, Buyer.Name_Buyer, Buyer.Patronymic_Buyer " +
                                   "FROM Request " +
                                   "LEFT JOIN Payment ON Request.ID_Request = Payment.RequestID " +
                                   "INNER JOIN Buyer ON Request.BuyerID = Buyer.ID_Buyer " +
                                   "INNER JOIN [Request and SZI] ON Request.ID_Request = [Request and SZI].RequestID " +
                                   "WHERE Request.Date_Request > '2023-05-24' " +
                                   "AND Payment.ID_Payment IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int requestID = reader.GetInt32(0);
                        DateTime requestDate = reader.GetDateTime(1);
                        string buyerSurname = reader.GetString(2);
                        string buyerName = reader.GetString(3);
                        string buyerPatronymic = reader.GetString(4);

                        string fullName = $"{buyerSurname} {buyerName} {buyerPatronymic}";
                        string displayText = $"{fullName} - {requestDate}";

                        comboBox2.Items.Add(displayText);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FillRequestDatesComplexComboBox()
        {
            comboBox17.Items.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Request.ID_Request, Request.Date_Request, Buyer.Surname_Buyer, Buyer.Name_Buyer, Buyer.Patronymic_Buyer " +
                                   "FROM Request " +
                                   "LEFT JOIN Payment ON Request.ID_Request = Payment.RequestID " +
                                   "INNER JOIN Buyer ON Request.BuyerID = Buyer.ID_Buyer " +
                                   "INNER JOIN [Complex and Request] ON Request.ID_Request = [Complex and Request].RequestID " +
                                   "WHERE Request.Date_Request > '2023-05-24' " +
                                   "AND Payment.ID_Payment IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int requestID = reader.GetInt32(0);
                        DateTime requestDate = reader.GetDateTime(1);
                        string buyerSurname = reader.GetString(2);
                        string buyerName = reader.GetString(3);
                        string buyerPatronymic = reader.GetString(4);

                        string fullName = $"{buyerSurname} {buyerName} {buyerPatronymic}";
                        string displayText = $"{fullName} - {requestDate}";

                        comboBox17.Items.Add(displayText);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button18_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectedText = comboBox2.SelectedItem.ToString();

                string[] selectedData = selectedText.Split(new string[] { " - " }, StringSplitOptions.None);
                string fullName = selectedData[0];
                DateTime requestDate = DateTime.Parse(selectedData[1]);

                string IdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand IdCommand = new SqlCommand(IdQuery, connection);
                IdCommand.Parameters.AddWithValue("@FullName", fullName);
                IdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)IdCommand.ExecuteScalar();

                string CountQuery = "SELECT rs.SziID, rs.Сount_SZI, w.Сount " +
                                            "FROM [Request and SZI] rs " +
                                            "JOIN Warehouse w ON rs.SziID = w.SziID " +
                                            "WHERE rs.RequestID = @RequestID";
                SqlCommand CountCommand = new SqlCommand(CountQuery, connection);
                CountCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                SqlDataReader CountReader = CountCommand.ExecuteReader();

                bool hasEnoughQuantity = true;
                List<string> insufficientSZIs = new List<string>();

                while (CountReader.Read())
                {
                    int requestedCount = Convert.ToInt32(CountReader["Сount_SZI"]);
                    int warehouseCount = Convert.ToInt32(CountReader["Сount"]);

                    if (requestedCount > warehouseCount)
                    {
                        hasEnoughQuantity = false;
                        string sziID = CountReader["SziID"].ToString();
                        insufficientSZIs.Add(sziID);
                    }
                }
                CountReader.Close();

                if (!hasEnoughQuantity)
                {
                    string errorMessage = "Недостаточное количество СЗИ: ";
                    errorMessage += string.Join(", ", insufficientSZIs);
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string selectedPaymentMethod = comboBox9.SelectedItem.ToString();

                DateTime selectedPaymentDate = dateTimePicker2.Value;

                decimal totalPrice = decimal.Parse(textBox1.Text);

                string PaymentQuery = "INSERT INTO Payment (RequestID, payment_methodr, Date_Payment, Amount) VALUES (@RequestID, @PaymentMethod, @PaymentDate, @Amount)";
                SqlCommand PaymentCommand = new SqlCommand(PaymentQuery, connection);
                PaymentCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                PaymentCommand.Parameters.AddWithValue("@PaymentMethod", selectedPaymentMethod);
                PaymentCommand.Parameters.AddWithValue("@PaymentDate", selectedPaymentDate);
                PaymentCommand.Parameters.AddWithValue("@Amount", totalPrice);
                PaymentCommand.ExecuteNonQuery();

                string QuantityQuery = "UPDATE Warehouse SET Сount = Сount - rs.Сount_SZI " +
                                             "FROM [Request and SZI] rs " +
                                             "WHERE rs.RequestID = @RequestID AND rs.SziID = Warehouse.SziID";
                SqlCommand QuantityCommand = new SqlCommand(QuantityQuery, connection);
                QuantityCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                QuantityCommand.ExecuteNonQuery();

                string RelatedDataQuery = "DELETE FROM [Request and SZI] WHERE RequestID = @RequestID";
                SqlCommand RelatedDataCommand = new SqlCommand(RelatedDataQuery, connection);
                RelatedDataCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                RelatedDataCommand.ExecuteNonQuery();

                MessageBox.Show("Оплата успешно зарегистрирована!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectedText = comboBox2.SelectedItem.ToString();

                string[] selectedData = selectedText.Split(new string[] { " - " }, StringSplitOptions.None);
                string fullName = selectedData[0];
                DateTime requestDate = DateTime.Parse(selectedData[1]);

                string requestIdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand requestIdCommand = new SqlCommand(requestIdQuery, connection);
                requestIdCommand.Parameters.AddWithValue("@FullName", fullName);
                requestIdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)requestIdCommand.ExecuteScalar();
                //стоимость выбранной заявки
                string query = "SELECT r.ID_Request AS 'ID заявки', CONCAT(b.Surname_Buyer, ' ', b.Name_Buyer, ' ', b.Patronymic_Buyer) AS 'ФИО покупателя', " +
                               "SUM(COALESCE(w.Price_Work, 0) + (w2.Wholesale_price * c.Ratio) * rs.Сount_SZI) AS 'Стоимость заказа' " +
                               "FROM Request r " +
                               "JOIN Buyer b ON r.BuyerID = b.ID_Buyer " +
                               "LEFT JOIN [Request and SZI] rs ON r.ID_Request = rs.RequestID " +
                               "LEFT JOIN Szi s ON rs.SziID = s.ID_Szi " +
                               "LEFT JOIN [Classifier and SZI] cs ON s.ID_Szi = cs.SziID " +
                               "LEFT JOIN Classifier c ON cs.ClassifierID = c.ID_Classifier " +
                               "LEFT JOIN [Work on the request] wr ON r.ID_Request = wr.RequestID " +
                               "LEFT JOIN Work w ON wr.WorkID = w.ID_Work " +
                               "LEFT JOIN Warehouse w2 ON s.ID_Szi = w2.SziID " +
                               "WHERE r.ID_Request = @RequestID " +
                               "GROUP BY r.ID_Request, b.Surname_Buyer, b.Name_Buyer, b.Patronymic_Buyer";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RequestID", selectedRequestID);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (!DBNull.Value.Equals(reader["Стоимость заказа"]))
                    {
                        decimal totalPrice = Convert.ToDecimal(reader["Стоимость заказа"]);
                        textBox1.Text = totalPrice.ToString();
                        button18.Enabled = true;
                    }
                    else
                    {
                        textBox1.Text = "товар отсутствует";
                        button18.Enabled = false;
                    }
                    reader.Close();
                }
            }
        }
        private void button20_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectedText = comboBox17.SelectedItem.ToString();

                string[] selectedData = selectedText.Split(new string[] { " - " }, StringSplitOptions.None);
                string fullName = selectedData[0];
                DateTime requestDate = DateTime.Parse(selectedData[1]);

                string IdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand IdCommand = new SqlCommand(IdQuery, connection);
                IdCommand.Parameters.AddWithValue("@FullName", fullName);
                IdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)IdCommand.ExecuteScalar();

                string CountQuery = "SELECT WS.SziID, WS.[Сount], ISNULL(SUM(CR.[Сount_Complex]), 0) AS TotalRequestedCount " +
                            "FROM [Warehouse] WS " +
                            "LEFT JOIN [Request and SZI] RS ON WS.SziID = RS.SziID " +
                            "LEFT JOIN [Complex and Request] CR ON RS.RequestID = CR.RequestID " +
                            "WHERE WS.SziID IN (SELECT SziID FROM [Complex solution and SZI] WHERE ComplexID IN (SELECT ComplexxID FROM [Complex and Request] WHERE RequestID = @RequestID)) " +
                            "GROUP BY WS.SziID, WS.[Сount]";
                SqlCommand CountCommand = new SqlCommand(CountQuery, connection);
                CountCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);

                SqlDataReader quantityReader = CountCommand.ExecuteReader();

                bool hasInsufficientQuantity = false;
                StringBuilder countMessage = new StringBuilder("Недостаточное количество на складе для следующих СЗИ:\n");

                while (quantityReader.Read())
                {
                    int sziID = (int)quantityReader["SziID"];
                    int countOnWarehouse = (int)quantityReader["Сount"];
                    int totalRequestedCount = (int)quantityReader["TotalRequestedCount"];

                    if (countOnWarehouse < totalRequestedCount)
                    {
                        hasInsufficientQuantity = true;

                        string sziTitleQuery = "SELECT Title_szi FROM Szi WHERE ID_Szi = @SziID";
                        SqlCommand sziTitleCommand = new SqlCommand(sziTitleQuery, connection);
                        sziTitleCommand.Parameters.AddWithValue("@SziID", sziID);
                        string sziTitle = sziTitleCommand.ExecuteScalar().ToString();

                        countMessage.AppendLine($"{sziTitle}: Запрошено - {totalRequestedCount}, На складе - {countOnWarehouse}");
                    }
                }
                quantityReader.Close();
                if (hasInsufficientQuantity)
                {
                    MessageBox.Show(countMessage.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string paymentQuery = "INSERT INTO Payment (RequestID, payment_methodr, Date_Payment, Amount) " +
                                      "VALUES (@RequestID, @PaymentMethod, @DatePayment, @Amount)";
                SqlCommand paymentCommand = new SqlCommand(paymentQuery, connection);
                paymentCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                paymentCommand.Parameters.AddWithValue("@PaymentMethod", comboBox3.SelectedItem.ToString());
                paymentCommand.Parameters.AddWithValue("@DatePayment", dateTimePicker3.Value);
                paymentCommand.Parameters.AddWithValue("@Amount", decimal.Parse(textBox2.Text));
                paymentCommand.ExecuteNonQuery();

                string WarehouseQuery = "UPDATE [Warehouse] " +
                              "SET [Сount] = [Сount] - (SELECT SUM([Сount_Complex]) FROM [Complex and Request] WHERE RequestID = @RequestID) " +
                              "WHERE SziID IN (SELECT SziID FROM [Complex solution and SZI] WHERE ComplexID IN (SELECT ComplexxID FROM [Complex and Request] WHERE RequestID = @RequestID))";
                SqlCommand WarehouseCommand = new SqlCommand(WarehouseQuery, connection);
                WarehouseCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                WarehouseCommand.ExecuteNonQuery();

                string RelatedDataQuery = "DELETE FROM [Complex and Request] WHERE RequestID = @RequestID";
                SqlCommand RelatedDataCommand = new SqlCommand(RelatedDataQuery, connection);
                RelatedDataCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                RelatedDataCommand.ExecuteNonQuery();

                comboBox17.Items.Clear();   
                FillRequestDatesComplexComboBox();
                MessageBox.Show("Оплата успешно зарегистрирована!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void comboBox17_SelectedValueChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectedText = comboBox17.SelectedItem.ToString();

                string[] selectedData = selectedText.Split(new string[] { " - " }, StringSplitOptions.None);
                string fullName = selectedData[0];
                DateTime requestDate = DateTime.Parse(selectedData[1]);

                string IdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand IdCommand = new SqlCommand(IdQuery, connection);
                IdCommand.Parameters.AddWithValue("@FullName", fullName);
                IdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)IdCommand.ExecuteScalar();

                string query = "SELECT R.ID_Request AS 'ID заказа', " +
                               "CONCAT(B.Surname_Buyer, ' ', B.Name_Buyer, ' ', B.Patronymic_Buyer) AS 'ФИО покупателя', " +
                               "SUM((COALESCE(W.Price_Work, 0) + WS.Wholesale_price * C.Ratio)) AS 'Стоимость заявки на комплексное решение' " +
                               "FROM Request R " +
                               "INNER JOIN Buyer B ON R.BuyerID = B.ID_Buyer " +
                               "INNER JOIN [Complex and Request] CR ON R.ID_Request = CR.RequestID " +
                               "INNER JOIN [Complex solution and SZI] CSZI ON CR.ComplexxID = CSZI.ComplexID " +
                               "INNER JOIN Szi SZ ON CSZI.SziID = SZ.ID_Szi " +
                               "INNER JOIN [Classifier and SZI] CS ON SZ.ID_Szi = CS.SziID " +
                               "INNER JOIN Classifier C ON CS.ClassifierID = C.ID_Classifier " +
                               "LEFT JOIN [Work on the request] WR ON R.ID_Request = WR.RequestID " +
                               "LEFT JOIN Work W ON WR.WorkID = W.ID_Work " +
                               "LEFT JOIN [Warehouse] WS ON SZ.ID_Szi = WS.SziID " +
                               "WHERE R.ID_Request NOT IN (SELECT RequestID FROM Payment) " +
                               "GROUP BY R.ID_Request, B.Surname_Buyer, B.Name_Buyer, B.Patronymic_Buyer " +
                               "HAVING SUM((COALESCE(W.Price_Work, 0) + WS.Wholesale_price * C.Ratio)) IS NOT NULL";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RequestID", selectedRequestID);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (!DBNull.Value.Equals(reader["Стоимость заявки на комплексное решение"]))
                    {
                        decimal totalPrice = Convert.ToDecimal(reader["Стоимость заявки на комплексное решение"]);

                        textBox2.Text = totalPrice.ToString();
                        button20.Enabled = true;
                    }
                    else
                    {
                        textBox2.Text = "товар отсутствует";
                        button20.Enabled = false;
                    }
                    reader.Close();
                }
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            string selectedEmployee = comboBox16.Text; 

            string[] employeeNames = selectedEmployee.Split(' ');
            string surname = employeeNames[0];
            string name = employeeNames[1];
            string patronymic = employeeNames[2];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string employeeIdQuery = "SELECT ID_Employee FROM Employee WHERE Surname_Employee = @surname AND Name_Employee = @name AND Patronymic_Employee = @patronymic";
                            SqlCommand employeeIdCommand = new SqlCommand(employeeIdQuery, connection, transaction);
                            employeeIdCommand.Parameters.AddWithValue("@surname", surname);
                            employeeIdCommand.Parameters.AddWithValue("@name", name);
                            employeeIdCommand.Parameters.AddWithValue("@patronymic", patronymic);

                            int employeeId = (int)employeeIdCommand.ExecuteScalar();

                            string WorkRequestQuery = "DELETE FROM [Work on the request] WHERE EmployeeID = @employeeId";
                            SqlCommand WorkRequestCommand = new SqlCommand(WorkRequestQuery, connection, transaction);
                            WorkRequestCommand.Parameters.AddWithValue("@employeeId", employeeId);
                            WorkRequestCommand.ExecuteNonQuery();

                            string DepartmentEmployeeQuery = "DELETE FROM [Department and employee] WHERE EmployeeID = @employeeId";
                            SqlCommand DepartmentEmployeeCommand = new SqlCommand(DepartmentEmployeeQuery, connection, transaction);
                            DepartmentEmployeeCommand.Parameters.AddWithValue("@employeeId", employeeId);
                            DepartmentEmployeeCommand.ExecuteNonQuery();

                            string EmployeeQuery = "DELETE FROM Employee WHERE ID_Employee = @employeeId";
                            SqlCommand EmployeeCommand = new SqlCommand(EmployeeQuery, connection, transaction);
                            EmployeeCommand.Parameters.AddWithValue("@employeeId", employeeId);
                            EmployeeCommand.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("Данные о сотруднике успешно удалены!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Ошибка при удалении данных сотрудника: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}