using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenStream : MonoBehaviour
{
    [SerializeField]
    PlayerObjectScript parent;

    /* site index
     * aftv = 1
     * twitch = 2
     * trovo = 3
     */

    public void OpenBrowser(int site)
    {
        string url = "";

        switch (site)
        {
            case 1:
                url = $"http://play.afreecatv.com/{parent.playerInfo.AFTV}";
                break;
            case 2:
                url = $"https://www.twitch.tv/{parent.playerInfo.Twitch}";
                break;
        }
        
        System.Diagnostics.Process.Start(url);
    }
}
