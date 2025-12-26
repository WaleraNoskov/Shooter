using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Shooter.Contracts;

namespace Shooter.GameStates;

public class MainMenuGameState(GraphicsDevice graphicsDevice) : IGameState
{
    private Desktop? _desktop;

    public GameStateCommand Command { get; private set; } = GameStateCommand.None;

    public void Enter()
    {
        SetStyle();
        RenderMenu();
    }

    public void Exit()
    {
        _desktop?.Dispose();
        _desktop = null;
    }

    public void Update(GameTime time)
    {
    }

    public void Draw(GameTime time)
    {
        graphicsDevice.Clear(new Color(70, 66, 94));
        _desktop?.Render();
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
            Border = new SolidBrush(new Color(255, 105, 115))
        };
        defaultButtonStyle.BorderThickness = new Thickness(4);
        Stylesheet.Current.ButtonStyles.Add("default", defaultButtonStyle);
    }

    private void RenderMenu()
    {
        //Main grid
        var grid = new Grid();
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));

        //Logo
        var logo = new Label
        {
            Text = "Super Pong!",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = Stylesheet.Current.Fonts["display"],
            TextColor = new Color(255, 238, 204)
        };

        grid.Widgets.Add(logo);

        //Buttons panel
        var buttonsPanel = new VerticalStackPanel
        {
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        Grid.SetRow(buttonsPanel, 1);
        grid.Widgets.Add(buttonsPanel);

        //Start button
        var startButton = new Button
        {
            Content = new Label
            {
                Text = "Start",
                Font = Stylesheet.Current.Fonts["normal"],
                HorizontalAlignment =  HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = new Color(70, 66, 94)
            },
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        startButton.SetStyle("default");
        startButton.Width = 226;
        startButton.Height = 48;
        startButton.Click += (_, _) => Command = GameStateCommand.Start;

        buttonsPanel.Widgets.Add(startButton);

        //Exit button
        var exitButton = new Button
        {
            Content = new Label
            {
                Text = "Exit",
                Font = Stylesheet.Current.Fonts["normal"],
                HorizontalAlignment =  HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = new Color(70, 66, 94)
            },
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        exitButton.SetStyle("default");
        exitButton.Width = 228;
        exitButton.Height = 48;
        exitButton.Click += (_, _) => Command = GameStateCommand.Exit;

        buttonsPanel.Widgets.Add(exitButton);

        _desktop = new Desktop();
        _desktop.Root = grid;
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
            _desktop?.Dispose();
            _desktop = null;
        }

        _disposed = true;
    }

    ~MainMenuGameState() => Dispose(false);

    #endregion
}