using System.Security.Principal;

namespace TatumWelcome2
{
    public class Account
    {
        public string Name { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; } = new();

        public bool Deposit(decimal amount, string description)
        {
            if (amount <= 0)
            {
                return false;
            }
            Balance += amount;

            Transaction userTransaction = new Transaction
            {
                Type = TransactionType.Deposit,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            };

            Transactions.Add(userTransaction);

            return true;
        }

        public WithdrawResult CheckWithdraw(decimal amount)
        {
            if (amount <= 0)
            {
                return WithdrawResult.InvalidAmount;
            }

            if (amount > Balance)
            {
                return WithdrawResult.InsufficientFunds;
            }

            return WithdrawResult.Success;
        }

        public WithdrawResult Withdraw(decimal amount, string description)
        {
            WithdrawResult check = CheckWithdraw(amount);

            if (check != WithdrawResult.Success)
            {
                return check;
            }

            Balance -= amount;

            Transaction userTransaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            };

            Transactions.Add(userTransaction);

            return WithdrawResult.Success;
        }

        public WithdrawResult TransferOut(decimal amount, string description)
        {
            WithdrawResult check = CheckWithdraw(amount);

            if (check != WithdrawResult.Success)
            {
                return check;
            }

            Balance -= amount;

            Transaction userTransaction = new Transaction
            {
                Type = TransactionType.TransferOut,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            };

            Transactions.Add(userTransaction);

            return WithdrawResult.Success;
        }

        public bool TransferIn(decimal amount, string description)
        {
            if (amount <= 0)
            {
                return false;
            }

            Balance += amount;

            Transaction userTransaction = new Transaction
            {
                Type = TransactionType.TransferIn,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            };

            Transactions.Add(userTransaction);

            return true;
        }
    }
    
}
