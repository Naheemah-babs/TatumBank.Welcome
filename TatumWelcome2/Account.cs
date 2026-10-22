namespace TatumWelcome2
{
    public class Account
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; } = new();

        public TransactionResult Deposit(decimal amount, string description)
        {
            return Credit(amount, description, TransactionType.Deposit);
        }

        public TransactionResult TransferIn(decimal amount, string description)
        {
            return Credit(amount, description, TransactionType.TransferIn);
        }

        public TransactionResult CheckWithdraw(decimal amount)
        {
            if (amount <= 0)
            {
                return TransactionResult.InvalidAmount;
            }

            if (amount > Balance)
            {
                return TransactionResult.InsufficientFunds;
            }

            return TransactionResult.Success;
        }

        public TransactionResult Withdraw(decimal amount, string description, TransactionType type = TransactionType.Withdrawal)
        {
            return Debit(amount, description, type);
        }

        public TransactionResult TransferOut(decimal amount, string description)
        {
            return Debit(amount, description, TransactionType.TransferOut);
        }

        private TransactionResult Credit(decimal amount, string description, TransactionType type)
        {
            if (amount <= 0)
            {
                return TransactionResult.InvalidAmount;
            }

            Balance += amount;

            Transactions.Add(new Transaction
            {
                Type = type,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            });

            return TransactionResult.Success;
        }

        private TransactionResult Debit(decimal amount, string description, TransactionType type)
        {
            TransactionResult check = CheckWithdraw(amount);

            if (check != TransactionResult.Success)
            {
                return check;
            }

            Balance -= amount;

            Transactions.Add(new Transaction
            {
                Type = type,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            });

            return TransactionResult.Success;
        }
    }
}