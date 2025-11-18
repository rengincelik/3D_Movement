using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;


namespace Movement.Assets._ObjectMovement.Scripts
{

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Header("Setup")]
    public MovementDataSO movementData;
    public bool autoStart = true;

    [Header("Callbacks")]
    public UnityEvent onMovementStart;
    public UnityEvent onMovementComplete;
    public UnityEvent onMovementKilled;

    [Header("Debug")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;
    public Color targetGizmoColor = Color.green;

    private Tween activeTween;
    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private bool isMoving;

    public bool IsMoving => isMoving && activeTween != null && activeTween.IsActive();
    public float Progress => activeTween != null && activeTween.IsActive() ?
        activeTween.ElapsedPercentage() : 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        CacheStartTransform();
    }

    void Start()
    {
        if (autoStart && movementData != null)
        {
            StartMovement();
        }
    }

    private void CacheStartTransform()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
    }

    public void StartMovement()
    {
        if (movementData == null)
        {
            Debug.LogWarning($"[{name}] MovementDataSO is not assigned!");
            return;
        }

        // Validate movement data
        if (!movementData.Validate(out string error))
        {
            Debug.LogError($"[{name}] Invalid movement data: {error}");
            return;
        }

        KillActiveTween();

        LoopType doTweenLoop = ConvertLoopType(movementData.loopType);

        // Create appropriate tween based on type
        activeTween = CreateTween(doTweenLoop);

        if (activeTween != null)
        {
            ConfigureTween(activeTween);
            isMoving = true;
            onMovementStart?.Invoke();
        }
    }

    private Tween CreateTween(LoopType doTweenLoop)
    {
        Vector3 targetValue = movementData.GetTargetPosition(transform.position);

        switch (movementData.doType)
        {
            case DOType.Move:
                return DOTweenMover.Move(
                    rb, targetValue,
                    movementData.duration, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );

            case DOType.MoveX:
                return DOTweenMover.MoveX(
                    rb, movementData.toEnd,
                    movementData.duration, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );

            case DOType.MoveY:
                return DOTweenMover.MoveY(
                    rb, movementData.toEnd,
                    movementData.duration, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );

            case DOType.Jump:
                return DOTweenMover.Jump(
                    rb, targetValue,
                    movementData.jumpPower, movementData.jumpCount,
                    movementData.duration, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );



            case DOType.Path:
                Vector3[] pathPoints = movementData.GetPathPoints(transform.position);
                return DOTweenMover.Path(
                    rb, pathPoints,
                    movementData.duration, movementData.pathType,
                    movementData.pathMode, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );

            case DOType.LocalPath:
                Vector3[] localPathPoints = movementData.GetPathPoints(transform.position);
                return DOTweenMover.LocalPath(
                    rb, localPathPoints,
                    movementData.duration, movementData.pathType,
                    movementData.pathMode, movementData.ease,
                    movementData.loopCount, doTweenLoop
                );

            default:
                Debug.LogError($"[{name}] Unhandled DOType: {movementData.doType}");
                return null;
        }
    }

    private void ConfigureTween(Tween tween)
    {
        tween.SetAutoKill(false);

        if (movementData.delay > 0f)
        {
            tween.SetDelay(movementData.delay);
        }

        tween.OnComplete(() =>
        {
            isMoving = false;
            onMovementComplete?.Invoke();
        });

        tween.OnKill(() =>
        {
            isMoving = false;
            onMovementKilled?.Invoke();
        });
    }

    public void StopMovement()
    {
        KillActiveTween();
        isMoving = false;
    }

    public void PauseMovement()
    {
        if (activeTween != null && activeTween.IsActive() && activeTween.IsPlaying())
        {
            activeTween.Pause();
        }
    }

    public void ResumeMovement()
    {
        if (activeTween != null && activeTween.IsActive() && !activeTween.IsPlaying())
        {
            activeTween.Play();
        }
    }

    public void RestartMovement()
    {
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Restart();
        }
        else
        {
            StartMovement();
        }
    }

    public void ResetToStart()
    {
        StopMovement();
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;
    }

    public void RecacheStartTransform()
    {
        CacheStartTransform();
    }

    private void KillActiveTween()
    {
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill();
        }
    }

    LoopType ConvertLoopType(LoopTypeCustom customLoop)
    {
        return customLoop switch
        {
            LoopTypeCustom.None => LoopType.Restart,
            LoopTypeCustom.Loop => LoopType.Restart,
            LoopTypeCustom.PingPong => LoopType.Yoyo,
            _ => LoopType.Restart
        };
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || movementData == null) return;

        Gizmos.color = gizmoColor;

        Vector3 startPos = Application.isPlaying ? startPosition : transform.position;
        Vector3 targetPos;

        switch (movementData.doType)
        {
            case DOType.Move:
            case DOType.Jump:
                targetPos = movementData.GetTargetPosition(startPos);
                DrawMovementGizmo(startPos, targetPos);
                break;

            case DOType.MoveX:
                targetPos = new Vector3(
                    movementData.useRelativeValues ? startPos.x + movementData.targetValue.x : movementData.targetValue.x,
                    startPos.y,
                    startPos.z
                );
                DrawMovementGizmo(startPos, targetPos);
                break;

            case DOType.MoveY:
                targetPos = new Vector3(
                    startPos.x,
                    movementData.useRelativeValues ? startPos.y + movementData.targetValue.y : movementData.targetValue.y,
                    startPos.z
                );
                DrawMovementGizmo(startPos, targetPos);
                break;


            case DOType.Rotate:
                // Draw rotation arc
                float currentAngle = Application.isPlaying ? startRotation.eulerAngles.z : transform.eulerAngles.z;
                float targetAngle = movementData.useRelativeValues
                    ? currentAngle + movementData.toEnd
                    : movementData.toEnd;

                DrawRotationGizmo(startPos, currentAngle, targetAngle);
                break;

            case DOType.Path:
            case DOType.LocalPath:
                if (movementData.pathPoints != null && movementData.pathPoints.Length > 1)
                {
                    Vector3[] points = movementData.GetPathPoints(startPos);
                    DrawPathGizmo(points);
                }
                break;
        }
    }

    void DrawMovementGizmo(Vector3 from, Vector3 to)
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(from, 0.2f);

        Gizmos.color = targetGizmoColor;
        Gizmos.DrawWireSphere(to, 0.2f);

        DrawArrow(from, to);
    }

    void DrawPathGizmo(Vector3[] points)
    {
        if (points == null || points.Length < 2) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(points[0], 0.15f);

        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i - 1], points[i]);
            Gizmos.DrawWireSphere(points[i], 0.15f);
        }

        Gizmos.color = targetGizmoColor;
        Gizmos.DrawWireSphere(points[points.Length - 1], 0.2f);
    }

    void DrawRotationGizmo(Vector3 center, float fromAngle, float toAngle)
    {
        float radius = 0.5f;
        int segments = 20;

        float angleDiff = Mathf.DeltaAngle(fromAngle, toAngle);
        float angleStep = angleDiff / segments;

        Gizmos.color = gizmoColor;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = (fromAngle + angleStep * i) * Mathf.Deg2Rad;
            float angle2 = (fromAngle + angleStep * (i + 1)) * Mathf.Deg2Rad;

            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Gizmos.DrawLine(p1, p2);
        }

        // Draw end arrow
        Gizmos.color = targetGizmoColor;
        float endAngle = toAngle * Mathf.Deg2Rad;
        Vector3 endPos = center + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle), 0) * radius;
        Gizmos.DrawWireSphere(endPos, 0.1f);
    }

    void DrawArrow(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        if (direction.sqrMagnitude < 0.001f) return;

        Vector3 right = Vector3.Cross(Vector3.forward, direction);
        if (right.sqrMagnitude < 0.001f)
            right = Vector3.Cross(Vector3.up, direction);

        right = right.normalized * 0.2f;
        Vector3 arrowTip = to - direction * 0.3f;

        Gizmos.color = targetGizmoColor;
        Gizmos.DrawLine(to, arrowTip + right);
        Gizmos.DrawLine(to, arrowTip - right);
    }

    void OnDestroy()
    {
        KillActiveTween();
    }
}



}
