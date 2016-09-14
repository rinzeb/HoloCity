using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using System.Linq;


public class Meshes : MonoBehaviour
{

    [SerializeField]
    private Material _meshMaterial;

  

    private void CreateMesh(List<Vector3> corners, float height, List<Vector3> verts, List<int> indices)
    {
        var tris = new Triangulator(corners);
        var vertsStartCount = verts.Count;
        verts.AddRange(corners.Select(x => new Vector3(x.x, height, x.z)).ToList());
        indices.AddRange(tris.Triangulate().Select(x => vertsStartCount + x));

        // if (typeSettings.IsVolumetric)
        {

            Vector3 v1;
            Vector3 v2;
            int ind = 0;
            for (int i = 1; i < corners.Count; i++)
            {
                v1 = verts[vertsStartCount + i - 1];
                v2 = verts[vertsStartCount + i];
                ind = verts.Count;
                verts.Add(v1);
                verts.Add(v2);
                verts.Add(new Vector3(v1.x, 0, v1.z));
                verts.Add(new Vector3(v2.x, 0, v2.z));

                indices.Add(ind);
                indices.Add(ind + 2);
                indices.Add(ind + 1);

                indices.Add(ind + 1);
                indices.Add(ind + 2);
                indices.Add(ind + 3);
            }

            v1 = verts[vertsStartCount];
            v2 = verts[vertsStartCount + corners.Count - 1];
            ind = verts.Count;
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(new Vector3(v1.x, 0, v1.z));
            verts.Add(new Vector3(v2.x, 0, v2.z));

            indices.Add(ind);
            indices.Add(ind + 1);
            indices.Add(ind + 2);

            indices.Add(ind + 1);
            indices.Add(ind + 3);
            indices.Add(ind + 2);
        }
    }

    private void CreateGameObject(string name, List<Vector3> vertices, List<int> indices, GameObject main)
    {
        var go = new GameObject(name + " Building");
        var mesh = go.AddComponent<MeshFilter>().mesh;
        go.AddComponent<MeshRenderer>();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        go.GetComponent<MeshRenderer>().material = _meshMaterial;// _settings.GetSettingsFor(kind).Material;
        //go.transform.position += Vector3.up * Order;
        go.transform.SetParent(main.transform, false);
    }

    // Use this for initialization
    void Start()
    {
        var main = new GameObject("Buildings Layer");

        main.transform.position = new Vector3(0, -0.5f, 2f);
        //      main.transform.Rotate(new Vector3(0.9f, 0, 0));
        main.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        for (int i = 0; i < 5; i++)
        {
            List<Vector3> floor = new List<Vector3> {
                                            new Vector3(.3f  + (.7f*i), 0, -.3f),
                                            new Vector3(-.3f+ (.7f*i),  0, -.3f),
                                            new Vector3(-.3f+ (.7f*i),  0,  .3f),
                                            new Vector3(.1f+ (.7f*i),  0,  .3f),
                                            new Vector3(.1f+ (.7f*i) ,0, .1f),
                                            new Vector3(.3f+ (.7f*i), 0, .1f),

                };

            var verts = new List<Vector3>();
            var indices = new List<int>();
            CreateMesh(floor, 1, verts, indices);
            CreateGameObject("a" + i.ToString(), verts, indices, main);
        }
        /*Mesh mesh = new Mesh();
        transform.GetComponent<MeshFilter>();

        if (!transform.GetComponent<MeshFilter>() || !transform.GetComponent<MeshRenderer>()) //If you will havent got any meshrenderer or filter
        {
            transform.gameObject.AddComponent<MeshFilter>();
            transform.gameObject.AddComponent<MeshRenderer>();
        }

        transform.GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = verts.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        var myRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        myRenderer.material = _meshMaterial;
        */
        //Cube();
    }

    // Update is called once per frame
    void Update()
    {

    }
}