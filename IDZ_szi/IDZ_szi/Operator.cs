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
    public partial class Operator : Form
    {
        public Operator()
        {
            InitializeComponent();
        }
        private SqlConnection connection;
        private string connectionString = "Data Source=192.168.101.159, 1433;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";

        private void Operator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Input input = new Input();
            input.Show();
            Hide();
        }
        private void FillRequestDatesComboBox()
        {
            comboBox2.Items.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение списка заявок, связанных с СЗИ, которые больше 24 мая и нет в таблице "Оплата"
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
                MessageBox.Show("Ошибка: " + ex.Message);
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
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

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
                        "WHERE r.ID_Request NOT IN (SELECT RequestID FROM Payment) AND r.Date_Request > '2023-05-24' " +
                        "AND s.ID_Szi IS NOT NULL " +
                        "GROUP BY r.ID_Request, b.Surname_Buyer, b.Name_Buyer, b.Patronymic_Buyer";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataGridView3.DataSource = dataTable;

                    dataGridView3.Columns["ID заявки"].HeaderText = "ID заявки";
                    dataGridView3.Columns["ФИО покупателя"].HeaderText = "ФИО покупателя";
                    dataGridView3.Columns["Стоимость заказа"].HeaderText = "Стоимость заказа";

                    dataGridView3.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SZI szi = new SZI();
            szi.Show();
            Hide();
            FillRequestDatesComboBox();
        }

        private void Operator_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            FillRequestDatesComboBox();
            FillRequestDatesComplexComboBox();
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

                string requestIdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand requestIdCommand = new SqlCommand(requestIdQuery, connection);
                requestIdCommand.Parameters.AddWithValue("@FullName", fullName);
                requestIdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)requestIdCommand.ExecuteScalar();

                string checkQuantityQuery = "SELECT rs.SziID, rs.Сount_SZI, w.Сount " +
                                            "FROM [Request and SZI] rs " +
                                            "JOIN Warehouse w ON rs.SziID = w.SziID " +
                                            "WHERE rs.RequestID = @RequestID";
                SqlCommand checkQuantityCommand = new SqlCommand(checkQuantityQuery, connection);
                checkQuantityCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                SqlDataReader quantityReader = checkQuantityCommand.ExecuteReader();

                bool hasEnoughQuantity = true;
                List<string> insufficientSZIs = new List<string>();

                while (quantityReader.Read())
                {
                    int requestedCount = Convert.ToInt32(quantityReader["Сount_SZI"]);
                    int warehouseCount = Convert.ToInt32(quantityReader["Сount"]);

                    if (requestedCount > warehouseCount)
                    {
                        hasEnoughQuantity = false;
                        string sziID = quantityReader["SziID"].ToString();
                        insufficientSZIs.Add(sziID);
                    }
                }

                quantityReader.Close();

                if (!hasEnoughQuantity)
                {
                    string errorMessage = "Insufficient quantity for the following SZIs: ";
                    errorMessage += string.Join(", ", insufficientSZIs);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string selectedPaymentMethod = comboBox9.SelectedItem.ToString();

                DateTime selectedPaymentDate = dateTimePicker2.Value;

                decimal totalPrice = decimal.Parse(textBox1.Text);

                string insertPaymentQuery = "INSERT INTO Payment (RequestID, payment_methodr, Date_Payment, Amount) VALUES (@RequestID, @PaymentMethod, @PaymentDate, @Amount)";
                SqlCommand insertPaymentCommand = new SqlCommand(insertPaymentQuery, connection);
                insertPaymentCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                insertPaymentCommand.Parameters.AddWithValue("@PaymentMethod", selectedPaymentMethod);
                insertPaymentCommand.Parameters.AddWithValue("@PaymentDate", selectedPaymentDate);
                insertPaymentCommand.Parameters.AddWithValue("@Amount", totalPrice);
                insertPaymentCommand.ExecuteNonQuery();

                string updateQuantityQuery = "UPDATE Warehouse SET Сount = Сount - rs.Сount_SZI " +
                                             "FROM [Request and SZI] rs " +
                                             "WHERE rs.RequestID = @RequestID AND rs.SziID = Warehouse.SziID";
                SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection);
                updateQuantityCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                updateQuantityCommand.ExecuteNonQuery();

                // Удаление связанных данных
                string deleteRelatedDataQuery = "DELETE FROM [Request and SZI] WHERE RequestID = @RequestID";
                SqlCommand deleteRelatedDataCommand = new SqlCommand(deleteRelatedDataQuery, connection);
                deleteRelatedDataCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                deleteRelatedDataCommand.ExecuteNonQuery();

                MessageBox.Show("Payment recorded and warehouse quantity updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                // Получение стоимости выбранной заявки
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

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

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
                                   "GROUP BY R.ID_Request, B.Surname_Buyer, B.Name_Buyer, B.Patronymic_Buyer;";

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView8.DataSource = dataTable;

                    dataGridView8.Columns["ID заказа"].HeaderText = "ID заявки";
                    dataGridView8.Columns["ФИО покупателя"].HeaderText = "ФИО покупателя";
                    dataGridView8.Columns["Стоимость заявки на комплексное решение"].HeaderText = "Стоимость заказа";

                    dataGridView8.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Complex complex = new Complex();
            complex.Show();
            Hide();
            FillRequestDatesComboBox();
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

                string requestIdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand requestIdCommand = new SqlCommand(requestIdQuery, connection);
                requestIdCommand.Parameters.AddWithValue("@FullName", fullName);
                requestIdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)requestIdCommand.ExecuteScalar();

                // Проверка количества СЗИ в заявке и на складе
                string checkQuantityQuery = "SELECT WS.SziID, WS.[Сount], ISNULL(SUM(CR.[Сount_Complex]), 0) AS TotalRequestedCount " +
                            "FROM [Warehouse] WS " +
                            "LEFT JOIN [Request and SZI] RS ON WS.SziID = RS.SziID " +
                            "LEFT JOIN [Complex and Request] CR ON RS.RequestID = CR.RequestID " +
                            "WHERE WS.SziID IN (SELECT SziID FROM [Complex solution and SZI] WHERE ComplexID IN (SELECT ComplexxID FROM [Complex and Request] WHERE RequestID = @RequestID)) " +
                            "GROUP BY WS.SziID, WS.[Сount]";
                SqlCommand checkQuantityCommand = new SqlCommand(checkQuantityQuery, connection);
                checkQuantityCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);

                SqlDataReader quantityReader = checkQuantityCommand.ExecuteReader();

                bool hasInsufficientQuantity = false;
                StringBuilder insufficientQuantityMessage = new StringBuilder("Недостаточное количество на складе для следующих СЗИ:\n");

                while (quantityReader.Read())
                {
                    int sziID = (int)quantityReader["SziID"];
                    int countOnWarehouse = (int)quantityReader["Сount"];
                    int totalRequestedCount = (int)quantityReader["TotalRequestedCount"];

                    if (countOnWarehouse < totalRequestedCount)
                    {
                        hasInsufficientQuantity = true;

                        // Получение названия СЗИ
                        string sziTitleQuery = "SELECT Title_szi FROM Szi WHERE ID_Szi = @SziID";
                        SqlCommand sziTitleCommand = new SqlCommand(sziTitleQuery, connection);
                        sziTitleCommand.Parameters.AddWithValue("@SziID", sziID);
                        string sziTitle = sziTitleCommand.ExecuteScalar().ToString();

                        insufficientQuantityMessage.AppendLine($"{sziTitle}: Запрошено - {totalRequestedCount}, На складе - {countOnWarehouse}");
                    }
                }

                quantityReader.Close();

                if (hasInsufficientQuantity)
                {
                    MessageBox.Show(insufficientQuantityMessage.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Запись данных об оплате
                string paymentQuery = "INSERT INTO Payment (RequestID, payment_methodr, Date_Payment, Amount) " +
                                      "VALUES (@RequestID, @PaymentMethod, @DatePayment, @Amount)";
                SqlCommand paymentCommand = new SqlCommand(paymentQuery, connection);
                paymentCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                paymentCommand.Parameters.AddWithValue("@PaymentMethod", comboBox3.SelectedItem.ToString());
                paymentCommand.Parameters.AddWithValue("@DatePayment", dateTimePicker3.Value);
                paymentCommand.Parameters.AddWithValue("@Amount", decimal.Parse(textBox2.Text));
                paymentCommand.ExecuteNonQuery();

                // Обновление количества на складе
                string updateWarehouseQuery = "UPDATE [Warehouse] " +
                              "SET [Сount] = [Сount] - (SELECT SUM([Сount_Complex]) FROM [Complex and Request] WHERE RequestID = @RequestID) " +
                              "WHERE SziID IN (SELECT SziID FROM [Complex solution and SZI] WHERE ComplexID IN (SELECT ComplexxID FROM [Complex and Request] WHERE RequestID = @RequestID))";
                SqlCommand updateWarehouseCommand = new SqlCommand(updateWarehouseQuery, connection);
                updateWarehouseCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                updateWarehouseCommand.ExecuteNonQuery();

                // Удаление связанных данных
                string deleteRelatedDataQuery = "DELETE FROM [Complex and Request] WHERE RequestID = @RequestID";
                SqlCommand deleteRelatedDataCommand = new SqlCommand(deleteRelatedDataQuery, connection);
                deleteRelatedDataCommand.Parameters.AddWithValue("@RequestID", selectedRequestID);
                deleteRelatedDataCommand.ExecuteNonQuery();


                MessageBox.Show("Оплата произведена успешно и данные обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                string requestIdQuery = "SELECT ID_Request FROM Request WHERE BuyerID IN (SELECT ID_Buyer FROM Buyer WHERE CONCAT(Surname_Buyer, ' ', Name_Buyer, ' ', Patronymic_Buyer) = @FullName) AND Date_Request = @RequestDate";
                SqlCommand requestIdCommand = new SqlCommand(requestIdQuery, connection);
                requestIdCommand.Parameters.AddWithValue("@FullName", fullName);
                requestIdCommand.Parameters.AddWithValue("@RequestDate", requestDate);
                int selectedRequestID = (int)requestIdCommand.ExecuteScalar();

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
    }
}
