using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sockets.Cliente
{
    internal class Cliente
    {
        private string IP = "127.0.0.1";
        private int PORT = 4444;
        TcpClient tcpClient;
        StreamWriter output;
        double RESULTADO = 0;

        public static void Main(string[] args)
        {
            Cliente c = new Cliente();
            c.iniciar();
        }
        public void iniciar()
        {
            Thread t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
            string mensaje = "n";
            Console.Write("Ingrese mensaje: ");
            while (!mensaje.Equals("s"))
            {
                mensaje = Console.ReadLine();
                ClienteEnvia(mensaje);
            }
            Console.WriteLine("Adios");
        }

        public void ThreadProc()
        {
            tcpClient = new TcpClient(IP, PORT);
            while (true)
            {
                byte[] data = new byte[256];
                string responseData = string.Empty;
                int bytes = tcpClient.GetStream().Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Function f = JsonSerializer.Deserialize<Function>(responseData);
                Console.WriteLine("De " + f.a + " a " + f.b);
                RESULTADO = Operacion.integrar(f);
                ClienteEnvia(RESULTADO.ToString());
            }
        }

        public void ClienteEnvia(string mensaje)
        {
            //Byte[] data = Encoding.ASCII.GetBytes(mensaje);
            //tcpClient.GetStream().Write(data, 0, data.Length);

            output = new StreamWriter(tcpClient.GetStream());
            output.WriteLine(mensaje);
            Console.WriteLine("Enviado: {0}", mensaje);
            output.Flush();
        }
    }
}
