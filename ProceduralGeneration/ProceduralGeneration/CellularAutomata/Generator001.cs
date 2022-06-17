//using Misc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CellularAutomata
//{
//    class GeneratorTest001
//    {
//        public void Start()
//        {
//            Rect r1 = new Rect(0, 0, -2, 2);
//            r1 = new Rect(0, 0, -2, -5);

//            const int STATE_EMPTY = 0;
//            const int STATE_SOLID = 1;

//            Random random = new Random();

//            while (true)
//            {
//                GeneratorPointRule[] generatorPointRules = new GeneratorPointRule[random.Next(5) + 2];
//                generatorPointRules[0] = new GeneratorPointRule(float.MinValue, (float)random.NextDouble(), STATE_SOLID, (float)random.NextDouble());
//                generatorPointRules[1] = new GeneratorPointRule(-(float)random.NextDouble(), float.MaxValue, STATE_EMPTY, (float)random.NextDouble());

//                for (int i = 2; i < generatorPointRules.Length; i++)
//                {
//                    generatorPointRules[i] = new GeneratorPointRule(
//                        -(float)random.NextDouble() * random.Next(3),
//                        (float)random.NextDouble() * random.Next(3),
//                        random.Next(2),
//                        (float)random.NextDouble());
//                }


//                GeneratorSettings settings = new GeneratorSettings(random.Next(20),
//                    generatorPointRules
//                    );

//                GeneratorStateRule[] generatorStateRules = new GeneratorStateRule[random.Next(5) + 1];
//                for (int i = 0; i < generatorStateRules.Length; i++)
//                {
//                    generatorStateRules[i] = new GeneratorStateRule(
//                        new Rect(random.Next(-5, 6), random.Next(-5, 6), 1 + random.Next(5), 1 + random.Next(5)),
//                        new StateValue(STATE_EMPTY, (float)random.NextDouble() * 2 - 1),
//                        new StateValue(STATE_SOLID, (float)random.NextDouble() * 2 - 1));
//                }


//                do
//                {
//                    Generator gen = new Generator(50, 30, settings,
//                    generatorStateRules);

//                    gen.Print(
//                        new GeneratorPointVisual(STATE_SOLID, '#'),
//                        new GeneratorPointVisual(STATE_EMPTY, ','));

//                }
//                while (Console.ReadKey().Key == ConsoleKey.Enter);

//                //GeneratorSettings settings = new GeneratorSettings(5,
//                //    new GeneratorPointRule(float.MinValue, .1f, STATE_SOLID, .25f),
//                //    new GeneratorPointRule(-.1f, float.MaxValue, STATE_EMPTY, .25f)
//                //    );

//                //Generator gen = new Generator(50, 30, settings,
//                //    new GeneratorStateRule(new Rect(-1, -1, 3, 4), new StateValue(STATE_EMPTY, -.5f), new StateValue(STATE_SOLID, .15f)));

//            }
//        }

//    }

//    class Generator
//    {
//        public int Width { get; private set; }
//        public int Height { get; private set; }

//        private Random Random { get; set; }

//        public GeneratorPoint[,] Points { get; private set; }

//        private GeneratorSettings Settings { get; set; }
//        private GeneratorStateRule[] Rules { get; set; }


//        private int[,] States { get; set; }


//        public Generator(int width, int height, GeneratorSettings settings, params GeneratorStateRule[] rules)
//        {
//            this.Random = new Random();

//            this.Width = width;
//            this.Height = height;

//            this.Settings = settings;
//            this.Rules = rules;

//            this.Generate();
//        }

//        void Generate()
//        {
//            this.Points = new GeneratorPoint[this.Width, this.Height];
//            this.States = new int[this.Width, this.Height];

//            int[] validStates = this.Settings.PointRules.Select(rule => rule.State).Distinct().ToArray();

//            for (int y = 0; y < this.Height; y++)
//            {
//                for (int x = 0; x < this.Width; x++)
//                {
//                    this.Points[x, y] = new GeneratorPoint(x, y);
//                    int stateIndex = this.Random.Next(validStates.Length);
//                    this.States[x, y] = validStates[stateIndex];
//                }
//            }

//            for (int iteration = 0; iteration < this.Settings.RuleIterations; iteration++)
//            {
//                this.ApplyRules();
//            }
//        }

//        void ApplyRules()
//        {
//            int[,] toReturn = new int[this.Width, this.Height];

//            for (int y = 0; y < this.Height; y++)
//            {
//                for (int x = 0; x < this.Width; x++)
//                {
//                    GeneratorPoint point = this.Points[x, y];
//                    float value = this.CalculateRuleValue(x, y);

//                    point.Weight += value;

//                    GeneratorPointRule[] validRules = this.Settings.PointRules.Where(rule => rule.WeightMin <= point.Weight && rule.WeightMax >= point.Weight).ToArray();

//                    if (validRules.Length > 0)
//                    {
//                        double sum = validRules.Sum(rule => rule.Chance);
//                        double rng = this.Random.NextDouble() * sum;
//                        int ruleIndex = 0; // this.Random.Next(validRules.Length);

//                        for (int i = 0; i < validRules.Length; i++)
//                        {
//                            if (validRules[i].Chance <= rng)
//                            {
//                                ruleIndex = i;
//                                break;
//                            }

//                            rng -= validRules[i].Chance;
//                        }

//                        toReturn[x, y] = validRules[ruleIndex].State;
//                    }
//                }
//            }

//            this.States = toReturn;
//        }

//        float CalculateRuleValue(int x, int y)
//        {
//            float toReturn = 0;

//            foreach (var rule in this.Rules)
//            {
//                for (int ruleY = 0; ruleY <= rule.Area.Height; ruleY++)
//                {
//                    for (int ruleX = 0; ruleX <= rule.Area.Width; ruleX++)
//                    {
//                        int fX = x + rule.Area.OffsetX + ruleX;
//                        int fY = y + rule.Area.OffsetY + ruleY;

//                        if (!this.ValidIndex(fX, fY))
//                            continue;

//                        int state = this.States[fX, fY];

//                        foreach (var stateValue in rule.StateValues)
//                        {
//                            if (stateValue.State != state)
//                                continue;

//                            toReturn += stateValue.Weight;
//                        }
//                    }
//                }
//            }

//            return toReturn;
//        }

//        private bool ValidIndex(int x, int y)
//        {
//            return !(x < 0 || x >= this.Width - 1 || y < 0 || y >= this.Height - 1);
//        }

//        public void Print(params GeneratorPointVisual[] visuals)
//        {
//            Console.Clear();

//            for (int y = 0; y < this.Height; y++)
//            {
//                string line = "";
//                for (int x = 0; x < this.Width; x++)
//                {
//                    int state = this.States[x, y];
//                    line += visuals.FirstOrDefault(visual => visual.State == state)?.Symbol ?? '?';
//                }
//                Console.WriteLine(line);
//            }
//        }
//    }

//    class GeneratorPoint : Int2
//    {
//        public float Weight { get; set; }


//        public GeneratorPoint(int x, int y) : base(x, y)
//        {
//        }
//    }

//    class GeneratorPointVisual
//    {
//        public int State { get; private set; }
//        public char Symbol { get; private set; }


//        public GeneratorPointVisual(int state, char symbol)
//        {
//            this.State = state;
//            this.Symbol = symbol;
//        }

//    }

//    class GeneratorSettings
//    {
//        public GeneratorPointRule[] PointRules { get; private set; }
//        public int RuleIterations { get; private set; }


//        public GeneratorSettings(int ruleIterations, params GeneratorPointRule[] pointRules)
//        {
//            this.RuleIterations = ruleIterations;
//            this.PointRules = pointRules.OrderBy(rule => rule.Chance).ToArray();
//        }
//    }

//    class GeneratorPointRule
//    {
//        public float WeightMin { get; private set; }
//        public float WeightMax { get; private set; }

//        public int State { get; private set; }
//        public float Chance { get; private set; }

//        public GeneratorPointRule(float weightMin, float weightMax, int state, float chance)
//        {
//            this.WeightMin = weightMin;
//            this.WeightMax = weightMax;
//            this.State = state;
//            this.Chance = chance;
//        }

//    }

//    class GeneratorStateRule
//    {
//        public Rect Area { get; set; }
//        public StateValue[] StateValues { get; private set; }

//        public GeneratorStateRule(Rect area, params StateValue[] stateValues)
//        {
//            this.Area = area;
//            this.StateValues = stateValues;
//        }

//        public void Calculate(GeneratorPoint point)
//        {

//        }
//    }

//    class StateValue
//    {
//        public int State { get; private set; }
//        public float Weight { get; private set; }

//        public StateValue(int state, float weight)
//        {
//            this.State = state;
//            this.Weight = weight;
//        }

//    }
//}
