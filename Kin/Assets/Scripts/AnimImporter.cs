using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;

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

        Debug.Log(objName);

        // load gif containing values for frame duration
        //Image gif = Image.FromFile("Gifs/"+objName+".gif");
        object gif = null;

        // Number of frames
        int frameCount = 0/*gif.GetFrameCount(new FrameDimension(gif.FrameDimensionsList[0]))*/;
        //Image.GetPropertyItem(0x5100);

        // if no gif exists, quit
        if (gif == null) return;

        // get frame durations from gif
        int[] frmDurs = new int[frameCount];
        for (int i = 0; i < frameCount; i++) {

        }

        int l0 = GCD(frmDurs);
        int s0 = (int)Mathf.Round(1000f / l0);

        foreach (AnimationClip aC in anim) {
            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(aC);
            Debug.Log(aC.name + ":" + events[0].time);

            // get start index of sprite for current animClip
            int start = 0;

            // get length of current animation
            int len = 0;

            //clear all sprites in animation

            //for each sprite, set an event to its time from duration (in ms or samples?)
            for (int i = start; i < len; i++) {
                Sprite sprite = Resources.Load("fruits_" + 1, typeof(Sprite)) as Sprite;
                Debug.Log(sprite.name);
            }
        }
    }

    static int GCD(int[] numbers) {
        return numbers.Aggregate(GCD);
    }

    static int GCD(int a, int b) {
        return b == 0 ? a : GCD(b, a % b);
    }
}
