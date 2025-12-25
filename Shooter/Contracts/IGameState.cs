using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter.Contracts;

public interface IGameState : IDisposable
{
    public GameStateCommand Command { get; }
    
    void Enter();
    void Exit();

    void Update(GameTime time);
    void Draw(GameTime time);
}