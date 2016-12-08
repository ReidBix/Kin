using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using System.IO;
using System;

public class AnimImporter : MonoBehaviour {

    public GameObject go;

    public void importAnims() {
        if (go != null)
            importAnims(go.GetComponent<Animation>());
    }

    /// <summary>
    /// imports the sprites of a given animation with appropiate frame rates
    /// </summary>
    /// <param name="anim"></param>
    public void importAnims(Animation anim) {
        // get name of object
        SpriteRenderer obj = anim.gameObject.GetComponent<SpriteRenderer>();
        string objName = obj.sprite.name;
        objName = objName.Substring(0, objName.Length - 2);

        // load txt file containing values for frame durations
        string filename = "Gifs/" + objName + ".txt";
        if (!File.Exists(filename)) {
            Debug.Log("No dice, sweet pea");
            return;
        }

        string[] lines = File.ReadAllLines(filename)[0].
            Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Number of frames
        int frameCount = lines.Length;
        int[] frmDurs = new int[frameCount];

        // get frame durations from gif
        for (int i = 0; i < frameCount; i++)
            frmDurs[i] = int.Parse(lines[i].Trim());
        
        // for each clip, adjust frames
        foreach (AnimationClip aC in AnimationUtility.GetAnimationClips(go)) {
            EditorCurveBinding[] ecb = AnimationUtility.GetCurveBindings(aC);
            Debug.Log(ecb);
            //AnimationUtility.GetObjectReferenceCurve(aC, ecb);
            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(aC);
            Debug.Log(aC.name + ":" + events.Length);

            // get start index of sprite for current animClip
            int start = 0;
            // get length of current animation
            int len = 0;

            //clear all sprites in animation
            int[] frames = frmDurs.SubArray(start, len);

            bool dynamicRate = false;
            foreach (int i in frames)
                if (i != frames[0]) {
                    dynamicRate = true;
                    break;
                }

            float l0 = dynamicRate ? GCD(frames) : frames[0],
            s0 = (int)Math.Round(1000f / l0);

            // for each frame, place event at sample index
            int end = (dynamicRate) ? frames.Length + 1 : frames.Length;
            for (int i0 = 0; i0 < end; i0++) {
                int sample = i0;
                if (dynamicRate) {
                    sample = (int)(sum(frames, i0) / l0);
                    if ((i0 == frames.Length)) sample--;
                }

                Sprite sprite = Resources.Load(objName + "_" +(start+ i0), typeof(Sprite)) as Sprite;
                Debug.Log(sprite.name);
            }
        }
    }

    static int sum(int[] f, int i0) {
        int sum = 0;
        for (int i = 0; i < i0; i++) sum += f[i];
        return sum;
    }

    static int GCD(int[] numbers) {  return numbers.Aggregate(GCD); }
    static int GCD(int a, int b) { return b == 0 ? a : GCD(b, a % b); }
}

public static class LinqHelper {
    public static T[] SubArray<T>(this T[] a, int i, int len) {
        T[] res = new T[len];
        Array.Copy(a, i, res, 0, len);
        return res;
    }
}
