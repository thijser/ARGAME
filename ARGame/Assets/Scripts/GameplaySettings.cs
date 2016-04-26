using UnityEngine;
using System.Collections;

public class GameplaySettings : MonoBehaviour {
    public bool EnablePlayerViews = true;
    public bool EnableOverview = true;
    public bool EnableFrustrums = true;
    public bool EnableIndicators = true;
    public bool EnableMonkeyHeads = true;

    public static GameplaySettings Instance {
        get; private set;
    }

    public void Start() {
        Instance = this;
    }
}
