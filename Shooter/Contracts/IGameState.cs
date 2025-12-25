using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter.Contracts;

public interface IGameState : IDisposable
{
    void Enter(GraphicsDevice device);
    void Exit();

    void Update(GameTime time);
    void Draw(GameTime time);
}