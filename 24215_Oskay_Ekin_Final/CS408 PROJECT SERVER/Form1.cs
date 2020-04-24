using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS408_PROJECT_SERVER{
    public partial class Form1 : Form{

        public Form1(){
            InitializeComponent();
        }
        static int port;
        static string ip;
        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static int count = 0, connectedCount = 0;
        static List<Socket> nameSockets = new List<Socket>();
        static List<Socket> playerSockets = new List<Socket>();
        static bool accept = true, terminating = false;
        static List<string> Names = new List<string>();
        const string askingForName = "ASKING_FOR_NAME_123456789";
        const string dcClient = "---------11111----";
        const string askQuestion = "askingForQuestion-1";
        const string answerQuestion = "aNsWeRtheQuestiOn-1";
        const string trueQuestion = "theQuEstIoniSTruE-1";
        const string falseQuestion = "theQuEstIoniSfAllSe-1";
        const string sucConnection = "theConnectionisOk---";

        private void ip_textbox_TextChanged(object sender, EventArgs e){

        }

        private void port_textbox_TextChanged(object sender, EventArgs e){
        }


        private void button1_Click(object sender, EventArgs e){
            serverSocket.ReceiveTimeout = 10000000;
            serverSocket.SendTimeout = 10000000;
            port = int.Parse(port_textbox.Text);

            try
            {

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(100);
                richTextBox.AppendText("Server has started with IP: "+ endPoint + " and the port: " + port+"\n");
                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();

                Thread questionThread = new Thread(sendQuestions);
                questionThread.Start();



            }
            catch
            {
                Console.WriteLine("ERROR1 There is a problem! Check the port number and try again!");
            }
        }


        private void Accept(){
            while (accept){
                try{
                    Socket newClient = serverSocket.Accept();
                    nameSockets.Add(newClient);
                    bool isExist = true;
                    int clientNumber = count;
                    count++;
                    string currentName="";
                    while (isExist){
                        currentName = askForName(clientNumber);
                        if (currentName == dcClient) {
                            count--;
                            newClient.Close();
                            return;
                        }
                        bool isOk = false;
                        for(int i=0; i<Names.Count(); i++){
                            if (Names[i] == currentName){
                                isOk = true;
                            }
                        }
                        isExist = false;
                        if (isOk) {
                            isExist = true;
                            richTextBox.AppendText("Please choose another name, that one is already taken\n");
                        }
                    }
                    newClient.Send(Encoding.Default.GetBytes(sucConnection));
                    connectedCount++;
                    Names.Add(currentName);
                    richTextBox.AppendText(currentName + " connected successfully!\n");

                    
                }
                catch{
                    if (terminating){
                        richTextBox.AppendText("Server stopped working...");
                        accept = false;
                    }
                    else{
                        richTextBox.AppendText("Problem occured while accepting the client?!");
                    }
                }
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string askForName(int whichClient){
            int reportCount = 0;
            while (true) {
                try{
                    string message = askingForName;
                    Socket thisClient = nameSockets[whichClient];
                    thisClient.Send(Encoding.Default.GetBytes(message));
                    byte[] buffer = new Byte[64];
                    thisClient.Receive(buffer);
                    string currentName = Encoding.Default.GetString(buffer).Replace("\0", String.Empty);
                    message = String.Concat(currentName, " is trying to connect\n");
                    richTextBox.AppendText(message);
                    return currentName;
                }
                catch{
                    richTextBox.AppendText("An error occured while client number " + whichClient + " is trying to send its name\n");
                    reportCount++;
                    if (reportCount == 2) {
                        richTextBox.AppendText("The client with number " + whichClient + " probably disconnected, sorry!\n");
                        return dcClient;
                    }
                }
            }
        }

        private void sendQuestions()
        {
            while (connectedCount != 2) ;
            int isFirstGo = 0;
            if (String.Compare(Names[1], Names[0]) > 0)
            {
                isFirstGo = 1;
            }
            while (true)
            {
                nameSockets[isFirstGo].Send(Encoding.Default.GetBytes(askQuestion));
                Byte[] question = new Byte[64];
                nameSockets[isFirstGo].Receive(question);
                Byte[] answer = new Byte[64];
                nameSockets[isFirstGo].Receive(answer);
                string theQuestion = Encoding.Default.GetString(question).Trim('\0');
                string theAnswer = Encoding.Default.GetString(answer).Trim('\0');
                richTextBox.AppendText("The question is: \n" + theQuestion + "\nThe answer is: \n" + theAnswer + "\n");
                nameSockets[(isFirstGo + 1) % 2].Send(Encoding.Default.GetBytes(answerQuestion));
                nameSockets[(isFirstGo + 1) % 2].Send(Encoding.Default.GetBytes(theQuestion));
                Byte[] answer2 = new Byte[64];
                nameSockets[(isFirstGo + 1) % 2].Receive(answer2);
                string theAnswer2 = Encoding.Default.GetString(answer2).Trim('\0');
                string messageToSend = "";
                if (theAnswer2 == theAnswer) messageToSend = trueQuestion;
                else messageToSend = falseQuestion;
                if (messageToSend == trueQuestion) richTextBox.AppendText("The answer is CORRECT, will send a command for new question");
                else                               richTextBox.AppendText("The answer is INCORRECT, will send a command for new question");
                foreach (Socket player in nameSockets)
                {
                    player.Send(Encoding.Default.GetBytes(messageToSend));
                }
                System.Threading.Thread.Sleep(1000);
                isFirstGo++;
                isFirstGo %= 2;
            }
        }


    }
}


