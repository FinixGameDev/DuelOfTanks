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
        VertexPositionColor[] debugVertexList;
        BasicEffect debugEffect;

        BasicEffect effect;
        VertexBuffer vBuffer;
        IndexBuffer iBuffer;
        float[] heightMapData;
        Vector3[] normalData;
        public Texture2D heightMap;

        public Map(Texture2D heightMap, Texture2D texture,GraphicsDevice device)
        {

            this.heightMap = heightMap;

            // Vamos usar um efeito básico
            debugEffect = new BasicEffect(device);
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
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            // Cria os eixos 3D
            CreateGeometry(device);
        }

        private void CreateGeometry(GraphicsDevice device)
        {
            //Light normals array
            normalData = new Vector3[heightMap.Width * heightMap.Height];

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

            debugVertexList = new VertexPositionColor[vertexCount * 2];

            for (int z = 0; z < heightMap.Height; z++)
                for (int x = 0; x < heightMap.Width; x++)
                {
                    Vector3[] n;
                    Vector3 addNormal;

                    int i;
                    i = z * heightMap.Width + x;

                    float h, u, v;
                    h = heightMapData[i];
                    u = x % 2;
                    v = z % 2;


                    //Normal Vector calculation
                    if (i == 0) //Upper Left Corner
                    {
                        n = new Vector3[3];

                        n[0] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down
                        n[1] = new Vector3(1f, heightMapData[i + heightMap.Width + 1] - h, 1f); //Lower Right
                        n[2] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2])) / 2;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if (i == heightMap.Width - 1) //Upper Right Corner
                    {
                        n = new Vector3[3];

                        n[0] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); //Left
                        n[1] = new Vector3(-1f, heightMapData[i + heightMap.Width - 1] - h, 1f); // Lower Left
                        n[2] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2])) / 2;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if (i == (heightMap.Height - 1) * heightMap.Width) //Lower Left Corner
                    {
                        n = new Vector3[3];

                        n[0] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right
                        n[1] = new Vector3(1f, heightMapData[i - heightMap.Width + 1] - h, -1f); //Upper Right
                        n[2] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2])) / 2;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if (i == (heightMap.Height - 1) * heightMap.Width + heightMap.Width - 1) // Lower Right Corner
                    {
                        n = new Vector3[3];

                        n[0] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up
                        n[1] = new Vector3(-1f, heightMapData[i - heightMap.Width - 1] - h, -1f); // Upper Left 
                        n[2] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); // Left

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2])) / 2;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if (i % heightMap.Height == 0) // Left Border   
                    {
                        n = new Vector3[5];

                        n[0] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down
                        n[1] = new Vector3(1f, heightMapData[i + heightMap.Width + 1] - h, 1f); //Lower Right
                        n[2] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right
                        n[3] = new Vector3(1f, heightMapData[i - heightMap.Width + 1] - h, -1f); //Upper Right
                        n[4] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2]) + Vector3.Cross(n[2], n[3]) + Vector3.Cross(n[3], n[4])) / 4;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if (i % heightMap.Height == heightMap.Width - 1) // Right Border
                    {
                        n = new Vector3[5];

                        n[0] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up
                        n[1] = new Vector3(-1f, heightMapData[i - heightMap.Width - 1] - h, -1f); // Upper Left 
                        n[2] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); // Left
                        n[3] = new Vector3(-1f, heightMapData[i + heightMap.Width - 1] - h, 1f); // Lower Left
                        n[4] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2]) + Vector3.Cross(n[2], n[3]) + Vector3.Cross(n[3], n[4])) / 4;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if ( i < heightMap.Width) // Up Border
                    {
                        n = new Vector3[5];

                        n[0] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); // Left
                        n[1] = new Vector3(-1f, heightMapData[i + heightMap.Width - 1] - h, 1f); // Lower Left
                        n[2] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down
                        n[3] = new Vector3(1f, heightMapData[i + heightMap.Width + 1] - h, 1f); //Lower Right
                        n[4] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2]) + Vector3.Cross(n[2], n[3]) + Vector3.Cross(n[3], n[4])) / 4;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else if ((i > (heightMap.Height - 1) * heightMap.Width)) // Down Border
                    {
                        n = new Vector3[5];

                        n[0] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right
                        n[1] = new Vector3(1f, heightMapData[i - heightMap.Width + 1] - h, -1f); //Upper Right
                        n[2] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up
                        n[3] = new Vector3(-1f, heightMapData[i - heightMap.Width - 1] - h, -1f); // Upper Left 
                        n[4] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); // Left

                        addNormal = (Vector3.Cross(n[0], n[1]) + Vector3.Cross(n[1], n[2]) + Vector3.Cross(n[2], n[3]) + Vector3.Cross(n[3], n[4])) / 4;
                        addNormal.Normalize();

                        normalData[i] = addNormal;
                    }
                    else // Nespresso what ELSE
                    {
                        n = new Vector3[8];

                        n[0] = new Vector3(0f, heightMapData[i - heightMap.Width] - h, -1f); //Up
                        n[1] = new Vector3(-1f, heightMapData[i - heightMap.Width - 1] - h, -1f); // Upper Left 
                        n[2] = new Vector3(-1f, heightMapData[i - 1] - h, 0f); // Left
                        n[3] = new Vector3(-1f, heightMapData[i + heightMap.Width - 1] - h, 1f); // Lower Left
                        n[4] = new Vector3(0f, heightMapData[i + heightMap.Width] - h, 1f); //Down
                        n[5] = new Vector3(1f, heightMapData[i + heightMap.Width + 1] - h, 1f); //Lower Right
                        n[6] = new Vector3(1f, heightMapData[i + 1] - h, 0f); //Right
                        n[7] = new Vector3(1f, heightMapData[i - heightMap.Width + 1] - h, -1f); //Upper Right
                        

                        for (int j = 0; j < 8; j++)
			            {
                            if (j == 7)
	                        {
                                addNormal = Vector3.Cross(n[7], n[0]);
                                addNormal.Normalize();

                                normalData[i] += addNormal;
	                        }
                            else
	                        {
                                addNormal = Vector3.Cross(n[j], n[j + 1]);
                                addNormal.Normalize();

                                normalData[i] += addNormal;
	                        }
			            }
                       
                        normalData[i] = Vector3.Divide(normalData[i], 8);

                    }

                    vertices[i] = new VertexPositionNormalTexture(
                        new Vector3(x, h, z), 
                        normalData[i], 
                        new Vector2(u, v));

                    //Debug Normal
                    debugVertexList[2 * i] = new VertexPositionColor(new Vector3(x, h, z), Color.Red);
                    debugVertexList[2 * i + 1] = new VertexPositionColor(new Vector3(x, h, z) + normalData[i], Color.Cyan);

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
            effect.LightingEnabled = true;
            effect.EnableDefaultLighting();

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
            effect.LightingEnabled = true;
            effect.EnableDefaultLighting();

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

        public void DebugDraw(GraphicsDevice device, SurfaceFollowCamera camera)
        {
            //Debug Normals
            debugEffect.Projection = camera.projectionMatrix;
            debugEffect.View = camera.viewMatrix;
            debugEffect.World = Matrix.Identity;
            debugEffect.LightingEnabled = false;
            debugEffect.VertexColorEnabled = true;
            debugEffect.CurrentTechnique.Passes[0].Apply();

            for (int i = 0; i < vBuffer.VertexCount; i++)
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, debugVertexList, 2*i, 1);
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
            b = a + 1;
            c = a + heightMap.Height;
            d = c + 1;

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