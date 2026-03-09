using Gum.DataTypes.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;

namespace NEA
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Vector2 _spritePosition;
        private Vector2 _spriteVelocity;
        private Texture2D _spriteSheet;
        Rigidbody playerBody;
        Rigidbody ground;
        Texture2D groundTexture;
        bool isGrounded;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
            
        }
        void ResolveGroundCollision()
        {
            Rectangle player = playerBody.GetBounds();
            Rectangle floor = ground.GetBounds();

            // Check if player is falling onto the floor
            if (playerBody.Velocity.Y >= 0)
            {
                float newY = floor.Top - playerBody.Height / 2;

                playerBody.Position = new Vector2(playerBody.Position.X, newY);

                playerBody.SetVelocity(new Vector2(playerBody.Velocity.X, 0));
            }
        }

        protected override void Initialize()
        {
            _spritePosition = new Vector2(640, 360);
            


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load your sprite sheet texture
            _spriteSheet = Content.Load<Texture2D>("images/sprite");

            // Create a texture region from the top-left frame
            int frameWidth = 30;
            int frameHeight = 32;
            var textureRegion = new TextureRegion(_spriteSheet, 0, 0, frameWidth, frameHeight);

            // Initialize sprite with the single texture region
            _sprite = new Sprite(textureRegion);
            _sprite.CenterOrigin();
            
            // Scale the sprite (1.0 = original size, 2.0 = double size, etc.)
            _sprite.Scale = new Vector2(3.0f, 3.0f);

            playerBody = new Rigidbody(
            new Vector2(400, 200), // starting position
            30,
            32
            );

            groundTexture = new Texture2D(GraphicsDevice, 1, 1);
            groundTexture.SetData(new[] { Color.White });

            ground = new Rigidbody(new Vector2(640, 650), 1280, 50);
            ground.IsKinematic = true; // ground should not move
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            
            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            

            Vector2 moveForce = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.A))
                playerBody.AddForce(new Vector2(-4000, 0));

            if (keyboardState.IsKeyDown(Keys.D))
                playerBody.AddForce(new Vector2(4000, 0));

            if (keyboardState.IsKeyDown(Keys.Space) && isGrounded)
                playerBody.AddForce(new Vector2(0, -5000)); // jump
            playerBody.Update(gameTime);
            // Update sprite position based on velocity and delta time
            if (playerBody.CheckCollision(ground))
            {
                isGrounded = true;
                ResolveGroundCollision();
            }
            _spritePosition = playerBody.Position;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            
            // Draw sprite at current position
            _sprite.Draw(_spriteBatch, _spritePosition);
            _spriteBatch.Draw(
    groundTexture,
    ground.GetBounds(),
    Color.Green
);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
