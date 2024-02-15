using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a sprite animation that can be applied to sprites and animated using SpriteAnimators
/// </summary>
[CreateAssetMenu(menuName = "Sprite Based Animation/Single Animation", fileName = "New Single Animation")]
public class SpriteAnimation : ScriptableObject
{
    /// <summary>
    /// The frames of this animation
    /// </summary>
    public SpriteFrame[] Frames = new SpriteFrame[] { new SpriteFrame() };


    /// <summary>
    /// The default frame time for every frame of this animation
    /// </summary>
    public float FrameTime = 1f;

    /// <summary>
    /// The time that the Frame time will be divided by
    /// </summary>
    public float FrameTimeDivider = 60f;

    /// <summary>
    /// The way this animation handles its playback
    /// </summary>
    public AnimationPlaybackMode PlaybackMode { get; set; }

	public AnimationTag[] tags;
	public List<string> tagz = new List<string>();

    /// <summary>
    /// The amount of frames this animation has
    /// </summary>
    public int FrameCount
    {
        get
        {
            return Frames.Length;
        }
    }

    /// <summary>
    /// Gets the index of the loop point of this animation. When the animation concludes it will loop back here if looping is enabled
    /// </summary>
    public int LoopPoint { get; set; }

    /// <summary>
    /// Represents a single frame of animation in a SpriteAnimation
    /// </summary>
    [System.Serializable]
    public class SpriteFrame
    {
        /// <summary>
        /// The sprite of this frame
        /// </summary>
        public Sprite Sprite;

        /// <summary>
        /// Number to be multiplied by the animations FrameTime
        /// </summary>
        public float FrameTimeMultiplier = 1f;

        public override bool Equals(object obj)
        {
            if (obj is SpriteFrame)
            {
                return this == obj as SpriteFrame;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Sprite.GetHashCode() ^ FrameTimeMultiplier.GetHashCode();
        }

        public static bool operator ==(SpriteFrame left, SpriteFrame right)
        {
            return left.Sprite == right.Sprite && left.FrameTimeMultiplier == right.FrameTimeMultiplier;
        }

        public static bool operator !=(SpriteFrame left, SpriteFrame right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Defines the different ways a sprite animation can run
    /// </summary>
    public enum AnimationPlaybackMode
    {
        /// <summary>
        /// The animation will loop
        /// </summary>
        Loop,

        /// <summary>
        /// The animation will not loop, idling at the last frame
        /// </summary>
        OneShot,

        /// <summary>
        /// When the animation is done, it will play backwards
        /// </summary>
        PingPong
    }

	[System.Serializable]
	public class AnimationTag
	{
		public string tagID;
	}
}

