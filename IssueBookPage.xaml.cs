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
    public partial class IssueBookPage : Window
    {


        public ObservableCollection<Borrower> Borrowers { get; set; }
        public IssueBookPage(ObservableCollection<Borrower> borrowers)
        {
            InitializeComponent();
            Borrowers = borrowers;

        }

        private void IssueButton_Click(object sender, RoutedEventArgs e)
        {

            string NameSurname = NameSurnameTextBox.Text;
            string TitleAuthor = TitleAuthorTextBox.Text;
            string Email = EmailTextBox.Text;
            string PhoneNumber = PhoneNumberTextBox.Text;
            int days = int.Parse(DurationTextBox.Text);
            
            



            Random random = new Random();
            Color randomColor = Color.FromRgb(
                (byte)random.Next(256), // Red
                (byte)random.Next(256), // Green
                (byte)random.Next(256)  // Blue
            );

            SolidColorBrush randomBrush = new SolidColorBrush(randomColor);

            Borrowers.Add(new Borrower 
            { Number = (Borrowers.Count + 1).ToString(), 
              Character = NameSurname.Substring(0, 1).ToUpper(), 
              BgColor = randomBrush, 
              NameSurname = NameSurname, 
              TitleAuthor = TitleAuthor,
              Email = Email, 
              Phone = PhoneNumber, 
              IssueDate = DateTime.Now,
              ReturnDate = DateTime.Now.AddDays(days)

            }
            );



            if (string.IsNullOrWhiteSpace(NameSurname) || string.IsNullOrWhiteSpace(TitleAuthor) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            


            // Database connection string
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

            // SQL query to insert data
            string query = "INSERT INTO IssuedBooks (NameSurname, TitleAuthor, Email, PhoneNumber, IssueDate, ReturnDate) VALUES (@NameSurname, @TitleAuthor, @Email, @PhoneNumber, @IssueDate, @ReturnDate)";

            // Perform database operation
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@NameSurname", NameSurname );
                        command.Parameters.AddWithValue("@TitleAuthor", TitleAuthor);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@IssueDate",DateTime.Now);
                        command.Parameters.AddWithValue("@ReturnDate", DateTime.Now.AddDays(days));


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
