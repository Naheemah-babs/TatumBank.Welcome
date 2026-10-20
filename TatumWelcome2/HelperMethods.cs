namespace TatumWelcome2;

public static class HelperMethods
{
    public static bool IsCancel(string input)
    {
        if (input != "0")
        {
            return false;
        }

        Console.WriteLine("Operation cancelled.");
        return true;
    }

    public static decimal? ReadAmount(string prompt, Account? payer = null, decimal minimum = 0m)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim();

            if (IsCancel(input)) return null;

            if (!decimal.TryParse(input, out decimal amount))
            {
                Console.WriteLine("Enter a valid number.");
                continue;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
                continue;
            }

            if (amount < minimum)
            {
                Console.WriteLine($"Amount must be at least {minimum:N2}.");
                continue;
            }

            if (payer != null && payer.CheckWithdraw(amount) == WithdrawResult.InsufficientFunds)
            {
                Console.WriteLine($"You don't have enough balance. Your balance is {payer.Balance:N2}.");
                continue;
            }

            return amount;
        }
    }

    public static bool ConfirmPin(Account account, string prompt)
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";

            if (IsCancel(input)) return false;

            if (input == account.Pin)
            {
                return true;
            }

            Console.WriteLine("Incorrect PIN.");
        }

        Console.WriteLine("Too many wrong PIN attempts. Operation cancelled.");
        return false;
    }

    public static int ReadChoice(string prompt, int count)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim();

            if (IsCancel(input)) return 0;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > count)
            {
                Console.WriteLine("Invalid choice. Choose a number from the list.");
                continue;
            }

            return choice;
        }
    }

    public static string? ReadNetwork()
    {
        Console.WriteLine("Network:");
        for (int i = 0; i < ServicePlans.Networks.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {ServicePlans.Networks[i]}");
        }

        int choice = ReadChoice("Choose network: ", ServicePlans.Networks.Length);

        if (choice == 0)
        {
            return null;
        }

        return ServicePlans.Networks[choice - 1];
    }

    public static DataPlan? ReadPlan()
    {
        Console.WriteLine("Plans:");
        for (int i = 0; i < ServicePlans.DataPlans.Count; i++)
        {
            DataPlan plan = ServicePlans.DataPlans[i];
            Console.WriteLine($"{i + 1}. {plan.Name} - {plan.Price:N2}");
        }

        int choice = ReadChoice("Choose plan: ", ServicePlans.DataPlans.Count);

        if (choice == 0)
        {
            return null;
        }

        return ServicePlans.DataPlans[choice - 1];
    }

    public static string? ReadPhone()
    {
        while (true)
        {
            Console.Write("Phone number: ");
            string input = (Console.ReadLine() ?? "").Trim();

            if (IsCancel(input)) return null;

            string cleaned = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            if (cleaned.StartsWith("+234") && cleaned.Length == 14)
            {
                cleaned = "0" + cleaned.Substring(4);
            }
            else if (cleaned.StartsWith("234") && cleaned.Length == 13)
            {
                cleaned = "0" + cleaned.Substring(3);
            }
            else if (cleaned.Length == 10 && "789".Contains(cleaned[0]))
            {
                cleaned = "0" + cleaned;
            }

            bool digitsOnly = cleaned.Length == 11;
            foreach (char c in cleaned)
            {
                if (!char.IsDigit(c))
                    digitsOnly = false;
            }

            if (!digitsOnly || cleaned[0] != '0' || !"789".Contains(cleaned[1]))
            {
                Console.WriteLine("Enter a valid Nigerian phone number, e.g. 08012345678.");
                continue;
            }

            return cleaned;
        }
    }
}