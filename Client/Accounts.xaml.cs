using Client.Models;
using RestSharp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : Page
    {
        RestClient client = new RestClient("http://localhost:5235/");
        Models.Account account;
        public Accounts()
        {
            InitializeComponent();
        }

        private void UpdateAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            account.Balance = int.Parse(EditBalance.Text);
            RestRequest request = new RestRequest("api/bank/accounts", Method.Put);
            request.AddJsonBody(account);
            RestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Account data updated successfully.");
            }
            else
            {
                MessageBox.Show($"Failed to update account data. Error: {response.ErrorMessage}");
            }
        }

        private void DeleteAccBtn_Click(object sender, RoutedEventArgs e)
        {
            int retrieveAccountId = int.Parse(AccountIdInput.Text);

            RestRequest request = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Delete);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Account deleted successfully.");
            }
            else
            {
                MessageBox.Show($"Failed to delete account data. Error: {response.ErrorMessage}");
            }
        }

        private void RetrieveAccDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            int retrieveAccountId = int.Parse(AccountIdInput.Text);

            RestRequest accountRequest = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Get);
            RestResponse<Models.Account> accountResponse = client.Execute<Models.Account>(accountRequest);

            if (accountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                int retrievedUserId = accountResponse.Data.UserId;

                RestRequest userRequest = new RestRequest("api/bank/users", Method.Get);
                var userListResponse = client.Execute<List<Models.UserProfile>>(userRequest);

                if (userListResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var matchedUser = userListResponse.Data.FirstOrDefault(u => u.UserId == retrievedUserId);

                    if (matchedUser != null)
                    {
                        DisplayAccountHolderBox.Text = $"{matchedUser.FirstName} {matchedUser.LastName}";
                        DisplayBalanceBox.Text = accountResponse.Data.Balance.ToString();
                        DisplayAccountIdBox.Text = retrieveAccountId.ToString();

                        account = new Account();
                        account.AccountId = accountResponse.Data.AccountId;
                        account.UserId = retrievedUserId;
                        account.Balance = accountResponse.Data.Balance;
                    }
                    else
                    {
                        MessageBox.Show("No user found for the given account.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve user list.");
                }
            }
            else
            {
                MessageBox.Show("Account Id does not exist");
            }
        }

        private void CreateNewAcc_Click(object sender, RoutedEventArgs e)
        {
            int userId = int.Parse(UserIdInput.Text);

            // Check if user exists
            RestRequest userRequest = new RestRequest("api/bank/users", Method.Get);
            var userResponse = client.Execute<List<Models.UserProfile>>(userRequest);
            if (userResponse.Data.Any(u => u.UserId == userId))
            {
                // Get existing accounts to determine the next AccountId
                RestRequest accountRequest = new RestRequest("api/bank/accounts", Method.Get);
                var accountResponse = client.Execute<List<Account>>(accountRequest);
                int nextAccountId = accountResponse.Data.Count + 1;

                Account newAccount = new Account
                {
                    AccountId = nextAccountId,
                    UserId = userId,
                    Balance = int.Parse(BalanceInput.Text)
                };

                RestRequest createRequest = new RestRequest("api/bank/accounts", Method.Post);
                createRequest.AddJsonBody(newAccount);
                var createResponse = client.Execute(createRequest);
                if (createResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show($"Successfully created account with AccountId: {nextAccountId}");
                }
                else
                {
                    MessageBox.Show("Account already exists with UserId " + userId);
                }
            }
            else
            {
                MessageBox.Show($"User with UserId {userId} does not exist.");
            }
        }
    }
}
