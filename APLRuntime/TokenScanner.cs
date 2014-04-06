using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Tokeniser
{
    public class TokenScanner
    {
        private string _line;
        private string _nextToken;
        private int _curPos = -1;
        private TokenType _nextType;
        
        public TokenScanner(string line)
        {
            _line = line;
            if (_line.Length >= 1)
            {
                _curPos = 0;
                NextNode();
            }
        }

        /// <summary>
        /// Check if the TokenScanner has another node to return
        /// </summary>
        /// <returns>boolean</returns>
        public bool HasNext()
        {
            return _nextType != TokenType.EOF;
        }

   
        /// <summary>
        /// Return the next token from the expression
        /// </summary>
        /// <returns>Token</returns>
        public Token Next()
        {
            if (_nextType == TokenType.EOF)
                throw new IndexOutOfRangeException();
            Token t = new Token(_nextType, _nextToken.ToString());
            if (_curPos < _line.Length)
                NextNode();
            else
                _nextType = TokenType.EOF;
            return t;
        }

        /***
         * Set instance variables to point at the next node
         ***/
        private void NextNode ()
        {
            Char c = _line[_curPos];
            StringBuilder sb = new StringBuilder();
            if (Char.IsUpper(c))
            {
                ReadUppercaseString(sb, c);
            }
            else if (Char.IsLower(c))
            {
                ReadLowercaseString(sb, c);
            }
            else if (c.Equals('"'))
            {
                ReadQuotedString(sb, c);
            }
            else if ((Char.IsNumber(c)))
            {
                ReadNumber(sb, c);
            }
            else if ((Char.IsSymbol(c) || Char.IsPunctuation(c)))
            {
                ReadSymbol(sb, c);
            }
            else if (Char.IsWhiteSpace(c))
            {
                ReadWhitespace(sb, c);
            }
            else
            {
                _nextToken = "";
                _nextType = TokenType.EOF;
            }
        }
        private bool ValidFunctionName(Char c)
        {
            return !(c.Equals('(') || c.Equals(')') || c.Equals('[') || c.Equals(']') || c.Equals('\\') || c.Equals('.') || c.Equals('/') || c.Equals('"'));
        }
        private void ReadUppercaseString(StringBuilder sb, Char c)
        {
            while (Char.IsUpper(c) && _curPos < _line.Length)
            {
                sb.Append(c);
                try
                {
                    c = _line[++_curPos];
                }
                catch (IndexOutOfRangeException i)
                {
                    break;
                }
            }
            _nextToken = sb.ToString();
            _nextType = TokenType.VariableName;
        }
        private void ReadLowercaseString(StringBuilder sb, Char c)
        {
            while (Char.IsLower(c) && _curPos < _line.Length)
            {
                sb.Append(c);
                try
                {
                    c = _line[++_curPos];
                }
                catch (IndexOutOfRangeException i)
                {
                    break;
                }
            }
            string a = sb.ToString();
            if (a.Equals("if"))
                _nextType = TokenType.If;
            else if (a.Equals("then"))
                _nextType = TokenType.Then;
            else if (a.Equals("else"))
                _nextType = TokenType.Else;
            else if (a.Equals("let"))
                _nextType = TokenType.Let;
            else if (a.Equals("as"))
                _nextType = TokenType.As;
            else if (a.Equals("in"))
                _nextType = TokenType.In;
            else if (a.Equals("function"))
                _nextType = TokenType.FunctionDeclaration;
            else if (a.Equals("variable"))
                _nextType = TokenType.VariableDeclaration;
            else
                _nextType = TokenType.FunctionName;
            _nextToken = a;

        }
        private void ReadQuotedString(StringBuilder sb, Char c)
        {
            c = _line[++_curPos];
            while (!c.Equals('"'))
            {
                sb.Append(c);
                try
                {
                    c = _line[++_curPos];
                }
                catch (IndexOutOfRangeException i)
                {
                    break;
                }
            }
            c = _line[_curPos++];
            _nextToken = sb.ToString();
            _nextType = TokenType.String;
        }
        private void ReadNumber(StringBuilder sb, Char c)
        {
            while (Char.IsNumber(c) || c.Equals('.'))
            {
                sb.Append(c);
                try
                {
                    c = _line[++_curPos];
                }
                catch (IndexOutOfRangeException i)
                {
                    break;
                }
            }
            _nextToken = sb.ToString();
            _nextType = TokenType.Literal;
        }
        private void ReadSymbol(StringBuilder sb, Char c)
        {
            switch (c)
            {
                case '(':
                    _nextToken = "(";
                    _nextType = TokenType.LeftBracket;
                    _curPos++;
                    break;
                case ')':
                    _nextToken = ")";
                    _nextType = TokenType.RightBracket;
                    _curPos++;
                    break;
                case '[':
                    _nextToken = "[";
                    _nextType = TokenType.LeftSquareBracket;
                    _curPos++;
                    break;
                case ']':
                    _nextToken = "]";
                    _nextType = TokenType.RightSquareBracket;
                    _curPos++;
                    break;
                case '.':
                    _nextToken = ".";
                    _nextType = TokenType.Operator;
                    _curPos++;
                    break;
                case '\\':
                    _nextToken = "\\";
                    _nextType = TokenType.Operator;
                    _curPos++;
                    break;
                case '|':
                    _nextToken = "|";
                    _nextType = TokenType.FunctionName;
                    _curPos++;
                    break;
                case ',':
                    _nextToken = ",";
                    _nextType = TokenType.Operator;
                    _curPos++;
                    break;
                case '@':
                    _nextToken = "@";
                    _nextType = TokenType.Operator;
                    _curPos++;
                    break;
                case '+':
                    _nextToken = "+";
                    _nextType = TokenType.FunctionName;
                    _curPos++;
                    break;
                case '-':
                    _nextToken = "-";
                    _nextType = TokenType.FunctionName;
                    _curPos++;
                    break;
                case '*':
                    _nextToken = "*";
                    _nextType = TokenType.FunctionName;
                    _curPos++;
                    break;
                case '/':
                    _nextToken = "/";
                    _nextType = TokenType.Operator;
                    _curPos++;
                    break;
                default:
                    while ((Char.IsSymbol(c) || Char.IsPunctuation(c)) && _curPos < _line.Length)
                    {
                        sb.Append(c);
                        try
                        {
                            c = _line[++_curPos];
                            if (!ValidFunctionName(c))
                                break;
                        }
                        catch (IndexOutOfRangeException i)
                        {
                            break;
                        }
                    }
                    _nextToken = sb.ToString();
                    _nextType = TokenType.FunctionName;
                    break;
            }
        }
        private void ReadWhitespace(StringBuilder sb, Char c)
        {
            while (Char.IsWhiteSpace(c) && _curPos < _line.Length)
            {
                sb.Append(c);
                try
                {
                    c = _line[++_curPos];
                }
                catch (IndexOutOfRangeException i)
                {
                    break;
                }
            }
            _nextToken = sb.ToString();
            _nextType = TokenType.WhiteSpace;
        }
    }
}
