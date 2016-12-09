using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using System.IO;
using System;
using LitJson;
using System.Collections.Generic;

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
        string filename = "Gifs/" + objName + ".json";
        if (!File.Exists(filename)) {
            Debug.LogError("Please extract data from the ase files using Anim_Extractor.exe before attempting to import animation data.");
            return;
        }

        AnimDat animJSON = readJSON(filename);
        if (animJSON == null) {
            Debug.LogError("Error parsing \""+filename+"\".");
            return;
        }
        
        // for each clip, adjust frames
        foreach (AnimationClip aC in AnimationUtility.GetAnimationClips(go)) {
            //clear all sprites in animation
            
            AnimDat.Clip clip = animJSON[aC.name];
            if (clip == null) {
                Debug.LogError(objName + ": Animation Clip \"" + aC.name + "\" does not exist in the ase file."
                    + "\nCannot import its animation data.");
                continue;
            }

            bool dynamicRate = false;
            foreach (int i in clip.frames)
                if (i != clip.frames[0]) {
                    dynamicRate = true;
                    break;
                }

            float l0 = dynamicRate ? GCD(clip.frames) : clip.frames[0], // sample duration
            s0 = (int)Math.Round(1000f / l0); //sample rate
            Debug.Log(objName+": "+clip.name+"\ts0: "+s0);

            // for each frame, place event at sample index
            int end = (dynamicRate) ? clip.len + 1 : clip.len;
            for (int i0 = 0; i0 < end; i0++) {
                int sample = i0;
                if (dynamicRate) {
                    sample = (int)(sum(clip.frames, i0) / l0);
                    if ((i0 == clip.len)) sample--;
                }

                string sprName = objName + "_" + (clip.start + i0);
                Sprite sprite = Resources.Load(sprName, typeof(Sprite)) as Sprite;
                //Debug.Log(sprName);
            }
        }
    }

    /// <summary>
    /// Reads animation data from an extracted json file.
    /// </summary>
    /// <param name="file"> location of the json file </param>
    /// <returns></returns>
    private AnimDat readJSON(string file) {
        AnimDat anim = new AnimDat();
        JsonData dat = JsonMapper.ToObject(File.ReadAllText(file));
        JsonData frames = dat["frames"];
        JsonData tags = dat["meta"]["frameTags"];
        anim.clips = new List<AnimDat.Clip>();

        anim.durations = new int[frames.Count];
        for(int i = 0; i<frames.Count; i++) {
            anim.durations[i] = int.Parse(frames[i]["duration"].ToString());
        }

        // load loop names
        for(int i = 0; i<tags.Count; i++) {
            anim.clips.Add(new AnimDat.Clip(anim,
                tags[i]["name"].ToString(),
                int.Parse(tags[i]["from"].ToString()),
                int.Parse(tags[i]["to"].ToString())));
        }
        return anim;
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

public class AnimDat {
    public int[] durations;
    public List<Clip> clips;
    public int frameCount { get { return durations.Length; } }
    public Clip this[string clipName]{
        get {
            foreach (Clip c in clips)
                if (c.name.ToLower().Equals(clipName.ToLower()))
                    return c;
            return null;
        }
    }

    public class Clip {
        public string name;
        public int start, end;
        private AnimDat owner;

        public int len { get { return end - start + 1; } }
        public int[] frames { get { return owner.durations.SubArray(start, len); } }

        public Clip(AnimDat owner, string name, int s, int e) {
            this.owner = owner;
            this.name = name;
            start = s;
            end = e;
        }
    }
}
