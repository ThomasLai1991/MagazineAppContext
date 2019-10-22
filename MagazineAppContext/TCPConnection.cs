using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MagazineAppContext
{
    public class TCPConnection 
    {
        public static TcpClient Connection;
        public int Port;
        public string IPAddress;
        public static NetworkStream netStream;
        public static StreamReader streamReader;
        public static StringBuilder stringBuilder;
        private static string Buffer;
        private System.Timers.Timer tmrDisconnect;


        // constructor, default disconnet timer interval set to 5 mins 
        public TCPConnection(double tmrInterval=3000000)
        {
            Port = 5046;
            IPAddress = "52.187.249.56";
            Connection = new TcpClient();
            stringBuilder = new StringBuilder();
            //set up timer and register disconnect event upon elapse
            tmrDisconnect = new System.Timers.Timer(tmrInterval);
            tmrDisconnect.Elapsed += Disconnect;
        }


        public string Connect()
        {

            try
            {
                //connect and then get the netstream to prepare for any write/read actions
                Connection.Connect(IPAddress, Port);
                netStream = Connection.GetStream();
               
                //read the response and return it
                return ReadStream();
            }
            catch (Exception ex)
            {
                //$ is the sign telling the computer to do string interpolation
                return $"Erro: {ex.Message}, connection failed. ";
            }
           
        }

        public string ReadStream (){

            //auto reconnect if connection is lost
            MakeSureConnected();
            //stop timer to prevent auto disconnet
            tmrDisconnect.Stop();

            var response = "empty";
            Read();

     

            //restart the disconnect  counter
            tmrDisconnect.Start();
            return response;

            void Read()
            {
                //set up two variable, one is to get the netstream to read, one is a string for response
                stringBuilder.Clear();
                
                try
                {
                    streamReader = new StreamReader(netStream);
                    if (streamReader.Peek() > -1)
                    {
                        while (streamReader.Peek() > -1)
                        {
                            stringBuilder.Append(Convert.ToChar(streamReader.Read()).ToString());
                        }

                        string a = stringBuilder.ToString();
                        response = RemoveDataTerminator(a);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            // remove DataTerminator and use buffer when fail to load complete message
            string RemoveDataTerminator(string Data)
            {
                char DataTerminator = Convert.ToChar(4);
                string DT = DataTerminator.ToString();


                if (Data.Substring(Data.Length - 1, 1) == DT)
                {
                    // Data complete; transfer to ReceivedData
                    Data = Buffer + Data.Substring(0, Data.Length - 1); // remove the DataTerminator first
                    Buffer = "";

                }
                else
                {
                    Buffer = Buffer + Data;
                    Data = ReadStream();
                }
                return Data;
            }

        }




        public string SendStream(string sData)
        {
            //auto reconnect if the connection is lost
            MakeSureConnected();
            //timer must be stoped after reconnection
            tmrDisconnect.Stop();
            //send out message
            Send();
            //after sending, read response
            var response = ReadStream();
            return response;

            void Send()
            {
                try
                {
                    if (netStream.CanWrite)
                    {

                        byte[] myWriteBuffer = Encoding.UTF8.GetBytes(sData);
                        netStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
                        netStream.Flush();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }



        private void MakeSureConnected()
        {
            if (Connection.Connected == false)
            {
                Connect();
            }
        }



        //Automatic disconnect 
        public void Disconnect(Object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Connection.Close();
     
            }
            catch
            {
                // Connection already closed.
            }
        }
    }
}
