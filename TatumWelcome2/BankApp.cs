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

            bool running = true;

            while (running)
            {
                bool operationCompleted = false;

                Console.WriteLine("\n1. Deposit  2. Withdraw  3. Transfer  4. Airtime  5. Data  6. History  7. Balance  8. Logout");
                Console.Write("Choose: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        {
                            Console.WriteLine("(Enter 0 at any step to cancel)");

                            bool depositDone = false;
                            while (!depositDone)
                            {
                                Console.Write("Deposit amount: ");
                                string depositInput = Console.ReadLine() ?? "";
                                if (depositInput == "0")
                                {
                                    Console.WriteLine("Deposit cancelled.");
                                    break;
                                }

                                if (!decimal.TryParse(depositInput, out decimal depositAmount))
                                {
                                    Console.WriteLine("Enter a valid number.");
                                    continue;
                                }

                                if (current.Deposit(depositAmount, "deposit"))
                                {
                                    Console.WriteLine($"Deposited. Balance: {current.Balance:N2}");
                                    depositDone = true;
                                    operationCompleted = true;
                                }
                                else
                                {
                                    Console.WriteLine("Amount must be greater than zero.");
                                }
                            }
                            break;
                        }

                    case "2":
                        {
                            Console.WriteLine("(Enter 0 at any step to cancel)");

                            bool withdrawDone = false;
                            while (!withdrawDone)
                            {
                                Console.Write("Withdraw amount: ");
                                string withdrawInput = Console.ReadLine() ?? "";
                                if (withdrawInput == "0")
                                {
                                    Console.WriteLine("Withdrawal cancelled.");
                                    break;
                                }

                                if (!decimal.TryParse(withdrawInput, out decimal withdrawAmount))
                                {
                                    Console.WriteLine("Enter a valid number.");
                                    continue;
                                }

                                WithdrawResult withdrawCheck = current.CheckWithdraw(withdrawAmount);

                                if (withdrawCheck == WithdrawResult.InvalidAmount)
                                {
                                    Console.WriteLine("Amount must be greater than zero.");
                                    continue;
                                }

                                if (withdrawCheck == WithdrawResult.InsufficientFunds)
                                {
                                    Console.WriteLine($"You don't have enough balance. Your balance is {current.Balance:N2}.");
                                    continue;
                                }

                                bool withdrawPinOk = false;
                                bool withdrawPinCancelled = false;
                                int withdrawPinAttempts = 0;
                                while (!withdrawPinOk && !withdrawPinCancelled && withdrawPinAttempts < 3)
                                {
                                    Console.Write($"Enter your PIN to confirm withdrawal of {withdrawAmount:N2}: ");
                                    string withdrawPinInput = Console.ReadLine() ?? "";

                                    if (withdrawPinInput == "0")
                                    {
                                        withdrawPinCancelled = true;
                                    }
                                    else if (withdrawPinInput == current.Pin)
                                    {
                                        withdrawPinOk = true;
                                    }
                                    else
                                    {
                                        withdrawPinAttempts++;
                                        Console.WriteLine("Incorrect PIN.");
                                    }
                                }

                                if (withdrawPinCancelled)
                                {
                                    Console.WriteLine("Withdrawal cancelled.");
                                    break;
                                }

                                if (!withdrawPinOk)
                                {
                                    Console.WriteLine("Too many wrong PIN attempts. Transaction cancelled.");
                                    break;
                                }

                                current.Withdraw(withdrawAmount, "withdrawal");
                                Console.WriteLine($"Withdrawn. Balance: {current.Balance:N2}");
                                withdrawDone = true;
                                operationCompleted = true;
                            }
                            break;
                        }

                    case "3":
                        {
                            Console.WriteLine("(Enter 0 at any step to cancel)");

                            Account? recipient = null;
                            bool recipientValid = false;
                            while (!recipientValid)
                            {
                                Console.Write("Recipient account number: ");
                                string recipientInput = Console.ReadLine() ?? "";
                                if (recipientInput == "0")
                                {
                                    Console.WriteLine("Transfer cancelled.");
                                    break;
                                }

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

                            bool transferDone = false;
                            while (!transferDone)
                            {
                                Console.Write("Transfer amount: ");
                                string transferInput = Console.ReadLine() ?? "";
                                if (transferInput == "0")
                                {
                                    Console.WriteLine("Transfer cancelled.");
                                    break;
                                }

                                if (!decimal.TryParse(transferInput, out decimal transferAmount))
                                {
                                    Console.WriteLine("Enter a valid number.");
                                    continue;
                                }

                                WithdrawResult transferCheck = current.CheckWithdraw(transferAmount);

                                if (transferCheck == WithdrawResult.InvalidAmount)
                                {
                                    Console.WriteLine("Amount must be greater than zero.");
                                    continue;
                                }

                                if (transferCheck == WithdrawResult.InsufficientFunds)
                                {
                                    Console.WriteLine($"You don't have enough balance. Your balance is {current.Balance:N2}.");
                                    continue;
                                }

                                Console.WriteLine($"You are sending {transferAmount:N2} to {recipient.Name} ({recipient.AccountNumber}).");

                                bool transferPinOk = false;
                                bool transferPinCancelled = false;
                                int transferPinAttempts = 0;
                                while (!transferPinOk && !transferPinCancelled && transferPinAttempts < 3)
                                {
                                    Console.Write("Enter your PIN to confirm: ");
                                    string transferPinInput = Console.ReadLine() ?? "";

                                    if (transferPinInput == "0")
                                    {
                                        transferPinCancelled = true;
                                    }
                                    else if (transferPinInput == current.Pin)
                                    {
                                        transferPinOk = true;
                                    }
                                    else
                                    {
                                        transferPinAttempts++;
                                        Console.WriteLine("Incorrect PIN.");
                                    }
                                }

                                if (transferPinCancelled)
                                {
                                    Console.WriteLine("Transfer cancelled.");
                                    break;
                                }

                                if (!transferPinOk)
                                {
                                    Console.WriteLine("Too many wrong PIN attempts. Transaction cancelled.");
                                    break;
                                }

                                AccountService.Transfer(current, recipient, transferAmount);
                                Console.WriteLine($"{transferAmount:N2} sent to {recipient.Name}. Balance: {current.Balance:N2}");
                                transferDone = true;
                                operationCompleted = true;
                            }
                            break;
                        }

                    case "4":
                        {
                            Console.WriteLine("(Enter 0 at any step to cancel)");

                            int airtimeNetChoice = 0;
                            bool airtimeNetValid = false;
                            while (!airtimeNetValid)
                            {
                                Console.WriteLine("Network: 1. MTN  2. Airtel  3. Glo  4. 9mobile");
                                Console.Write("Choose network: ");
                                string airtimeNetInput = Console.ReadLine() ?? "";
                                if (airtimeNetInput == "0")
                                {
                                    Console.WriteLine("Airtime purchase cancelled.");
                                    break;
                                }

                                if (!int.TryParse(airtimeNetInput, out airtimeNetChoice) || airtimeNetChoice < 1 || airtimeNetChoice > ServicePlans.Networks.Length)
                                {
                                    Console.WriteLine("Invalid network. Choose a number from the list.");
                                    continue;
                                }

                                airtimeNetValid = true;
                            }
                            if (!airtimeNetValid) break;
                            string airtimeNetwork = ServicePlans.Networks[airtimeNetChoice - 1];

                            string airtimePhone = "";
                            bool airtimePhoneValid = false;
                            while (!airtimePhoneValid)
                            {
                                Console.Write("Phone number, 11 digits: ");
                                airtimePhone = Console.ReadLine() ?? "";
                                if (airtimePhone == "0")
                                {
                                    Console.WriteLine("Airtime purchase cancelled.");
                                    break;
                                }

                                bool airtimeDigitsOnly = airtimePhone.Length == 11;
                                foreach (char c in airtimePhone)
                                {
                                    if (!char.IsDigit(c))
                                        airtimeDigitsOnly = false;
                                }

                                if (!airtimeDigitsOnly)
                                {
                                    Console.WriteLine("Enter a valid 11-digit phone number.");
                                    continue;
                                }

                                airtimePhoneValid = true;
                            }
                            if (!airtimePhoneValid) break;

                            bool airtimeDone = false;
                            while (!airtimeDone)
                            {
                                Console.Write("Airtime amount, minimum 50: ");
                                string airtimeAmountInput = Console.ReadLine() ?? "";
                                if (airtimeAmountInput == "0")
                                {
                                    Console.WriteLine("Airtime purchase cancelled.");
                                    break;
                                }

                                if (!decimal.TryParse(airtimeAmountInput, out decimal airtimeAmount) || airtimeAmount < 50)
                                {
                                    Console.WriteLine("Enter a valid amount of at least 50.");
                                    continue;
                                }

                                WithdrawResult airtimeResult = current.Withdraw(airtimeAmount, $"Airtime {airtimeNetwork} {airtimePhone}");
                                if (airtimeResult == WithdrawResult.Success)
                                {
                                    Console.WriteLine($"{airtimeAmount:N2} airtime sent to {airtimePhone}. Balance: {current.Balance:N2}");
                                    airtimeDone = true;
                                    operationCompleted = true;
                                }
                                else
                                {
                                    Console.WriteLine($"You don't have enough balance. Your balance is {current.Balance:N2}.");
                                }
                            }
                            break;
                        }

                    case "5":
                        {
                            Console.WriteLine("(Enter 0 at any step to cancel)");

                            int dataNetChoice = 0;
                            bool dataNetValid = false;
                            while (!dataNetValid)
                            {
                                Console.WriteLine("Network: 1. MTN  2. Airtel  3. Glo  4. 9mobile");
                                Console.Write("Choose network: ");
                                string dataNetInput = Console.ReadLine() ?? "";
                                if (dataNetInput == "0")
                                {
                                    Console.WriteLine("Data purchase cancelled.");
                                    break;
                                }

                                if (!int.TryParse(dataNetInput, out dataNetChoice) || dataNetChoice < 1 || dataNetChoice > ServicePlans.Networks.Length)
                                {
                                    Console.WriteLine("Invalid network. Choose a number from the list.");
                                    continue;
                                }

                                dataNetValid = true;
                            }
                            if (!dataNetValid) break;
                            string dataNetwork = ServicePlans.Networks[dataNetChoice - 1];

                            int planChoice = 0;
                            bool planValid = false;
                            while (!planValid)
                            {
                                Console.WriteLine("Plans:");
                                for (int i = 0; i < ServicePlans.DataPlans.Count; i++)
                                {
                                    DataPlan plan = ServicePlans.DataPlans[i];
                                    Console.WriteLine($"{i + 1}. {plan.Name} - {plan.Price:N2}");
                                }

                                Console.Write("Choose plan: ");
                                string planInput = Console.ReadLine() ?? "";
                                if (planInput == "0")
                                {
                                    Console.WriteLine("Data purchase cancelled.");
                                    break;
                                }

                                if (!int.TryParse(planInput, out planChoice) || planChoice < 1 || planChoice > ServicePlans.DataPlans.Count)
                                {
                                    Console.WriteLine("Invalid plan. Choose a number from the list.");
                                    continue;
                                }

                                planValid = true;
                            }
                            if (!planValid) break;
                            DataPlan selectedPlan = ServicePlans.DataPlans[planChoice - 1];

                            string dataPhone = "";
                            bool dataPhoneValid = false;
                            while (!dataPhoneValid)
                            {
                                Console.Write("Phone number, 11 digits: ");
                                dataPhone = Console.ReadLine() ?? "";
                                if (dataPhone == "0")
                                {
                                    Console.WriteLine("Data purchase cancelled.");
                                    break;
                                }

                                bool dataDigitsOnly = dataPhone.Length == 11;
                                foreach (char c in dataPhone)
                                {
                                    if (!char.IsDigit(c))
                                        dataDigitsOnly = false;
                                }

                                if (!dataDigitsOnly)
                                {
                                    Console.WriteLine("Enter a valid 11-digit phone number.");
                                    continue;
                                }

                                dataPhoneValid = true;
                            }
                            if (!dataPhoneValid) break;

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
                        running = false;
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
                        running = false;
                        AsciiArt.ShowGoodbye();
                    }
                }
            }
        }
    }
}