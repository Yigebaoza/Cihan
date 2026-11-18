using Cihan.Core.Ast;
using Cihan.Core.Lexing;
using Cihan.Core.Parsing;
using Cihan.Runtime;

namespace Cihan.Cli;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("用法: cihan <file.cihan>");
            return;
        }
        try
        {
            bool showTokens = false;
            bool showStatements = false;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-showTks": showTokens = true; break;
                    case "-showStms": showStatements = true; break;
                }
            }
            string code = File.ReadAllText(args[0]);
            List<Token> tokens = new Tokenizer(code).Tokenize();
            if (showTokens)
            {
                Console.WriteLine("\tTokens Start");
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token);
                }
                Console.WriteLine("\tTokens Stop");
            }
            Core.Ast.Program program = new Parser(tokens).ParseProgram();
            if (showStatements)
            {
                Console.WriteLine("\tStatements Start");
                foreach (Stmt stmt in program.Statements)
                {
                    Console.WriteLine(stmt);
                }
                Console.WriteLine("\tStatements Stop");
            }
            new Evaluator().Execute(program);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message.Replace("System.Exception: ", "")); // 去掉开头的System.Exception:
            return;
        }
    }
}