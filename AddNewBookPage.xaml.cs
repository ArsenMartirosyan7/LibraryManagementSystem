using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using Microsoft.Data.SqlClient;
using System.IO;


namespace LibraryManagementSystem
{
    public partial class AddNewBookPage : Window
    {
        public string BookTitle { get; private set; }
        public string Author { get; private set; }
        public BitmapImage CoverImage { get; private set; }
        public bool IsBookAdded { get; private set; }

        public AddNewBookPage()
        {
            InitializeComponent();
            IsBookAdded = false; 
        }

        // Event handler for the "Cancel" button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the window
        }

     




        // Event handler for the "Choose Image" button
        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png, *.jpeg)|*.jpg;*.png;*.jpeg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                BookImagePreview.Source = bitmap;
                CoverImage = bitmap; // Store the image for later use
            }
        }


        public static class ImageHelper
        {
            public static string ConvertImageToBase64(BitmapImage image)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }

            public static BitmapImage ConvertBase64ToImage(string base64String)
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var stream = new MemoryStream(imageBytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
            }
        }







        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            BookTitle = BookNameTextBox.Text;
            Author = AuthorTextBox.Text;

            if (string.IsNullOrWhiteSpace(BookTitle) || string.IsNullOrWhiteSpace(Author) || CoverImage == null)
            {
                MessageBox.Show("Please fill in all fields and select a book cover.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Convert the image to Base64
            string base64Image = ImageHelper.ConvertImageToBase64(CoverImage);

            // Database connection string
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

            // SQL query to insert the book with Base64 image
            string query = "INSERT INTO Books (Title, Author, CoverImage) VALUES (@Title, @Author, @CoverImage)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Title", BookTitle);
                        command.Parameters.AddWithValue("@Author", Author);
                        command.Parameters.AddWithValue("@CoverImage", base64Image);

                        command.ExecuteNonQuery();
                    }
                }

                IsBookAdded = true; // Indicate that the book was added successfully
                this.Close(); // Close the Add New Book page
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}