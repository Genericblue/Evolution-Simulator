using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EvoSim2
{
    class Map
    {
        public Tile[,] tiles;
        public List<Creature> creatures;
        public int scale;

        public Map(int nscale)
        {
            scale = nscale;
            Random r = new Random();
            tiles = new Tile[scale, scale];
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    tiles[i, j] = new Tile(i,j,r);
                }
            }
            creatures = new List<Creature>();
            for (int i = 0; i < 100; i++)
            {
                creatures.Add(new Creature(r.Next() % 1000, r.Next() % 1000, 50f, r));
            }
        }

        public void Collider()
        {
            /*
                if (creature.PX < 0)
                    creature.PX = 0;
                if (creature.PX >= scale * 100)
                    creature.PX = scale * 100-1;
                if (creature.PY < 0)
                    creature.PY = 0;
                if (creature.PY >= scale*100)
                    creature.PY = scale*100-1;
                 * */
            for(int i = 0; i < creatures.Count; i++)
            {
                for (int j = i+1; j < creatures.Count; j++)
                {
                    /*
                    if (creatures[i] == creatures[j])
                        continue;
                                         *                      * */
                    if (creatures[i].PX + (creatures[i].Size / 2) + (creatures[j].Size / 2) > creatures[j].PX
                        && creatures[i].PX < creatures[j].PX + (creatures[i].Size / 2) + (creatures[j].Size / 2)
                        && creatures[i].PY + (creatures[i].Size / 2) + (creatures[j].Size / 2) > creatures[j].PY
                        && creatures[i].PY < creatures[j].PY + (creatures[i].Size / 2) + (creatures[j].Size / 2))
                    {
                        if (Distance(creatures[i].PX, creatures[j].PX, creatures[i].PY, creatures[j].PY) < (creatures[i].Size + creatures[j].Size) / 2)
                        {
                            creatures[i].touch = 1;
                            creatures[j].touch = 1;
                            doCollide(creatures[i],creatures[j]);
                        }
                    }
                }  
            }
        }
        public void doCollide(Creature firstBall, Creature secondBall)
        {
            /*
             * newVelX1 = (firstBall.speed.x * (firstBall.mass – secondBall.mass) + (2 * secondBall.mass * secondBall.speed.x)) / (firstBall.mass + secondBall.mass);
                newVelY1 = (firstBall.speed.y * (firstBall.mass – secondBall.mass) + (2 * secondBall.mass * secondBall.speed.y)) / (firstBall.mass + secondBall.mass);
                newVelX2 = (secondBall.speed.x * (secondBall.mass – firstBall.mass) + (2 * firstBall.mass * firstBall.speed.x)) / (firstBall.mass + secondBall.mass);
                newVelY2 = (secondBall.speed.y * (secondBall.mass – firstBall.mass) + (2 * firstBall.mass * firstBall.speed.y)) / (firstBall.mass + secondBall.mass);
             * */

            float newAngle = MathHelper.ToDegrees((float)Math.Atan2((firstBall.PY - secondBall.PY), (firstBall.PX - secondBall.PX)));
            firstBall.PX = secondBall.PX + ((firstBall.Size + secondBall.Size) / 2 * (float)Math.Cos((newAngle) * Math.PI / 180));
            firstBall.PY = secondBall.PY + ((firstBall.Size + secondBall.Size) / 2 * (float)Math.Sin((newAngle) * Math.PI / 180));

            float newVelX1 = (firstBall.vX * (firstBall.Size - secondBall.Size) + (2 * secondBall.Size * secondBall.vX)) / (firstBall.Size + secondBall.Size);
            float newVelY1 = (firstBall.vY * (firstBall.Size - secondBall.Size) + (2 * secondBall.Size * secondBall.vY)) / (firstBall.Size + secondBall.Size);
            float newVelX2 = (secondBall.vX * (secondBall.Size - firstBall.Size) + (2 * firstBall.Size * firstBall.vX)) / (firstBall.Size + secondBall.Size);
            float newVelY2 = (secondBall.vY * (secondBall.Size - firstBall.Size) + (2 * firstBall.Size * firstBall.vY)) / (firstBall.Size + secondBall.Size);
            //firstBall.PX += newVelX1;
            //firstBall.PY += newVelY1;
            //secondBall.PX += newVelX2;
            //secondBall.PY += newVelY2;
            firstBall.vX = newVelX1;
            firstBall.vY = newVelY1;
            secondBall.vX = newVelX2;
            secondBall.vY = newVelY2;

            
        }

        private float Distance(float x1, float x2, float y1, float y2)
        {
            return (float)Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
            //return (float)(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public void Update()
        {
            Random r = new Random();
            for (int i = 0; i < creatures.Count; i++)
            {
                if (creatures[i].energy < 0)
                {
                    creatures.Remove(creatures[i]);
                    if (creatures.Count < 25)
                    {

                        creatures.Add(new Creature(r.Next() % 1000, r.Next() % 1000, 50f, r));
                    }
                }
                else
                {
                    creatures[i].tile = tiles[(int)(creatures[i].PX / (scale * 10)), (int)(creatures[i].PY / (scale * 10))];
                    try
                    {
                        creatures[i].tile2 = tiles[(int)(creatures[i].PX + (150 * (float)Math.Cos((creatures[i].angle + 20) * Math.PI / 180))) / (scale * 10), (int)(creatures[i].PY + (150 * (float)Math.Sin((creatures[i].angle + 20) * Math.PI / 180))) / (scale * 10)];
                    }
                    catch
                    {
                        creatures[i].tile2 = creatures[i].tile;
                    }
                    try
                    {
                        creatures[i].tile3 = tiles[(int)(creatures[i].PX + (150 * (float)Math.Cos((creatures[i].angle - 20) * Math.PI / 180))) / (scale * 10), (int)(creatures[i].PY + (150 * (float)Math.Sin((creatures[i].angle - 20) * Math.PI / 180))) / (scale * 10)];
                    }
                    catch
                    {
                        creatures[i].tile3 = creatures[i].tile;
                    }
                    if (creatures[i].birth == true)
                    {
                        creatures[i].energy -= 50;
                        creatures.Add(new Creature(creatures[i].PX + (150 * (float)Math.Cos(creatures[i].angle * Math.PI / 180)), creatures[i].PY + (150 * (float)Math.Sin(creatures[i].angle * Math.PI / 180)), 50f, r, creatures[i]));
                    }
                    creatures[i].Update();
                }
            }
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    tiles[i, j].Update();
                }
            }
            Collider();
        }
    }
}
