using Gum.Forms;
using Microsoft.Xna.Framework;
using MonoGameGum;
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

        GumService.Default.Initialize(this, DefaultVisualsVersion.V3);
        
        ChangeState(new MainMenuGameState(_graphics.GraphicsDevice));
    }

    protected override void Update(GameTime gameTime)
    {
        if(_currentState?.Command is not null and not GameStateCommand.None)
            HandleCommand(_currentState.Command);
        
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

    private void HandleCommand(GameStateCommand command)
    {
        switch (command)
        {
            case GameStateCommand.Exit:
                Exit();
                break;
            case GameStateCommand.ExitToMenu:
                ChangeState(new MainMenuGameState(_graphics.GraphicsDevice));
                break;
            case GameStateCommand.Start:
                ChangeState(new PongGameState(_graphics.GraphicsDevice));
                break;
            case GameStateCommand.None:
            case GameStateCommand.Pause:
            default:
                return;
        }
    }
    
    private void ChangeState(IGameState newState)
    {
        _currentState?.Exit();
        _currentState?.Dispose();

        _currentState = newState;
        _currentState.Enter();
    }
}