using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Page
    {
        Models.UserProfile newUser;
        public CreateUser()
        {
            InitializeComponent();
            newUser = new Models.UserProfile();
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SystemOptions());
        }

        private void RetrieveUserBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RetrieveUser());
        }

        private void CreateNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            RestClient restClient = new RestClient("http://localhost:5235");

            RestRequest getUsersRequest = new RestRequest("/api/bank/users", Method.Get);
            RestResponse<List<Models.UserProfile>> getUsersResponse = restClient.Execute<List<Models.UserProfile>>(getUsersRequest);
            if (getUsersResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                int numberOfExistingUsers = getUsersResponse.Data.Count;
                newUser.UserId = numberOfExistingUsers + 1;
            }
            else
            {
                MessageBox.Show("Error fetching the number of users. Please try again.");
                return;
            }


            newUser.FirstName = FirstNameBox.Text; 
            newUser.LastName = LastNameBox.Text;
            newUser.Email = EmailBox.Text;
            newUser.Password = PasswordBox.Text;
            newUser.Address = AddressBox.Text;
            newUser.Phone = PhoneBox.Text;
            newUser.Username = UsernameBox.Text;


            List<Models.UserProfile> userProfiles = getUsersResponse.Data;
            for (int i = 0; i < userProfiles.Count; i++)
            {
                if (userProfiles[i].Username == newUser.Username)
                {
                    MessageBox.Show("Username already exists");
                    return;
                }
            }

            RestRequest restRequest = new RestRequest("/api/bank/adduser", Method.Post);
            restRequest.AddJsonBody(newUser);
            RestResponse restResponse = restClient.Execute(restRequest);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show($"Successfully created user {newUser.FullName}\nUser id is: " + newUser.UserId);
            }
            else
            {
                MessageBox.Show($"Failed to create user. Error: {restResponse.ErrorMessage}");
            }
        }

        private void UploadDP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Added New Profile Picture to User Profile");

            // Generate a red image as a base64 string
            newUser.Picture = GenerateRedImageAsBase64(64, 64);

            // Convert the base64 string to a BitmapImage and set as the Image source
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new System.IO.MemoryStream(System.Convert.FromBase64String(newUser.Picture));
            bi.EndInit();
            ProfilePicture.Source = bi;  // Assuming the Image control's name is "ProfilePicture"
        }

        private static string GenerateRedImageAsBase64(int width, int height)
        {
            using (Bitmap bmp = new System.Drawing.Bitmap(width, height))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(System.Drawing.Color.Red); // Fill the entire image with a red color
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
