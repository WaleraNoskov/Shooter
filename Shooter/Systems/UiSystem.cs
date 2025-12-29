using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arch.Core;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Shooter.Components;

namespace Shooter.Systems;

public class UiSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToDisplay = new QueryDescription().WithAll<Player>();

    private readonly Desktop _desktop = new();
    private Grid? _oldScoreGrid;

    public override void Update(in GameTime gameTime)
    {
        List<Player> players = [];

        var query = World.Query(in _entitiesToDisplay);
        foreach (ref var chunk in query)
        {
            chunk.GetSpan<Player>();

            foreach (var index in chunk)
            {
                var player = chunk.Get<Player>(index);
                players.Add(player);
            }
        }

        ReRenderPlayersScores(players);
        _desktop.Render();
    }

    private void ReRenderPlayersScores(List<Player> players)
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

        if (_oldScoreGrid is not null)
            _desktop.Widgets.Remove(_oldScoreGrid);

        if (_oldScoreGrid is not null)
            _desktop.Widgets.Remove(_oldScoreGrid);
        _oldScoreGrid = grid;
        
        _desktop.Widgets.Add(grid);
    }
}