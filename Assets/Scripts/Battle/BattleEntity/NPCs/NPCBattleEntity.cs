using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class NPCBattleEntity : BattleEntity
{
    [Header("NPC")]
    [SerializeField] float m_maxHp = 100.0f;
    [SerializeField] AffinityTable affinities;
    public override float maxHp => m_maxHp;
    public override float hp { get; protected set; }
    public override void Set(int battlePosition, Alliance alliance)
    {
        base.Set(battlePosition, alliance);
        SetHP(maxHp);
        Debug.Log(affinities.table.Count);
    }
    protected virtual bool removeOnDeath => true;
    protected override void OnDeath()
    {
        base.OnDeath();
        if (removeOnDeath)
        {
            if (alliance == Alliance.Player)
            {
                BattleManager.Instance.playerSide[battlePosition] = null;
            }
            else
            {
                BattleManager.Instance.enemySide[battlePosition] = null;
            }
        }
    }
    readonly Dictionary<Element, Affinity> getBaseAffinitiesCache = new();
    protected override Dictionary<Element, Affinity> GetBaseAffinities()
    {
        getBaseAffinitiesCache.Clear();
        foreach (var i in affinities.table) getBaseAffinitiesCache[i.element] = i.affinity;
        return getBaseAffinitiesCache;
    }
}
[System.Serializable]
public class AffinityTable
{
    public List<ElementAffinityPair> table = new();
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AffinityTable))]
public class AffinityTable_Drawer : PropertyDrawer
{
    bool foldout = false;
    const float foldoutSize = 30.0f;
    const float rowSize = 20.0f;
    const float radioWidth = 20.0f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = foldoutSize;
        foldout = EditorGUI.Foldout(position, foldout, label.text);
        if (foldout)
        {
            position.y += foldoutSize;
            position.height = rowSize;
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 1.0f / 5.0f, width = position.width / 5.0f }, new GUIContent("None"));
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 2.0f / 5.0f, width = position.width / 5.0f }, new GUIContent("Weakness"));
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 3.0f / 5.0f, width = position.width / 5.0f }, new GUIContent("Resist"));
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 4.0f / 5.0f, width = position.width / 5.0f }, new GUIContent("Block"));

            SerializedProperty tableProperty = property.FindPropertyRelative("table");
            int tableSize = tableProperty.arraySize;

            List<ElementAffinityPair> table = new();
            for (int i = 0; i < tableSize; i++)
            {
                table.Add(new()
                {
                    element = (Element)tableProperty.GetArrayElementAtIndex(i).FindPropertyRelative("element").enumValueIndex,
                    affinity = (Affinity)tableProperty.GetArrayElementAtIndex(i).FindPropertyRelative("affinity").enumValueIndex
                });
            }
            bool changed = false;
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                if (element == Element.None) continue;

                position.y += rowSize;
                position.height = rowSize;

                var tmp = table.Find(item => item.element == element);
                Affinity affinity = tmp == null ? Affinity.None : tmp.affinity;

                EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 0.0f / 5.0f, width = position.width / 5.0f }, new GUIContent(element.ToString()));
                if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 1.0f / 5.0f, width = radioWidth }, affinity == Affinity.None) && affinity != Affinity.None)
                {
                    if (tmp != null) table.Remove(tmp);
                    changed = true;
                }
                if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 2.0f / 5.0f, width = radioWidth }, affinity == Affinity.Weakness) && affinity != Affinity.Weakness)
                {
                    if (tmp != null) tmp.affinity = Affinity.Weakness;
                    else table.Add(new() { element = element, affinity = Affinity.Weakness });
                    changed = true;
                }
                if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 3.0f / 5.0f, width = radioWidth }, affinity == Affinity.Resist) && affinity != Affinity.Resist)
                {
                    if (tmp != null) tmp.affinity = Affinity.Resist;
                    else table.Add(new() { element = element, affinity = Affinity.Resist });
                    changed = true;
                }
                if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 4.0f / 5.0f, width = radioWidth }, affinity == Affinity.Block) && affinity != Affinity.Block)
                {
                    if (tmp != null) tmp.affinity = Affinity.Block;
                    else table.Add(new() { element = element, affinity = Affinity.Block });
                    changed = true;
                }
            }

            if (changed)
            {
                tableProperty.ClearArray();
                for (int i = 0; i < table.Count; i++)
                {
                    tableProperty.InsertArrayElementAtIndex(i);
                    tableProperty.GetArrayElementAtIndex(i).FindPropertyRelative("element").enumValueIndex = (int)table[i].element;
                    tableProperty.GetArrayElementAtIndex(i).FindPropertyRelative("affinity").enumValueIndex = (int)table[i].affinity;
                }
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldout)
        {
            return foldoutSize + (rowSize * Enum.GetValues(typeof(Element)).Length + 1);
        }
        else return foldoutSize;
    }
}
#endif