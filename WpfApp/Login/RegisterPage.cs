using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp;

/// <summary>
/// Interaction logic for RegisterPage.xaml
/// </summary>
public partial class RegisterPage : Page
{
    public RegisterPage()
    {
        InitializeComponent();

        //TODO Placed into a seperate method to avoid code duplication, consider refactoring
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri("https://img.icons8.com/?size=100&id=fLQSwrK589zz&format=png&color=000000");
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        Logo.Source = bitmap;
    }

    public async void RegisterClick(object sender, RoutedEventArgs e)
    {
        // var parentFrame = (Frame)Window.GetWindow(this).FindName("MainFrame");
        // parentFrame.Navigate(new LoginPage());
        if (PasswordBox.Password != ConfirmPasswordBox.Password)
        {
            MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        else if (PasswordBox.Password.Length < 8)
        {
            MessageBox.Show("Password must be at least 8 characters long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var RegisterData = new
        {
            Username = UsernameTextBox.Text,
            Password = ConfirmPasswordBox.Password
        };

        var url = "http://localhost:5000/api/login/register";
        var json = JsonSerializer.Serialize(RegisterData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // NOTE/TODO
            // Creating a new HttpClient instance for each request is not optimal for production code.
            // Consider using a static HttpClient or IHttpClientFactory for better performance and resource management.
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10); // Set a timeout for the request
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GoToLogin();
                return;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Registration failed: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show("Oops! Could not reach the server: " + ex.Message);
        }
        catch (TaskCanceledException)
        {
            MessageBox.Show("Request timed out. Please try again.");
        }
    }

    public void GoToLoginEvent(object sender, RoutedEventArgs e)
    {
        GoToLogin();
    }

    public void GoToLogin()
    {
        var parentFrame = (Frame)Window.GetWindow(this).FindName("MainFrame");
        parentFrame.Navigate(new LoginPage());
    }
}