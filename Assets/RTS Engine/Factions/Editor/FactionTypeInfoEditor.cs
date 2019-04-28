using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RTSEngine;

[CustomEditor(typeof(FactionTypeInfo))]
public class FactionTypeInfoEditor : Editor {

    FactionTypeInfo Target;
    SerializedObject SOTarget;

    //GUI Style:
    GUILayoutOption[] SmallButtonLayout;

    public void OnEnable()
    {
        SmallButtonLayout = new GUILayoutOption[] { GUILayout.Width(20.0f), GUILayout.Height(20.0f) };

        Target = (FactionTypeInfo)target;

        SOTarget = new SerializedObject(Target);
    }

    public override void OnInspectorGUI()
    {
        SOTarget.Update(); //Always update the Serialized Object.

        FactionSettings();

        SOTarget.ApplyModifiedProperties(); //Apply all modified properties always at the end of this method.

        EditorUtility.SetDirty(target);
    }

    void FactionSettings ()
    {
        Target.Name = EditorGUILayout.TextField("Name", Target.Name);
        Target.Code = EditorGUILayout.TextField("Code", Target.Code);

        EditorGUILayout.Space();

        Target.CapitalBuilding = EditorGUILayout.ObjectField("Capital Building", Target.CapitalBuilding, typeof(Building), true) as Building;
        Target.BuildingCenter = EditorGUILayout.ObjectField("Building Center", Target.BuildingCenter, typeof(Building), true) as Building;
        Target.PopulationBuilding = EditorGUILayout.ObjectField("Population Building", Target.PopulationBuilding, typeof(Building), true) as Building;
        Target.DropOffBuilding = EditorGUILayout.ObjectField("Drop Off Building", Target.DropOffBuilding, typeof(Building), true) as Building;

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Extra Buildings");
        EditorGUILayout.Space();
        if (GUILayout.Button("+", SmallButtonLayout))
        {
            Target.ExtraBuildings.Add(null);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (Target.ExtraBuildings.Count > 0)
        {
            for (int i = 0; i < Target.ExtraBuildings.Count; i++)
            {
                GUILayout.BeginHorizontal();
                Target.ExtraBuildings[i] = EditorGUILayout.ObjectField(Target.ExtraBuildings[i], typeof(Building), false) as Building;
                if (GUILayout.Button("-", SmallButtonLayout))
                {
                    Target.ExtraBuildings.Remove(Target.ExtraBuildings[i]);
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No extra buildings defined for this faction type", MessageType.Warning);
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Limits");
        EditorGUILayout.Space();
        if (GUILayout.Button("+", SmallButtonLayout))
        {
            FactionTypeInfo.FactionLimitsVars NewLimitElement = new FactionTypeInfo.FactionLimitsVars();

            Target.Limits.Add(NewLimitElement);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (Target.Limits.Count > 0)
        {
            for (int i = 0; i < Target.Limits.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                Target.Limits[i].Code = EditorGUILayout.TextField("Code", Target.Limits[i].Code);
                Target.Limits[i].MaxAmount = EditorGUILayout.IntField("Max Amount", Target.Limits[i].MaxAmount);
                GUILayout.EndVertical();

                if (GUILayout.Button("-", SmallButtonLayout))
                {
                    Target.Limits.Remove(Target.Limits[i]);
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No building/unit limits defined for this faction type", MessageType.Warning);
        }
    }
}
