using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Tokeniser
{
    /// <summary>
    /// Each token has a different type associated with it
    /// </summary>
    public enum TokenType
    {
        EOF, 
        Literal, 
        String, 
        WhiteSpace, 
        VariableName, 
        VariableDeclaration,
        LeftBracket,
        RightBracket,
        LeftSquareBracket,
        RightSquareBracket,
        Comma, 
        Operator, 
        If, 
        Then, 
        Else, 
        Let, 
        Import, 
        As, 
        In, 
        FunctionName, 
        FunctionDeclaration
    }

    /// <summary>
    /// A Token is a block of text within an expression
    /// With a format associated with it.
    /// </summary>
    public class Token
    {
        private TokenType _type;
        private string _value;
        /// <summary>
        /// Returns the type of this token
        /// </summary>
        public TokenType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Returns the value of this token
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type associated with this token</param>
        /// <param name="value">Value associated with this token</param>
        public Token(TokenType type, string value)
        {
            _type = type;
            _value = value;
        }
    }
}
