using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NourishmentLevels {
    public static float HighMultiplier = 100.0f;
    public static float LowMultiplier = 0.01f;
    public static float VeryLowMultiplier = 0.005f;
    private static double NourishmentThresholdN = 1.15;
    private static double HRegenerationN = 1.9;
    private static double DecayN = 1/1.1;
    private static double MsN = 1.45;
    private static double HealthN = Mathf.Sqrt(2);
    public static double StaminaN = Mathf.Sqrt(2.1f);
    private static double SRegenerationN = 1 / 0.5;

    public static Dictionary<int, float> NourishmentThreshold = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(NourishmentThresholdN, -2) * HighMultiplier },
        { -1, (float)System.Math.Pow(NourishmentThresholdN, -1) * HighMultiplier },
        { 0, (float)System.Math.Pow(NourishmentThresholdN, 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(NourishmentThresholdN, 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(NourishmentThresholdN, 2) * HighMultiplier }
    };

    public static Dictionary<int, float> BaseHealthRegeneration = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(HRegenerationN, -2) },
        { -1, (float)System.Math.Pow(HRegenerationN, -1) },
        { 0, (float)System.Math.Pow(HRegenerationN, 0)  },
        { 1, (float)System.Math.Pow(HRegenerationN, 1) },
        { 2, (float)System.Math.Pow(HRegenerationN, 2)}
    };

    public static Dictionary<int, float> NourishmentDecayRate = new Dictionary<int, float>() {
        { -2, (float)(System.Math.Pow(DecayN, -2) - 0.5)  },
        { -1, (float)(System.Math.Pow(DecayN, -1) - 0.5)  },
        { 0, (float)(System.Math.Pow(DecayN, 0) - 0.5)  },
        { 1, (float)(System.Math.Pow(DecayN, 1) - 0.5)  },
        { 2, (float)(System.Math.Pow(DecayN, 2) - 0.5) }
    };

    public static Dictionary<int, float> BaseMovementSpeed = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(MsN, -2) + 0.5f},
        { -1, (float)System.Math.Pow(MsN, -1) + 0.5f },
        { 0, (float)System.Math.Pow(MsN, 0) + 0.5f },
        { 1, (float)System.Math.Pow(MsN, 1) + 0.5f },
        { 2, (float)System.Math.Pow(MsN, 2) + 0.5f }
    };

    public static Dictionary<int, float> BaseMaximumHealth = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(HealthN, -2) * HighMultiplier },
        { -1, (float)System.Math.Pow(HealthN, -1) * HighMultiplier },
        { 0, (float)System.Math.Pow(HealthN, 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(HealthN, 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(HealthN, 2) * HighMultiplier }

    };

    public static Dictionary<int, float> BaseMaximumStamina = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(StaminaN, 2) * HighMultiplier },
        { -1, (float)System.Math.Pow(StaminaN, -2) * HighMultiplier },
        { 0, (float)System.Math.Pow(StaminaN, 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(StaminaN, 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(StaminaN, 2) * HighMultiplier }
    };

    public static Dictionary<int, float> BaseStaminaRegeneration = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(SRegenerationN, -2) * VeryLowMultiplier },
        { -1, (float)System.Math.Pow(SRegenerationN, -1) * VeryLowMultiplier },
        { 0, (float)System.Math.Pow(SRegenerationN, 0) * VeryLowMultiplier },
        { 1, (float)System.Math.Pow(SRegenerationN, 1) * VeryLowMultiplier },
        { 2, (float)System.Math.Pow(SRegenerationN, 2) * VeryLowMultiplier }
    };                               
}
