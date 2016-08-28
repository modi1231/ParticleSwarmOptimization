// 2016.08.28 - modi123_1 @ dreamincode.net
using System;
using System.Collections.Generic;


namespace ParticleSwarmOptimization
{
    #region This_Problem_Explained
    /*
       Find the five values that add up to 72.
       x1 + x2 + x3 + x4 + x5 = 72
     */
    #endregion
    #region PSO_Rules
    /*
     1. Evaluate fitness of each particle
     2. Update each individual and 'global best' fitness and their position.
     3. Update each particle's velocity and position.                
     */
    #endregion
    #region Equation_Explained
    /* V[t+1]=w∗V[t]+c1∗Rand1()∗(pb[t]-X[t])+c2∗Rand2()∗(pg[t]-x[t])
         -------------
         V[t+1]: the updated velocity for the next time epoch
         w : inertia weight
         V[t] : current velocity
         c1 and c2: constants..  cognitive confidence coefficients .. 
          -- typically c1 == c2 == 2
          -- helps determine size of movement to the best.
         Rand1() and Rand2() : random numbers from 0 to 1
         pb[t] : local neighborhood best
         X[t] : current particle's position
         pg[t] :  global particle best
         x[t] : current particle position
         ---------------
         ---------------
        1.  w∗V[t] is inertia portion. inertia to keep particle going in same direction..
          -- lower w vals -> faster convergence.. 
          -- higher w vals -> more solution space looked at.. 
          -- typically .8 to 1.2
        2.  c1∗Rand1()∗(pb[t]-X[t]) is the cognitive component. 
          -- Since tracking 'local neighborhood best' try and keep where the particle's been
        3.  c2∗Rand2()∗(pg[t]-x[t]) is the social part
          -- Wants to head towards best area found over all. Hang out with the cool kids.
     */
    #endregion

    class PSO
    {
        private const int _lGoal = 72;
        private const int _lMaxPoly = 2;

        private const int _lMaxParticles = 10;
        private const int _lVelocityMaximum = 10;

        private const int _lInitRangeMin = 40;
        private const int _lInitRangeMax = 110;

        private const int _lMaxEpoch = 100; //time cut off.

        private const int _lW = 1;

        private List<Particle> _myParticles = null;

        private Random _r;

        public PSO()
        {
            int tempTotal = 0;
            Particle tempPart = null;

            _r = new Random();

            _myParticles = new List<Particle>();

            // randomize the input and make the list of particles.
            for (int i = 0; i < _lMaxParticles; i++)
            {
                tempTotal = 0;
                tempPart = new Particle(_lMaxPoly);

                for (int z = 0; z < _lMaxPoly; z++)
                {
                    tempPart.Data[z] = _r.Next(_lInitRangeMin, _lInitRangeMax);
                    tempTotal += tempPart.Data[z];
                }
                tempPart.pGlobalBest = tempTotal;
                _myParticles.Add(tempPart);
            }
        }

        public void DoPSO()
        {
            bool bFound = false;
            int lEpoch = 0;
            int lBestIndex = 0;
            int lBestIndexTemp = 0;

            //found or time runs out.
            while ((!bFound) && (lEpoch < _lMaxEpoch))
            {
                //-- 1. Print out data so far.
                Console.WriteLine("Epoch: " + lEpoch);
                Console.WriteLine("-------");
                PrintParticles(true);

                //-- 2. check to see if a solution was found.
                for (int i = 0; i < _lMaxParticles; i++)
                {
                    if (GetResults(i) == _lGoal)
                    {
                        bFound = true;
                        Console.WriteLine("!!Found!! at index " + i.ToString());
                        PrintParticle(i);
                        break;
                    }
                }

                //-- 3.  If nothing is found then find new best, update velocities, and update particle values.
                if (!bFound)
                {
                    //-- 3.1.  get the new minimum.
                    lBestIndexTemp = GetMinimumIndex();

                    //-- 3.2.  compare the new minimum to the current best minimum.
                    if (Math.Abs(_lGoal - GetResults(lBestIndexTemp)) < Math.Abs(_lGoal - GetResults(lBestIndex)))
                    {
                        // in the new is better, update who the best is.
                        lBestIndex = lBestIndexTemp;
                    }

                    Console.WriteLine("Current Best: " + lBestIndex.ToString());

                    //-- 3.3.  Using the new global best, update everyone's velocities
                    // -- note, for simplicitiy's sake 'local neighborhood best' is not used.. just global.
                    SetNewVelocity(lBestIndex);

                    //-- 3.4.  Start updating all the particle's data to zero in on the goal.
                    UpdateParticleValues(lBestIndex);

                    //-- 3.5.  
                    lEpoch += 1;
                    Console.WriteLine(string.Empty);

                }
            }
        }

        //tweak the particle values by the velocity to be closer to a result.
        // if the current particle's poly matches the current best's poly at that index don't updated it.. 
        // this means other values can change and certain ones stay the same.
        private void UpdateParticleValues(int lBestIndex)
        {
            int lTempCurrentTotal = 0;

            for (int i = 0; i < _lMaxParticles; i++)
            {
                for (int z = 0; z < _lMaxPoly; z++)
                {
                    if (_myParticles[i].Data[z] != _myParticles[lBestIndex].Data[z])
                    {
                        _myParticles[i].Data[z] += _myParticles[i].Velocity;
                    }
                }

                lTempCurrentTotal = GetResults(i);

                if (Math.Abs(_lGoal - lTempCurrentTotal) < _myParticles[i].pGlobalBest)
                {
                    _myParticles[i].pGlobalBest = lTempCurrentTotal;
                }
            }
        }

        //Based on the PSO equation (see at the top) - determine all the particles the new velocity based on the best, but keep inside the constraints so it doesn't wander too far.
        //Note to keep this simple 'w' is 1 and is not a factor.
        private void SetNewVelocity(int lBestIndex)
        {
            int lCurrentResult = 0;
            int lBestResult = 0;
            int lTempNewVelocity = 0;

            lBestResult = GetResults(lBestIndex);

            for (int i = 0; i < _lMaxParticles; i++)
            {
                lCurrentResult = GetResults(i);
                lTempNewVelocity = (int)(_lW * _myParticles[i].Velocity + // inertia
                    2 * _r.NextDouble() * (_myParticles[i].pGlobalBest - lCurrentResult) + //cognative
                    2 * _r.NextDouble() * (lBestResult - lCurrentResult)); // social

                // keep the velocity constrained so it is not going wild.
                if (lTempNewVelocity > _lVelocityMaximum)
                    _myParticles[i].Velocity = _lVelocityMaximum;
                else if (lTempNewVelocity < -_lVelocityMaximum)
                    _myParticles[i].Velocity = -_lVelocityMaximum;
                else
                    _myParticles[i].Velocity = lTempNewVelocity;
            }
        }

        // unique to finding this solution because I want to narrow in from large numbers to the closest matching our goal
        // In this stripped down example I am treating local neighborhood best and global best as the same thing.
        // This would be considered 'global best'.  You could implement a 'neighborhood best' in a different function.
        private int GetMinimumIndex()
        {
            int lLowestIndex = 0;

            //examine all the particles and find the one now closest to the goal.
            for (int i = 0; i < _lMaxParticles; i++)
            {
                if (Math.Abs(_lGoal - GetResults(i)) < Math.Abs(_lGoal - GetResults(lLowestIndex)))
                {
                    lLowestIndex = i;
                }
            }

            return lLowestIndex;
        }


        // Get the current 'fitness' based on the equation we are trying to ge
        private int GetResults(int index)
        {
            int lResults = 0;

            for (int i = 0; i < _lMaxPoly; i++)
            {
                lResults += _myParticles[index].Data[i];
            }

            return lResults;
        }

        private void PrintParticles(bool lineNumbers)
        {
            string sTemp = string.Empty;

            for (int i = 0; i < _lMaxParticles; i++)
            {
                sTemp = string.Empty;

                if (lineNumbers)
                        sTemp +=  i.ToString().PadLeft(3) + ": ";

                for (int z = 0; z < _lMaxPoly; z++)
                {
                    sTemp += _myParticles[i].Data[z].ToString().PadLeft(3);
                    if (z != _lMaxPoly - 1)
                        sTemp += " + ";
                }

                sTemp += " = " + GetResults(i).ToString().PadLeft(3);
                sTemp += " (v:" + _myParticles[i].Velocity.ToString().PadLeft(3) + ")";
                Console.WriteLine(sTemp);
            }
        }

        private void PrintParticle(int index)
        {
            string sTemp = string.Empty;

                 sTemp = string.Empty;
                for (int z = 0; z < _lMaxPoly; z++)
                {
                    sTemp += _myParticles[index].Data[z].ToString();
                    if (z != _lMaxPoly - 1)
                        sTemp += " + ";
                }

                sTemp += " = " + GetResults(index);
                Console.WriteLine(sTemp);
         }

    }

    class Particle
    {
        public int pGlobalBest { get; set; }// best global position
        public int Velocity { get; set; }
        public int[] Data { get; set; }


        // in this case treating global best as local.
        //public int pBestLocal { get; set; }//best local position
        public Particle()
        {
            pGlobalBest = 0;
            Velocity = 0;
        }

        public Particle(int maxPoly)
        {
            Data = new int[maxPoly];
            pGlobalBest = 0;
            Velocity = 0;
        }
    }
}
