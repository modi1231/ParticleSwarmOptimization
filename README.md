# ParticleSwarmOptimization
Intro to Particle Swarm Optimization in C# .

Full tutorial here: http://www.dreamincode.net/forums/topic/396550-intro-to-particle-swarm-optimization/

=================
dreamincode.net tutorial backup ahead of decommissioning

 Posted 28 August 2016 - 03:41 PM 


[i]Tools: Visual Studio 2015, .NET 4.5.2, console project[/i]

[i]Note: This is a high level over view and shouldn't be taken as the end-all, be-all gospel on PSO.[/i]

[u][b]Introduction[/b][/u]

[url="https://en.wikipedia.org/wiki/Particle_swarm_optimization"]Particle Swarm Optimization[/url] (PSO) is a novel way to optimize a problem over a series of steps to find a solution to a measurable problem.  That is to say you have a solution goal, a given set of unknown blocks that make up that goal, and you want to find the pieces that fit for that goal.

A set of 'particles' (widgets, objects, etc) are given values (position), and are converge (velocity) to a local and global best solution each iteration.

You will need to do a bit of outside-the-box thinking on this one.. A particle is one of many possible solutions objects you are trying to push to a goal.  The 'position' is just the data; be it two numbers, a picture, five variables, whatever.  It is what you will be modifying with the 'velocity'.  Velocity is just a fancy way of saying 'fitness' and helps you modify the particle's data to reach the goal.

Folks tend to use a flock of birds as an example.  You have a flock of birds and they are trying to find food.  The birds are the particles, and the food is the goal.

The birds do not know exactly where the food is, but they know how close they are (fitness).  This means each time slice a few things happen.
1.  All the birds check how close they are to the food
2.  The birds closest to each other (local neighborhood) say who is closest
3.  Globally the closest bird to the food yells out their position.
4.  All the birds also start moving (changing their data of x and y) towards the local and global best positions (velocity)
5.  Repeat until the food is found.

This is considered a 'global search [url="https://en.wikipedia.org/wiki/Metaheuristic"]metaheuristic[/url]', and keep in mind PSO is not guaranteeing an optimal solution.  It may certainly find a solution, but nothing says it is the 'best of all solutions'.  In the same vein as [url="https://en.wikipedia.org/wiki/Genetic_algorithm"]Genetic Algorithms[/url], but unlike Genetic Algorithms that operate with crossovers, mutations, etc PSO continuously tweak the data values they start with.  

[url="http://www.dreamincode.net/forums/topic/319577-intro-to-genetic-algorithms/"]VB.NET GA example[/url]


[u][b]General PSO Guidelines[/b][/u]
The three wiz-bangs that thought up PSO were Kennedy, Eberhart and Shi and they provided three core principles to structure your code by.

1. Evaluate fitness of each particle
2. Update each individual and 'global best' fitness and their position.
3. Update each particle's velocity and position.                

In relation to the bird example 
1.  Figure out each bird's closeness to food.
2.  Local neighborhood bird and global bird who are closest yell out their position
3.  Each bird tweaks their velocity towards the global and local birds and uses that to tweak their position (aka their data).


[u][b]The Velocity Equation[/b][/u]

This may not show up right, but here's the general velocity equation used:
V[t+1]=w∗V[t]+c1∗Rand1()∗(pb[t]-X[t])+c2∗Rand2()∗(pg[t]-x[t])

V[t+1]: the updated velocity for the next time epoch of this given particle
w : inertia weight
V[t] : current velocity
c1 and c2: Both are constants called "cognitive confidence coefficients", and typically c1 == c2 == 2.  They helps determine size of movement to the best position.

Rand1() and Rand2() : random numbers from 0 to 1
pb[t] : local neighborhood best
X[t] : current particle's position
pg[t] :  global particle best
x[t] : current particle position

The equation is seen as being three parts in one.
1.  w∗V[t] is inertia portion. inertia to keep particle going in same direction.  The lower the w vals the faster convergence to any solution, but the higher w vals the more solution space looked at.  As far as I have seen the values is typically 0.8 to 1.2 and doesn't change at all.

2.  c1∗Rand1()∗(pb[t]-X[t]) is the "cognitive component".  Since tracking 'local neighborhood best' try and keep to where the particle's been.

3.  c2∗Rand2()∗(pg[t]-x[t]) is the "social part".. The particles wants to head towards best area found over all. Hang out with the cool kids.


[u][b]The Problem[/b][/u]

Birds are great, but how does this apply to any problem you may want to solve.  With most metaheuristic half the battle is figuring out the encoding of the problem in some data structure, and tweaking 'fitness' to drive velocity to a goal.

When trying out a new search concept I am always a fan of trying to solve pesky [url="https://en.wikipedia.org/wiki/Polynomial"]polynomial[/url] equations.  You know x + 4 = 8 and you have to solve for x.  They can be made complex enough to have a large solution space, and are easy enough to verify the math is valid.

In this case I am looking to find the five values that add up to 72 using the equation of: x + y + z + a + b = 72

The values could be positive or negative, or even the same values.  All I am concerned is getting five values to add up to 72.

The values make up the particle's "position" in the solution space.  Less so the actual physics definition but more of a concept.

I could approach this problem in a few ways.  I could start out with a bunch of small numbers and gradually find their maximum until I hit the goal, or start out with a bunch of large numbers and find the smallest until I hit my goal.  Here I opted to go for starting off large and driving towards a minimum.

A few constants to keep the problem in line.. 
[list][*]This is a simpler problem so my number of particles will be low.. like ten. 
[*]Since the population is small I am going to treat the global and local neighborhood best as the same.
[*]I do not want each particle to go crazy on velocity so I cap the maximum at 10.  If a new velocity is too much, or too little, use 10.
[*]My only other constraint is to set the high random values - I have a range I want to start with before driving home to the goal.
[/list]


[u][b]Example Walk Through[/b][/u]
A quick time out for a walk through how this would work with a simpler format.

Say I want to find a solution to: x + y = 72.  Two unknown variables I want to drive towards a goal.

My constants:
goal: 72
max number of polynomials: 2
max number of particles: 10
inital min/max: 40, 110 (arbitrarily picked by me)
max time: 100

Epoch: 0

Here is my list of particles coming out of the gate.

This is read as particle's index: x + y = total (velocity applied to last epoch's values to get here)

[code]  0:  65 +  98 = 163 (v:  0)
  1:  71 +  76 = 147 (v:  0)
  2: 106 +  80 = 186 (v:  0)
  3: 101 +  91 = 192 (v:  0)
  4:  68 +  54 = 122 (v:  0)
  5:  63 +  80 = 143 (v:  0)
  6:  76 +  64 = 140 (v:  0)
  7: 100 +  44 = 144 (v:  0)
  8:  74 +  66 = 140 (v:  0)
  9: 102 + 109 = 211 (v:  0)[/code]

Remember I am looking for the best which, in this case is the minimum and that happens to be 4.  122 is the closest to the goal of 72.

No one has velocities yet as this is the initial run.

Epoch: 1 happens.  Here I apply the velocities to each variable assuming they are different from the best.
-------
[code]  0:  55 +  88 = 143 (v:-10)
  1:  61 +  66 = 127 (v:-10)
  2:  96 +  70 = 166 (v:-10)
  3:  98 +  88 = 186 (v: -3)
  4:  68 +  54 = 122 (v:  0)
  5:  53 +  70 = 123 (v:-10)
  6:  66 +  54 = 120 (v:-10)
  7:  99 +  43 = 142 (v: -1)
  8:  64 +  56 = 120 (v:-10)
  9:  92 +  99 = 191 (v:-10)[/code]


Take particle 0.
Epoch 0:
0:  65 +  98 = 163 (v:  0)
The best:
4:  68 +  54 = 122 (v:  0)

The code compares data values to the current best's value in the same index position.

is 65 == 68?  No so 65 minus velocity gets us the new value of 55.
is 98 == 54?  No so 98 minus velocity gets us the new value of 88.

This gives us the new particle 0 (with the velocity applied) for Epoch 1: 
0:  55 +  88 = 143 (v:-10)

In this case both values of particle's 0 data have been changed.

On the other hand - particle 6 only had one value changed.
Epoch 0
6:  76 +  64 = 140 (v:  0)
The best:
4:  68 +  54 = 122 (v:  0)

is 76 == 68?  No so 76 minus velocity gets us the new value of 66.
is 54 == 54?  Yes, so no change because that guy's on the right track.

This givs us the new particle 6 (with the velocity applied) for Epoch 1: 
6:  66 +  54 = 120 (v:-10)

This continues on for a few iterations until a best value is found.
[spoiler]
[code]Epoch: 2
-------
  0:  45 +  78 = 123 (v:-10)
  1:  51 +  56 = 107 (v:-10)
  2:  86 +  60 = 146 (v:-10)
  3:  88 +  78 = 166 (v:-10)
  4:  65 +  54 = 119 (v: -3)
  5:  43 +  60 = 103 (v:-10)
  6:  66 +  54 = 120 (v:-10)
  7:  89 +  33 = 122 (v:-10)
  8:  54 +  46 = 100 (v:-10)
  9:  82 +  89 = 171 (v:-10)
Current Best: 8

Epoch: 3
-------
  0:  35 +  68 = 103 (v:-10)
  1:  41 +  46 =  87 (v:-10)
  2:  76 +  50 = 126 (v:-10)
  3:  78 +  68 = 146 (v:-10)
  4:  55 +  44 =  99 (v:-10)
  5:  33 +  50 =  83 (v:-10)
  6:  56 +  44 = 100 (v:-10)
  7:  79 +  23 = 102 (v:-10)
  8:  54 +  46 = 100 (v:-10)
  9:  72 +  79 = 151 (v:-10)
Current Best: 5

Epoch: 4
-------
  0:  25 +  58 =  83 (v:-10)
  1:  31 +  36 =  67 (v:-10)
  2:  66 +  50 = 116 (v:-10)
  3:  68 +  58 = 126 (v:-10)
  4:  45 +  34 =  79 (v:-10)
  5:  33 +  50 =  83 (v:-10)
  6:  46 +  34 =  80 (v:-10)
  7:  69 +  13 =  82 (v:-10)
  8:  44 +  36 =  80 (v:-10)
  9:  62 +  69 = 131 (v:-10)
Current Best: 1

Epoch: 5
-------
  0:  15 +  48 =  63 (v:-10)
  1:  31 +  36 =  67 (v:-10)
  2:  56 +  40 =  96 (v:-10)
  3:  58 +  48 = 106 (v:-10)
  4:  35 +  24 =  59 (v:-10)
  5:  23 +  40 =  63 (v:-10)
  6:  36 +  24 =  60 (v:-10)
  7:  59 +   3 =  62 (v:-10)
  8:  34 +  36 =  70 (v:-10)
  9:  52 +  59 = 111 (v:-10)
Current Best: 8

Epoch: 6
-------
  0:  16 +  49 =  65 (v:  1)
  1:  25 +  36 =  61 (v: -6)
  2:  46 +  30 =  76 (v:-10)
  3:  48 +  38 =  86 (v:-10)
  4:  32 +  21 =  53 (v: -3)
  5:  24 +  41 =  65 (v:  1)
  6:  29 +  17 =  46 (v: -7)
  7:  60 +   4 =  64 (v:  1)
  8:  34 +  36 =  70 (v:-10)
  9:  42 +  49 =  91 (v:-10)
Current Best: 8

Epoch: 7
-------
  0:  20 +  53 =  73 (v:  4)
  1:  26 +  36 =  62 (v:  1)
  2:  36 +  20 =  56 (v:-10)
  3:  38 +  28 =  66 (v:-10)
  4:  42 +  31 =  73 (v: 10)
  5:  34 +  51 =  85 (v: 10)
  6:  37 +  25 =  62 (v:  8)
  7:  68 +  12 =  80 (v:  8)
  8:  34 +  36 =  70 (v:-10)
  9:  32 +  39 =  71 (v:-10)
Current Best: 0

Epoch: 8
-------
  0:  20 +  53 =  73 (v:  4)
  1:  36 +  46 =  82 (v: 10)
  2:  45 +  29 =  74 (v:  9)
  3:  39 +  29 =  68 (v:  1)
  4:  52 +  41 =  93 (v: 10)
  5:  24 +  41 =  65 (v:-10)
  6:  46 +  34 =  80 (v:  9)
  7:  74 +  18 =  92 (v:  6)
  8:  27 +  29 =  56 (v: -7)
  9:  25 +  32 =  57 (v: -7)
Current Best: 0[/code]
[/spoiler]
Epoch: 9
-------
[code]  0:  20 +  53 =  73 (v:  4)
  1:  38 +  48 =  86 (v:  2)
  2:  53 +  37 =  90 (v:  8)
  3:  48 +  38 =  86 (v:  9)
  4:  42 +  31 =  73 (v:-10)
  5:  16 +  33 =  49 (v: -8)
  6:  53 +  41 =  94 (v:  7)
  7:  64 +   8 =  72 (v:-10)
  8:  23 +  25 =  48 (v: -4)
  9:  20 +  27 =  47 (v: -5)
!!Found!! at index 7
64 + 8 = 72[/code]

Fancy!  Let's get back to are problem at hand.


[u][b]The Code[/b][/u]

[u]Particle class[/u]
Small and compact.  A few values on the particle's velocity, a global best, and the data (aka position).

[code]
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
[/code]

[u]PSO class[/u]

All of the constants mentioned in the problem break down, initialization, and operation.
[code]
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

        // Get the current 'fitness' based on the equation we are trying to solve
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

[/code]

A sample run:
[spoiler]
[code]
[code]Epoch: 0
-------
  0:  86 +  58 +  88 +  69 +  42 = 343 (v:  0)
  1:  73 +  89 +  86 +  44 +  44 = 336 (v:  0)
  2:  78 +  51 +  67 + 108 +  50 = 354 (v:  0)
  3:  89 + 106 +  94 + 101 +  66 = 456 (v:  0)
  4:  99 + 107 +  90 +  83 +  88 = 467 (v:  0)
  5:  74 +  44 +  50 + 106 +  91 = 365 (v:  0)
  6: 107 +  63 +  81 +  83 +  52 = 386 (v:  0)
  7:  55 +  58 +  59 +  58 +  53 = 283 (v:  0)
  8:  99 +  98 +  77 + 102 +  56 = 432 (v:  0)
  9: 102 +  89 +  50 +  50 +  80 = 371 (v:  0)
Current Best: 7

Epoch: 1
-------
  0:  76 +  58 +  78 +  59 +  32 = 303 (v:-10)
  1:  63 +  79 +  76 +  34 +  34 = 286 (v:-10)
  2:  68 +  41 +  57 +  98 +  40 = 304 (v:-10)
  3:  79 +  96 +  84 +  91 +  56 = 406 (v:-10)
  4:  89 +  97 +  80 +  73 +  78 = 417 (v:-10)
  5:  64 +  34 +  40 +  96 +  81 = 315 (v:-10)
  6:  97 +  53 +  71 +  73 +  42 = 336 (v:-10)
  7:  55 +  58 +  59 +  58 +  53 = 283 (v:  0)
  8:  89 +  88 +  67 +  92 +  46 = 382 (v:-10)
  9:  92 +  79 +  40 +  40 +  70 = 321 (v:-10)
Current Best: 7

Epoch: 2
-------
  0:  66 +  58 +  68 +  49 +  22 = 263 (v:-10)
  1:  53 +  69 +  66 +  24 +  24 = 236 (v:-10)
  2:  58 +  31 +  47 +  88 +  30 = 254 (v:-10)
  3:  69 +  86 +  74 +  81 +  46 = 356 (v:-10)
  4:  79 +  87 +  70 +  63 +  68 = 367 (v:-10)
  5:  54 +  24 +  30 +  86 +  71 = 265 (v:-10)
  6:  87 +  43 +  61 +  63 +  32 = 286 (v:-10)
  7:  55 +  58 +  59 +  58 +  53 = 283 (v:  0)
  8:  79 +  78 +  57 +  82 +  36 = 332 (v:-10)
  9:  82 +  69 +  30 +  30 +  60 = 271 (v:-10)
Current Best: 1

Epoch: 3
-------
  0:  56 +  48 +  58 +  39 +  12 = 213 (v:-10)
  1:  53 +  69 +  66 +  24 +  24 = 236 (v:-10)
  2:  48 +  21 +  37 +  78 +  20 = 204 (v:-10)
  3:  59 +  76 +  64 +  71 +  36 = 306 (v:-10)
  4:  69 +  77 +  60 +  53 +  58 = 317 (v:-10)
  5:  44 +  14 +  20 +  76 +  61 = 215 (v:-10)
  6:  77 +  33 +  51 +  53 +  22 = 236 (v:-10)
  7:  45 +  48 +  49 +  48 +  43 = 233 (v:-10)
  8:  69 +  68 +  47 +  72 +  26 = 282 (v:-10)
  9:  72 +  69 +  20 +  20 +  50 = 231 (v:-10)
Current Best: 2

Epoch: 4
-------
  0:  46 +  38 +  48 +  29 +   2 = 163 (v:-10)
  1:  43 +  59 +  56 +  14 +  14 = 186 (v:-10)
  2:  48 +  21 +  37 +  78 +  20 = 204 (v:-10)
  3:  49 +  66 +  54 +  61 +  26 = 256 (v:-10)
  4:  59 +  67 +  50 +  43 +  48 = 267 (v:-10)
  5:  34 +   4 +  10 +  66 +  51 = 165 (v:-10)
  6:  67 +  23 +  41 +  43 +  12 = 186 (v:-10)
  7:  35 +  38 +  39 +  38 +  33 = 183 (v:-10)
  8:  59 +  58 +  37 +  62 +  16 = 232 (v:-10)
  9:  62 +  59 +  10 +  10 +  40 = 181 (v:-10)
Current Best: 0

Epoch: 5
-------
  0:  46 +  38 +  48 +  29 +   2 = 163 (v:-10)
  1:  33 +  49 +  46 +   4 +   4 = 136 (v:-10)
  2:  38 +  11 +  27 +  68 +  10 = 154 (v:-10)
  3:  39 +  56 +  44 +  51 +  16 = 206 (v:-10)
  4:  49 +  57 +  40 +  33 +  38 = 217 (v:-10)
  5:  24 +  -6 +   0 +  56 +  41 = 115 (v:-10)
  6:  57 +  13 +  31 +  33 +   2 = 136 (v:-10)
  7:  25 +  38 +  29 +  28 +  23 = 143 (v:-10)
  8:  49 +  48 +  27 +  52 +   6 = 182 (v:-10)
  9:  52 +  49 +   0 +   0 +  30 = 131 (v:-10)
Current Best: 5

Epoch: 6
-------
  0:  36 +  28 +  38 +  19 +  -8 = 113 (v:-10)
  1:  23 +  39 +  36 +  -6 +  -6 =  86 (v:-10)
  2:  28 +   1 +  17 +  58 +   0 = 104 (v:-10)
  3:  29 +  46 +  34 +  41 +   6 = 156 (v:-10)
  4:  39 +  47 +  30 +  23 +  28 = 167 (v:-10)
  5:  24 +  -6 +   0 +  56 +  41 = 115 (v:-10)
  6:  47 +   3 +  21 +  23 +  -8 =  86 (v:-10)
  7:  15 +  28 +  19 +  18 +  13 =  93 (v:-10)
  8:  39 +  38 +  17 +  42 +  -4 = 132 (v:-10)
  9:  42 +  39 +   0 + -10 +  20 =  91 (v:-10)
Current Best: 1

Epoch: 7
-------
  0:  26 +  18 +  28 +   9 + -18 =  63 (v:-10)
  1:  23 +  39 +  36 +  -6 +  -6 =  86 (v:-10)
  2:  18 +  -9 +   7 +  48 + -10 =  54 (v:-10)
  3:  19 +  36 +  24 +  31 +  -4 = 106 (v:-10)
  4:  29 +  37 +  20 +  13 +  18 = 117 (v:-10)
  5:  14 + -16 + -10 +  46 +  31 =  65 (v:-10)
  6:  37 +  -7 +  11 +  13 + -18 =  36 (v:-10)
  7:   5 +  18 +   9 +   8 +   3 =  43 (v:-10)
  8:  29 +  28 +   7 +  32 + -14 =  82 (v:-10)
  9:  32 +  39 + -10 + -20 +  10 =  51 (v:-10)
Current Best: 5

Epoch: 8
-------
  0:  19 +  11 +  21 +   2 + -25 =  28 (v: -7)
  1:  13 +  29 +  26 + -16 + -16 =  36 (v:-10)
  2:   9 + -18 +  -2 +  39 + -19 =   9 (v: -9)
  3:   9 +  26 +  14 +  21 + -14 =  56 (v:-10)
  4:  19 +  27 +  10 +   3 +   8 =  67 (v:-10)
  5:  14 + -16 + -10 +  46 +  31 =  65 (v:-10)
  6:  28 + -16 +   2 +   4 + -27 =  -9 (v: -9)
  7:  15 +  28 +  19 +  18 +  13 =  93 (v: 10)
  8:  19 +  18 +  -3 +  22 + -24 =  32 (v:-10)
  9:  42 +  49 + -10 + -10 +  20 =  91 (v: 10)
Current Best: 4

Epoch: 9
-------
  0:  19 +  21 +  31 +  12 + -15 =  68 (v: 10)
  1:  23 +  39 +  36 +  -6 +  -6 =  86 (v: 10)
  2:  19 +  -8 +   8 +  49 +  -9 =  59 (v: 10)
  3:   1 +  18 +   6 +  13 + -22 =  16 (v: -8)
  4:  19 +  27 +  10 +   3 +   8 =  67 (v:-10)
  5:   5 + -25 + -19 +  37 +  22 =  20 (v: -9)
  6:  38 +  -6 +  12 +  14 + -17 =  41 (v: 10)
  7:   5 +  18 +   9 +   8 +   3 =  43 (v:-10)
  8:  19 +  28 +   7 +  32 + -14 =  72 (v: 10)
  9:  33 +  40 + -19 + -19 +  11 =  46 (v: -9)
!!Found!! at index 8
19 + 28 + 7 + 32 + -14 = 72[/code]
[/code]
[/spoiler] 


Not too bad for applying some general concepts to a problem.  I want to stress, again, that how you determine to encode your problem and determine fitness is coupled to your solution you are looking for.  This tutorial is not a set-in-stone boiler plate.   Not all problems will be searching for the most minimal each time or not using local neighborhood best. The Particle class may grow bigger, have more parts, or have less.  

Also I want to reiterate that basic PSO is not guarenteeing to find [i]the most optimal[/i] solution, but it will certainly try and find _a_ solution.  You certainly can tweak your route to collect solutions and determine a 'most optimal', but that is on a problem-by-problem basis.

FYI - I made the code tweakable enough in the constants you can try for different goals, number of polynomials, ranges, etc.

[u][b]GitHub link:[/b][/u]
https://github.com/modi1231/ParticleSwarmOptimization


[u][b]Advanced Topics:[/b][/u]
[list][*]use local neighborhood best
[*]try to make visualization for it
[*]use on finding best path in a graph
[*]Investigate a multi-objective problem
[/list]


[u][b]Reading[/b][/u]
[url="https://www.amazon.com/Intelligence-Morgan-Kaufmann-Evolutionary-Computation/dp/1558605959"]Swarm Intelligence[/url] by Russell C. Eberhart (Author), Yuhui Shi (Author), James Kennedy (

[u][b]Applications:[/b][/u]
[list][*][url="http://www.sciencedirect.com/science/article/pii/S1018364711000188"]Travel optimization[/url]
[*][url="http://link.springer.com/article/10.1007/s10596-009-9142-1"]Well placement[/url]
[*]neural network tuning
[*][url="https://www.researchgate.net/publication/224292515_A_Survey_of_Particle_Swarm_Optimization_Applications_in_Electric_Power_Systems"]electrical power system area optimizing[/url]
[*]http://www.scholarpedia.org/article/Particle_swarm_optimization#Applications_of_PSO_and_Current_Trends
[/list]
