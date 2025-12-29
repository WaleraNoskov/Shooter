using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Styles;
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
            PreferredBackBufferHeight = 600, 
            SynchronizeWithVerticalRetrace = true
        };

        IsFixedTimeStep = false;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        MyraEnvironment.Game = this;
        SetStyle();
        
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
                ChangeState(new PongGameState(_graphics.GraphicsDevice, Content));
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
    
    private static void SetStyle()
    {
        var ttfData = File.ReadAllBytes("./PixelifySans.ttf");

        var fontSystem = new FontSystem();
        fontSystem.AddFont(ttfData);

        Stylesheet.Current.Fonts.Add("display", fontSystem.GetFont(96));
        Stylesheet.Current.Fonts.Add("normal", fontSystem.GetFont(40));

        var defaultButtonStyle = new ButtonStyle
        {
            OverBackground = new SolidBrush(new Color(255, 105, 115)),
            Background = new SolidBrush(new Color(255, 176, 163)),
            Border = new SolidBrush(new Color(255, 105, 115)),
            BorderThickness = new Thickness(4)
        };
        Stylesheet.Current.ButtonStyles.Add("default", defaultButtonStyle);
    }
}