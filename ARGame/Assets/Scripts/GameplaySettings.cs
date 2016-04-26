using UnityEngine;
using System.Collections;
using System;

// These settings can only be changed in the editor.
// At least one of the player views/overview settings must be enabled (for obvious reasons).
// The overview setting is ignored if there are no local players (because then there's no choice but to show the overview).
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

        if (!EnableOverview && !EnablePlayerViews) {
            throw new Exception("You can't disable both the overview and player views!");
        }
    }
}
