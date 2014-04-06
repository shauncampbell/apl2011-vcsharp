using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using APL.SyntaxTreeGeneration;
using APL.Tokeniser;
using APL.Runtime;
using APL.Errors;
namespace APLDeveloper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(string line)
        {
            TokenScanner ts = new TokenScanner(line);
            List<Token> tokens = new List<Token>();
            while (ts.HasNext())
            {
                Token t = ts.Next();
                if (t.Type != TokenType.WhiteSpace)
                    tokens.Add(t);
            }

            Parser p = new Parser(tokens);
            textBox2.Text += "\t";
            textBox2.Text += (Interpreter.GetInstance().InterpretLine(p.GetExpression()).ToString());
            textBox2.Text += "\r\n";
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyData == Keys.Enter)
            {
                textBox2.Text += "> " + textBox1.Text;
                textBox2.Text += "\r\n";
                button1_Click(textBox1.Text);
                textBox1.Text = "";
                e.SuppressKeyPress = true;
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
