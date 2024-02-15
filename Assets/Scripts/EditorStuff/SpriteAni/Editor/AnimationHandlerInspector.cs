using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteAnimation))]
public class AnimationHandlerInspector : Editor
{
    /// <summary>
    /// Draws the icon on the file thumbnails in the browser
    /// </summary>
    static AnimationHandlerInspector()
    {
        EditorApplication.ProjectWindowItemCallback currentBehaviour = EditorApplication.projectWindowItemOnGUI;
        EditorApplication.projectWindowItemOnGUI = (guid, rect) =>
        {
            currentBehaviour?.Invoke(guid, rect);

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            SpriteAnimation obj = AssetDatabase.LoadAssetAtPath<SpriteAnimation>(assetPath);

            if (obj != null)
            {
                if (obj.Frames[0].Sprite == null)
                {
                    return;
                }
                GUI.DrawTexture(new Rect(new Bounds(rect.center, rect.size * 0.1f).min, rect.size * 0.1f), AssetPreview.GetAssetPreview(obj.Frames[0].Sprite));
            }
        };
    }

    /// <summary>
    /// Draws the actual inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorUtility.SetDirty(target);
        SpriteAnimation _ = target as SpriteAnimation;

        //Title bar
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Frame Time (" + _.FrameTime + "/" + _.FrameTimeDivider + "=" + _.FrameTime / _.FrameTimeDivider + "s):");
            _.FrameTime = EditorGUILayout.FloatField(_.FrameTime);
            _.FrameTimeDivider = EditorGUILayout.FloatField(_.FrameTimeDivider);

			
        }
        EditorGUILayout.EndHorizontal();

        //The playback mode selector
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(_.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.Loop, "Loop"))
            {
                _.PlaybackMode = SpriteAnimation.AnimationPlaybackMode.Loop;
            }

            if (GUILayout.Toggle(_.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.PingPong, "Ping-Pong"))
            {
                _.PlaybackMode = SpriteAnimation.AnimationPlaybackMode.PingPong;
            }

            if (GUILayout.Toggle(_.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.OneShot, "One Shot"))
            {
                _.PlaybackMode = SpriteAnimation.AnimationPlaybackMode.OneShot;
            }

        }
        EditorGUILayout.EndHorizontal();

	
		GUILayout.Space(15);
		EditorGUILayout.LabelField("Tags");

		//The animation tags used as references when calling animations
		EditorGUILayout.BeginHorizontal();
		{
			SerializedObject so = new SerializedObject(_);
			SerializedProperty stringsProperty = so.FindProperty("tags");
			EditorGUILayout.PropertyField(stringsProperty, true);
			so.ApplyModifiedProperties();
		}
		EditorGUILayout.EndHorizontal();



		GUILayout.Space(20);
        EditorGUILayout.LabelField("Frames");

        /*
         * Frames
         */
        List<SpriteAnimation.SpriteFrame> frames = _.Frames.ToList();

        for (int n = 0; n < _.FrameCount; n++)
        {
            SpriteAnimation.SpriteFrame i = _.Frames[n];

            if (n == _.LoopPoint && _.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.Loop)
            {
                GUI.backgroundColor = Color.red;
            }

            EditorGUILayout.BeginHorizontal();
            {
                //The thumbnail
                i.Sprite = EditorGUILayout.ObjectField(i.Sprite, typeof(Sprite), false, GUILayout.Width(90), GUILayout.Height(90)) as Sprite;

                EditorGUILayout.BeginVertical();
                {
                    //The delete button (Only shown when there this more than one frame)
                    if (_.FrameCount > 1 && GUILayout.Button("✕", GUILayout.MaxWidth(48)))
                    {
                        frames.RemoveAt(n);
                    }

                    //The up button (Only shown when the frame is not topmost)
                    if (n != 0 && GUILayout.Button("↑", GUILayout.MaxWidth(48)))
                    {
                        frames.RemoveAt(n);
                        frames.Insert(Mathf.Max(0, n - 1), i);
                    }

                    //The loop button
                    if (_.LoopPoint != n && _.PlaybackMode == SpriteAnimation.AnimationPlaybackMode.Loop && GUILayout.Button("Loop", GUILayout.MaxWidth(48)))
                    {
                        _.LoopPoint = n;
                    }

                    //The down button (Only shown when the frame is not bottommost)
                    if (n != _.FrameCount - 1 && GUILayout.Button("↓", GUILayout.MaxWidth(48)))
                    {
                        frames.RemoveAt(n);
                        frames.Insert(Mathf.Min(frames.Count, n + 1), i);
                    }

                    //The add button
                    if (GUILayout.Button("+", GUILayout.MaxWidth(48)))
                    {
                        frames.Insert(n + 1, new SpriteAnimation.SpriteFrame());
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;

            //The frame time multiplier
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Frame Time Multiplier: ");
                i.FrameTimeMultiplier = EditorGUILayout.FloatField(i.FrameTimeMultiplier);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
        }

        //Updates the frames of the animation
        if (!_.Frames.SequenceEqual(frames))
        {
            _.Frames = frames.ToArray();
        }
    }
}       