using System.Text;

namespace LR1;

public static class Analyzer
{
    private const char Period = '.';
    private const char OpenParentheses = '(';
    private const char CloseParentheses = ')';
    private static readonly HashSet<char> Operators = ['+', '-', '*', '/', '^'];

    public static List<Error> Analyze(string expression)
    {
        var result = new List<Error>();
        var openParenthesesIndexes = new Stack<int>();

        for (var index = 0; index < expression.Length; index++)
        {
            var currChar = expression[index];

            if (char.IsNumber(currChar))
            {
                var (outIndex, error) = ValidateNumber(index, expression);

                if (error != null) result.Add(error);

                index = outIndex;
            }
            else if (Operators.Contains(currChar))
            {
                var (outIndex, errors) = ValidateOperator(index, expression);

                if (errors.Count > 0) result.AddRange(errors);

                index = outIndex;
            }
            else if (currChar == OpenParentheses)
            {
                openParenthesesIndexes.Push(index);
            }
            else if (currChar == CloseParentheses)
            {
                if (openParenthesesIndexes.Count > 0)
                    openParenthesesIndexes.Pop();
                else
                    result.Add(Error.Create(Message.ExtraClosingParentheses(), index));
            }
            else if ((char.IsLetter(currChar) &&
                      !(char.ToLower(currChar) >= 'a' && char.ToLower(currChar) <= 'z')) ||
                     currChar == Period)
            {
                result.Add(Error.Create(Message.InvalidCharacter(currChar), index));
            }
        }

        while (openParenthesesIndexes.Count > 0)
        {
            var openIndex = openParenthesesIndexes.Pop();
            result.Add(Error.Create(Message.MissingClosingParentheses(), openIndex));
        }

        return result;
    }

    private static (int outIndex, Error? error) ValidateNumber(int index, string expression)
    {
        var number = new StringBuilder();
        number.Append(expression[index]);

        var outIndex = index + 1;
        var periodCount = 0;

        for (var i = outIndex; i < expression.Length; i++)
        {
            outIndex = i;
            var currChar = expression[i];

            if (char.IsNumber(currChar))
            {
                number.Append(currChar);
            }
            else if (currChar == Period)
            {
                number.Append(currChar);
                periodCount++;
            }
            else
            {
                --outIndex;
                break;
            }
        }

        return periodCount > 1
            ? (outIndex, Error.Create(Message.InvalidNumberFormat(number.ToString()), outIndex))
            : (outIndex, null);
    }

    private static (int outIndex, List<Error> errors) ValidateOperator(int index, string expression)
    {
        var errors = new List<Error>();
        var previousOperator = expression[index];
        var outIndex = index + 1;

        for (var i = outIndex; i < expression.Length; i++)
        {
            outIndex = i;
            var currChar = expression[i];

            if (Operators.Contains(currChar))
            {
                errors.Add(Error.Create(Message.InvalidOperatorSequence(previousOperator, currChar), index));
            }
            else if (currChar == CloseParentheses)
            {
                errors.Add(Error.Create(Message.MissingOperand(previousOperator), index));
                --outIndex;
                break;
            }
            else
            {
                --outIndex;
                break;
            }
        }

        return (outIndex, errors);
    }
}