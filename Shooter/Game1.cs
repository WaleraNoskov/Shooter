using Microsoft.Xna.Framework;
using Shooter.Contracts;
using Shooter.GameStates;
using Game = Microsoft.Xna.Framework.Game;

namespace Shooter;

public class Game1 : Game
{
    private IGameState? _currentState;
    private readonly GraphicsDeviceManager _graphics;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 600
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        ChangeState(new PongGameState());
    }

    protected override void Update(GameTime gameTime)
    {
        _currentState?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _currentState?.Draw(gameTime);
        base.Draw(gameTime);
    }

    protected override void EndRun()
    {
        _currentState?.Exit();
        base.EndRun();
    }

    private void ChangeState(IGameState newState)
    {
        _currentState?.Exit();
        _currentState?.Dispose();

        _currentState = newState;
        _currentState.Enter(_graphics.GraphicsDevice);
    }
}