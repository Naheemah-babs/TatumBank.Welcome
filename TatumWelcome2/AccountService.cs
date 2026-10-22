namespace TatumWelcome2
{
    public static class AccountService
    {

        public static Account CreateAccount(
    List<Account> accounts,
    string name,
    string phone,
    string city,
    string pin,
    decimal initialDeposit)
        {
            Account account = new Account
            {
                Name = name,
                Phone = phone,
                City = city,
                Pin = pin,
                AccountNumber = GenerateAccountNumber(accounts)
            };

            if (initialDeposit > 0)
            {
                account.Deposit(initialDeposit, "Initial deposit");
            }

            accounts.Add(account);
            return account;
        }

        private static string GenerateAccountNumber(List<Account> accounts)
        {
            string number;

            do
            {
                number = $"00{Random.Shared.Next(100_000_000):D8}";
            }
            while (FindAccount(accounts, number) != null);

            return number;

        }
        public static Account? FindAccount(List<Account> accounts, string accountNumber)
        {
            foreach (Account account in accounts)
            {
                if (account.AccountNumber == accountNumber)
                {
                    return account;
                }
            }

            return null;
        }

        public static Account? Login(List<Account> accounts, string accountNumber, string pin)
        {
            Account? account = FindAccount(accounts, accountNumber);

            if (account == null)
            {
                return null;
            }

            if (account.Pin != pin)
            {
                return null;
            }

            return account;
        }

        public static TransactionResult CheckRecipient(List<Account> accounts, Account from, string accountNumber, out Account? recipient)
        {
            recipient = FindAccount(accounts, accountNumber);

            if (recipient == null)
            {
                return TransactionResult.RecipientNotFound;
            }

            if (recipient.AccountNumber == from.AccountNumber)
            {
                return TransactionResult.SameAccount;
            }

            return TransactionResult.Success;
        }

        public static TransactionResult Transfer(Account from, Account to, decimal amount)
        {
            if (from.AccountNumber == to.AccountNumber)
            {
                return TransactionResult.SameAccount;
            }

            TransactionResult outResult = from.TransferOut(amount, $"Transfer to {to.Name} ({to.AccountNumber})");

            if (outResult != TransactionResult.Success)
            {
                return outResult;
            }

            return to.TransferIn(amount, $"Transfer from {from.Name} ({from.AccountNumber})");
        }
    }
}