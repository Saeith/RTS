using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTSEngine;

[CreateAssetMenu(fileName = "NewFactionType", menuName = "RTS Engine/Faction Type", order = 1)]
public class FactionTypeInfo : ScriptableObject
{
    [HideInInspector]
    public string Name = "Faction0"; //Provide a name for each faction.
    [HideInInspector]
    public string Code = "faction0"; //A unique code for each faction.

    //for NPC factions, if one of the buildings below is unique for the faction, it must be assigned (leave empty if it's not the case)
    [HideInInspector]
    public Building CapitalBuilding;
    [HideInInspector]
    public Building BuildingCenter;
    [HideInInspector]
    public Building PopulationBuilding;
    [HideInInspector]
    public Building DropOffBuilding;

    [HideInInspector]
    public List<Building> ExtraBuildings = new List<Building>(); //A list of extra buildings that only this faction can place. 

    [HideInInspector]
    public List<FactionLimitsVars> Limits = new List<FactionLimitsVars>(); //building/unit limits for this faction type

    [System.Serializable]
    public class FactionLimitsVars
    {
        public string Code = "unit/building code"; //the building/unit prefab to limit
        public int MaxAmount = 1; //the maximum amount of spawned building/units from the prefab above at the same time

        public int CurrentAmount; //current amount spawned of the above assigned unit/building
    }
}