using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using static LibraryManagementSystem.HomePage;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using Microsoft.Data.SqlClient;
using static LibraryManagementSystem.AddNewBookPage;
using System.Linq;
using System.Windows.Data;


namespace LibraryManagementSystem
{
    public partial class HomePage : Window
    {
        public ObservableCollection<Book> Books { get; set; }
        private ICollectionView BooksView { get; set; }

        public HomePage()
        {
            try
            {
                InitializeComponent();

                Books = new ObservableCollection<Book>();
                BooksView = CollectionViewSource.GetDefaultView(Books);


                LoadBooks();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }


        private void LoadBooks()
        {
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";
            string query = "SELECT Title, Author, CoverImage FROM Books";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["Title"].ToString();
                            string author = reader["Author"].ToString();
                            string base64Image = reader["CoverImage"].ToString();

                            // Convert Base64 to BitmapImage
                            BitmapImage coverImage = ImageHelper.ConvertBase64ToImage(base64Image);

                            // Add book to grid
                            AddBookToGrid(title, author, coverImage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading books: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void AddNewBook_Click(object sender, RoutedEventArgs e)
        {
            // Open the "Add New Book" page
            AddNewBookPage addNewBookPage = new AddNewBookPage();
            addNewBookPage.ShowDialog();

            // Check if a book was added
            if (addNewBookPage.IsBookAdded)
            {
                // Add the new book to the grid
                AddBookToGrid(addNewBookPage.BookTitle, addNewBookPage.Author, addNewBookPage.CoverImage);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ApplySearch(string searchText)
        {
            if (BooksView == null)
                return;



            if (string.IsNullOrWhiteSpace(searchText))
            {
                BooksView.Filter = null;
            }
            else
            {
                BooksView.Filter = item =>
                {
                    var book = item as Book;
                    return book != null &&
                           (book.Title.ToLower().Contains(searchText.ToLower()) || book.Title.ToLower().Contains(searchText.ToLower()));
                };
            }

            BooksView.Refresh();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxSearch.Text;
            ApplySearch(searchText);
        }

        private void AddBookToGrid(string title, string author, BitmapImage coverImage)
        {
            // Determine grid position
            int columns = 4;  // Example: 4 books per row
            int bookCount = BooksGrid.Children.Count;

            int rowIndex = bookCount / columns;
            int columnIndex = bookCount % columns;

            // Dynamically add rows and columns if needed
            if (rowIndex >= BooksGrid.RowDefinitions.Count)
            {
                BooksGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            if (columnIndex >= BooksGrid.ColumnDefinitions.Count)
            {
                // Correctly define a star-based column width
                BooksGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            } 




            // Create a container for the book
            Grid bookContainer = new Grid
            {
                Width = 200,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Add a row for the delete button and the rest of the content
            bookContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            bookContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create the Delete button
            Button deleteButton = new Button
            {
                Content = "🗑",
                Background = Brushes.DarkRed,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 30,
                Height = 30,
                
            };

            deleteButton.Click += (s, e) => DeleteBook(title, author); // Attach delete event handler

            // Create a stack panel for the book details
            StackPanel bookDetails = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add the book cover image
            Image bookCover = new Image
            {
                Source = coverImage,
                Width = 150,
                Height = 220,
                Stretch = Stretch.UniformToFill
            };
            bookDetails.Children.Add(bookCover);

            // Add the book title
            TextBlock bookTitle = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 10, 5, 0)
            };
            bookDetails.Children.Add(bookTitle);

            // Add the book author
            TextBlock bookAuthor = new TextBlock
            {
                Text = $"by {author}",
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 0, 5, 10)
            };
            bookDetails.Children.Add(bookAuthor);

            // Add delete button and book details to the container
            Grid.SetRow(deleteButton, 0);
            Grid.SetRow(bookDetails, 1);

            bookContainer.Children.Add(deleteButton);
            bookContainer.Children.Add(bookDetails);

            // Set the position of the book container in the grid
            Grid.SetRow(bookContainer, rowIndex);
            Grid.SetColumn(bookContainer, columnIndex);

            // Add the book container to the grid
            BooksGrid.Children.Add(bookContainer);
        }


        private void DeleteBook(string title, string author)
        {
            // Database connection string
            string connectionString = "Data Source=LAPTOP-E5R5VIMU\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=true;TrustServerCertificate=true;";

            // SQL query to delete the book
            string query = "DELETE FROM Books WHERE Title = @Title AND Author = @Author";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Author", author);

                        command.ExecuteNonQuery();
                    }
                }

                // Find and remove the book container from the grid
                var bookContainer = BooksGrid.Children
                    .OfType<Grid>()
                    .FirstOrDefault(container =>
                    {
                        var titleBlock = container.Children
                            .OfType<StackPanel>()
                            .FirstOrDefault()?
                            .Children
                            .OfType<TextBlock>()
                            .FirstOrDefault(tb => tb.Text == title);

                        return titleBlock != null;
                    });

                if (bookContainer != null)
                {
                    BooksGrid.Children.Remove(bookContainer);
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        


      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
          
        }




        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Additional functionality can be added here if needed
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }


       
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Button Clicked!");
            
        }

        // Event handler for search TextBox (search icon click or TextBox action)
        private void SearchBox_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        // Event handler for settings button (cogwheel icon)
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings Button Clicked!");
            // Navigate to settings page or show settings dialog
        }

        // Event handler for notification button (bell icon)
        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Notification Button Clicked!");
            // Show notifications or open notifications panel
        }

        // Event handler for Add New Member button
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add New Member Button Clicked!");
            // Implement logic to add a new member
        }

        // Event handler for tab button click
        private void TabButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Event handler for editing a grid row
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Button Clicked!");
            // Implement logic to edit the selected row
        }

        // Event handler for deleting a grid row
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Remove Button Clicked!");
            
        }

        // Event handler for pagination buttons
        private void PagingButton_Click(object sender, RoutedEventArgs e)
        {

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







        public class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string ImagePath { get; set; }
        }

        
    }
}
