using BankServer_WebAPI.Models;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankServer_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        [HttpGet]
        [Route ("users")]
        public IEnumerable<UserProfile> getUsers()
        {
            List<UserProfile> users = DBManager.GetAllUsers();
            return users;
        }

        [HttpGet]
        [Route("users/{identifier}")]
        public IActionResult getUserByNameOrEmail(string identifier)
        {
            UserProfile userProfile = DBManager.GetUserProfile(identifier);
            if (userProfile == null)
            {
                return NotFound();
            }
            return Ok(userProfile);
        }

        [HttpPost]
        [Route("adduser")]
        public IActionResult newUser([FromBody] UserProfile user)
        {
            if (DBManager.InsertUserProfile(user))
            {
                var response = new { Message = "User added successfully" };
                return new ObjectResult(response)
                {
                    StatusCode = 200,
                    ContentTypes = { "application/json" }
                };
            }
            return BadRequest("Error in user addition");
        }

        [HttpDelete]
        [Route("users/{identifier}")]
        public IActionResult deleteUser(int identifier)
        {
            if (DBManager.DeleteUserProfile(identifier))
            {
                return Ok("Successfully deleted user.");
            }
            return BadRequest("User could not be deleted");
        }

        [HttpPut]
        public IActionResult updateUser(UserProfile user)
        {
            if (DBManager.UpdateUserProfile(user))
            {
                return Ok("Successfully updated user profile");
            }
            return BadRequest("Could not update user profile");
        }

        [HttpGet]
        [Route("accounts")]
        public IEnumerable<Account> GetAccounts()
        {
            List<Account> accounts = DBManager.GetAllAccounts();
            return accounts;
        }

        [HttpGet]
        [Route("accounts/{accountId}")]
        public IActionResult GetAccountById(int accountId)
        {
            Account account = DBManager.GetAccountById(accountId);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPost]
        [Route("accounts")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            if (DBManager.InsertAccount(account))
            {
                return Ok("Successfully created account.");
            }
            return BadRequest("Error in account creation");
        }

        [HttpPut]
        [Route("accounts")]
        public IActionResult UpdateAccount(Account account)
        {
            if (DBManager.UpdateAccount(account))
            {
                return Ok("Successfully updated account.");
            }
            return BadRequest("Could not update account");
        }

        [HttpDelete]
        [Route("accounts/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            if (DBManager.DeleteAccount(accountId))
            {
                return Ok("Successfully deleted account.");
            }
            return BadRequest("Account could not be deleted");
        }

        [HttpGet]
        [Route("transactions")]
        public IEnumerable<Transaction> GetTransactions()
        {
            List<Transaction> transactions = DBManager.GetAllTransactions();
            return transactions;
        }

        [HttpGet]
        [Route("transactions/{transactionId}")]
        public IActionResult GetTransactionById(int transactionId)
        {
            Transaction transaction = DBManager.GetTransactionById(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost]
        [Route("transactions")]
        public IActionResult CreateTransaction([FromBody] Transaction transaction)
        {
            if (DBManager.InsertTransaction(transaction))
            {
                return Ok("Successfully created transaction.");
            }
            return BadRequest("Error in transaction creation");
        }

        [HttpPut]
        [Route("transactions")]
        public IActionResult UpdateTransaction(Transaction transaction)
        {
            if (DBManager.UpdateTransaction(transaction))
            {
                return Ok("Successfully updated transaction.");
            }
            return BadRequest("Could not update transaction");
        }

        [HttpDelete]
        [Route("transactions/{transactionId}")]
        public IActionResult DeleteTransaction(int transactionId)
        {
            if (DBManager.DeleteTransaction(transactionId))
            {
                return Ok("Successfully deleted transaction.");
            }
            return BadRequest("Transaction could not be deleted");
        }

        [HttpGet]
        [Route("transactions/account/{accountId}")]
        public IEnumerable<Transaction> GetTransactionsByAccountId(int accountId)
        {
            List<Transaction> transactions = DBManager.GetAllTransactions();
            List<Transaction> returnTransactions = new List<Transaction>(); 

            foreach (Transaction transaction in transactions) 
            { 
                if (transaction.AccountId == accountId)
                {
                    returnTransactions.Add(transaction);
                }    
            }

            return returnTransactions;
        }
    }
}
