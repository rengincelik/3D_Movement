using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using DG.Tweening;

namespace Movement.Assets._ObjectMovement.Scripts
{

[CreateAssetMenu(fileName = "MovementData", menuName = "ScriptableObjects/MovementDataSO")]
public class MovementDataSO : ScriptableObject
{
    [Header("Movement Type")]
    public DOType doType;

    [Tooltip("For Move/Rotate: target position or rotation\nFor Jump: end position\nFor Scale: target scale")]
    public Vector3 targetValue;
    public float toEnd;

    [Header("Jump Settings")]
    [Tooltip("Height of the jump arc")]
    public float jumpPower = 2f;
    [Tooltip("Number of jumps in the animation")]
    public int jumpCount = 1;

    [Header("Path Settings")]
    [Tooltip("Array of waypoints for path movement (2D)")]
    public Vector3[] pathPoints;
    public PathType pathType = PathType.Linear;
    public PathMode pathMode = PathMode.Full3D;

    [Header("Timing")]
    [Tooltip("Duration of the entire animation")]
    public float duration = 1f;
    public Ease ease = Ease.Linear;
    [Tooltip("Delay before animation starts")]
    public float delay = 0f;

    [Header("Loop")]
    public LoopTypeCustom loopType = LoopTypeCustom.Loop;
    [Tooltip("-1 for infinite loops, 0 for no loop, positive number for specific loops")]
    public int loopCount = -1;

    [Header("Options")]
    [Tooltip("If true, targetValue is added to current value. If false, it's an absolute target")]
    public bool useRelativeValues = false;


    /// <summary>
    /// Validates the movement data and returns true if valid
    /// </summary>
    public bool Validate(out string error)
    {
        error = string.Empty;

        if (duration <= 0f)
        {
            error = "Duration must be greater than 0";
            return false;
        }

        if (doType == DOType.Path || doType == DOType.LocalPath)
        {
            if (pathPoints == null || pathPoints.Length < 2)
            {
                error = "Path movement requires at least 2 waypoints";
                return false;
            }
        }

        if (doType == DOType.Jump && jumpCount < 1)
        {
            error = "Jump count must be at least 1";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the target position considering relative values
    /// </summary>
    public Vector3 GetTargetPosition(Vector3 currentPosition)
    {
        return useRelativeValues ? currentPosition + targetValue : targetValue;
    }

    /// <summary>
    /// Gets path points with optional relative offset
    /// </summary>
    public Vector3[] GetPathPoints(Vector3 currentPosition)
    {
        if (pathPoints == null || pathPoints.Length == 0)
            return new Vector3[0];

        if (!useRelativeValues)
            return pathPoints;

        Vector3[] relativePath = new Vector3[pathPoints.Length];
        Vector3 offset = currentPosition;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            relativePath[i] = pathPoints[i] + offset;
        }

        return relativePath;
    }
}

public enum DOType
{
    Move,      // Move in 2D space (XY)
    MoveX,     // Move only on X axis
    MoveY,     // Move only on Y axis
    Jump,      // Jump with arc
    Rotate,    // Rotate in 2D (Z axis)
    Path,      // Follow path in world space
    LocalPath  // Follow path in local space
}

public enum LoopTypeCustom
{
    None,      // Play once, no loop
    Loop,      // Restart from beginning
    PingPong   // Play forward then backward
}

}
