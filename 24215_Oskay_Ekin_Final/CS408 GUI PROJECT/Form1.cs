using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS408_GUI_PROJECT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static int port;
        static string ip;
        static string name;
        static string question;
        static string answer;
        const string askQuestion = "askingForQuestion-1";
        const string answerQuestion = "aNsWeRtheQuestiOn-1";
        const string trueQuestion = "theQuEstIoniSTruE-1";
        const string falseQuestion = "theQuEstIoniSfAllSe-1";
        const string sucConnection = "theConnectionisOk---";
        static bool askQuestionTurn = false;
        static bool answerTurn = false;
        static bool proceedNext = false;
        static bool connected = false;


        static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private void port_textbox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ip_textbox_TextChanged(object sender, EventArgs e)
        {
            ip = ip_textbox.Text;
        }

        private void name_textbox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void question_textbox_TextChanged(object sender, EventArgs e)
        {
            question = question_textbox.Text;
        }

        private void answer_textbox_TextChanged(object sender, EventArgs e)
        {
            answer = answer_textbox.Text;
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            clientSocket.ReceiveTimeout = 10000000;
            clientSocket.SendTimeout = 10000000;
            ip = ip_textbox.Text;
            port = int.Parse(port_textbox.Text);
            name = name_textbox.Text;
            try
            {
                clientSocket.Connect(ip, port);
                richTextBox1.AppendText("Trying to connect with the name of " + name + "\n");
                Thread tryToConnect = new Thread(tryConnect);
                tryToConnect.Start();
                Thread questionLoops = new Thread(questionLoop);
                questionLoops.Start();
            }
            catch
            {

            }
        }

        private void questionLoop(){
            while (!connected) ;
            while (true) { 
                proceedNext = false;
                Byte[] buffer = new Byte[128];
                clientSocket.Receive(buffer);
                string message = Encoding.Default.GetString(buffer).Trim('\0');
                if (message == askQuestion){
                    answer_textbox.Text = "";
                    question_textbox.Text = "";
                    richTextBox1.AppendText("You should send a question and its' answer \n");
                    askQuestionTurn = true;
                }
                else if (message == answerQuestion){
                    Byte[] quest = new Byte[128];
                    clientSocket.Receive(quest);
                    string theQuest = Encoding.Default.GetString(quest).Trim('\0');
                    richTextBox1.AppendText("You should send an answer to this question: \n" + theQuest + "\n");
                    answer_textbox.Text ="";
                    question_textbox.Text = theQuest;
                    answerTurn = true;
                }
                while (!proceedNext);
                Byte[] situation = new Byte[64];
                clientSocket.Receive(situation);
                string tORf = Encoding.Default.GetString(situation).Trim('\0');
                if (tORf == trueQuestion){
                    richTextBox1.AppendText("The question has been answered correctly \n");
                }
                else{
                    richTextBox1.AppendText("The answer is incorrect \n");
                }

            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (askQuestionTurn){
                clientSocket.Send(Encoding.Default.GetBytes(question));
                clientSocket.Send(Encoding.Default.GetBytes(answer));
                richTextBox1.AppendText("Your question and answer has been sent \n");
                askQuestionTurn = false;
                proceedNext = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (answerTurn){
                clientSocket.Send(Encoding.Default.GetBytes(answer));
                richTextBox1.AppendText("Your answer has been sent \n");
                answerTurn = false;
                proceedNext = true;
            }
        }

        private void tryConnect()
        {
            while (!connected) {
                Byte[] buffer = new Byte[64];
                clientSocket.Receive(buffer);
                string message = Encoding.Default.GetString(buffer).Trim('\0');
                if (message == sucConnection) {
                    connected = true;
                    break;
                }
                    clientSocket.Send(Encoding.Default.GetBytes(name));
                    
                }
            richTextBox1.AppendText("Connection is successfull!\n");
        }

    }

}
