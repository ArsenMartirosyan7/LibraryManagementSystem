
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem
{
    public partial class LoginPage : Window
    {

       
        public LoginPage()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }
        // In LoginPage.xaml.cs
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                // Display a message or highlight the text box if empty
                textUsername.Visibility = Visibility.Visible;
            }
            else
            {
                textUsername.Visibility = Visibility.Collapsed; // Hide placeholder when there's input
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                textPassword.Visibility = Visibility.Visible;
            }
            else
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        
        string username = txtUsername.Text.Trim(); 
        string password = passwordBox.Password.Trim();

        // Connection string for the database
        string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;"; 

        try
        {
            // SQL query to check if the username and password exist in the database
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

            // Using block to ensure the connection is properly closed and disposed
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlCommand object to execute the query
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                    command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;

                    // Execute the query and get the result
                    int count = Convert.ToInt32(command.ExecuteScalar());


                        HomePage HomePageWindow = new HomePage();

                        HomePageWindow.Show();

                        this.Close();
                    }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the database operation
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpPage secondWindow = new SignUpPage();

            secondWindow.Show();
 
            this.Close();
        }


    }
}
