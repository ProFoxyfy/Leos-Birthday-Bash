using System.Collections.Generic;
using UnityEngine;

namespace TweenX
{
    public class TweenManager : MonoBehaviour
    {
        public bool useUnscaled { private get; set; }
        private Dictionary<TweenRef, ActiveTween> tweens = new Dictionary<TweenRef, ActiveTween>();
        private Dictionary<TweenRef, float> deltas = new Dictionary<TweenRef, float>();
        private Queue<TweenRef> tweenQueue = new Queue<TweenRef>();

        public TweenRef PlayTween(ref XFloat target, Tween data)
        {
            TweenRef pointer = new TweenRef();
            tweens.Add(pointer, new ActiveTween(data, target, 0));
            deltas.Add(pointer, 0f);
            tweenQueue.Enqueue(pointer);
            return pointer;
        }

        public TweenRef PlayTweenSingle(ref XFloat target, Tween data)
        {
            CancelAllTweens();
            return PlayTween(ref target, data);
        }

        /// <summary>
        /// Stops a certain tween, but lets the next tween take over.
        /// If no other tweens are present, it will essentially freeze the value in place.
        /// </summary>
        /// <param name="pointer">Pointer to tween to cancel.</param>
        public void CancelTween(TweenRef pointer)
        {
            // Remove references, but don't reset delta
            tweens.Remove(pointer);
            deltas.Remove(pointer);
        }

        /// <summary>
        /// Stops a certain tween and resets it.
        /// </summary>
        /// <param name="pointer">Pointer to tween to stop.</param>
        public void StopTween(TweenRef pointer)
        {
            // Set delta to 0 (to stop tween) & remove references
            deltas[pointer] = 0f;
            tweens.Remove(pointer);
            deltas.Remove(pointer);
        }

        /// <summary>
        /// Stops all tweens, and resets them.
        /// </summary>
        public void StopAllTweens()
        {
            // Iterate through TweenRefs and stop them
            while (tweenQueue.Count > 0)
            {
                TweenRef pointer = tweenQueue.Dequeue();
                StopTween(pointer);
            }
        }

        /// <summary>
        /// Cancels all tweens.
        /// </summary>
        public void CancelAllTweens()
        {
            // Iterate through TweenRefs and stop them
            while (tweenQueue.Count > 0)
            {
                TweenRef pointer = tweenQueue.Dequeue();
                CancelTween(pointer);
            }
        }

        private void Update()
        {
            while (tweenQueue.Count > 0)
            {
                TweenRef pointer = tweenQueue.Dequeue();
                ActiveTween activeT = tweens[pointer];
                float newVal = Mathf.Lerp(
                    activeT.data.start,
                    activeT.data.end,
                    deltas[pointer] / activeT.data.duration
                );
                deltas[pointer] += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                activeT.target.Set(newVal);
                if (deltas[pointer] / activeT.data.duration >= 1f)
                {
                    this.CancelTween(pointer);
                }
            }
            foreach (TweenRef tween in tweens.Keys)
            {
                tweenQueue.Enqueue(tween);
            }
        }
    }
}