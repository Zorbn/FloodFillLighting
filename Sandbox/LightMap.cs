using System.Numerics;
using Raylib_CsLo;

namespace Sandbox;

public unsafe class LightMap
{
    private const int ColorComponents = 4;
    private const int LightLevels = 16;
    
    private readonly int width;
    private readonly int height;
    private readonly int tileSize;

    private readonly int[,] lightmap;
    private readonly byte[] lightPixels;
    private readonly Dictionary<Point, bool> lights;

    private readonly Texture lightTex;
    private readonly Rectangle lightTexSrc;
    private readonly Rectangle lightTexDst;

    public LightMap(int width, int height, int tileSize)
    {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        
        lightmap = new int[width, height];
        lightPixels = new byte[width * height * ColorComponents];
        lights = new Dictionary<Point, bool>();

        Image lightImg = new()
        {
            width = width,
            height = height,
            format = (int) PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
            mipmaps = 1
        };

        lightTex = Raylib.LoadTextureFromImage(lightImg);
        
        Raylib.UnloadImage(lightImg);
        
        lightTexSrc = new Rectangle(0f, 0f, width, height);
        lightTexDst = new Rectangle(0f, 0, width * tileSize, height * tileSize);
    }

    public void UpdateAt(TileMap tileMap, int x, int y)
    {
        for (int ix = Math.Max(x - LightLevels, 0); ix <= Math.Min(x + LightLevels, width - 1); ix++)
        {
            for (int iy = Math.Max(y - LightLevels, 0); iy <= Math.Min(y + LightLevels, height - 1); iy++)
            {
                lightmap[ix, iy] = 0;

                Point affectedPoint = new(ix, iy);
                if (lights.ContainsKey(affectedPoint))
                {
                    lights[affectedPoint] = true;
                }
            }
        }

        int xBorderStart = Math.Max(x - LightLevels - 1, 0);
        int xBorderEnd = Math.Min(x + LightLevels + 1, width - 1);
        int yBorderStart = Math.Max(y - LightLevels - 1, 0);
        int yBorderEnd = Math.Min(y + LightLevels + 1, height - 1);

        void FloodFillRect(int xStart, int xEnd, int yStart, int yEnd)
        {
            for (int ix = xStart; ix <= xEnd; ix++)
            {
                for (int iy = yStart; iy <= yEnd; iy++)
                {
                    FloodFill(ix, iy, lightmap[ix, iy], tileMap, lightmap, true);
                }
            }
        }

        FloodFillRect(xBorderStart + 1, xBorderEnd - 1, yBorderStart, yBorderStart);
        FloodFillRect(xBorderStart + 1, xBorderEnd - 1, yBorderEnd, yBorderEnd);
        FloodFillRect(xBorderStart, xBorderStart, yBorderStart + 1, yBorderEnd - 1);
        FloodFillRect(xBorderEnd, xBorderEnd, yBorderStart + 1, yBorderEnd - 1);
    }
    
    public void Update(TileMap tileMap)
    {
        foreach ((Point light, bool needsUpdate) in lights)
        {
            if (!needsUpdate) continue;
            FloodFill(light.X, light.Y, LightLevels, tileMap, lightmap);
            lights[light] = false;
        }
    }

    public void Draw()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int i = (x + y * width) * ColorComponents;
                Color color = GetLightColor(lightmap[x, y], LightLevels);
                lightPixels[i] = color.r;
                lightPixels[i + 1] = color.g;
                lightPixels[i + 2] = color.b;
                lightPixels[i + 3] = color.a;
            }
        }
            
        fixed (byte* pToPixels = &lightPixels[0])
        {
            Raylib.UpdateTexture(lightTex, pToPixels);
        }
        
        Raylib.DrawTexturePro(lightTex, lightTexSrc, lightTexDst, Vector2.Zero, 0f, Raylib.WHITE);        
    }

    public void AddLight(Vector2 position)
    {
        int x = (int)(position.X / tileSize);
        int y = (int)(position.Y / tileSize);

        if (x < 0 || x >= width || y < 0 || y >= height) return;
        
        Point newLight = new(x, y);

        if (!lights.ContainsKey(newLight))
        {
            lights.Add(newLight, true);
        }
    }

    ~LightMap()
    {
        Raylib.UnloadTexture(lightTex);
    }
    
    private static void FloodFill(int x, int y, int lightLevel, TileMap tileMap, int[,] lightmap, bool force = false)
    {
        if (lightLevel < 1) return;
        if (x < 0 || x >= tileMap.Width) return;
        if (y < 0 || y >= tileMap.Height) return;
        if (lightmap[x, y] >= lightLevel && !force) return;

        lightmap[x, y] = lightLevel;

        int nextLightLevel = lightLevel - 1;
        
        // Light should pass through only 1 tile.
        if (!tileMap.IsTileEmpty(x, y) && nextLightLevel > 1) nextLightLevel = 1;
        
        FloodFill(x + 1, y, nextLightLevel, tileMap, lightmap);
        FloodFill(x - 1, y, nextLightLevel, tileMap, lightmap);
        FloodFill(x, y + 1, nextLightLevel, tileMap, lightmap);
        FloodFill(x, y - 1, nextLightLevel, tileMap, lightmap);
    }

    private static Color GetLightColor(int lightLevel, int lightLevels)
    {
        Color color = Raylib.BLACK;
        float delta = (float) lightLevel / lightLevels;
        color.a = MathUtils.Lerp(color.a, 0, delta);

        return color;
    }
}