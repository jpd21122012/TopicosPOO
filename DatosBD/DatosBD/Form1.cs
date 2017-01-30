using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;

namespace DatosBD
{
    public partial class Form1 : Form
    {
       public string cadena = ConfigurationManager.ConnectionStrings["conection"].ToString();

        public Form1()
        {
            InitializeComponent();
            this.Opacity = 0.0;
            cargarDatos();
        }
        public void MostrarTB() {
            if (dtgDatos.CurrentRow == null)
                return;
            string id = dtgDatos.CurrentRow.Cells["matricula"].Value.ToString();
            Console.WriteLine(id);
            using (SqlConnection cnn = new SqlConnection(cadena))
            {
                cnn.Open();
                string query = "SELECT * FROM [dbo].[user] WHERE dbo.[user].matricula = @id";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    textBox1.Text= myReader["matricula"].ToString();
                    textBox2.Text= myReader["fabricante"].ToString();
                    textBox3.Text = myReader["modelo"].ToString();
                    textBox4.Text = myReader["version"].ToString();
                    textBox5.Text = myReader["serial"].ToString();
                    textBox6.Text = myReader["email"].ToString();
                }
                cnn.Close();
                myReader.Close();
                cmd.Dispose();
            }
            CargarImagen(id);
        }
        public void CargarImagen(string id) {
            using (SqlConnection con = new SqlConnection(cadena))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Image FROM [dbo].[user] WHERE dbo.[user].matricula = @id";
                    SqlCommand command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@id", id);
                    byte[] datosImagen = (byte[])command.ExecuteScalar();
                    MemoryStream ms = new MemoryStream(datosImagen);
                    Image img = Image.FromStream(ms);
                    pictureBox1.Show();
                    pictureBox1.Image = img;
                }
                catch (Exception e)
                {
                    if (e!=null)
                    {
                        pictureBox1.Hide();
                    }
                }
               }
        }
        public void ActualizarRegistro() {
            if (dtgDatos.CurrentRow == null)
                return;
            string id = dtgDatos.CurrentRow.Cells["matricula"].Value.ToString();
            using (SqlConnection cnn = new SqlConnection(cadena))
            {
                cnn.Open();
                string query = "UPDATE [Upt].[dbo].[user] SET [matricula]=@matricula, [fabricante]=@fabricante, [modelo]=@modelo, [version]=@version, [serial]=@serial, [email]=@email, [Image]=@Image WHERE ([matricula]="+id+")";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@matricula", Convert.ToInt32(textBox1.Text));
                cmd.Parameters.AddWithValue("@fabricante", textBox2.Text);
                cmd.Parameters.AddWithValue("@modelo", textBox3.Text);
                cmd.Parameters.AddWithValue("@version", textBox4.Text);
                cmd.Parameters.AddWithValue("@serial", textBox5.Text);
                cmd.Parameters.AddWithValue("@email", textBox6.Text);
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                cmd.Parameters.AddWithValue("@Image", ms.GetBuffer());
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
            cargarDatos();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }
        public void EliminarRegistro() {
            if (dtgDatos.CurrentRow == null)
                return;
            string id = dtgDatos.CurrentRow.Cells["matricula"].Value.ToString();
            using (SqlConnection cnn = new SqlConnection(cadena))
            {
                cnn.Open();
                string query = "DELETE FROM [Upt].[dbo].[user] WHERE ([matricula]=@id)";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }
        public void InsertarRegistro() {
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                string query = "INSERT INTO [Upt].[dbo].[user] ([matricula], [fabricante], [modelo], [version], [serial], [email],[Image]) VALUES (@matricula, @fabricante, @modelo, @version, @serial, @email,@Image)";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@matricula", Convert.ToInt32( textBox1.Text));
                cmd.Parameters.AddWithValue("@fabricante", textBox2.Text);
                cmd.Parameters.AddWithValue("@modelo", textBox3.Text);
                cmd.Parameters.AddWithValue("@version", textBox4.Text);
                cmd.Parameters.AddWithValue("@serial", textBox5.Text);
                cmd.Parameters.AddWithValue("@email", textBox6.Text);
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                cmd.Parameters.AddWithValue("@Image", ms.GetBuffer());
                int rows = cmd.ExecuteNonQuery();
                if (rows>0)
                {
                    MessageBox.Show("Se agregaron los datos!!!");
                }
                else
                {
                    MessageBox.Show("Transaccion incompleta");
                }
                conexion.Close();
            }
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            pictureBox1.Image = null;
        }
        public void cargarDatos() {
            using (SqlConnection conexion = new SqlConnection(cadena)) {
                string query = "SELECT * FROM [dbo].[user]";
                SqlDataAdapter adaptador = new SqlDataAdapter(query,conexion);
                DataSet setDatos = new DataSet();
                adaptador.Fill(setDatos,"Productos");
                if (setDatos.Tables.Count>0)
                {
                    dtgDatos.DataSource = setDatos;
                    dtgDatos.DataMember = "Productos";
                }
                else
                {
                    dtgDatos.DataSource = null;
                    MessageBox.Show("No hay registros");
                }
            }
        }
        private void btnregistrar_Click(object sender, EventArgs e)
        {
            InsertarRegistro();
            cargarDatos();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EliminarRegistro();
            cargarDatos();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ActualizarRegistro();
        }
        public void StoredProcedure() {
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Procedure]", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    MessageBox.Show("Numero de registros :\n\t"+myReader[0].ToString());
                }
                conexion.Close();
            }
        }
        private void btnProcedure_Click(object sender, EventArgs e)
        {
            StoredProcedure();
            
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // Se muestra al usuario esperando una acción
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(dialog.FileName);
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {

        }

        private void dtgDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MostrarTB();
        }

        private void dtgDatos_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            MostrarTB();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Opacity += .02;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Reportes.Resultado obj = new Reportes.Resultado();
            obj.Show();
        }
    }
}