using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for AddFee.xaml
    /// </summary>
    public partial class AddFee : Window
    {

        public ObservableCollection<Debtor> Debtors { get; set; }
        public AddFee(ObservableCollection<Debtor> debtors)
        {
            InitializeComponent();
            Debtors = debtors;
        }

        private void IssueButton_Click(object sender, RoutedEventArgs e)
        {

            string NameSurname = NameSurnameTextBox.Text;
            string TitleAuthor = TitleAuthorTextBox.Text;
            string LateDays = LateDaysTextBox.Text;
            string DayFee = DayFeeTextBox.Text;
            int total = int.Parse(LateDays) * int.Parse(DayFee);
            string Total  = Convert.ToString(total); 





            Random random = new Random();
            Color randomColor = Color.FromRgb(
                (byte)random.Next(256), // Red
                (byte)random.Next(256), // Green
                (byte)random.Next(256)  // Blue
            );

            SolidColorBrush randomBrush = new SolidColorBrush(randomColor);

            Debtors.Add(new Debtor
            {
                Number = (Debtors.Count + 1).ToString(),
                Character = NameSurname.Substring(0, 1).ToUpper(),
                BgColor = randomBrush,
                NameSurname = NameSurname,
                TitleAuthor = TitleAuthor,
                LateDays = LateDays + " days",
                DayFee = DayFee + "$",
                Total = Total + "$",

            }
            );



            if (string.IsNullOrWhiteSpace(NameSurname) || 
                string.IsNullOrWhiteSpace(TitleAuthor) || 
                string.IsNullOrWhiteSpace(LateDays) || 
                string.IsNullOrWhiteSpace(DayFee))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Database connection string
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

            // SQL query to insert data
            string query = "INSERT INTO Debtors (NameSurname, TitleAuthor, LateDays, DayFee, Total) VALUES (@NameSurname, @TitleAuthor, @LateDays, @DayFee, @Total)";

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
                        command.Parameters.AddWithValue("@TitleAuthor", TitleAuthor);
                        command.Parameters.AddWithValue("@LateDays", LateDays);
                        command.Parameters.AddWithValue("@DayFee", DayFee);
                        command.Parameters.AddWithValue("@Total", Total);

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
