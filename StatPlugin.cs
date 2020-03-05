using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace DNWS
{
    class StatPlugin : IPlugin
    {
        private static Mutex mut = new Mutex();
        protected static Dictionary<String, int> statDictionary = null;
        public StatPlugin()
        {
            mut.WaitOne();
            if (statDictionary == null)
            {
                statDictionary = new Dictionary<String, int>();

            }
            mut.ReleaseMutex();
        }

        public void PreProcessing(HTTPRequest request)
        {
            mut.WaitOne();
            if (statDictionary.ContainsKey(request.Url))
            {
                statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
            }
            else
            {
                statDictionary[request.Url] = 1;
            }
            mut.ReleaseMutex();
        }
        public HTTPResponse GetResponse(HTTPRequest request)
        {
            HTTPResponse response = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body><h1>Stat:</h1>");
            foreach (KeyValuePair<String, int> entry in statDictionary)
            {
                sb.Append(entry.Key + ": " + entry.Value.ToString() + "<br />");
            }
            sb.Append("</body></html>");
            response = new HTTPResponse(200);
            response.body = Encoding.UTF8.GetBytes(sb.ToString());
            return response;
        }

        public HTTPResponse PostProcessing(HTTPResponse response)
        {
            throw new NotImplementedException();
        }
    }
}