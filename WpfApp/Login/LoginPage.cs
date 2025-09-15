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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();

        //TODO Placed into a seperate method to avoid code duplication, consider refactoring
        // Image loading from URL has to be done in code-behind due to WPF limitations, working in Frame
        // WPF pages cannot directly bind Image-> Source to a URL in XAML, unlike using Window
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri("https://img.icons8.com/fluency/96/warehouse.png");
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        Logo.Source = bitmap;

    }

    private async void LoginClick(object sender, RoutedEventArgs e)
    {
      
        var loginData = new
        {
            Username = UsernameTextBox.Text,
            Password = PasswordBox.Password
        };

        var url = "http://localhost:5000/api/login/authenticate"; // Adjust the URL as needed
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {   
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10); // Set a timeout for the request
            // Send POST and wait for response
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await LoginSuccessful(response);
                return;
            }

            // Handle bad credentials or other HTTP errors
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Invalid username or password 😅");
            }
            else
            {
                MessageBox.Show($"Login failed. Server returned: {(int)response.StatusCode} - {response.ReasonPhrase}");
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

    private async Task LoginSuccessful(HttpResponseMessage response)
    {
        string responseContent = await response.Content.ReadAsStringAsync(); //Convert response to string
        using JsonDocument doc = JsonDocument.Parse(responseContent);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("token", out JsonElement tokenElement))
        {
            string? token = tokenElement.GetString();
        }
        else
        {
            MessageBox.Show("Login succeeded but no token received.");
            return;
        }
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Window.GetWindow(this).Close();
    }
    
    private void GoToRegister(object sender, RoutedEventArgs e)
    {
        var parentFrame = (Frame)Window.GetWindow(this).FindName("MainFrame");
        parentFrame.Navigate(new RegisterPage());
    }
}