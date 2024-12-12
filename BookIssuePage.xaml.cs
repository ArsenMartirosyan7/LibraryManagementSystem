using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for BookIssuePage.xaml
    /// </summary>
    public partial class BookIssuePage : Window
    {

        public ObservableCollection<Borrower> Borrowers { get; set; }

        public ICollectionView BorrowersView { get; set; }


        public BookIssuePage()
        {
            InitializeComponent();


            
            var converter = new BrushConverter();
            Borrowers = new ObservableCollection<Borrower>();



            
            LoadBorrowersFromDatabase("Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;");

            BorrowersView = CollectionViewSource.GetDefaultView(Borrowers);
            BorrowersDataGrid.ItemsSource = BorrowersView; // Bind DataGrid to the filtered collection
        }


        public void LoadBorrowersFromDatabase(string connectionString)
        {


            string query = "SELECT NameSurname, TitleAuthor, Email, PhoneNumber, IssueDate, ReturnDate FROM IssuedBooks";

            


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Borrowers.Clear(); // Clear existing data to avoid duplication
                            Random random = new Random();

                            while (reader.Read())
                            {
                                
                                Color randomColor = Color.FromRgb(
                                    (byte)random.Next(256), // Red
                                    (byte)random.Next(256), // Green
                                    (byte)random.Next(256)  // Blue
                                );

                                SolidColorBrush randomBrush = new SolidColorBrush(randomColor);




                                Borrowers.Add(new Borrower
                                {
                                    Number = (Borrowers.Count + 1).ToString(),
                                    Character = reader["NameSurname"].ToString().Substring(0, 1).ToUpper(),
                                    BgColor = randomBrush,
                                    NameSurname = reader["NameSurname"].ToString(),
                                    TitleAuthor = reader["TitleAuthor"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Phone = reader["PhoneNumber"].ToString(),
                                    IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading borrowers: {ex.Message}");
            }
        }


        private void ApplyFilter(string filterText)
        {
            if (BorrowersView == null)
                return;

            if (string.IsNullOrWhiteSpace(filterText))
            {
                BorrowersView.Filter = null; // Clear filter if no text is entered
            }
            else
            {
                BorrowersView.Filter = item =>
                {
                    var borrower = item as Borrower;
                    return borrower != null &&
                           (borrower.NameSurname.ToLower().Contains(filterText.ToLower()) ||
                            borrower.TitleAuthor.ToLower().Contains(filterText.ToLower()) ||
                            borrower.Email.ToLower().Contains(filterText.ToLower()) ||
                            borrower.Phone.ToLower().Contains(filterText.ToLower()));
                };
            }

            BorrowersView.Refresh();
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxFilter.Text; // Get the text entered by the user
            ApplyFilter(filterText);
        }

       


        private void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {

            IssueBookPage issueBookWindow = new IssueBookPage(Borrowers);
            issueBookWindow.ShowDialog();
        }

        private void BorrowerDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var borrowerToDelete = button?.CommandParameter as Borrower;

            if (borrowerToDelete != null)
            {
                // Remove the borrower from the collection (UI update)
                Borrowers.Remove(borrowerToDelete);

                // Database connection string
                string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

                // SQL query to delete the borrower
                string deleteQuery = "DELETE FROM IssuedBooks WHERE NameSurname = @NameSurname AND TitleAuthor = @TitleAuthor AND Email = @Email AND PhoneNumber = @PhoneNumber";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            // Add parameters to avoid SQL injection
                            command.Parameters.AddWithValue("@NameSurname", borrowerToDelete.NameSurname);
                            command.Parameters.AddWithValue("@TitleAuthor", borrowerToDelete.TitleAuthor);
                            command.Parameters.AddWithValue("Email", borrowerToDelete.Email);
                            command.Parameters.AddWithValue("PhoneNumber", borrowerToDelete.Phone);

                            // Execute the delete command
                            int rowsAffected = command.ExecuteNonQuery();

                           
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the borrower: {ex.Message}");
                }
            }
        }



        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {

            HomePage HomePageWindow = new HomePage();

            HomePageWindow.Show();

            this.Close();

        }

        private void BookIssuePageButton_Click(object sender, RoutedEventArgs e)
        {
            BookIssuePage bookIssuePage = new BookIssuePage();

            bookIssuePage.Show();

            this.Close();

        }


        private void MembersPageButton_Click(object sender, RoutedEventArgs e)
        {
            MembersPage membersPage = new MembersPage();
            membersPage.Show();
            this.Close();


        }

        private void LateReturnFeePageButton_Click(object sender, RoutedEventArgs e)
        {
            LateReturnFeePage lateReturnFeePage = new LateReturnFeePage();
            lateReturnFeePage.Show();
            this.Close();

        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();

        }

    }

    public class Borrower
    {
        public string Character { get; set; }
        public Brush BgColor { get; set; }
        public string Number { get; set; }
        public string NameSurname { get; set; }
        public string TitleAuthor { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ReturnDate { get; set; }

    }

}
