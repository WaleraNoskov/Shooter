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
    private AutoMovementSystem _autoMovementSystem;
    private ManualMovementSystem _manualMovementSystem;
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
        _jobScheduler = new(new JobScheduler.Config()
        {
            ThreadCount = 0,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        });
        World.SharedJobScheduler = _jobScheduler;

        _inputSystem = new InputSystem(_world);
        _autoMovementSystem = new AutoMovementSystem(_world, GraphicsDevice.Viewport.Bounds);
        _manualMovementSystem = new ManualMovementSystem(_world);
        _colorSystem = new ColorSystem(_world);
        _drawSystem = new DrawSystem(_world, _spriteBatch);

        _world.Create(
            new Position { Vector = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f) },
            new Velocity { Vector = _random.NextVector2(-0.25f, 0.25f) },
            new Sprite { Texture = _ballTexture, Color = _random.NextColor() }
        );

        _world.Create(
            new Input() { PlayerIndex = 1 },
            new Position { Vector = new Vector2(_graphics.PreferredBackBufferWidth - 128, _graphics.PreferredBackBufferHeight / 2f - 128) },
            new Velocity { Vector = new Vector2(0.25f, 0.25f) },
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
        _world.Create(
            new Input() { PlayerIndex = 2 },
            new Position { Vector = new Vector2(128, _graphics.PreferredBackBufferHeight / 2f - 128) },
            new Velocity { Vector = new Vector2(0.25f, 0.25f) },
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

//         // Add a random amount of new entities
//         if (Keyboard.GetState().IsKeyDown(Keys.K))
//         {
//             // Bulk create entities
//             const int amount = 3;
//             Span<Entity> entities = stackalloc Entity[amount];
//             _world.Create(entities, [typeof(Position), typeof(Velocity), typeof(Sprite)], amount);
//
//             // Set variables
//             foreach (var entity in entities)
//             {
// #if DEBUG_PUREECS || RELEASE_PUREECS
//                 _world.Set(entity,
//                     new Position { Vector = _random.NextVector2(GraphicsDevice.Viewport.Bounds) },
//                     new Velocity { Vector = _random.NextVector2(-0.25f, 0.25f) },
//                     new Sprite { Texture2D = _texture2D, Color = _random.NextColor() }
//                 );
// #else
//                 entity.Set(
//                     new Position { Vector = _random.NextVector2(GraphicsDevice.Viewport.Bounds) },
//                     new Velocity { Vector = _random.NextVector2(-0.25f, 0.25f) },
//                     new Sprite { Texture = _ballTexture, Color = _random.NextColor() }
//                 );
// #endif
//             }
//         }
//
//         // Remove a random amount of new entities
//         if (Keyboard.GetState().IsKeyDown(Keys.L))
//         {
//             // Find all entities
//             var entities = new Entity[_world.Size];
//             _world.GetEntities(new QueryDescription().WithNone<Input>(), entities.AsSpan());
//
//             // Delete random entities
//             var amount = Math.Min(3, entities.Length);
//             for (var index = 0; index < amount; index++)
//             {
//                 var randomIndex = _random.Next(0, entities.Length);
//                 var randomEntity = entities[randomIndex];
//
// #if DEBUG_PUREECS || RELEASE_PUREECS
//                 if (_world.IsAlive(randomEntity))
// #else
//                 if (randomEntity.IsAlive())
// #endif
//                 {
//                     _world.Destroy(randomEntity);
//                 }
//
//                 entities[randomIndex] = Entity.Null;
//             }
//         }

        _inputSystem.Update(gameTime);
        _autoMovementSystem.Update(in gameTime);
        _manualMovementSystem.Update(gameTime);
        _colorSystem.Update(in gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.White);
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