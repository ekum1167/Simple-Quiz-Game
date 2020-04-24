CS408 DEMO QUIZ PROGRAM

EKIN OSKAY
FURKAN ERGUN
ARTUN SARIOGLU



SERVER MODULE:

In the server module, when program is started a screen pops up with Port number and Rich Text Box.
User will enter port number for the server and program initializes the server with specified port number on the localhost.

Every status information, questions and answers will be shown in the Rich Text Box

Details:

We created Server socket and list of player sockets. We initially take port number from the user and parse it to the integer value. After that we create a IPEndPoint with given port number and 127.0.0.1 which is localhost. We bind it to the server socket. Then server starts to listen and we append text to the rich text box that connection has been established.
 We start to accept threads of players and start question threads

In the accept,
we create a new client and make server accept it. After that we ask for a name from the user and while its not same with the other player we ask it again.  After everything done correctly, we add name of the player to the list and we increase the connectedcount and append text that another player has been connected.

Disconnect button:

We append text ("Server is closing") and we abort each thread and clear the socket list.

askForName:

This function repedeatly ask for the name of client until the client gives a proper name that not exist.
After the proper name, sends a verify strings so that client can move on to the next step.
This function is executed when every new client is trying to connect.

sendQuestions:

We get the question from the buffer and show it to the other player. Server sees the questions and answers. According to the answers, program shows a message if answer is correct or not. Then player that receives the question now asks questions and it goes on like that.





CLIENT MODULE:

In this client module program, user enters IP adress (localhost) and a port number that is same with the server, and name. When connect button is pressed user connects to the server and according to their names server gives one of the user turn.

User enters a question and enters an answer for it. This question is sent to the other player and other player writes his answer and presses answer button. If user2 answers correctly he gets the message that answer is correct and he is asked to enter a question and goes on like that. 

DETAILS:

we took ip port and the name from gui and try to connect the client socket with them. After that connect starts with a thread and we also start the questionloop with another thread
Question loop continues until connection is lost and . We first show message to the user that, program is waiting for the question. After user sends it and presses the send button we receive the question with buffer and send it to the other player
According to the answer we reply back that answer is correct or not. And other player starts to ask question. And loop goes on like that until one of the players or server disconnects.


Known bugs:
If user sends a name that exist in server, connection time is delayed between 3-7 seconds.
User also answer the question he/or she sent.