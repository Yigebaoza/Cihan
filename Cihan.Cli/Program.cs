using Cihan.Core.Lexing;
using Cihan.Core.Parsing;
using Cihan.Runtime;

var code = "Console.writeln(File.open(\"E:\\Cs\\Cihan\\Cihan.Cli\\data.txt\").read())";
var tokens = new Tokenizer(code).Tokenize();
var ast = new Parser(tokens).Parse();
var evaluator = new Evaluator();
evaluator.Evaluate(ast);