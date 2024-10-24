﻿namespace LR1;

public sealed class Message
{
    private Message(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Message InvalidCharacter(char invalidChar)
    {
        return new Message($"Неприпустимий символ '{invalidChar}'.");
    }

    public static Message ExtraClosingParentheses()
    {
        return new Message("Зайва закриваюча дужка");
    }

    public static Message MissingClosingParentheses()
    {
        return new Message("Відсутня закриваюча дужка");
    }

    public static Message InvalidOperatorSequence(char operator1, char operator2)
    {
        return new Message($"Некоректна послідовність операторів '{operator1}' і '{operator2}'");
    }

    public static Message InvalidNumberFormat(string number)
    {
        return new Message($"Некоректний десятковий дріб '{number}'");
    }

    public static Message MissingOperand(char operatorChar)
    {
        return new Message($"Відсутній операнд після оператора '{operatorChar}'");
    }
}