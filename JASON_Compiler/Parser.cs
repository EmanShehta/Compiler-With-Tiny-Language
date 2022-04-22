using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
           root.Children.Add(progra()); ////////////////////////////////////////////////////
            return root;
        }
        // function call
        Node arguments() {
            Node a = new Node("arguments");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Idenifier)
            {
                a.Children.Add(match(Token_Class.T_Idenifier)); 
            }
            a.Children.Add(arg());
                return a;
        }
       
        Node arg()
        {
            Node a = new Node("Arg");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                a.Children.Add(match(Token_Class.T_Comma));
                a.Children.Add(arguments());
                return a;
            }
            else
                return null;
        }
        Node Function_call()
        {
            Node a = new Node("Function_call");

            a.Children.Add(match(Token_Class.T_Idenifier));
            a.Children.Add(match(Token_Class.T_LParanthesis));
            a.Children.Add(arguments());
            a.Children.Add(match(Token_Class.T_RParanthesis));
                return a;
           
        }
        // term
        Node Term()
        {
            Node a = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Number)
                a.Children.Add(match(Token_Class.T_Number));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Idenifier)
                a.Children.Add(match(Token_Class.T_Idenifier));

            else
                a.Children.Add(Function_call());

            return a;

        }
        // equation
        Node Equation_p1()
        {
            Node a = new Node("Equation_p1");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Idenifier ))
            {
                a.Children.Add(Term());
                if  (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp)
                    a.Children.Add(match(Token_Class.T_PlusOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MinusOp)
                    a.Children.Add(match(Token_Class.T_MinusOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
                    a.Children.Add(match(Token_Class.T_DivideOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp)
                    a.Children.Add(match(Token_Class.T_MultiplyOp));
                a.Children.Add(Equation_p1());
                return a;
            }
            else
                return null;
        }
        Node Equation_p2()
        {
            Node a = new Node("Equation_p2");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_PlusOp || TokenStream[InputPointer].token_type == Token_Class.T_MinusOp || TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp)
                    a.Children.Add(match(Token_Class.T_PlusOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MinusOp)
                    a.Children.Add(match(Token_Class.T_MinusOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
                    a.Children.Add(match(Token_Class.T_DivideOp));
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp)
                    a.Children.Add(match(Token_Class.T_MultiplyOp));
                a.Children.Add(Term());
                a.Children.Add(Equation_p2());
                return a;
            }
            else
                return null;
        }
        Node Equation_p3()
        {
            Node a = new Node("Equation_p3");
           
                a.Children.Add(match(Token_Class.T_LParanthesis));
                a.Children.Add(Equation_p1());
                a.Children.Add(Term());
                a.Children.Add(match(Token_Class.T_RParanthesis));
                return a;
           
        }
        Node Equation_p4()
        {
            Node a = new Node("Equation_p4");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_PlusOp || TokenStream[InputPointer].token_type == Token_Class.T_MinusOp || TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
                a.Children.Add(Equation_p2());
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_LParanthesis)
                a.Children.Add(Equation_p3());

            return a; 
        }
        Node Equation()
        {
            Node a = new Node("Equation");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Idenifier))
            {
                a.Children.Add(Equation_p1());
                a.Children.Add(Term());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_LParanthesis)
            {
                a.Children.Add(Equation_p3());
                a.Children.Add(Equation_p4());
            }
            return a;
        }
        // expression
        Node Expression()
        {
            Node a = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_String)
                a.Children.Add(match(Token_Class.T_String));
            else if (  InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Idenifier)
                a.Children.Add(Term());
            else
                a.Children.Add(Equation());
            return a;
        }
        // assinment statement
        Node Assinment_statement()
        {
            Node a = new Node("Assinment_statement");
            a.Children.Add(match(Token_Class.T_Idenifier));
            a.Children.Add(match(Token_Class.T_assign));
            a.Children.Add(Expression());
            a.Children.Add(match(Token_Class.T_Semicolon));

            return a;
        }
        // data type
        Node Data_type()
        {
            Node a = new Node("Data_type");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Int_DataType)
                a.Children.Add(match(Token_Class.T_Int_DataType));
           else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_String_DataType)
                a.Children.Add(match(Token_Class.T_String_DataType));
           else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Float_DataType)
                a.Children.Add(match(Token_Class.T_Float_DataType));
            return a;
        }
        // declaration statement

        Node Dec1()
        {
            Node a = new Node("Dec1");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.T_assign)
                {
                    a.Children.Add(match(Token_Class.T_Idenifier));
                    a.Children.Add(match(Token_Class.T_assign));
                    a.Children.Add(Expression());
                }
                else
                    a.Children.Add(match(Token_Class.T_Idenifier));
            }
           

            return a;

        }
        Node Dec2()
        {
            Node a = new Node("Dec2");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                a.Children.Add(match(Token_Class.T_Comma));
                a.Children.Add(match(Token_Class.T_Idenifier));
                a.Children.Add(Dec2());
                return a;
            }
            else return null;

        }
        Node Declaration_statement()
        {
            Node a = new Node("Declaration_statement");
            a.Children.Add(Data_type());
            a.Children.Add(Dec1());
            a.Children.Add(Dec2());
          //  if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.T_Semicolon)
            a.Children.Add(match(Token_Class.T_Semicolon));
            return a;
        }
        // write statement
        Node write1()
        {
            Node a = new Node("write1");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Endl)
                a.Children.Add(match(Token_Class.T_Endl));
            else
                a.Children.Add(Expression());
            return a;
        }
        Node Write_statement()
        {
            Node a = new Node("Write_statement");
            a.Children.Add(match(Token_Class.T_Write));
            a.Children.Add(write1());
            a.Children.Add(match(Token_Class.T_Semicolon));
            return a ;
        }
        // read statement
        Node Read_statement()
        {
            Node a = new Node("Read_statement");
            a.Children.Add(match(Token_Class.T_Read));
            a.Children.Add(match(Token_Class.T_Idenifier));
            a.Children.Add(match(Token_Class.T_Semicolon));

            return a;
        }
        // return statement
        Node Return_statement()
        {
            Node a = new Node("Return_statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Return)
            {
                a.Children.Add(match(Token_Class.T_Return));
                a.Children.Add(Expression());
                a.Children.Add(match(Token_Class.T_Semicolon));
            }
            return a;
        }
        // condition
        Node condition()
        {
            Node a = new Node("condition");
            a.Children.Add(match(Token_Class.T_Idenifier));

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_EqualOp)
                a.Children.Add(match(Token_Class.T_EqualOp));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_NotEqualOp)
                a.Children.Add(match(Token_Class.T_NotEqualOp));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_LessThanOp)
                a.Children.Add(match(Token_Class.T_LessThanOp));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_GreaterThanOp)
                a.Children.Add(match(Token_Class.T_GreaterThanOp));
            a.Children.Add(Term());

            return a;
        }
        // condition stetment
        Node cond1()
        {
            Node a = new Node("cond1");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_AndOp || TokenStream[InputPointer].token_type == Token_Class.T_OROp))
            {
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_AndOp)
                    a.Children.Add(match(Token_Class.T_AndOp));
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_OROp)
                    a.Children.Add(match(Token_Class.T_OROp));
                a.Children.Add(condition());
                a.Children.Add(cond1());

                return a;
            }

            else return null;
        }
        Node condition_staement()
        {
            Node a = new Node("condition_staement");
            a.Children.Add(condition());
            a.Children.Add(cond1());

            return a;
        }
        // statement
        Node statement()
        {
            Node a = new Node("statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Write)
            {
                a.Children.Add(Write_statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Read)
            {
                a.Children.Add(Read_statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Idenifier)
            {
                a.Children.Add(Assinment_statement());
            }
            /*  else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Return)
              {
                  a.Children.Add(Return_statement());
              }*/
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_If)
            {
                a.Children.Add(If_Statement());
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Int_DataType || TokenStream[InputPointer].token_type == Token_Class.T_Float_DataType || TokenStream[InputPointer].token_type == Token_Class.T_String_DataType))
            {
                a.Children.Add(Declaration_statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Repeat)
            {
                a.Children.Add(Repeat_statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_comment)
            {
                //comment Statement
                 a.Children.Add(match(Token_Class.T_comment));
            }
            return a;
        }
        // statements
        Node statementss()
        {
            Node a = new Node("statements");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Int_DataType ||
                TokenStream[InputPointer].token_type == Token_Class.T_String_DataType || TokenStream[InputPointer].token_type == Token_Class.T_Float_DataType
                || TokenStream[InputPointer].token_type == Token_Class.T_Write || TokenStream[InputPointer].token_type == Token_Class.T_Read
                || /*TokenStream[InputPointer].token_type == Token_Class.T_Return ||*/ TokenStream[InputPointer].token_type == Token_Class.T_If
                || TokenStream[InputPointer].token_type == Token_Class.T_Repeat || TokenStream[InputPointer].token_type == Token_Class.T_comment
                || TokenStream[InputPointer].token_type == Token_Class.T_Idenifier))
            {
                a.Children.Add(statement());
                a.Children.Add(statementss());
                return a;
            }
            else
            {
                return null;
            }
           

        }
        // if statement
       
       
        Node If_Statement()
        {
            Node a = new Node("If_Statement");
            a.Children.Add(match(Token_Class.T_If));
            a.Children.Add(condition_staement());
            a.Children.Add(match(Token_Class.T_Then));
            a.Children.Add(statementss());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Elseif)
            {
                a.Children.Add(D1());
            }
             if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Else))
            {
                a.Children.Add(Else_statement());
            }
            a.Children.Add(match(Token_Class.T_End));

            return a;
        }
       /* Node F1()
        {
            Node a = new Node("F1");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Else))
            {
                a.Children.Add(Else_statement());
            }
            else
            {
                return null;
            }
            return a;
        }*/
        // else if statement
        Node ElseIf_statement()
        {
            Node a = new Node("ElseIf_statement");
           
                a.Children.Add(match(Token_Class.T_Elseif));
                a.Children.Add(condition_staement());
                a.Children.Add(match(Token_Class.T_Then));
                a.Children.Add(statementss());
            //    a.Children.Add(ElseIf_statement());
                return a; 
           
        }
        Node D1()
        {
            Node a = new Node("D1");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Elseif)
            {
                a.Children.Add(ElseIf_statement());
                a.Children.Add(D1());
                return a;
            }
            else
            {
                return null;
            }
           
        }
        // else statement
        Node Else_statement()
        {
            Node a = new Node("Else_statement");
            a.Children.Add(match(Token_Class.T_Else));
            a.Children.Add(statementss());
            return a;
        }
        // function name
        Node Function_name()
        {
            Node a = new Node("Function name");
            a.Children.Add(match(Token_Class.T_Idenifier));
            return a;
        }
        // repeat statement
        Node Repeat_statement()
        {
            Node a = new Node("Repeat_statement");
            a.Children.Add(match(Token_Class.T_Repeat));
            a.Children.Add(statementss());
            a.Children.Add(match(Token_Class.T_Until));
            a.Children.Add(condition_staement());
            return a;
        }

        // parameter
        Node parameter()
        {
            Node a = new Node("parameter");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Int_DataType || TokenStream[InputPointer].token_type == Token_Class.T_Float_DataType || TokenStream[InputPointer].token_type == Token_Class.T_String_DataType))
            {
                a.Children.Add(Data_type());
                a.Children.Add(match(Token_Class.T_Idenifier));
            }
            a.Children.Add(fun_param());
            return a;
        }

        // function declaration
        Node fun_param()
        {
            Node a = new Node("func_param");
            if (InputPointer < TokenStream.Count &&  TokenStream[InputPointer].token_type  == Token_Class.T_Comma)
                {
                a.Children.Add(match(Token_Class.T_Comma));
                a.Children.Add(parameter());
                a.Children.Add(fun_param());
                return a;
            }
            else
                return null;
           
        }

        Node function_declaration()
        {
            Node a = new Node("function_declaration");
            a.Children.Add(Data_type());
            a.Children.Add(Function_name());
            a.Children.Add(match(Token_Class.T_LParanthesis));
            a.Children.Add(parameter());
            a.Children.Add(match(Token_Class.T_RParanthesis));
            return a;
        }

        // function body 
        Node function_body()
        {
            Node a = new Node("function_body");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Leftbracket)
            {
                a.Children.Add(match(Token_Class.T_Leftbracket));
                a.Children.Add(statementss());
                a.Children.Add(Return_statement());
                a.Children.Add(match(Token_Class.T_Rightbracket));
            }
                return a;
        }

        // function statement 
        Node function_statement()
        {
            Node a = new Node("function_statement");
            a.Children.Add(function_declaration());
            a.Children.Add(function_body());
            return a;
        }

        // main function
        Node main_function()
        {
            Node a = new Node("main_function");
            a.Children.Add(Data_type());
            a.Children.Add(match(Token_Class.T_Main));
            a.Children.Add(match(Token_Class.T_LParanthesis));
            a.Children.Add(match(Token_Class.T_RParanthesis));
            a.Children.Add(function_body());
            return a;
        }

        // program
        Node prog()
        {
            Node a = new Node("prog");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_Int_DataType || TokenStream[InputPointer].token_type == Token_Class.T_Float_DataType || TokenStream[InputPointer].token_type == Token_Class.T_String_DataType) 
                && TokenStream[InputPointer+1].token_type != Token_Class.T_Main)
            {
                a.Children.Add(function_statement());
                a.Children.Add(prog());
                return a;
            }
            else
                return null;
        }

        Node progra()
        {
            Node a = new Node("program");
            
                    a.Children.Add(prog());
                    a.Children.Add(main_function());
            
            return a;
        }
            // Implement your logic here

            public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
