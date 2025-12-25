using System;
using Microsoft.Xna.Framework;
using Shooter.Contracts;

namespace Shooter.GameStates;

public class MainMenuGameState : IGameState
{
    public GameStateCommand Command { get; }
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime time)
    {
        throw new System.NotImplementedException();
    }

    public void Draw(GameTime time)
    {
        throw new System.NotImplementedException();
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