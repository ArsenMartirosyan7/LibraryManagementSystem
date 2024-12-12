using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;


namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for IssueBookPage.xaml
    /// </summary>
    public partial class AddNewMemberPage : Window
    {


        public ObservableCollection<Member> Members { get; set; }
        public AddNewMemberPage(ObservableCollection<Member> members)
        {
            InitializeComponent();
            Members = members;

        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {

            string NameSurname = NameSurnameTextBox.Text;
            string Address = AddressTextBox.Text;
            string Email = EmailTextBox.Text;
            string PhoneNumber = PhoneNumberTextBox.Text;




            Random random = new Random();
            Color randomColor = Color.FromRgb(
                (byte)random.Next(256), // Red
                (byte)random.Next(256), // Green
                (byte)random.Next(256)  // Blue
            );

            SolidColorBrush randomBrush = new SolidColorBrush(randomColor);

            Members.Add(new Member
            {
                Number = (Members.Count + 1).ToString(),
                Character = NameSurname.Substring(0, 1).ToUpper(),
                BgColor = randomBrush,
                NameSurname = NameSurname,
                Address = Address,
                Email = Email,
                Phone = PhoneNumber
            }
            );



            if (string.IsNullOrWhiteSpace(NameSurname) || string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }




            // Database connection string
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

            // SQL query to insert data
            string query = "INSERT INTO Members (NameSurname, Address, Email, PhoneNumber) VALUES (@NameSurname, @Address, @Email, @PhoneNumber)";

            // Perform database operation
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@NameSurname", NameSurname);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                        int result = command.ExecuteNonQuery();


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



            this.Close();



        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
