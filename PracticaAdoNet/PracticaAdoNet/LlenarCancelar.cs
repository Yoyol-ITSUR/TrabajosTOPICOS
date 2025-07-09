using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace PracticaAdoNet
{
    public partial class LlenarCancelar : Form
    {

        Form padre;
        private int parsedOrderID;

        private bool IsOrderValid()
        {
            if (txtOrderID.Text == "")
            {
                MessageBox.Show("Porfavor especifica el id de la orden.");
                return false;
            }
            else if (Regex.IsMatch(txtOrderID.Text, @"^\D*$"))
            {
                MessageBox.Show("ID de la orden debe de contener unicamente números.");
                txtOrderID.Clear();
                return false;
            }
            else
            {
                this.parsedOrderID = Int32.Parse(txtOrderID.Text);
                return true;
            }
        }


        public LlenarCancelar(Form padre)
        {
            this.padre = padre;
            InitializeComponent();
        }

        private void LlenarCancelar_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.padre.Show();
        }

        // Ejecuta un SELECT statement para obtener datos de órdenes con una ID
        // específica, de ahí lo muestra en un DataGridView.
        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsOrderValid())
            {
                using (MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.connString))
                {
                    // Definimos el query SQL, que tendrá el parámetro para orderID
                    // Se asume que la conexión ya está en la base de datos 'Sales'
                    const string sql = "SELECT * FROM Orders WHERE OrderID=@p_OrderID"; // <--- Cambiado el nombre de la tabla y el parámetro

                    // Creamos el comando
                    using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection)) // <--- Cambiado a MySqlCommand
                    {
                        // Definimos los parámetros y su valor
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_OrderID", MySqlDbType.Int32)); // <--- Cambiado el nombre del parámetro
                        mySqlCommand.Parameters["@p_OrderID"].Value = parsedOrderID; // <--- Cambiado el nombre del parámetro

                        try
                        {
                            connection.Open();

                            // ejecutaremos el comando usando un dataReader
                            using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader()) // <--- Cambiado a MySqlDataReader
                            {
                                // Creamos una data table para guardar los datos conseguidos
                                DataTable dataTable = new DataTable();

                                // y la cargamos
                                dataTable.Load(dataReader);

                                // Y la mostramos en la gridview
                                this.dgvCustomerOrders.DataSource = dataTable;

                                dataReader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"La orden requerida no pudo ser cargada. Error: {ex.Message}"); // Agregado ex.Message para depuración
                        }
                        finally
                        {
                            try { connection.Close(); }
                            catch (Exception ex) { /* Manejo silencioso de excepción al cerrar, considerar loggear */ }
                        }
                    }
                }
            }
        }

        // Cancelamos la orden llamando el procedimiento uspCancelOrder
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderValid())
            {
                // Creamos una conexion
                using (MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.connString))
                {
                    using (MySqlCommand command = new MySqlCommand("uspCancelOrder", connection)) // <--- Cambiado a uspCancelOrder (sin "Sales.")
                    {
                        command.CommandType = CommandType.StoredProcedure; // Aseguramos que es un SP

                        command.Parameters.Add(new MySqlParameter("@p_OrderID", MySqlDbType.Int32)); // <--- Cambiado el nombre del parámetro
                        command.Parameters["@p_OrderID"].Value = parsedOrderID; // <--- Cambiado el nombre del parámetro

                        try
                        {
                            // Abrimos la conexion
                            connection.Open();

                            command.ExecuteNonQuery();

                            MessageBox.Show($"Orden {parsedOrderID} cancelada exitosamente."); // Mensaje de éxito
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"No se logró cancelar la operación. Error: {ex.Message}"); // Agregado ex.Message para depuración
                        }
                        finally
                        {
                            try { connection.Close(); }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
        }

        // Se llena la orden llamando al procedimiento uspFillOrder
        private void btnFillOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderValid())
            {
                using (MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.connString))
                {
                    using (MySqlCommand mySqlCommand = new MySqlCommand("uspFillOrder", connection)) // <--- Cambiado a uspFillOrder (sin "Sales.")
                    {
                        mySqlCommand.CommandType = CommandType.StoredProcedure; // Aseguramos que es un SP

                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_OrderID", MySqlDbType.Int32)); // <--- Cambiado el nombre del parámetro
                        mySqlCommand.Parameters["@p_OrderID"].Value = parsedOrderID; // <--- Cambiado el nombre del parámetro

                        // Agregamos la fecha como parametro
                        mySqlCommand.Parameters.Add(new MySqlParameter("@p_FilledDate", MySqlDbType.DateTime)); // <--- Cambiado el nombre del parámetro y eliminado el tamaño
                        mySqlCommand.Parameters["@p_FilledDate"].Value = dtpFillDate.Value; // <--- Cambiado el nombre del parámetro

                        try
                        {
                            connection.Open();

                            mySqlCommand.ExecuteNonQuery();

                            MessageBox.Show($"Orden {parsedOrderID} llenada exitosamente."); // Mensaje de éxito
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"No se pudo hacer la operación. Error: {ex.Message}"); // Agregado ex.Message para depuración
                        }
                        finally
                        {
                            try { connection.Close(); }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
        }

        private void btnFinishUpdates_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
