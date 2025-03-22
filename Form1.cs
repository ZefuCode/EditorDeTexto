using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq.Expressions;
using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;

namespace EditorTextos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            compilarSoluciónToolStripMenuItem.Enabled = false;
            //inicializa la opcion de compilar como inhabilitada.
        }
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog VentanaAbrir = new OpenFileDialog
            {
                Filter = "Texto|*.c"
            };
            if (VentanaAbrir.ShowDialog() == DialogResult.OK)
            {
                archivo = VentanaAbrir.FileName;
                using (StreamReader Leer = new StreamReader(archivo))
                {
                    richTextBox1.Text = Leer.ReadToEnd();
                }

            }
            Form1.ActiveForm.Text = "Mi Compilador - " + archivo;
            compilarSoluciónToolStripMenuItem.Enabled = true;
            //habilita la opcion compilar cuando se carga un archivo.
        }
        private void Guardar()
        {
            SaveFileDialog VentanaGuardar = new SaveFileDialog
            {
                Filter = "Texto|*.c"
            };
            if (archivo != null)
            {
                using (StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
            }
            else
            {
                if (VentanaGuardar.ShowDialog() == DialogResult.OK)
                {
                    archivo = VentanaGuardar.FileName;
                    using (StreamWriter Escribir = new StreamWriter(archivo))
                    {
                        Escribir.Write(richTextBox1.Text);
                    }
                }
            }
            Form1.ActiveForm.Text = "Mi Compilador - " + archivo;
        }
        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Guardar();

        }
        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            archivo = null;
        }
        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog VentanaGuardar = new SaveFileDialog
            {
                Filter = "Texto|*.c"
            };
            if (VentanaGuardar.ShowDialog() == DialogResult.OK)
            {
                archivo = VentanaGuardar.FileName;
                using (StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
            }
            Form1.ActiveForm.Text = "Mi Compilador - " + archivo;
        }
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            compilarSoluciónToolStripMenuItem.Enabled = true;
            //habilita la opcion compilar cuando se realiza un cambio en el texto.
        }

        //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////funciones del analisis lexico/////////////////////////
        ///
        private char Tipo_caracter(int caracter)
        {
            if (caracter >= 65 && caracter <= 90 || caracter >= 97 && caracter <= 122) { return 'l'; } //letra 
            else
            {
                if (caracter >= 48 && caracter <= 57) { return 'd'; } //digito 
                else
                {
                    switch (caracter)
                    {
                        case 10: return 'n'; //salto de linea
                        case 34: return '"';//inicio de cadena
                        case 39: return 'c';//inicio de caracter
                        case 47: return '/';//inicio de comentario de linea o de bloque
                        case 32: return 'e';//espacio
                        default: return 's';//simbolo
                    }

                }
            }

        }
        private void Simbolo()
        {
            if (i_caracter == 33 ||
                i_caracter >= 35 && i_caracter <= 38 ||
                i_caracter >= 40 && i_caracter <= 45 ||
                i_caracter >= 58 && i_caracter <= 62 ||
                i_caracter == 91 ||
                i_caracter == 93 ||
                i_caracter == 94 ||
                i_caracter == 123 ||
                i_caracter == 124 ||
                i_caracter == 125
                ) { elemento = elemento + (char)i_caracter + "\n"; } //simbolos validos 
            else { Error(i_caracter); }
        }
        private void Cadena()
        {
            do
            {
                i_caracter = Leer.Read();
                if (i_caracter == 10) Numero_linea++;
            } while (i_caracter != 34 && i_caracter != -1);
            if (i_caracter == -1) Error(34);
        }
        private void Caracter()
        {
            i_caracter = Leer.Read();
            //programar para los casos donde el caracter se imprime  '\n','\r','\t' etc.
            i_caracter = Leer.Read();
            if (i_caracter != 39) Error(39);
        }
        private void Error(int i_caracter)
        {
            Rtbx_salida.AppendText("Error léxico " + (char)i_caracter + ", línea " + Numero_linea + "\n");
            N_error++;
        }
        private void Archivo_Libreria()
        {
            i_caracter = Leer.Read();
            if ((char)i_caracter == 'h') { elemento = "Libreria\n"; i_caracter = Leer.Read(); }
            else { Error(i_caracter); }
        }
        private bool Palabra_Reservada()
        {
            if (P_Reservadas.IndexOf(elemento) >= 0) return true;
            return false;
        }
        private void Identificador()
        {
            do
            {
                elemento += (char)i_caracter;
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'l' || Tipo_caracter(i_caracter) == 'd');
            if (Palabra_Reservada()) elemento += "\n";
            else
            {
                switch (i_caracter)
                {
                    case '.': Archivo_Libreria(); break;
                    case '(': elemento = "funcion\n"; break;
                    default: elemento = "identificador\n"; break;
                }
            }
        }
        private bool Comentario()
        {
            i_caracter = Leer.Read();
            switch (i_caracter)
            {
                case 47:
                    do
                    {
                        i_caracter = Leer.Read();
                    } while (i_caracter != 10);
                    return true;
                case 42:
                    do
                    {
                        do
                        {
                            i_caracter = Leer.Read();
                            if (i_caracter == 10) { Numero_linea++; }
                        } while (i_caracter != 42 && i_caracter != -1);
                        i_caracter = Leer.Read();
                    } while (i_caracter != 47 && i_caracter != -1);
                    if (i_caracter == -1) { Error(i_caracter); }
                    i_caracter = Leer.Read();
                    return true;
                default: return false;
            }
        }
        private void Numero_Real()
        {
            do
            {
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'd');
            elemento = "numero_real\n";
        }
        private void Numero()
        {
            do
            {
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'd');
            if ((char)i_caracter == '.') { Numero_Real(); }
            else
            {
                elemento = "numero_entero\n";
            }
        }
        ///////////////////Inicio del analisis léxico////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        private void compilarSoluciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rtbx_salida.Text = "Analizando...\n";
            Guardar();
            elemento = "";
            N_error = 0;
            Numero_linea = 1;
            archivoback = archivo.Remove(archivo.Length - 1) + "back";
            Escribir = new StreamWriter(archivoback);
            Leer = new StreamReader(archivo);
            i_caracter = Leer.Read();
            do
            {
                elemento = "";
                switch (Tipo_caracter(i_caracter))
                {
                    case 'l': Identificador(); Escribir.Write(elemento); break;
                    case 'd': Numero(); Escribir.Write(elemento); break;
                    case 's': Simbolo(); Escribir.Write(elemento); i_caracter = Leer.Read(); break;
                    case '"': Cadena(); Escribir.Write("cadena\n"); i_caracter = Leer.Read(); break;
                    case 'c': Caracter(); Escribir.Write("caracter\n"); i_caracter = Leer.Read(); break;
                    case '/': if (Comentario()) { Escribir.Write("comentario\n"); } else { Escribir.Write("/\n"); } break;
                    case 'n': i_caracter = Leer.Read(); Numero_linea++; Escribir.Write("LF\n"); break;
                    case 'e': i_caracter = Leer.Read(); break;
                    default: Error(i_caracter); break;
                };

            } while (i_caracter != -1);
            Escribir.Write("Fin");
            Escribir.Close();
            Leer.Close();
            if (N_error == 0) { Rtbx_salida.AppendText("Errores Lexicos: " + N_error); A_Sintactico(); }
            else { Rtbx_salida.AppendText("Errores: " + N_error); }
        }

        //////////////////////////////////////////////////////////////////////////
        ////////////////////Funciones del análisis sintáctico///////////////////////////////////
        private void ErrorS(string tokenActual, string esperado)
        {
            Rtbx_salida.AppendText("Linea: " + Numero_linea + ". Error de sintaxis: encontrado '" + tokenActual + "', se esperaba '" + esperado + "'\n");
            N_error++;
        }


        //----------------------------------------------------------------------------
        private void Include()
        {
            token = Leer.ReadLine();
            switch (token)
            {
                case "<":
                    token = Leer.ReadLine();
                    if (token == "Libreria")
                    {
                        token = Leer.ReadLine();
                        if (token == ">")
                        {
                            token = Leer.ReadLine();
                        }
                        else { ErrorS(token, ">"); N_error++; }
                    }
                    else { ErrorS(token, "nombre de archivo libreria"); N_error++; }
                    break;
                case "cadena": token = Leer.ReadLine(); break;
                //case "identificador": token = Leer.ReadLine(); break;
                default: ErrorS(token, "inclusión valida "); N_error++; break;
            }
        }
        //--------------------------------------------------------------------------
        private void Directriz()
        {
            token = Leer.ReadLine();
            switch (token)
            {
                case "include": Include(); break;
                case "define"://estructura para directriz #define 
                    break;
                case "if":    //estructura para directriz #if
                    break;
                case "error":    //estructura para directriz #error
                    break;
                // misma forma para las restantes directivas de procesador,
                default: ErrorS(token, "directriz de procesador"); break; ;
            }
        }
        //---------------------------------------------------------------------------
        private int Constante()
        {
            token = Leer.ReadLine();
            switch (token)
            {
                case "numero_real": return 1;
                case "numero_entero": return 1;
                case "caracter": return 1;
                case "identificador": return 1;
                default: return 0;
            }
        }
        //-----------------------------------------------------------------------------
        private void Bloque_Inicializacion()
        {
            do
            {
                token = Leer.ReadLine();
                if (token == "{")
                {
                    do
                    {
                        if (Constante() == 1) { token = "elemento"; }
                        switch (token)
                        {
                            case "elemento": token = Leer.ReadLine(); break;
                            case "{":
                                do
                                {
                                    if (Constante() == 0) { ErrorS(token, " inicializacion valida de arreglo."); }
                                    else { token = Leer.ReadLine(); }
                                } while (token == ",");
                                if (token == "}") { token = Leer.ReadLine(); }
                                else { ErrorS(token, "}"); }
                                break;
                        }
                    } while (token == ",");
                    if (token == "}") { token = Leer.ReadLine(); }
                    else { ErrorS(token, "}"); }
                }
                else { ErrorS(token, "{"); }
            } while (token == ",");
        }
        //-------------------------------------------------------------------------------
        private void D_Arreglos()
        {
            while (token == "[")
            {
                token = Leer.ReadLine();
                if (token == "identificador" || token == "numero_entero")
                {
                    token = Leer.ReadLine();
                    if (token == "]") { token = Leer.ReadLine(); }
                    else { ErrorS(token, "]"); }
                }
                else ErrorS(token, "valor de longitud");
            }
            switch (token)
            {
                case ";": token = Leer.ReadLine(); break;
                case "=":
                    token = Leer.ReadLine();
                    if (token == "{")
                    {
                        Bloque_Inicializacion();
                        if (token == "}")
                        {
                            token = Leer.ReadLine();
                            if (token == ";") { token = Leer.ReadLine(); }
                            else { ErrorS(token, ";"); }
                        }
                        else { ErrorS(token, "}"); }
                    }
                    else { ErrorS(token, "{"); }
                    break;
                default: ErrorS(token, "declaracion valida para arreglos."); break;
            }
        }
        //----------------------------------------------------------------------------
        private void Dec_VGlobal() //se ha leido tipo e identificador
        {
            token = Leer.ReadLine();
            switch (token)
            {
                case "=":
                    if (Constante() == 1)
                    {
                        token = Leer.ReadLine();
                        if (token == ";") { token = Leer.ReadLine(); }
                        else { ErrorS(token, ";"); }
                    }
                    else { ErrorS(token, "inicializacion global valida"); }
                    break;
                case "[": D_Arreglos(); break;
                case ";": token = Leer.ReadLine(); break;
                default: ErrorS(token, ";"); break;
            }
        }
        //--------------------------------------------------------------------------
        private void Declaracion()
        {
            switch (token)
            {
                case "identificador": Dec_VGlobal(); break;
                case "funcion": Dec_Funcion(); break;
                default: ErrorS(token, "declaracion global valida"); break;
            }
        }
        
        
        private void Dec_Funcion() { }
        private void F_Main() {
            token = Leer.ReadLine();
            if (token == "{"){
                token = Leer.ReadLine();
                if (token == ")")
                {
                    token = Leer.ReadLine();
                    Bloque();
                }
                else ErrorS(token, ")");

            }
            else ErrorS(token, "(");
        }
        

        
        //-------------------------------------------------------------------------
        private int Cabecera()
        {
            token = Leer.ReadLine();
            do
            {
                if (P_Res_Tipo.IndexOf(token) >= 0) { token = "tipo"; }
                switch (token)
                {    //en este caso practico solamente se considera la directiva #include
                    case "#": Directriz(); break;
                    case "tipo":
                        token = Leer.ReadLine();
                        if (token == "main") return 1;
                        else Declaracion();
                        break;
                    case "comentario": token = Leer.ReadLine(); break;
                    case "typedef": //estructura typedef
                        break;
                    case "const": //estrucutura const
                        break;
                    case "extern": //estrucutura extern
                        break;
                    case "": token = Leer.ReadLine(); break;
                    case "LF": Numero_linea++; token = Leer.ReadLine(); break;
                    default: token = Leer.ReadLine(); break;

                }
            } while (token != "Fin" && token != "main");
            return 0;
        }
        //////////Inicio del análisis sintáctico //////////
        private void A_Sintactico()
        {
            Rtbx_salida.AppendText("\nAnalizando sintaxis...\n");
            N_error = 0;
            Numero_linea = 1;
            Leer = new StreamReader(archivoback);

            LeerSiguienteToken();
            if (Cabecera() == 1)
            {
                F_Main();
            }
            else
            {
                ErrorS(token, "funcion main()");
            }

            Rtbx_salida.AppendText("Errores sintácticos: " + N_error);
            Leer.Close();
        }


        private void LeerSiguienteToken()
        {
            token = Leer.ReadLine()?.Trim(); 
        }

        private void IfStatement()
        {
            LeerSiguienteToken();
            if (token == "(")
            {
                Expresion();
                if (token == ")")
                {

                    Bloque();
                    LeerSiguienteToken();
                    if (token == "else")
                    {

                        Bloque();


                    }

                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            else
            {
                ErrorS(token, "(");
            }
        }




        private void ForLoop()
        {
            LeerSiguienteToken();
            if (token == "(")
            {
                Expresion(); 
                if (token == ";")
                {
                    Expresion(); 
                    if (token == ";")
                    {
                        Expresion(); 
                        if (token == ")")
                        {
                            Bloque();
                        }
                        else
                        {
                            ErrorS(token, ")");
                        }
                    }
                    else
                    {
                        ErrorS(token, ";");
                    }
                }
                else
                {
                    ErrorS(token, ";");
                }
            }
            else
            {
                ErrorS(token, "(");
            }
        }
    

    private void WhileLoop()
        {
            LeerSiguienteToken();
            if (token == "(")
            {
                Expresion();
                if (token == ")")
                {
                    LeerSiguienteToken();
                    if (token == "{")
                    {
                        Bloque();
                    }
                    else
                    {
                        ErrorS(token, "{");
                    }
                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            else
            {
                ErrorS(token, "(");
            }
        }


        private void DoWhileLoop()
        {

            Bloque();
            LeerSiguienteToken();
            if (token == "while")
            {
                LeerSiguienteToken();
                if (token == "(")
                {
                    Expresion();
                    if (token == ")")
                    {
                        LeerSiguienteToken();
                        if (token != ";")
                        {
                            ErrorS(token, ";");
                        }
                    }
                    else
                    {
                        ErrorS(token, ")");
                    }
                }
                else
                {
                    ErrorS(token, "(");
                }
            }
            else
            {
                ErrorS(token, "while");
            }

        }


        private void SwitchStatement()
        {
            LeerSiguienteToken();
            if (token == "(")
            {
                Expresion();
                if (token == ")")
                {
                    Bloque();
                    do
                    {
                        LeerSiguienteToken();
                        switch (token)
                        {
                            case "case":
                                Constante();
                                LeerSiguienteToken();
                                if (token == ":")
                                {
                                    Bloque();
                                }
                                else
                                {
                                    ErrorS(token, ":");
                                }
                                break;
                            case "default":
                                LeerSiguienteToken();
                                if (token == ":")
                                {
                                    Bloque();
                                }
                                else
                                {
                                    ErrorS(token, ":");
                                }
                                break;
                            case "}":
                                return;
                            default:
                                ErrorS(token, "case o default");
                                break;
                        }
                    } while (token != "}");


                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            else
            {
                ErrorS(token, "(");
            }
        }

        private void AnalisisSintactico()
        {
            LeerSiguienteToken();
            switch (token)
            {
                case "if":
                    IfStatement();
                    break;
                case "for":
                    ForLoop();
                    break;
                case "while":
                    WhileLoop();
                    break;
                case "do":
                    DoWhileLoop();
                    break;
                case "switch":
                    SwitchStatement();
                    break;
                default:
                    ErrorS(token, "estructura de control válida");
                    break;
            }
        }


        private void Expresion()
        {
            Termino();

            while (token == "+" || token == "-" || token == "==" || token == "!=" || token == "<" || token == ">" || token == "<=" || token == ">=")
            {
                LeerSiguienteToken(); 
                Termino(); 
            }
        }


        private void Bloque()
        {
            LeerSiguienteToken();
            if (token == "{")
            {
                while ((token = Leer.ReadLine()?.Trim()) != null && token != "}")
                {
                    Sentencias(); 
                }

                if (token != "}") 
                {
                    ErrorS(token, "}"); 
                }
            }
            else
            {
                ErrorS(token, "{");
            }
        }


        private void InvocacionFuncion()
        {
            LeerSiguienteToken();
            if (token == "(")
            {
                Parametros(); 
                if (token == ")")
                {
                    LeerSiguienteToken();
                    if (token != ";") 
                    {
                        ErrorS(token, ";");
                    }
                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            else
            {
                ErrorS(token, "(");
            }
        }

        private void Parametros()
        {
            do
            {
                LeerSiguienteToken();
                if (token == "identificador" || token == "numero_entero" || token == "numero_real")
                {
                    LeerSiguienteToken();
                }
                else
                {
                    ErrorS(token, "identificador o constante válido");
                    break;
                }

                
            } while (token == ",");
        }

        private void Sentencias()
        {
            LeerSiguienteToken();
            if (token == "identificador")
            {
                LeerSiguienteToken();
                if (token == "=") 
                {
                    Asignacion(); 
                }
                else if (token == "(") 
                {
                    InvocacionFuncion();
                }
                else
                {
                    ErrorS(token, "= o invocación de función válida");
                }
            }
            else if (P_Res_Tipo.Contains(token)) 
            {
                Declaracion();
            }
            else
            {
                ErrorS(token, "declaración o asignación válida");
            }
        }


        private void Asignacion()
        {
            LeerSiguienteToken();
            if (token == "=")
            {
                LeerSiguienteToken(); 
                Expresion(); 
                if (token == ";")
                {
                    LeerSiguienteToken();
                }
                else
                {
                    ErrorS(token, ";"); 
                }
            }
            else
            {
                ErrorS(token, "="); 
            }
        }


        private void Termino()
        {
            Factor(); 

            while (token == "*" || token == "/")
            {
                LeerSiguienteToken();
                Factor(); 
            }
        }

        private void Factor()
        {
            if (token == "identificador" || token == "numero_entero" || token == "numero_real")
            {
                LeerSiguienteToken(); 
            else if (token == "(")
            {
                LeerSiguienteToken();
                Expresion(); 
                if (token == ")")
                {
                    LeerSiguienteToken();
                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            else
            {
                ErrorS(token, "identificador, número o expresión válida");
            }
        }


    }
}