using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Movement.Assets._ObjectMovement.Scripts
{

    public static class DOTweenMover
    {
        public static Tween Move(Rigidbody rb, Vector3 target, float duration,
                                Ease ease, int loopCount, LoopType loopType)
        {
            if (!rb) return null;
            return Configure(rb.DOMove(target, duration), ease, loopCount, loopType);
        }

        public static Tween MoveX(Rigidbody rb, float x, float duration,
                                Ease ease, int loopCount, LoopType loopType)
        {
            if (!rb) return null;
            return Configure(rb.DOMoveX(x, duration), ease, loopCount, loopType);
        }

        public static Tween MoveY(Rigidbody rb, float y, float duration,
                                Ease ease, int loopCount, LoopType loopType)
        {
            if (!rb) return null;
            return Configure(rb.DOMoveY(y, duration), ease, loopCount, loopType);
        }

        public static Tween Jump(Rigidbody rb, Vector3 target, float power,
                                int jumps, float duration, Ease ease,
                                int loopCount, LoopType loopType)
        {
            if (!rb) return null;
            return Configure(rb.DOJump(target, power, jumps, duration),
                            ease, loopCount, loopType);
        }

        public static Tween Rotate(Rigidbody rb, Vector3 angles, float duration,
                                Ease ease, int loopCount, LoopType loopType)
        {
            if (!rb) return null;
            return Configure(rb.DORotate(angles, duration, RotateMode.Fast),
                            ease, loopCount, loopType);
        }

        public static Tween Scale(Transform t, Vector3 scale, float duration,
                                Ease ease, int loopCount, LoopType loopType)
        {
            if (!t) return null;
            return Configure(t.DOScale(scale, duration), ease, loopCount, loopType);
        }

        public static Tween Path(Rigidbody rb, Vector3[] path, float duration,
                                PathType type, PathMode mode, Ease ease,
                                int loopCount, LoopType loopType)
        {
            if (!rb || path == null || path.Length < 2) return null;
            return Configure(rb.DOPath(path, duration, type, mode),
                            ease, loopCount, loopType);
        }

        public static Tween LocalPath(Rigidbody rb, Vector3[] path, float duration,
                                    PathType type, PathMode mode, Ease ease,
                                    int loopCount, LoopType loopType)
        {
            if (!rb || path == null || path.Length < 2) return null;
            return Configure(rb.DOLocalPath(path, duration, type, mode),
                            ease, loopCount, loopType);
        }

        private static Tween Configure(Tween t, Ease ease, int loops, LoopType loopType)
        {
            if (t == null) return null;
            t.SetEase(ease);
            if (loops != 0) t.SetLoops(loops, loopType);
            return t;
        }
    }



}
