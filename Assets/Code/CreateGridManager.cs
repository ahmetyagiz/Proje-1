using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGridManager : MonoBehaviour
{
    private int gridSize = 5; // Grid boyutu
    public GameObject cellPrefab; // UI h�cre prefab'i
    public Transform gridParent; // Grid'in yerle�tirilece�i parent

    private GameObject[,] gridArray; // Grid h�crelerini saklamak i�in dizi
    private bool[,] grid; // 2D grid durumu
    private const int targetMarkedCount = 3; // Gizlemek i�in gereken i�aretli h�cre say�s�
    private bool[,] visited; // H�crelerin ziyaret edilip edilmedi�ini takip etmek i�in

    [SerializeField] private InputFieldManager inputFieldManager; // Zenject ile y�netilecek

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
        visited = new bool[gridSize, gridSize]; // Ziyaret edilen h�creleri takip edece�iz
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
        // Grid'in geni�li�i ve y�ksekli�i
        float gridWidth = gridSize * cellSize;
        float gridHeight = gridSize * cellSize;
        return new Vector2(-gridWidth / 2 + cellSize / 2, -gridHeight / 2 + cellSize / 2);
    }

    private void CreateCell(int x, int y, Vector2 startPosition, float cellSize)
    {
        // Butonlar� olu�tur
        Vector2 position = startPosition + new Vector2(x * cellSize, y * cellSize);
        GameObject newCell = Instantiate(cellPrefab, gridParent);
        RectTransform rectTransform = newCell.GetComponent<RectTransform>();

        // Butonu konumland�r
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

        gridArray[x, y] = newCell;
        newCell.name = $"Cell ({x}, {y})";

        // Butonun t�klama olay�n� ayarla
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

    // Kom�ular� kontrol eden DFS fonksiyonu
    private bool FindAndDestroyMatch(int startX, int startY)
    {
        visited = new bool[gridSize, gridSize]; // Ziyaret edilen h�creleri s�f�rla
        List<Vector2Int> matchingCells = new List<Vector2Int>(); // E�le�en h�creleri toplayaca��z
        DFS(startX, startY, matchingCells);

        // E�er e�le�en h�cre say�s� 3 veya fazlaysa true d�neriz
        return matchingCells.Count >= targetMarkedCount;
    }

    // DFS ile kom�u h�creleri bul ve e�le�enleri listeye ekle
    private void DFS(int x, int y, List<Vector2Int> matchingCells)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize || visited[x, y] || !grid[x, y])
        {
            return;
        }

        visited[x, y] = true;
        matchingCells.Add(new Vector2Int(x, y)); // E�le�en h�creleri kaydet

        // 4 y�nl� arama: yukar�, a�a��, sol, sa�
        DFS(x + 1, y, matchingCells);
        DFS(x - 1, y, matchingCells);
        DFS(x, y + 1, matchingCells);
        DFS(x, y - 1, matchingCells);
    }

    // E�le�en h�creler ve kom�ular�n� gizle
    private void HideConnectedCells(int startX, int startY)
    {
        visited = new bool[gridSize, gridSize];
        List<Vector2Int> connectedCells = new List<Vector2Int>();
        DFS(startX, startY, connectedCells); // E�le�en ve ba�l� h�creleri topla

        foreach (var cell in connectedCells)
        {
            // H�crenin ilk child'�n� kapat
            gridArray[cell.x, cell.y].transform.GetChild(0).gameObject.SetActive(false);
            grid[cell.x, cell.y] = false; // ��aretli h�creyi s�f�rla
        }
    }
}