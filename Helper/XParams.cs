using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace DraftHorse.Helper
{
    public static class XParams
    {
        //Intent of this class is to be able to record and return parameter objects of variable type from a single list.
        //goal: add overload to set default properties use "@default" as variable name

        //Methods
        public static object[] GenerateNumberParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Number thisParam = new Param_Number
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Number" };

            return array;
        }
        public static object[] GenerateNumberParam(string name, string nickname, string description, GH_ParamAccess access, double @default)
        {
            Param_Number thisParam = new Param_Number
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Number" };

            return array;
        }
        public static object[] GenerateIntegerParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Integer thisParam = new Param_Integer
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Integer" };

            return array;
        }
        public static object[] GenerateIntegerParam(string name, string nickname, string description, GH_ParamAccess access, int @default)
        {
            Param_Integer thisParam = new Param_Integer
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Integer" };

            return array;
        }
        public static object[] GenerateIntegerParam(string name, string nickname, string description, GH_ParamAccess access, List<int> @default)
        {
            Param_Integer thisParam = new Param_Integer
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Integer" };

            return array;
        }
        public static object[] GenerateCurveParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Curve thisParam = new Param_Curve
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Curve" };

            return array;
        }
        public static object[] GenerateCurveParam(string name, string nickname, string description, GH_ParamAccess access, Curve @default)
        {
            Param_Curve thisParam = new Param_Curve
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Curve" };

            return array;
        }
        public static object[] GenerateGenericParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_GenericObject thisParam = new Param_GenericObject
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Generic" };

            return array;
        }
        public static object[] GenerateStringParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_String thisParam = new Param_String
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "String" };

            return array;
        }
        public static object[] GenerateStringParam(string name, string nickname, string description, GH_ParamAccess access, string @default)
        {
            Param_String thisParam = new Param_String
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "String" };

            return array;
        }
        public static object[] GenerateGeometryParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Geometry thisParam = new Param_Geometry
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Geometry" };

            return array;
        }
        public static object[] GenerateGeometryParam(string name, string nickname, string description, GH_ParamAccess access, GeometryBase @default)
        {
            Param_Geometry thisParam = new Param_Geometry
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Geometry" };

            return array;
        }
        public static object[] GeneratePlaneParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Plane thisParam = new Param_Plane
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Plane" };

            return array;
        }
        public static object[] GeneratePlaneParam(string name, string nickname, string description, GH_ParamAccess access, Plane @default)
        {
            Param_Plane thisParam = new Param_Plane
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Plane" };

            return array;
        }
        public static object[] GeneratePointParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Point thisParam = new Param_Point
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Point" };

            return array;
        }
        public static object[] GeneratePointParam(string name, string nickname, string description, GH_ParamAccess access, Point3d @default)
        {
            Param_Point thisParam = new Param_Point
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Point" };

            return array;
        }
        public static object[] GenerateVectorParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Vector thisParam = new Param_Vector
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Vector" };

            return array;
        }
        public static object[] GenerateBooleanParam(string name, string nickname, string description, GH_ParamAccess access)
        {
            Param_Boolean thisParam = new Param_Boolean
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            object[] array = new object[] { thisParam, "Boolean" };

            return array;
        }
        public static object[] GenerateBooleanParam(string name, string nickname, string description, GH_ParamAccess access, bool @default)
        {
            Param_Boolean thisParam = new Param_Boolean
            {
                Name = name,
                NickName = nickname,
                Description = description,
                Access = access,
                Optional = true
            };
            thisParam.SetPersistentData(@default);
            object[] array = new object[] { thisParam, "Boolean" };

            return array;
        }


        /*
         * To add:
         * AngleParam
         * RectangleParam
         * Arc
         
         * Box
         * Brep
         * Circle
         * Colour
         * Field
         * Line
         * Interval2D
         * Matrix
         * MeshFace
         * Mesh
         * Text (?)
         * Time
         * Transform
         */

        public static IGH_Param ReturnParam(object[] thisArray)
        {
            switch (thisArray[1])
            {
                case "Number":
                    return thisArray[0] as Param_Number;
                case "Integer":
                    return thisArray[0] as Param_Integer;
                case "String":
                    return thisArray[0] as Param_String;
                case "Interval":
                    return thisArray[0] as Param_Interval;
                case "Geometry":
                    return thisArray[0] as Param_Geometry;
                case "Generic":
                    return thisArray[0] as Param_GenericObject;
                case "Curve":
                    return thisArray[0] as Param_Curve;
                case "Plane":
                    return thisArray[0] as Param_Plane;
                case "Point":
                    return thisArray[0] as Param_Point;
                case "Vector":
                    return thisArray[0] as Param_Vector;
                case "Boolean":
                    return thisArray[0] as Param_Boolean;

                default:
                    throw new NotImplementedException("this type is not supported by this function");
            }
        }
    }
}
