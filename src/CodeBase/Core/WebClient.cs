using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace CodeBase.Core
{
    public class WebClient
    {
        private static HttpClient client;

        public static readonly Action<HttpStatusCode> HTTPError = (code) =>
        {
            MessageHelper.Error($"{(int)code}/{code}", "HTTP Error");
        };

        public static void InitClient()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", Hardware.GetID());
            }
        }

        public static async void Request(string URL, Action<HttpResponseMessage> response)
        {
            InitClient();

            try
            {
                var res = await client.GetAsync(URL);
                response(res);
            }
            catch (Exception ex)
            {
                MessageHelper.ThrowException(ex);
            }
        }

        public static async void PostRequest(string url, Dictionary<string, string> fields, Action<HttpResponseMessage> response)
        {
            InitClient();

            using var message = new HttpRequestMessage(HttpMethod.Post, url);
            var pairs = fields.Select(pair => $"{HttpUtility.UrlEncode(pair.Key)}={HttpUtility.UrlEncode(pair.Value)}");
            var query = string.Join("&", pairs);
            message.Content = new StringContent(query, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                var res = await client.SendAsync(message);
                response(res);
            }
            catch (Exception ex)
            {
                MessageHelper.ThrowException(ex);
            }
        }

        public static async void GetRequest(string url, Dictionary<string, string> fields, Action<HttpResponseMessage> response)
        {
            InitClient();

            var pairs = fields.Select(pair => $"{pair.Key}={pair.Value}");
            var query = string.Join("&", pairs);

            using var message = new HttpRequestMessage(HttpMethod.Get, $"{url}?{query}");

            try
            {
                var res = await client.SendAsync(message);
                response(res);
            }
            catch (Exception ex)
            {
                MessageHelper.ThrowException(ex);
            }
        }

        public static void ProcessResponse(HttpWebResponse response, Action<string> onSuccess, Action<HttpStatusCode> onFailure = null)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            onFailure ??= HTTPError;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream);
                var data = reader.ReadToEnd();
                onSuccess?.Invoke(data);
            }
            else
            {
                onFailure?.Invoke(response.StatusCode);
            }
        }

        public static async void ProcessResponse(HttpResponseMessage response, Action<string> onSuccess, Action<HttpStatusCode> onFailure = null)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            onFailure ??= HTTPError;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                onSuccess?.Invoke(data);
            }
            else
            {
                onFailure?.Invoke(response.StatusCode);
            }
        }
    }

}
