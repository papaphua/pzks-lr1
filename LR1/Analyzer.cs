using System.Text;

namespace LR1;

public static class Analyzer
{
    private const char Period = '.';
    private const char OpenParentheses = '(';
    private const char CloseParentheses = ')';
    private static readonly HashSet<char> AdditiveOperators = ['+', '-'];
    private static readonly HashSet<char> MultiplicativeOperators = ['*', '/', '^'];
    private static readonly HashSet<char> Operators = [..AdditiveOperators, ..MultiplicativeOperators];

    public static List<Error> Analyze(string expression)
    {
        expression += ' ';
        var result = new List<Error>();

        var isCheckingNumber = false;
        var numberPeriodCount = 0;
        var numberStartIndex = 0;
        var checkedNumber = new StringBuilder();

        var openParenthesesIndexes = new Stack<int>();

        for (var index = 0; index < expression.Length; index++)
        {
            var currChar = expression[index];
            var prevChar = index - 1 >= 0 ? expression[index - 1] : (char?)null;
            var nextChar = index + 1 < expression.Length ? expression[index + 1] : (char?)null;

            // Marks the current index as the beginning of the number sequence validation
            if (char.IsNumber(currChar) && !isCheckingNumber)
            {
                isCheckingNumber = true;
                numberStartIndex = index;
            }

            // Verifies that the number sequence contains no more than one period
            if (isCheckingNumber)
            {
                if (char.IsNumber(currChar))
                {
                    checkedNumber.Append(currChar);
                }
                else if (currChar == Period)
                {
                    checkedNumber.Append(currChar);
                    ++numberPeriodCount;
                }
                else
                {
                    if (numberPeriodCount > 1)
                        result.Add(
                            Error.Create(Message.InvalidNumberFormat(checkedNumber.ToString()), numberStartIndex));

                    isCheckingNumber = false;
                    numberPeriodCount = 0;
                    numberStartIndex = 0;
                    checkedNumber.Clear();
                }
            }

            if (currChar == OpenParentheses)
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

            // Verifies that operator has valid operands before and after it
            if (Operators.Contains(currChar))
            {
                // Ensures that '*' '/' '^' are preceded by a number or letter, but not by '('
                // Treats '+' and '-' as indicators of positive or negative signs for the current character
                if (MultiplicativeOperators.Contains(currChar) &&
                    (prevChar == null || !IsPreviousOperand(prevChar.Value)))
                    result.Add(Error.Create(Message.MissingOperandBefore(currChar), index));

                // Ensures that the character following current operator is a number, letter, or '('
                if (nextChar != null && !IsNextOperand(nextChar.Value))
                    result.Add(Error.Create(Message.MissingOperandAfter(currChar), index));
            }
        }

        while (openParenthesesIndexes.Count > 0)
        {
            var index = openParenthesesIndexes.Pop();
            result.Add(Error.Create(Message.MissingClosingParentheses(), index));
        }

        return result;
    }

    // Checks if the previous character is a valid operand ( letter, number, or ')' )
    private static bool IsPreviousOperand(char character)
    {
        return char.IsNumber(character) || char.IsLetter(character) || character == CloseParentheses;
    }

    // Checks whether the next character is a valid operand ( letter, number, or '(' )
    private static bool IsNextOperand(char character)
    {
        return char.IsNumber(character) || char.IsLetter(character) || character == OpenParentheses;
    }
}