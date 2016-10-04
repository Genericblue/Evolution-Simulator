using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvoSim2
{
    class Tile
    {
        public int x;
        public int y;
        public int hue;//type
        public float maxenergy;
        public float sat;//how much energy is left
        public float energy;
        public float reduceEnergy(float i)
        {
            /*
            if (i > energy)
            {
                i = energy;
                energy = 0;
                sat = energy / 100f;
                return i;
            }
             * */
            i = i * energy / 100;//take i percent of energy per frame.
            energy -= i;
            sat = energy/100f;
            return i;
        }
        public void Update()
        {
            energy += maxenergy * .001f;//gain .1% energy back per frame
            if (energy > maxenergy)
            {
                energy = maxenergy;
            }
            sat = energy / 100f;
        }
        public Tile(int i, int j, Random r)
        {
            x = i;
            y = j;
            //maxenergy = r.Next() % 50 + 50;
            //maxenergy = 100;
            maxenergy = (float)Math.Abs(i - 4.5) * 10 + (float)Math.Abs(j - 4.5) * 10;
            energy = maxenergy;
            sat = energy/100f;
            //hue = r.Next() % 360;
            hue = (i + j) * 18;
        }
    }
}
