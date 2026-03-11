using UnityEngine;

public enum TileType { Path, Buildable }

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    private int _col;
    private int _row;
    private SpriteRenderer _renderer;

    public int Col => _col;
    public int Row => _row;
    public TileType TileType { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_renderer != null, "[Tile] SpriteRenderer component is missing");
    }

    public void Initialize(int col, int row)
    {
        _col = col;
        _row = row;
        // Default to Buildable until ColorTiles() runs in Start(). This prevents
        // stale TileType.Path reads during the Awake→Start initialization window.
        TileType = TileType.Buildable;
    }

    public void Initialize(TileType tileType, Color color)
    {
        TileType = tileType;
        _renderer.color = color;
    }
}
