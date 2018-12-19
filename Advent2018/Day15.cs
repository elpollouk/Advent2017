using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day15
    {
        enum EntityType
        {
            None,
            Wall,
            Elf,
            Goblin
        }

        struct PathStep
        {
            public static readonly PathStep MaxDistance = new PathStep(int.MaxValue, (0, 0), (0, 0));

            public readonly int DistanceFromStart;
            public readonly (int x, int y) Pos;
            public readonly (int x, int y) From;
    
            public PathStep(int distance, (int x, int y) pos, (int x, int y) from)
            {
                DistanceFromStart = distance;
                Pos = pos;
                From = from;
            }

            public bool IsQuickerThan(PathStep other)
            {
                if (DistanceFromStart < other.DistanceFromStart)
                    return true;

                return false;
            }
        }

        class Entity
        {
            public static readonly Entity None = new Entity(EntityType.None, (-1, -1));
            public static readonly Entity Wall = new Entity(EntityType.Wall, (-1, -1));

            public readonly EntityType Type;
            public (int x, int y) Pos;
            public int Health = 200;
            public int Power = 3;

            public Entity(EntityType type, (int x, int y) pos)
            {
                Type = type;
                Pos = pos;
            }

            public Entity GetEngaged(Entity[,] environment)
            {
                var enemy = Type == EntityType.Elf ? EntityType.Goblin : EntityType.Elf;

                var adjacent = new List<Entity>
                {
                    environment[Pos.x, Pos.y - 1],
                    environment[Pos.x - 1, Pos.y],
                    environment[Pos.x + 1, Pos.y],
                    environment[Pos.x, Pos.y + 1]
                };

                var engaged = adjacent.Where(e => e.Type == enemy)
                                      .OrderBy(e => e.Health)
                                      .ThenBy(e => e.Pos.y)
                                      .ThenBy(e => e.Pos.x);

                return engaged.FirstOrDefault();
            }
        }

        static Entity CharToCellState(char value, int x, int y)
        {
            switch (value)
            {
                case '.':
                    return Entity.None;

                case '#':
                    return Entity.Wall;

                case 'E':
                    return new Entity(EntityType.Elf, (x, y));

                case 'G':
                    return new Entity(EntityType.Goblin, (x, y));

                default:
                    Oh.Bugger();
                    return Entity.None;
            }
        }

        List<(int x, int y)> LoadTuples(int[] input)
        {
            var path = new List<(int x, int y)>();

            for (var i = 0; i < input.Length; i += 2)
                path.Add((input[i], input[i + 1]));

            return path;
        }

        ICollection<(int x, int y)> PathToNearestTarget(Entity[,] grid, (int x, int y) start, params (int x, int y)[] targets)
        {
            var pathMap = new Dictionary<(int x, int y), PathStep>();
            var frontier = new Queue<PathStep>();
            frontier.Enqueue(new PathStep(0, start, start));

            // Clear out the starting cell temporarily to make it a valid path location
            var originalCellEntity = grid[start.x, start.y];
            grid[start.x, start.y] = Entity.None;

            while (frontier.Count != 0)
            {
                var step = frontier.Dequeue();
                if (grid[step.Pos.x, step.Pos.y].Type != EntityType.None)
                    continue;

                var existingStep = pathMap.GetOrDefault(step.Pos, PathStep.MaxDistance);

                if (step.IsQuickerThan(existingStep))
                {
                    pathMap[step.Pos] = step;

                    frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x, step.Pos.y - 1), step.Pos));
                    frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x - 1, step.Pos.y), step.Pos));
                    frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x + 1, step.Pos.y), step.Pos));
                    frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x, step.Pos.y + 1), step.Pos));
                }
            }

            // Restore the original start location value
            grid[start.x, start.y] = originalCellEntity;

            var path = new LinkedList<(int x, int y)>();
            var orderedTargets = targets.Where(t => pathMap.ContainsKey(t))
                                        .OrderBy(t => pathMap[t].DistanceFromStart)
                                        .ThenBy(t => t.y)
                                        .ThenBy(t => t.x);

            if (orderedTargets.Count() != 0 )
            {
                var location = pathMap[orderedTargets.First()];
                while (location.DistanceFromStart != 0)
                {
                    path.AddFirst(location.Pos);
                    location = pathMap[location.From];
                }
            }

            return path;
        }

        ICollection<Entity> GatherEntities(Entity[,] environment)
        {
            var entities = new HashSet<Entity>();
            foreach (var (x, y) in environment.Rectangle())
            {
                switch (environment[x, y].Type)
                {
                    case EntityType.Elf:
                    case EntityType.Goblin:
                        entities.Add(environment[x, y]);
                        break;
                }
            }
            return entities;
        }

        (int x, int y)[] GetValidTargets(Entity[,] environment, ICollection<Entity> entities, EntityType targetsFor)
        {
            var list = new HashSet<(int x, int y)>();
            var targetType = targetsFor == EntityType.Elf ? EntityType.Goblin : EntityType.Elf;

            foreach (var entity in entities.Where(e => e.Type == targetType))
            {
                if (environment[entity.Pos.x, entity.Pos.y - 1].Type == EntityType.None)
                    list.Add((entity.Pos.x, entity.Pos.y - 1));
                if (environment[entity.Pos.x - 1, entity.Pos.y].Type == EntityType.None)
                    list.Add((entity.Pos.x - 1, entity.Pos.y));
                if (environment[entity.Pos.x + 1, entity.Pos.y].Type == EntityType.None)
                    list.Add((entity.Pos.x + 1, entity.Pos.y));
                if (environment[entity.Pos.x, entity.Pos.y + 1].Type == EntityType.None)
                    list.Add((entity.Pos.x, entity.Pos.y + 1));
            }

            return list.ToArray();
        }

        bool Step(Entity[,] environment, ICollection<Entity> entities)
        {
            if (WinningTeam(entities) != EntityType.None)
                return false; // The game is over

            var orderedEntities = entities.OrderBy(e => e.Pos.y).ThenBy(e => e.Pos.x).ToArray();
            for (var i = 0; i < orderedEntities.Length; i++)
            {
                var entity = orderedEntities[i];

                // Check that the entity hasn't been killed this round
                if (entity.Health <= 0)
                    continue;

                // Move first
                if (entity.GetEngaged(environment) == null)
                {
                    var targets = GetValidTargets(environment, entities, entity.Type);
                    var path = PathToNearestTarget(environment, entity.Pos, targets);
                    if (path.Count != 0)
                    {
                        var newLocation = path.First();
                        environment[entity.Pos.x, entity.Pos.y] = Entity.None;
                        entity.Pos = newLocation;
                        environment[newLocation.x, newLocation.y] = entity;
                    }
                }

                // Then attack
                var target = entity.GetEngaged(environment);
                if (target != null)
                {
                    target.Health -= entity.Power;
                    if (target.Health <= 0)
                    {
                        environment[target.Pos.x, target.Pos.y] = Entity.None;
                        entities.Remove(target);

                        // Determine if this was a full round or not
                        if (i != orderedEntities.Length - 1) // It's a full round if this is the last entity to act
                            if (WinningTeam(entities) == entity.Type)
                                return false; // Not a full round
                    }
                }
            }

            return true; // A full round
        }

        EntityType WinningTeam(ICollection<Entity> entities)
        {
            var firstType = entities.First().Type;

            if (entities.Count == entities.Where(e => e.Type == firstType).Count())
                return firstType;

            return EntityType.None;
        }

        [Theory]
        [InlineData(2, 3, 6, 3, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 6, 3)]
        [InlineData(2, 1, 6, 1, 3, 1, 3, 2, 4, 2, 5, 2, 5, 1, 6, 1)]
        [InlineData(2, 5, 6, 5, 2, 4, 3, 4, 4, 4, 5, 4, 6, 4, 6, 5)]
        [InlineData(1, 1, 1, 5, 2, 1, 2, 2, 2, 3, 2, 4, 1, 4, 1, 5)]
        [InlineData(7, 1, 7, 5, 6, 1, 6, 2, 6, 3, 6, 4, 7, 4, 7, 5)]
        [InlineData(2, 2, 2, 4, 2, 3, 2, 4)]
        [InlineData(2, 2, 9, 1)]
        void TestNavigation(int startX, int startY, int endX, int endY, params int[] expectedPath)
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            var path = LoadTuples(expectedPath);
            PathToNearestTarget(environment, (startX, startY), (endX, endY)).Should().BeEquivalentTo(path, options => options.WithStrictOrdering());
        }

        [Theory]
        [InlineData(2, 3, 2, 2, 2, 5, 2, 1)] // Vertical read order wins
        [InlineData(3, 4, 2, 4, 1, 4, 5, 4)] // Horizontal read order wins
        [InlineData(3, 4, 2, 4, 5, 4, 1, 4)] // Horizontal read order wins, alternative target order
        [InlineData(3, 2, 2, 2, 3, 4, 5, 2, 1, 2)] // Three way tie, read order wins
        [InlineData(3, 3, 3, 4, 3, 1, 3, 4)] // Non-read order is closer
        [InlineData(3, 2, 3, 3, 9, 1, 3, 5)] // First read order target not reachable
        void TestNavigation_MultipleTargets_FirstStep(int startX, int startY, int firstX, int firstY, params int[] targets)
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            var targetList = LoadTuples(targets).ToArray();
            var path = PathToNearestTarget(environment, (startX, startY), targetList);

            path.First().Should().Be((firstX, firstY));
        }

        [Fact]
        void TestNavigation_MultipleTargets_NoPaths()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            var path = PathToNearestTarget(environment, (2, 3), (9, 1), (9, 5));
            path.Count.Should().Be(0);
        }

        [Fact]
        void TestNavigation_ElvesAndGoblinsBlockPath()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            environment[2, 3] = new Entity(EntityType.Elf, (2, 3));
            environment[3, 3] = new Entity(EntityType.Goblin, (3, 3));

            var path = PathToNearestTarget(environment, (4, 2), (2, 4));
            path.First().Should().Be((5, 2));
        }

        [Fact]
        void TestGatherEntities()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-Example1.txt", CharToCellState);
            var entities = GatherEntities(environment);
            var sorted = entities.OrderBy(e => e.Pos.y).ThenBy(e => e.Pos.x).ToArray();

            sorted.Length.Should().Be(4);
            sorted[0].Type.Should().Be(EntityType.Elf);
            sorted[0].Pos.Should().Be((1, 1));
            sorted[1].Type.Should().Be(EntityType.Goblin);
            sorted[1].Pos.Should().Be((4, 1));
            sorted[2].Type.Should().Be(EntityType.Goblin);
            sorted[2].Pos.Should().Be((2, 3));
            sorted[3].Type.Should().Be(EntityType.Goblin);
            sorted[3].Pos.Should().Be((5, 3));
        }

        [Fact]
        void TestGetValidTargets()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-Example1.txt", CharToCellState);
            var entities = GatherEntities(environment);

            var targets = GetValidTargets(environment, entities, EntityType.Elf);
            var sorted = targets.OrderBy(t => t.y).ThenBy(t => t.x).ToArray();

            sorted.Length.Should().Be(6);
            sorted[0].Should().Be((3, 1));
            sorted[1].Should().Be((5, 1));
            sorted[2].Should().Be((2, 2));
            sorted[3].Should().Be((5, 2));
            sorted[4].Should().Be((1, 3));
            sorted[5].Should().Be((3, 3));

            targets = GetValidTargets(environment, entities, EntityType.Goblin);
            sorted = targets.OrderBy(t => t.y).ThenBy(t => t.x).ToArray();

            sorted.Length.Should().Be(2);
            sorted[0].Should().Be((2, 1));
            sorted[1].Should().Be((1, 2));
        }

        [Fact]
        void TestExample1()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-Example1.txt", CharToCellState);
            var entities = GatherEntities(environment);
            var targets = GetValidTargets(environment, entities, EntityType.Elf);
            var path = PathToNearestTarget(environment, (1, 1), targets);

            path.First().Should().Be((2, 1));
        }

        [Fact]
        void TestExample2_Movement()
        {
            var environment = FileIterator.LoadGrid("Data/Day15-Example2.txt", CharToCellState);
            var entities = GatherEntities(environment);

            Step(environment, entities);
            Step(environment, entities);
            Step(environment, entities);

            var sorted = entities.OrderBy(e => e.Pos.y).ThenBy(e => e.Pos.x).ToArray();
            sorted.Count().Should().Be(9);
            sorted[0].Type.Should().Be(EntityType.Goblin);
            sorted[0].Pos.Should().Be((3, 2));
            sorted[1].Type.Should().Be(EntityType.Goblin);
            sorted[1].Pos.Should().Be((4, 2));
            sorted[2].Type.Should().Be(EntityType.Goblin);
            sorted[2].Pos.Should().Be((5, 2));
            sorted[3].Type.Should().Be(EntityType.Goblin);
            sorted[3].Pos.Should().Be((3, 3));
            sorted[4].Type.Should().Be(EntityType.Elf);
            sorted[4].Pos.Should().Be((4, 3));
            sorted[5].Type.Should().Be(EntityType.Goblin);
            sorted[5].Pos.Should().Be((5, 3));
            sorted[6].Type.Should().Be(EntityType.Goblin);
            sorted[6].Pos.Should().Be((1, 4));
            sorted[7].Type.Should().Be(EntityType.Goblin);
            sorted[7].Pos.Should().Be((4, 4));
            sorted[8].Type.Should().Be(EntityType.Goblin);
            sorted[8].Pos.Should().Be((7, 5));
        }

        [Theory]
        [InlineData(27730, "Data/Day15-Battle1.txt")]
        [InlineData(36334, "Data/Day15-Battle2.txt")]
        [InlineData(39514, "Data/Day15-Battle3.txt")]
        [InlineData(27755, "Data/Day15-Battle4.txt")]
        [InlineData(28944, "Data/Day15-Battle5.txt")]
        [InlineData(18740, "Data/Day15-Battle6.txt")]
        [InlineData(243390, "Data/Day15.txt")] // Solution
        void Problem1(int expectedOutcome, string inputFile)
        {
            var environment = FileIterator.LoadGrid(inputFile, CharToCellState);
            var entities = GatherEntities(environment);

            var round = 0;
            while (Step(environment, entities))
                round++;

            var totalHealth = entities.Select(e => e.Health).Sum();
            (totalHealth * round).Should().Be(expectedOutcome);
        }

        [Fact]
        void Problem2()
        {
            var environment = FileIterator.LoadGrid("Data/Day15.txt", CharToCellState);
            var entities = GatherEntities(environment);
            var numElves = 0;
            foreach (var entity in entities)
            {
                if (entity.Type == EntityType.Elf)
                {
                    numElves++;
                    entity.Power = 13;
                }
            }

            var round = 0;
            while (Step(environment, entities))
                round++;

            entities.Where(e => e.Type == EntityType.Elf).Count().Should().Be(numElves);
            var totalHealth = entities.Select(e => e.Health).Sum();
            (totalHealth * round).Should().Be(59886);
        }
    }
}
