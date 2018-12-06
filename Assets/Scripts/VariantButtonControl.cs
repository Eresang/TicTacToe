using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantButtonControl : MonoBehaviour
{
    private GridGameControl pOppressiveOverlord;
    public GridGameControl GridControl { set { pOppressiveOverlord = value; } }

    public int Variant;

    // Defer input up to parent grid
    void OnMouseDown()
    {
        if (pOppressiveOverlord != null)
            pOppressiveOverlord.OnVariantButtonDown(Variant);
    }
}
