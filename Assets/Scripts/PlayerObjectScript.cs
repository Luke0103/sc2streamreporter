using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectScript : MonoBehaviour
{
    [SerializeField]
    Text nickname;
    public Text Nickname { get { return nickname; } }

    [SerializeField]
    Button twitchLogo;
    public Button TwitchLogo { get { return twitchLogo; } }

    [SerializeField]
    Button aftvLogo;
    public Button AFTVLogo { get { return aftvLogo; } }

    [SerializeField]
    Text onlineText;
    public Text OnlineText { get { return onlineText; } }

    [SerializeField]
    Image currentImage;
    public Image CurrentImage { get { return currentImage; } }

    [SerializeField]
    Sprite aftvLogoBW;

    [SerializeField]
    Sprite twitchLogoBW;

    public PlayerInfo playerInfo { get; set; }

    public void SetNickname(string id)
    {
        nickname.text = id;
    }

    public void SetAFTVLogo(bool isLive)
    {
        if (!isLive)
        {
            aftvLogo.image.sprite = aftvLogoBW;
        }
    }

    public void SetTwitchLogo(bool isLive)
    {
        if (!isLive)
        {
            twitchLogo.image.sprite = twitchLogoBW;
        }
    }
}
