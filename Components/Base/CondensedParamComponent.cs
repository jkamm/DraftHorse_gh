using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using static DraftHorse.Helper.XParams;



namespace DraftHorse.Component.Base
{

    public abstract class CondensedParamComponent : GH_Component, IGH_VariableParameterComponent
    //make this an abstract class so it is not instantiated?
    {
        public int MinInputs { get; set; }
        public int MinOutputs { get; set; }
        protected List<object[]> XParamInputs { get; set; }
        protected List<object[]> XParamOutputs { get; set; }

        //public override GH_Exposure Exposure => GH_Exposure.hidden;

        #region Methods of GH_Component interface

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CondensedParamComponent()
            : base("VariableParameterTest", "Nickname",
                "Description",
                "DraftHorse", "Test")
        {
            MinInputs = 2;  //Drive this off the list of inputs in the RegisterInputs command?
            MinOutputs = 1;

            XParamInputs = new List<object[]>  //this could be a method of the interface for the condensedParamsComponent
            {
                GenerateNumberParam("Number", "N", "generated number parameter", GH_ParamAccess.item),
                GenerateNumberParam("Number", "N2", "generated Next number parameter", GH_ParamAccess.item),
                GenerateIntegerParam("Integer", "I", "generated integer", GH_ParamAccess.item)
            };
            XParamOutputs = new List<object[]>()
            {
                GenerateNumberParam("Num2Out", "N2o", "generated Next number parameter", GH_ParamAccess.item),
                GenerateIntegerParam("IntOut", "Io", "generated integer", GH_ParamAccess.item),
                GenerateStringParam("Out", "Out", "Error messages", GH_ParamAccess.list)
            };
        }

        public CondensedParamComponent(string name, string nickname, string description, string category, string subcategory)
        : base(name, nickname,
                description,
                category, subcategory)
        {
            MinInputs = 0;
            MinOutputs = 0;

            XParamInputs = new List<object[]>
            {
                //use "Generate<Type>ParamArray" methods from XParams to populate list of extra parameters
            };
            XParamOutputs = new List<object[]>()
            {
                //use "Generate<Type>ParamArray" methods from XParams to populate list of extra parameters
            };
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("First", "1", "First input (static)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Second", "2", "Second input (static)", GH_ParamAccess.item);
            //MinInputs = Params.Input.Count;  I don't think this list is updated until after this command runs.
            //should do this action in VariableParamMaintenance
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("FirstOut", "1o", "First output (static)", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int outputparamno = 0;   //these are not being used.  Not sure exactly what they do.
            int inputparamno = 0;

            if (!DA.GetData(0, ref inputparamno)) { return; }
            if (!DA.GetData(1, ref outputparamno)) { return; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{fdc25843-e26e-4c2d-b985-fd7fc13ed282}"); }
        }

        #endregion Methods of GH_Component interface

        #region Condensed Input Methods




        #endregion Condensed Input Methods

        #region Methods of IGH_VariableParameterComponent interface

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            //can insert only at the bottom of the component
            if (side == GH_ParameterSide.Input)
            {
                if (index == Params.Input.Count && index < XParamInputs.Count + MinInputs)
                    return true;
                else return false;
            }
            else
            {
                if (index == Params.Output.Count && index < XParamOutputs.Count + MinOutputs)
                    return true;
                else return false;
            }
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            //leave two inputs
            if (side == GH_ParameterSide.Input)
            {
                if (Params.Input.Count > MinInputs && index == Params.Input.Count - 1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (Params.Output.Count > MinOutputs && index == Params.Output.Count - 1)
                    return true;
                else
                    return false;
            }


        }

        //goal:
        //Automatically populate the list of conditions in the Create Parameter class - use a list of lists or something like it
        //If the list is of objects, does it destroy the properties of the object?  I don't think so.
        //Can populate a List of Arrays.  The arrays will have the appropriate entries
        //A method for adding entries to the list will require the inputs.  Can call the type using a struct?  Limit the types that can be set
        //Then query the types to retrieve the appropriate class to construct using a switch command?
        //Or make a class of object which has all the attributes of a param without being instantiated
        //Maybe as a list of arrays where the first item is the object and the second item is an identifier of the type.

        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input) return ReturnParam(XParamInputs[index - MinInputs]);
            else return ReturnParam(XParamOutputs[index - MinOutputs]);
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            //Nothing to do here at the moment
            return true;
        }

        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            //MatchParameterCount();
        }

        //public void MatchParameterCount()
        //{
        //    while (outputparamno != Params.Output.Count)
        //    {
        //        if (outputparamno > Params.Output.Count)
        //        {
        //            Params.RegisterOutputParam(new Param_GenericObject());
        //        }
        //        if (outputparamno < Params.Output.Count)
        //        {
        //            Params.UnregisterOutputParameter(Params.Output[Params.Output.Count - 1]);
        //        }
        //    }

        //    this.OnAttributesChanged();

        //    this.ExpireSolution(true);

        //}

        #endregion

    }





}