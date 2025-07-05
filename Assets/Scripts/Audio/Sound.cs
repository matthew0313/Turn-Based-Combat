using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public float volume = 1.0f;
    public float pitch = 1.0f;
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Sound))]
public class Sound_Drawer : PropertyDrawer
{
    AudioSource testSource;
    bool foldout = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        foldout = EditorGUILayout.Foldout(foldout, label);
        if (foldout)
        {
            SerializedProperty clipProperty = property.FindPropertyRelative("clip");
            SerializedProperty volumeProperty = property.FindPropertyRelative("volume");
            SerializedProperty pitchProperty = property.FindPropertyRelative("pitch");

            EditorGUILayout.PropertyField(clipProperty);
            if (testSource != null) testSource.clip = clipProperty.objectReferenceValue as AudioClip;
            EditorGUILayout.PropertyField(volumeProperty);
            if (testSource != null) testSource.volume = volumeProperty.floatValue;
            EditorGUILayout.PropertyField(pitchProperty);
            if (testSource != null) testSource.pitch = pitchProperty.floatValue;

            if(GUILayout.Button("Play"))
            {
                if (testSource == null) testSource = new GameObject() { hideFlags = HideFlags.HideAndDontSave }.AddComponent<AudioSource>();
                testSource.clip = clipProperty.objectReferenceValue as AudioClip;
                testSource.volume = volumeProperty.floatValue;
                testSource.pitch = pitchProperty.floatValue;
                testSource.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                if(testSource != null) testSource.Pause();
            }
        }
    }
}
#endif