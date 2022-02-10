using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreePlacer : MonoBehaviour {

    #region Class Variables

    [System.Serializable]
    public class TreeTypes
    {
        [SerializeField]
        public TextAsset TreeType;
        [SerializeField]
        [Range(0,100)]
        public int spawnChance = 50;
        [SerializeField]
        public bool spawned = false;
    }
    [SerializeField]
    TreeTypes[] typesOfTrees;
    /// <summary>
    /// The amount of trees to be generated, within a range
    /// </summary>
    [SerializeField]
    [Range(1, 25)]
    private int density = 1;
    /// <summary>
    /// The size of the sphere in which trees can be placed, based of Unity transform units
    /// </summary>
    [SerializeField]
    private int radius = 10;
    /// <summary>
    /// Transparent material to be used on the placement sphere
    /// </summary>
    [SerializeField]
    private Material transMaterial;
    private GameObject radiusGO;
    private List<GameObject> trees;
    private GameObject parentMesh;
    #endregion

    /// <summary>
    /// Creates a primimtive object and sets a transparent material to it.
    /// Also moves with mouse position on screen
    /// </summary>
    public void DrawSphere()
    {
        //Gets radius game object, assigns material and then moves in sync with mouse position
        radiusGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        radiusGO.GetComponent<Renderer>().material = transMaterial;
        radiusGO.transform.position = Input.mousePosition;
    }

    void Start()
    {
        //Creates the initial game object and sets it size/material aswell as disabling collider so it doesn't interact with terrain
        radiusGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        radiusGO.transform.localScale = new Vector3(radius, radius, radius);
        radiusGO.GetComponent<Renderer>().material = transMaterial;
        radiusGO.GetComponent<SphereCollider>().enabled = false;

        trees = new List<GameObject>();
    }

    void Update()
    {
        //Creating Raycast hit info variable and the actual Ray, which points from screen to mouse position
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hit))
        {
            //Sets position of gameobject to match whatever it hit on the collision
            radiusGO.transform.position = hit.point;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            parentMesh = new GameObject();
            //Once the user presses the key it will itterate through and generate the number of trees
            for (int i = 0; i < density; i++)
            {
                for (int x = 0; x < typesOfTrees.Length; x++)
                {
                    int random = UnityEngine.Random.Range(0, 100);
                    if (random < typesOfTrees[x].spawnChance)
                    {
                        GameObject tree = new GameObject();
                        tree.name = "Tree";
                        tree.transform.parent = parentMesh.transform;
                        tree.transform.position = Random.insideUnitSphere * radius + radiusGO.transform.position;
                        tree.transform.position = new Vector3(tree.transform.position.x, hit.point.y, tree.transform.position.z);

                        tree.AddComponent<LSystem>().TreePreset = typesOfTrees[x].TreeType;
                        tree.GetComponent<LSystem>().ParseTreeFile();

                        typesOfTrees[x].spawned = true;

                        trees.Add(tree);
                    }
                }
                //Spawns Bunch Of Grass
                GameObject grassPrefab = Resources.Load<GameObject>("Prefabs/GrassPrefab");
                GameObject grass = Instantiate(grassPrefab);
                grass.transform.position = Random.insideUnitSphere * radius + radiusGO.transform.position;
                grass.transform.position = new Vector3(grass.transform.position.x, hit.point.y + 0.5f, grass.transform.position.z);
            }
        }
    }

}
