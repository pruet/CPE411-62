using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;  

namespace DNWS
{
  class StatPlugin : IPlugin
  {
    public static Mutex MuTexLock_pre = new Mutex(); 
    public static Mutex MuTexLock_res = new Mutex(); 
    protected static Dictionary<String, int> statDictionary = null;
    public StatPlugin()
    {
      if (statDictionary == null)
      {
        statDictionary = new Dictionary<String, int>();

      }
    }

    public void PreProcessing(HTTPRequest request)
    {
      MuTexLock_pre.WaitOne();
      if (statDictionary.ContainsKey(request.Url))
      {
        statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
      }
      else
      {
        statDictionary[request.Url] = 1;
      }
      MuTexLock_pre.ReleaseMutex();
    }
    public HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();
      MuTexLock_res.WaitOne(); 
      // Console.WriteLine("IncThread acquires the mutex.");  
      sb.Append("<html><body><h1>Stat:</h1>");
      foreach (KeyValuePair<String, int> entry in statDictionary)
      {
        sb.Append(entry.Key + ": " + entry.Value.ToString() + "<br />");
      }
      sb.Append("</body></html>");
      response = new HTTPResponse(200);
      response.body = Encoding.UTF8.GetBytes(sb.ToString());
      // Console.WriteLine("IncThread releases the mutex."); 
      MuTexLock_res.ReleaseMutex(); 
      return response;
    }

    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      throw new NotImplementedException();
    }
  }
}