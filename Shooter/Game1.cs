using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Schedulers;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.Managers;
using Shooter.Systems;
using World = Arch.Core.World;

namespace Shooter;

public class Game1 : Game
{
    private World _world;
    private nkast.Aether.Physics2D.Dynamics.World _physicsWorld;
    private JobScheduler _jobScheduler;
    private PhysicObjectManager _physicObjectManager;

    private InputSystem _inputSystem;
    private MovementSystem _movementSystem;
    private PhysicsSystem _physicsSystem;
    private SyncSystem _syncSystem;
    private ColorSystem _colorSystem;
    private DrawSystem _drawSystem;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _ballTexture;
    private Texture2D _playerTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _ballTexture = new Texture2D(GraphicsDevice, 16, 16);
        _playerTexture = new Texture2D(GraphicsDevice, 32, 192);

        var data = new Color[16 * 16];
        for (var i = 0; i < data.Length; ++i)
            data[i] = Color.White;
        _ballTexture.SetData(data);

        data = new Color[32 * 192];
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
        _jobScheduler = new JobScheduler(new JobScheduler.Config
        {
            ThreadCount = 0,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        });
        World.SharedJobScheduler = _jobScheduler;
        _physicsWorld = new nkast.Aether.Physics2D.Dynamics.World()
        {
            Gravity = new nkast.Aether.Physics2D.Common.Vector2(0, 0)
        };

        _physicObjectManager = new PhysicObjectManager();

        _inputSystem = new InputSystem(_world);
        _movementSystem = new MovementSystem(_world);
        _physicsSystem = new PhysicsSystem(_world, _physicsWorld);
        _syncSystem = new SyncSystem(_world);
        _colorSystem = new ColorSystem(_world);
        _drawSystem = new DrawSystem(_world, _spriteBatch);

        CreateEntities();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _inputSystem.Update(gameTime);
        _movementSystem.Update(gameTime);
        _physicsSystem.Update(gameTime);
        _syncSystem.Update(gameTime);

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

    private void CreateEntities()
    {
        //ball
        var ballBody = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(40, 30),
            bodyType: BodyType.Dynamic);
        ballBody.Mass = 10;
        ballBody.IsBullet = true;
        ballBody.LinearDamping = 0;
        ballBody.AngularDamping = 0;
        ballBody.FixedRotation = true;
        
        var ballCollider = ballBody.CreateRectangle(1.6f, 1.6f, 1f, nkast.Aether.Physics2D.Common.Vector2.Zero);
        ballCollider.Restitution = 1f;
        ballCollider.Friction = 0;

        _world.Create(
            new Position { Vector = new Vector2(40, 30) },
            new Sprite { Texture = _ballTexture, Color = Color.Red });
        
        ballBody.ApplyLinearImpulse(new nkast.Aether.Physics2D.Common.Vector2(100f, 50f));

        //player 1
        var player1Entity = _world.Create(
            new Input { PlayerIndex = 1 },
            new Position { Vector = new Vector2(70, 30) },
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
        
        var player1Body = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(70, 30),
            bodyType: BodyType.Dynamic);
        player1Body.Mass = 1000;
        player1Body.FixedRotation = true;
        player1Body.LinearDamping = 0;
        var player1Collider = player1Body.CreateRectangle(3.2f, 19.2f, 1f, nkast.Aether.Physics2D.Common.Vector2.Zero);
        player1Collider.Friction = 0;
        player1Collider.Restitution = 1;

        _physicObjectManager.Add(player1Entity, PhysicObjectTypes.PhysicsBody, player1Body);
        
        //joint for player 1
        var anchorPlayer1 = _physicsWorld.CreateBody(new nkast.Aether.Physics2D.Common.Vector2(70, 30));
        var jointPlayer1 = new PrismaticJoint(
            anchorPlayer1,
            player1Body,
            anchorPlayer1.Position,
            new nkast.Aether.Physics2D.Common.Vector2(0, 1f))
        {
            LimitEnabled = true,
            LowerLimit = -25,
            UpperLimit = 25,
            MotorEnabled = true
        };
        
        _physicsWorld.Add(jointPlayer1);
        _physicObjectManager.Add(player1Entity, PhysicObjectTypes.PrismaticJoint, jointPlayer1);

        //player 2
        var player2Entity = _world.Create(
            new Input { PlayerIndex = 2 },
            new Position { Vector = new Vector2(10, 30) },
            new Sprite { Texture = _playerTexture, Color = Color.Black }
        );
        
        var player2Body = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(10, 30),
            bodyType: BodyType.Dynamic);
        player2Body.Mass = 10000;
        player2Body.FixedRotation = true;
        player2Body.LinearDamping = 0;
        var player2Collider = player2Body.CreateRectangle(3.2f, 19.2f, 1f, nkast.Aether.Physics2D.Common.Vector2.Zero);
        player2Collider.Friction = 0;
        player2Collider.Restitution = 1;
        
        _physicObjectManager.Add(player2Entity, PhysicObjectTypes.PhysicsBody, player2Body);
        
        //joint for player 2
        var anchorPlayer2 = _physicsWorld.CreateBody(new nkast.Aether.Physics2D.Common.Vector2(70, 30));
        var jointPlayer2 = new PrismaticJoint(
            anchorPlayer2,
            player2Body,
            anchorPlayer2.Position,
            new nkast.Aether.Physics2D.Common.Vector2(0, 1f))
        {
            LimitEnabled = true,
            LowerLimit = -25,
            UpperLimit = 25,
            MotorEnabled = true
        };
        
        _physicsWorld.Add(jointPlayer2);
        _physicObjectManager.Add(player2Entity, PhysicObjectTypes.PrismaticJoint, jointPlayer2);
        
        //wall left
        var wallLeftBody = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(0, 30),
            bodyType: BodyType.Static);
        var wallLeftCollider = wallLeftBody.CreateRectangle(0.1f, 80, 1, nkast.Aether.Physics2D.Common.Vector2.Zero);
        wallLeftCollider.Friction = 0;
        wallLeftCollider.Restitution = 1;
        
        //wall top
        var wallTopBody = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(40, 0),
            bodyType: BodyType.Static);
        var wallTopCollider = wallTopBody.CreateRectangle(80, 0.1f, 1, nkast.Aether.Physics2D.Common.Vector2.Zero);
        wallTopCollider.Friction = 0;
        wallTopCollider.Restitution = 1;
        
        //wall right
        var wallRightBody = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(80, 30),
            bodyType: BodyType.Static);
        var wallRightCollider = wallRightBody.CreateRectangle(0.1f, 80, 1, nkast.Aether.Physics2D.Common.Vector2.Zero);
        wallRightCollider.Friction = 0;
        wallRightCollider.Restitution = 1;
        
        //wall bottom
        var wallBottomBody = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(40, 60),
            bodyType: BodyType.Static);
        var wallBottomCollider = wallBottomBody.CreateRectangle(80, 0.1f, 1, nkast.Aether.Physics2D.Common.Vector2.Zero);
        wallBottomCollider.Friction = 0;
        wallBottomCollider.Restitution = 1;
    }
}