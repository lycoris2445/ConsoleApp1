﻿using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.IO;
using System.Transactions;
using static FinancialManager.Program;
using static FinancialManager.Program.GamificationManager;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using static ConsoleApp1.Test;
namespace FinancialManager
{
    class Program
    {
        private static List<Transaction> transactions = new List<Transaction>();
        private static List<Transaction> Transactions = new List<Transaction>();
        static GamificationManager gamificationManager = new GamificationManager();
        private static ChallengeManager challengeManager = new ChallengeManager();
        private static string transactionFilePath = @"F:\ConsoleApp1";
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Financial Manager =====");
                Console.WriteLine("1. Home");
                Console.WriteLine("2. Transaction");
                Console.WriteLine("3. Add Record");
                Console.WriteLine("4. Budget");
                Console.WriteLine("5. Saving");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Home();
                        break;
                    case "2":
                        Transactions.Clear();
                        LoadTransactionsFromFile("Spending.csv");
                        LoadTransactionsFromFile("Income.csv");
                        LoadTransactionsFromFile("Loan.csv");
                        LoadTransactionsFromFile("Debit.csv");
                        ShowTransaction();
                        break;
                    case "3":
                        AddRecord();
                        break;
                    case "4":
                        Budget();
                        break;
                    case "5":
                        Saving();
                        break;
                    case "6":
                        // Exit warning
                        Console.Write("Are you sure you want to exit? (y/n): ");
                        string exitConfirmation = Console.ReadLine().ToLower();
                        if (exitConfirmation == "y")
                        {
                            Console.WriteLine("Bye, bye (╥_╥)");
                            return;  // Exit the application
                        }
                        else
                        {
                            Console.WriteLine("Returning to main menu...");
                            Console.ReadKey();  // Wait for user to press any key before continuing
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddRecord()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== Add Record ===");
            Console.ResetColor();

            Console.WriteLine("Select the type of record to add:");
            Console.WriteLine("[1] Spending");
            Console.WriteLine("[2] Income");
            Console.WriteLine("[3] Loan");
            Console.WriteLine("[4] Debit");

            string choice = Console.ReadLine();
            // Declare the variables outside of the switch statement
            decimal amount = 0;
            string category = string.Empty;
            string description = string.Empty;
            string borrower = string.Empty;
            string lender = string.Empty;
            DateTime date = DateTime.Now;
            switch (choice)
            {
                case "1":
                    AddSpendingRecord(out amount, out category, out description, out date);
                    string spendingFilePath = @"F:\ConsoleApp1\Spending.csv";
                    string SpendingFileName = "Spending.csv";
                    string SpendingFilePath = Path.Combine(spendingFilePath, SpendingFileName);
                    WriteSpendingToCsv(spendingFilePath, amount, category, description, date);
                    Console.ReadKey();
                    break;
                case "2":
                    AddIncomeRecord(out amount, out category, out description, out date);
                    string incomeFilePath = @"F:\ConsoleApp1\Income.csv";
                    string IncomeFileName = "Income.csv";
                    string IncomeFilePath = Path.Combine(incomeFilePath, IncomeFileName);
                    WriteIncomeToCsv(incomeFilePath, amount, category, description, date);
                    Console.ReadKey();
                    break;
                case "3":
                    AddLoanRecord(out amount, out borrower, out description, out date);
                    string loanFilePath = @"F:\ConsoleApp1\Loan.csv";
                    string LoanFileName = "Loan.csv";
                    string LoanFilePath = Path.Combine(loanFilePath, LoanFileName);
                    WriteLoanToCsv(loanFilePath, amount, borrower, description, date);
                    Console.ReadKey();
                    break;
                case "4":
                    AddDebitRecord(out amount, out lender, out description, out date);
                    string debitFilePath = @"F:\ConsoleApp1\Debit.csv";
                    string DebitFileName = "Debit.csv";
                    string DebitFilePath = Path.Combine(debitFilePath, DebitFileName);
                    WriteDebitToCsv(debitFilePath, amount, lender, description, date);
                    Console.ReadKey();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Returning to the main menu.");
                    Console.ResetColor();
                    break;
            }
        }

        static void StoreTransaction(decimal amount, string category, string description, DateTime date, string type)
        {
            // Add a new transaction to the static list
            Transactions.Add(new Transaction(amount, category, description, date, type));
            SaveTransactionsToFile();
        }

        static void AddSpendingRecord(out decimal amount, out string category, out string description, out DateTime date)
        {
            Console.Clear();
            Console.WriteLine("=== Add Spending Record ===");

            Console.Write("Enter amount (e.g., 30k, 3m, 3b, 30.000): ");
            amount = ReadDecimalInput();

            Console.Write("Enter category (e.g., Food, Shopping, Housing): ");
            category = Console.ReadLine();

            Console.Write("Enter description: ");
            description = Console.ReadLine();

            date = GetDateInput("Enter date (leave blank for today): ");
            
            // Store the spending record
            StoreTransaction(amount, category, description, date, "Spending");

            Console.WriteLine($"Spending Record Added: {FormatCurrency(amount)} VND, On {category}, For {description}, On {date.ToShortDateString()}");
            // TODO: Store this record in a collection or file
        }
        static void WriteSpendingToCsv(string SpendingFilePath, decimal amount, string category, string description, DateTime date)
        {

            try
            {
                if (!File.Exists(SpendingFilePath))
                {
                    using (File.Create(SpendingFilePath)) { }
                }


                List<Spending> spendings = new List<Spending>
                {
                    // Thêm thông tin vào danh sách chi tiêu
                    new Spending
                    {
                        Amount = amount,
                        Category = category,
                        Description = description,
                        Date = date
                    }
                };


                var configSpendings = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                using (StreamWriter streamWriter = new StreamWriter(SpendingFilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configSpendings))
                {
                    csvWriter.WriteRecords(spendings);
                }

                Console.WriteLine("Data written to CSV successfully.");
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public class Spending
        {
            public decimal Amount { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }

        static void AddIncomeRecord(out decimal amount, out string category, out string description, out DateTime date)
        {
            Console.Clear();
            Console.WriteLine("=== Add Income Record ===");

            Console.Write("Enter amount (e.g., 30k, 3m, 3B, 30.000): ");
            amount = ReadDecimalInput();

            Console.Write("Enter category (e.g., Main Job, Part-Time Job, Savings): ");
            category = Console.ReadLine();

            Console.Write("Enter description: ");
            description = Console.ReadLine();

            date = GetDateInput("Enter date (leave blank for today): ");

            // Store the income record
            StoreTransaction(amount, category, description, date, "Income");

            Console.WriteLine($"Income Record Added: {FormatCurrency(amount)} VND, From {category}, For {description}, On {date.ToShortDateString()}");
            // TODO: Store this record in a collection or file
        }
        static void WriteIncomeToCsv(string IncomeFilePath, decimal amount, string category, string description, DateTime date)
        {

            try
            {
                if (!File.Exists(IncomeFilePath))
                {
                    using (File.Create(IncomeFilePath)) { }
                }


                List<Income> incomes = new List<Income>
                {
                    // Thêm thông tin vào danh sách chi tiêu
                    new Income
                    {
                        Amount = amount,
                        Category = category,
                        Description = description,
                        Date = date
                    }
                };


                var configIncomes = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                using (StreamWriter streamWriter = new StreamWriter(IncomeFilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configIncomes))
                {
                    csvWriter.WriteRecords(incomes);
                }

                Console.WriteLine("Data written to CSV successfully.");
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public class Income
        {
            public decimal Amount { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }
        static void AddLoanRecord(out decimal amount, out string borrower, out string description, out DateTime date)
        {
            Console.Clear();
            Console.WriteLine("=== Add Loan Record ===");

            Console.Write("Enter amount (e.g., 30k, 3m, 3B, 30.000): ");
            amount = ReadDecimalInput();

            Console.Write("Enter who you lend to: ");
            borrower = Console.ReadLine();

            Console.Write("Enter description (e.g: what they borrow for): ");
            description = Console.ReadLine();

            date = GetDateInput("Enter date (leave blank for today): ");

            //Store the loan record
            Program.StoreTransaction(amount, borrower, description, date, "Loan");

            Console.WriteLine($"Loan Record Added: {FormatCurrency(amount)} VND, To {borrower}, For {description}, On {date.ToShortDateString()}");
            // TODO: Store this record in a collection or file
        }
        static void WriteLoanToCsv(string LoanFilePath, decimal amount, string borrower, string description, DateTime date)
        {

            try
            {
                if (!File.Exists(LoanFilePath))
                {
                    using (File.Create(LoanFilePath)) { }
                }


                List<Loan> loans = new List<Loan>
                {
                    // Thêm thông tin vào danh sách chi tiêu
                    new Loan
                    {
                        Amount = amount,
                        borrower = borrower,
                        Description = description,
                        Date = date
                    }
                };


                var configIncomes = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                using (StreamWriter streamWriter = new StreamWriter(LoanFilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configIncomes))
                {
                    csvWriter.WriteRecords(loans);
                }

                Console.WriteLine("Data written to CSV successfully.");
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public class Loan
        {
            public decimal Amount { get; set; }
            public string borrower { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }
        static void AddDebitRecord(out decimal amount, out string lender, out string description, out DateTime date)
        {
            Console.Clear();
            Console.WriteLine("=== Add Debit Record ===");

            Console.Write("Enter amount (e.g., 30k, 3m, 3B, 30.000): ");
            amount = ReadDecimalInput();

            Console.Write("Enter who you take loan from (e.g: bank's name, a person's name): ");
            lender = Console.ReadLine();

            Console.Write("Enter description (e.g: What you used it for?): ");
            description = Console.ReadLine();

            date = GetDateInput("Enter date (leave blank for today): ");

            // Store the debit record
            StoreTransaction(amount, lender, description, date, "Debit");

            Console.WriteLine($"Debit Record Added: {FormatCurrency(amount)} VND, From {lender}, To {description}, On {date.ToShortDateString()}");
            // TODO: Store this record in a collection or file
        }
        static void WriteDebitToCsv(string DebitFilePath, decimal amount, string lender, string description, DateTime date)
        {

            try
            {
                if (!File.Exists(DebitFilePath))
                {
                    using (File.Create(DebitFilePath)) { }
                }


                List<Debit> debits = new List<Debit>
                {
                    // Thêm thông tin vào danh sách chi tiêu
                    new Debit
                    {
                        Amount = amount,
                        lender = lender,
                        Description = description,
                        Date = date
                    }
                };


                var configIncomes = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                using (StreamWriter streamWriter = new StreamWriter(DebitFilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configIncomes))
                {
                    csvWriter.WriteRecords(debits);
                }

                Console.WriteLine("Data written to CSV successfully.");
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public class Debit
        {
            public decimal Amount { get; set; }
            public string lender { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }

        static decimal ReadDecimalInput()
        {
            while (true)
            {
                string input = Console.ReadLine().Trim().ToLower();
                try
                {
                    decimal multiplier = 1;
                    if (input.EndsWith("k")) // Handle 'k' for thousands
                    {
                        input = input.Substring(0, input.Length - 1);
                        multiplier = 1000;
                    }
                    else if (input.EndsWith("m")) // Handle 'm' for millions
                    {
                        input = input.Substring(0, input.Length - 1);
                        multiplier = 1_000_000;
                    }
                    else if (input.EndsWith("b")) // Handle 'b' for billions
                    {
                        input = input.Substring(0, input.Length - 1);
                        multiplier = 1_000_000_000;
                    }

                    // Remove non-numeric characters (e.g., "vnd", ",", ".")
                    string sanitizedInput = input.Replace(".", "").Replace(",", "").Replace("vnd", "").Trim();

                    if (decimal.TryParse(sanitizedInput, out decimal amount))
                    {
                        return amount * multiplier;
                    }

                    throw new FormatException("Invalid numeric format.");
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid amount format. Please try again (e.g., 30k, 3m, 3b, 30.000).");
                    Console.ResetColor();
                }
            }
        }

        static string FormatCurrency(decimal amount)
        {
            // Format as Vietnamese currency with dot separator and add ₫ symbol
            return $"{amount:N0}".Replace(",", ".");
        }

        static DateTime GetDateInput(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return DateTime.Now;
            }

            if (DateTime.TryParse(input, out DateTime date))
            {
                return date;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid date. Using today's date instead.");
            Console.ResetColor();
            return DateTime.Now;
        }

        
        static void Budget()
        {
            Console.Clear();
            Console.WriteLine("=== Budget ===");
            Console.WriteLine("Feature not implemented yet.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        static void Saving()
        {
            Console.Clear();
            Console.WriteLine("=== Saving ===");
            Console.WriteLine("Feature not implemented yet.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        static void SaveToFile(string transaction)
        {
            try
            {
                File.AppendAllText("transactions.txt", transaction + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transaction: {ex.Message}");
            }
        }

        static void Home()
        {
            Console.Clear();
            Console.WriteLine("    /\\_/\\  ");
            Console.WriteLine("   ( o.o ) ");
            Console.WriteLine("    > ^ <  ");
            Console.WriteLine("~Meow, meow~");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Financial system =====");
                Console.WriteLine("1. Transaction record");
                Console.WriteLine("2. Check level up and unlocked badges");
                Console.WriteLine("3. Create challenge");
                Console.WriteLine("4. Join challenge");
                Console.WriteLine("5. Compare progress");
                Console.WriteLine("6. Main menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Ghi chép giao dịch và cộng điểm XP
                        gamificationManager.EarnXP("Transaction record", 100);
                        gamificationManager.CheckLevelUp();
                        break;
                    case "2":
                        // Hiển thị cấp độ và huy chương của người dùng
                        gamificationManager.ShowUserInfo();
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case "3":
                        // Tạo thử thách tài chính
                        Console.Write("Enter challenge name: ");
                        string challengeName = Console.ReadLine();
                        Console.Write("Enter goal (amount): ");
                        if (int.TryParse(Console.ReadLine(), out int targetAmount))
                        {
                            challengeManager.CreateChallenge(challengeName, targetAmount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount. Press any key to retry...");
                            Console.ReadKey();
                        }
                        break;
                    case "4":
                        // Tham gia thử thách tài chính
                        Console.Write("Enter your challenge name: ");
                        string challengeToJoin = Console.ReadLine();
                        challengeManager.JoinChallenge(challengeToJoin);
                        break;
                    case "5":
                        // So sánh tiến độ giữa 2 người dùng
                        challengeManager.ViewAllChallenges();
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case "6":
                        // Thoát chương trình
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to retry...");
                        Console.ReadKey();
                        break;
                }
            }
        }
        public class GamificationManager
        {
            private int userXP = 0;
            private int userLevel = 1;
            private List<string> badges = new List<string>();

            // Earn XP for completing tasks
            public void EarnXP(string task, int xp)
            {
                Console.WriteLine($"Mission complete: {task}. Earned {xp} XP.");
                userXP += xp;
            }

            // Check and level up the user
            public void CheckLevelUp()
            {
                int requiredXPForLevelUp = userLevel * 500; // 500 XP per level
                if (userXP >= requiredXPForLevelUp)
                {
                    userLevel++;
                    Console.WriteLine($"Congratulations! You reached level {userLevel}.");
                    UnlockBadge($"Level {userLevel} Badge");
                }
                else
                {
                    Console.WriteLine($"Current XP: {userXP}. You need {requiredXPForLevelUp - userXP} XP to level up.");
                }
            }

            // Unlock badges when goals are achieved
            public void UnlockBadge(string badge)
            {
                badges.Add(badge);
                Console.WriteLine($"Badge unlocked: {badge}");
            }

            // Show user information (level, XP, badges)
            public void ShowUserInfo()
            {
                Console.WriteLine($"Level: {userLevel}, XP: {userXP}");
                Console.WriteLine("Unlocked Badges:");
                foreach (var badge in badges)
                {
                    Console.WriteLine($"- {badge}");
                }
            }
        }

        public class ChallengeManager
        {
            private List<FinancialChallenge> challenges = new List<FinancialChallenge>();

            // Create a financial challenge
            public void CreateChallenge(string challengeName, int targetAmount)
            {
                var challenge = new FinancialChallenge(challengeName, targetAmount);
                challenges.Add(challenge);
                Console.WriteLine($"Challenge '{challengeName}' has been created with a target of {targetAmount}!");
            }

            // Join a financial challenge
            public void JoinChallenge(string challengeName)
            {
                var challenge = challenges.Find(ch => ch.Name == challengeName);
                if (challenge != null)
                {
                    challenge.Join();
                    Console.WriteLine($"You have joined the challenge: {challengeName}");
                }
                else
                {
                    Console.WriteLine($"Challenge '{challengeName}' not found.");
                }
            }

            // View all challenges
            public void ViewAllChallenges()
            {
                Console.WriteLine("Active Challenges:");
                foreach (var challenge in challenges)
                {
                    challenge.DisplayChallengeStatus();
                }
            }
        }

        public class FinancialChallenge
        {
            public string Name { get; }
            public int TargetAmount { get; }
            private bool isJoined = false;

            public FinancialChallenge(string name, int targetAmount)
            {
                Name = name;
                TargetAmount = targetAmount;
            }

            public void Join()
            {
                isJoined = true;
            }

            public void DisplayChallengeStatus()
            {
                Console.WriteLine($"Challenge: {Name}, Target: {TargetAmount}, Joined: {isJoined}");
            }
        }

        static void LoadTransactionsFromFile(string type)
        {
            string filepath = transactionFilePath + type;
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Transaction file not found.");
                return;
            }

            string[] rows = File.ReadAllLines(filepath);


            for (int i = 1; i < rows.Length; i++) // Assuming first row is header
            {
                string[] columns = rows[i].Split(',');

                if (columns.Length < 5)
                {
                    Console.WriteLine($"Invalid data on row {i + 1}. Skipping...");
                    continue;
                }

                try
                {
                    string types = columns[0].Trim();
                    decimal amount = decimal.Parse(columns[1].Trim());
                    string category = columns[2].Trim();
                    string description = columns[3].Trim();
                    DateTime date = DateTime.Parse(columns[4].Trim());

                    Transactions.Add(new Transaction(amount, category, description, date, types));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing row {i + 1}: {ex.Message}. Skipping...");
                }
            }
        }

        static void SaveTransactionsToFile()
        {
            if (Transactions == null || Transactions.Count == 0)
            {
                Console.WriteLine("No transactions to save.");
                return;
            }

            List<Transaction> spendingTrans = new List<Transaction>();
            List<Transaction> incomeTrans = new List<Transaction>();
            List<Transaction> loanTrans = new List<Transaction>();
            List<Transaction> debitTrans = new List<Transaction>();
            // Add each transaction as a row
            foreach (var transaction in Transactions)
            {
                switch (transaction.Type)
                {
                    case "Spending":
                        spendingTrans.Add(transaction);
                        break;
                    case "Income":
                        incomeTrans.Add(transaction);
                        break;
                    case "Loan":
                        loanTrans.Add(transaction);
                        break;
                    case "Debit":
                        debitTrans.Add(transaction);
                        break;

                }
            }
            SaveFileByType("Spending", spendingTrans);
            SaveFileByType("Income", incomeTrans);
            SaveFileByType("Loan", loanTrans);
            SaveFileByType("Debit", debitTrans);
        }

        static void SaveFileByType(string type, List<Transaction> transactions)
        {
            string fileName = type + ".csv";


            string filePath = transactionFilePath + fileName;
            List<string> rows = new List<string>();

            // Add header row
            rows.Add("Type,Amount,Category,Description,Date");

            // Add each transaction as a row
            foreach (var transaction in transactions)
            {
                rows.Add($"{transaction.Type},{transaction.Amount},{transaction.Category},{transaction.Description},{transaction.Date:yyyy-MM-dd}");
            }

            // Write to file, overwriting any existing content
            File.WriteAllLines(filePath, rows);

            Console.WriteLine(type + " transactions saved to file successfully.");

        }

        static void ShowTransaction()
        {
            Console.Clear();
            Console.WriteLine("=== View Transactions ===");
            Console.WriteLine("Select filter by time:");
            Console.WriteLine("[1] This Week");
            Console.WriteLine("[2] This Month");
            Console.WriteLine("[3] This Year");
            Console.WriteLine("[4] Custom Date Range");
            Console.WriteLine("[5] Show all");
            string filterChoice = Console.ReadLine();

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            switch (filterChoice)
            {
                case "1":
                    startDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                    endDate = DateTime.Now;
                    break;
                case "2":
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = DateTime.Now;
                    break;
                case "3":
                    startDate = new DateTime(DateTime.Now.Year, 1, 1);
                    endDate = DateTime.Now;
                    break;
                case "4":
                    Console.Write("Enter start date (yyyy-mm-dd): ");
                    while (!DateTime.TryParse(Console.ReadLine(), out startDate))
                    {
                        Console.Write("Invalid start date format. Please enter in yyyy-mm-dd format: ");
                    }

                    Console.Write("Enter end date (yyyy-mm-dd): ");
                    while (!DateTime.TryParse(Console.ReadLine(), out endDate))
                    {
                        Console.Write("Invalid end date format. Please enter in yyyy-mm-dd format: ");
                    }
                    break;
                case "5":
                    // No filtering needed
                    break;
                default:
                    Console.WriteLine("Invalid choice. Showing all records.");
                    break;
            }

            // Filter the transactions by date range
            var filteredTransactions = Transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();

            Console.WriteLine("Select sort order:");
            Console.WriteLine("[1] Sort by Amount (High to Low)");
            Console.WriteLine("[2] Sort by Amount (Low to High)");
            Console.WriteLine("[3] Sort by Date (Newest first)");
            Console.WriteLine("[4] Sort by Date (Oldest first)");
            string sortChoice = Console.ReadLine();

            switch (sortChoice)
            {
                case "1":
                    filteredTransactions = filteredTransactions.OrderByDescending(t => t.Amount).ToList();
                    break;
                case "2":
                    filteredTransactions = filteredTransactions.OrderBy(t => t.Amount).ToList();
                    break;
                case "3":
                    filteredTransactions = filteredTransactions.OrderByDescending(t => t.Date).ToList();
                    break;
                case "4":
                    filteredTransactions = filteredTransactions.OrderBy(t => t.Date).ToList();
                    break;
                default:
                    Console.WriteLine("Invalid sort choice. Showing unsorted transactions.");
                    break;
            }

            Console.WriteLine("\nFiltered and Sorted Transactions:");
            foreach (var transaction in filteredTransactions)
            {
                Console.WriteLine(transaction);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }

    public class Transaction
    {
        public string Type { get; set; } // "Income" or "Expense"
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public Transaction(decimal amount, string category, string description, DateTime date, string type)
        {
            Amount = amount;
            Category = category;
            Description = description;
            Date = date;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} - {Type} - {Amount:C} - {Category} - {Description}";
        }
    }
}
