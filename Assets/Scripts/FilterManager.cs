using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterManager : MonoBehaviour
{
    public static FilterManager Instance { get; private set; }

    public bool IsTerranToggled { get; set; }
    public bool IsZergToggled { get; set; }
    public bool IsProtossToggled { get; set; }

    void Awake()
    {
        IsTerranToggled = IsZergToggled = IsProtossToggled = true;
        Instance = this;
    }
}
