﻿using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using System;

namespace DraftHorse.Params
{
    public class GH_DetailView : IGH_Goo
    {
        public bool IsValid => throw new NotImplementedException();

        public string IsValidWhyNot => throw new NotImplementedException();

        public string TypeName => throw new NotImplementedException();

        public string TypeDescription => throw new NotImplementedException();

        public bool CastFrom(object source)
        {
            throw new NotImplementedException();
        }

        public bool CastTo<T>(out T target)
        {
            throw new NotImplementedException();
        }

        public IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public IGH_GooProxy EmitProxy()
        {
            throw new NotImplementedException();
        }

        public bool Read(GH_IReader reader)
        {
            throw new NotImplementedException();
        }

        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
