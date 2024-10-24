using LR1;

var input = string.Empty;

Console.WriteLine("Введіть 'q' для завершення.\n");

while (true)
{
    Console.Write("Введіть вираз для аналізу: ");

    while (string.IsNullOrWhiteSpace(input)) input = Console.ReadLine();

    if (input.Equals("q", StringComparison.CurrentCultureIgnoreCase)) break;

    var errors = Analyzer.Analyze(input);

    if (errors.Count != 0)
    {
        Console.WriteLine();

        errors = errors
            .OrderBy(e => e.Position)
            .ToList();

        foreach (var error in errors)
            error.Display();
    }
    else
    {
        Console.WriteLine("Вираз коректний.");
    }

    input = string.Empty;

    Console.WriteLine();
}