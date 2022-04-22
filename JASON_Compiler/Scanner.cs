using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


public enum Token_Class
{
    T_Int_DataType, T_Float_DataType, T_String_DataType, T_Read, T_Write, T_If, T_Then, T_Return, T_Endl,
     T_Semicolon, T_Comma, T_LParanthesis, T_RParanthesis, T_EqualOp, T_LessThanOp, T_Repeat, T_Until,
    T_GreaterThanOp, T_NotEqualOp, T_PlusOp, T_MinusOp, T_MultiplyOp, T_DivideOp, T_String, T_comment, T_assign,
    T_Idenifier, T_Rightbracket, T_Leftbracket, T_AndOp, T_OROp, T_Number, T_Main, T_End, T_Elseif, T_Else
}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Arithmatic_Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Logical_Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> conditional_Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.T_If);
            ReservedWords.Add("else", Token_Class.T_Else);
            ReservedWords.Add("int", Token_Class.T_Int_DataType);
            ReservedWords.Add("float", Token_Class.T_Float_DataType);
            ReservedWords.Add("string", Token_Class.T_String_DataType);
            ReservedWords.Add("main", Token_Class.T_Main);
            ReservedWords.Add("end", Token_Class.T_End);
            ReservedWords.Add("read", Token_Class.T_Read);
            ReservedWords.Add("write", Token_Class.T_Write);
            ReservedWords.Add("repeat", Token_Class.T_Repeat);
            ReservedWords.Add("until", Token_Class.T_Until);
            ReservedWords.Add("elseif", Token_Class.T_Elseif);
            ReservedWords.Add("then", Token_Class.T_Then);
            ReservedWords.Add("return", Token_Class.T_Return);
            ReservedWords.Add("endl", Token_Class.T_Endl);


            Operators.Add(";", Token_Class.T_Semicolon);
            Operators.Add(",", Token_Class.T_Comma);
            Operators.Add("(", Token_Class.T_LParanthesis);
            Operators.Add(")", Token_Class.T_RParanthesis);
            Operators.Add(":=", Token_Class.T_assign);
            Operators.Add("{", Token_Class.T_Leftbracket);
            Operators.Add("}", Token_Class.T_Rightbracket);


            Logical_Operators.Add("&&", Token_Class.T_AndOp);
            Logical_Operators.Add("||", Token_Class.T_OROp);


            Arithmatic_Operators.Add("+", Token_Class.T_PlusOp);
            Arithmatic_Operators.Add("-", Token_Class.T_MinusOp);
            Arithmatic_Operators.Add("*", Token_Class.T_MultiplyOp);
            Arithmatic_Operators.Add("/", Token_Class.T_DivideOp);


            conditional_Operators.Add("=", Token_Class.T_EqualOp);
            conditional_Operators.Add("<", Token_Class.T_LessThanOp);
            conditional_Operators.Add(">", Token_Class.T_GreaterThanOp);
            conditional_Operators.Add("<>", Token_Class.T_NotEqualOp);
        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;
                //comment                
                if (CurrentChar == '/' && j + 1 <= SourceCode.Length - 1 && SourceCode[j + 1] == '*')
                {
                    string s = "";
                    while (true)
                    {
                        s += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length) break;
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '*' && j + 1 <= SourceCode.Length - 1 && SourceCode[j + 1] == '/') break;
                    }
                    if (CurrentChar == '*' && j + 1 <= SourceCode.Length - 1 && SourceCode[j + 1] == '/')
                    {
                        s += CurrentChar.ToString();
                        j++;
                        CurrentChar = SourceCode[j];
                        s += CurrentChar.ToString();
                        FindTokenClass(s);
                    }
                    else
                    {
                        FindTokenClass(s);
                    }
                    i = j;
                }
                // reserved or Idenifier 
                else if (char.IsLetter(CurrentChar))
                {
                    string s = "";
                    while (char.IsLetterOrDigit(CurrentChar))
                    {
                        s += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length) break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(s);
                    i = j - 1;
                }
                // number
                else if (char.IsDigit(CurrentChar))
                {
                    string s = "";
                    while (char.IsDigit(CurrentChar) || CurrentChar == '.')
                    {

                        s += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length) break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(s);
                    i = j - 1;
                }
                // string
                else if (CurrentChar == '"')
                {
                    char c = '"';
                    string s = c.ToString();
                    j++;
                    if (j >= SourceCode.Length)
                    {
                        FindTokenClass(s);
                        break;
                    }
                    CurrentChar = SourceCode[j];
                    while (CurrentChar != '"')
                    {
                        s += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length) break;
                        CurrentChar = SourceCode[j];
                    }
                    if (CurrentChar == '"')
                    {
                        s += c.ToString();
                        FindTokenClass(s);
                    }
                    else
                    {
                        FindTokenClass(s);
                    }
                    i = j;
                }
                //opertor
                else if (j + 1 <= SourceCode.Length - 1 && CurrentChar == '&' && SourceCode[j + 1] == '&' || j + 1 <= SourceCode.Length - 1 && CurrentChar == '|' && SourceCode[j + 1] == '|' || j + 1 <= SourceCode.Length - 1 && CurrentChar == ':' && SourceCode[j + 1] == '=' || j + 1 <= SourceCode.Length - 1 && CurrentChar == '<' && SourceCode[j + 1] == '>')
                {
                    string s = "";
                    s += CurrentChar.ToString();
                    j++;
                    CurrentChar = SourceCode[j];
                    s += CurrentChar.ToString();
                    FindTokenClass(s);
                    i = j;
                }
                else if (!char.IsLetterOrDigit(CurrentChar))
                {
                    string s = "";
                    s += CurrentChar.ToString();
                    FindTokenClass(s);
                    i = j;
                }
            }
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
        //    Token_Class TC;
            Token Tok = new Token();

            Tok.lex = Lex;
            Regex r2 = new Regex(@"^\d+(\.\d+)?$", RegexOptions.Compiled);
            Regex r3 = new Regex(@"\/\*.*|\n\*\/", RegexOptions.Compiled);

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            else if ((Lex[0] >= 'a' && Lex[0] <= 'z') || (Lex[0] >= 'A' && Lex[0] <= 'Z'))
            {
                bool check = true;
                for (int i = 1; i < Lex.Length; i++)
                {
                    if ((Lex[i] >= 'a' && Lex[i] <= 'z') || (Lex[i] >= 'A' && Lex[i] <= 'Z') || (Lex[i] >= '0' && Lex[i] <= '9'))
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                        break;
                    }
                }
                if (check == true)
                {
                    Tok.token_type = Token_Class.T_Idenifier;
                    Tokens.Add(Tok);
                }
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (Arithmatic_Operators.ContainsKey(Lex))
            {
                Tok.token_type = Arithmatic_Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (Logical_Operators.ContainsKey(Lex))
            {
                Tok.token_type = Logical_Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (conditional_Operators.ContainsKey(Lex))
            {
                Tok.token_type = conditional_Operators[Lex];
                Tokens.Add(Tok);
            }
            // number 
            else if (r2.IsMatch(Lex))
            {
                Tok.token_type = Token_Class.T_Number;
                Tokens.Add(Tok);
            }
            // string
            else if (Lex[0] == '"' && Lex[Lex.Length - 1] == '"' && Lex.Length > 1)
            {
                Tok.token_type = Token_Class.T_String;
                Tokens.Add(Tok);
            }
            //comment
            else if (r3.IsMatch(Lex))
            {
                Tok.token_type = Token_Class.T_comment;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add("Unrecognized token" + '\t' + Lex);
            }

        }
    }
}

