using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmailAgent;

namespace AgentGui
{
    public partial class Form1 : Form
    {
        Emailer agent = new Emailer();
        int id_number = 1;
        public Form1()
        {
            InitializeComponent();
            try
            {
                string usernames_string = agent.GetCredentials();
                //resultBox.Text = credentials;
                string[] usernames = usernames_string.Split(' ');
                foreach (string username in usernames)
                {
                    listBox1.Items.Add(username);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {

                await agent.SendEmail(id_number, textBox1.Text, textBox2.Text, textBox3.Text);
                MessageBox.Show("Message Sent");
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string text = listBox1.GetItemText(listBox1.SelectedItem);
            id_number = listBox1.SelectedIndex;
            label3.Text = "Sending from: " + listBox1.GetItemText(listBox1.SelectedItem);

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            id_number = agent.AddAccount(textBox4.Text, textBox5.Text)-1;
            label3.Text = "Sending from: " + textBox4.Text;
            //MessageBox.Show("");
            //listBox1.Items.Add("claritytestacc2@gmail.com");
        }
    }
}
