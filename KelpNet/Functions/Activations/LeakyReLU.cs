﻿using System;
using System.Linq;
using Cloo;
using KelpNet.Common;
using KelpNet.Common.Functions;

namespace KelpNet.Functions.Activations
{
    [Serializable]
    public class LeakyReLU : NeedPreviousOutputFunction
    {
        private readonly double _slope;

        public LeakyReLU(double slope = 0.2, string name = "LeakyReLU") : base(name)
        {
            this._slope = slope;

            //カーネルを作成
            if (IsGpu)
            {
                ForwardKernel = Weaver.CreateKernel(ForwardKernelSource, "LeakyReLUForward");
                //BackwardKernel = Weaver.CreateKernel(BackwardKernelSource, "ReLUBackward");
            }
        }

        const string ForwardKernelSource =
@"
#pragma OPENCL EXTENSION cl_khr_fp64 : enable
__kernel void LeakyReLUForward(
	         const double slope,
	__global double *gpuY)
{
	int i = get_global_id(0);

    if(gpuY[i] < 0.0)
    {
        gpuY[i] *= slope;
    }
}";

        protected override BatchArray NeedPreviousForward(BatchArray x)
        {
            double[] y = x.Data.ToArray();

            if (!IsGpu) {
                for (int i = 0; i < x.Data.Length; i++)
                {
                    if (y[i] < 0) y[i] *= this._slope;
                }
            }
            else
            {

                using (ComputeBuffer<double> gpuY = new ComputeBuffer<double>(Weaver.Context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.CopyHostPointer, y))
                {
                    ForwardKernel.SetValueArgument(0, this._slope);
                    ForwardKernel.SetMemoryArgument(1, gpuY);

                    Weaver.CommandQueue.Execute
                        (
                            ForwardKernel,
                            null,
                            new long[] { x.Data.Length },
                            null,
                            null
                        );

                    Weaver.CommandQueue.Finish();
                    Weaver.CommandQueue.ReadFromBuffer(gpuY, ref y, true, null);                    
                }
            }

            return BatchArray.Convert(y, x.Shape, x.BatchCount);
        }

        protected override BatchArray NeedPreviousBackward(BatchArray gy, BatchArray prevOutput)
        {
            double[] gx = new double[gy.Data.Length];

            for (int i = 0; i < gy.Data.Length; i++)
            {
                gx[i] = prevOutput.Data[i] > 0 ? gy.Data[i] : prevOutput.Data[i] * this._slope;
            }

            return BatchArray.Convert(gx, gy.Shape, gy.BatchCount);
        }
    }
}
