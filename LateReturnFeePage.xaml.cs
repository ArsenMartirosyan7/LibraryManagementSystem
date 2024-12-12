using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for LateReturnFeePage.xaml
    /// </summary>
    public partial class LateReturnFeePage : Window
    {

        public ObservableCollection<Debtor> Debtors { get; set; }
        public ICollectionView DebtorsView { get; set; }

        public LateReturnFeePage()
        {
            InitializeComponent();

            var converter = new BrushConverter();
            Debtors = new ObservableCollection<Debtor>();

            

            LoadDebtorsFromDatabase("Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;");

            DebtorsView = CollectionViewSource.GetDefaultView(Debtors);
            DebtorsDataGrid.ItemsSource = DebtorsView;
        }



        public void LoadDebtorsFromDatabase(string connectionString)
        {


            string query = "SELECT NameSurname, TitleAuthor, LateDays, DayFee, Total FROM Debtors";




            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Debtors.Clear(); // Clear existing data to avoid duplication
                            Random random = new Random();

                            while (reader.Read())
                            {

                                Color randomColor = Color.FromRgb(
                                    (byte)random.Next(256), // Red
                                    (byte)random.Next(256), // Green
                                    (byte)random.Next(256)  // Blue
                                );

                                SolidColorBrush randomBrush = new SolidColorBrush(randomColor);




                                Debtors.Add(new Debtor
                                {
                                    Number = (Debtors.Count + 1).ToString(),
                                    Character = reader["NameSurname"].ToString().Substring(0, 1).ToUpper(),
                                    TitleAuthor = reader["TitleAuthor"].ToString(),
                                    LateDays = reader["LateDays"].ToString(),
                                    BgColor = randomBrush,
                                    NameSurname = reader["NameSurname"].ToString(),
                                    DayFee = reader["DayFee"].ToString(),
                                    Total = reader["Total"].ToString()
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
            if (DebtorsView == null)
                return;

            if (string.IsNullOrWhiteSpace(filterText))
            {
                DebtorsView.Filter = null; // Clear filter if no text is entered
            }
            else
            {
                DebtorsView.Filter = item =>
                {
                    var Debtor = item as Debtor;
                    return Debtor != null &&
                           (Debtor.NameSurname.ToLower().Contains(filterText.ToLower()) ||
                            Debtor.TitleAuthor.ToLower().Contains(filterText.ToLower()) ||
                            Debtor.DayFee.ToLower().Contains(filterText.ToLower()) ||
                            Debtor.LateDays.ToLower().Contains(filterText.ToLower()) ||
                            Debtor.Total.ToLower().Contains(filterText.ToLower()));
                };
            }

            DebtorsView.Refresh();
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxFilter.Text; 
            ApplyFilter(filterText);
        }


         private void FeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddFee addFee = new AddFee(Debtors);
            addFee.ShowDialog();

        }

       private void PaidButton_Click(Object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var debtorToDelete = button?.CommandParameter as Debtor;

            if (debtorToDelete != null)
            {

                Debtors.Remove(debtorToDelete);

                // Database connection string
                string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

                // SQL query to delete the borrower
                string deleteQuery = "DELETE FROM Debtors WHERE NameSurname = @NameSurname AND TitleAuthor = @TitleAuthor";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            // Add parameters to avoid SQL injection
                            command.Parameters.AddWithValue("@NameSurname", debtorToDelete.NameSurname);
                            command.Parameters.AddWithValue("@TitleAuthor", debtorToDelete.TitleAuthor);
                            

                            // Execute the delete command
                            int rowsAffected = command.ExecuteNonQuery();


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the debtor: {ex.Message}");
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

    public class Debtor
    {
        public string Character { get; set; }
        public Brush BgColor { get; set; }
        public string Number { get; set; }
        public string NameSurname { get; set; }
        public string LateDays { get; set; }
        public string TitleAuthor { get; set; }
        public string DayFee { get; set; }
        public string Total { get; set; }
    }

}
