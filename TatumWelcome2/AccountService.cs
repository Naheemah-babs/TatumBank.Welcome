using System;
using System.Collections.Generic;
using System.Text;

namespace TatumWelcome2
{
    public class AccountService
    {
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

        public static TransferResult Transfer(Account from, Account to, decimal amount)
        {
            if (from.AccountNumber == to.AccountNumber)
            {
                return TransferResult.SameAccount;
            }

            WithdrawResult outResult = from.TransferOut(amount, $"Transfer to {to.Name} ({to.AccountNumber})");

            if (outResult == WithdrawResult.InvalidAmount)
            {
                return TransferResult.InvalidAmount;
            }

            if (outResult == WithdrawResult.InsufficientFunds)
            {
                return TransferResult.InsufficientFunds;
            }

            to.TransferIn(amount, $"Transfer from {from.Name} ({from.AccountNumber})");

            return TransferResult.Success;
        }
    }

}
