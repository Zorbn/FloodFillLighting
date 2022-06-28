using System.Numerics;
using Raylib_CsLo;

namespace Sandbox;

/*
 * TODO:
 * Make tilemap into a class
 * Make lights count as tiles so a tile and a light can't occupy the same space.
 */

internal static class Program
{
    public static void Main()
    {
        Raylib.InitWindow(1280, 720, "FloodFill");
        
        const int width = 100;
        const int height = 100;
        const int tileSize = 16;

        TileMap tileMap = new(width, height, tileSize);
        LightMap lightMap = new(width, height, tileSize);
        
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.RAYWHITE);

            if (Raylib.IsMouseButtonDown(0))
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                int x = (int)(mousePos.X / tileSize);
                int y = (int)(mousePos.Y / tileSize);

                if (x is >= 0 and < width && y is >= 0 and < height)
                {
                    tileMap.TrySetTile(x, y, 1);
                }

                lightMap.UpdateAt(tileMap, x, y);
            }

            if (Raylib.IsMouseButtonDown(1))
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                lightMap.AddLight(mousePos);
            }

            lightMap.Update(tileMap);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tileMap.IsTileEmpty(x, y)) continue;

                    Color color = new(125, 55, 55, 255);
                    Raylib.DrawRectangle(x * tileSize, y * tileSize, tileSize, tileSize, color);
                }
            }

            lightMap.Draw();
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}