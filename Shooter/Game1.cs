using System;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Schedulers;
using Shooter.Components;
using Shooter.Systems;

namespace Shooter;

public class Game1 : Game
{
    private World _world;
    private JobScheduler _jobScheduler;
    private Random _random;

    private InputSystem _inputSystem;
    private UserControlSystem _userControlSystem;
    private MovementSystem _movementSystem;
    private CollisionSystem _collisionSystem;
    private ColorSystem _colorSystem;
    private DrawSystem _drawSystem;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _ballTexture;
    private Texture2D _playerTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _random = new Random();
        _ballTexture = new Texture2D(GraphicsDevice, 16, 16);
        _playerTexture = new Texture2D(GraphicsDevice, 32, 256);

        var data = new Color[16 * 16];
        for (var i = 0; i < data.Length; ++i)
            data[i] = Color.White;
        _ballTexture.SetData(data);

        data = new Color[32 * 256];
        for (var i = 0; i < data.Length; ++i)
            data[i] = Color.Black;
        _playerTexture.SetData(data);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void BeginRun()
    {
        base.BeginRun();

        _world = World.Create();
        _jobScheduler = new(new JobScheduler.Config
        {
            ThreadCount = 0,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        });
        World.SharedJobScheduler = _jobScheduler;

        _inputSystem = new InputSystem(_world);
        _userControlSystem = new UserControlSystem(_world);
        _collisionSystem = new CollisionSystem(_world);
        _movementSystem = new MovementSystem(_world);
        _colorSystem = new ColorSystem(_world);
        _drawSystem = new DrawSystem(_world, _spriteBatch);

        _world.Create(
            new Position { Vector = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f) },
            new Rigidbody { Velocity = new Vector2(0.5f, 0.05f), MaxVelocity = new Vector2(0.25f, 0.25f), BouncingFactor = 1},
            new Collider {X1 = -8, Y1 = -8, X2 = 8,  Y2 = 8},
            new Sprite { Texture = _ballTexture, Color = _random.NextColor() }
        );

        _world.Create(
            new Input { PlayerIndex = 1 },
            new Position { Vector = new Vector2(_graphics.PreferredBackBufferWidth - 128, _graphics.PreferredBackBufferHeight / 2f - 128) },
            new Rigidbody { Velocity = new Vector2(0, 0), MaxVelocity = new Vector2(0.25f, 0.25f), BouncingFactor = 0},
            new Collider {X1 = -16, Y1 = -128, X2 = 16,  Y2 = 128},
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
        _world.Create(
            new Input { PlayerIndex = 2 },
            new Position { Vector = new Vector2(128, _graphics.PreferredBackBufferHeight / 2f - 128) },
            new Rigidbody { Velocity = new Vector2(0, 0), MaxVelocity = new Vector2(0.25f, 0.25f),  BouncingFactor = 0},
            new Collider {X1 = -16, Y1 = -128, X2 = 16,  Y2 = 128},
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _inputSystem.Update(gameTime);
        _userControlSystem.Update(gameTime);
        _collisionSystem.Update(gameTime);
        _movementSystem.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.White);
        _colorSystem.Update(in gameTime);
        _drawSystem.Update(in gameTime);

        base.Draw(gameTime);
    }

    protected override void EndRun()
    {
        base.EndRun();

        World.Destroy(_world);
        _jobScheduler.Dispose();
    }
}