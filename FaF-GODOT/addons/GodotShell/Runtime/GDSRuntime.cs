using System;
using System.Collections.Generic;
using GodotShell.Runtime.Commands;

namespace GodotShell.Runtime;

public partial struct CallerData {}

public class GDSRuntime
{
    public Dictionary<string, GDSCommand> Commands = [];

    public GDSCommand Command(string name, string description)
    {
        GDSCommand command = new(name, description) { Runtime = this };
        Commands[name] = command;
        return command;
    }

    public GDSRuntime SetupBaseLibrary()
    {
        var logCommand = Command("log", "Logs to the console. Allows the use of BBCode");
        
        logCommand
            .Argument<string>()

            .Executes((CallerData _, string message)
                => Log(message)
            );
        
        logCommand
            .Command("colored", "Prints colored text to the console")

            .Argument<string>()
            .Argument<string>()

            .Executes((CallerData _, string colorCode, string message)
                => Log($"[color={colorCode}]{message}")
            );

        return this;
    }

    public GDSRuntime SetupLib()
        => SetupBaseLibrary();
    
    public delegate void LoggedEventHandler(string message);
    public event LoggedEventHandler Logged;

    public void Log(string message)
        => Logged?.Invoke(message);

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

    private Queue<string> Tokens = [];

    private string Advance()
        => Tokens.Dequeue();
    
    private string Peek()
        => Tokens.Peek();

    public void Interpret(string[] tokens, string originalText, CallerData callerData = default)
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
                    throw new CommandNotFoundException(Peek());
            
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

        node.Callback?.DynamicInvoke([callerData, .. args]);
    }

    public void Execute(string text, CallerData callerData = default)
        => Interpret(Tokenize(text), text, callerData);
    
    public class SyntaxException(string message) : Exception(message);

    public class CommandNotFoundException(string commandText) : Exception($"The given command was not found: {commandText}")
    {
        public readonly string CommandText = commandText;
    }
}