using System;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Schedulers;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.EventComponents;
using Shooter.Services;
using Shooter.Services.InputHandlers;
using Shooter.Services.MovementHandlers;
using Shooter.Systems;
using World = Arch.Core.World;

namespace Shooter.GameStates;

public class PongGameState(GraphicsDevice graphicsDevice, ContentManager contentManager) : IGameState
{
    private World? _world;
    private nkast.Aether.Physics2D.Dynamics.World? _physicsWorld;
    private JobScheduler? _jobScheduler;
    private readonly PhysicObjectManager _physicObjectManager = new();
    private readonly InputManager _inputManager = new();
    private readonly MovementManager _movementManager = new();
    private readonly GameManager _gameManager = new() { Status = GameStatus.Playing };
    private float _accumulator;
    private float _alpha;

    //Systems
    private UserInputSystem? _userInputSystem;
    private InputHandleSystem? _inputHandleSystem;
    private MovementSystem? _movementSystem;
    private BallCollisionSystem? _ballCollisionSystem;
    private PlayerCollisionSystem? _playerCollisionSystem;
    private CollisionCleanupSystem? _collisionCleanupSystem;
    private PhysicsSystem? _physicsSystem;
    private SyncSystem? _syncSystem;
    private DrawSystem? _drawSystem;
    private UiSystem? _uiSystem;

    //Graphics
    private Texture2D? _ballTexture;
    private Texture2D? _player1Texture;
    private Texture2D? _player2Texture;

    public GameStateCommand Command { get; private set; }

    public void Enter()
    {
        _ballTexture = contentManager.Load<Texture2D>("Sprites/ball");
        _player1Texture = contentManager.Load<Texture2D>("Sprites/player1");
        _player2Texture = contentManager.Load<Texture2D>("Sprites/player2");

        _world = World.Create();
        _jobScheduler = new JobScheduler(new JobScheduler.Config
        {
            ThreadCount = 0,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        });
        World.SharedJobScheduler = _jobScheduler;
        _physicsWorld = new nkast.Aether.Physics2D.Dynamics.World
        {
            Gravity = new nkast.Aether.Physics2D.Common.Vector2(0, 0)
        };

        _inputManager.Register(MovementTypes.VerticalPaddle, new VerticalPaddlerInputHandler());

        _movementManager.Register(MovementTypes.VerticalPaddle, new VerticalPaddlerMovementHandler());
        _movementManager.Register(MovementTypes.Ball, new BallMovementHandler());

        _userInputSystem = new UserInputSystem(_world);
        _inputHandleSystem = new InputHandleSystem(_world, _inputManager);
        _movementSystem = new MovementSystem(_world, _movementManager, _physicObjectManager);
        _ballCollisionSystem = new BallCollisionSystem(_world);
        _playerCollisionSystem = new PlayerCollisionSystem(_world);
        _collisionCleanupSystem = new CollisionCleanupSystem(_world);
        _physicsSystem = new PhysicsSystem(_world, _physicsWorld);
        _syncSystem = new SyncSystem(_world, _physicObjectManager);
        _drawSystem = new DrawSystem(_world, new SpriteBatch(graphicsDevice), _gameManager);
        _uiSystem = new UiSystem(_world, _gameManager);

        CreateLevel();
    }

    public void Exit()
    {
        if (_world is not null)
            World.Destroy(_world);
        _jobScheduler?.Dispose();
    }

    public void Update(GameTime time)
    {
        if (_gameManager.Status == GameStatus.End)
        {
            Command = GameStateCommand.ExitToMenu;
            return;
        }

        if (_gameManager.Status == GameStatus.Playing)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _gameManager.Status = GameStatus.Paused;
                return;
            }

            const float fixedStep = 1f / 100;
            _accumulator += (float)time.ElapsedGameTime.TotalSeconds;

            _userInputSystem!.FixedUpdate(fixedStep);
            _inputHandleSystem!.FixedUpdate(fixedStep);

            _playerCollisionSystem!.FixedUpdate(fixedStep);
            _ballCollisionSystem!.FixedUpdate(fixedStep);
            _collisionCleanupSystem!.FixedUpdate(fixedStep);

            _movementSystem!.FixedUpdate(fixedStep);

            while (_accumulator > fixedStep)
            {
                _physicsSystem!.FixedUpdate(fixedStep);
                _syncSystem!.FixedUpdate(fixedStep);

                _accumulator -= fixedStep;
            }

            _alpha = _accumulator / fixedStep;
        }
    }

    public void Draw(GameTime time)
    {
        graphicsDevice.Clear(new Color(255, 238, 204, 255));

        if (_drawSystem is not null)
        {
            _drawSystem.Alpha = _alpha;
            _drawSystem.Update(in time);
        }

        _uiSystem?.Update(time);
    }

    private void CreateLevel()
    {
        if (_world is null
            || _physicsWorld is null
            || _ballTexture is null
            || _player1Texture is null
            || _player2Texture is null)
            return;

        //ball
        var ballEntity = _world.Create(
            new ActualMovement(),
            new TargetMovement
            {
                MaxVelocity = 30,
                Velocity = 40,
                Direction = new System.Numerics.Vector2(1, 0.2f),
                Type = MovementTypes.Ball,
                NeedToMove = true
            },
            new Sprite { Texture = _ballTexture, Color = Color.Red },
            new RectangleCollider { Width = 1.6f, Height = 1.6f, Layer = CollisionLayer.Ball },
            new Ball());

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

        _physicObjectManager.Add(ballEntity, PhysicObjectTypes.PhysicsBody, ballBody, PhysicTags.MainBody);

        //player 1
        var player1Entity = _world.Create(
            new UserInput { PlayerIndex = 1 },
            new TargetMovement
            {
                MaxVelocity = 25,
                TargetForce = 100000,
                Type = MovementTypes.VerticalPaddle
            },
            new ActualMovement(),
            new Sprite { Texture = _player1Texture, Color = Color.Black },
            new Player { Index = 1 },
            new RectangleCollider { Width = 3.2f, Height = 20f, Layer = CollisionLayer.Player }
        );

        var player1Body = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(70, 30),
            bodyType: BodyType.Dynamic);
        player1Body.Mass = 0.1f;
        player1Body.FixedRotation = true;
        player1Body.LinearDamping = 0;
        player1Body.AngularDamping = 0;
        var player1Collider = player1Body.CreateRectangle(3.2f, 20f, 1f, nkast.Aether.Physics2D.Common.Vector2.Zero);
        player1Collider.Friction = 0;
        player1Collider.Restitution = 1;

        _physicObjectManager.Add(player1Entity, PhysicObjectTypes.PhysicsBody, player1Body, PhysicTags.MainBody);

        //joint for player 1
        var anchorPlayer1 = _physicsWorld.CreateBody(new nkast.Aether.Physics2D.Common.Vector2(70, 30));
        var jointPlayer1 = new PrismaticJoint(
            anchorPlayer1,
            player1Body,
            anchorPlayer1.Position,
            new nkast.Aether.Physics2D.Common.Vector2(0, 1f))
        {
            LimitEnabled = true,
            LowerLimit = -20,
            UpperLimit = 20,
            MotorEnabled = true
        };

        _physicsWorld.Add(jointPlayer1);
        _physicObjectManager.Add(player1Entity, PhysicObjectTypes.PrismaticJoint, jointPlayer1);

        //player 2
        var player2Entity = _world.Create(
            new UserInput { PlayerIndex = 2 },
            new TargetMovement
            {
                MaxVelocity = 25,
                TargetForce = 100000,
                Type = MovementTypes.VerticalPaddle
            },
            new ActualMovement(),
            new Sprite { Texture = _player2Texture, Color = Color.Black },
            new Player { Index = 2 },
            new RectangleCollider { Width = 3.2f, Height = 20f, Layer = CollisionLayer.Player }
        );

        var player2Body = _physicsWorld.CreateBody(
            new nkast.Aether.Physics2D.Common.Vector2(10, 30),
            bodyType: BodyType.Dynamic);
        player2Body.Mass = 0.1f;
        player2Body.FixedRotation = true;
        player2Body.LinearDamping = 0;
        player2Body.AngularDamping = 0;
        var player2Collider = player2Body.CreateRectangle(3.2f, 20f, 1f, nkast.Aether.Physics2D.Common.Vector2.Zero);
        player2Collider.Friction = 0;
        player2Collider.Restitution = 1;

        _physicObjectManager.Add(player2Entity, PhysicObjectTypes.PhysicsBody, player2Body, PhysicTags.MainBody);

        //joint for player 2
        var anchorPlayer2 = _physicsWorld.CreateBody(new nkast.Aether.Physics2D.Common.Vector2(10, 30));
        var jointPlayer2 = new PrismaticJoint(
            anchorPlayer2,
            player2Body,
            anchorPlayer2.Position,
            new nkast.Aether.Physics2D.Common.Vector2(0, 1f))
        {
            LimitEnabled = true,
            LowerLimit = -20,
            UpperLimit = 20,
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
        var wallBottomCollider =
            wallBottomBody.CreateRectangle(80, 0.1f, 1, nkast.Aether.Physics2D.Common.Vector2.Zero);
        wallBottomCollider.Friction = 0;
        wallBottomCollider.Restitution = 1;

        // //player 1 lose catcher
        // var player1LoseCatcher = _world.Create(
        //     new RectangleCollider { Width = 1f, Height = 60f, Layer = CollisionLayer.LooseCatcher },
        //     new LooseCatcher { PlayerIndex = 1 }
        // );
        //
        // var player1LooseCatcherBody = _physicsWorld.CreateBody(
        //     new nkast.Aether.Physics2D.Common.Vector2(70, 30),
        //     bodyType: BodyType.Static);
        // var player1LooseCatcher = player1LooseCatcherBody.CreateRectangle(1f, 60f, 1f,
        //     nkast.Aether.Physics2D.Common.Vector2.Zero);
        // player1LooseCatcher.Friction = 0;
        // player1LooseCatcher.Restitution = 0;
        //
        // //player 2 lose catcher
        // var player2LoseCatcher = _world.Create(
        //     new RectangleCollider { Width = 1f, Height = 60f, Layer = CollisionLayer.LooseCatcher },
        //     new LooseCatcher { PlayerIndex = 2 }
        // );
        //
        // var player2LooseCatcherBody = _physicsWorld.CreateBody(
        //     new nkast.Aether.Physics2D.Common.Vector2(10, 30),
        //     bodyType: BodyType.Static);
        // var player2LooseCatcher = player2LooseCatcherBody.CreateRectangle(1f, 60f, 1f,
        //     nkast.Aether.Physics2D.Common.Vector2.Zero);
        // player2LooseCatcher.Friction = 0;
        // player2LooseCatcher.Restitution = 0;

        //physics events
        _physicsWorld.ContactManager.BeginContact += contact =>
        {
            var bodyA = contact.FixtureA.Body;
            var entityA = _physicObjectManager.GetEntity(bodyA);

            var bodyB = contact.FixtureB.Body;
            var entityB = _physicObjectManager.GetEntity(bodyB);

            if (!entityA.HasValue
                || !entityA.Value.Has<RectangleCollider>()
                || !entityB.HasValue
                || !entityB.Value.Has<RectangleCollider>())
                return true;

            var colliderA = entityA.Value.Get<RectangleCollider>();
            var colliderB = entityB.Value.Get<RectangleCollider>();

            contact.GetWorldManifold(out var normal, out var points);
            var sideA = GetEdgeFromNormal(new Vector2(-normal.X, -normal.Y));
            var sideB = GetEdgeFromNormal(new Vector2(normal.X, normal.Y));

            _world.Add(entityA.Value, new Collision
            {
                Normal = new System.Numerics.Vector2(-normal.X, -normal.Y),
                Point = new System.Numerics.Vector2(points[0].X, points[0].Y),
                OtherLayer = colliderB.Layer,
                OtherVelocity = new System.Numerics.Vector2(bodyB.LinearVelocity.X, bodyB.LinearVelocity.Y),
                OtherEdge = GetEdgeFromNormal(new Vector2(normal.X, normal.Y)),
                OtherEdgeLength = sideB is RectEdge.Top or RectEdge.Bottom ? colliderB.Height : colliderB.Width
            });

            _world.Add(entityB.Value, new Collision
            {
                Normal = new System.Numerics.Vector2(normal.X, normal.Y),
                Point = new System.Numerics.Vector2(points[1].X, points[1].Y),
                OtherLayer = colliderA.Layer,
                OtherVelocity = new System.Numerics.Vector2(bodyA.LinearVelocity.X, bodyA.LinearVelocity.Y),
                OtherEdge = GetEdgeFromNormal(new Vector2(-normal.X, -normal.Y)),
                OtherEdgeLength = sideA is RectEdge.Left or RectEdge.Right ? colliderA.Height : colliderA.Width
            });

            return true;
        };
    }

    private static RectEdge GetEdgeFromNormal(Vector2 normal)
    {
        if (MathF.Abs(normal.X) > MathF.Abs(normal.Y))
            return normal.X > 0 ? RectEdge.Left : RectEdge.Right;

        return normal.Y > 0 ? RectEdge.Bottom : RectEdge.Top;
    }

    #region Disposing

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (_world is not null)
                World.Destroy(_world);
            _jobScheduler?.Dispose();
        }

        _disposed = true;
    }

    ~PongGameState() => Dispose(false);

    #endregion
}