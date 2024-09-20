using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOfLife : MonoBehaviour
{
    private Cell[,] cells;
    private float cellSize = 0.1f;
    private int numberOfColums, numberOfRows;
    private int spawnChancePercentage = 20;
    private int targetFrameRate = 10;
    private bool showUI = true; 
    
    public GameObject cellPrefab;
    public Slider percentageSlider;
    public TMP_Text spawnChanceText;
    public Slider frameRateSlider;
    public TMP_Text frameRateText;
    public TMP_Dropdown patternDropdown;
    public GameObject uiPanel;
    public Button toggleButton;
    public TMP_Text toggleButtonText;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        InitializeGrid();
        InitializeUI();
    }

    void Update()
    {
        bool[,] nextGeneration = new bool[numberOfColums, numberOfRows];

        for (int y = 0; y < numberOfRows; y++)
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);

                if (cells[x, y].alive)
                {
                    nextGeneration[x, y] = aliveNeighbors >= 2 && aliveNeighbors <= 3;
                }
                else
                {
                    nextGeneration[x, y] = aliveNeighbors == 3;
                }
            }
        }

        for (int x = 0; x < numberOfColums; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                cells[x, y].alive = nextGeneration[x, y];
                cells[x, y].UpdateStatus();
            }
        }
    }

    private void InitializeGrid()
    {
        numberOfColums = (int)Mathf.Floor((Camera.main.orthographicSize * Camera.main.aspect * 2) / cellSize + 2f);
        numberOfRows = (int)Mathf.Floor(Camera.main.orthographicSize * 2 / cellSize + 2f);
        cells = new Cell[numberOfColums, numberOfRows];

        for (int y = 0; y < numberOfRows; y++)
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                Vector2 newPos = new Vector2(x * cellSize - Camera.main.orthographicSize * Camera.main.aspect,
                    y * cellSize - Camera.main.orthographicSize);

                var newCell = Instantiate(cellPrefab, newPos, Quaternion.identity);
                newCell.transform.localScale = Vector2.one * cellSize;
                cells[x, y] = newCell.GetComponent<Cell>();

                UpdateCellState(x, y);

                cells[x, y].UpdateStatus();
            }
        }
    }

    private void InitializeUI()
    {
        if (percentageSlider)
        {
            percentageSlider.value = spawnChancePercentage;
            percentageSlider.onValueChanged.AddListener(OnSpawnChanceChanged);
        }

        if (spawnChanceText)
        {
            spawnChanceText.text = spawnChancePercentage + "%";
        }

        if (frameRateSlider)
        {
            frameRateSlider.value = targetFrameRate;
            frameRateSlider.onValueChanged.AddListener(OnFrameRateChanged);
        }

        if (frameRateText)
        {
            frameRateText.text = targetFrameRate + "fps";
        }

        if (patternDropdown)
        {
            patternDropdown.options.Clear();
            patternDropdown.options.Add(new TMP_Dropdown.OptionData("Randomize"));
            patternDropdown.options.Add(new TMP_Dropdown.OptionData("Acorn"));
            patternDropdown.options.Add(new TMP_Dropdown.OptionData("Pulsar"));
            patternDropdown.options.Add(new TMP_Dropdown.OptionData("Gosper Glider Gun"));
            patternDropdown.onValueChanged.AddListener(OnPatternChanged);
        }

        if (uiPanel)
        {
            toggleButton.onClick.AddListener(toggleUI);
        }
    }



    private void UpdateAllCells()
    {
        for (int y = 0; y < numberOfRows; y++)
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                UpdateCellState(x, y);
                cells[x, y].UpdateStatus();
            }
        }
    }
    
    private void UpdateCellState(int x, int y)
    {
        if (Random.Range(0, 100) < spawnChancePercentage)
        {
            cells[x, y].alive = true;
        }
        else
        {
            cells[x, y].alive = false;
        }
    }

    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int checkX = x + i;
                int checkY = y + j;

                if (checkX >= 0 && checkX < numberOfColums && checkY >= 0 && checkY < numberOfRows)
                {
                    if (cells[checkX, checkY].alive)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private void OnSpawnChanceChanged(float value)
    {
        spawnChancePercentage = (int)value;
        spawnChanceText.text = spawnChancePercentage + "%";
        
        UpdateAllCells();
    }

    private void OnFrameRateChanged(float value)
    {
        targetFrameRate = (int)value;
        Application.targetFrameRate = targetFrameRate;
        frameRateText.text = targetFrameRate + "fps";
    }

    private void OnPatternChanged(int patternIndex)
    {
        ClearAllCells();

        switch (patternIndex)
        {
            case 0:
                UpdateAllCells();
                break;
            case 1:
                ApplyPattern(Patterns.acornPattern);
                break;
            case 2:
                ApplyPattern(Patterns.pulsarPattern);
                break;
            case 3:
                ApplyPattern(Patterns.gosperGliderGunPattern);
                break;
        }
    }

    private void ClearAllCells()
    {
        for (int y = 0; y < numberOfRows; y++)
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                cells[x, y].alive = false;
                cells[x, y].UpdateStatus();
            }
        }
    }

    private void ApplyPattern(int[,] pattern)
    {
        int patternWidth = pattern.GetLength(1); //alive cell
        int patternHeight = pattern.GetLength(0); //dead cell
        
        int startX = numberOfColums / 2 - patternWidth / 2;
        int startY = numberOfRows / 2 - patternHeight / 2;

        for (int y = 0; y < patternHeight; y++)
        {
            for (int x = 0; x < patternWidth; x++)
            {
                if (startX + x >= 0 && startX + x < numberOfColums && startY + y >= 0 && startY + y < numberOfRows)
                {
                    cells[startX + x, startY + y].alive = pattern[y, x] == 1;
                    cells[startX + x, startY + y].UpdateStatus();
                }
            }
        }
    }
    
    public void toggleUI()
    {
        showUI = !showUI;
        uiPanel.SetActive(showUI);
        toggleButtonText.text = showUI ? "Hide UI" : "Show UI";
    }
}
