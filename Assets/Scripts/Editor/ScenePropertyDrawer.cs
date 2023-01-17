using UnityEditor;
using UnityEngine;

using Utility;

namespace Core.Editor
{
    //  https://forum.unity.com/threads/how-to-link-scenes-in-the-inspector.383140/
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneAsset sceneObject = GetSceneObject(property.stringValue, out string path);
                Object scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);
                if (scene == null)
                {
                    property.stringValue = "";
                }
                else if (scene.name != property.stringValue)
                {
                    SceneAsset sceneObj = GetSceneObject(scene.name, out path);
                    if (sceneObj == null)
                    {
                        UnityEngine.Debug.LogWarning("The scene " + scene.name + " cannot be used. To use this scene add it to the build settings for the project");
                    }
                    else
                    {
                        property.stringValue = path;
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
            }
        }
        protected SceneAsset GetSceneObject(string sceneObjectName, out string path)
        {
            path = "";

            if (string.IsNullOrEmpty(sceneObjectName)) return null;

            foreach (var editorScene in EditorBuildSettings.scenes)
            {
                if (editorScene.path.IndexOf(sceneObjectName) != -1)
                {
                    path = editorScene.path.Replace("Assets/", "").Replace(".unity", "");
                    return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
                }
            }

            UnityEngine.Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
            return null;
        }
    }
}
