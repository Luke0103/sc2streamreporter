using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class HttpRequestManager : MonoBehaviour
{
    public static string clientID = "jyu1hxazewimmyyjyh0ljk143eq29m";
    public static string clientSecret = "9a7wttyowd9285v5kodjh97llaxc8b";
    public static string localHost = "http://localhost";
    public static string Token { get; set; }

    public static string SendHttpRequest(string method, string uri, List<Tuple<string, string>> headerPairs = null)
    {
        string result;

        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod(method), uri))
            {
                if (headerPairs != null)
                    for (int i = 0; i < headerPairs.Count; i++)
                        request.Headers.Add(headerPairs[i].Item1, headerPairs[i].Item2);

                var response = Task.Run(() => httpClient.SendAsync(request)).Result;

                result = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
            }
        }

        return result;
    }
}
