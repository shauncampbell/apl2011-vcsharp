using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using APL2011.BuiltinFunctions;
namespace APL2011
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("APL 2011 By Shaun Campbell");
            Console.WriteLine("Version 0.1 (Sept 2010)");
            Console.WriteLine("");
            String line = "";
            Console.Write("> ");
            APLInterpreter interpreter = new APLInterpreter();
            while ((line = Console.ReadLine()) != "")
            {
                String command1 = line;
                Console.WriteLine(interpreter.EvaluateExpression(line).ToString());
                
                Console.Write("> ");
            }

        }
    }
}
