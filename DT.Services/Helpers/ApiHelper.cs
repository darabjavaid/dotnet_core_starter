using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DT.Services.Helpers
{
    public static class ApiHelper
    {
        public static object Get(string baseURL, string param, string bearerToken = "")
        {
            string querystring = string.Concat(baseURL, param);
            var client = new RestClient(baseURL);
            var request = new RestRequest(querystring, Method.GET);
            if (!string.IsNullOrEmpty(bearerToken))
                request.AddHeader("Authorization", bearerToken);

            var result = client.Execute(request);
            return result.Content.ToString();
        }

        public static object Post<T>(T requestObj, string baseURL, int timeout = 0, string bearerToken="") where T : class
        {
            var client = new RestClient(baseURL);
            if (timeout > 0)
                client.Timeout = timeout;

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(requestObj);
            if (!string.IsNullOrEmpty(bearerToken))
                request.AddHeader("Authorization", bearerToken);

            var response = client.Execute(request);
            var result = response.IsSuccessful ? Newtonsoft.Json.JsonConvert.DeserializeObject<object>(response.Content) : null;
            return result;
        }
    }
}
