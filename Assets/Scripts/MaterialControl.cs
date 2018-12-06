using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;

[RequireComponent(typeof(Renderer))]
public class MaterialControl : MonoBehaviour
{
    private Renderer pRenderer;

    // The different materials
    public Material InactiveMaterial;
    public Material ActiveMaterial;
    public Material DrawMaterial;
    public Material VictoryMaterial;

    // Set the material
    public void SetMaterial(GameState CurrentGameState)
    {
        switch (CurrentGameState)
        {
            case GameState.Inactive:
                pRenderer.material = InactiveMaterial;
                break;
            case GameState.Active:
                pRenderer.material = ActiveMaterial;
                break;
            case GameState.Draw:
                pRenderer.material = DrawMaterial;
                break;
            case GameState.Victory:
                pRenderer.material = VictoryMaterial;
                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        pRenderer = GetComponent<Renderer>();
    }
}
