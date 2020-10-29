using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuelOfTanks
{
    class Map
    {
        BasicEffect effect;
        VertexBuffer vBuffer;
        IndexBuffer iBuffer;
        float[] heightMapData;
        public Texture2D heightMap;

        public Map(Texture2D heightMap, Texture2D texture,GraphicsDevice device)
        {

            this.heightMap = heightMap;

            // Vamos usar um efeito básico
            effect = new BasicEffect(device);
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width /
                device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(
                new Vector3(64.0f, 5.0f, 64.0f),
                new Vector3(64.0f, 0.0f, 0.0f), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio, 0.1f, 1000.0f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            // Cria os eixos 3D
            CreateGeometry(device);
        }
        private void CreateGeometry(GraphicsDevice device)
        {
            //Creat Height Map color Array
            Color[] heightColors;
            heightColors = new Color[heightMap.Width * heightMap.Height];
            heightMapData = new float[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(heightColors);

            //Transform color data into terrain height
            float verticalScale = 0.025f;
            for (int i = 0; i < heightMap.Width * heightMap.Height; i++)
                heightMapData[i] = heightColors[i].R * verticalScale;

            VertexPositionNormalTexture[] vertices;
            int vertexCount = heightMap.Width * heightMap.Height;
            vertices = new VertexPositionNormalTexture[vertexCount];

            for (int z = 0; z < heightMap.Height; z++)
                for (int x = 0; x < heightMap.Width; x++)
                {
                    int i;
                    i = z * heightMap.Width + x;

                    float h, u, v;
                    h = heightMapData[i];
                    u = x % 2;
                    v = z % 2;

                    vertices[i] = new VertexPositionNormalTexture(
                        new Vector3(x, h, z), 
                        Vector3.Up, 
                        new Vector2(u, v));

                }

            short[] index;
            int indexCount = (heightMap.Height - 1)*(heightMap.Width) * 2;
            index = new short[indexCount];

            for (int z = 0; z < heightMap.Height - 1; z++)
                for (int x = 0; x < heightMap.Width; x++)
                {
                    index[z * (2 * heightMap.Height) + 2 * x] = (short)(z + x * heightMap.Width);
                    index[z * (2 * heightMap.Height) + 2 * x + 1] = (short)(z + x * heightMap.Width + 1);
                }

            vBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            vBuffer.SetData<VertexPositionNormalTexture>(vertices);
            iBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            iBuffer.SetData<short>(index);
        }

        public void Draw(GraphicsDevice device, FreeCamera camera)
        {
            // World Matrix
            effect.Projection = camera.projectionMatrix;
            effect.View = camera.viewMatrix;
            effect.World = Matrix.Identity;


            // Set Effect to draw
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vBuffer);
            device.Indices = iBuffer;
            for (int i = 0; i < heightMap.Height; i++)
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,
                    0,
                    i * (2 * heightMap.Height),
                    heightMap.Height * 2 - 2
                    );
        }

        public void Draw(GraphicsDevice device, SurfaceFollowCamera camera)
        {
            // World Matrix
            effect.Projection = camera.projectionMatrix;
            effect.View = camera.viewMatrix;
            effect.World = Matrix.Identity;


            // Set Effect to draw
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vBuffer);
            device.Indices = iBuffer;
            for (int i = 0; i < heightMap.Height; i++)
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,
                    0,
                    i * (2 * heightMap.Height),
                    heightMap.Height * 2 - 2
                    );
        }

        public float GetHeight(float x, float z) 
        {
            //Vertex Calculation
            /*
             * a = upper left vertex
             * b = upper right vertex
             * c = lower left vertex
             * d = lower right vertex
             */ 
            int a, b, c, d;

            a = (int)z * heightMap.Height + (int)x;
            b = (int)z * heightMap.Height + (int)x + 1;
            c =  (int)(z + 1) * heightMap.Height + (int)x;
            d = (int)(z + 1) * heightMap.Height + (int)x + 1;

            //Distance diference calculation
            float da, db, dab, dcd, y, yab, ycd;

            da = x - (int)x;
            db = (int)(x + 1) - x;
            yab = heightMapData[a] * db + heightMapData[b] * da;
            ycd = heightMapData[c] * db + heightMapData[d] * da;

            dab = z - (int)z;
            dcd = (int)(z + 1) - z;
            y = yab * dcd + ycd * dab;

            //Return weighted average
            return y;

        }
    }
}