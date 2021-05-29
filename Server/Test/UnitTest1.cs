using NUnit.Framework;
using Common.Util;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void FlattenTo2D()
        {
            var arr = new byte[,] {{1, 2}, {3, 4}, {5, 6}};
            var arr2 = ((byte[,]) arr.Clone()).Flatten().To2D(arr.GetLength(0), arr.GetLength(1));
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] != arr2[x, y]) Assert.Fail();
                }
            }
        }
    }
}