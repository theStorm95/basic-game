using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private Color _pathTileColor      = new Color(0.76f, 0.60f, 0.42f); // tan/brown
    [SerializeField] private Color _buildableTileColor = new Color(0.27f, 0.55f, 0.27f); // green

    private Tile[,] _grid;
    private Texture2D _tileTexture;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildGrid();
    }

    private void Start()
    {
        Debug.Assert(PathManager.Instance != null, "[GridManager] PathManager.Instance is null in Start");
        if (PathManager.Instance == null) return;
        ColorTiles();
    }

    private void BuildGrid()
    {
        int width = GameConstants.GRID_WIDTH;
        int height = GameConstants.GRID_HEIGHT;
        _grid = new Tile[width, height];

        Sprite tileSprite = CreateWhiteSquareSprite();

        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                float x = col - width / 2f + 0.5f;
                float y = row - height / 2f + 0.5f;

                var go = new GameObject($"Tile_{col}_{row}");
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x, y, 0f);
                go.transform.localScale = new Vector3(0.95f, 0.95f, 1f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = tileSprite;

                var tile = go.AddComponent<Tile>();
                tile.Initialize(col, row);
                _grid[col, row] = tile;
            }
        }

        GameLog.Info("GridManager", $"Built {width}x{height} grid");
    }

    private void ColorTiles()
    {
        HashSet<Vector2Int> pathCells = PathManager.Instance.GetPathCells();

        for (int col = 0; col < GameConstants.GRID_WIDTH; col++)
        {
            for (int row = 0; row < GameConstants.GRID_HEIGHT; row++)
            {
                // gridX/gridY are integer grid coordinates, NOT tile world-space centers.
                // Tile center is at (gridX + 0.5, gridY + 0.5). PathManager waypoints use
                // these same integer grid coordinates, so the lookup is correct.
                int gridX = col - GameConstants.GRID_WIDTH / 2;
                int gridY = row - GameConstants.GRID_HEIGHT / 2;

                TileType type = pathCells.Contains(new Vector2Int(gridX, gridY))
                    ? TileType.Path
                    : TileType.Buildable;

                _grid[col, row].Initialize(type, type == TileType.Path ? _pathTileColor : _buildableTileColor);
            }
        }
    }

    private Sprite CreateWhiteSquareSprite()
    {
        _tileTexture = new Texture2D(2, 2);
        _tileTexture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        _tileTexture.Apply();
        return Sprite.Create(_tileTexture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 2f);
    }

    private void OnDestroy()
    {
        if (_tileTexture != null)
            Destroy(_tileTexture);
    }

    public Tile GetTile(int col, int row)
    {
        if (col < 0 || col >= GameConstants.GRID_WIDTH || row < 0 || row >= GameConstants.GRID_HEIGHT)
            return null;
        return _grid[col, row];
    }
}
