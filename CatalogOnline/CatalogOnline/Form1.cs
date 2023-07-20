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
using System.Data.SqlClient;

namespace CatalogOnline
{
    public partial class Form1 : Form
    {
        SqlConnection con;
        int nume_clasa = 0, sala = 0, nume_elev = 0, prenume_elev = 0, clasa_selectata = 0, data_selectata = 0, nota_selectata = 0, elev_selectat = 0, materie_selectata=0;
        string val0, val1, cmd, idClasa, elev, nume, prenume, idElev, data, materie, nota;


        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            materie_selectata = 1;
        }

        SqlDataReader reader;
        SqlCommand comanda;

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Scoala.mdf;Integrated Security=True");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            elev_selectat = 1;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            nota_selectata = 1;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            data_selectata = 1;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            sala = 1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            nume_clasa = 1;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            nume_elev = 1;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            prenume_elev = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            clasa_selectata = 1;
            con.Open();
            //ADD TO COMBOBOX
            val0 = comboBox1.SelectedItem.ToString();
            val0 = String.Format(@"'{0}'", val0);
            cmd = String.Format("SELECT * FROM Clasa WHERE numeClasa={0}", val0);
            comanda = new SqlCommand(cmd, con);
            reader = comanda.ExecuteReader();
            reader.Read();
            val1 = reader[0].ToString();
            val1 = String.Format(@"'{0}'", val1);
            reader.Close();

            cmd = String.Format("SELECT * FROM Elev WHERE idClasa={0};", val1);
            comanda = new SqlCommand(cmd, con);
            reader = comanda.ExecuteReader();
            while (reader.Read())
            {
                elev = String.Concat(reader[1].ToString(), " ");
                elev = String.Concat(elev, reader[2].ToString());
                comboBox2.Items.Add(elev);
            }
            reader.Close();
            con.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                //RESET DATABASE
                SqlCommand DELETE = new SqlCommand("DELETE FROM Nota;", con);
                DELETE.ExecuteNonQuery();
                DELETE = new SqlCommand("DELETE FROM ELev;", con);
                DELETE.ExecuteNonQuery();
                DELETE = new SqlCommand("DELETE FROM Clasa;", con);
                DELETE.ExecuteNonQuery();
                SqlCommand RESET = new SqlCommand("DBCC CHECKIDENT (Clasa, RESEED, 0);", con);
                RESET.ExecuteNonQuery();
                RESET = new SqlCommand("DBCC CHECKIDENT (Elev, RESEED, 0);", con);
                RESET.ExecuteNonQuery();
                RESET = new SqlCommand("DBCC CHECKIDENT (Nota, RESEED, 0);", con);
                RESET.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nume_clasa == 1 && sala == 1)
            {
                try
                {
                    con.Open();

                    //ADD TO DATABASE
                    val0 = textBox1.Text;
                    val0 = string.Format(@"'{0}'", val0);
                    val1 = textBox2.Text;
                    val1 = string.Format(@"'{0}'", val1);
                    cmd = String.Format("INSERT INTO Clasa(numeClasa,sala) VALUES ({0},{1});", val0, val1);
                    comanda = new SqlCommand(cmd, con);
                    comanda.ExecuteNonQuery();
                    nume_clasa = 2;
                    sala = 2;

                    //ADD TO COMBOBOX
                    cmd = "SELECT * FROM Clasa WHERE idClasa=(SELECT max(idClasa) FROM Clasa);";
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    comboBox1.Items.Add(reader[1].ToString());
                    reader.Close();

                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (nume_clasa == 2 || sala == 2)
                {
                    MessageBox.Show("Ai inserat deja aceasta clasa/sala!");
                }
                else
                {
                    MessageBox.Show("Informatii Insuficiente!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((nume_elev != 0 && prenume_elev != 0 && clasa_selectata != 0) && (nume_elev != 2 || prenume_elev != 2))  //nu acceptam dubluri
            {
                try
                {
                    con.Open();

                    //preluarea id-ului Clasei in care va fi adaugat elevul
                    val0 = comboBox1.SelectedItem.ToString();
                    val0 = String.Format("'{0}'", val0);
                    cmd = String.Format("SELECT * FROM Clasa where numeClasa={0}", val0);
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    idClasa = (reader[0].ToString());
                    idClasa = String.Format(@"'{0}'", idClasa);
                    reader.Close();

                    //ADD TO DATABASE
                    val0 = textBox3.Text;
                    val0 = string.Format(@"'{0}'", val0);
                    val1 = textBox4.Text;
                    val1 = string.Format(@"'{0}'", val1);
                    cmd = String.Format("INSERT INTO Elev(Nume,Prenume,idClasa) VALUES ({0},{1},{2});", val0, val1, idClasa);
                    comanda = new SqlCommand(cmd, con);
                    comanda.ExecuteNonQuery();
                    nume_elev = 2;
                    prenume_elev = 2;

                    //ADD TO COMBOBOX ultimul elev adaugat la clasa respectiva
                    cmd = "SELECT * FROM Elev WHERE idElev=(SELECT max(idElev) FROM Elev);";
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    elev = String.Concat(reader[1], " ");
                    elev = String.Concat(elev, reader[2]);
                    comboBox2.Items.Add(elev);
                    reader.Close();

                    con.Close();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }

            }
            else
            {
                if (nume_elev == 2 && prenume_elev == 2)
                {
                    MessageBox.Show("Elev Adaugat deja!");
                }
                else
                {
                    if (nume_elev == 0 || prenume_elev == 0)
                    {
                        MessageBox.Show("Informatii insuficiente!");
                    }
                    else
                    {
                        if(clasa_selectata==0)
                        {
                            MessageBox.Show("Selecteaza clasa!");
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (data_selectata == 1 && nota_selectata == 1 && clasa_selectata == 1 && elev_selectat == 1 && materie_selectata == 1)
            {
                try
                {

                    con.Open();
                    //AFLA IDUL CLASEI SELECTATE
                    val0 = comboBox1.SelectedItem.ToString();
                    val0 = string.Format(@"'{0}'", val0);
                    cmd = String.Format("SELECT * FROM Clasa where numeClasa={0}", val0);
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    idClasa = (reader[0].ToString());
                    idClasa = String.Format(@"'{0}'", idClasa);
                    reader.Close();

                    //afla idul elevului selectat
                    val1 = comboBox2.SelectedItem.ToString();
                    string[] fullname = val1.Split(' ');
                    nume = fullname[0];
                    prenume = fullname[1];
                    nume = string.Format(@"'{0}'", nume);
                    prenume = string.Format(@"'{0}'", prenume);
                    cmd = String.Format("SELECT * FROM Elev WHERE (Nume={0} AND Prenume={1})", nume, prenume);
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    idElev = (reader[0].ToString());
                    idElev = String.Format(@"'{0}'", idElev);
                    reader.Close();


                    //TRANSOFRMA DATA IN FEL DE DATE OK
                    data = dateTimePicker1.Value.ToShortDateString();
                    data = String.Format(@"'{0}'", data);
                    //transforma nota selectata in iNT
                    nota = comboBox3.SelectedItem.ToString();
                    //mateire
                    materie = textBox6.Text;
                    materie = String.Format(@"'{0}'", materie);
                    //INSEREAZA NOTA
                    cmd = String.Format("INSERT INTO Nota(idElev,idClasa,Materie,Data,Valoare) VALUES ({0},{1},{2},{3},{4});", idElev, idClasa, materie, data, nota);
                    comanda = new SqlCommand(cmd, con);
                    comanda.ExecuteNonQuery();
                    nume_elev = 2;
                    prenume_elev = 2;


                    con.Close();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Informatii insuficiente!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(elev_selectat==1)
            {   try
                {
                    con.Open();
                    textBox5.Clear();

                    //gasim idul elevului selectat careia sa-i afisezi informatiile
                    val1 = comboBox2.SelectedItem.ToString();
                    string[] fullname = val1.Split(' ');
                    nume = fullname[0];
                    prenume = fullname[1];
                    nume = string.Format(@"'{0}'", nume);
                    prenume = string.Format(@"'{0}'", prenume);
                    cmd = String.Format("SELECT * FROM Elev WHERE (Nume={0} AND Prenume={1})", nume, prenume);
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    reader.Read();
                    idElev = (reader[0].ToString());
                    idElev = String.Format(@"'{0}'", idElev);
                    reader.Close();
                    cmd = String.Format("SELECT * FROM Nota WHERE (idElev={0})", idElev);
                    comanda = new SqlCommand(cmd, con);
                    reader = comanda.ExecuteReader();
                    while (reader.Read())
                    {
                        textBox5.AppendText(reader[3].ToString());
                        textBox5.AppendText(" ");
                        textBox5.AppendText(reader[5].ToString());
                        textBox5.AppendText(System.Environment.NewLine); ///nu merge ?
                    }
                    reader.Close();

                    con.Close();
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.Message); ;
                }
                
            }
        }
    }
}
