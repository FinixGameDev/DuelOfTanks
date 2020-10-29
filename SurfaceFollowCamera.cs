using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DuelOfTanks
{
    class SurfaceFollowCamera
    {
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        Vector3 position;
        float yaw, pitch;
        float offset;
        int width, height;
        float velocity;
        Map terrain;

        public SurfaceFollowCamera(GraphicsDevice device, Map terrain,Vector3 position, float yaw, float pitch)
        {
            this.position = position;
            this.yaw = yaw;
            this.pitch = pitch;
            this.terrain = terrain;
            velocity = 5f;
            offset = 1.7f;

            this.width = device.Viewport.Width;
            this.height = device.Viewport.Height;

            Matrix rotation = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f);
            Vector3 direction = Vector3.Transform(Vector3.UnitX, rotation);
            Vector3 target = this.position + direction;

            float aspectRatio = (float)device.Viewport.Width /
                device.Viewport.Height;
            viewMatrix = Matrix.CreateLookAt(this.position, target, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio, 0.1f, 1000.0f);
        }

        public void Update(GameTime gameTime, KeyboardState kb, MouseState ms)
        {
            float degreesPerPixelX = 0.3f;
            float degreesPerPixelY = 0.3f;
            int deltaX = ms.Position.X - width / 2;
            int deltaY = ms.Position.Y - height / 2;
            this.yaw -= deltaX * MathHelper.ToRadians(degreesPerPixelX);
            this.pitch -= deltaY * MathHelper.ToRadians(degreesPerPixelY);

            if (pitch >= Math.PI/3)
                pitch = (float)Math.PI / 3;
            else if (pitch <= -Math.PI/3)
                pitch = -(float)Math.PI / 3;

            Vector3 vectorBase = Vector3.UnitX;
            Vector3 directionH = Vector3.Transform(
                vectorBase, 
                Matrix.CreateRotationY(yaw));

            Vector3 right = Vector3.Cross(directionH, Vector3.UnitY);
            Matrix rotPitch = Matrix.CreateFromAxisAngle(right, pitch);

            Vector3 direction = Vector3.Transform(directionH, rotPitch);

            if (kb.IsKeyDown(Keys.Up))
                position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.Down))
                position -= direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.Right))
                position += right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.Left))
                position -= right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X >= 0 & position.X <= terrain.heightMap.Width & position.Z > 0 & position.Z < terrain.heightMap.Height - 1)
                position.Y = terrain.GetHeight(position.X, position.Z) + offset;

            Vector3 target = this.position + direction;

            viewMatrix = Matrix.CreateLookAt(this.position, target, Vector3.Up);

            Mouse.SetPosition(width / 2, height / 2);
        }
    }
}
