using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSEngine
{
    public class TerrainManager : MonoBehaviour
    {
        public static TerrainManager Instance;

        public LayerMask GroundTerrainMask; //layers used for the ground terrain objects
        public GameObject FlatTerrain;

        public LayerMask AirTerrainMask; //layers used for the air terrain objects
        public GameObject AirTerrain;

        void Awake ()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }

        //get the height of the closest terrain tile
        public float SampleHeight (Vector3 Position, bool AirTerrain = false)
        {
            float Height = 0.0f;
            float Distance = 0.0f;

            LayerMask TerrainLayerMask = GroundTerrainMask; //by default, we'll use the ground terrain mask

            if (AirTerrain == true) //if we're sampling height regarding the air terrain
            {
                TerrainLayerMask = AirTerrainMask; //use the air terrain mask
            }

            RaycastHit[] Hits = Physics.RaycastAll(new Vector3(Position.x, Position.y+20.0f, Position.z), Vector3.down,50.0f, TerrainLayerMask);

            if(Hits.Length > 0)
            {
                Height = Hits[0].point.y;
                Distance = Vector3.Distance(Position, Hits[0].point);

                if (Hits.Length > 1)
                {
                    for (int i = 1; i < Hits.Length; i++)
                    {
                        if(Distance > Vector3.Distance(Hits[i].point, Position))
                        {
                            Height = Hits[i].point.y;
                            Distance = Vector3.Distance(Position, Hits[i].point);
                        }
                    }
                }
            }

            return Height;
        }

        //determine if an object belongs to the terrain tiles: (only regarding ground terrain objects)
        public bool IsTerrainTile (GameObject Obj)
        {
            return GroundTerrainMask == (GroundTerrainMask | (1 << Obj.layer));
        }
    }
}