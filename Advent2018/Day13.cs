using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public bool IsCollided(Cart other) => x == other.x && y == other.y;
        }

        Cart GetCollidedCart(IList<Cart> carts)
        {
            for (var i = 0; i < carts.Count - 1; i++)
                for (var j = i + 1; j < carts.Count; j++)
                    if (carts[i].IsCollided(carts[j]))
                        return carts[i];

            return null;
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
    }
}
