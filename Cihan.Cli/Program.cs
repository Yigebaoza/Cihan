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
            string code = File.ReadAllText(args[0]);
            new Evaluator().Execute(new Parser(new Tokenizer(code).Tokenize()).ParseProgram());
        }
        catch (Exception e)
        {
            Console.WriteLine($"无法读取文件: {e}");
            return;
        }
    }
}