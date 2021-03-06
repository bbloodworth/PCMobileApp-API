﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Contains a list of accounts used for testing purposes.
    /// </summary>
    public sealed class TestAccounts {
        private static List<Account> _accounts;
        private static DemoAccount _demoAccounts;
        private static CaesarsAccount _caesarsAccounts;
        private static TyLinAccount _tyLinAccounts;
        public static List<Account> Accounts {
            get {
                if (_accounts == null) {
                    InitTestAccounts();
                }
                return _accounts;
            }
            private set {
                _accounts = value;
            }
        }
        public static DemoAccount DemoAccounts {
            get {
                if (_demoAccounts == null) {
                    InitDemoTestAccounts();
                }
                return _demoAccounts;
            }
            private set {
                _demoAccounts = value;
            }
        }
        public static CaesarsAccount CaesarsAccounts {
            get {
                if (_caesarsAccounts == null) {
                    InitCaesarsTestAccounts();
                }
                return _caesarsAccounts;
            }
            private set {
                _caesarsAccounts = value;
            }
        }
        public static TyLinAccount TyLinAccounts {
            get {
                if (_tyLinAccounts == null) {
                    InitTyLinTestAccounts();
                }
                return _tyLinAccounts;
            }
            private set {
                _tyLinAccounts = value;
            }
        }

        public TestAccounts() {
            InitTestAccounts();
        }
        private static void InitTestAccounts() {
            InitCaesarsTestAccounts();
            InitDemoTestAccounts();

            Accounts = new List<Account>();
            Accounts.AddRange(CaesarsAccounts.Accounts);
            //Accounts.AddRange(DemoAccounts.Accounts); //UnitTesting class doesn't like this account.
        }

        private static void InitCaesarsTestAccounts() {
            CaesarsAccounts = new CaesarsAccount();
        }

        private static void InitDemoTestAccounts() {
            DemoAccounts = new DemoAccount();
        }

        private static void InitTyLinTestAccounts() {
            TyLinAccounts = new TyLinAccount();
        }
    }
    public class Account {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullSsn { get; set; }
        public string SecretAnswer { get; set; }
        public string Phone { get; set; }
        public string ApiKey { get; set; }
        public string AuthHash { get; set; }
        public int EmployerId { get; set; }
        public int CchId { get; set; }
        public string PaycheckDocumentId { get; set; }
        public int[] BenefitPlans { get; set; }
        public int MedicalPlanId { get; set; }

        public Account() { }
    }

    public class TestAccount {
        public List<Account> Accounts { get; private set; }

        public TestAccount() {
            Accounts = new List<Account>();
        }
    }
    #region Demo accounts
    public class DemoAccount : TestAccount {
        //public List<Account> Accounts { get; private set; }
        public Account MaryS { get; private set; }

        public DemoAccount() 
            : base() {
            InitAccounts();
        }

        private void InitAccounts() {
            MaryS = GetMaryS();

            Accounts.Add(MaryS);
        }

        private Account GetMaryS() {
            var account = new Account {
                Username = "mary.s@cchdemo.com",
                Password = "password1",
                FullSsn = "999993333",
                SecretAnswer = "",
                Phone = "8162226688",
                ApiKey = "DB366C62-88B6-402D-BCB7-E3FC384776E1",
                AuthHash = "631B756ADC1A2347E1815DD719BB59E1A032C345AE8C6539C22C568CC5DCBBF0D55A0BF28C3A0AC638D7C399E469A0658CB2E028517F521B98DE3D6192BA5AB08D52BF36035C6FB812F53F89F1EB83BF5BEDC6688BEDACFAB657C15673335CA64FD7BF1C1334354B89E41E025E8479D0",
                EmployerId = 21,
                CchId = 63841,
                PaycheckDocumentId = "123",
                BenefitPlans = new int[] { 49 },
                MedicalPlanId = 45
            };

            return account;
        }
    }
    #endregion
    #region Caesars accounts
    public class CaesarsAccount : TestAccount {
        public Account MarySmith { get; private set; }

        public CaesarsAccount()
            : base() {
            InitAccounts();
        }

        private void InitAccounts() {
            MarySmith = GetMarySmith();

            Accounts.Add(MarySmith);
        }

        private Account GetMarySmith() {
            return new Account {
                Username = "mary.smith@cchcaesars.com",
                Password = "dem0-User",
                FullSsn = "000004835",
                SecretAnswer = "crazytown",
                Phone = "6176668888",
                ApiKey = "DB366C62-88B6-402D-BCB7-E3FC384776E1",
                AuthHash = "631B756ADC1A2347EF43A82F21E65B3A77C30C4141714F8C4E9968962D6E6538B43B8DC963E03A63B068FBC5CF91124D7F102F138D2D7C524ED6ABB76A5ACAE952D2F2EB93CE01112B63854F8735BE65EAE60B8CF777606612DC7ED073D48C2416FAC31B2C975BD3A1889BDB9613015E9BC782D715E73F63",
                EmployerId = 11,
                CchId = 57020,
            };
        }
    }
    #endregion
    #region Demo accounts
    public class TyLinAccount : TestAccount {
        //public List<Account> Accounts { get; private set; }
        public Account TomHo { get; private set; }

        public TyLinAccount()
            : base() {
            InitAccounts();
        }

        private void InitAccounts() {
            TomHo = GetTomHo();

            Accounts.Add(TomHo);
        }

        private Account GetTomHo() {
            var account = new Account {
                Username = "t.ho@mpm.com",
                Password = "password1",
                FullSsn = "413251738",
                SecretAnswer = "",
                Phone = "7025552222",
                ApiKey = "DB366C62-88B6-402D-BCB7-E3FC384776E1",
                AuthHash = "631B756ADC1A234733AC419CD0C988D00145205BB29480B036FCC47530AED5D555A3659F25866D3864F67C9516AC860E53561D00113C020D159259D01077D79C3B5427E9EBA93730C861021A39DD807A848F09EE00053664833F48B38BD5F9B8B651DCA974BDED37",
                EmployerId = 34,
                CchId = 6,
                PaycheckDocumentId = "60454",
                BenefitPlans = new int[] { 49 },
                MedicalPlanId = 49
            };

            return account;
        }
    }
    #endregion
}
