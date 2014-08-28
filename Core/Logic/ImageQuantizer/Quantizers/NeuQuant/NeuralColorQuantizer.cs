using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.Quantizers.NeuQuant
{
    /// <summary>
    ///   The NeuQuant Neural-Net image quantization algorithm (© Anthony Dekker 1994) 
    ///   is a replacement for the common Median Cut algorithm. It is described in the 
    ///   article Kohonen neural networks for optimal colour quantization  in Volume 5, 
    ///   pp 351-367 of the journal Network: Computation in Neural Systems, Institute of 
    ///   Physics Publishing, 1994 (PDF version available).
    /// </summary>
    public class NeuralColorQuantizer : BaseColorQuantizer
    {
        #region | Constants |

        private const Byte DefaultQuality = 10; // 10

        private const Int32 AlphaBiasShift = 10;
        private const Int32 AlphaRadiusBias = 1 << AlphaRadiusBetaShift;
        private const Int32 AlphaRadiusBetaShift = AlphaBiasShift + RadiusBiasShift;
        private const Int32 Beta = (InitialBias >> BetaShift);
        private const Int32 BetaShift = 10;
        private const Int32 BetaGamma = (InitialBias << (GammaShift - BetaShift));
        private const Int32 DefaultRadius = NetworkSize >> 3;
        private const Int32 DefaultRadiusBiasShift = 6;
        private const Int32 DefaultRadiusBias = 1 << DefaultRadiusBiasShift;
        private const Int32 GammaShift = 10;
        private const Int32 InitialAlpha = 1 << AlphaBiasShift;
        private const Int32 InitialBias = 1 << InitialBiasShift;
        private const Int32 InitialBiasShift = 16;
        private const Int32 InitialRadius = (DefaultRadius * DefaultRadiusBias);
        private const Int32 MaximalNetworkPosition = NetworkSize - 1;
        private const Int32 NetworkSize = 256;
        private const Int32 NetworkBiasShift = 4;
        private const Int32 RadiusBiasShift = 8;
        private const Int32 RadiusDecrease = 30;
        private const Int32 RadiusBias = 1 << RadiusBiasShift;

        #endregion

        #region | Fields |

        private readonly FastRandom random;
        private readonly ConcurrentDictionary<Int32, Boolean> uniqueColors;
        private Int32 alpha;
        private Int32 alphaDecrease;

        private Int32[] bias;
        private Int32 delta;
        private Int32[] frequency;
        private Int32 initialRadius;
        private Int32[][] network;
        private Int32[] networkIndexLookup;

        private Byte quality;
        private Int32 radius;
        private Int32[] radiusPower;

        #endregion

        #region | Properties |

        /// <summary>
        ///   Gets or sets the quality.
        /// </summary>
        /// <value> The quality. </value>
        public Byte Quality
        {
            get { return quality; }
            set
            {
                quality = value;
                alphaDecrease = 30 + (quality - 1);
            }
        }

        #endregion

        #region | Constructors |

        /// <summary>
        ///   Initializes a new instance of the <see cref="NeuralColorQuantizer" /> class.
        /// </summary>
        public NeuralColorQuantizer()
        {
            Quality = DefaultQuality;

            random = new FastRandom(0);
            uniqueColors = new ConcurrentDictionary<Int32, Boolean>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NeuralColorQuantizer" /> class.
        /// </summary>
        /// <param name="quality"> The quality. </param>
        public NeuralColorQuantizer(Byte quality)
            : this()
        {
            Quality = quality;
        }

        #endregion

        #region | Helper methods |

        private Int32 FindClosestNeuron(Int32 red, Int32 green, Int32 blue)
        {
            // initializes the search variables
            var bestIndex = -1;
            var bestDistance = ~(1 << 31);
            var bestBiasIndex = bestIndex;
            var bestBiasDistance = bestDistance;

            for (var index = 0; index < NetworkSize; index++)
            {
                var neuron = network[index];

                // computes differences between neuron (color), and provided color
                var deltaRed = neuron[2] - red;
                var deltaGreen = neuron[1] - green;
                var deltaBlue = neuron[0] - blue;

                // makes values absolute
                if (deltaRed < 0) deltaRed = -deltaRed;
                if (deltaGreen < 0) deltaGreen = -deltaGreen;
                if (deltaBlue < 0) deltaBlue = -deltaBlue;

                // sums the distance
                var distance = deltaRed + deltaGreen + deltaBlue;

                // if best so far, store it
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = index;
                }

                // calculates biase distance
                var biasDistance = distance - ((bias[index]) >> (InitialBiasShift - NetworkBiasShift));

                // if best so far, store it
                if (biasDistance < bestBiasDistance)
                {
                    bestBiasDistance = biasDistance;
                    bestBiasIndex = index;
                }

                var betaFrequency = (frequency[index] >> BetaShift);
                frequency[index] -= betaFrequency;
                bias[index] += (betaFrequency << GammaShift);
            }

            frequency[bestIndex] += Beta;
            bias[bestIndex] -= BetaGamma;
            return bestBiasIndex;
        }

        /// <summary>
        ///   Forces the strength of bias of the neuron towards certain color.
        /// </summary>
        private void LearnNeuron(Int32 alpha, Int32 red, Int32 green, Int32 blue, Int32 networkIndex)
        {
            /* alter hit neuron */
            var neuron = network[networkIndex];
            neuron[2] -= (alpha * (neuron[2] - red)) / InitialAlpha;
            neuron[1] -= (alpha * (neuron[1] - green)) / InitialAlpha;
            neuron[0] -= (alpha * (neuron[0] - blue)) / InitialAlpha;
        }

        /// <summary>
        ///   Spread the bias to neuron neighbors.
        /// </summary>
        protected void LearnNeuronNeighbors(Int32 red, Int32 green, Int32 blue, Int32 networkIndex, Int32 radius)
        {
            // detects lower border
            var lowBound = networkIndex - radius;
            if (lowBound < -1) lowBound = -1;

            // detects high border
            var highBound = networkIndex + radius;
            if (highBound > NetworkSize) highBound = NetworkSize;

            // initializes the variables
            var increaseIndex = networkIndex + 1;
            var decreaseIndex = networkIndex - 1;
            var radiusStep = 1;

            // learns neurons in a given radius
            while (increaseIndex < highBound || decreaseIndex > lowBound)
            {
                Int32[] neuron;
                var alphaMultiplicator = radiusPower[radiusStep++];

                if (increaseIndex < highBound)
                {
                    neuron = network[increaseIndex++];
                    neuron[0] -= (alphaMultiplicator * (neuron[0] - blue)) / AlphaRadiusBias;
                    neuron[1] -= (alphaMultiplicator * (neuron[1] - green)) / AlphaRadiusBias;
                    neuron[2] -= (alphaMultiplicator * (neuron[2] - red)) / AlphaRadiusBias;
                }

                if (decreaseIndex > lowBound)
                {
                    neuron = network[decreaseIndex--];
                    neuron[0] -= (alphaMultiplicator * (neuron[0] - blue)) / AlphaRadiusBias;
                    neuron[1] -= (alphaMultiplicator * (neuron[1] - green)) / AlphaRadiusBias;
                    neuron[2] -= (alphaMultiplicator * (neuron[2] - red)) / AlphaRadiusBias;
                }
            }
        }

        #endregion

        #region | Network methods |

        private void UnbiasNetwork()
        {
            for (var index = 0; index < NetworkSize; index++)
            {
                network[index][0] >>= NetworkBiasShift;
                network[index][1] >>= NetworkBiasShift;
                network[index][2] >>= NetworkBiasShift;
                network[index][3] = index;
            }
        }

        private void SortNetwork()
        {
            var startIndex = 0;
            var previousValue = 0;

            for (var index = 0; index < NetworkSize; index++)
            {
                var neuron = network[index];

                var bestIndex = index;
                var bestValue = neuron[1];

                for (var subIndex = index + 1; subIndex < NetworkSize; subIndex++)
                {
                    var subNeuron = network[subIndex];

                    if (subNeuron[1] < bestValue)
                    {
                        bestIndex = subIndex;
                        bestValue = subNeuron[1];
                    }
                }

                // swaps the neuron components
                if (index != bestIndex)
                {
                    var neuronB = network[bestIndex];

                    for (var subIndex = 0; subIndex < 4; subIndex++)
                    {
                        var swap = neuronB[subIndex];
                        neuronB[subIndex] = neuron[subIndex];
                        neuron[subIndex] = swap;
                    }
                }

                // if the value is still not optimal
                if (bestValue != previousValue)
                {
                    networkIndexLookup[previousValue] = (startIndex + index) >> 1;

                    for (var subIndex = previousValue + 1; subIndex < bestValue; subIndex++)
                    {
                        networkIndexLookup[subIndex] = index;
                    }

                    previousValue = bestValue;
                    startIndex = index;
                }
            }

            networkIndexLookup[previousValue] = (startIndex + MaximalNetworkPosition) >> 1;

            // resets certain portion of the index lookup
            for (var index = previousValue + 1; index < 256; index++)
            {
                networkIndexLookup[index] = MaximalNetworkPosition;
            }
        }

        private void LearnSampleColor(Color color)
        {
            var red = color.R << NetworkBiasShift;
            var green = color.G << NetworkBiasShift;
            var blue = color.B << NetworkBiasShift;

            var neuronIndex = FindClosestNeuron(red, green, blue);

            LearnNeuron(alpha, red, green, blue, neuronIndex);

            if (radius != 0)
            {
                LearnNeuronNeighbors(red, green, blue, neuronIndex, radius);
            }

            if (delta == 0) delta = 1;

            alpha -= alpha / alphaDecrease;
            initialRadius -= initialRadius / RadiusDecrease;
            radius = initialRadius >> DefaultRadiusBiasShift;

            if (radius <= 1) radius = 0;
            var radiusSquared = radius * radius;

            for (var index = 0; index < radius; index++)
            {
                var indexSquared = index * index;
                radiusPower[index] = alpha * (((radiusSquared - indexSquared) * RadiusBias) / radiusSquared);
            }
        }

        #endregion

        #region << BaseColorQuantizer >>

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnPrepare" /> for more details.
        /// </summary>
        protected override void OnPrepare(ImageBuffer image)
        {
            base.OnPrepare(image);

            OnFinish();

            network = new Int32[NetworkSize][];
            uniqueColors.Clear();

            // initializes all the neurons in the network
            for (var neuronIndex = 0; neuronIndex < NetworkSize; neuronIndex++)
            {
                var neuron = new Int32[4];

                // calculates the base value for all the components
                var baseValue = (neuronIndex << (NetworkBiasShift + 8)) / NetworkSize;
                neuron[0] = baseValue;
                neuron[1] = baseValue;
                neuron[2] = baseValue;

                // determines other per neuron values
                bias[neuronIndex] = 0;
                network[neuronIndex] = neuron;
                frequency[neuronIndex] = InitialBias / NetworkSize;
            }

            // initializes the some variables
            alpha = InitialAlpha;
            initialRadius = InitialRadius;

            // determines the radius
            const int potentialRadius = InitialRadius >> DefaultRadiusBiasShift;
            radius = potentialRadius;
            var radiusSquared = radius * radius;

            // precalculates the powers for all the radiuses
            for (var index = 0; index < radius; index++)
            {
                var indexSquared = index * index;
                radiusPower[index] = alpha * (((radiusSquared - indexSquared) * RadiusBias) / radiusSquared);
            }
        }

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnAddColor" /> for more details.
        /// </summary>
        protected override void OnAddColor(Color color, Int32 key, Int32 x, Int32 y)
        {
            base.OnAddColor(color, key, x, y);

            if (random.Next(DefaultQuality) == 0)
            {
                LearnSampleColor(color);
            }
        }

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnGetPalette" /> for more details.
        /// </summary>
        protected override List<Color> OnGetPalette(Int32 colorCount)
        {
            // post-process the neural network
            UnbiasNetwork();
            SortNetwork();

            // initialize the index cache
            var indices = new Int32[NetworkSize];

            // re-index the network cache
            for (var index = 0; index < NetworkSize; index++)
            {
                indices[network[index][3]] = index;
            }

            // initializes the empty palette
            var result = new List<Color>();

            // grabs the best palette, from the neurons
            for (var index = 0; index < NetworkSize; index++)
            {
                var neuronIndex = indices[index];

                var red = network[neuronIndex][2];
                var green = network[neuronIndex][1];
                var blue = network[neuronIndex][0];

                var color = Color.FromArgb(255, red, green, blue);
                result.Add(color);
            }

            return result;
        }

        /// <summary>
        ///   See <see cref="IColorQuantizer.GetPaletteIndex" /> for more details.
        /// </summary>
        protected override void OnGetPaletteIndex(Color color, Int32 key, Int32 x, Int32 y, out Int32 paletteIndex)
        {
            var bestDistance = 1000;
            var increaseIndex = networkIndexLookup[color.G];
            var decreaseIndex = increaseIndex - 1;
            paletteIndex = -1;

            while (increaseIndex < NetworkSize || decreaseIndex >= 0)
            {
                if (increaseIndex < NetworkSize)
                {
                    var neuron = network[increaseIndex];

                    // add green delta
                    var deltaG = neuron[1] - color.G;
                    if (deltaG < 0) deltaG = -deltaG;
                    var distance = deltaG;

                    if (distance >= bestDistance)
                    {
                        increaseIndex = NetworkSize;
                    }
                    else
                    {
                        increaseIndex++;

                        // add blue delta
                        var deltaB = neuron[0] - color.B;
                        if (deltaB < 0) deltaB = -deltaB;
                        distance += deltaB;

                        if (distance < bestDistance)
                        {
                            // add red delta
                            var deltaR = neuron[2] - color.R;
                            if (deltaR < 0) deltaR = -deltaR;
                            distance += deltaR;

                            if (distance < bestDistance)
                            {
                                bestDistance = distance;
                                paletteIndex = neuron[3];
                            }
                        }
                    }
                }

                if (decreaseIndex >= 0)
                {
                    var neuron = network[decreaseIndex];

                    // add green delta
                    var deltaG = color.G - neuron[1];
                    if (deltaG < 0) deltaG = -deltaG;
                    var distance = deltaG;

                    if (distance >= bestDistance)
                    {
                        decreaseIndex = -1;
                    }
                    else
                    {
                        decreaseIndex--;

                        // add blue delta
                        var deltaBlue = neuron[0] - color.B;
                        if (deltaBlue < 0) deltaBlue = -deltaBlue;
                        distance += deltaBlue;

                        if (distance < bestDistance)
                        {
                            // add red delta
                            var deltaRed = neuron[2] - color.R;
                            if (deltaRed < 0) deltaRed = -deltaRed;
                            distance += deltaRed;

                            if (distance < bestDistance)
                            {
                                bestDistance = distance;
                                paletteIndex = neuron[3];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnFinish" /> for more details.
        /// </summary>
        protected override void OnFinish()
        {
            base.OnFinish();

            bias = new Int32[NetworkSize];
            frequency = new Int32[NetworkSize];
            networkIndexLookup = new Int32[256];
            radiusPower = new Int32[DefaultRadius];
            network = null;
        }

        #endregion

        #region << IColorQuantizer >>

        /// <summary>
        ///   See <see cref="IColorQuantizer.AllowParallel" /> for more details.
        /// </summary>
        public override Boolean AllowParallel
        {
            get { return true; }
        }

        #endregion
    }
}