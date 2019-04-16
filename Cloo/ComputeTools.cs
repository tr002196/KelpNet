﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Cloo.Bindings;

namespace Cloo
{
    public class ComputeTools
    {
        public static Version ParseVersionString(String versionString, int substringIndex)
        {
            string[] verstring = versionString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new Version(verstring[substringIndex]);
        }

        internal static IntPtr[] ConvertArray(long[] array)
        {
            if (array == null) return null;

            NumberFormatInfo nfi = new NumberFormatInfo();

            IntPtr[] result = new IntPtr[array.Length];
            for (long i = 0; i < array.Length; i++)
                result[i] = new IntPtr(array[i]);
            return result;
        }

        internal static long[] ConvertArray(IntPtr[] array)
        {
            if (array == null) return null;

            NumberFormatInfo nfi = new NumberFormatInfo();

            long[] result = new long[array.Length];
            for (long i = 0; i < array.Length; i++)
                result[i] = array[i].ToInt64();
            return result;
        }

        internal static CLDeviceHandle[] ExtractHandles(ICollection<ComputeDevice> computeObjects, out int handleCount)
        {
            if (computeObjects == null || computeObjects.Count == 0)
            {
                handleCount = 0;
                return null;
            }

            CLDeviceHandle[] result = new CLDeviceHandle[computeObjects.Count];
            int i = 0;
            foreach (ComputeDevice computeObj in computeObjects)
            {
                result[i] = computeObj.Handle;
                i++;
            }
            handleCount = computeObjects.Count;
            return result;
        }

        internal static CLEventHandle[] ExtractHandles(ICollection<ComputeEventBase> computeObjects, out int handleCount)
        {
            if (computeObjects == null || computeObjects.Count == 0)
            {
                handleCount = 0;
                return null;
            }

            CLEventHandle[] result = new CLEventHandle[computeObjects.Count];
            int i = 0;
            foreach (ComputeEventBase computeObj in computeObjects)
            {
                result[i] = computeObj.Handle;
                i++;
            }
            handleCount = computeObjects.Count;
            return result;
        }
    }
}