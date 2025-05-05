// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com


using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IDZ_szi
{
    public partial class Employee : Form
    {
        public Employee()
        {
            InitializeComponent();
        }
        private SqlConnection connection;
        private string connectionString = "Data Source=LAPTOP-JSVR0I7M;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";

        private void Employee_FormClosed(object sender, FormClosedEventArgs e)
        {
            Input input = new Input();
            input.Show();
            Hide();
        }
        private void Employee_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            LoadBuyers(); FillEmployeesComboBox(connection);
            FillBuyerComboBox();FillComboBoxWithSzi();
        }
        private void LoadBuyers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT DISTINCT B.ID_Buyer, CONCAT(B.Surname_Buyer, ' ', B.Name_Buyer, ' ', B.Patronymic_Buyer) AS FullName " +
                                   "FROM Buyer B " +
                                   "INNER JOIN Request R ON B.ID_Buyer = R.BuyerID " +
                                   "INNER JOIN Payment P ON R.ID_Request = P.RequestID";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    comboBox1.DisplayMember = "FullName";
                    comboBox1.ValueMember = "ID_Buyer";
                    comboBox1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке списка покупателей: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FillEmployeesComboBox(SqlConnection connection)
        {
            string query = "SELECT CONCAT(Surname_Employee, ' ', Name_Employee, ' ', Patronymic_Employee) FROM Employee";

            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string employeeName = reader.GetString(0);
                    comboBox2.Items.Add(employeeName);
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedBuyerId = Convert.ToInt32(comboBox1.SelectedValue);
            LoadSZI(selectedBuyerId); LoadComplexSolutions(selectedBuyerId);
            LoadWorkOnRequest(selectedBuyerId); CalculateTotalPrice(selectedBuyerId);
        }
        private void LoadSZI(int buyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT s.Title_szi AS 'название СЗИ', rs.[Сount_SZI] AS 'количество', " +
               "((w2.Wholesale_price * c.Ratio)* rs.[Сount_SZI]) AS 'стоимость' " +
               "FROM [Request and SZI] rs " +
               "JOIN Szi s ON rs.SziID = s.ID_Szi " +
               "LEFT JOIN Warehouse w2 ON s.ID_Szi = w2.SziID " +
               "LEFT JOIN [Classifier and SZI] cs ON s.ID_Szi = cs.SziID " +
               "LEFT JOIN Classifier c ON cs.ClassifierID = c.ID_Classifier " +
               "LEFT JOIN [Work on the request] wr ON rs.RequestID = wr.RequestID " +
               "LEFT JOIN Work w ON wr.WorkID = w.ID_Work " +
               "WHERE rs.RequestID IN (SELECT ID_Request FROM Request WHERE BuyerID = @BuyerID)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BuyerID", buyerId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказанных СЗИ: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FillBuyerComboBox()
        {
            string query = "SELECT CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) FROM Buyer";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string fullName = reader.GetString(0);
                        comboBox3.Items.Add(fullName);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при заполнении комбобокса: " + ex.Message);
            }
        }
        private void LoadComplexSolutions(int buyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT cs.Title_Complex AS 'название комплексного решения', cr.[Сount_Complex] AS 'количество', " +
                        "SUM(ws.Wholesale_price * c.Ratio) * cr.[Сount_Complex] AS 'стоимость' " +
                        "FROM [Complex solution and SZI] cszi " +
                        "JOIN [Complex solution] cs ON cszi.ComplexID = cs.ID_Complex " +
                        "JOIN [Complex and Request] cr ON cszi.ComplexID = cr.ComplexxID " +
                        "LEFT JOIN [Work on the request] wr ON cr.RequestID = wr.RequestID " +
                        "LEFT JOIN Work w ON wr.WorkID = w.ID_Work " +
                        "LEFT JOIN Szi s ON cszi.SziID = s.ID_Szi " +
                        "LEFT JOIN [Classifier and SZI] csi ON s.ID_Szi = csi.SziID " +
                        "LEFT JOIN Classifier c ON csi.ClassifierID = c.ID_Classifier " +
                        "LEFT JOIN Warehouse ws ON s.ID_Szi = ws.SziID " +
                        "WHERE cr.RequestID IN (SELECT ID_Request FROM Request WHERE BuyerID = @BuyerID) " +
                        "GROUP BY cs.Title_Complex, cr.[Сount_Complex]";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BuyerID", buyerId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataGridView2.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказанных комплексных решений: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadWorkOnRequest(int buyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT W.Title_Work, W.Price_Work " +
                                   "FROM [Work on the request] WR " +
                                   "INNER JOIN Request R ON WR.RequestID = R.ID_Request " +
                                   "INNER JOIN Work W ON WR.WorkID = W.ID_Work " +
                                   "WHERE R.BuyerID = @BuyerID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BuyerID", buyerId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataTable.Columns[0].ColumnName = "название";
                    dataTable.Columns[1].ColumnName = "стоимость";

                    dataGridView3.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке работы по заявке: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CalculateTotalPrice(int buyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT P.Amount AS TotalAmount " +
                                   "FROM Request R " +
                                   "INNER JOIN Payment P ON R.ID_Request = P.RequestID " +
                                   "WHERE R.BuyerID = @BuyerID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BuyerID", buyerId);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        label2.Text = "Общая стоимость заявки: " + Convert.ToDecimal(result).ToString("C");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при расчете общей стоимости заявки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query1 = @"SELECT S.Title_szi AS 'название СЗИ',
                                    SUM(RS.Сount_SZI) AS 'проданное количество',
                                    (w.Wholesale_price * c.Ratio) AS 'цена'
                                    FROM Payment P
                                    INNER JOIN Request R ON P.RequestID = R.ID_Request
                                    INNER JOIN [Request and SZI] RS ON R.ID_Request = RS.RequestID
                                    INNER JOIN [Classifier and SZI] CS ON RS.SziID = CS.SziID
                                    INNER JOIN Classifier C ON CS.ClassifierID = C.ID_Classifier
                                    LEFT JOIN [Complex and Request] CR ON R.ID_Request = CR.RequestID
                                    LEFT JOIN [Complex solution and SZI] CSS ON CR.ComplexxID = CSS.ComplexID
                                    LEFT JOIN Szi S ON RS.SziID = S.ID_Szi OR CSS.SziID = S.ID_Szi
                                    LEFT JOIN Warehouse w ON S.ID_Szi = w.SziID
                                    GROUP BY S.Title_szi, w.Wholesale_price, c.Ratio";

                    SqlCommand command1 = new SqlCommand(query1, connection);
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    System.Data.DataTable dataTable1 = new System.Data.DataTable();
                    adapter1.Fill(dataTable1);

                    dataTable1.Columns[0].ColumnName = "название";
                    dataTable1.Columns[1].ColumnName = "проданное количество";
                    dataTable1.Columns[2].ColumnName = "цена";

                    dataGridView4.DataSource = dataTable1;
                    string query2 = @"SELECT C.Title_Complex AS 'название комплексного решения',
                                    SUM(CR.[Сount_Complex]) AS 'проданное количество',
                                    SUM((w.Wholesale_price * Cl.Ratio)) AS 'цена'
                                    FROM Payment P
                                    INNER JOIN Request R ON P.RequestID = R.ID_Request
                                    INNER JOIN [Complex and Request] CR ON R.ID_Request = CR.RequestID
                                    INNER JOIN [Complex solution] C ON CR.ComplexxID = C.ID_Complex
                                    INNER JOIN [Complex solution and SZI] CSZI ON C.ID_Complex = CSZI.ComplexID
                                    INNER JOIN [Classifier and SZI] CS ON CSZI.SziID = CS.SziID
                                    INNER JOIN Classifier Cl ON CS.ClassifierID = Cl.ID_Classifier
                                    INNER JOIN Szi SZ ON CS.SziID = SZ.ID_Szi
                                    LEFT JOIN Warehouse w ON SZ.ID_Szi = w.SziID
                                    LEFT JOIN [Request and SZI] RS ON R.ID_Request = RS.RequestID AND CS.SziID = RS.SziID
                                    GROUP BY
                                    C.Title_Complex";

                    SqlCommand command2 = new SqlCommand(query2, connection);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(command2);
                    System.Data.DataTable dataTable2 = new System.Data.DataTable();
                    adapter2.Fill(dataTable2);

                    dataTable1.Columns[0].ColumnName = "название";
                    dataTable1.Columns[1].ColumnName = "проданное количество";
                    dataTable1.Columns[2].ColumnName = "цена";

                    dataGridView5.DataSource = dataTable2;

                    string query3 = "SELECT SUM(Amount) FROM Payment";
                    SqlCommand command3 = new SqlCommand(query3, connection);
                    double totalAmount = Convert.ToDouble(command3.ExecuteScalar());
                    label3.Text = "Общая сумма: " + totalAmount.ToString("C2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении запросов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string selectedEmployee;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query1 = "SELECT Szi.Title_szi " +
                                    "FROM Szi " +
                                    "INNER JOIN [Classifier and SZI] ON Szi.ID_Szi = [Classifier and SZI].SziID " +
                                    "INNER JOIN Classifier ON [Classifier and SZI].ClassifierID = Classifier.ID_Classifier " +
                                    "WHERE Classifier.Ratio > 1.2";

                    SqlCommand command1 = new SqlCommand(query1, connection);
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    System.Data.DataTable dataTable1 = new System.Data.DataTable();
                    adapter1.Fill(dataTable1);

                    dataTable1.Columns[0].ColumnName = "название";

                    dataGridView6.DataSource = dataTable1;

                    selectedEmployee = comboBox2.SelectedItem.ToString(); 
                    string[] nameParts = selectedEmployee.Split(' ');
                    string firstName = nameParts[0];
                    string lastName = nameParts[1];
                    string patronymic = nameParts[2];

                    string query2 = "SELECT [Complex solution].[Title_Complex] " +
                        "FROM [Complex solution] " +
                        "INNER JOIN [Complex solution and SZI] ON [Complex solution].ID_Complex = [Complex solution and SZI].ComplexID " +
                        "INNER JOIN Szi ON [Complex solution and SZI].SziID = Szi.ID_Szi " +
                        "INNER JOIN [Work on the request] ON [Complex solution and SZI].ComplexID = [Work on the request].WorkID " +
                        "INNER JOIN Employee ON [Work on the request].EmployeeID = Employee.ID_Employee " +
                        "WHERE Szi.ID_Szi IN (SELECT SziID FROM [Complex solution and SZI] GROUP BY SziID HAVING COUNT(*) > 3) " +
                        "AND Employee.Name_Employee = @LastName AND Employee.Surname_Employee = @FirstName AND Employee.Patronymic_Employee = @Patronymic";

                    SqlCommand command = new SqlCommand(query2, connection);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataTable.Columns[0].ColumnName = "название";

                    dataGridView7.DataSource = dataTable;

                    DateTime selectedDate = dateTimePicker1.Value; 

                    string query3 = "SELECT Request.Date_Request, " +
                                    "Buyer.Surname_Buyer + ' ' + Buyer.Name_Buyer + ' ' + Buyer.Patronymic_Buyer AS Full_Name " +
                                    "FROM Request " +
                                    "INNER JOIN Payment ON Request.ID_Request = Payment.RequestID " +
                                    "INNER JOIN Buyer ON Request.BuyerID = Buyer.ID_Buyer " +
                                    "WHERE Payment.Date_Payment > @SelectedDate";


                    SqlCommand command3 = new SqlCommand(query3, connection);
                    command3.Parameters.AddWithValue("@SelectedDate", selectedDate);

                    SqlDataAdapter adapter3 = new SqlDataAdapter(command3);
                    System.Data.DataTable dataTable3 = new System.Data.DataTable();
                    adapter3.Fill(dataTable3);

                    dataTable3.Columns[0].ColumnName = "дата заявки";
                    dataTable3.Columns[1].ColumnName = "ФИО покупателя";

                    dataGridView8.DataSource = dataTable3;

                    comboBox2.Items.Clear();
                    FillEmployeesComboBox(connection);
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

                    //Получение суммарного количества сзи по поставщикам
                    string query1 = "SELECT Provider.Totle_Provider, SUM(Warehouse.[Сount]) AS TotalCount " +
                                    "FROM Provider " +
                                    "INNER JOIN Warehouse ON Provider.ID_Provider = Warehouse.ProviderID " +
                                    "GROUP BY Provider.Totle_Provider";

                    SqlCommand command1 = new SqlCommand(query1, connection);
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    System.Data.DataTable dataTable1 = new System.Data.DataTable();
                    adapter1.Fill(dataTable1);

                    dataTable1.Columns[0].ColumnName = "поставщик";
                    dataTable1.Columns[1].ColumnName = "количество";

                    dataGridView9.DataSource = dataTable1;

                    //Получение количества сотрудников в каждом отделе
                    string query2 = "SELECT Department.Title_Department, COUNT(Employee.ID_Employee) AS EmployeeCount " +
                                    "FROM [Department and employee] " +
                                    "JOIN Department ON [Department and employee].DepartmentID = Department.ID_Department " +
                                    "JOIN Employee ON [Department and employee].EmployeeID = Employee.ID_Employee " +
                                    "GROUP BY Department.Title_Department";

                    
                    SqlCommand command2 = new SqlCommand(query2, connection);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(command2);
                    System.Data.DataTable dataTable2 = new System.Data.DataTable();
                    adapter2.Fill(dataTable2);

                    dataTable2.Columns[0].ColumnName = "отдел";
                    dataTable2.Columns[1].ColumnName = "количество";

                    dataGridView10.DataSource = dataTable2;

                    // Поиск покупателей с максимальной суммой оплаты
                    string query3 = "SELECT TOP 1 Buyer.Surname_Buyer, Buyer.Name_Buyer, MAX(Payment.Amount) AS MaxPayment " +
                                    "FROM Buyer " +
                                    "JOIN Request ON Buyer.ID_Buyer = Request.BuyerID " +
                                    "JOIN Payment ON Request.ID_Request = Payment.RequestID " +
                                    "GROUP BY Buyer.Surname_Buyer, Buyer.Name_Buyer " +
                                    "ORDER BY MaxPayment DESC ";

                    SqlCommand command3 = new SqlCommand(query3, connection);
                    SqlDataAdapter adapter3 = new SqlDataAdapter(command3);
                    System.Data.DataTable dataTable3 = new System.Data.DataTable();
                    adapter3.Fill(dataTable3);

                    dataTable3.Columns[0].ColumnName = "фамилия";
                    dataTable3.Columns[1].ColumnName = "имя";
                    dataTable3.Columns[2].ColumnName = "максимальная цена";

                    dataGridView11.DataSource = dataTable3;

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFullName = comboBox3.SelectedItem.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", selectedFullName);

                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    dataTable.Columns[0].ColumnName = "id";
                    dataTable.Columns[1].ColumnName = "фамилия";
                    dataTable.Columns[2].ColumnName = "отчество";
                    dataTable.Columns[3].ColumnName = "email";
                    dataTable.Columns[4].ColumnName = "серия паспорта";
                    dataTable.Columns[5].ColumnName = "номер паспорта";
                    dataTable.Columns[6].ColumnName = "день рождения";

                    dataGridView12.DataSource = dataTable;
                }
                connection.Close();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand( "SELECT e.Surname_Employee, s.Title_szi, SUM(cr.Сount_Complex) AS TotalCount " +
                                "FROM [Complex solution and SZI] csz " +
                                "INNER JOIN [Complex solution] c ON csz.ComplexID = c.ID_Complex " +
                                "INNER JOIN Employee e ON c.EmployeeID = e.ID_Employee " +
                                "INNER JOIN Szi s ON csz.SziID = s.ID_Szi " +
                                "INNER JOIN [Complex and Request] cr ON c.ID_Complex = cr.ComplexxID " +
                                "GROUP BY e.Surname_Employee, s.Title_szi;", connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            adapter.Fill(dataTable);

            dataGridView13.Columns.Clear();

            dataGridView13.Columns.Add("SZI", "СЗИ");

            var distinctSurnames = dataTable.AsEnumerable()
                .Select(row => row.Field<string>("Surname_Employee"))
                .Distinct()
                .ToList();

            foreach (string surname in distinctSurnames)
            {
                dataGridView13.Columns.Add(surname, surname);
            }

            var distinctSziTitles = dataTable.AsEnumerable()
                .Select(row => row.Field<string>("Title_szi"))
                .Distinct()
                .ToList();

            foreach (string sziTitle in distinctSziTitles)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.CreateCells(dataGridView13);
                dataGridViewRow.Cells[0].Value = sziTitle;

                foreach (string surname in distinctSurnames)
                {
                    DataRow[] matchingRows = dataTable.Select($"Surname_Employee = '{surname}' AND Title_szi = '{sziTitle}'");
                    int totalCount = matchingRows.Length > 0 ? Convert.ToInt32(matchingRows[0]["TotalCount"]) : 0;
                    int columnIndex = dataGridView13.Columns[surname].Index;
                    dataGridViewRow.Cells[columnIndex].Value = totalCount;
                }

                dataGridView13.Rows.Add(dataGridViewRow);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT C.Title_Classifier, AVG(W.Wholesale_price) AS AveragePrice " +
                               "FROM Classifier C " +
                               "INNER JOIN [Classifier and SZI] CS ON C.ID_Classifier = CS.ClassifierID " +
                               "INNER JOIN Szi S ON CS.SziID = S.ID_Szi " +
                               "INNER JOIN Warehouse W ON S.ID_Szi = W.SziID " +
                               "GROUP BY C.Title_Classifier";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns[0].ColumnName = "название";
                dataTable.Columns[1].ColumnName = "средняя цена";

                dataGridView14.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT S.Title_szi, IIf(W.Сount > 5, 'Много', 'Мало') AS CountStatus " +
               "FROM Szi AS S " +
               "INNER JOIN Warehouse AS W ON S.ID_Szi = W.SziID";
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);

                connection.Close();

                dataTable.Columns[0].ColumnName = "название";
                dataTable.Columns[1].ColumnName = "количество";

                dataGridView15.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            string selectedSzi = comboBox4.SelectedItem.ToString();
            string query = @"SELECT Totle_Provider, Address_Provider
                     FROM Provider
                     WHERE ID_Provider IN (SELECT ProviderID FROM Warehouse
                     WHERE SziID = ( SELECT ID_Szi FROM Szi  WHERE Title_szi = @TitleSzi ))";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TitleSzi", selectedSzi);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            adapter.Fill(dataTable);

            dataTable.Columns[0].ColumnName = "поставщик";
            dataTable.Columns[1].ColumnName = "адрес";

            dataGridView16.DataSource = dataTable;
        }
        private void FillComboBoxWithSzi()
        {
            string query = "SELECT DISTINCT S.Title_szi FROM Szi AS S INNER JOIN Warehouse AS W ON S.ID_Szi = W.SziID";

            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            adapter.Fill(dataTable);

            comboBox4.DataSource = dataTable;
            comboBox4.DisplayMember = "Title_szi";
            comboBox4.ValueMember = "Title_szi";
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSzi = comboBox4.SelectedValue.ToString(); 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT Totle_Provider, Address_Provider
                         FROM Provider
                         WHERE ID_Provider IN (SELECT ProviderID FROM Warehouse
                         WHERE SziID = (SELECT ID_Szi FROM Szi WHERE Title_szi = @TitleSzi))";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TitleSzi", selectedSzi);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataGridView16.DataSource = dataTable;
                }
                connection.Close();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView17.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;

            string query = @"SELECT s.Title_szi, SUM(cr.Сount_Complex) AS TotalCount
                FROM [Complex solution and SZI] csz
                INNER JOIN Szi s ON csz.SziID = s.ID_Szi
                INNER JOIN [Complex and Request] cr ON csz.ComplexID = cr.ComplexxID
                GROUP BY s.Title_szi";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns[0].ColumnName = "название";
                dataTable.Columns[1].ColumnName = "количество";

                dataGridView17.DataSource = dataTable;
            }
            chart1.Series.Clear();

            chart1.Series.Add("Проданные СЗИ");
            chart1.Series["Проданные СЗИ"].ChartType = SeriesChartType.Column;

            foreach (DataGridViewRow row in dataGridView17.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    string sziTitle = row.Cells[0].Value.ToString();
                    int totalCount = Convert.ToInt32(row.Cells[1].Value);
                    chart1.Series["Проданные СЗИ"].Points.AddXY(sziTitle, totalCount);
                    chart1.Series["Проданные СЗИ"].Points.Last().Label = totalCount.ToString();
                }
            }
            chart1.Series["Проданные СЗИ"].IsValueShownAsLabel = true;
            chart1.ChartAreas[0].AxisX.Interval = 1;

            chart1.ChartAreas[0].AxisX.Title = "СЗИ";
            chart1.ChartAreas[0].AxisY.Title = "Количество";

            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 8);

            chart1.ChartAreas[0].AxisX.Interval = 1;

            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // Создание нового документа Word
                string fileName = "Заявка.docx";
                object missing = System.Reflection.Missing.Value;

                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Document doc = wordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                try
                {
                    // Создание таблицы с данными
                    CreateTable(doc, comboBox1.Text, dataGridView1, dataGridView2, dataGridView3);

                    // Добавление надписи об общей стоимости
                    Paragraph totalPriceParagraph = doc.Content.Paragraphs.Add();
                    totalPriceParagraph.Range.Text = "Общая стоимость заявки: " + label2.Text;

                    // Сохранение документа в файл
                    doc.SaveAs(Path.GetFullPath(fileName));

                    MessageBox.Show("Данные сохранены в файл Word.", "Сохранение завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    doc.Close();
                    wordApp.Quit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении в файл Word: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateTable(Document doc, string buyerName, params DataGridView[] dataGridViews)
        {
            object missing = System.Reflection.Missing.Value;

            Paragraph buyerParagraph = doc.Content.Paragraphs.Add();
            buyerParagraph.Range.Text = "Покупатель: " + buyerName;

            Table table = doc.Content.Tables.Add(buyerParagraph.Range, dataGridViews.Sum(d => d.RowCount) + 2, dataGridViews[0].ColumnCount, ref missing, ref missing);

            for (int i = 0; i < dataGridViews[0].ColumnCount; i++)
            {
                table.Cell(1, i + 1).Range.Text = dataGridViews[0].Columns[i].HeaderText;
            }

            int buyerRowIndex = 2; // Индекс строк, где будет добавлена информация о покупателе

            // Добавление данных из дгв в таблицу
            foreach (DataGridView dataGridView in dataGridViews)
            {
                for (int row = 0; row < dataGridView.RowCount; row++)
                {
                    for (int col = 0; col < dataGridView.ColumnCount; col++)
                    {
                        if (dataGridView.Rows[row].Cells[col].Value != null)
                        {
                            table.Cell(buyerRowIndex, col + 1).Range.Text = dataGridView.Rows[row].Cells[col].Value.ToString();
                        }
                        else
                        {
                            table.Cell(buyerRowIndex, col + 1).Range.Text = string.Empty;
                        }
                    }
                    buyerRowIndex++;
                }
            }
            table.Cell(buyerRowIndex, 1).Range.Text = $"Заказчик: {buyerName}";
            table.Cell(buyerRowIndex, 1).Merge(table.Cell(buyerRowIndex, dataGridViews[0].ColumnCount));
        }

        private void gradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

