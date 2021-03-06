﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTSEngine 
{
	public class FactionManager : MonoBehaviour {

		public int FactionID; //the faction ID that this manager belongs to.

		public enum Tasks {Build,Attack,Collect};

		//The lists below hold all different types of units.
		[HideInInspector]
		public List<Unit> Units; //list containing all the units that this faction owns.
		[HideInInspector]
		public List<Builder> Builders; //list containing all the builders that this faction owns.
		[HideInInspector]
		public List<GatherResource> Collectors; //list containing all the resource collectors that this faction owns.
		[HideInInspector]
		public List<Healer> Healers; //list containing all the healers that this faction owns.
		[HideInInspector]
		public List<Converter> Converters; //list containing all the converters that this faction owns.
		[HideInInspector]
		public List<Attack> Army; //list containing all the army units this faction owns.
        [HideInInspector]
        public List<Unit> NonArmy; //list containing units that don't have an Attack component in this faction
		[HideInInspector]
		public List<Unit> EnemyUnits; //list containing all the enemy units.

        //The listes below hold all different buildings:
        [HideInInspector]
		public List<Building> Buildings = new List<Building>();//list containing all the buildings this faction owns.
		[HideInInspector]
		public List<Building> BuildingCenters = new List<Building>(); //list containing all the building centers this faction owns.
		[HideInInspector]
		public List<Building> DropOffBuildings = new List<Building>(); //list containing all the resource drop off buildings that this faction owns.

        //Task Launchers:
        public List<TaskLauncher> TaskLaunchers = new List<TaskLauncher>();

        //holds the building/unit for for the faction type that this faction belongs to
        public List<FactionTypeInfo.FactionLimitsVars> Limits = new List<FactionTypeInfo.FactionLimitsVars>();

        //The list that contains all in range resources:
        [HideInInspector]
		public List<Resource> ResourcesInRange; //A list of all the resources that are inside the faction's terrirtory.

		[HideInInspector]
		public float ArmyPower; //The current power of the faction's army.

		//NPC Manager scripts:
		[HideInInspector]
		public NPCBuildingPlacement BuildingMgr;
		[HideInInspector]
		public NPCArmy ArmyMgr;
		[HideInInspector]
		public NPCResource ResourceMgr;
		[HideInInspector]
		public NPCUnitSpawner UnitSpawner;

		[HideInInspector]
		public GameManager GameMgr;

		void Awake () {

			//Assign the faction manager for this team in the game manager list of all factions:
			GameMgr = GameManager.Instance;

			//If we don't need this faction (it has not been added in the map menu):
			if (FactionID >= GameMgr.Factions.Count) {
				//Remove it
				DestroyImmediate (gameObject);
				return;
			}

			//if this faction is controlled by the player:
			if (FactionID == GameManager.PlayerFactionID) {
				//set the player faction manager static var:
				GameManager.PlayerFactionMgr = this;
			}

		}

		//the method that registers the building:
		public void AddBuildingToList (Building Building)
		{
			//building is registered:
			Buildings.Add (Building);
            //update the limits list:
            UpdateLimitsList(Building.Code, true);
		}

		public void RemoveBuilding (Building Building)
		{
			Buildings.Remove (Building);
            //update the limits list:
            UpdateLimitsList(Building.Code, false);
        }

		//the method that registers the unit to the faction:
		public void AddUnitToLists (Unit Unit)
		{
			Units.Add (Unit);

			//add to the other faction's enemy list:
			for(int i = 0; i < GameMgr.Factions.Count; i++)
			{
				if (i != Unit.FactionID) {
					GameMgr.Factions [i].FactionMgr.EnemyUnits.Add (Unit);
				}
			}

			//Add the builders in one list
			if (Unit.gameObject.GetComponent<Builder> ()) {
				Builders.Add (Unit.gameObject.GetComponent<Builder> ());
			}
			//Add the resource gatherers in one list
			if (Unit.gameObject.GetComponent<GatherResource> ()) {
				Collectors.Add (Unit.gameObject.GetComponent<GatherResource> ());
			}

			//Add the army units (that have the attack component) in one list.
			if (Unit.gameObject.GetComponent<Attack> ()) {
				Army.Add (Unit.gameObject.GetComponent<Attack> ());
				ArmyPower += Unit.gameObject.GetComponent<Attack> ().UnitDamage;
			}
            else
            {
                //add non army units to another list:
                NonArmy.Add(Unit);
            }

			//Add the healers (that have the healer component) in one list.
			if (Unit.gameObject.GetComponent<Healer> ()) {
				Healers.Add (Unit.gameObject.GetComponent<Healer> ());
			}
			//Add the healers in one list.
			if (Unit.gameObject.GetComponent<Converter> ()) {
				Converters.Add (Unit.gameObject.GetComponent<Converter> ());
			}

			//NPC Unit Spawner:
			if (UnitSpawner != null && Unit.NPCUnitSpawnerID >= 0) {
				UnitSpawner.Units [Unit.NPCUnitSpawnerID].CurrentUnits.Add (Unit);
				UnitSpawner.Units [Unit.NPCUnitSpawnerID].ProgressAmount--; //mark that one of the progress tasks is over.
			}
        }

		//a method that removes the unit from all the lists:
		public void RemoveUnitFromLists (Unit Unit)
		{
			Units.Remove (Unit);

			//remove from the other faction's enemy lists:
			for(int i = 0; i < GameMgr.Factions.Count; i++)
			{
				if (i != Unit.FactionID) {
					GameMgr.Factions [i].FactionMgr.EnemyUnits.Remove (Unit);
				}
			}

			//Add the builders in one list
			if (Unit.gameObject.GetComponent<Builder> ()) {
				Builders.Remove (Unit.gameObject.GetComponent<Builder> ());
			}
			//Add the resource gatherers in one list
			if (Unit.gameObject.GetComponent<GatherResource> ()) {
				Collectors.Remove (Unit.gameObject.GetComponent<GatherResource> ());
			}

			//Add the army units (that have the attack component) in one list.
			if (Unit.gameObject.GetComponent<Attack> ()) {
				Army.Remove (Unit.gameObject.GetComponent<Attack> ());
				ArmyPower -= Unit.gameObject.GetComponent<Attack> ().UnitDamage;
			}
            else
            {
                //non attack units?
                NonArmy.Remove(Unit);
            }

			//Add the healers (that have the healer component) in one list.
			if (Unit.gameObject.GetComponent<Healer> ()) {
				Healers.Remove (Unit.gameObject.GetComponent<Healer> ());
			}
			//Add the healers in one list.
			if (Unit.gameObject.GetComponent<Converter> ()) {
				Converters.Remove (Unit.gameObject.GetComponent<Converter> ());
			}

			//NPC Unit Spawner:
			if (UnitSpawner != null && Unit.NPCUnitSpawnerID >= 0) {
				UnitSpawner.Units [Unit.NPCUnitSpawnerID].CurrentUnits.Remove (Unit);
				UnitSpawner.Units [Unit.NPCUnitSpawnerID].AllCreated = false;
				UnitSpawner.AllCreated = false;
			}

            //update the limits list:
            UpdateLimitsList(Unit.Code, false);
        }

		//When a new resource drop off building is spawned, all collectors check if this building can suit them or not.
		public void CheckCollectorsDropOffBuilding ()
		{
			if (Collectors.Count > 0) {
				//go through all the resource collectors:
				for (int i = 0; i < Collectors.Count; i++) {
					if (Collectors [i].TargetResource != null) {
						//check if they are actually currently gathering a resource:
						Collectors [i].FindClosetDropOffBuilding (); //ask them to re-search for the closet drop off building:
						if (Collectors [i].DroppingOff == true && Collectors [i].DropOffBuilding == null) { //if they are already waiting to drop off resources and they don't have a drop off building.
							Collectors [i].SendUnitToDropOffResources (); //ask them to because we just had a drop off building
						}
					}
				}
			}
		}

		//check all buildings in the list are spawned and built:
		public bool AreBuildingsSpawned (Building[] BuildingsList)
		{
			if (BuildingsList.Length > 0) {
				for (int i = 0; i < BuildingsList.Length; i++) { //go through the buildings list
					int j = 0;
					bool Found = false;
					while (j < Buildings.Count && Found == false) { //go through all the faction's buildings.
						if (Buildings [j].IsBuilt == true) {
							if (Buildings [j].Code == BuildingsList [i].Code) { //if we have the same building code.
								Found = true;
							}
						}
						j++;
					}

					//if one of the buildings is not found:
					if (Found == false) {
						return false;
					}
				}
			}

			//if we reach this point then all the buildings are found:
			return true;
		}

        //Faction limits:
        //check if the faction has hit its limit with placing a specific building/unit
        public bool HasReachedLimit(string Code)
        {
            if (Limits.Count > 0) //if there are buildings in the building/unit limits list
            {
                //go through each one of them
                for(int i = 0; i < Limits.Count; i++)
                {
                    //if this is the building/unit we're looking for:
                    if (Code == Limits[i].Code)
                    {
                        //if there's still space to add one more of this building/unit, return true, if not then false
                        return Limits[i].CurrentAmount >= Limits[i].MaxAmount;
                    }
                }
            }

            //if the building/unit is not found in the list
            return false;
        }

        //when a unit/building is added, this is called to increment the limits list
        public void UpdateLimitsList(string Code, bool Increment)
        {
            if (Limits.Count > 0) //if there are elements in the building/unit limits list
            {
                //go through each one of them
                for (int i = 0; i < Limits.Count; i++)
                {
                    //if this is the building/unit we're looking for:
                    if (Code == Limits[i].Code)
                    {
                        //increment the current amount:
                        Limits[i].CurrentAmount += (Increment == true) ? 1 : -1;
                        return;
                    }
                }
            }
        }

        public bool CheckRequiredBuildings(Building RefBuilding)
        {
            //the building requirement array is an array of array, one building must be built and spawned from each array element in order to meet the requirements
            //so we'll be going through each array element, one by one

            foreach (Building.RequiredBuildingVars BList in RefBuilding.RequiredBuildings)
            {
                int i = 0;
                List<string> RequiredBuildingCodes = new List<string>();

                //go through the building requirements for the ref building and collect the building codes:
                foreach (Building B in BList.BuildingList)
                {
                    RequiredBuildingCodes.Add(B.Code);
                }

                bool Found = false;

                //go through all spawned buildings for the building's faction 
                while (RequiredBuildingCodes.Count > 0 && i < Buildings.Count && Found == false)
                {
                    if (Buildings[i].IsBuilt == true && RequiredBuildingCodes.Contains(Buildings[i].Code)) //if the building is built && its code is found in the building list codes
                    {
                        //mark as found: building requiremenet for this array element is met:
                        Found = true;
                    }
                    i++;
                }

                if (Found == false)
                    return false; //no building in the array element is found, then return false
            }

            //if we reach this stage, this means that all the required building conditions are met and therefore:
            return true;
        }
    }
}
