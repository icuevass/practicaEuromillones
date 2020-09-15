using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoEuromillones
{    
    public partial class Form1 : Form
    {
        Random aleatorio = new Random();
        int[] vNumUser = new int[6];
        int[] vNumWinner = new int[6];

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //listBoxAc.DrawMode = DrawMode.OwnerDrawFixed;                     // can be set in design mode
            listBoxAc.DrawItem += new DrawItemEventHandler(listBox_DrawItem);
        }

        /*
         * Method which controls the graphical representation of each item of a ListBox 
         * in order to center-align its text. (https://stackoverflow.com/a/2943427)
         */
        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox list = (ListBox)sender;
            object item;
            Brush brush;
            SizeF size;                   
            if (e.Index > -1)
            {
                item = list.Items[e.Index];
                e.DrawBackground();
                e.DrawFocusRectangle();
                brush = new SolidBrush(e.ForeColor);
                size = e.Graphics.MeasureString(item.ToString(), e.Font);
                e.Graphics.DrawString(item.ToString(), e.Font, brush, e.Bounds.Left + (e.Bounds.Width / 2 - size.Width / 2), e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));
            }
        }

        /*
         * Excepción para número introducido fuera del rango establecido [1-50].
         */
        public class RangoException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Número fuera de rango [1-50].";
                }
            }
        }

        /*
         * Excepción para número introducido repetido.
         */
        public class RepetidoException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Número repetido.";
                }
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         * Método que itera sobre los elementos de un vector numérico (parámetro) anteriores 
         * a una posición límite (parámetro) en busca de un número repetido (parámetro) 
         * devolviendo true si lo encuentra y false si no. 
         */
        private bool EsRepe(int[] vNum, int limite, int num)
        {
            for (int i = 0; i < limite; i++)
            {
                if (vNum[i] == num)
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Método que desplaza todos los elementos del vector (parámetro) una posición
         * a la derecha empezando desde una posición concreta (parámetro), para hacer
         * sitio al nuevo valor.
         */
        public static void moveRight(int[] vNum, int posicion)
        {
            int aux, anterior = vNum[posicion];
            for (int i = posicion; i < vNum.Length - 1; i++)
            {
                aux = vNum[i + 1];
                vNum[i + 1] = anterior;
                anterior = aux;
            }
        }

        /*
         * Método que inserta número (parámetro) en vector (parámetro) en posición
         * idónea para mantener orden creciente.
         */
        public static void insOrder(int[] vNum, int valor)
        {
            int i = 0;
            bool situado = false;
            while (!situado && i < vNum.Length)
            {
                if (vNum[i] > valor || vNum[i] == 0) // vector numérico establece por defecto todos sus elementos a 0 en construcción
                {
                    if (vNum[i] != 0) // solamente hace falta mover números una posición a la derecha si posición actual está ocupada por un valor previamente generado y no inicial (0)
                    {
                        moveRight(vNum, i);
                    }             
                    vNum[i] = valor;
                    situado = true;
                }
                i++;
            }
        }

        /*
         * Método que la combinación ganadora del sorteo y la almacena en un vector.
         */
        private void GenerarCombinacion()
        {
            int num;
            for (int i = 0; i < vNumWinner.Length; i++)
            {
                do
                {
                    num = aleatorio.Next(1, 51);                  
                } while (EsRepe(vNumWinner, i, num));
                insOrder(vNumWinner, num); // inserta en vector número generado aleatoriamente (posición ordenada) 
            }
        }

        /*
         * Método que la combinación ganadora del sorteo, la almacena en un vector y 
         * la muestra en el Group Box. 
         */
        public void MostrarCombinacion()
        {
            int i = 0;

            /*
                For Each loop iterates over the GroupBox controls in order of the Z-order of the controls 
                in the same parent container (top-most to bottom-most). Each added control has a Z-order 
                value higher than the rest, increased by one. Z-order and  tab-index order are NOT the same 
                concept, although both are automatically increased  by one on each new item added to the form 
                (tab-index can be set independently from Z-order).
            */

            foreach (Control ctrl in groupBoxWinner.Controls.Cast<Control>().Reverse())                     // esta lógica permite iterar los controles en sentido Z-order inverso
            //foreach (Control ctrl in groupBoxWinner.Controls.Cast<Control>().OrderBy(c => c.TabIndex))    // esta lógica permite iterar los controles específicamente en orden de tabulación (https://stackoverflow.com/a/3302455)
            {
                ctrl.Text = Convert.ToString(vNumWinner[i]);
                i++;
            }

            /*
                Otra solución al problema de la iteración inversa sería intercambiar manualmente la posición
                de cada TextBox en la ventana de diseño (los últimos por los primeros) y aunque ello invertiría
                la secuencia de tabulación, no sería un problema porque se puede modificar manualmente el valor 
                de la variable TabIndex de cada campo.
            */
        }

        /*
         * Método que comprueba la cantidad de números escritos por el usuario que están
         * contenidos en el vector de combinación ganadora.
         */
        private void ComprobarAciertos()
        {
            List<int> listaAciertos = new List<int>();

            /*
                List<T> is a generic class. It supports storing values of a specific type without casting 
                to or from object (which would have incurred boxing/unboxing overhead when T is a value 
                type in the ArrayList case). ArrayList simply stores object references. As a generic 
                collection, List<T> implements the generic IEnumerable<T> interface and can be used easily 
                in LINQ (without requiring any Cast or OfType call).
                
                ArrayList belongs to the days that C# didn't have generics. It's deprecated in favor of 
                List<T>. You shouldn't use ArrayList in new code that targets .NET >= 2.0 unless you have 
                to interface with an old API that uses it.
            */

            int aciertos = 0;
            int i;
            bool encontrado;
            foreach (int n in vNumUser)
            {
                encontrado = false;
                i = 0;
                while (!encontrado && i < vNumWinner.Length)
                {
                    if (vNumWinner[i] == n)
                    {
                        listaAciertos.Add(n);
                        aciertos++;
                        encontrado = true;
                    }
                    i++;
                }
                
            }

            //labelAciertos.Text = "Nº aciertos: " + aciertos + "\n\n" +
            //    (listaAciertos.Count > 0
            //        ? "[ " + string.Join(", ", listaAciertos.Select(x => x.ToString()).ToArray()) + " ]"  // https://stackoverflow.com/a/3610448	
            //        : "");

            labelAciertos.Text = "Nº aciertos: " + aciertos;
            foreach (int n in listaAciertos)
            {
                listBoxAc.Items.Add(n);
            }
            listBoxAc.Visible = true;
        } 
        
        /*
         * Método que limpia el texto de todos los elementos de un Group Box (parámetro).
         */
        private void clearGroupBox(GroupBox gb)
        {
            foreach (Control ctrl in gb.Controls)
            {
                ctrl.Text = "";
            }
        }        

        private void Comprobar_Click(object sender, EventArgs e)
        {
            int num;
            int i = 0;
            labelAciertos.Text = "";
            listBoxAc.Items.Clear();
            listBoxAc.Visible = false;

            /*
                NOTA: no es posible combinar las lógicas de generar y mostrar combinación en un 
                mismo bucle porque estamos ordenando el vector según se va generando, lo que 
                implica que el índice del número del vector insertado puede no corresponderse con 
                el índice actual de iteración.
            */

            GenerarCombinacion();
            MostrarCombinacion();
            try
            {
                foreach (Control ctrl in groupBoxUser.Controls)                 
                {
                    num = Convert.ToInt16(ctrl.Text);
                    if (num < 1 || num > 50) // comprobación de rango
                    {
                        throw new RangoException();
                    }
                    else if (EsRepe(vNumUser, i, num)) // comprobación de repetición
                    {
                        throw new RepetidoException();
                    }
                    else
                    {
                        vNumUser[i] = num; // vector se rellena empezando primero por el último campo (primer elemento del vector es número de campo textBox6) porque la iteración de controles se realiza en sentido inverso al orden de tabulación
                    }           
                    i++;
                }
                ComprobarAciertos();
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Por favor, rellene todos los campos con numeros.");
            }
            catch (RangoException ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Solo puede introducir números del 1 al 50.");
            }
            catch (RepetidoException ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("No puede haber números repetidos.");
            }
            finally
            {
                clearGroupBox(groupBoxUser);
                //textBox1.Focus();
                textBox1.Select();

                /*
                    Focus is a low-level method intended primarily for custom control authors. Select 
                    is a higer-level method. It first looks iteratively upward in the control's parent 
                    hierarchy until it finds a container control. Then it sets that container's 
                    ActiveControl property (to the called control).

                    Application programmers should use the Select method or the ActiveControl property 
                    for child controls, or the Activate method for forms.

                */

                vNumWinner = new int[6]; // genera un nuevo vector vacío (relleno de ceros)
            }                     
        }
    }
}
