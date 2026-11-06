
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace NEA.Scenes;

public class TilemapTestScene : Scene
{
    private Tilemap _tilemap;

    public override void LoadContent()
    {
        // Load the dungeon tileset image
        Texture2D texture =
            Content.Load<Texture2D>("Tiles/DungeonTileset");

        // Create a region covering the entire image
        TextureRegion region = new TextureRegion(
            texture, 0, 0, 240, 288
        );

        // Split it into 16x16 tiles
        Tileset tileset = new Tileset(region, 16, 16);

        // Create the tilemap
        _tilemap = new Tilemap(tileset, 15, 18);

        // Display every tile in its original position
        for (int i = 0; i < tileset.Count; i++)
        {
            _tilemap.SetTile(i, i);
        }

        // Double the tile size
        _tilemap.Scale = new Vector2(2, 2);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();

        _tilemap.Draw(Core.SpriteBatch);

        Core.SpriteBatch.End();

        base.Draw(gameTime);
    }
}

