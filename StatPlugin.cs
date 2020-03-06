using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace DNWS
{
  class StatPlugin : IPlugin
  {
    protected static Dictionary<String, int> statDictionary = null;
    //create Mutex (mut) for managing accession of mutithreading
    protected static Mutex mut = new Mutex();
    public StatPlugin()
    {
      if (statDictionary == null)
      {
        statDictionary = new Dictionary<String, int>();

      }
    }
    // Use mutex for protecting accessing concurrent statDictionary
    public void PreProcessing(HTTPRequest request)
    {
      //Acquire lock
      mut.WaitOne();
      
      if (statDictionary.ContainsKey(request.Url))
      {
        statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
      }
      else
      {
        statDictionary[request.Url] = 1;
      }
      //release lock
      mut.ReleaseMutex();
    }
    //ใส่mutexตรงส่วนGetResponse เพื่อป้องกันการแย่งกันใช้ข้อมูล
    public HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();
      //Acquire lock
      mut.WaitOne();
      sb.Append("<html><body><h1>Stat:</h1>");
      foreach (KeyValuePair<String, int> entry in statDictionary)
      {
        sb.Append(entry.Key + ": " + entry.Value.ToString() + "<br />");
      }
      sb.Append("</body></html>");
      response = new HTTPResponse(200);
      response.body = Encoding.UTF8.GetBytes(sb.ToString());
      //release lock
      mut.ReleaseMutex();
      return response;
    }

    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      throw new NotImplementedException();
    }
  }
}