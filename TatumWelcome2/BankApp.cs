namespace TatumWelcome2;

public static class BankApp
{
    public static void Run()
    {
        List<Account> accounts = AccountData.GetAccounts();

        bool appRunning = true;

        while (appRunning)
        {
            AsciiArt.ShowWelcome();

            Account? current = null;
            int attempts = 0;

            while (current == null && attempts < 3)
            {
                Console.Write("Account number: ");
                string number = Console.ReadLine() ?? "";

                Console.Write("PIN: ");
                string pin = Console.ReadLine() ?? "";

                current = AccountService.Login(accounts, number, pin);

                if (current == null)
                {
                    attempts++;
                    Console.WriteLine("Invalid account number or PIN.");
                }
            }

            if (!appRunning)
            {
                Console.WriteLine("Application closed.");
                break;
            }

            if (current == null)
            {
                Console.WriteLine("Too many failed attempts. Returning to login...");
                continue;
            }

            Console.WriteLine($"Welcome, {current.Name}.");
            Console.WriteLine("(Enter 0 at any step of an operation to cancel)");

            bool loggedIn = true;

            while (loggedIn)
            {
                bool operationCompleted = false;

                Console.WriteLine("\n1. Deposit  2. Withdraw  3. Transfer  4. Airtime  5. Data  6. History  7. Balance  8. Logout");
                Console.Write("Choose: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        {
                            decimal? depositAmount = HelperMethods.ReadAmount("Deposit amount: ");
                            if (depositAmount == null) break;

                            current.Deposit(depositAmount.Value, "deposit");
                            Console.WriteLine($"Deposited. Balance: {current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "2":
                        {
                            decimal? withdrawAmount = HelperMethods.ReadAmount("Withdraw amount: ", current);
                            if (withdrawAmount == null) break;

                            if (!HelperMethods.ConfirmPin(current, $"Enter your PIN to confirm withdrawal of {withdrawAmount.Value:N2}: ")) break;

                            current.Withdraw(withdrawAmount.Value, "withdrawal");
                            Console.WriteLine($"Withdrawn. Balance: {current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "3":
                        {
                            Account? recipient = null;
                            bool recipientValid = false;
                            while (!recipientValid)
                            {
                                Console.Write("Recipient account number: ");
                                string recipientInput = (Console.ReadLine() ?? "").Trim();
                                if (HelperMethods.IsCancel(recipientInput)) break;

                                recipient = AccountService.FindAccount(accounts, recipientInput);

                                if (recipient == null)
                                {
                                    Console.WriteLine("Account not found.");
                                    continue;
                                }

                                if (recipient.AccountNumber == current.AccountNumber)
                                {
                                    Console.WriteLine("You can't transfer to your own account.");
                                    continue;
                                }

                                recipientValid = true;
                            }
                            if (!recipientValid || recipient == null) break;

                            Console.WriteLine($"Recipient: {recipient.Name}");

                            decimal? transferAmount = HelperMethods.ReadAmount("Transfer amount: ", current);
                            if (transferAmount == null) break;

                            Console.WriteLine($"You are sending {transferAmount.Value:N2} to {recipient.Name} ({recipient.AccountNumber}).");

                            if (!HelperMethods.ConfirmPin(current, "Enter your PIN to confirm: ")) break;

                            AccountService.Transfer(current, recipient, transferAmount.Value);
                            Console.WriteLine($"{transferAmount.Value:N2} sent to {recipient.Name}. Balance: {current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "4":
                        {
                            string? airtimeNetwork = HelperMethods.ReadNetwork();
                            if (airtimeNetwork == null) break;

                            string? airtimePhone = HelperMethods.ReadPhone();
                            if (airtimePhone == null) break;

                            decimal? airtimeAmount = HelperMethods.ReadAmount("Airtime amount, minimum 50: ", current, 50m);
                            if (airtimeAmount == null) break;

                            Console.WriteLine($"Sending {airtimeAmount.Value:N2} {airtimeNetwork} airtime to {airtimePhone}.");

                            current.Withdraw(airtimeAmount.Value, $"Airtime {airtimeNetwork} {airtimePhone}");
                            Console.WriteLine($"{airtimeAmount.Value:N2} airtime sent to {airtimePhone}. Balance: {current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "5":
                        {
                            string? dataNetwork = HelperMethods.ReadNetwork();
                            if (dataNetwork == null) break;

                            DataPlan? selectedPlan = HelperMethods.ReadPlan();
                            if (selectedPlan == null) break;

                            string? dataPhone = HelperMethods.ReadPhone();
                            if (dataPhone == null) break;

                            Console.WriteLine($"Sending {selectedPlan.Name} ({dataNetwork}) to {dataPhone}.");

                            WithdrawResult dataResult = current.Withdraw(selectedPlan.Price, $"Data {dataNetwork} {selectedPlan.Name} {dataPhone}");
                            if (dataResult == WithdrawResult.Success)
                            {
                                Console.WriteLine($"{selectedPlan.Name} sent to {dataPhone}. Balance: {current.Balance:N2}");
                                operationCompleted = true;
                            }
                            else
                            {
                                Console.WriteLine($"You don't have enough balance for this plan. Your balance is {current.Balance:N2}.");
                            }
                            break;
                        }

                    case "6":
                        AccountHistory.PrintHistory(current);
                        operationCompleted = true;
                        break;

                    case "7":
                        Console.WriteLine($"Balance: {current.Balance:N2}");
                        operationCompleted = true;
                        break;

                    case "8":
                        loggedIn = false;
                        AsciiArt.ShowGoodbye();
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (operationCompleted)
                {
                    string again = "";
                    while (again != "yes" && again != "no")
                    {
                        Console.Write("\nWould you like to perform another operation? (yes/no): ");
                        again = (Console.ReadLine() ?? "").Trim().ToLower();

                        if (again == "y") again = "yes";
                        if (again == "n") again = "no";

                        if (again != "yes" && again != "no")
                        {
                            Console.WriteLine("Please type yes or no.");
                        }
                    }

                    if (again == "no")
                    {
                        loggedIn = false;
                        AsciiArt.ShowGoodbye();
                    }
                }
            }
        }
    }
}