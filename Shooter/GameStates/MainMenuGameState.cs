using System;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using Shooter.Contracts;

namespace Shooter.GameStates;

public class MainMenuGameState(GraphicsDevice graphicsDevice) : IGameState
{
    private GumService GumService => GumService.Default;

    public GameStateCommand Command { get; private set; } = GameStateCommand.None;

    public void Enter()
    {
        RenderMenu();
    }

    public void Exit()
    {
    }

    public void Update(GameTime time)
    {
        GumService.Default.Update(time);
    }

    public void Draw(GameTime time)
    {
        graphicsDevice.Clear(Color.White);
        GumService.Default.Draw();
    }

    private void RenderMenu()
    {
        var title = new Label { Text = "Super Pong!"};
        var titleVisual = (LabelVisual)title.Visual;
        titleVisual.Color = Color.Black;
        
        var startButton = new Button { Text = "Start" };
        startButton.Click += (_, _) => Command = GameStateCommand.Start;
        
        var exitButton = new Button { Text = "Exit" };
        exitButton.Click += (_, _) => Command = GameStateCommand.Exit;

        var stackPanel = new StackPanel { Spacing = 4 };
        stackPanel.AddToRoot();
        stackPanel.AddChild(title);
        stackPanel.AddChild(startButton);
        stackPanel.AddChild(exitButton);
        stackPanel.Anchor(Gum.Wireframe.Anchor.Center);
        stackPanel.AddToRoot();
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
        }

        _disposed = true;
    }

    ~MainMenuGameState() => Dispose(false);

    #endregion
}