using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBalance : ScriptableObject
{
    public static string AssetPath = "Configs/GameBalance";
    [Header("Player Balance")]
    public AnimationCurve playerSpeedGraph;
    [Header("Active Enemy Balance")]
    public AnimationCurve activeEnemySpeedGraph;
    public AnimationCurve playerFindRefreshRate;
    [Header("Lazy Enemy Balance")]
    public AnimationCurve lazyEnemySpeedGraph;
    [Header("Patrol Enemy Balance")]
    public AnimationCurve patrolEnemySpeedGraph;

    public float GetPlayerSpeed(float value)
    {
        return playerSpeedGraph.Evaluate(value);
    }
    public float GetActiveEnemySpeed(float value)
    {
        return activeEnemySpeedGraph.Evaluate(value);
    }
    public float GetLazyEnemySpeed(float value)
    {
        return lazyEnemySpeedGraph.Evaluate(value);
    }
    public float GetPatrolEnemySpeed(float value)
    {
        return patrolEnemySpeedGraph.Evaluate(value);
    }
    public float GetPathFindRefreshRate(float value)
    {
        return playerFindRefreshRate.Evaluate(value);
    }
}
