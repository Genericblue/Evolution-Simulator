using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvoSim2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D sprite;
        Texture2D t;
        Map map;
        KeyboardState kbs = new KeyboardState();
        int scale;
        GameTime pGameTime;
        private SpriteFont font;
        int currentCreature;
        bool update;
        int tenergy;
        int cenergy;

        public Game1()
        {
            //test
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1500;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1000;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            //graphics.ToggleFullScreen();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scale = 10;
            map = new Map(scale);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.Black });
            font = Content.Load<SpriteFont>("specs");
            currentCreature = 0;
            kbs = Keyboard.GetState();
            //sprite = Content.Load<Texture2D>("Tile");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                map.creatures[0].pX += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                map.creatures[0].pX -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                map.creatures[0].pY += 1f;
            }
             * */
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                map.creatures[currentCreature].goForward(1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                map.creatures[currentCreature].angle += 5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                map.creatures[currentCreature].angle -= 5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                map.creatures[currentCreature].doEat(.1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && (kbs.IsKeyUp(Keys.Left)))
            {
                currentCreature--;
                if (currentCreature < 0)
                    currentCreature = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && (kbs.IsKeyUp(Keys.Right)))
            {
                currentCreature++;
                if (currentCreature > map.creatures.Count-1)
                    currentCreature = map.creatures.Count-1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && (kbs.IsKeyUp(Keys.Space)))
            {
                update = !update;
            }

            if(update)
                map.Update();
            //map.creatures[0].Update();
            // TODO: Add your update logic here
            kbs = Keyboard.GetState();
            if (currentCreature > map.creatures.Count - 1)
                currentCreature = map.creatures.Count - 1;
            pGameTime = gameTime;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            Color mColor = Color.White;
            int r, g, b;
            sprite = Content.Load<Texture2D>("Tile");
            tenergy = 0;
            cenergy = 0;
 
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    tenergy += (int)map.tiles[i, j].energy;
                    HsvToRgb(map.tiles[i, j].hue, map.tiles[i, j].sat, .5, out r, out g, out b);
                    mColor.R = Convert.ToByte(r);
                    mColor.G = Convert.ToByte(g);
                    mColor.B = Convert.ToByte(b);

                    spriteBatch.Draw(sprite, new Rectangle(map.tiles[i, j].x * scale * 10, map.tiles[i, j].y * scale * 10, scale * 10, scale * 10), mColor);
                    spriteBatch.DrawString(font, (int)map.tiles[i, j].energy + "%", new Vector2(map.tiles[i, j].x * scale * 10 + (scale * 4), map.tiles[i, j].y * scale * 10 + (scale * 4)), Color.White);
                }
            }

            sprite = Content.Load<Texture2D>("Node");
            foreach (Creature creature in map.creatures)
            {
                cenergy += (int)creature.energy;
                if (creature == map.creatures[currentCreature])
                {
                    creature.skinhue = 300;
                    spriteBatch.DrawString(font, "Creature: " + currentCreature + "/" + map.creatures.Count, new Vector2(1150, 100), Color.White);
                    for (int i = 0; i < 11; i++)
                    {
                        spriteBatch.DrawString(font, creature.inputs[i].ToString(), new Vector2(1050, 150 + 50 * i), Color.White);
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        spriteBatch.DrawString(font, creature.hidden[i,0].ToString(), new Vector2(1150, 150 + 50 * i), Color.White);
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        switch (i)
                        {
                            case 1:
                                spriteBatch.DrawString(font, "Accel", new Vector2(1350, 150 + 50 * i), Color.White);
                                spriteBatch.DrawString(font, (creature.outputs[i, 0]).ToString(), new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(font, "Turn", new Vector2(1350, 150 + 50 * i), Color.White);
                                spriteBatch.DrawString(font, (creature.outputs[i, 0]).ToString(), new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(font, "Eat", new Vector2(1350, 150 + 50 * i), Color.White);
                                spriteBatch.DrawString(font, (creature.outputs[i, 0]).ToString(), new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(font, "Skin Hue", new Vector2(1350, 150 + 50 * i), Color.White);
                                spriteBatch.DrawString(font, (creature.outputs[i, 0]).ToString(), new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                            case 5:
                                spriteBatch.DrawString(font, "Birth", new Vector2(1350, 150 + 50 * i), Color.White);
                                if (creature.outputs[i, 0] > 0)
                                    spriteBatch.DrawString(font, "YES", new Vector2(1250, 150 + 50 * i), Color.White);
                                else
                                    spriteBatch.DrawString(font, "NO", new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                            case 9:
                                spriteBatch.DrawString(font, "Mem", new Vector2(1350, 150 + 50 * i), Color.White);
                                spriteBatch.DrawString(font, (creature.outputs[i, 0]).ToString(), new Vector2(1250, 150 + 50 * i), Color.White);
                                break;
                        }
                    }
                }
                HsvToRgb(creature.skinhue, 1, .75, out r, out g, out b);
                mColor.R = Convert.ToByte(r);
                mColor.G = Convert.ToByte(g);
                mColor.B = Convert.ToByte(b);
                spriteBatch.Draw(sprite, new Rectangle((int)creature.PX - creature.Size / 2, (int)creature.PY - creature.Size / 2, creature.Size, creature.Size), mColor);
                HsvToRgb(creature.hue, 1, .75, out r, out g, out b);
                mColor.R = Convert.ToByte(r);
                mColor.G = Convert.ToByte(g);
                mColor.B = Convert.ToByte(b);
                spriteBatch.Draw(sprite, new Rectangle((int)creature.PX - creature.noSkinSize / 2, (int)creature.PY - creature.noSkinSize / 2, creature.noSkinSize, creature.noSkinSize), mColor);
                DrawLine(spriteBatch, //draw line
                new Vector2(creature.PX, creature.PY), //start of line
                new Vector2(creature.PX + (150 * (float)Math.Cos((creature.angle+20) * Math.PI / 180)), creature.PY + (150 * (float)Math.Sin((creature.angle+20) * Math.PI / 180))) //end of line
                );
                DrawLine(spriteBatch, //draw line
                new Vector2(creature.PX, creature.PY), //start of line
                new Vector2(creature.PX + (150 * (float)Math.Cos((creature.angle-20) * Math.PI / 180)), creature.PY + (150 * (float)Math.Sin((creature.angle-20) * Math.PI / 180))) //end of line
                );
                
            }
            spriteBatch.DrawString(font, "Tile Energy: " + tenergy, new Vector2(1100, 25), Color.White);
            spriteBatch.DrawString(font, "Creature Energy: " + cenergy, new Vector2(1100, 50), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        /*
        public Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }
         * */
        /*
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
         * */
        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
