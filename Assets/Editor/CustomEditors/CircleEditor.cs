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
            circle.circleTransparencySettled = EditorGUILayout.FloatField("Settled Circle Transparency: ", circle.circleTransparencySettled);
            circle.circleTransparencyGrowing = EditorGUILayout.FloatField("Growing Circle Transparency: ", circle.circleTransparencyGrowing);
            circle.outlinerTransparencySettled = EditorGUILayout.FloatField("Settled Outliner Transparency: ", circle.outlinerTransparencySettled);
            circle.outlinerTransparencyGrowing = EditorGUILayout.FloatField("Growing Outliner Transparency: ", circle.outlinerTransparencyGrowing);
        }

        EditorUtility.SetDirty(target);
    }
}