using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class PlayerInfo
{
    public string ID { get; set; }
    public string Race { get; set; }
    public string Twitch { get; set; }
    public string AFTV { get; set; }
    public string Trovo { get; set; }
}

public class GetPlayerList : MonoBehaviour
{
    public static string clientID = "180290445728-qo8pspgcmic157m5v8fpnmcflflant6g.apps.googleusercontent.com";
    public static string clientSecret = "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";

    string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    string applicationName = "SC2StreamReporter";

    readonly int playerIndex = 0;
    readonly int raceIndex = 1;
    readonly int afreecaIndex = 2;
    readonly int twitchIndex = 3;

    public static List<PlayerInfo> PlayerInfoList { get; set; }
    public static List<string> PlayerNameList { get; set; }

    void Start()
    {
        UserCredential credential;

        PlayerInfoList = new List<PlayerInfo>();
        PlayerNameList = new List<string>();

        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);
        }

        // Create Google Sheets API service.
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName,
        });

        // Define request parameters.
        string spreadsheetId = "1QWJrDYs1AXc2dzQEjlUEADS2e4KFdrXhC-uVUcArkM4";
        string range = "Sheet1!A2:E999";
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);
        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        for (int i = 0; i < values.Count; i++)
        {
            if (values[i][playerIndex] == null || values[i][playerIndex].ToString() == "")
            {
                break;
            }
            PlayerInfo info = new PlayerInfo();

            info.ID = values[i][playerIndex].ToString();
            info.Race = values[i][raceIndex].ToString();
            try
            {
                info.AFTV = values[i][afreecaIndex].ToString();
            }
            catch
            {
                
            }
            
            try
            {
                info.Twitch = values[i][twitchIndex].ToString();
            }
            catch
            {

            }
            //info.Trovo = values[i][trovoIndex].ToString();

            PlayerInfoList.Add(info);
            PlayerNameList.Add(info.ID);
        }
    }
}
