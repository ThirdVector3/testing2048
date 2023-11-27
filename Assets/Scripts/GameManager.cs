using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private Cell[,] cells = new Cell[4, 4];
    [SerializeField] private GameObject[] poses;
    private Vector2[,] positions = new Vector2[4,4];
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    private void Start()
    {
        gameOverPanel.SetActive(false);
        cells = new Cell[4, 4];
        positions = new Vector2[4, 4];
        InputManager.MoveInput.AddListener(MoveAction);
        for (int i = 0;  i < poses.Length; i++)
        {
            positions[i % 4, i / 4] = poses[i].transform.position;
        }
        Tile firstTile = Instantiate(tilePrefab, positions[1, 2], Quaternion.identity, tilesParent);
        Tile secondTile = Instantiate(tilePrefab, positions[2, 2], Quaternion.identity, tilesParent);
        cells[1, 2] = new Cell(firstTile);
        cells[2, 2] = new Cell(secondTile);
    }
    private void MoveAction(Vector2 direction)
    {
        StartCoroutine(IEMoveAction(direction));
    }
    private IEnumerator IEMoveAction(Vector2 direction)
    {
        Cell[,] startActionSells = new Cell[4,4];
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                startActionSells[x, y].tile = cells[x, y].tile;
        if (direction == Vector2.up || direction == Vector2.down)
        {
            GravityVertical(direction == Vector2.down, LeanTweenType.easeInCubic);
            yield return new WaitForSeconds(0.1f);
            CompareVertical();
            yield return new WaitForSeconds(0.05f);
            GravityVertical(direction == Vector2.down, LeanTweenType.easeOutCubic);
        }
        else if (direction == Vector2.right || direction == Vector2.left)
        {
            GravityHorizontal(direction == Vector2.right, LeanTweenType.easeInCubic);
            yield return new WaitForSeconds(0.1f);
            CompareHorizontal();
            yield return new WaitForSeconds(0.05f);
            GravityHorizontal(direction == Vector2.right, LeanTweenType.easeOutCubic);
        }
        if (IsSameFilling(startActionSells, cells))
            yield break;
        SpawnNew();
        scoreText.text = GetNowScore().ToString();
        if (!HasLegalMoves())
            GameOver();
    }
    private void GravityVertical(bool down, LeanTweenType easing)
    {
        for (int x = 0; x < 4; x++)
        {
            if (down)
            {
                for (int y = 3; y >= 0; y--)
                {
                    if (cells[x, y].tile == null)
                        continue;
                    for (int y2 = 3; y2 >= y; y2--)
                    {
                        if (cells[x, y2].tile == null)
                        {
                            cells[x, y].tile.transform.LeanMove(positions[x, y2], 0.1f).setEase(easing);
                            cells[x, y2].tile = cells[x, y].tile;
                            cells[x, y].tile = null;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < 4; y++)
                {
                    if (cells[x, y].tile == null)
                        continue;
                    for (int y2 = 0; y2 < y; y2++)
                    {
                        if (cells[x, y2].tile == null)
                        {
                            cells[x, y].tile.transform.LeanMove(positions[x, y2], 0.1f).setEase(easing);
                            cells[x, y2].tile = cells[x, y].tile;
                            cells[x, y].tile = null;
                            break;
                        }
                    }
                }
            }
        }
    }
    private void GravityHorizontal(bool right, LeanTweenType easing)
    {
        for (int y = 0; y < 4; y++)
        {
            if (right)
            {
                for (int x = 3; x >= 0; x--)
                {
                    if (cells[x, y].tile == null)
                        continue;
                    for (int x2 = 3; x2 >= x; x2--)
                    {
                        if (cells[x2, y].tile == null)
                        {
                            cells[x, y].tile.transform.LeanMove(positions[x2, y], 0.1f).setEase(easing);
                            cells[x2, y].tile = cells[x, y].tile;
                            cells[x, y].tile = null;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 4; x++)
                {
                    if (cells[x, y].tile == null)
                        continue;
                    for (int x2 = 0; x2 < x; x2++)
                    {
                        if (cells[x2, y].tile == null)
                        {
                            cells[x, y].tile.transform.LeanMove(positions[x2, y], 0.1f).setEase(easing);
                            cells[x2, y].tile = cells[x, y].tile;
                            cells[x, y].tile = null;
                            break;
                        }
                    }
                }
            }
        }
    }
    private void CompareVertical()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (cells[x, y].tile != null && cells[x, y + 1].tile != null && cells[x, y].tile.value == cells[x, y + 1].tile.value)
                {
                    cells[x, y].tile.transform.LeanMove(positions[x, y + 1], 0.05f);
                    Destroy(cells[x, y].tile.gameObject);
                    cells[x, y].tile = null;
                    cells[x, y + 1].tile.value++;
                    cells[x, y + 1].tile.UpdateColor();
                    cells[x, y + 1].tile.transform.LeanScale(Vector3.one * 0.4f, 0.1f).setEaseOutCubic();
                    cells[x, y + 1].tile.transform.LeanScale(Vector3.one * 0.18f, 0.1f).setEaseInCubic();
                }
            }
        }
    }
    private void CompareHorizontal()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (cells[x,y].tile != null && cells[x + 1, y].tile != null && cells[x, y].tile.value == cells[x + 1, y].tile.value)
                {
                    cells[x, y].tile.transform.LeanMove(positions[x + 1, y], 0.05f);
                    Destroy(cells[x, y].tile.gameObject);
                    cells[x, y].tile = null;
                    cells[x + 1, y].tile.value++;
                    cells[x + 1, y].tile.UpdateColor();
                    cells[x + 1, y].tile.transform.LeanScale(Vector3.one * 0.4f, 0.1f).setEaseOutCubic();
                    cells[x + 1, y].tile.transform.LeanScale(Vector3.one * 0.18f, 0.1f).setEaseInCubic();
                }
            }
        }
    }
    private void SpawnNew()
    {
        List<Vector2Int> availableCells = new List<Vector2Int>();
        for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
                if (cells[x,y].tile == null)
                    availableCells.Add(new Vector2Int(x,y));
        Vector2Int randomCell = availableCells[Random.Range(0,availableCells.Count)];
        Tile spawnedTile = Instantiate(tilePrefab, positions[randomCell.x, randomCell.y], Quaternion.identity, tilesParent);
        spawnedTile.value = Random.Range(0, 2);
        spawnedTile.UpdateColor();
        cells[randomCell.x, randomCell.y] = new Cell(spawnedTile);
    }
    private bool IsSameFilling(Cell[,] cells1, Cell[,] cells2)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (!(cells1[x, y].tile == cells2[x, y].tile))
                    return false;
            }
        }
        return true;
    }
    private int GetNowScore()
    {
        int score = 0;
        foreach (Cell cell in cells)
        {
            score += cell.tile ? Mathf.RoundToInt(Mathf.Pow(2, cell.tile.value)) : 0;
        }
        return score;
    }
    private bool HasLegalMoves()
    {
        foreach (Cell cell in cells)
        {
            if (cell.tile == null)
                return true;
        }
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (cells[x, y].tile != null && cells[x, y + 1].tile != null && cells[x, y].tile.value == cells[x, y + 1].tile.value)
                {
                    return true;
                }
            }
        }
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (cells[x, y].tile != null && cells[x + 1, y].tile != null && cells[x, y].tile.value == cells[x + 1, y].tile.value)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
    public void TryAgain()
    {
        SceneManager.LoadScene("Game");
    }
}
