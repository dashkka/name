// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDZ_szi
{
    public partial class Moderator : Form
    {
        public Moderator()
        {
            InitializeComponent();
        }
        private SqlConnection connection;
        private string connectionString = "Data Source=LAPTOP-JSVR0I7M;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";

        private void Moderator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Input input = new Input();
            input.Show();
            Hide();
        }
        private void UpdateComplexSolutionsTable(SqlConnection connection)
        {
            try
            {
                string selectQuery = "SELECT cs.ID_Complex, cs.Title_Complex,  e.Surname_Employee, e.Name_Employee, e.Patronymic_Employee " +
                    "FROM [Complex solution] cs " +
                    "LEFT JOIN Employee e ON cs.EmployeeID = e.ID_Employee " +
                    "GROUP BY cs.ID_Complex, cs.Title_Complex, e.Name_Employee";
                SqlCommand command = new SqlCommand(selectQuery, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView5.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void FillComplexComboBox()
        {
            string query = "SELECT ID_Complex, Title_Complex FROM [Complex solution]";

            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string employeeName = reader.GetString(1);
                    comboBox12.Items.Add(employeeName);
                    comboBox13.Items.Add(employeeName);
                    comboBox11.Items.Add(employeeName);
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string complexTitle = textBox8.Text;
            string employeeName = comboBox5.SelectedItem.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Получение ID сотрудника по его имени
                    string getEmployeeIdQuery = "SELECT ID_Employee FROM Employee WHERE CONCAT(Surname_Employee, ' ', Name_Employee, ' ', Patronymic_Employee) = @EmployeeName";
                    using (SqlCommand getEmployeeIdCommand = new SqlCommand(getEmployeeIdQuery, connection))
                    {
                        // Передача параметра в запрос
                        getEmployeeIdCommand.Parameters.AddWithValue("@EmployeeName", employeeName);

                        connection.Open();
                        int employeeID = (int)getEmployeeIdCommand.ExecuteScalar();

                        // Вставка комплексного решения в таблицу [Complex solution]
                        string insertComplexQuery = "INSERT INTO [Complex solution] (Title_Complex, EmployeeID) VALUES (@ComplexTitle, @EmployeeID)";
                        using (SqlCommand insertComplexCommand = new SqlCommand(insertComplexQuery, connection))
                        {
                            // Передача параметров в запрос
                            insertComplexCommand.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                            insertComplexCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

                            insertComplexCommand.ExecuteNonQuery();
                        }

                        // Обновление данных в таблице datagridView5
                        UpdateComplexSolutionsTable(connection);
                    }

                    connection.Close(); // Закрытие подключения после использования
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string complexTitle = comboBox12.SelectedItem?.ToString();
            string sziTitle = comboBox6.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(complexTitle) || string.IsNullOrEmpty(sziTitle))
            {
                MessageBox.Show("Please select a complex solution and an SZI.");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Поиск ID комплексного решения на основе названия
                    string findComplexIDQuery = $"SELECT ID_Complex FROM [Complex solution] WHERE Title_Complex = @ComplexTitle";
                    using (SqlCommand findComplexIDCommand = new SqlCommand(findComplexIDQuery, connection))
                    {
                        findComplexIDCommand.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                        int complexID = (int)findComplexIDCommand.ExecuteScalar();

                        if (complexID == 0)
                        {
                            MessageBox.Show("Complex solution with the specified title does not exist.");
                            return; // Остановка выполнения операции
                        }

                        // Поиск ID СЗИ на основе названия
                        string findSziIDQuery = $"SELECT ID_Szi FROM Szi WHERE Title_szi = @SziTitle";
                        using (SqlCommand findSziIDCommand = new SqlCommand(findSziIDQuery, connection))
                        {
                            findSziIDCommand.Parameters.AddWithValue("@SziTitle", sziTitle);
                            int sziID = (int)findSziIDCommand.ExecuteScalar();

                            if (sziID == 0)
                            {
                                MessageBox.Show("SZI with the specified title does not exist.");
                                return; // Остановка выполнения операции
                            }

                            // Вставка данных в таблицу [Complex solution and SZI]
                            string insertComplexSziQuery = $"INSERT INTO [Complex solution and SZI] (ComplexID, SziID) VALUES (@ComplexID, @SziID)";
                            using (SqlCommand insertComplexSziCommand = new SqlCommand(insertComplexSziQuery, connection))
                            {
                                insertComplexSziCommand.Parameters.AddWithValue("@ComplexID", complexID);
                                insertComplexSziCommand.Parameters.AddWithValue("@SziID", sziID);

                                insertComplexSziCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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

                    // Поиск ID комплексного решения на основе выбранного названия
                    string getComplexIDQuery = "SELECT ID_Complex FROM [Complex solution] WHERE Title_Complex = @ComplexTitle";
                    using (SqlCommand getComplexIDCommand = new SqlCommand(getComplexIDQuery, connection))
                    {
                        getComplexIDCommand.Parameters.AddWithValue("@ComplexTitle", complexTitle);
                        int complexID = (int)getComplexIDCommand.ExecuteScalar();

                        if (complexID == 0)
                        {
                            MessageBox.Show("Complex solution with the selected title does not exist.");
                            return; // Остановка выполнения операции
                        }

                        // Удаление комплексного решения из таблицы [Complex solution]
                        string deleteComplexQuery = $"DELETE FROM [Complex solution] WHERE ID_Complex = @ComplexID";
                        using (SqlCommand deleteComplexCommand = new SqlCommand(deleteComplexQuery, connection))
                        {
                            deleteComplexCommand.Parameters.AddWithValue("@ComplexID", complexID);
                            deleteComplexCommand.ExecuteNonQuery();
                        }

                        // Удаление связей комплексного решения с СЗИ из таблицы [Complex solution and SZI]
                        string deleteComplexSziQuery = $"DELETE FROM [Complex solution and SZI] WHERE ComplexID = @ComplexID";
                        using (SqlCommand deleteComplexSziCommand = new SqlCommand(deleteComplexSziQuery, connection))
                        {
                            deleteComplexSziCommand.Parameters.AddWithValue("@ComplexID", complexID);
                            deleteComplexSziCommand.ExecuteNonQuery();
                        }

                        UpdateComplexSolutionsTable(connection);
                        comboBox11.Items.Clear();
                        comboBox12.Items.Clear();
                        comboBox13.Items.Clear();

                        FillComplexComboBox();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT cs.ID_Complex, cs.Title_Complex, e.Surname_Employee, e.Name_Employee, e.Patronymic_Employee " +
                        "FROM [Complex solution] cs " +
                        "LEFT JOIN Employee e ON cs.EmployeeID = e.ID_Employee";

                    SqlCommand command = new SqlCommand(selectQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView5.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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

                        string selectQuery = "SELECT s.ID_Szi, s.Title_szi, s.Description_szi " +
                            "FROM Szi s " +
                            "JOIN [Complex solution and SZI] cszi ON s.ID_Szi = cszi.SziID " +
                            "JOIN [Complex solution] cs ON cszi.ComplexID = cs.ID_Complex " +
                            "WHERE cs.Title_Complex = @ComplexTitle";

                        SqlCommand command = new SqlCommand(selectQuery, connection);
                        command.Parameters.AddWithValue("@ComplexTitle", selectedComplexTitle);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView4.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
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
        private void Moderator_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            FillWorkComboBox();
            FillComplexComboBox();
            FillSZIComboBox();
            PopulateProviderComboBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение выбранного классификатора из comboBox7
                    string selectedClassifier = comboBox7.SelectedItem.ToString();

                    // Вставка данных в таблицу "Szi"
                    string insertSziQuery = "INSERT INTO Szi (Title_szi, Description_szi) VALUES (@Title, @Description); SELECT SCOPE_IDENTITY();";
                    SqlCommand insertSziCommand = new SqlCommand(insertSziQuery, connection);
                    insertSziCommand.Parameters.AddWithValue("@Title", textBox9.Text);
                    insertSziCommand.Parameters.AddWithValue("@Description", textBox10.Text);
                    int sziId = Convert.ToInt32(insertSziCommand.ExecuteScalar());

                    // Получение ID классификатора
                    string selectedClassifierIdQuery = "SELECT ID_Classifier FROM Classifier WHERE Title_Classifier = @Title";
                    SqlCommand selectedClassifierIdCommand = new SqlCommand(selectedClassifierIdQuery, connection);
                    selectedClassifierIdCommand.Parameters.AddWithValue("@Title", selectedClassifier);
                    int classifierId = Convert.ToInt32(selectedClassifierIdCommand.ExecuteScalar());

                    // Вставка данных в таблицу "Classifier and SZI"
                    string insertClassifierSziQuery = "INSERT INTO [Classifier and SZI] (SziID, ClassifierID) VALUES (@SziID, @ClassifierID)";
                    SqlCommand insertClassifierSziCommand = new SqlCommand(insertClassifierSziQuery, connection);
                    insertClassifierSziCommand.Parameters.AddWithValue("@SziID", sziId);
                    insertClassifierSziCommand.Parameters.AddWithValue("@ClassifierID", classifierId);
                    insertClassifierSziCommand.ExecuteNonQuery();

                    MessageBox.Show("Записи успешно добавлены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                // Получение значений из формы
                int sziCount = Convert.ToInt32(textBox11.Text);
                decimal wholesalePrice = Convert.ToDecimal(textBox12.Text);
                string sziTitle = comboBox4.Text;
                DateTime deliveryDate = dateTimePicker1.Value;
                string providerName = comboBox8.Text;

                // Получение ID СЗИ по названию
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

                // Получение ID поставщика по названию
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

                // Добавление записи в таблицу Warehouse
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Warehouse (SziID, Delivery_date, Count, Wholesale_price, ProviderID) " +
                                   "VALUES (@SziID, @DeliveryDate, @Count, @WholesalePrice, @ProviderID)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                    command.Parameters.AddWithValue("@Count", sziCount);
                    command.Parameters.AddWithValue("@WholesalePrice", wholesalePrice);
                    command.Parameters.AddWithValue("@ProviderID", providerID);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Запись успешно добавлена в таблицу Warehouse.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                string sziTitle = comboBox1.Text;

                // Получение ID СЗИ по названию
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

                // Удаление связей с классификаторами
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [Classifier and SZI] WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }

                // Удаление связей с заявками
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [Request and SZI] WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }

                // Удаление записи из таблицы Warehouse
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Warehouse WHERE SziID = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }

                // Удаление записи из таблицы Szi
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Szi WHERE ID_Szi = @SziID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SziID", sziID);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("СЗИ и связанные данные успешно удалены из базы данных.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Очистка существующих данных в dataGridView2
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();

                // Добавление столбцов
                dataGridView2.Columns.Add("ColumnSzi", "СЗИ");
                dataGridView2.Columns.Add("ColumnDescription", "Описание");
                dataGridView2.Columns.Add("ColumnCount", "Количество");
                dataGridView2.Columns.Add("ColumnProvider", "Поставщик");

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
                MessageBox.Show("Ошибка: " + ex.Message);
            }
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox10.SelectedItem != null)
            {
                string selectedWork = comboBox10.SelectedItem.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение ID работы на основе выбранного названия
                    string workIDQuery = "SELECT ID_Work FROM Work WHERE Title_Work = @WorkTitle";
                    SqlCommand workIDCommand = new SqlCommand(workIDQuery, connection);
                    workIDCommand.Parameters.AddWithValue("@WorkTitle", selectedWork);
                    int workID = (int)workIDCommand.ExecuteScalar();

                    // Удаление связей работы с заявками
                    string deleteWorkReqQuery = "DELETE FROM [Work on the request] WHERE WorkID = @WorkID";
                    SqlCommand deleteWorkReqCommand = new SqlCommand(deleteWorkReqQuery, connection);
                    deleteWorkReqCommand.Parameters.AddWithValue("@WorkID", workID);
                    deleteWorkReqCommand.ExecuteNonQuery();

                    // Удаление связей работы с комплексными решениями
                    string deleteComplexReqQuery = "DELETE FROM [Complex and Request] WHERE ComplexxID IN (SELECT ID_ComplexSZI FROM [Complex solution and SZI] WHERE SziID = @WorkID)";
                    SqlCommand deleteComplexReqCommand = new SqlCommand(deleteComplexReqQuery, connection);
                    deleteComplexReqCommand.Parameters.AddWithValue("@WorkID", workID);
                    deleteComplexReqCommand.ExecuteNonQuery();

                    // Удаление работы
                    string deleteWorkQuery = "DELETE FROM Work WHERE ID_Work = @WorkID";
                    SqlCommand deleteWorkCommand = new SqlCommand(deleteWorkQuery, connection);
                    deleteWorkCommand.Parameters.AddWithValue("@WorkID", workID);
                    deleteWorkCommand.ExecuteNonQuery();
                }
                comboBox10.Items.Clear();
                FillWorkComboBox();

                MessageBox.Show("Работа успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите работу для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Title_Work, Price_Work, Period " +
                               "FROM Work";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView6.DataSource = dataTable;
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
