using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Shooter;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NextVector2(this Random random, in Rectangle rectangle)
    {
        return new(random.Next(rectangle.X, rectangle.X + rectangle.Width), random.Next(rectangle.Y, rectangle.Y + rectangle.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NextVector2(this Random random, float min, float max)
    {
        return new((float)((random.NextDouble() * (max - min)) + min), (float)((random.NextDouble() * (max - min)) + min));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color NextColor(this Random random)
    {
        return new(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
    }
}