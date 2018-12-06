using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;

public class GridGameControl : MonoBehaviour
{
    public GameObject TurnBlockPrefab;
    public MaterialControl BorderObject;

    // Flow
    public VariantButtonControl Variant1Button;
    public VariantButtonControl Variant2Button;

    private GameLogic_Grid3x3 pGameGrid;
    private GameState pGameState = GameState.Inactive;

    // Receive variant button input
    public void OnVariantButtonDown(int Variant)
    {
        Variant1Button.gameObject.SetActive(false);
        Variant2Button.gameObject.SetActive(false);

        if (TurnBlockPrefab)
        {
            if (pGameGrid != null)
                pGameGrid.Clear();

            if (Variant == 1)
                pGameGrid = new TicTacToeLogic(TurnBlockPrefab);
            else
                pGameGrid = new ToeTicTacLogic(TurnBlockPrefab);

            for (int i = 0; i < 9; i++)
            {
                int x = i / 3 - 1;
                int y = i % 3 - 1;

                pGameGrid.SetBlockTransform(i, new Vector3(x, y, 0f), Quaternion.identity);
            }
        }
        else
            Debug.Log("TicTacToeControl.OnVariantButtonDown -> TurnBlockPrefab has not been assigned");
    }

    // Use this for initialization
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if ((Variant1Button) && (Variant2Button))
        {
            Variant1Button.gameObject.SetActive(true);
            Variant2Button.gameObject.SetActive(true);

            Variant1Button.GridControl = this;
            Variant2Button.GridControl = this;
        }
        else
            Debug.Log("TicTacToeControl.Start -> Variant1Button and/or Variant2Button have/has not been assigned");
    }

    // Update is called once per frame
    void Update()
    {
        if (pGameGrid != null)
        {
            if ((BorderObject) && pGameGrid.CurrentGameState != pGameState)
            {
                pGameState = pGameGrid.CurrentGameState;

                BorderObject.SetMaterial(pGameGrid.CurrentGameState);

                if (pGameState == GameState.Draw || pGameState == GameState.Victory)
                {
                    Variant1Button.gameObject.SetActive(true);
                    Variant2Button.gameObject.SetActive(true);
                }
            }
        }
        else
            if (BorderObject)
            BorderObject.SetMaterial(GameState.Inactive);
    }
}
