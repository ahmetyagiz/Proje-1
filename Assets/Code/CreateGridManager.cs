using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGridManager : MonoBehaviour
{
    private int gridSize = 5; // Grid boyutu
    public GameObject cellPrefab; // UI hücre prefab'i
    public Transform gridParent; // Grid'in yerleþtirileceði parent

    private GameObject[,] gridArray; // Grid hücrelerini saklamak için dizi
    private bool[,] grid; // 2D grid durumu
    private const int targetMarkedCount = 3; // Gizlemek için gereken iþaretli hücre sayýsý
    private bool[,] visited; // Hücrelerin ziyaret edilip edilmediðini takip etmek için

    [SerializeField] private InputFieldManager inputFieldManager; // Zenject ile yönetilecek

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        DestroyOldGrid();
        gridSize = inputFieldManager.ReadLastInput();
        CreateBoolGrid();
        InitializeGridArray();
        CreateCells();
    }

    private void CreateBoolGrid()
    {
        grid = new bool[gridSize, gridSize];
        visited = new bool[gridSize, gridSize]; // Ziyaret edilen hücreleri takip edeceðiz
    }

    private void DestroyOldGrid()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    private void InitializeGridArray()
    {
        gridArray = new GameObject[gridSize, gridSize];
    }

    private void CreateCells()
    {
        float cellSize = 300 / gridSize;
        Vector2 startPosition = GetStartPosition(cellSize);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                CreateCell(x, y, startPosition, cellSize);
            }
        }
    }

    private Vector2 GetStartPosition(float cellSize)
    {
        // Grid'in geniþliði ve yüksekliði
        float gridWidth = gridSize * cellSize;
        float gridHeight = gridSize * cellSize;
        return new Vector2(-gridWidth / 2 + cellSize / 2, -gridHeight / 2 + cellSize / 2);
    }

    private void CreateCell(int x, int y, Vector2 startPosition, float cellSize)
    {
        // Butonlarý oluþtur
        Vector2 position = startPosition + new Vector2(x * cellSize, y * cellSize);
        GameObject newCell = Instantiate(cellPrefab, gridParent);
        RectTransform rectTransform = newCell.GetComponent<RectTransform>();

        // Butonu konumlandýr
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

        gridArray[x, y] = newCell;
        newCell.name = $"Cell ({x}, {y})";

        // Butonun týklama olayýný ayarla
        Button button = newCell.GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonClickEvent(button, x, y));
    }

    private void OnButtonClickEvent(Button button, int x, int y)
    {
        button.transform.GetChild(0).gameObject.SetActive(true);
        grid[x, y] = true;

        if (FindAndDestroyMatch(x, y))
        {
            HideConnectedCells(x, y);
        }
    }

    // Komþularý kontrol eden DFS fonksiyonu
    private bool FindAndDestroyMatch(int startX, int startY)
    {
        visited = new bool[gridSize, gridSize]; // Ziyaret edilen hücreleri sýfýrla
        List<Vector2Int> matchingCells = new List<Vector2Int>(); // Eþleþen hücreleri toplayacaðýz
        DFS(startX, startY, matchingCells);

        // Eðer eþleþen hücre sayýsý 3 veya fazlaysa true döneriz
        return matchingCells.Count >= targetMarkedCount;
    }

    // DFS ile komþu hücreleri bul ve eþleþenleri listeye ekle
    private void DFS(int x, int y, List<Vector2Int> matchingCells)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize || visited[x, y] || !grid[x, y])
        {
            return;
        }

        visited[x, y] = true;
        matchingCells.Add(new Vector2Int(x, y)); // Eþleþen hücreleri kaydet

        // 4 yönlü arama: yukarý, aþaðý, sol, sað
        DFS(x + 1, y, matchingCells);
        DFS(x - 1, y, matchingCells);
        DFS(x, y + 1, matchingCells);
        DFS(x, y - 1, matchingCells);
    }

    // Eþleþen hücreler ve komþularýný gizle
    private void HideConnectedCells(int startX, int startY)
    {
        visited = new bool[gridSize, gridSize];
        List<Vector2Int> connectedCells = new List<Vector2Int>();
        DFS(startX, startY, connectedCells); // Eþleþen ve baðlý hücreleri topla

        foreach (var cell in connectedCells)
        {
            // Hücrenin ilk child'ýný kapat
            gridArray[cell.x, cell.y].transform.GetChild(0).gameObject.SetActive(false);
            grid[cell.x, cell.y] = false; // Ýþaretli hücreyi sýfýrla
        }
    }
}