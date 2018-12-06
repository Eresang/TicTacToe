using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridShapes;
using GameLogic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Collider))]
public class GridSpaceControl : MonoBehaviour
{
    // Divide texture map into four quadrants by default
    public Vector4[] stateUVBounds = new Vector4[4] { new Vector4(0f, 0f, 0.5f, 0.5f), new Vector4(0.5f, 0f, 1f, 0.5f), new Vector4(0f, 0.5f, 0.5f, 1f), new Vector4(0.5f, 0.5f, 1f, 1f) };

    // Current state
    private int pState = -1;
    public int State { get { return pState; } }

    // Parent tic tac toe grid
    private GameLogic_Grid3x3 pOppressiveOverlord;
    public GameLogic_Grid3x3 GridControl { set { pOppressiveOverlord = value; } }

    // Index of self within parent grid
    private int pGridIndex = -1;
    public int GridIndex { set { pGridIndex = value; } }

    // Visual mesh
    public GridSpaceShapes Shape = GridSpaceShapes.Plate;
    private GridSpaceShape pShape;

    // Component references
    private MeshFilter pMeshFilter;
    private MeshRenderer pMeshRenderer;
    private Material pMaterial;
    private Color pColor;

    // Rotation timings
    public float OptionalRotationDelay;
    public float RotationDuration;
    public float DelayedRotationDuration;

    // Rotation animation
    private Quaternion pTargetRotation, pStartRotation;
    private float pDelay = 0f;
    private float pTime = 0f;
    private float pDuration = 0f;

    // Returns wether GridSpaceControl is rotating
    public bool IsRotating()
    {
        return pTime + pDelay <= Time.time && pTime + pDelay + pDuration >= Time.time;
    }

    // Set material color
    public void SetColor(float Brightness)
    {
        pMaterial.color = new Color(pColor.r * Brightness, pColor.g * Brightness, pColor.b * Brightness);
    }

    // Turn face into the last mapping of the material
    public void SetDisabled()
    {
        if (stateUVBounds.Length > 0)
        {
            pShape.Rotate(stateUVBounds[stateUVBounds.Length - 1]);

            pMeshFilter.sharedMesh = pShape.ShapeMesh;

            pTime = Time.time;

            pDelay = 0f;
            pDuration = RotationDuration;

            pTargetRotation = pShape.ShapeOrientation;
            pStartRotation = transform.rotation;
        }
    }

    // Change state, initiate rotation
    public void SetState(int NewState, bool Delayed)
    {
        if (NewState != pState && NewState <= stateUVBounds.Length && !IsRotating())
        {
            pState = NewState;

            pShape.Rotate(stateUVBounds[pState + 1]);

            pMeshFilter.sharedMesh = pShape.ShapeMesh;

            pTime = Time.time;

            if (Delayed)
            {
                pDelay = OptionalRotationDelay;
                pDuration = DelayedRotationDuration;
            }
            else
            {
                pDelay = 0f;
                pDuration = RotationDuration;
            }

            pTargetRotation = pShape.ShapeOrientation;
            pStartRotation = transform.rotation;
        }
        else
            Debug.Log("GridSpaceControl.SetState -> Could not set state");
    }

    // Obtain component references
    private void Awake()
    {
        pMeshFilter = GetComponent<MeshFilter>();
        pMeshRenderer = GetComponent<MeshRenderer>();
        pMaterial = new Material(pMeshRenderer.material);
        pColor = pMaterial.color;

        pMeshRenderer.material = pMaterial;
    }

    // Initiate shape
    void Start()
    {
        pShape = new Plate(stateUVBounds.Length > 0 ? stateUVBounds[0] : Vector4.zero);

        pMeshFilter.sharedMesh = pShape.ShapeMesh;

        pTargetRotation = transform.rotation;
        pStartRotation = transform.rotation;
    }

    // Rotation
    void Update()
    {
        if (IsRotating() && pDuration > 0f)
            transform.rotation = pStartRotation * Quaternion.Slerp(Quaternion.identity, pShape.ShapeRotation, (Time.time - pTime + pDelay) / pDuration);
        else
            transform.rotation = pTargetRotation;
    }

    // Refer input up to parent grid
    void OnMouseDown()
    {
        if (pOppressiveOverlord != null)
            pOppressiveOverlord.OnBlockMouseDown(pGridIndex);
    }
}
