using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRC.Cloud.Native
{
    using System;
    using System.Net;
    public class Program
    {
        static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:9090/");
            listener.Start();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("NRC Device Logger Cloud Native is Ready ...");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Listening on:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Dns.GetHostByName(Dns.GetHostName()).AddressList.ToList().ForEach(a =>
            {
                Console.WriteLine($"\thttp://{a}:9090");
            });
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                try
                {

                    var context = listener.GetContext();
                    var request = context.Request;

                    var param = new Dictionary<string, string>();
                    request.Url.Query.TrimStart('?').Split('&').ToList().ForEach(x =>
                    {
                        var k = x.Split('=');
                        param[k[0]] = k[1];
                    });
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[{DateTime.Now}]  ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{request.RemoteEndPoint.Address}({param["n"]}) ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"=> ");
                    Console.ForegroundColor = ConsoleColor.Cyan;

                    if (param["act"].ToUpper() == "INP")
                    {
                        Console.WriteLine($"{param["act"].ToUpper()} SWI= {param["swi"]} HVI= {param["hvi"]}");
                    }
                    else
                    {
                        Console.WriteLine($"{param["act"].ToUpper()} Relay {param["r"]}");
                    }
                    Console.ForegroundColor = ConsoleColor.White;


                    if (request.HasEntityBody)
                    {
                        var encoding = request.ContentEncoding;
                        var reader = new System.IO.StreamReader(request.InputStream, encoding);
                        Console.WriteLine(reader.ReadToEnd());
                    }

                    var response = context.Response;
                    var responseString = "OK";
                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
// https://rebox98.ir