using System;
using System.Collections.Generic;
using System.Text;

namespace TatumWelcome2
{
    public static class AccountHistory
    {
        public static void PrintHistory(Account account)
        {
            if (account.Transactions.Count == 0)
            {
                Console.WriteLine("No transactions yet.");
                return;
            }
            Console.WriteLine($"{"Date",-18}{"Type",-16}{"Amount",14}  Description");
            Console.WriteLine(new string('-', 64));
            foreach (Transaction t in account.Transactions)
            {
                string amountText = "₦" + t.Amount.ToString("N2");
                Console.WriteLine($"{t.Date,-18:dd MMM yyyy HH:mm}{t.Type,-16}{amountText,14}  {t.Description}");
            }
            Console.WriteLine(new string('-', 64));
            string balanceText = "₦" + account.Balance.ToString("N2");
            Console.WriteLine($"{"Balance",-34}{balanceText,14}");
        }
    }
    
}
