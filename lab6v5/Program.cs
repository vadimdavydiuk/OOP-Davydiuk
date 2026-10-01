using System;

namespace Lab6V5
{
    // Базовий клас BankAccount
    public class BankAccount
    {
        private string _accountNumber;
        private decimal _balance;

        public string AccountNumber
        {
            get => _accountNumber;
            set => _accountNumber = value;
        }

        public decimal Balance
        {
            get => _balance;
            protected set => _balance = value;
        }

        public BankAccount(string accountNumber, decimal initialBalance)
        {
            _accountNumber = accountNumber;
            _balance = initialBalance;
        }

        // Віртуальний метод для перевизначення
        public virtual void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"[BankAccount] Поповнено рахунок {AccountNumber} на {amount:C}. Поточний баланс: {Balance:C}");
        }

        // Невіртуальний метод для демонстрації приховування (new)
        public string GetAccountType()
        {
            return "Базовий банківський рахунок";
        }
    }

    // Похідний клас SavingsAccount
    public class SavingsAccount : BankAccount
    {
        public decimal InterestRate { get; set; }

        public SavingsAccount(string accountNumber, decimal initialBalance, decimal interestRate)
            : base(accountNumber, initialBalance)
        {
            InterestRate = interestRate;
        }

        // Перевизначення віртуального методу (override)
        public override void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"[SavingsAccount] Поповнено ощадний рахунок {AccountNumber} на {amount:C}. Баланс: {Balance:C}");
        }

        // Власний унікальний метод
        public void AddInterest()
        {
            decimal interest = Balance * (InterestRate / 100);
            Balance += interest;
            Console.WriteLine($"[SavingsAccount] Нараховано відсотки ({InterestRate}%): +{interest:C}. Новий баланс: {Balance:C}");
        }

        // Демонстрація приховування методу (new)
        public new string GetAccountType()
        {
            return "Ощадний рахунок (Savings Account)";
        }
    }

    // Похідний клас CheckingAccount
    public class CheckingAccount : BankAccount
    {
        public decimal OverdraftLimit { get; set; }

        public CheckingAccount(string accountNumber, decimal initialBalance, decimal overdraftLimit)
            : base(accountNumber, initialBalance)
        {
            OverdraftLimit = overdraftLimit;
        }

        // Перевизначення віртуального методу (override)
        public override void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"[CheckingAccount] Поповнено розрахунковий рахунок {AccountNumber} на {amount:C}. Баланс: {Balance:C} (Овердрафт: {OverdraftLimit:C})");
        }

        // Власний унікальний метод
        public void ProcessCheck()
        {
            Console.WriteLine($"[CheckingAccount] Оброблено чек для рахунку {AccountNumber}.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створення об'єктів
            BankAccount baseAcc = new BankAccount("UA001", 1000m);
            SavingsAccount savingsAcc = new SavingsAccount("UA002", 2000m, 5.0m);
            CheckingAccount checkingAcc = new CheckingAccount("UA003", 1500m, 500m);

            Console.WriteLine("--- Поліморфні виклики (override) ---");
            // Масив базового типу з об'єктами похідних класів
            BankAccount[] accounts = new BankAccount[] { baseAcc, savingsAcc, checkingAcc };

            foreach (BankAccount acc in accounts)
            {
                // Завдяки override викликається відповідна реалізація кожного класу
                acc.Deposit(500m);
            }

            Console.WriteLine("\n--- Унікальні методи похідних класів ---");
            savingsAcc.AddInterest();
            checkingAcc.ProcessCheck();

            Console.WriteLine("\n--- Демонстрація різниці між override та new ---");
            // Виклики через посилання похідного типу
            Console.WriteLine($"savingsAcc.GetAccountType(): {savingsAcc.GetAccountType()}");

            // Приведення до базового типу (Upcasting)
            BankAccount baseRef = savingsAcc;

            // Демонстрація: приховування new залежить від типу посилання, а не від типу об'єкта
            Console.WriteLine($"baseRef.GetAccountType():    {baseRef.GetAccountType()}");
        }
    }
}