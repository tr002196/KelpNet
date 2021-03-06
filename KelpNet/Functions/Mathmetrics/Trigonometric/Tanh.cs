﻿using System;

namespace KelpNet
{
    public class Tanh : SingleInputFunction
    {
        private const string FUNCTION_NAME = "Tanh";

        public Tanh(string name = FUNCTION_NAME, string[] inputNames = null, string[] outputNames = null) : base(name, inputNames, outputNames)
        {
        }

        public override NdArray SingleInputForward(NdArray x)
        {
            Real[] resultData = new Real[x.Data.Length];

            for (int i = 0; i < resultData.Length; i++)
            {
                resultData[i] = (Real)Math.Tanh(x.Data[i]);
            }

            return new NdArray(resultData, x.Shape, x.BatchCount, this);
        }

        public override void SingleOutputBackward(NdArray y, NdArray x)
        {
            for (int i = 0; i < y.Grad.Length; i++)
            {
                x.Grad[i] += y.Grad[i] * (1 - y.Data[i] * y.Data[i]);
            }
        }
    }
}
