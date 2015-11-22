using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using KynetLib;
using System.Reflection;
using KynetServer.Web;
using System.Web;
using System.Collections.Specialized;

namespace KynetServer
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerContext, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerContext, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("This framework version does not support the HttpListener.");

            // URI prefixes are required
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("Insufficient prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("Method required");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerContext, string> method, params string[] prefixes) : this(prefixes, method)
        { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }

    public class WebAPI
    {
        public static void StartWebServer()
        {
            WebServer webServer = new WebServer(ProcessRequest, "http://*:20524/");
            webServer.Run();
        }

        public static string ProcessRequest(HttpListenerContext ctx)
        {
            string functionName = ctx.Request.Url.Segments[1].Replace("/", "");
            Uri uri = ctx.Request.Url;

            switch (functionName)
            {
                case "getallusers":
                    return GetAllUsers();
                case "getuser":
                    return GetUser(uri);
                case "searchusers":
                    return SearchUsers(ctx.Request);
                default:
                    return "Nothing";
            }
        }

        public static string GetAllUsers()
        {
            return JsonConvert.SerializeObject(Server.ConnectedClients);
        }
        public static string GetUser(Uri uri)
        {
            string fingerprint = HttpUtility.ParseQueryString(uri.Query).Get("fingerprint");
            return JsonConvert.SerializeObject(Server.ConnectedClients.Where(f => f.Fingerprint == fingerprint));
        }
        public static string SearchUsers(HttpListenerRequest request)
        {
            List<UserClient> foundClients = new List<UserClient>();

            string key = request.QueryString.AllKeys[0];
            string value = request.QueryString[key];

            //Server.ConnectedClients.Where(f => nameof(f) == key)
            //  Response.Write("Key: " + key + " Value: " + Request.QueryString[key]);

            //Server.ConnectedClients.FindAll

            return "Not implemented.";
        }
    }

}