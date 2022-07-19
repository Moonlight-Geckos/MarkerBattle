using UnityEditor;
[CustomEditor(typeof(Circle))]
public class CircleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Circle circle = target as Circle;

        circle.Active = EditorGUILayout.Toggle("Active", circle.Active);

        if (circle.Active)
        {
            circle.PlayerOwnerNumber = EditorGUILayout.IntField("Player number: ", circle.PlayerOwnerNumber);
        }

        EditorUtility.SetDirty(target);
    }
}