using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Allows a sprite to be animated using a SpriteAnimation
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public delegate void SpriteAnimatorEvent(SpriteAnimator sender, SpriteAnimation animation);

    /// <summary>
    /// This field is used to check every frame if the current animation has changed.
    /// </summary>
    private SpriteAnimation _animation;

    /// <summary>
    /// Gets or sets the current animation. Setting a animation will reset the CurrentFrame value
    /// </summary>
    public SpriteAnimation Animation;

    /// <summary>
    /// Event triggered after the final frame of an animation is played
    /// </summary>
    public event SpriteAnimatorEvent OnAnimationCompleted;

    /// <summary>
    /// The sprite renderer of to animate
    /// </summary>
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_animation == Animation)
        {
            return;
        }

        _animation = Animation;

        StopAllCoroutines();
        if (_animation == null || _animation.FrameCount == 0)
        {
            return;
        }

        CurrentFrame = 0;
        StartCoroutine("Animate", Animation);
    }

    private int _currentFrame;

    /// <summary>
    /// Gets or set the current frame of animation
    /// </summary>
    public int CurrentFrame
    {
        get => _currentFrame;
        set => _currentFrame = Mathf.Clamp(value, 0, _animation.FrameCount);
    }

    private IEnumerator Animate()
    {
        if (_animation.FrameCount == 0)
        {
            yield break;
        }

        int loopCount = 0;
        while (true)
        {
            _renderer.sprite = _animation.Frames[CurrentFrame].Sprite;
            yield return new WaitForSeconds(_animation.FrameTime / _animation.FrameTimeDivider * _animation.Frames[CurrentFrame].FrameTimeMultiplier);

            //Increment/decrement current frame
            if (Animation.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.PingPong && loopCount % 2 == 1)
            {
                CurrentFrame = (CurrentFrame - 1 < 0 ? Animation.FrameCount - 1 : CurrentFrame - 1);
            }
            else
            {
                CurrentFrame = (CurrentFrame + 1) % _animation.FrameCount;
            }

            unchecked
            {
                if (CurrentFrame == Animation.FrameCount - 1)
                {
                    OnAnimationCompleted?.Invoke(this, Animation);

                    switch (Animation.PlaybackMode)
                    {
                        case SpriteAnimation.AnimationPlaybackMode.OneShot:
                            yield break;

                        case SpriteAnimation.AnimationPlaybackMode.Loop:
                            loopCount++;
                            break;

                        case SpriteAnimation.AnimationPlaybackMode.PingPong:
                            loopCount++;
                            continue;
                    }
                }
            }
           
        }
    }
}
