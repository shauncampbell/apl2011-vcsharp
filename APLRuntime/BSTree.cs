using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.SyntaxTreeGeneration
{
    /// <summary>
    /// Each node has a different type associated with it
    /// </summary>
    public enum NodeType
    {
        VariableName,
        VariableDeclaration,
        FunctionName,
        FunctionDeclaration,
        FunctionCall,
        AtomicExpression, 
        ImportModule,
        LetExpression, 
        ConditionalExpression, 
        Operator, 
        MatrixDeclaration, 
        Variable, 
        Literal, 
        Matrix,
        EOF
    }

    /// <summary>
    /// Generic Tree structure for Abstract Syntax Tree
    /// </summary>
    public class STree
    {
        private List<STree> _nodes;
        private NodeType _type;
        private string _value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of this Node</param>
        /// <param name="value">Value of this Node</param>
        public STree(NodeType type, string value)
        {
            _nodes = new List<STree>();
            _type = type;
            _value = value;
        }

        /// <summary>
        /// Returns list of child nodes
        /// </summary>
        public List<STree> Children
        {
            get { return new List<STree>(_nodes); }
        }

        /// <summary>
        /// Returns number of child nodes
        /// </summary>
        public int Count
        {
            get { return _nodes.Count; }
        }
        
        /// <summary>
        /// Return child node at index
        /// </summary>
        /// <param name="Index">index of child node</param>
        /// <returns></returns>
        public STree Item(int Index)
        {
            return _nodes[Index];
        }
        
        /// <summary>
        /// Return the value of this node
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Get Type of this node
        /// </summary>
        public NodeType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Add a new Node to the tree
        /// </summary>
        /// <param name="Node">Node to add</param>
        public void Add(STree Node)
        {
            _nodes.Add(Node);
        }

        /// <summary>
        /// Return the Abstract Syntax Tree in
        /// indented string notation
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Value + " [" + Type + "]");
            foreach (STree b in Children)
            {
                sb.AppendLine("\t" + b.Value + " [ " + b.Type + "]");
                sb.Append(b.ToString(1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Produces a string of the current tree indented by 
        /// specified tabs.
        /// </summary>
        /// <param name="indent">level of indentation</param>
        /// <returns></returns>
        public string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            foreach (STree b in Children)
            {
                for (int i = 0; i <= indent; i++)
                    sb.Append("\t");
                sb.AppendLine(b.Value + " [ " + b.Type + "]");
                sb.Append(b.ToString(indent+1));
            }
            return sb.ToString();
        }
    }
}
