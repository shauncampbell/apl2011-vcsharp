using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using APL.Tokeniser;
using APL.SyntaxTreeGeneration;
using APL.Runtime;
using APL.Errors;
namespace APLConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter interpreter = Interpreter.GetInstance();

            Console.Write("APL 2011\nBy Shaun Campbell\n> ");
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    FileStream fs = new FileStream(arg, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);

                    string lin;
                    while ((lin = sr.ReadLine()) != "" && lin != null)
                    {
                        Console.WriteLine(lin);
                        List<Token> tokens = new List<Token>();
                        TokenScanner ts = new TokenScanner(lin);
                        while (ts.HasNext())
                        {
                            Token t = ts.Next();
                            if (t.Type != TokenType.WhiteSpace)
                                tokens.Add(t);
                        }

                        Parser p = new Parser(tokens);
                        try
                        {
                            Console.WriteLine(interpreter.InterpretLine(p.GetExpression()));
                        }
                        catch (APLRuntimeError ex)
                        {
                            Console.WriteLine("[RUNTIME ERROR] " + ex.Message);
                        }
                        catch (APLParseError ex)
                        {
                            Console.WriteLine("[PARSE ERROR] " + ex.Message);
                        }
                        Console.Write("> ");
                    }
                }
            }
            string line;
            while ((line = Console.ReadLine()) != "")
            {
                List<Token> tokens = new List<Token>();
                TokenScanner ts = new TokenScanner(line);
                while (ts.HasNext())
                {
                    Token t = ts.Next();
                    if (t.Type != TokenType.WhiteSpace)
                        tokens.Add(t);
                }

                Parser p = new Parser(tokens);
                try
                {
                    APLVariable returnLine = interpreter.InterpretLine(p.GetExpression());
                    if (!returnLine.Equals(""))
                        Console.WriteLine(returnLine.ToString());
                }
                catch (APLRuntimeError ex)
                {
                    Console.WriteLine("[RUNTIME ERROR] " + ex.Message);
                }
                catch (APLParseError ex)
                {
                    Console.WriteLine("[PARSE ERROR] " + ex.Message);
                }
                catch (NotImplementedException ex)
                {
                    Console.WriteLine("[INFO] This operation has not yet been implemented.");
                }
                Console.Write("> ");
            }
            Console.ReadLine();
        }

    }
}
