using tech.onsen.vrc.ndmf.moveexmenu.runtime;
using UnityEditor;

namespace tech.onsen.vrc.ndmf.moveexmenu.editor
{
    [CustomEditor(typeof(MoveExpressionMenuItem))]
    public class MoveExpressionMenuItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Help:");
            EditorGUILayout.LabelField("- Sourceは '/' 区切りで階層を指定できます。");
            EditorGUILayout.LabelField("- Destinationを空にするとメニューが削除されます。");
            EditorGUILayout.LabelField("- Destinationの末尾が '/' の時、Sourceの項目がそのメニュー配下へ移動されます。");
            EditorGUILayout.LabelField("- Destinationの末尾が '/' でない時、Sourceの項目がその名前に変更されます。");
            EditorGUILayout.LabelField("- HasPositionがONの時");
            EditorGUILayout.LabelField("   * Positionが正の場合は先頭からの位置に項目が追加されます。");
            EditorGUILayout.LabelField("   * Positionが負の場合は末尾からの位置に項目が追加されます。");
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MoveExpressionMenuItem.replacements)), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
