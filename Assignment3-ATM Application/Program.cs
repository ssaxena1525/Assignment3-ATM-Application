using System;
using System.Collections.Generic;

namespace ATMApplication
{
    public class BankAccount
    {
        // Properties of a bank account
        public int AccountNumber { get; private set; }
        public string AccountHolderName { get; private set; }
        public double Balance { get; private set; }
        public double InterestRate { get; private set; } = 0.03; 
        
        private string Password;

        // Transaction history
        private List<string> Transactions = new List<string>();

        // Constructor for account details
        public BankAccount(int accountNumber, string accountHolderName, double initialBalance, string password)
        {
            AccountNumber = accountNumber;
            AccountHolderName = accountHolderName;
            Balance = initialBalance;
            Password = password;
            Transactions.Add($"Account created with balance: {Balance:C}");
        }

        // Method to verify password
        public bool VerifyPassword(string inputPassword)
        {
            return Password == inputPassword;
        }

        // method to deposit
        public void Deposit(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }

            Balance += amount;
            Transactions.Add($"Deposited: {amount:C}. New balance: {Balance:C}");
        }

        // method for withdrawal 
        public void Withdraw(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }

            if (amount > Balance)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            Balance -= amount;
            Transactions.Add($"Withdrew: {amount:C}. New balance: {Balance:C}");
        }

        // method to display balance
        public double GetBalance()
        {
            return Balance;
        }

        // method to get transaction history
        public List<string> GetTransactions()
        {
            return new List<string>(Transactions); // Return a copy of the transaction list
        }

        // method to display account details
        public string GetAccountDetails()
        {
            return $"Account Holder: {AccountHolderName}\n" +
                   $"Account Number: {AccountNumber}\n" +
                   $"Interest Rate: {InterestRate:P}\n" +
                   $"Balance: {Balance:C}";
        }
    }

    public class ATM
    {
        private Dictionary<int, BankAccount> accounts = new Dictionary<int, BankAccount>();
        private int nextAccountNumber = 103; // Start for new account numbers

        // initail pre exesting accounts
        public ATM()
        {
            accounts[100] = new BankAccount(100, "Neel", 400.00, "neelpassword");
            accounts[101] = new BankAccount(101, "Gautam", 1200.00, "gautampassword");
            accounts[102] = new BankAccount(102, "Ruchi", 850.00, "ruchipassword");
        }

        // method to create a new account's I have provided the space for a total of 10 accounts out of which 3 are already present in the code
        public BankAccount CreateAccount(string accountHolderName, double initialBalance, string password)
        {
            if (accounts.Count >= 10)
                throw new InvalidOperationException("Maximum number of accounts reached.");

            int accountNumber = nextAccountNumber++;
            BankAccount newAccount = new BankAccount(accountNumber, accountHolderName, initialBalance, password);
            accounts[accountNumber] = newAccount;
            return newAccount;
        }

        // method to fetch an existing account
        public BankAccount GetAccount(int accountNumber, string password)
        {
            if (accounts.ContainsKey(accountNumber) && accounts[accountNumber].VerifyPassword(password))
            {
                return accounts[accountNumber];
            }

            throw new UnauthorizedAccessException("Invalid account number or password.");
        }

        // display available account spaces
        public int GetAvailableAccountSpaces()
        {
            return 10 - accounts.Count;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            ATM atm = new ATM();
            Console.WriteLine("Welcome to the ATM!");

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Create a new account");
                Console.WriteLine("2. Login to an existing account");
                Console.WriteLine("3. Exit");
                Console.Write("Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateNewAccount(atm);
                        break;
                    case "2":
                        LoginToAccount(atm);
                        break;
                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static void CreateNewAccount(ATM atm)
        {
            try
            {
                Console.WriteLine("Creating a new account...");
                Console.Write("Enter your name: ");
                string name = Console.ReadLine();

                Console.Write("Enter an initial deposit amount: ");
                if (!double.TryParse(Console.ReadLine(), out double initialBalance) || initialBalance < 0)
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }

                Console.Write("Set a password: ");
                string password = Console.ReadLine();

                BankAccount newAccount = atm.CreateAccount(name, initialBalance, password);
                Console.WriteLine("Account created successfully!");
                Console.WriteLine(newAccount.GetAccountDetails());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void LoginToAccount(ATM atm)
        {
            try
            {
                Console.WriteLine("Logging into an account...");
                Console.Write("Enter your account number: ");
                if (!int.TryParse(Console.ReadLine(), out int accountNumber))
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                Console.Write("Enter your password: ");
                string password = Console.ReadLine();

                BankAccount account = atm.GetAccount(accountNumber, password);

                Console.WriteLine("Login successful!");
                Console.WriteLine(account.GetAccountDetails());

                while (true)
                {
                    Console.WriteLine("Choose an option:");
                    Console.WriteLine("1. Deposit");
                    Console.WriteLine("2. Withdraw");
                    Console.WriteLine("3. Show Transaction Summary");
                    Console.WriteLine("4. Logout");
                    Console.Write("Your choice: ");
                    string action = Console.ReadLine();

                    switch (action)
                    {
                        case "1":
                            Console.Write("Enter amount to deposit: ");
                            if (double.TryParse(Console.ReadLine(), out double depositAmount))
                            {
                                account.Deposit(depositAmount);
                                Console.WriteLine($"Deposit successful. New balance: {account.GetBalance():C}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid amount.");
                            }
                            break;
                        case "2":
                            Console.Write("Enter amount to withdraw: ");
                            if (double.TryParse(Console.ReadLine(), out double withdrawAmount))
                            {
                                account.Withdraw(withdrawAmount);
                                Console.WriteLine($"Withdrawal successful. New balance: {account.GetBalance():C}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid amount.");
                            }
                            break;
                        case "3":
                            Console.WriteLine("Transaction Summary:");
                            foreach (string transaction in account.GetTransactions())
                            {
                                Console.WriteLine(transaction);
                            }
                            break;
                        case "4":
                            Console.WriteLine("Logging out...");
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
