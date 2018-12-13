using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day13
    {
        class Cart
        {
            public int x;
            public int y;
            public int dX;
            public int dY;
            private int crossingCount = 0;

            public Cart(int x, int y, char initialDirection)
            {
                this.x = x;
                this.y = y;
                switch (initialDirection)
                {
                    case '<':
                        dX = -1;
                        dY = 0;
                        break;

                    case '>':
                        dX = 1;
                        dY = 0;
                        break;

                    case '^':
                        dX = 0;
                        dY = -1;
                        break;

                    case 'v':
                    case 'V':
                        dX = 0;
                        dY = 1;
                        break;

                    default:
                        Oh.Bugger();
                        break;
                }
            }

            public void Step(char currentLocation)
            {
                switch (currentLocation)
                {
                    case '/':
                        if (dX == 0)
                            TurnRight();
                        else
                            TurnLeft();
                        break;

                    case '\\':
                        if (dX == 0)
                            TurnLeft();
                        else
                            TurnRight();
                        break;

                    case '+':
                        UpdateCrossing();
                        break;

                    case '.':
                        break;

                    default:
                        Oh.ShttingHell();
                        break;
                }

                x += dX;
                y += dY;
            }

            public void TurnLeft()
            {
                if (dX == 0)
                {
                    dX = dY;
                    dY = 0;
                }
                else
                {
                    dY = -dX;
                    dX = 0;
                }
            }

            public void TurnRight()
            {
                if (dX == 0)
                {
                    dX = -dY;
                    dY = 0;
                }
                else
                {
                    dY = dX;
                    dX = 0;
                }
            }

            public void UpdateCrossing()
            {
                switch (crossingCount)
                {
                    case 0:
                        TurnLeft();
                        crossingCount = 1;
                        break;

                    case 1:
                        crossingCount = 2;
                        break;

                    case 2:
                        TurnRight();
                        crossingCount = 0;
                        break;
                }
            }

            public bool IsCollided(Cart other) => x == other.x && y == other.y;

            public override string ToString() => $"{x}, {y}";
        }

        Cart GetCollidedCart(IList<Cart> carts)
        {
            for (var i = 0; i < carts.Count - 1; i++)
                for (var j = i + 1; j < carts.Count; j++)
                    if (carts[i].IsCollided(carts[j]))
                        return carts[i];

            return null;
        }

        Cart GetCollidedWith(ICollection<Cart> carts, Cart cart)
        {
            foreach (var other in carts)
                if (cart != other && cart.IsCollided(other))
                    return other;

            return null;
        }

        (char[,], HashSet<Cart>) LoadSimulation(string inputFile)
        {
            var lines = FileIterator.LoadLines<string>(inputFile);
            var width = lines[0].Length;
            var height = lines.Length;
            var cavern = new char[width, height];
            var carts = new HashSet<Cart>();

            foreach (var(x, y) in Generators.Rectangle(width, height))
            {
                var thing = lines[y][x];
                switch (thing)
                {
                    case '<':
                    case '>':
                    case '^':
                    case 'V':
                    case 'v':
                        carts.Add(new Cart(x, y, thing));
                        cavern[x, y] = '.';
                        break;

                    case '-':
                    case '|':
                        cavern[x, y] = '.';
                        break;

                    case '+':
                    case '/':
                    case '\\':
                    case ' ':
                        cavern[x, y] = thing;
                        break;

                    default:
                        Oh.Bollocks();
                        break;
                }
            }

            return (cavern, carts);
        }

        void DebugDumpCavern(char[,] cavern)
        {
            foreach (var (x, y) in cavern.Rectangle())
            {
                if (x == 0)
                    Debug.WriteLine("");

                Debug.Write(cavern[x, y]);
            }
            Debug.WriteLine("");
        }

        ICollection<Cart> Step(char[,] cavern, ICollection<Cart> carts)
        {
            var collidedCarts = new List<Cart>();
            var orderedCarts = carts.OrderBy(c => c.y).ThenBy(c => c.x);

            foreach (var cart in orderedCarts)
            {
                cart.Step(cavern[cart.x, cart.y]);
                var collidedWith = GetCollidedWith(carts, cart);
                if (collidedWith != null)
                {
                    collidedCarts.Add(cart);
                    collidedCarts.Add(collidedWith);
                    carts.Remove(cart);
                    carts.Remove(collidedWith);
                }
            }

            return collidedCarts;
        }

        [Theory]
        [InlineData('^', -1, 0)]
        [InlineData('<', 0, 1)]
        [InlineData('v', 1, 0)]
        [InlineData('>', 0, -1)]
        void Cart_TurnLeft_Test(char initialDirection, int expectedDX, int expectedDY)
        {
            var cart = new Cart(0, 0, initialDirection);
            cart.TurnLeft();
            cart.dX.Should().Be(expectedDX);
            cart.dY.Should().Be(expectedDY);
        }

        [Theory]
        [InlineData('^', 1, 0)]
        [InlineData('<', 0, -1)]
        [InlineData('v', -1, 0)]
        [InlineData('>', 0, 1)]
        void Cart_TurnRight_Test(char initialDirection, int expectedDX, int expectedDY)
        {
            var cart = new Cart(0, 0, initialDirection);
            cart.TurnRight();
            cart.dX.Should().Be(expectedDX);
            cart.dY.Should().Be(expectedDY);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, 1, 2)]
        [InlineData(3, 4, 0, 0, 2, 3, 3, 4, 3, 4)]
        [InlineData(7, 9, 7, 9, 2, 3, 7, 9, 3, 4)]
        [InlineData(6, 5, 7, 9, 6, 5, 7, 2, 6, 5, 3, 4)]
        void GetCollidedCart_Test(int expectedX, int expectedY, params int[] cartLocations)
        {
            var carts = new List<Cart>();
            for (var i = 0; i < cartLocations.Length; i += 2)
                carts.Add(new Cart(cartLocations[i], cartLocations[i + 1], '<'));

            var collided = GetCollidedCart(carts);
            collided.Should().NotBeNull();
            collided.x.Should().Be(expectedX);
            collided.y.Should().Be(expectedY);
        }

        [Theory]
        [InlineData(7, 3, "Data/Day13-Test.txt")]
        [InlineData(2, 0, "Data/Day13-Test2.txt")]
        [InlineData(113, 136, "Data/Day13.txt")] // Solution
        void Problem1(int expectedX, int expectedY, string inputFile)
        {
            var (cavern, carts) = LoadSimulation(inputFile);
            ICollection<Cart> collidedCarts = null;
            do
            {
                collidedCarts = Step(cavern, carts);
            }
            while (collidedCarts.Count == 0);

            collidedCarts.First().x.Should().Be(expectedX);
            collidedCarts.First().y.Should().Be(expectedY);
        }

        [Theory]
        [InlineData(6, 4, "Data/Day13-Test2.txt")]
        [InlineData(114, 136, "Data/Day13.txt")] // Solution
        void Problem2(int expectedX, int expectedY, string inputFile)
        {
            var (cavern, carts) = LoadSimulation(inputFile);

            while (carts.Count > 1)
                Step(cavern, carts);

            carts.First().x.Should().Be(expectedX);
            carts.First().y.Should().Be(expectedY);
        }
    }
}
