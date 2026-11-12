using System;
using System.Collections.Generic;
using Godot;
using GodotShell.Runtime.Commands;

namespace GodotShell.Runtime;

public class GDSRuntime
{
    public Dictionary<string, GDSCommand> Commands = [];

    public GDSCommand Command(string name, string description)
    {
        GDSCommand command = new(name, description);
        Commands[name] = command;
        return command;
    }

    public GDSRuntime SetupBaseLibrary()
    {
        var printCommand = Command("print", "Prints text to the console");
        
        printCommand
            .Argument<string>()

            .Executes((string message)
                => GD.Print(message)
            );

        var logCommand = Command("log", "Logs to the console");
        
        logCommand
            .Command("raw", "Prints raw text to the console")
            .Argument<string>()

            .Executes((string message)
                => GD.Print(message)
            );
        
        logCommand
            .Command("rich", "Prints rich text to the console")
            .Argument<string>()

            .Executes((string richMessage)
                => GD.PrintRich(richMessage)
            );
        
        logCommand
            .Command("colored", "Prints colored text to the console")

            .Argument<string>()
            .Argument<string>()

            .Executes((string colorCode, string message)
                => GD.PrintRich($"[color={colorCode}]{message}")
            );

        return this;
    }

    public GDSRuntime SetupLib()
        => SetupBaseLibrary();

    public string[] Tokenize(string text)
    {
        List<string> tokens = [];

        string collectedText = "";
        bool insideString = false;

        foreach (char currentChar in text.ToCharArray())
        {
            if (currentChar == '"' || currentChar == '\'')
            {
                insideString = !insideString;
                continue;
            }
            
            if (!insideString && currentChar == ' ')
            {
                tokens.Add(collectedText);
                collectedText = "";
                continue;
            }

            collectedText = $"{collectedText}{currentChar}";
        }

        if (collectedText.Length > 0)
            tokens.Add(collectedText);

        if (insideString)
            throw new SyntaxException("String was never excited");
        
        tokens.Add("<!EOF!>");

        return [.. tokens];
    }

    public Queue<string> Tokens = [];

    public string Advance()
        => Tokens.Dequeue();
    
    public string Peek()
        => Tokens.Peek();

    public void Interpret(string[] tokens, string originalText)
    {
        Tokens.Clear();
        
        foreach (string token in tokens)
            Tokens.Enqueue(token);
        
        if (!Commands.TryGetValue(Advance(), out GDSCommand command))
            throw new CommandNotFoundException(originalText);
        
        GDSNode node = command;
        
        List<object> args = [];
        
        while (Peek() != "<!EOF!>")
        {
            if (command.NextCommands != null)
                if (command.NextCommands.TryGetValue(Peek(), out GDSCommand subCommand))
                {
                    Advance();

                    command = subCommand;
                    node = command;
                    continue;
                } else if (command.NextArgument == null)
                    throw new CommandNotFoundException(originalText);
            
            node = node.NextArgument;
            
            if (node is GDSArgument<int>)
                args.Add(int.Parse(Advance()));
            else if (node is GDSArgument<float>)
                args.Add(float.Parse(Advance()));
            else if (node is GDSArgument<double>)
                args.Add(double.Parse(Advance()));
            else
                args.Add(Advance());
        }

        node.Callback?.DynamicInvoke([.. args]);
    }

    public void Execute(string text)
        => Interpret(Tokenize(text), text);
    
    public class SyntaxException(string message) : Exception(message);

    public class CommandNotFoundException(string commandText) : Exception($"The given command was not found: {commandText}")
    {
        public readonly string CommandText = commandText;
    }
}