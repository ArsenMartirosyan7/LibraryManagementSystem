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
using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for MembersPage.xaml
    /// </summary>
    public partial class MembersPage : Window
    {
        private readonly object memberToDelete;

        public ObservableCollection<Member> Members { get; set; }
        public ICollectionView MembersView { get; set; }


        public MembersPage()
        {
            InitializeComponent();

            var converter = new BrushConverter();
            Members = new ObservableCollection<Member>();

            
            LoadMembersFromDatabase("Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;");


            MembersView = CollectionViewSource.GetDefaultView(Members);
            MembersDataGrid.ItemsSource = MembersView;

        }


        public void LoadMembersFromDatabase(string connectionString)
        {


            string query = "SELECT NameSurname, Address, Email, PhoneNumber FROM Members";




            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Members.Clear(); // Clear existing data to avoid duplication
                            Random random = new Random();

                            while (reader.Read())
                            {

                                Color randomColor = Color.FromRgb(
                                    (byte)random.Next(256), // Red
                                    (byte)random.Next(256), // Green
                                    (byte)random.Next(256)  // Blue
                                );

                                SolidColorBrush randomBrush = new SolidColorBrush(randomColor);




                                Members.Add(new Member
                                {
                                    Number = (Members.Count + 1).ToString(),
                                    Character = reader["NameSurname"].ToString().Substring(0, 1).ToUpper(),
                                    Address = reader["Address"].ToString(),
                                    BgColor = randomBrush,
                                    NameSurname = reader["NameSurname"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Phone = reader["PhoneNumber"].ToString()
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
            if (MembersView == null)
                return;

            if (string.IsNullOrWhiteSpace(filterText))
            {
                MembersView.Filter = null; // Clear filter if no text is entered
            }
            else
            {
                MembersView.Filter = item =>
                {
                    var member = item as Member;
                    return member != null &&
                           (member.NameSurname.ToLower().Contains(filterText.ToLower()) ||
                            member.Address.ToLower().Contains(filterText.ToLower()) ||
                            member.Email.ToLower().Contains(filterText.ToLower()) ||
                            member.Phone.ToLower().Contains(filterText.ToLower()));
                };
            }

            MembersView.Refresh();
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxFilter.Text; // Get the text entered by the user
            ApplyFilter(filterText);
        }



        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewMemberPage addNewMemberPage = new AddNewMemberPage(Members);
            addNewMemberPage.ShowDialog();
            
        }

        private void MemberDeleteButton_Click(Object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var memberToDelete = button?.CommandParameter as Member;

            if (memberToDelete != null)
            {
                
                Members.Remove(memberToDelete);

                // Database connection string
                string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

                // SQL query to delete the borrower
                string deleteQuery = "DELETE FROM Members WHERE NameSurname = @NameSurname AND Address = @Address AND Email = @Email AND PhoneNumber = @PhoneNumber";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            // Add parameters to avoid SQL injection
                            command.Parameters.AddWithValue("@NameSurname", memberToDelete.NameSurname);
                            command.Parameters.AddWithValue("@Address", memberToDelete.Address);
                            command.Parameters.AddWithValue("@Email", memberToDelete.Email);
                            command.Parameters.AddWithValue("@PhoneNumber", memberToDelete.Phone);

                            // Execute the delete command
                            int rowsAffected = command.ExecuteNonQuery();


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the member: {ex.Message}");
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

    public class Member
    {
        public string Character { get; set; }
        public Brush BgColor { get; set; }
        public string Number { get; set; }
        public string NameSurname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }


}
