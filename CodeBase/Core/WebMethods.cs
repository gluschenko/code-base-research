using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CodeBase;

namespace CodeBase
{
    public class WebMethods
    {
        public static string ReceiverURL = "";

        private static void CallMethod(string method, Dictionary<string, string> fields, Action<HttpResponseMessage> response)
        {
            if (!fields.ContainsKey("Method"))
                fields.Add("Method", method);
            else
                fields["Method"] = method;

            try
            {
                WebClient.PostRequest(ReceiverURL, fields, response);
            }
            catch (Exception ex)
            {
                MainWindow.Error(ex.ToString(), ex.GetType().Name);
            }

            /*var pairs = new List<string>();
            foreach (var pair in fields)
            {
                pairs.Add($"{pair.Key}={pair.Value}");
            }

            string url = $"{ReceiverURL}?{string.Join("&", pairs)}";

            WebClient.Request(url, response);*/
        }

        public static Dictionary<string, string> AuthForm(ApplicationData data)
        {
            return new Dictionary<string, string> {
                { "UserID", data.UserID },
                { "Token", data.Token },
            };
        }

        //

        public static void Login(string UserID, string Password, Action<HttpResponseMessage> response)
        {
            CallMethod("Login", new Dictionary<string, string> {
                { "UserID", UserID },
                { "Password", Password },
            }, response);
        }

        public static void CheckLogin(ApplicationData AppData, Action<HttpResponseMessage> response)
        {
            CallMethod("CheckLogin", AuthForm(AppData), response);
        }

        public static void UpdateProjects(ApplicationData AppData, ProjectEntity[] projects, Action<HttpResponseMessage> response)
        {
            string data = JsonUtility.ToJson(projects);
            data = Secure.Base64Encode(data);

            var form = AuthForm(AppData);
            form.Add("Projects", data);

            CallMethod("UpdateProjects", form, response);
        }
    }
}
