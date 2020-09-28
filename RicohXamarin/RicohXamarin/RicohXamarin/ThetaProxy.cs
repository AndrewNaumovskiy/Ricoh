using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RicohXamarin
{
    public static class ThetaProxy
    {
        public static async Task<string> CheckConnection()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri("http://192.168.1.1:80/osc/state");
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("Accept", "application/json");

                    client.Timeout = TimeSpan.FromSeconds(5);

                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            HttpContent responseContent = response.Content;
                            var json = await responseContent.ReadAsStringAsync();
                            return json;
                        }
                        else
                        {
                            
                        }
                    }
                    catch (Exception e)
                    {
                        var kek = e;
                    }
                }
            }

            return "";
        }

    }
}
