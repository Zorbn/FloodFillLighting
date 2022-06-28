namespace Sandbox;

public class TileMap
{
    public readonly int Width;
    public readonly int Height;
    private readonly int tileSize;

    private readonly int[] tiles;
    
    public TileMap(int width, int height, int tileSize)
    {
        this.Width = width;
        this.Height = height;
        this.tileSize = tileSize;

        tiles = new int[width * height];
    }

    public bool TrySetTile(int x, int y, int type)
    {
        if (GetTile(x, y) != 0) return false;
        
        SetTile(x, y, type);
        return true;
    }
    
    public void SetTile(int x, int y, int type)
    {
        tiles[x + y * Width] = type;
    }

    public int GetTile(int x, int y)
    {
        return tiles[x + y * Width];
    }

    public bool IsTileEmpty(int x, int y)
    {
        return GetTile(x, y) == 0;
    }
}