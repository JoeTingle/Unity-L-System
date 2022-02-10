using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for combing multiple meshes together for optimisation
/// </summary>
public class CombineMesh : MonoBehaviour {

    /// <summary>
    /// Combines together child meshes, warning do not use with too many meshes
    /// </summary>
    /// <param name="alterPosition"></param>
    public void BeginCombineMesh(bool alterPosition)
    {
        //Creating variables to store old rotations and positions
        Quaternion oldRot = new Quaternion();
        Vector3 oldPos = new Vector3();
        if (alterPosition) // only changes position if set by the using in the paramter
        {
            oldRot = transform.rotation;
            oldPos = transform.position;
        }

        //Array to store all of the child objects, meshes
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        //Creating a new mesh to then use the combined meshes on
        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i].transform == transform) { continue; }

            //Goes through each combiner getting the filters and adding their mesh to the shared mesh
            //The shared mesh then keeps getting added to untill it is added to the final mesh
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = filters[i].sharedMesh;
            combiners[i].transform = filters[i].transform.localToWorldMatrix;

        }

        //Combining into the final mesh
        finalMesh.CombineMeshes(combiners);

        //Checks if the object has a filter or renderer on it, if not it adds them
        if (!gameObject.GetComponent<MeshFilter>() || !gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshFilter>().mesh = finalMesh;
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Branch");
        }
        else
        {
            gameObject.GetComponent<MeshFilter>().mesh = finalMesh;
            gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Branch");

        }


        if (alterPosition) // Altering positions if the user wants to in the method paramter
        {
            transform.position = oldPos;
            transform.rotation = oldRot;
        }

        //Deactivates all child objects after the final mesh is ready
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }


}
