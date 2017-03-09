using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NourishmentLevels {
    public static float HighMultiplier = 100.0f;
    public static float LowMultiplier = 0.01f;
    private static double NourishmentThresholdN = 1.15;
    private static double HRegenerationN = 1.75;
    private static double DecayN = 1/1.05;
    private static double MsN = 1.65;
    private static double HealthN = System.Math.Pow(2, 1 / 2);
    public static double StaminaN = 2;
    private static double SRegenerationN = 0;

    public static Dictionary<int, float> NourishmentThreshold = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(NourishmentThresholdN, -2) * HighMultiplier },
        { -1, (float)System.Math.Pow(NourishmentThresholdN, -1) * HighMultiplier },
        { 0, (float)System.Math.Pow(NourishmentThresholdN, 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(NourishmentThresholdN, 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(NourishmentThresholdN, 2) * HighMultiplier }
    };

    public static Dictionary<int, float> BaseHealthRegeneration = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(HRegenerationN, -2) * LowMultiplier },
        { -1, (float)System.Math.Pow(HRegenerationN, -1) * LowMultiplier },
        { 0, (float)System.Math.Pow(HRegenerationN, 0) * LowMultiplier },
        { 1, (float)System.Math.Pow(HRegenerationN, 1) * LowMultiplier },
        { 2, (float)System.Math.Pow(HRegenerationN, 2) * LowMultiplier }
    };

    public static Dictionary<int, float> NourishmentDecayRate = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(DecayN, -2) * LowMultiplier },
        { -1, (float)System.Math.Pow(DecayN, -1) * LowMultiplier },
        { 0, (float)System.Math.Pow(DecayN, 0) * LowMultiplier },
        { 1, (float)System.Math.Pow(DecayN, 1) * LowMultiplier },
        { 2, (float)System.Math.Pow(DecayN, 2) * LowMultiplier }
    };

    public static Dictionary<int, float> BaseMovementSpeed = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(MsN, -2) },
        { -1, (float)System.Math.Pow(MsN, -1) },
        { 0, (float)System.Math.Pow(MsN, 0) },
        { 1, (float)System.Math.Pow(MsN, 1) },
        { 2, (float)System.Math.Pow(MsN, 2) }
    };

    public static Dictionary<int, float> BaseMaximumHealth = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(HealthN, -2) * HighMultiplier },
        { -1, (float)System.Math.Pow(HealthN, -1) * HighMultiplier },
        { 0, (float)System.Math.Pow(HealthN, 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(HealthN, 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(HealthN, 2) * HighMultiplier }

    };

    public static Dictionary<int, float> BaseMaximumStamina = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(System.Math.Pow(StaminaN, 1/2), -2) * HighMultiplier },
        { -1, (float)System.Math.Pow(System.Math.Pow(StaminaN, 1/2), -1) * HighMultiplier },
        { 0, (float)System.Math.Pow(System.Math.Pow(StaminaN, 1/2), 0) * HighMultiplier },
        { 1, (float)System.Math.Pow(System.Math.Pow(StaminaN, 1/2), 1) * HighMultiplier },
        { 2, (float)System.Math.Pow(System.Math.Pow(StaminaN, 1/2), 2) * HighMultiplier }
    };

    public static Dictionary<int, float> BaseStaminaRegeneration = new Dictionary<int, float>() {
        { -2, (float)System.Math.Pow(SRegenerationN, -2) * LowMultiplier },
        { -1, (float)System.Math.Pow(SRegenerationN, -1) * LowMultiplier },
        { 0, (float)System.Math.Pow(SRegenerationN, 0) * LowMultiplier },
        { 1, (float)System.Math.Pow(SRegenerationN, 1) * LowMultiplier },
        { 2, (float)System.Math.Pow(SRegenerationN, 2) * LowMultiplier }
    };                               
}
