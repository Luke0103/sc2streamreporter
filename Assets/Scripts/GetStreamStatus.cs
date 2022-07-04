using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HttpRequestManager;

public class ClientCredientialReturn
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonProperty("scope")]
    public List<string> Scope { get; set; }
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}

public class TwitchPlayerProfile
{
    [JsonProperty("broadcaster_language")]
    public string Language { get; set; }
    [JsonProperty("display_name")]
    public string Name { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("is_live")]
    public bool IsLive { get; set; }
}

public class GetStreamStatus : MonoBehaviour
{
    [SerializeField]
    Transform playerItemContentTransform;

    List<Tuple<string, string>> headers = new List<Tuple<string, string>>();

    string searchText;

    void Start()
    {
        string content = 
            (SendHttpRequest("POST", 
            $"https://id.twitch.tv/oauth2/token?client_id={clientID}&client_secret={clientSecret}&grant_type=client_credentials", 
            null));

        ClientCredientialReturn info = JsonConvert.DeserializeObject<ClientCredientialReturn>(content);
        
        Debug.Log(info.AccessToken);

        headers.Add(new Tuple<string, string>("Authorization", "Bearer " + info.AccessToken));
        headers.Add(new Tuple<string, string>("Client-Id", clientID));
    }

    public void SearchPlayer()
    {
        var infoList = GetPlayerList.PlayerInfoList;
        var nameList = GetPlayerList.PlayerNameList;

        List<string> lowerList = new List<string>();

        foreach (string s in nameList)
        {
            lowerList.Add(s.ToLower());
        }

        int index = lowerList.BinarySearch(searchText.ToLower());

        if (index < 0)
            return;

        GameObject obj = Instantiate(PrefabManager.Instance.PlayerInfoObject, playerItemContentTransform);
        PrefabManager.CurrentPlayerInfoObjects.Add(obj);
        PlayerObjectScript script = obj.GetComponent<PlayerObjectScript>();

        script.Nickname.text = nameList[index];

        bool twitch = GetTwitchStatus(script, infoList[index]);
        bool aftv = GetAFTVStatus(script, infoList[index]);

        if (!twitch && !aftv)
        {
            script.CurrentImage.color = new Color(1f, 0.8f, 0.8f);
            script.OnlineText.text = "Offline";
        }

        script.playerInfo = GetPlayerList.PlayerInfoList[index];
    }

    public void SetSearchText(string text)
    {
        searchText = text;
    }

    public void LoadFullList()
    {
        int index = 0;
        foreach (var p in GetPlayerList.PlayerInfoList)
        {
            GameObject obj = Instantiate(PrefabManager.Instance.PlayerInfoObject);
            PlayerObjectScript script = obj.GetComponent<PlayerObjectScript>();

            script.playerInfo = GetPlayerList.PlayerInfoList[index++];
            script.Nickname.text = p.ID;

            switch (script.playerInfo.Race)
            {
                case "T":
                    if (!FilterManager.Instance.IsTerranToggled)
                    {
                        obj.SetActive(false);
                        continue;
                    }
                    break;
                case "Z":
                    if (!FilterManager.Instance.IsZergToggled)
                    {
                        obj.SetActive(false);
                        continue;
                    }
                    break;
                case "P":
                    if (!FilterManager.Instance.IsProtossToggled)
                    {
                        obj.SetActive(false);
                        continue;
                    }
                    break;
            }

            bool twitch = GetTwitchStatus(script, p);
            bool aftv = GetAFTVStatus(script, p);

            if (!twitch && !aftv)
            {
                script.CurrentImage.color = new Color(1f, 0.8f, 0.8f);
                script.OnlineText.text = "Offline";
            }

            PrefabManager.CurrentPlayerInfoObjects.Add(obj);
        }

        PrefabManager.CurrentPlayerInfoObjects.Sort(PrefabManager.PlayerObjectSorter);

        foreach (var p in PrefabManager.CurrentPlayerInfoObjects)
        {
            p.transform.SetParent(playerItemContentTransform, false);
        }
    }

    bool GetTwitchStatus(PlayerObjectScript script, PlayerInfo info)
    {
        if (info.Twitch == null || info.Twitch.Equals(""))
        {
            script.SetTwitchLogo(false);
            return false;
        }

        JObject data = JObject.Parse(SendHttpRequest("GET", $"https://api.twitch.tv/helix/streams?user_login={info.Twitch}", headers));
        JArray dataArray = (JArray)data["data"];

        script.SetTwitchLogo(dataArray.HasValues);

        return dataArray.HasValues;
    }

    bool GetAFTVStatus(PlayerObjectScript script, PlayerInfo info)
    {
        if (info.AFTV == null || info.AFTV.Equals(""))
        {
            script.SetAFTVLogo(false);
            return false;
        }   

        var doc = new HtmlDocument();
        doc.LoadHtml(SendHttpRequest("GET", $"http://play.afreecatv.com/{info.AFTV}", null));

        string desc = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']").GetAttributeValue("content", null);

        if (desc.Equals("방송중이지 않습니다."))
        {
            script.SetAFTVLogo(false);
            return false;
        }

        return true;
    }

    public void ClearScrollView()
    {
        foreach (var obj in PrefabManager.CurrentPlayerInfoObjects)
        {
            Destroy(obj);
        }

        PrefabManager.CurrentPlayerInfoObjects.Clear();
    }
}
