using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Contains a list of accounts used for testing purposes.
    /// </summary>
    public class TestAccounts {
        public List<TestAccount> Accounts { get; set; }
        public TestAccounts() {
            InitTestAccounts();
        }
        private void InitTestAccounts() {
            Accounts = new List<TestAccount>();
            Accounts.Add(new TestAccount("mary.smith@cchcaesars.com", "dem0-User", "000004835", "crazytown", "6176668888"));
            //Accounts.Add(new TestAccount("mary.s@cchdemo.com", "password1", "999993333", ""));  //UnitTestContext throws an exception on this account.
        }
    }
    public class TestAccount {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullSsn { get; set; }
        public string SecretAnswer { get; set; }
        public string Phone { get; set; }

        public TestAccount() { }
        public TestAccount(string username, string password, string fullSsn, string secretAnswer, string phone) {
            Username = username;
            Password = password;
            FullSsn = fullSsn;
            SecretAnswer = secretAnswer;
            Phone = phone;
        }
    }
}
