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

            Console.WriteLine("1. Create a Tatum account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("\nPlease choose an option: ");
            string mainChoice = (Console.ReadLine() ?? "").Trim();

            switch (mainChoice)
            {
                case "1":
                    CreateAccountFlow(accounts);
                    continue;

                case "2":
                    break;

                case "3":
                    AsciiArt.ShowGoodbye();
                    appRunning = false;
                    continue;

                default:
                    Console.WriteLine("invalid choice");
                    continue;

            }
            Account? current = LoginFlow(accounts);
            if (current == null) continue;

            Console.WriteLine($"Welcome, {current.Name}.");
            Console.WriteLine("(Enter 0 at any step of an operation to cancel)");

            bool loggedIn = true;

            while (loggedIn)
            {
                bool operationCompleted = false;

                Console.WriteLine("\n1. Deposit  2. Withdraw  3. Transfer  4. Airtime  5. Data  6. History  7. Balance  8. Logout");
                Console.Write("Choose: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1":
                        {
                            decimal? depositAmount = HelperMethods.ReadAmount("Deposit amount: ");
                            if (depositAmount == null) break;

                            current.Deposit(depositAmount.Value, "deposit");
                            Console.WriteLine($"Deposited. Balance: ₦{current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "2":
                        {
                            decimal? withdrawAmount = HelperMethods.ReadAmount("Withdraw amount: ", current);
                            if (withdrawAmount == null) break;

                            if (!HelperMethods.ConfirmPin(current, $"Enter your PIN to confirm withdrawal of ₦{withdrawAmount.Value:N2}: ")) break;

                            current.Withdraw(withdrawAmount.Value, "withdrawal");
                            Console.WriteLine($"Withdrawn. Balance: ₦{current.Balance:N2}");
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

                                TransactionResult recipientCheck = AccountService.CheckRecipient(accounts, current, recipientInput, out recipient);

                                if (recipientCheck == TransactionResult.RecipientNotFound)
                                {
                                    Console.WriteLine("Account not found.");
                                    continue;
                                }

                                if (recipientCheck == TransactionResult.SameAccount)
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

                            Console.WriteLine($"You are sending ₦{transferAmount.Value:N2} to {recipient.Name} ({recipient.AccountNumber}).");

                            if (!HelperMethods.ConfirmPin(current, "Enter your PIN to confirm: ")) break;


                            TransactionResult transferResult = AccountService.Transfer(current, recipient, transferAmount.Value);

                            if (transferResult != TransactionResult.Success)
                            {
                                Console.WriteLine("Transfer failed.");
                                break;
                            }

                            Console.WriteLine($"₦{transferAmount.Value:N2} sent to {recipient.Name}. Balance: ₦{current.Balance:N2}");
                            operationCompleted = true;
                            break;
                        }

                    case "4":
                        {
                            string? airtimeNetwork = HelperMethods.ReadNetwork();
                            if (airtimeNetwork == null) break;

                            string? airtimePhone = HelperMethods.ReadPhone();
                            if (airtimePhone == null) break;

                            decimal? airtimeAmount = HelperMethods.ReadAmount("Airtime amount, minimum ₦50: ", current, 50m);
                            if (airtimeAmount == null) break;

                            operationCompleted = HelperMethods.ProcessPayment(
                                current,
                                airtimeAmount.Value,
                                $"Airtime {airtimeNetwork} to {airtimePhone}",
                                $" {airtimeNetwork} Airtime sent to {airtimePhone}."
                                );
                            break;
                        }

                    case "5":
                        {
                            string? network = HelperMethods.ReadNetwork();
                            if (network == null) break;

                            DataPlan? selectedPlan = HelperMethods.ReadPlan();
                            if (selectedPlan == null) break;

                            string? phone = HelperMethods.ReadPhone();
                            if (phone == null) break;

                            operationCompleted = HelperMethods.ProcessPayment(
                                current,
                                selectedPlan.Price,
                                $"Data {network} {selectedPlan.Name} {phone}",
                                $"Data plan {selectedPlan.Name} sent to {phone}.");

                            break;
                        }

                    case "6":
                        AccountHistory.PrintHistory(current);
                        operationCompleted = true;
                        break;

                    case "7":
                        Console.WriteLine($"Balance: ₦{current.Balance:N2}");
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

    private static void CreateAccountFlow(List<Account> accounts)
    {
        Console.WriteLine("\n--- Create a Tatum account ---");
        Console.WriteLine("(Enter 0 at any step to go back)");

        string? name = HelperMethods.ReadText("Full name: ");
        if (name == null) return;

        string? city = HelperMethods.ReadText("City: ");
        if (city == null) return;

        string? phone = HelperMethods.ReadPhone();
        if (phone == null) return;


        string? pin = HelperMethods.ReadNewPin();
        if (pin == null) return;

        decimal? initialDeposit = HelperMethods.ReadInitialDeposit();
        if (initialDeposit == null) return;

        Account created = AccountService.CreateAccount(
            accounts, name, phone, city, pin, initialDeposit.Value);


        Console.WriteLine($"\nAccount created successfully! Welcome to Tatum, {created.Name}.");
        Console.WriteLine($"Account number: {created.AccountNumber}");
        Console.WriteLine($"Opening balance: ₦{created.Balance:N2}");
        Console.WriteLine("Keep your account number safe. You will need it to log in.");
        Console.Write("\nPress Enter to return to the main menu...");
        Console.ReadLine();
    }

    private static Account? LoginFlow(List<Account> accounts)
    {
        Console.WriteLine("\n--- Login ---");
        Console.WriteLine("(Enter 0 to go back to the main menu)");

        for (int attempts = 0; attempts < 3; attempts++)
        {
            Console.Write("Account number: ");
            string number = (Console.ReadLine() ?? "").Trim();
            if (number == "0") return null;

            Console.Write("PIN: ");
            string pin = HelperMethods.ReadHidden().Trim();
            if (pin == "0") return null;

            Account? account = AccountService.Login(accounts, number, pin);
            if (account != null) return account;

            Console.WriteLine("Invalid account number or PIN.");
        }

        Console.WriteLine("Too many failed attempts. Returning to the main menu...");
        return null;
    }
};