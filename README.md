Descripción

Este proyecto es un editor de texto con funcionalidades de análisis léxico y sintáctico para código en lenguaje C. Permite abrir, editar, guardar y compilar archivos con extensión .c, proporcionando retroalimentación sobre errores léxicos y sintácticos.

Características

Abrir y guardar archivos .c

Edición de código mediante un RichTextBox

Análisis léxico de palabras clave, identificadores, operadores y símbolos

Detección de errores léxicos y sintácticos

Habilitación/deshabilitación del botón de compilación según cambios en el texto

Dependencias

El programa está desarrollado en C# utilizando Windows Forms. Algunas dependencias clave incluyen:

System.IO para operaciones de archivos

System.Windows.Forms para la interfaz gráfica

Estructura del Código

El archivo Form1.cs contiene la lógica del programa, que se divide en:

Funciones de manejo de archivos:

abrirToolStripMenuItem_Click(): Abre un archivo .c y lo carga en el editor.

guardarToolStripMenuItem_Click(): Guarda el archivo actual.

guardarComoToolStripMenuItem_Click(): Guarda el archivo con un nuevo nombre.

Análisis léxico:

Tipo_caracter(): Identifica el tipo de cada carácter.

Identificador(), Numero(), Simbolo(), etc.: Detectan elementos del código.

Error(): Manejo de errores léxicos.

Análisis sintáctico:

A_Sintactico(): Inicia el análisis sintáctico.

IfStatement(), ForLoop(), WhileLoop(), etc.: Manejan estructuras de control.

Expresion(), Termino(), Factor(): Análisis de expresiones matemáticas.

Uso

Ejecutar el programa.

Abrir un archivo .c o crear uno nuevo.

Escribir o modificar código.

Guardar los cambios.

Presionar "Compilar" para analizar el código.

Notas

Actualmente, el programa solo analiza sintaxis y léxico, pero no ejecuta el código.

Se pueden agregar más reglas de análisis para mejorar la detección de errores.

Autor

Desarrollado para un proyecto de análisis sintáctico en C# y Windows Forms.
