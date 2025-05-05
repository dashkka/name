// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SqlClient;


namespace IDZ_szi
{
    public partial class Input : Form
    {
        private SqlConnection connection;
        public static bool flagAnonim;
        public static string emaiAddress;
        public static string name;
        public static string patronymic;

        public Input()
        {
            InitializeComponent();
            string connectionString = "Data Source=LAPTOP-JSVR0I7M;Initial Catalog=IDZ;User Id=user1;Password=user1;Persist Security Info=True";
            connection = new SqlConnection(connectionString);
        }

        private void entry_Click(object sender, EventArgs e)
        {
            flagAnonim = false;
            var email = tb_mail.Text;
            emaiAddress = tb_mail.Text;
            var password = tb_password.Text;

            var hashedPassword = HashPassword(password);

            if (CheckUser(email, hashedPassword))
            {
                var roleId = GetRoleId(email);
                if (roleId == 1)// 1 это админ
                {
                    Admin admin = new Admin();
                    admin.Show();
                    Hide();
                }
                else if (roleId == 2) // 2 это модератор 
                {
                    
                    Moderator moderator = new Moderator();
                    moderator.Show();
                    Hide();
                }
                else if (roleId == 3) // 3 это оператор 
                {

                    Operator operat = new Operator();
                    operat.Show();
                    Hide();
                }
                else if (roleId == 4) // 4 это обычный сотрудник 
                {
                    Employee employee = new Employee();
                    employee.Show();
                    Hide();

                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
                tb_mail.Text = "";
                tb_password.Text = "";
            }
        }
        private int GetRoleId(string email)
        {
            try
            {
                connection.Open();

                var query = $"SELECT PostID FROM Employee WHERE Email_Employee='{email}'";
                var command = new SqlCommand(query, connection);
                var result = (int)command.ExecuteScalar();

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении роли пользователя: {ex.Message}");
                return -1;
            }
            finally
            {
                connection.Close();
            }
        }
        private bool CheckUser(string email, string hashedPassword)
        {
            try
            {
                connection.Open();

                var query = "SELECT COUNT(*) FROM Employee WHERE Email_Employee = @UserMail AND Password_Employee = @UserPassword";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserMail", email);
                command.Parameters.AddWithValue("@UserPassword", hashedPassword);

                var result = (int)command.ExecuteScalar();

                return result == 1;
            }
            catch (Exception)
            {
                return false;

            }
            finally
            {
                connection.Close();
            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create()) //создаем объект SHA256
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));//хэшируем переданный пароль
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();//тут конвертируется хэшированный пароль в чо-то
                return hash;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                tb_password.UseSystemPasswordChar = false;
            }
            else
            {
                tb_password.UseSystemPasswordChar = true;
            }
        }

        private void Input_Load(object sender, EventArgs e)
        {

        }
    }
}
