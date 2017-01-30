using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace DatosBD
{
    public partial class Login : Form
    {
        public string cadena = ConfigurationManager.ConnectionStrings["conection"].ToString();
        public Login()
        {
            InitializeComponent();
            this.Opacity = 0.0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Opacity += .02;
        }
        public bool CheckPass(string username,string password)
        {
            using (SqlConnection cnn = new SqlConnection(cadena))
            {
                cnn.Open();
                string query = "SELECT COunt(username) FROM [dbo].[administrator] where dbo.administrator.username='" + username+"'AND dbo.administrator.password = '"+password+"'";
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if (myReader[0].ToString()=="1")
                    {
                        Console.WriteLine("Existe");
                        return true;
                    }
                }
                return false;
                cnn.Close();
                myReader.Close();
                cmd.Dispose();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckPass(tbUser.Text,tbPass.Text))
            {
                this.Hide();
                Form1 next = new Form1();
                next.Show();
            }
            else
            {
                MessageBox.Show("Ingresa correctamente tus datos");
            }
        }
    }
}