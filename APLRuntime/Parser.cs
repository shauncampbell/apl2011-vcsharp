using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.Tokeniser;
using APL.Errors;
namespace APL.SyntaxTreeGeneration
{
    /// <summary>
    /// Parses an APL expression and generates
    /// An Abstract Syntax Tree
    /// </summary>
    public class Parser
    {
        private int _position = 0;
        private List<Token> _tokens;

        /// <summary>
        /// Constructor taking a list of tokens as 
        /// a parameter
        /// </summary>
        /// <param name="tokens">list of tokens</param>
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        /// <summary>
        /// Parse a Function Name
        /// </summary>
        /// <returns></returns>
        public STree GetFunctionName()
        {
            if (_tokens[_position].Type == TokenType.FunctionName)
                return new STree(NodeType.FunctionName, _tokens[_position++].Value);
            else
                throw new APLParseError("Malformed Function Name: A function name must be specified in lowercase letters");
        }

        /// <summary>
        /// Parse a Variable Name
        /// </summary>
        /// <returns></returns>
        public STree GetVariableName()
        {
            if (_tokens[_position].Type == TokenType.VariableName)
                return new STree(NodeType.VariableName, _tokens[_position++].Value);
            else
                throw new APLParseError("Malformed Variable Name: A variable name must be specified in uppercase letters");
        }

        /// <summary>
        /// Parse an APL Expression into an Abstract Syntax Tree
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetExpression()
        {
            try
            {
                //  throw exception if blank token list is passed
                if (_position >= _tokens.Count)
                    throw new APLParseError("Syntax Error: Blank Expressions cannot be evaluated");
                switch (_tokens[_position].Type)
                {
                    case TokenType.LeftBracket:
                        int j = _position;
                        int leftBrackets = 0;
                        while (j < _tokens.Count)
                        {
                            if (_tokens[j].Type == TokenType.RightBracket && leftBrackets == 1)
                                break;
                            else if (_tokens[j].Type == TokenType.RightBracket)
                                leftBrackets--;
                            else if (_tokens[j].Type == TokenType.LeftBracket)
                                leftBrackets++;
                            j++;
                        }
                        if (leftBrackets > 1)
                            throw new APLParseError("Unterminated parenthesis in expression");

                        j++;
                        if (j == _tokens.Count)
                            return GetBracketedExpression();
                        else
                            return GetBinaryFunction();
                    case TokenType.LeftSquareBracket:
                        int rsbindex = _tokens.FindIndex(_position, delegate(Token t) { return t.Type == TokenType.RightSquareBracket; });
                        if (rsbindex < 0)
                            throw new APLParseError("Matrix declaration is unterminated. Values of a matrix must be enclosed by square brackets");
                        if (_tokens.Count - rsbindex > 1)
                            return GetBinaryFunction();
                        else
                            return GetAtomicExpression();
                    case TokenType.String:
                        if ((_tokens.Count - _position) == 1)
                            return GetAtomicExpression();
                        else
                            return GetBinaryFunction();
                    case TokenType.Literal:
                        if ((_tokens.Count - _position) == 1)
                            return GetAtomicExpression();
                        else if (((_tokens.Count - _position) == 2) && (_tokens[_tokens.Count - 1].Type == TokenType.RightBracket))
                            return GetAtomicExpression();
                        else
                            return GetBinaryFunction();
                    case TokenType.VariableName:
                        if ((_tokens.Count - _position) == 1)
                            return GetAtomicExpression();
                        else
                            return GetBinaryFunction();
                    case TokenType.Operator:
                        return GetOperator();
                    case TokenType.FunctionName:
                        if (_tokens[_position + 1].Type == TokenType.Operator)
                            return GetOperator();
                        else
                            return GetUnaryFunction();
                    case TokenType.Let:
                        return GetLetExpression();
                    case TokenType.VariableDeclaration:
                        return GetVariableDeclaration();
                    case TokenType.FunctionDeclaration:
                        return GetFunctionDeclaration();
                    case TokenType.If:
                        return GetConditionalExpression();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new APLParseError("Malformed Expression");
            }
            return new STree(NodeType.EOF, "");            
        }

        /// <summary>
        /// Parse an APL Expression into an Abstract Syntax Tree
        /// Given a set of tokens
        /// </summary>
        /// <param name="tokens">tokens</param>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetExpression(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
            return GetExpression();
        }
        
        /// <summary>
        /// Parse an Atomic APL Expression
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetAtomicExpression()
        {
            switch (_tokens[_position].Type)
            {
                case TokenType.Literal:
                    return new STree(NodeType.Literal, _tokens[_position++].Value);
                case TokenType.Operator:
                    return GetOperator();
                case TokenType.VariableName:
                    return GetVariableName();
                case TokenType.String:
                    return new STree(NodeType.Literal, _tokens[_position++].Value);
                case TokenType.LeftBracket:
                    return GetBracketedExpression();
                case TokenType.LeftSquareBracket:
                    STree g = new STree(NodeType.Matrix, "ARRAY");
                    int posInit = _position++;
                    while (_position < _tokens.Count)
                    {
                        if (_tokens[_position].Type == TokenType.RightSquareBracket)
                        {
                            _position++;
                            break;
                        }
                        g.Add(GetAtomicExpression());
                    }
                    return g;
                case TokenType.FunctionName:
                    return GetUnaryFunction();
                default:
                    throw new IndexOutOfRangeException();
            }

        }

        /// <summary>
        /// Returns an expression enclosed by brackets
        /// </summary>
        /// <returns></returns>
        public STree GetBracketedExpression()
        {
            List<Token> newTokens = new List<Token>();
            int posInit = ++_position;
            int leftBracks = 0;
            while (posInit < _tokens.Count)
            {
                if (_tokens[posInit].Type == TokenType.RightBracket && leftBracks == 0)
                    break;
                else if (_tokens[posInit].Type == TokenType.RightBracket)
                    leftBracks--;
                else if (_tokens[posInit].Type == TokenType.LeftBracket)
                    leftBracks++;
                
                newTokens.Add(_tokens[posInit]);
                posInit++;
            }
            if (leftBracks > 0)
                throw new APLParseError("Unterminated parenthesis. Check that you have closed all open parenthesis");
            _position = ++posInit;
            Parser p = new Parser(newTokens);
            return p.GetExpression();
            
        }

        /// <summary>
        /// Parse a matrix expression
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetMatrixExpression()
        {
            int posInit = _position++;
            STree b = new STree(NodeType.Matrix, "Array");
            int maxTokens = _tokens.Count;

            while (_position < maxTokens)
            {
                if (_tokens[_position].Type == TokenType.RightSquareBracket)
                {
                    _position++;
                    break;
                }
                try
                {
                    STree c = GetAtomicExpression();
                    b.Add(c);
                }
                catch (ArgumentOutOfRangeException ex)
                {                    
                    return b;
                }
            }

            return b;
            
        }

        /// <summary>
        /// Parse a Unary Function APL Expression
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetUnaryFunction()
        {
            //  Throw an exception if insufficient arguments

            if ((_tokens.Count - _position) < 2)
            {
                try
                {
                    throw new APLParseError("The unary function '" + _tokens[_position].Value + "' must contain exactly 'one' parameter");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new APLParseError("Unary functions must contain exactly 'one' valid parameter");
                }
                
            }
            STree funcNode = new STree(NodeType.FunctionCall, _tokens[_position++].Value);
            STree rightNode = GetExpression();
            funcNode.Add(rightNode);
            return funcNode;
        }

        /// <summary>
        /// Parse a Binary Function APL Expression
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetBinaryFunction()
        {
            //  Throw exception if theres not enough parameters
            if ((_tokens.Count - _position) < 3)
                throw new APLParseError("Syntax error. The binary function '" + _tokens[_position+1].Value + "' must contain exactly 'two' parameters");
            STree leftNode = GetAtomicExpression();
            if (IsOperator(_tokens[_position].Value))
            {
                STree operatorNode = new STree(NodeType.Operator, _tokens[_position++].Value[0].ToString());
                if (_tokens[_position].Type == TokenType.FunctionName)
                {
                    STree funcNode = new STree(NodeType.FunctionCall, _tokens[_position++].Value);
                    STree rightNode = GetExpression();
                    funcNode.Add(leftNode);
                    funcNode.Add(rightNode);
                    operatorNode.Add(funcNode);
                    return operatorNode;
                }
                else
                {
                    STree rightNode = GetExpression();
                    operatorNode.Add(leftNode);
                    operatorNode.Add(rightNode);
                    return operatorNode;
                }
            }
            else
            {
                STree funcNode = new STree(NodeType.FunctionCall, _tokens[_position++].Value);
                
                if (IsOperator(_tokens[_position].Value))
                {
                    STree operatorNode = new STree(NodeType.Operator, _tokens[_position++].Value);
                    operatorNode.Add(funcNode);
                    STree rightFunction = new STree(NodeType.FunctionCall, _tokens[_position++].Value);
                    STree rightValue = GetExpression();

                    operatorNode.Add(rightFunction);
                    operatorNode.Add(leftNode);
                    operatorNode.Add(rightValue);
                    return operatorNode;
                } else {
                    STree rightNode = GetExpression();
                    funcNode.Add(leftNode);
                    funcNode.Add(rightNode);
                    return funcNode;
                }
                
            }
           
        }

        /// <summary>
        /// Compare the current function name to ensure that
        /// it has been correctly identified as an operator
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsOperator(string s )
        {
            if (s.Length > 0)
            {
                return (s[0].Equals('\\') || s[0].Equals('.') || s.Equals('@'));
            }
            return false;
        }

        /// <summary>
        /// Parse a let Expression
        /// </summary>
        /// <returns>Abstract Syntax Tree</returns>
        public STree GetLetExpression()
        {
            if ((_tokens.Count - _position) < 6)
                throw new APLParseError("Incorrect Parameter Count: A Let Expression requires at least 3 parameters.");
            //  Move along and ignore Whitespace
            _position++;
            STree varName = GetVariableName();
            _position++;
            STree asName = GetAtomicExpression();
            _position++;
            STree expr = GetExpression();

            STree b = new STree(NodeType.LetExpression, "let");
            b.Add(varName);
            b.Add(asName);
            b.Add(expr);
            return b;
        }

        /// <summary>
        /// Parse a Variable Declaration
        /// </summary>
        /// <returns></returns>
        public STree GetVariableDeclaration()
        {
            if ((_tokens.Count - _position) < 4)
                throw new APLParseError("Incorrect parameter count: Variable Declarations take the following format: variable <VARIABLENAME> is <VARIABLEVALUE>");
            //  Move along and ignore Whitespace
            _position++;
            STree varName = GetVariableName();
            _position++;
            STree expr = GetExpression();

            STree b = new STree(NodeType.VariableDeclaration, "variable");
            b.Add(varName);
            b.Add(expr);
            return b;
        }

        /// <summary>
        /// Parse an Operator
        /// </summary>
        /// <returns></returns>
        public STree GetOperator()
        {
            STree func = new STree(NodeType.FunctionName, _tokens[_position++].Value);
            STree oper = new STree(NodeType.Operator, _tokens[_position++].Value);
            STree arg = GetExpression();
            oper.Add(func);
            oper.Add(arg);
            return oper;
        }
        /// <summary>
        /// Parse a Function Declaration
        /// </summary>
        /// <returns></returns>
        public STree GetFunctionDeclaration()
        {
            if ((_tokens.Count - _position) < 4)
                throw new IndexOutOfRangeException();
            //  Move along and ignore Whitespace
            STree b = new STree(NodeType.FunctionDeclaration, "function");
            _position++;
            STree funcName = GetFunctionName();
            b.Add(funcName);
            if (_tokens[_position].Type == TokenType.VariableName)
            {
                STree var1 = GetVariableName();
                b.Add(var1);
                if (_tokens[_position].Type == TokenType.VariableName)
                {
                    STree var2 = GetVariableName();
                    b.Add(var2);
                }
            }
            else
            {
                throw new APLParseError("Incorrect Arguments: function Declarations must be unary or binary");
            }
            _position++;
            STree expr = GetExpression();
            b.Add(expr);
            return b;
        }

        /// <summary>
        /// Parse a Conditional Expression
        /// </summary>
        /// <returns></returns>
        public STree GetConditionalExpression()
        {
            STree b;

            //  Calculate the If condition expression
            List<Token> newTokens = new List<Token>();
            _position++;
            while (_position < _tokens.Count)
            {
                Token t = _tokens[_position++];
                if (t.Type != TokenType.Then)
                    newTokens.Add(t);
                else
                    break;
            }
            Parser p = new Parser(newTokens);
            STree ifcondition = p.GetExpression();
            //  Calculate the then expression
            newTokens = new List<Token>();
          //  _position++;
            while (_position < _tokens.Count)
            {
                Token t = _tokens[_position++];
                if (t.Type != TokenType.Else)
                    newTokens.Add(t);
                else
                    break;
            }
            Parser q = new Parser(newTokens);
            STree thenExpression = q.GetExpression();
            //  Check if we have an else expression
            if (_position < _tokens.Count)
            {
                b = new STree(NodeType.ConditionalExpression, "conditional");
                newTokens = new List<Token>();
                while (_position < _tokens.Count)
                {
                    Token t = _tokens[_position++];
                    newTokens.Add(t);
                }
                Parser r = new Parser(newTokens);
                STree elseCondition = r.GetExpression();
                b.Add(ifcondition);
                b.Add(thenExpression);
                b.Add(elseCondition);
            }
            else
            {
                b = new STree(NodeType.ConditionalExpression, "ifthenexpression");
                b.Add(ifcondition);
                b.Add(thenExpression);
            }
            
            return b;
        }
    }
}
