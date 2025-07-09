using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
// using System.Data.SqlClient; // <--- Esta línea ya no es necesaria
using MySql.Data.MySqlClient; // <--- ¡Esta es la que necesitas para MySQL!
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PracticaAdoNet
{
    public partial class NuevoCliente : Form
    {
        Form padre;
        private int parsedCustomerID;
        private int orderID;

        // Metodo que revisa si el nombre del cliente es valido.
        // Verifica que no sea nombre vacio.
        private bool isCustomerNameValid()
        {
            bool bValid;
            if (!(bValid = txtCustomerName.Text != ""))
            {
                MessageBox.Show("Porfavor ingresa un nombre.");
            }
            return bValid;
        }

        private bool isOrderDataValid()
        {
            if (txtCustomerID.Text == "")
            {
                MessageBox.Show("Porfavor crea una cuenta antes de\n" +
                    "agregar una orden");
                return false;
            }
            if (numOrderAmount.Value < 1)
            {
                MessageBox.Show("Porfavor especifica la cantidad.");
                return false;
            }
            return true;
        }

        private void cleanForm()
        {
            txtCustomerName.Clear();
            txtCustomerID.Clear();
            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;
            this.parsedCustomerID = 0;
        }

        public NuevoCliente(Form padre)
        {
            this.padre = padre;
            InitializeComponent();
        }

        private void NuevoCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.padre.Show();
        }

        // Crea un nuevo cliente llamando Sales.uspNewCustomer stored procedure.
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            // vemos si el nombre del cliente es valido
            if (isCustomerNameValid())
            {

                // crearemos variables temporales para solo un uso
                // Se crea la conexion
                using (MySqlConnection connection = // <--- Cambiado a MySqlConnection
                    new MySqlConnection(Properties.Settings.Default.connString))
                {

                    // Creamos el comando a ejecutar
                    using (MySqlCommand mySqlCommand = // <--- Cambiado a MySqlCommand
                        new MySqlCommand("uspNewCustomer", connection))
                    { 

                        // Cuerpo
                        mySqlCommand.CommandType = CommandType.StoredProcedure;

                        // Agregaremos un parametro de entrada, esto con el fin
                        // de guardar el procedimiento y especificar que se usara
                        // como su valor
                        mySqlCommand.Parameters.Add(new MySqlParameter( // <--- Cambiado a MySqlParameter
                            "@p_CustomerName", MySqlDbType.VarChar, 40)); // <--- Cambiado el nombre a @p_CustomerName
                        mySqlCommand.Parameters["@p_CustomerName"].Value = txtCustomerName.Text; // <--- Cambiado el nombre

                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_CustomerID", MySqlDbType.Int32)); // <--- Cambiado el nombre a @p_CustomerID
                        // Y agregamos el parametro de salida
                        mySqlCommand.Parameters["@p_CustomerID"].Direction = // <--- Cambiado el nombre
                            ParameterDirection.Output;

                        // Intentaremos hacer la conexion y el comando
                        try
                        {
                            connection.Open();

                            // se ejecuta el comando
                            mySqlCommand.ExecuteNonQuery();

                            // El id es una IDENTIDAD de la base de datos
                            this.parsedCustomerID = (int)mySqlCommand.Parameters["@p_CustomerID"].Value; // <--- Cambiado el nombre

                            // Ponemos el id en el texto que es solamente de lectura
                            this.txtCustomerID.Text = Convert.ToString(this.parsedCustomerID);

                        }
                        catch (Exception ex)
                        {
                            // Si hay un fallo, avisaremos al usuario
                            MessageBox.Show($"El ID del cliente no fue conseguido. La cuenta no pudo crearse. Error: {ex.Message}"); // Agregado ex.Message para depuración
                        }
                        finally
                        {
                            try
                            {
                                connection.Close();
                            }
                            catch (Exception ex) { /* Manejo silencioso de excepción al cerrar, considerar loggear */ }
                        }
                    }
                }
            }
        }

        // Llama a Sales.uspPlaceNewOrder stored procedure para agregar una orden
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (isOrderDataValid())
            {
                // Creamos la conexion
                using (MySqlConnection connection = // <--- Cambiado a MySqlConnection
                    new MySqlConnection(Properties.Settings.Default.connString))
                {

                    // Creamos el comando a ejecutar
                    using (MySqlCommand mySqlCommand = // <--- Cambiado a MySqlCommand
                        new MySqlCommand("uspPlaceNewOrder", connection)) 
                    {
                        mySqlCommand.CommandType = CommandType.StoredProcedure;

                        // Agregamos el parametro, el cual fue obtenido por uspNewCustomer
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_CustomerID", MySqlDbType.Int32)); // <--- Cambiado el nombre a @p_CustomerID
                        mySqlCommand.Parameters["@p_CustomerID"].Value = this.parsedCustomerID; // <--- Cambiado el nombre

                        // Se agrega ahora el parametro de @OrderDate
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_OrderDate", MySqlDbType.DateTime)); // <--- Cambiado el nombre a @p_OrderDate
                        mySqlCommand.Parameters["@p_OrderDate"].Value = dtpOrderDate.Value; // <--- Cambiado el nombre

                        // Se agrega ahora el parametro de @Amount
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_Amount", MySqlDbType.Int32)); // <--- Cambiado el nombre a @p_Amount
                        mySqlCommand.Parameters["@p_Amount"].Value = numOrderAmount.Value; // <--- Cambiado el nombre

                        // Se agrega ahora el parametro de @Status, TODAS las ordenes nuevas
                        // tienen por estado "O"
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_Status", MySqlDbType.String)); // <--- Cambiado el nombre a @p_Status
                        mySqlCommand.Parameters["@p_Status"].Value = "O"; // <--- Cambiado el nombre

                        // Ahora se agrega un parametro de retorno, el cual sera el ID de la orden
                        // En MySQL, si el SP retorna un valor con SELECT, se usa ExecuteScalar()
                        // en lugar de ParameterDirection.ReturnValue
                        // mySqlCommand.Parameters.Add(new MySqlParameter("@RC", MySqlDbType.Int32));
                        // mySqlCommand.Parameters["@RC"].Direction = ParameterDirection.ReturnValue;

                        try
                        {
                            connection.Open();

                            // Para obtener el valor retornado por SELECT en el SP de MySQL
                            object result = mySqlCommand.ExecuteScalar(); // <--- Usamos ExecuteScalar() aquí

                            if (result != null)
                            {
                                this.orderID = Convert.ToInt32(result);
                                MessageBox.Show($"Numero de orden: {this.orderID}, se ha enviado.");
                            }
                            else
                            {
                                MessageBox.Show("La orden se envió, pero no se pudo obtener el número de orden.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"La orden no pudo agregarse. Error: {ex.Message}"); // Agregado ex.Message para depuración
                        }
                        finally
                        {
                            try
                            {
                                connection.Close();
                            }
                            catch (Exception ex) { /* Manejo silencioso de excepción al cerrar, considerar loggear */ }
                        }
                    }
                }
            }
        }

        private void btnAddFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddAnotherAccount_Click(object sender, EventArgs e)
        {
            this.cleanForm();
        }
    }
}
