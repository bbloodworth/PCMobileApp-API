using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CchWebAPI.Handlers
{
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        private const string RequestLogFolder = "C:\\inetpub\\logs\\ResponseLogs\\";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Directory.Exists(RequestLogFolder))
            {
                string ticks = DateTime.Now.ToString("O").Substring(DateTime.Now.ToString("O").Length - 13, 6);

                string requestLog = string.Format("{0}wapi_request_{1}_{2}.txt", RequestLogFolder,
                    DateTime.Now.ToString("u").Replace(":", "-"), ticks);

                if (request.Content != null)
                {
                    var requestMessage = await request.Content.ReadAsByteArrayAsync();

                    string requestBody = await request.Content.ReadAsStringAsync();
                    requestBody = string.Format("Request: {0} {1}\r\n{2}", request.Method, request.RequestUri, 
                        Encoding.UTF8.GetString(requestMessage));

                    using (StreamWriter sw = new StreamWriter(requestLog))
                    {
                        sw.WriteLine(requestBody);
                        sw.Flush();
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    string ticks = DateTime.Now.ToString("O").Substring(DateTime.Now.ToString("O").Length - 13, 6);

                    string responseLog = string.Format("{0}wapi_response_{1}_{2}.txt", RequestLogFolder,
                        DateTime.Now.ToString("u").Replace(":", "-"), ticks);

                    if (task.Result.Content != null)
                    {
                        var responseBody = task.Result.Content.ReadAsStringAsync().Result;

                        using (StreamWriter sw = new StreamWriter(responseLog))
                        {
                            sw.WriteLine(responseBody);
                            sw.Flush();
                        }
                    }
                    return task.Result;
                }, cancellationToken);
        }
    }
}