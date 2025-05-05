// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System;
using System.Collections.Generic;

namespace IDZ_szi
{
    public partial class SZI : Form
    {

        private SqlConnection connection;
        private string connectionString = "Data Source=LAPTOP-JSVR0I7M;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";


        public SZI()
        {
            InitializeComponent();

            connection = new SqlConnection(connectionString);
            connection.Open();
            
        }

        private void FillEmployeesComboBox()
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
                    comboBox1.Items.Add(title);
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
                    comboBox15.Items.Add(title);
                }
            }
        }
        private void FillRequestDatesComboBox()
        {
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            HashSet<string> uniqueBuyers = new HashSet<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение списка заявок, которые больше 
                    string query = "SELECT Request.ID_Request, Request.Date_Request, Buyer.Surname_Buyer, Buyer.Name_Buyer, Buyer.Patronymic_Buyer FROM Request LEFT JOIN Payment ON Request.ID_Request = Payment.RequestID INNER JOIN Buyer ON Request.BuyerID = Buyer.ID_Buyer WHERE Request.Date_Request > '2023-05-24' AND Payment.ID_Payment IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int requestID = reader.GetInt32(0);
                        DateTime requestDate = reader.GetDateTime(1);
                        string buyerSurname = reader.GetString(2);
                        string buyerName = reader.GetString(3);
                        string buyerPatronymic = reader.GetString(4);

                        // Формирование ФИО и даты заказа
                        string fullName = $"{buyerSurname} {buyerName} {buyerPatronymic}";
                        string displayText = $"{fullName} - {requestDate}";

                        // Добавление в комбобоксы только уникальных значений покупателя
                        if (!uniqueBuyers.Contains(fullName))
                        {
                            comboBox3.Items.Add(displayText);
                            comboBox4.Items.Add(displayText);
                            uniqueBuyers.Add(fullName);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private int requestId;
        private void button10_Click(object sender, System.EventArgs e)
        {
            string surname = textBox19.Text;
            string name = textBox18.Text;
            string patronymic = textBox17.Text;
            DateTime requestDate = dateTimePicker1.Value;
            string email = textBox14.Text;
            string passportSeries = textBox1.Text;
            string passportNumber = textBox2.Text;
            DateTime birthday = dateTimePicker2.Value;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверка наличия покупателя в таблице "Покупатель"
                    string checkBuyerQuery = "SELECT COUNT(*) FROM Buyer WHERE Surname_Buyer = @Surname AND Name_Buyer = @Name AND Patronymic_Buyer = @Patronymic";
                    SqlCommand checkBuyerCommand = new SqlCommand(checkBuyerQuery, connection);
                    checkBuyerCommand.Parameters.AddWithValue("@Surname", surname);
                    checkBuyerCommand.Parameters.AddWithValue("@Name", name);
                    checkBuyerCommand.Parameters.AddWithValue("@Patronymic", patronymic);
                    int existingBuyerCount = Convert.ToInt32(checkBuyerCommand.ExecuteScalar());

                    if (existingBuyerCount > 0)
                    {
                        MessageBox.Show("Такой покупатель уже существует.");
                        return;
                    }

                    // Вставка данных в таблицу "Покупатель"
                    string insertBuyerQuery = "INSERT INTO Buyer (Surname_Buyer, Name_Buyer, Patronymic_Buyer, " +
                        "Email_Buyer, Passport_series, Passport_number, Birthday) " +
                        "VALUES (@Surname, @Name, @Patronymic, @Email, @PassportSeries, @PassportNumber, @Birthday); " +
                        "SELECT SCOPE_IDENTITY();";

                    SqlCommand insertBuyerCommand = new SqlCommand(insertBuyerQuery, connection);
                    insertBuyerCommand.Parameters.AddWithValue("@Surname", surname);
                    insertBuyerCommand.Parameters.AddWithValue("@Name", name);
                    insertBuyerCommand.Parameters.AddWithValue("@Patronymic", patronymic);
                    insertBuyerCommand.Parameters.AddWithValue("@Email", email);
                    insertBuyerCommand.Parameters.AddWithValue("@PassportSeries", passportSeries);
                    insertBuyerCommand.Parameters.AddWithValue("@PassportNumber", passportNumber);
                    insertBuyerCommand.Parameters.AddWithValue("@Birthday", birthday);

                    int buyerId = Convert.ToInt32(insertBuyerCommand.ExecuteScalar());

                    // Вставка данных в таблицу "Заявка"
                    string insertRequestQuery = "INSERT INTO Request (Date_Request, BuyerID) " +
                        "VALUES (@RequestDate, @BuyerId);";

                    SqlCommand insertRequestCommand = new SqlCommand(insertRequestQuery, connection);
                    insertRequestCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                    insertRequestCommand.Parameters.AddWithValue("@BuyerId", buyerId);

                    insertRequestCommand.ExecuteNonQuery();


                    comboBox3.Items.Clear();
                    comboBox4.Items.Clear();
                    FillRequestDatesComboBox();

                    MessageBox.Show("Данные успешно добавлены в базу данных.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение выбранных значений из комбобоксов и текстового поля
                    string selectedComboBox4Value = comboBox4.SelectedItem.ToString();
                    string selectedComboBox1Value = comboBox1.SelectedItem.ToString();
                    string selectedComboBox2Value = comboBox2.SelectedItem.ToString();

                    // Разделение значения комбобокса comboBox4 на ФИО и дату заказа
                    string[] selectedComboBox4Parts = selectedComboBox4Value.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    string fullName = selectedComboBox4Parts[0];
                    DateTime requestDate = DateTime.Parse(selectedComboBox4Parts[1]);

                    // Получение ID работы
                    string selectedWorkIDQuery = "SELECT ID_Work FROM Work WHERE Title_Work = @Title";
                    SqlCommand selectedWorkIDCommand = new SqlCommand(selectedWorkIDQuery, connection);
                    selectedWorkIDCommand.Parameters.AddWithValue("@Title", selectedComboBox1Value);
                    int selectedWorkID = Convert.ToInt32(selectedWorkIDCommand.ExecuteScalar());

                    string selectedEmployeeIDQuery = "SELECT ID_Employee FROM Employee WHERE CONCAT(Surname_Employee, ' ', Name_Employee, ' ', Patronymic_Employee) = @FullName";
                    SqlCommand selectedEmployeeIDCommand = new SqlCommand(selectedEmployeeIDQuery, connection);
                    selectedEmployeeIDCommand.Parameters.AddWithValue("@FullName", selectedComboBox2Value);
                    int selectedEmployeeID = Convert.ToInt32(selectedEmployeeIDCommand.ExecuteScalar());

                    // Получение ID заявки
                    string selectedRequestIDQuery = "SELECT TOP 1 ID_Request FROM Request WHERE Date_Request = @DateRequest AND BuyerID = (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName)";
                    SqlCommand selectedRequestIDCommand = new SqlCommand(selectedRequestIDQuery, connection);
                    selectedRequestIDCommand.Parameters.AddWithValue("@DateRequest", requestDate);
                    selectedRequestIDCommand.Parameters.AddWithValue("@FullName", fullName);
                    int selectedRequestID = Convert.ToInt32(selectedRequestIDCommand.ExecuteScalar());

                    // Вставка данных в таблицу "Work on the request"
                    string insertWorkOnRequestQuery = "INSERT INTO [Work on the request] (RequestID, WorkID, EmployeeID) " +
                        "VALUES (@RequestID, @WorkID, @EmployeeID)";
                    SqlCommand insertWorkOnRequestCommand = new SqlCommand(insertWorkOnRequestQuery, connection);
                    insertWorkOnRequestCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                    insertWorkOnRequestCommand.Parameters.AddWithValue("@WorkID", selectedWorkID);
                    insertWorkOnRequestCommand.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID);
                    insertWorkOnRequestCommand.ExecuteNonQuery();

                    MessageBox.Show("Работа по заявке успешно добавлена.");
                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение выбранных значений из комбобоксов и текстового поля
                    string selectedComboBox3Value = comboBox3.SelectedItem.ToString();
                    string selectedComboBox15Value = comboBox15.SelectedItem.ToString();
                    int selectedCount = int.Parse(textBox3.Text);

                    // Разделение значения комбобокса comboBox3 на ФИО и дату заказа
                    string[] selectedComboBox3Parts = selectedComboBox3Value.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    string fullName = selectedComboBox3Parts[0];
                    DateTime requestDate = DateTime.Parse(selectedComboBox3Parts[1]);

                    // Вставка данных в таблицу "Заявка" (Request)
                    string insertRequestQuery = "INSERT INTO Request (Date_Request, BuyerID) " +
                        "VALUES (@DateRequest, (SELECT TOP 1 ID_Buyer FROM Buyer WHERE Surname_Buyer = @Surname AND Name_Buyer = @Name and Patronymic_Buyer = @Patronymic))";
                    SqlCommand insertRequestCommand = new SqlCommand(insertRequestQuery, connection);
                    insertRequestCommand.Parameters.AddWithValue("@DateRequest", requestDate);
                    insertRequestCommand.Parameters.AddWithValue("@Surname", fullName.Split(' ')[0]);
                    insertRequestCommand.Parameters.AddWithValue("@Name", fullName.Split(' ')[1]);
                    insertRequestCommand.Parameters.AddWithValue("@Patronymic", fullName.Split(' ')[2]);
                    insertRequestCommand.ExecuteNonQuery();

                    // Получение ID только что созданной заявки
                    string selectLastRequestIDQuery = "SELECT IDENT_CURRENT('Request')";
                    SqlCommand selectLastRequestIDCommand = new SqlCommand(selectLastRequestIDQuery, connection);
                    int lastRequestID = Convert.ToInt32(selectLastRequestIDCommand.ExecuteScalar());

                    // Вставка данных в таблицу "[Request and SZI]"
                    string insertRequestSZIQuery = "INSERT INTO [Request and SZI] (RequestID, SziID, Сount_SZI) " +
                        "VALUES (@RequestID, (SELECT ID_Szi FROM Szi WHERE Title_szi = @SziTitle), @Count)";
                    SqlCommand insertRequestSZICommand = new SqlCommand(insertRequestSZIQuery, connection);
                    insertRequestSZICommand.Parameters.AddWithValue("@RequestID", lastRequestID);
                    insertRequestSZICommand.Parameters.AddWithValue("@SziTitle", selectedComboBox15Value);
                    insertRequestSZICommand.Parameters.AddWithValue("@Count", selectedCount);
                    insertRequestSZICommand.ExecuteNonQuery();


                    MessageBox.Show("Заявка успешно добавлена.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void SZI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
            Hide();
        }
        private void SZI_Load(object sender, EventArgs e)
        {
            FillWorkComboBox();
            FillSZIComboBox();
            FillEmployeesComboBox();
            FillRequestDatesComboBox();
        }
    }
}
