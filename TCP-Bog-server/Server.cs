using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookLibrary.Model;
using Newtonsoft.Json;

namespace TCP_Bog_server
{
    public class Server
    {

        #region InstanceFields

        private static List<Bog> _bøger = new List<Bog>(){
            new Bog("We are number 1", "Robbie Rotten", 1000, "lazytown12345"),
            new Bog("Za Warudo", "Dio Brando", 100, "tokiyotomare0"),
            new Bog("How to cook", "Gordon Ramsay", 437, "lambsauce6666"),
            new Bog("Know your memes", "Proffessor meme", 333, "douknowdawae5"),
            new Bog("The Swallow Zireael", "Ciri", 999, "witcherisdwae"),
            new Bog("White Wolf", "Geralt of Rivia", 597, "onthetrail908")
        };

        #endregion

        #region Methods

        public void Start()
        {

            TcpListener server = new TcpListener(IPAddress.Loopback, 4646);
            server.Start();


            while (true)
            {
                TcpClient socket = server.AcceptTcpClient();

                Task.Run(() =>
                {
                    TcpClient tempsocket = socket;
                    DoClient(socket);
                });
            }

        }




        #endregion

        #region HelpMethods

        private void DoClient(TcpClient socket)
        {
            using (StreamReader sr = new StreamReader(socket.GetStream()))
            using (StreamWriter sw = new StreamWriter(socket.GetStream()))
            {
                string requestFirstLine = sr.ReadLine();

                string requestSecondLine = sr.ReadLine();

                //Thread.Sleep(5000);//Kan inkluderes for at vise at serveren er concurrent

                if (requestFirstLine != null && requestFirstLine.ToUpper() == "HENTALLE")
                {
                    sw.WriteLine(GetAll(requestSecondLine));
                    sw.Flush();
                }
                else if (requestFirstLine != null && requestFirstLine.ToUpper() == "HENT")
                {
                    sw.WriteLine(GetOne(requestSecondLine));
                    sw.Flush();
                }
                else if (requestFirstLine != null && requestFirstLine.ToUpper() == "GEM")
                {
                    sw.WriteLine(Save(requestSecondLine));
                    sw.Flush();
                }


            }

            socket?.Close();
            
        }


        private string GetAll(string requestSecondLine)
        {
            if (string.IsNullOrEmpty(requestSecondLine))
            {
                return JsonConvert.SerializeObject(_bøger);
            }

            return "Forkert Forespoergsel \nRigtigt format:\nHent Alle\n(Tom linje)";
        }

        private string GetOne(string requestSecondLine)
        {
            if (requestSecondLine != null && requestSecondLine.Length == 13 && _bøger.Exists(b => b.Isbn13 == requestSecondLine))
            {
                Bog tempBog = _bøger.Find(b => b.Isbn13 == requestSecondLine);
                return JsonConvert.SerializeObject(tempBog);
            }
            else if (requestSecondLine != null && requestSecondLine.Length == 13 && !_bøger.Exists(b => b.Isbn13 == requestSecondLine))
            {
                return "Bogen findes ikke";
            }

            return "Forkert Forespoergsel\nRigtigt format:\nHent\nIsbn13-nummer(det skal vaere paa 13 tegn)";
        }

        private string Save(string requestSecondLine)
        {
            if (!string.IsNullOrWhiteSpace(requestSecondLine))
            {
                Bog tempBog = JsonConvert.DeserializeObject<Bog>(requestSecondLine);

                if (tempBog != null && !_bøger.Exists(b => b.Isbn13 == tempBog.Isbn13))
                {
                    _bøger.Add(tempBog);
                    return "Bogen blev oprettet";
                }
            }

            return "Forkert Forespoergsel\nRigtigt format:\nGem\n(Bog i Json-format)";
        }

        #endregion





    }
}
