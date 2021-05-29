using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Util
{
    public static class Array
    {
        public static int Width<T>(this T[,] arr) => arr.GetLength(0);
        public static int Height<T>(this T[,] arr) => arr.GetLength(1);
        public static T[] Flatten<T>(this T[,] inp) => inp.Cast<T>().ToArray();

        public static T[,] To2D<T>(this T[] inp, int w, int h)
        {
            if (inp.Length != w * h) throw new ArgumentException("Input array could not fit the dimensions provided.");
            
            var ret = new T[w, h];
            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    ret[x, y] = inp[x * h + y];
                }
            }
            return ret;
        }
        
        public static void Write<T>(this DarkRiftWriter writer, T[,] arr) where T : IDarkRiftSerializable
        {
            writer.Write(arr.GetLength(0));
            writer.Write(arr.GetLength(1));
            writer.Write(arr.Flatten());
        }
        
        public static void Write(this DarkRiftWriter writer, sbyte[,] arr)
        {
            writer.Write(arr.GetLength(0));
            writer.Write(arr.GetLength(1));
            writer.Write(arr.Flatten());
        }
        
        public static T[,] Read2D<T>(this DarkRiftReader reader) where T : IDarkRiftSerializable, new()
        {
            var w = reader.ReadInt32();
            var h = reader.ReadInt32();
            return reader.ReadSerializables<T>().To2D(w, h);
        }
        
        public static sbyte[,] Read2DSByte(this DarkRiftReader reader)
        {
            var w = reader.ReadInt32();
            var h = reader.ReadInt32();
            return reader.ReadSBytes().To2D(w, h);
        }

        //Flatten, Linq, To2D, return
        public static TRes[,] Select<TArr, TRes>(this TArr[,] arr, Func<TArr, TRes> func)
        {
            return arr.Flatten().Select(func).ToArray().To2D(arr.GetLength(0), arr.GetLength(1));
        }

        public static void For<TArr>(this TArr[,] arr, Action<TArr, int, int> func)
        {
            for (var x = 0; x < arr.Width(); x++)
            {
                for (var y = 0; y < arr.Height(); y++)
                {
                    func(arr[x, y], x, y);
                }
            }
        }

        public static void Fill<T>(this T[,] arr, Func<T> val)
        {
            for (var x = 0; x < arr.Width(); x++)
            {
                for (var y = 0; y < arr.Height(); y++)
                {
                    arr[x, y] = val();
                }
            }
        }
    }
}
