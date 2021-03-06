﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using KelpNet.CL.Common;
using KelpNet.CL.Properties;

namespace KelpNet.CL
{
    [DataContract(Name = "ReLU", Namespace = "KelpNet")]
    public class ReLU : CPU.ReLU, ICompressibleActivation
    {
        public string FunctionName => "ReLU";
        public string KernelSource => OpenCL.GetKernelSource(Resources.ReLU);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ComputeKernel ForwardKernel { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ComputeKernel BackwardKernel { get; set; }

        [DataMember]
        public KeyValuePair<string, string>[] ActivationParameters { get; set; }

        [DataMember]
        public string ActivateKernelString { get; set; }

        [DataMember]
        public string ForwardKernelName { get; set; }

        [DataMember]
        public string BackwardKernelName { get; set; }

        [DataMember]
        public bool IsParallel { get; set; }


        bool IParallelizable.SetParallel(bool enable)
        {
            return this.SetParallel(enable);
        }

        public ReLU(string name = "ReLU", string[] inputNames = null, string[] outputNames = null, bool gpuEnable = false) : base(name, inputNames, outputNames)
        {
            this.SetParallel(gpuEnable);
        }

        public ReLU(CPU.ReLU reLU) : base(reLU.Name, reLU.InputNames, reLU.OutputNames)
        {
            this.SetParallel(true);
        }

        public void InitParam()
        {
        }

        public override NdArray SingleInputForward(NdArray x)
        {
            return IsParallel ? this.NeedPreviousForwardGpu(x) : base.SingleInputForward(x);
        }

        public override void SingleOutputBackward(NdArray y, NdArray x)
        {
            if (IsParallel)
            {
                this.NeedPreviousBackwardGpu(y, x);
            }
            else
            {
                base.SingleOutputBackward(y, x);
            }
        }
    }
}
