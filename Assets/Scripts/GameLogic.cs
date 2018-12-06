using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public enum GameState { Inactive, Active, Draw, Victory }

    public class GameLogic_Grid3x3
    {
        // Turn control
        protected int pTurn = 0;
        public int Turn { get { return pTurn; } }

        // Game state
        protected GameState pGameState = GameState.Active;
        public GameState CurrentGameState { get { return pGameState; } }

        // Victory state
        protected int pVictoryTurn = -1;
        public int VictoryTurn { get { return pVictoryTurn; } }

        // References to turnblocks
        protected GridSpaceControl[] pGridObjects = new GridSpaceControl[9];

        // Set up initial grid
        public GameLogic_Grid3x3(GameObject TurnBlockPrefab)
        {
            if ((TurnBlockPrefab) && (TurnBlockPrefab.GetComponent<GridSpaceControl>()))
            {
                for (int i = 0; i < pGridObjects.Length; i++)
                {
                    pGridObjects[i] = GameObject.Instantiate(TurnBlockPrefab).GetComponent<GridSpaceControl>();
                    pGridObjects[i].GridIndex = i;
                    pGridObjects[i].GridControl = this;
                }
            }
            else
            {
                Debug.Log("GameLogic_Grid3x3.Constructor -> Invalid TurnBlockPrefab specified");
                pGameState = GameState.Inactive;
            }
        }

        // Clear all grid objects
        public void Clear()
        {
            for (int i = 0; i < pGridObjects.Length; i++)
                GameObject.Destroy(pGridObjects[i].gameObject);
        }

        // Position block
        public void SetBlockTransform(int GridIndex, Vector3 Position, Quaternion Rotation)
        {
            if (GridIndex >= 0 && GridIndex < pGridObjects.Length && pGameState != GameState.Inactive)
            {
                pGridObjects[GridIndex].transform.position = Position;
                pGridObjects[GridIndex].transform.rotation = Rotation;
            }
            else
                Debug.Log("GameLogic_Grid3x3.SetBlockTransform -> Index out of bounds");
        }

        // Receive input from blocks
        public void OnBlockMouseDown(int GridIndex)
        {
            if (GridIndex >= 0 && GridIndex < pGridObjects.Length && pGameState == GameState.Active)
            {
                TryPerformMove(GridIndex);

                if (CheckGameState())
                    Debug.Log("GameLogic_Grid3x3.OnBlockMouseDown -> " + pGameState);

                OnValidMove(GridIndex);
            }
        }

        protected virtual bool CheckGameState()
        {
            return false;
        }

        protected virtual bool ValidateMove(int GridIndex)
        {
            return false;
        }

        protected virtual void TryPerformMove(int GridIndex) { }

        protected virtual void OnValidMove(int GridIndex) { }
    }

    // Classic tic tac toe
    public class TicTacToeLogic : GameLogic_Grid3x3
    {
        public TicTacToeLogic(GameObject TurnBlockPrefab) : base(TurnBlockPrefab) { }

        // Check victory conditions
        protected override bool CheckGameState()
        {
            if (pGameState != GameState.Active)
                return false;

            int lVictoryMask = 0;

            // Check horizontals
            for (int i = 0; i < 3; i++)
                if (pGridObjects[i * 3].State != -1 && pGridObjects[i * 3].State == pGridObjects[i * 3 + 1].State && pGridObjects[i * 3 + 1].State == pGridObjects[i * 3 + 2].State)
                {
                    pVictoryTurn = pGridObjects[i * 3 + 2].State;
                    pGameState = GameState.Victory;

                    lVictoryMask |= (1 << i * 3) | (1 << i * 3 + 1) | (1 << i * 3 + 2);
                }

            // Check verticals
            for (int i = 0; i < 3; i++)
                if (pGridObjects[i].State != -1 && pGridObjects[i].State == pGridObjects[i + 3].State && pGridObjects[i + 3].State == pGridObjects[i + 6].State)
                {
                    pVictoryTurn = pGridObjects[i + 6].State;
                    pGameState = GameState.Victory;

                    lVictoryMask |= (1 << i) | (1 << i + 3) | (1 << i + 6);
                }

            // Check diagonals
            for (int i = 0; i < 2; i++)
                if (pGridObjects[4 - (i * 2 + 2)].State != -1 && pGridObjects[4 - (i * 2 + 2)].State == pGridObjects[4].State && pGridObjects[4].State == pGridObjects[4 + (i * 2 + 2)].State)
                {
                    pVictoryTurn = pGridObjects[4 + (i * 2 + 2)].State;
                    pGameState = GameState.Victory;

                    lVictoryMask |= (1 << 4 - (i * 2 + 2)) | (1 << 4) | (1 << 4 + (i * 2 + 2));
                }

            // Disable blocks that are not part of the victory
            if (lVictoryMask > 0)
            {
                for (int i = 0; i < 9; i++)
                    if (((1 << i) & lVictoryMask) == 0)
                        pGridObjects[i].SetDisabled();

                return true;
            }

            // Are there moves available
            for (int i = 0; i < 9; i++)
                if (pGridObjects[i].State == -1)
                    return false;

            // No victory, no more moves
            pGameState = GameState.Draw;
            return true;
        }

        // Check wether move would be valid
        protected override bool ValidateMove(int GridIndex)
        {
            return pGridObjects[GridIndex].State == -1 && !pGridObjects[GridIndex].IsRotating();
        }

        // Try to perform the move
        protected override void TryPerformMove(int GridIndex)
        {
            // Perform move if it is valid
            if (ValidateMove(GridIndex))
            {
                pGridObjects[GridIndex].SetState(pTurn, false);

                pTurn = (pTurn + 1) % 2;
            }
        }
    }

    // Classic tic tac toe
    public class ToeTicTacLogic : GameLogic_Grid3x3
    {
        private int pLastMove = -1;

        public ToeTicTacLogic(GameObject TurnBlockPrefab) : base(TurnBlockPrefab)
        {
            ShowValidMoves();
        }

        // Changes grid space material properties to show wether that space if a valid move
        protected void ShowValidMoves()
        {
            for (int i = 0; i < 9; i++)
                pGridObjects[i].SetColor(((i == pLastMove) || (pGridObjects[i].State == pTurn)) && pGameState == GameState.Active ? 0.5f : 1f);
        }

        // Check victory conditions
        protected override bool CheckGameState()
        {
            if (pGameState != GameState.Active)
                return false;

            int lVictoryMask = 0;
            bool[] lWinStates = new bool[2] { false, false };

            // Check horizontals
            for (int i = 0; i < 3; i++)
                if (pGridObjects[i * 3].State != -1 && pGridObjects[i * 3].State == pGridObjects[i * 3 + 1].State && pGridObjects[i * 3 + 1].State == pGridObjects[i * 3 + 2].State)
                {
                    pVictoryTurn = pGridObjects[i * 3 + 2].State;

                    lWinStates[pGridObjects[i * 3 + 2].State] = true;

                    lVictoryMask |= (1 << i * 3) | (1 << i * 3 + 1) | (1 << i * 3 + 2);
                }

            // Check verticals
            for (int i = 0; i < 3; i++)
                if (pGridObjects[i].State != -1 && pGridObjects[i].State == pGridObjects[i + 3].State && pGridObjects[i + 3].State == pGridObjects[i + 6].State)
                {
                    pVictoryTurn = pGridObjects[i + 6].State;

                    lWinStates[pGridObjects[i + 6].State] = true;

                    lVictoryMask |= (1 << i) | (1 << i + 3) | (1 << i + 6);
                }

            // Check diagonals
            for (int i = 0; i < 2; i++)
                if (pGridObjects[4 - (i * 2 + 2)].State != -1 && pGridObjects[4 - (i * 2 + 2)].State == pGridObjects[4].State && pGridObjects[4].State == pGridObjects[4 + (i * 2 + 2)].State)
                {
                    pVictoryTurn = pGridObjects[4 + (i * 2 + 2)].State;

                    lWinStates[pGridObjects[4 + (i * 2 + 2)].State] = true;

                    lVictoryMask |= (1 << 4 - (i * 2 + 2)) | (1 << 4) | (1 << 4 + (i * 2 + 2));
                }

            // Disable blocks that are not part of the victory
            if (lVictoryMask > 0)
            {
                for (int i = 0; i < 9; i++)
                    if (((1 << i) & lVictoryMask) == 0)
                        pGridObjects[i].SetDisabled();

                pGameState = lWinStates[0] == lWinStates[1] ? GameState.Draw : GameState.Victory;

                return true;
            }

            // Are there moves available
            for (int i = 0; i < 9; i++)
                if (pGridObjects[i].State == -1)
                    return false;

            // No victory, no more moves
            pGameState = GameState.Draw;
            return true;
        }

        // Check up to 9 grid spaces
        private bool LocalValidate(int GridIndex)
        {
            int lx = GridIndex / 3;
            int ly = GridIndex % 3;

            for (int i = 0; i < 9; i++)
            {
                int x = i / 3 - 1 + lx;
                int y = i % 3 - 1 + ly;

                if (x >= 0 && x < 3 && y >= 0 && y < 3)
                {
                    int li = x * 3 + y;

                    if (pGridObjects[li].IsRotating() && (li & 1) != 0)
                        return false;
                }
            }

            return !pGridObjects[GridIndex].IsRotating() && pGridObjects[GridIndex].State != pTurn;
        }

        // Check wether move would be valid
        protected override bool ValidateMove(int GridIndex)
        {
            // Prevent any action while any space is rotating
            for (int i = 0; i < 9; i++)
                if (pGridObjects[i].IsRotating())
                    return false;

            return GridIndex != pLastMove && LocalValidate(GridIndex);
        }

        // Change up to 9 grid spaces
        private void LocalPerformMove(int GridIndex)
        {
            int lx = GridIndex / 3;
            int ly = GridIndex % 3;

            for (int i = 0; i < 9; i++)
            {
                int x = i / 3 - 1 + lx;
                int y = i % 3 - 1 + ly;

                if (x >= 0 && x < 3 && y >= 0 && y < 3)
                {
                    int li = x * 3 + y;

                    if ((i & 1) != 0)
                        pGridObjects[li].SetState(pGridObjects[li].State == -1 ? -1 : (pGridObjects[li].State + 1) & 1, false);

                    pGridObjects[GridIndex].SetState(pTurn, false);
                }
            }
        }

        // Try to perform the move
        protected override void TryPerformMove(int GridIndex)
        {
            // Perform move if it is valid
            if (ValidateMove(GridIndex))
            {
                LocalPerformMove(GridIndex);

                pTurn = (pTurn + 1) % 2;

                pLastMove = GridIndex;
            }
        }

        protected override void OnValidMove(int GridIndex)
        {
            ShowValidMoves();
        }
    }
}