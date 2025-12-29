using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.Services;

namespace Shooter.Systems;

public class UiSystem(World world, GameManager gameManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _players = new QueryDescription().WithAll<Player>();

    private readonly Desktop _desktop = new();
    private Widget? _oldUi;

    public override void Update(in GameTime gameTime)
    {
        var needToRerender = false;
        Widget? ui = null;

        if (gameManager.Status == GameStatus.Playing)
        {
            List<Player> players = [];

            var query = World.Query(in _players);
            foreach (ref var chunk in query)
            {
                chunk.GetSpan<Player>();

                foreach (var index in chunk)
                {
                    var player = chunk.Get<Player>(index);
                    players.Add(player);
                }
            }

            ui = GetPlayersScoresUi(players);
            needToRerender = true;
        }
        else if (gameManager.Status == GameStatus.Paused
                 && _oldUi?.Tag is string pauseTag 
                 && pauseTag != "pause menu")
        {
            ui = GetPauseMenuUi();
            needToRerender = true;
        }
        else if (gameManager.Status == GameStatus.Results
                 && _oldUi?.Tag is string resultsTag
                 && resultsTag != "results menu")
        {
            ui = GetResultUiWidget();
            needToRerender = true;
        }

        if (needToRerender)
        {
            _desktop.Widgets.Remove(_oldUi);
            _oldUi = ui;
            _desktop.Widgets.Add(ui);
        }

        _desktop.Render();
    }

    private static Widget GetPlayersScoresUi(List<Player> players)
    {
        var grid = new Grid { Tag = "score ui" };

        var index = 0;
        foreach (var player in players.OrderByDescending(p => p.Index))
        {
            var playerLabel = new Label
            {
                Text = $"Player {player.Index}",
                Font = Stylesheet.Current.Fonts["normal"],
                TextColor = new Color(70, 66, 94),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var playerScoreLabel = new Label
            {
                Text = player.Score.ToString(),
                Font = Stylesheet.Current.Fonts["display"],
                TextColor = new Color(70, 66, 94)
            };

            var stackPanel = new VerticalStackPanel
            {
                Spacing = 4,
                Margin = new Thickness(4, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Widgets.Add(playerLabel);
            stackPanel.Widgets.Add(playerScoreLabel);

            if (index == 0)
            {
                playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
                playerScoreLabel.HorizontalAlignment = HorizontalAlignment.Left;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (index == players.Count - 1)
            {
                playerLabel.HorizontalAlignment = HorizontalAlignment.Right;
                playerScoreLabel.HorizontalAlignment = HorizontalAlignment.Right;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            }

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
            Grid.SetColumn(stackPanel, index);
            grid.Widgets.Add(stackPanel);

            index++;
        }

        return grid;
    }

    private Widget GetPauseMenuUi()
    {
        //Main grid
        var grid = new Grid { Tag = "pause menu" };
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));

        //Logo
        var logo = new Label
        {
            Text = "Paused",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = Stylesheet.Current.Fonts["display"],
            TextColor = new Color(70, 66, 94)
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
                Text = "Continue",
                Font = Stylesheet.Current.Fonts["normal"],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = new Color(70, 66, 94)
            },
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        startButton.SetStyle("default");
        startButton.Width = 226;
        startButton.Height = 48;
        startButton.Click += OnContinueButtonClick;

        buttonsPanel.Widgets.Add(startButton);

        //Exit button
        var exitButton = new Button
        {
            Content = new Label
            {
                Text = "Exit to menu",
                Font = Stylesheet.Current.Fonts["normal"],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = new Color(70, 66, 94)
            },
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        exitButton.SetStyle("default");
        exitButton.Width = 228;
        exitButton.Height = 48;
        exitButton.Click += OnExitToMenuButtonClick;

        buttonsPanel.Widgets.Add(exitButton);

        return grid;
    }

    private Widget GetResultUiWidget()
    {
        //Main grid
        var grid = new Grid { Tag = "results menu" };
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));

        //Logo
        var logo = new Label
        {
            Text = "Results",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = Stylesheet.Current.Fonts["display"],
            TextColor = new Color(70, 66, 94)
        };

        grid.Widgets.Add(logo);

        //Buttons panel
        var resultsPanel = new VerticalStackPanel
        {
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        Grid.SetRow(resultsPanel, 1);
        grid.Widgets.Add(resultsPanel);

        //Winner panel
        var winnerLabel = new Label
        {
            Text = $"Winner - Player {gameManager.WinnerPlayerIndex}",
            Font = Stylesheet.Current.Fonts["normal"],
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextColor = new Color(70, 66, 94)
        };
        
        resultsPanel.Widgets.Add(winnerLabel);
        
        //Loose panel
        var looseLabel = new Label
        {
            Text = $"Looser - Player {gameManager.LoosePlayerIndex}",
            Font = Stylesheet.Current.Fonts["normal"],
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextColor = new Color(70, 66, 94)
        };
        
        resultsPanel.Widgets.Add(looseLabel);
        
        //Exit button
        var exitButton = new Button
        {
            Content = new Label
            {
                Text = "Exit to menu",
                Font = Stylesheet.Current.Fonts["normal"],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = new Color(70, 66, 94)
            },
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        exitButton.SetStyle("default");
        exitButton.Width = 228;
        exitButton.Height = 48;
        exitButton.Click += OnExitToMenuButtonClick;

        resultsPanel.Widgets.Add(exitButton);

        return grid;
    }

    private void OnContinueButtonClick(object? sender, EventArgs e) => gameManager.Status = GameStatus.Playing;
    private void OnExitToMenuButtonClick(object? sender, EventArgs e) => gameManager.Status = GameStatus.End;
}