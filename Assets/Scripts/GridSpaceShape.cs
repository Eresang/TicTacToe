using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridShapes
{
    public enum GridSpaceShapes { Plate }

    public class GridSpaceShape
    {
        // Mesh
        protected Mesh pMesh;
        public Mesh ShapeMesh { get { return pMesh; } }
        protected Vector2[] pUVs;

        // Track rotation
        protected Quaternion pRotation = Quaternion.identity;
        protected Quaternion pOrientation = Quaternion.identity;
        public Quaternion ShapeRotation { get { return pRotation; } }
        public Quaternion ShapeOrientation { get { return pOrientation; } }

        // Constructor
        public GridSpaceShape()
        {
            pMesh = new Mesh();
        }

        public virtual void SetFace(Vector4 UVBounds) { }

        public virtual void Rotate(Vector4 UVBounds) { }
    }

    public class Plate : GridSpaceShape
    {

        private int pIntRotation = 0;

        // Set face indicated by rotation
        public override void SetFace(Vector4 UVBounds)
        {
            pUVs[pIntRotation * 4 + 0] = new Vector2(UVBounds.x, UVBounds.y);
            pUVs[pIntRotation * 4 + 1] = new Vector2(UVBounds.x, UVBounds.w);
            pUVs[pIntRotation * 4 + 2] = new Vector2(UVBounds.z, UVBounds.y);
            pUVs[pIntRotation * 4 + 3] = new Vector2(UVBounds.z, UVBounds.w);

            pMesh.uv = pUVs;
        }

        // Pick a direction to rotate towards
        // Change UVs of the face co-planar with XY and facing towards -Z (after rotation) to UVBounds
        public override void Rotate(Vector4 UVBounds)
        {
            pIntRotation = (pIntRotation + 1) & 1;

            SetFace(UVBounds);

            int lRandom = Mathf.RoundToInt(Random.value * 2f);

            pRotation = Quaternion.AngleAxis(180f * Mathf.Sign(Random.value - 0.5f), new Vector3((lRandom & 1) * Mathf.Sign(Random.value - 0.5f), ((lRandom + 1) & 1) * Mathf.Sign(Random.value - 0.5f), 0f));

            if (pIntRotation == 0)
                pOrientation = Quaternion.identity;
            else
                pOrientation = pRotation;
        }

        // Constructor
        public Plate(Vector4 UVBounds) : base()
        {
            Quaternion lRotation = Quaternion.identity;

            Vector3[] lVerts = new Vector3[8];
            for (int i = 0; i < 2; i++)
            {
                lVerts[i * 4 + 0] = lRotation * new Vector3(-0.5f, -0.5f, 0f);
                lVerts[i * 4 + 1] = lRotation * new Vector3(-0.5f, 0.5f, 0f);
                lVerts[i * 4 + 2] = lRotation * new Vector3(0.5f, -0.5f, 0f);
                lVerts[i * 4 + 3] = lRotation * new Vector3(0.5f, 0.5f, 0f);

                lRotation *= Quaternion.AngleAxis(180f, Vector3.right);
            }
            pMesh.vertices = lVerts;

            int[] lTris = new int[12];
            for (int i = 0; i < 2; i++)
            {
                lTris[i * 6 + 0] = i * 4 + 0;
                lTris[i * 6 + 1] = i * 4 + 1;
                lTris[i * 6 + 2] = i * 4 + 2;
                lTris[i * 6 + 3] = i * 4 + 1;
                lTris[i * 6 + 4] = i * 4 + 3;
                lTris[i * 6 + 5] = i * 4 + 2;
            }
            pMesh.triangles = lTris;

            pUVs = new Vector2[8];
            for (int i = 0; i < 2; i++)
            {
                pUVs[i * 4 + 0] = new Vector2(UVBounds.x, UVBounds.y);
                pUVs[i * 4 + 1] = new Vector2(UVBounds.x, UVBounds.w);
                pUVs[i * 4 + 2] = new Vector2(UVBounds.z, UVBounds.y);
                pUVs[i * 4 + 3] = new Vector2(UVBounds.z, UVBounds.w);
            }
            pMesh.uv = pUVs;

            pMesh.RecalculateNormals();
        }
    }
}