using UnityEngine;
using UnityEditor;
using System;
using NUnit.Framework;
using System.Linq;

[CustomEditor(typeof(AnimImporter))]
public class AnimImporterEditor : Editor {
    //AnimationUtility util = new AnimationUtility();

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Import Animations")) {
            ((AnimImporter)target).importAnims();
        }
    }
}
