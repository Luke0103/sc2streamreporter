using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    static PrefabManager instance;
    public static PrefabManager Instance { get { return instance; } }

    [SerializeField]
    GameObject playerInfoObject;
    public GameObject PlayerInfoObject { get { return playerInfoObject; } }

    public static List<GameObject> CurrentPlayerInfoObjects { get; set; }

    void Awake()
    {
        CurrentPlayerInfoObjects = new List<GameObject>();
        instance = this;
    }

    public static int PlayerObjectSorter(GameObject go1, GameObject go2)
    {
        PlayerObjectScript script1, script2;
        script1 = go1.GetComponent<PlayerObjectScript>();
        script2 = go2.GetComponent<PlayerObjectScript>();

        bool status1, status2;
        status1 = script1.OnlineText.text == "Online";
        status2 = script2.OnlineText.text == "Online";
        
        if (status1 && !status2)
        {
            return -1;
        }
        else if (!status1 && status2)
        {
            return 1;
        }

        return 0;
    }
}
