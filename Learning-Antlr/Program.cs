//Install, Nuget Antlr4
//Maybe install the Antlr4.Runtime.Standard

using Antlr4.Runtime;
using Learning_Antlr;
using Learning_Antlr.ANTLR;

var fileName = "SampleScript\\test.ss";

var fileContents = File.ReadAllText(fileName);

AntlrInputStream inputStream = new(fileContents);
SimpleLexer simpleLexer = new(inputStream);
CommonTokenStream commonTokenStream = new(simpleLexer);
SimpleParser simpleParser = new(commonTokenStream);

var simpleContext = simpleParser.program();
SimpleVisitor visitor = new();
visitor.Visit(simpleContext);