using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Layer
    {
        private readonly int _depth;
        private readonly int _range;
        private int _position = 0;
        private int _dY = 1;

        public int Depth => _depth;

        public int Score
        {
            get;
            private set;
        }

        public bool Caught
        {
            get;
            private set;
        }

        public int Period
        {
            get;
            private set;
        }

        public Layer(int depth, int range)
        {
            Caught = false;
            Score = depth * range;
            Period = (depth - 1) * 2;
            _depth = depth;
            _range = range;
        }

        public void update()
        {
            if (_depth <= 1) return;

            if (_position == 0)
                _dY = 1;
            else if (_position == (_depth - 1))
                _dY = -1;

            _position += _dY;
        }

        public void enter()
        {
            if (_position == 0 && _depth != 0)
                Caught = true;
        }

        public void Reset()
        {
            Caught = false;
            _position = 0;
        }
    }

    static class LayerExtentions
    {
        static readonly Layer EmptyLayer = new Layer(0, 0);

        public static Layer GetLayer(this Dictionary<int, Layer> dict, int range)
        {
            Layer r;
            if (dict.TryGetValue(range, out r))
                return r;

            return EmptyLayer;
        }
    }

    public class Problem1301
    {
        Dictionary<int, Layer> Load(string filename, out int maxRange)
        {
            var layers = new Dictionary<int, Layer>();
            var max = 0;
            FileIterator.ForEachLine<string>(filename, line =>
            {
                var spec = line.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (spec.Length != 2) Oh.ForFucksSake();

                var range = int.Parse(spec[0]);
                var depth = int.Parse(spec[1]);
                var layer = new Layer(depth, range);
                layers[range] = layer;
                max = range;
            });

            maxRange = max;
            return layers;
        }

        int GetScore(Dictionary<int, Layer> layers, int maxRange, int delay)
        {
            for (var i = 0; i < delay; i++)
                foreach (var layer in layers.Values)
                    layer.update();

            var total = 0;
            for (var i = 0; i <= maxRange; i++)
            {
                var l = layers.GetLayer(i);
                l.enter();
                if (l.Caught)
                {
                    total += l.Score;
                    if (delay != 0) return 1;
                }

                foreach (var layer in layers.Values)
                    layer.update();
            }


            return total;
        }

        int Solve1(string datafile, int delay)
        {
            int maxRange;
            var layers = Load(datafile, out maxRange);
            return GetScore(layers, maxRange, delay);
        }

        [Theory]
        [InlineData("Data/1301-example.txt", 24)]
        [InlineData("Data/1301.txt", 2160)]
        public void Part1(string datafile, int answer) => Solve1(datafile, 0).Should().Be(answer);


        bool CheckDelay(Dictionary<int, Layer> layers, int maxRange, int delay)
        {
            for (var i = 0; i <= maxRange; i++)
            {
                var l = layers.GetLayer(i);
                if (l.Depth == 0) continue;

                var pos = (delay + i) % l.Period;
                if (pos == 0)
                {
                    return false;
                }
            }

            return true;
        }

        [Theory]
        [InlineData("Data/1301-example.txt", 10)]
        [InlineData("Data/1301.txt", 3907470)]
        public void Part2Quick(string datafile, int answer)
        {
            int maxRange;
            var layers = Load(datafile, out maxRange);


            var delay = 1;
            while (true)
            {
                if (CheckDelay(layers, maxRange, delay))
                {
                    delay.Should().Be(answer);
                    return;
                }
                delay++;
            }
        }

        /*[Fact]
        public void Part2Check() => Solve1("Data/1301-example.txt", 10).Should().Be(0);

        [Theory]
        [InlineData("Data/1301-example.txt", 10)]
        [InlineData("Data/1301.txt", 0)]
        public void Part2(string datafile, int answer)
        {
            int maxRange;
            var layers = Load(datafile, out maxRange);

            var i = 1;
            while (true)
            {
                if (GetScore(layers, maxRange, i) == 0)
                {
                    i.Should().Be(answer);
                    return;
                }

                foreach (var layer in layers.Values)
                    layer.Reset();

                i++;
            }

        }*/
    }
}
