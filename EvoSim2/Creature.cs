using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EvoSim2
{
    class Creature
    {
        private float pX;
        public float PX
        {
            get { return pX; }
            set
            {
                pX = value;
                
                if (pX < 0)
                    pX = 0;
                if (pX >= 1000)
                    pX = 999;
                 
            }
        }
        private float pY;
        public float PY
        {
            get { return pY; }
            set
            {
                pY = value;
                
                if (pY < 0)
                    pY = 0;
                if (pY >= 1000)
                    pY = 999;
                 
            }
        }
        public float vX;
        public float vY;
        public float angle;
        public int hue;
        public int skinhue;
        public float energy;
        public Tile tile;
        public Tile tile2;
        public Tile tile3;
        public bool birth;
        public bool justborn;
        public int touch;
        public int touchhue;
        public int Size
        {
            get
            {
                if (energy > 125)
                {
                    return 150;
                }
                if(energy < 0)
                {
                    return 0;
                }
                else
                {
                    return (int)energy + 25;
                }
            }
        }
        public int noSkinSize
        {
            get
            {
                if (energy > 125)
                {
                    return 140;
                }
                if (energy < 0)
                {
                    return 0;
                }
                else
                {
                    return (int)energy + 15;
                }
            }
        }

        
        public Creature(int x, int y, float e, Random r)
        {
            pX = x;
            pY = y;
            angle = r.Next() % 360;
            hue = r.Next() % 360;
            skinhue = r.Next() % 360;
            energy = e;
            birth = false;
            justborn = false;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 1; j < 12; j++)
                {
                    hidden[i, j] = ((r.Next() % 21) - 10)/2;
                    outputs[i, j] = ((r.Next() % 21) - 10)/2;
                }
            }
        }

        public Creature(float x, float y, float e, Random r, Creature parent)
        {
            PX = x;
            PY = y;
            angle = r.Next() % 360;
            skinhue = r.Next() % 360;
            energy = e;
            birth = false;
            justborn = true;
            hue = parent.hue + (r.Next()%21) - 10;

            for (int i = 0; i < 11; i++)
            {
                for (int j = 1; j < 12; j++)
                {
                    if(r.Next() % 10 == 0)
                        hidden[i, j] = ((r.Next() % 21) - 10);
                    else
                        hidden[i, j] = parent.hidden[i,j];

                    if (r.Next() % 10 == 0)
                        outputs[i, j] = ((r.Next() % 21) - 10);
                    else
                        outputs[i, j] = parent.outputs[i, j];
                }
            }
        }

        public void goForward(float amount)
        {
            amount = Math.Abs(amount);
            energy -= .01f * amount;//lose 1% of the amount you try and move in energy
            vX += amount * (float)Math.Cos(angle * Math.PI / 180);
            vY += amount * (float)Math.Sin(angle * Math.PI / 180);
        }

        public void doEat(float amount)
        {
            amount *= 10;
            energy += normalize(0,180,1,-1f,(colorTest(hue,tile.hue))) * tile.reduceEnergy(amount);//try and eat 5 times amount in energy, lose some based on hue difference, +1 to avoid divide by zero.
            energy -= .1f * amount;//you lose a minimum at all times
        }

        public int colorTest(int hue1, int hue2)
        {
            int dist1, dist2;
            dist1 = hue1 - hue2;
            dist2 = hue2 - hue1;
            if (dist1 < 0)
                dist1 = (hue1) + (360 - hue2);
            else if(dist2 < 0)
                dist2 = (hue2) + (360 - hue1);
            if (dist1 < dist2)
                return dist1;
            else
                return dist2;
        }

        public void Update()
        {
                energy -= energy * .00001f +.005f;//.01% of its energy per frame, but also .005 so that they dont just stay at zero.
                PX += vX;
                PY += vY;
                vX *= .9f;
                vY *= .9f;
                if (justborn == false)
                {
                    UpdateNetwork();
                }
                justborn = false;
                touch = 0;
        }

        public float[] inputs = new float[11];
        public float[,] hidden = new float[11, 12];
        public float[,] outputs = new float[11, 12];

        private void UpdateNetwork()
        {
            inputs[0] = .5f;
            inputs[1] = normalize(15, 140, 0, 1, noSkinSize);
            inputs[2] = normalize(0, 360, 0, 1, tile.hue);
            inputs[3] = normalize(0, 1, 0, 1, tile.sat);
            inputs[4] = normalize(0, 360, 0, 1, tile2.hue);
            inputs[5] = normalize(0, 1, 0, 1, tile2.sat);
            inputs[6] = normalize(0, 360, 0, 1, tile3.hue);
            inputs[7] = normalize(0, 1, 0, 1, tile3.sat);
            inputs[8] = .5f;//touch;
            inputs[9] = Math.Abs(outputs[9,0]);// normalize(0, 1000, 0, 100, outputs[9, 0]);
            inputs[10] = 1f;

            for (int i = 0; i < 10; i++)
            {
                hidden[i,0] = 0;
                for (int j = 1; j < 12; j++)
                {
                    hidden[i, 0] += inputs[j - 1] * hidden[i, j];
                }
                hidden[i, 0] = normalize(-110, 110, -1, 1, hidden[i, 0]);
            }
            hidden[10, 0] = 1f;

            for (int i = 0; i < 11; i++)
            {
                outputs[i, 0] = 0;
                for (int j = 1; j < 12; j++)
                {
                    outputs[i, 0] += hidden[j - 1,0] * outputs[i, j];
                }
                outputs[i, 0] = normalize(-20, 20, -1, 1, outputs[i, 0]);
            }

            goForward(outputs[1, 0]);
            angle += outputs[2, 0];
            skinhue = (int)normalize(-1, 1, 0, 360, outputs[4, 0]);
            if (outputs[3, 0] >= 0)
            {
                //skinhue = 240 - 239 * (int)outputs[3, 0];
                doEat(outputs[3, 0]);
            }
            if (outputs[5, 0] >= 0)
            {
                //skinhue = 180;
                if (Size >= 150)
                    birth = true;
                else
                    birth = false;
            }
            else
                birth = false;
            
        }
        private float normalize(float pmin, float pmax, float nmin, float nmax, float num)
        {
            if (num > pmax)
                num = pmax;
           return (((num - pmin) * (nmax - nmin)) / (pmax - pmin)) + nmin;
        }

        public float Sigmoid(float x)
        {
            return (float)(2 / (1 + Math.Exp(-2 * x)) - 1);
        }
        public int posOrNeg(float number)
        {
            if (number > 0)
                return 1;
            return 0;
        }


        /*
Inputs:

Const
Energy
0Hue
0Sat
1H
1S
2H
2S
Temp
Mem
Touch

Outputs:

Const
Accelerate
Turn
Eat
Skin Hue
Birth
Fight
Sleep
Nothing1
Nothing2
Mem

*/

    }
}
