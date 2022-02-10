using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

/// <summary>
/// Lsystem class for creating trees/foliage with the provided values in the inspector
/// </summary>
/// 
[ExecuteInEditMode]
public class LSystem : MonoBehaviour
{
    #region Class Variables
    /// <summary>
    /// Simple structure to store position and rotation information for the branches
    /// </summary>
    private struct BranchInfo
    {
        public Vector3 pos;
        public Quaternion rot;
    }

    /// <summary>
    /// Enum for different tree presets for the user to choose from when placing trees
    /// </summary>
    public enum treePreset
    {
        A,
        B,
        C
    }

    /// <summary>
    /// Used to parse in a tree preset from any formatted .txt file
    /// </summary>
    [SerializeField]
    public TextAsset TreePreset;

    /// <summary>
    /// Currently selected tree preset to be used when generating from editor
    /// </summary>
    [NonSerialized]
    private treePreset currentTreePreset = treePreset.A;
    /// <summary>
    /// Angle at which the branch will turn when generating a new one
    /// </summary>
    [SerializeField]
    private float angle = 25.0f;
    /// <summary>
    /// Default axiom to be used alongside the rules fo the L system (Must Match Rules !)
    /// </summary>
    [SerializeField]
    private char axiom;
    /// <summary>
    /// The L system rules which will be itterated through when generated, will be checked against code in Generate() function
    /// </summary>
    [SerializeField]
    private string sRules;
    /// <summary>
    /// Controls the size of the tree when generated, default 10
    /// </summary>
    [SerializeField]
    private float size = 10.0f;
    /// <summary>
    /// Material used for the line renderer when using debug generation
    /// </summary>
    [SerializeField]
    private Material lineMat;
    /// <summary>
    /// Wheter to draw debug features or not (Line renderer and debug lines)
    /// </summary>
    [SerializeField]
    private bool drawDebugLines = false;
    /// <summary>
    /// Variation of how much the branches can rotate by on the z axis, Allows for more 3D looking trees
    /// </summary>
    [SerializeField]
    [Range(0, 10)]
    private int zAxisVariation;
    /// <summary>
    /// Controls how many times it will run through the rules by, more means different tree type
    /// </summary>
    [SerializeField]
    private int iterations = 3;

    private Dictionary<char, string> rules;
    private Stack<BranchInfo> bStack;
    private List<GameObject> objects;
    private string currentString = string.Empty;

    #endregion

    public void ParseTreeFile()
    {
        TextAsset content = TreePreset;
        if (!content)
        {
            throw new InvalidOperationException("No .txt File Selected In Unity Inspector !");
        }
        string newaxiom = "";
        LSystemTreeParser.ParseFile(content.text, out newaxiom, out angle, out iterations, out rules, out sRules,out zAxisVariation, out size);
        axiom = Char.Parse(newaxiom);
        rules = new Dictionary<char, string> {
            { axiom, sRules }
        };

    }

    private void Start()
    {
        //Initialising new lists and stacks to store current object and the transform information
        gameObject.AddComponent<CombineMesh>();
        objects = new List<GameObject>();
        bStack = new Stack<BranchInfo>();
        rules = new Dictionary<char, string> {
            { axiom, sRules }
        };
        currentString = axiom.ToString();

        //Correcting rotation to make it stand upright, and itterating through generation of the tree
        transform.Rotate(Vector3.right * -90.0f);
        for (int i = 0; i < iterations; i++)
        {
            Generate();
        }

        //Correctting rotation/position again incase generation alters it
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, 0);

    }

    /// <summary>
    /// The tree generation function used by the editor script to allow for editor generation
    /// </summary>
    public void EditorGenerate()
    {
        //Correcting rotation and setting intial size/lists etc.
        transform.rotation = Quaternion.Euler(0, 0, 0);
        objects = new List<GameObject>();
        bStack = new Stack<BranchInfo>();
        rules = new Dictionary<char, string> {
            { axiom, sRules }
        };
        currentString = axiom.ToString();

        //Itterating through generation of the tree
        for (int i = 0; i < iterations; i++)
        {
            Generate();
        }

        //Correctting rotation/position again incase generation alters it
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        transform.position = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Used in editor script to clear all child objects from parent
    /// </summary>
    public void Clear()
    {
        //Goes through each child with a transform attached to it and destroys it
        foreach (Transform child in gameObject.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>
    /// Standard function for generating trees within play mode
    /// </summary>
    private void Generate()
    {
        //Creating a string builder instead of standard string as its more efficient, going to be modifying it
        StringBuilder sBuilder = new StringBuilder();
        //Iterates through each character in string and adds it to string builder
        foreach (char c in currentString)
        {
            sBuilder.Append(
                rules.ContainsKey(c) ? rules[c] : c.ToString()
                );
        }
        currentString = sBuilder.ToString();
        //Call function to check string against rules for each action to be taken
        Rules();
        //Making Branches smaller after each itteration
        size /= 1.25f;
    }

    /// <summary>
    /// Checks each character in the rules and takes action, for example rotating branch etc.
    /// </summary>
    public void Rules()
    {
        //Goes through each character and performs a preset action on it, as long as it matches
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.forward * size);
                    if (drawDebugLines) // Will draw debug lines and use line renderer if true
                    {
                        Debug.DrawLine(initialPosition, transform.position, Color.white, 1000000f, false);

                        //Line Renderer
                        GameObject tmpGameObject = new GameObject();
                        tmpGameObject.name = "Branch";
                        tmpGameObject.transform.parent = transform;
                        tmpGameObject.AddComponent<LineRenderer>();
                        tmpGameObject.GetComponent<LineRenderer>().material = lineMat;
                        Vector3[] posArray = { initialPosition, transform.position };
                        tmpGameObject.GetComponent<LineRenderer>().SetPositions(posArray);
                        objects.Add(tmpGameObject);
                    }
                    //Creates the 3D cylinders, providing two points
                    CreateCylinderBetweenPoints(initialPosition, transform.position, size / 5);
                    break;
                case '+':

                    //Positivly adjusting the rotation using z axis aswell
                    UnityEngine.Random.InitState((int)Time.time);
                    int randomZ = UnityEngine.Random.Range(0, zAxisVariation);
                    transform.Rotate(Vector3.up * angle + new Vector3(0, 0, randomZ));
                    break;
                case '-':
                    //Negativly adjusting the rotation using z axis aswell
                    UnityEngine.Random.InitState((int)Time.time);
                    int randomNegZ = UnityEngine.Random.Range(0, zAxisVariation);
                    transform.Rotate(Vector3.up * -angle + new Vector3(0, 0, randomNegZ));
                    break;
                case '[':
                    //Adds new branch informaiton to stack
                    bStack.Push(new BranchInfo()
                    {
                        pos = transform.position,
                        rot = transform.rotation
                    });
                    break;
                case ']':
                    {
                       GameObject leaf = Resources.Load<GameObject>("Prefabs/LeafPrefab");
                       GameObject GOLeaf = Instantiate(leaf);
                       GOLeaf.transform.parent = transform;
                       GOLeaf.transform.localPosition = transform.position;
                       GOLeaf.transform.localScale *= size;
                       GOLeaf.transform.Rotate(Vector3.left * angle);

                        //Pops current information off stack ready for new branch
                        BranchInfo tInfo = bStack.Pop();
                        transform.position = tInfo.pos;
                        transform.rotation = tInfo.rot;
                    }
                    break;
                default:
                    //Throws expection if rules don't match axiom or string etc.
                    throw new InvalidOperationException("Invalid L-tree operation (Check Axiom and/or Rules)");
            }
        }
    }

    /// <summary>
    /// Creates a primitve cylinder between two vector3 points and adjusts size
    /// </summary>
    /// <param name="start">Start point of the cylinder</param>
    /// <param name="end">Ending point of the cylinder</param>
    /// <param name="width">Actual size of the cylinder </param>
    public void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width)
    {
        Vector3 offset = end - start;
        Vector3 scale = new Vector3(width, offset.magnitude / 2.0f, width);
        Vector3 position = start + (offset / 2.0f);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        DestroyImmediate(cylinder.GetComponent<CapsuleCollider>());  
        cylinder.transform.position = position;
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
        cylinder.transform.SetParent(transform, false);
        Material mat = Resources.Load<Material>("Materials/Branch");
        cylinder.GetComponent<MeshRenderer>().material = mat;
    }

    /// <summary>
    /// Used in editor script to update tree presets for specific results
    /// **NOT USED ANYMORE, REPLACED WITH PARSER**
    /// </summary>
    /// <param name="zAxisVar">Amount of variation to be added to branch rotation</param>
    public void UpdateTreeSettings(int zAxisVar)
    {
        if (currentTreePreset == treePreset.A)
        {
            zAxisVariation = zAxisVar;
            iterations = 5;
            angle = 25.7f;
            axiom = 'F';
            sRules = "F[+F]F[-F]F";
            rules = new Dictionary<char, string> {
            { axiom, sRules }
            };
        }
        if (currentTreePreset == treePreset.B)
        {
            zAxisVariation = zAxisVar;
            iterations = 5;
            angle = 20.0f;
            axiom = 'F';
            sRules = "F[+F]F[-F]F";
            rules = new Dictionary<char, string> {
            { axiom, sRules }
            };
        }
        if (currentTreePreset == treePreset.C)
        {
            zAxisVariation = zAxisVar;
            iterations = 4;
            angle = 22.5f;
            axiom = 'F';
            sRules = "FF-[-F+F+F]+[+F-F-F]";
            rules = new Dictionary<char, string> {
            { axiom, sRules }
            };
        }
    }
}



