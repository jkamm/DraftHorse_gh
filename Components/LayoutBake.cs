using Grasshopper.Kernel;
using System;

namespace DraftHorse.Component
{
    public class LayoutBake : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the LayoutBake class.
        /// </summary>
        public LayoutBake()
          : base("LayoutBake", "LOBake",
              "Bake to Rhino Layouts",
              "DraftHorse", "Layouts")
        {
            ButtonName = "Bake";
        }

        //while in testing, set component to Hidden    
        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //geo
            pManager.AddGenericParameter("Geometry", "G", "Geometry to bake", GH_ParamAccess.tree);
            //attributes - use Elefront attributes?
            //layoutIndex matching geo. or add to attributes
            //BakeName
            pManager.AddTextParameter("Bakename", "N", "Replace Existing Geometry with this name", GH_ParamAccess.item);
            Params.Input[1].Optional = true;
            //Bake bool
            var bButtonParam = new DraftHorse.Params.Param_BooleanButton();
            pManager.AddParameter(bButtonParam, "Bake", "B", "Bake Geometry to Rhino", GH_ParamAccess.item);
            Params.Input[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //Done
            pManager.AddBooleanParameter("Done", "D", "State of Baking Operation", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Bake component for zahner process
            //implement of python code
            //set output bool
            bool bake = false;
            DA.GetData("Bake", ref bake);

            var GeoTree = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_GeometricGooWrapper>();
            if (!DA.GetDataTree("Geometry", out GeoTree)) return;



            bool Done = false;

            if (bake || Execute)
            {
                //Variables
                int groupIndex = -1; //only for group not implemented
                                     //if(groupListTgther)
                                     //  groupIndex = doc.Groups.Add();

                //set output bool
                Done = false;

                /*
                for (int i = 0; i < Geo.Count; i++)
                {
                    var obj = Geo;

                    if (obj == null)
                    {
                        //Print("No objects to bake");
                        return;
                    }

                    //Delete bake name objects
                    string bakeNm = bakeName.Count > 0 ? bakeName[i % bakeName.Count] : null;

                    if (!string.IsNullOrEmpty(bakeNm))
                    {
                        Rhino.DocObjects.Tables.ObjectTable search = Rhino.RhinoDoc.ActiveDoc.Objects;
                        var del = search.FindByUserString("BakeName", bakeNm, false);
                        if (del.Length > 0)
                        {
                            foreach (var q in del)
                            {
                                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(q, false);
                            }
                        }
                    }

                    //Make new attributes to set name
                    Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes();

                    //check names for any null names and create a string variable
                    string name = Name.Count > 0 ? Name[i % Name.Count] : null;

                    //Set object name
                    if (!string.IsNullOrEmpty(name))
                    {
                        att.Name = name;
                    }
                    //att.Name = Name;

                    //check keyvalue for any null names and create a string variable
                    string key = Key.Count > 0 ? Key[i % Key.Count] : null;
                    string values = Value.Count > 0 ? Value[i % Value.Count] : null;
                    string bkNm = bakeName.Count > 0 ? bakeName[i % bakeName.Count] : null;

                    //Set object name
                    if (!string.IsNullOrEmpty(key))
                    {
                        att.SetUserString(key, values);
                    }

                    if (!string.IsNullOrEmpty(bkNm))
                    {
                        att.SetUserString("BakeName", bkNm);
                    }

                    //        att.SetUserString(key, values);
                    //        att.SetUserString("BakeName", bkNm);

                    //validate layers
                    string layers = Layer.Count > 0 ? Layer[i % Layer.Count] : null;
                    //Done = layers;

                    //set layer
                    if (!string.IsNullOrEmpty(layers) && Rhino.DocObjects.Layer.IsValidName(layers))
                    {

                        //Get the current layer index
                        Rhino.DocObjects.Tables.LayerTable layerTable = doc.Layers;

                        int layerIndex = layerTable.FindByFullPath(layers, -1);

                        if (layerIndex < 0) //This layer does not exist, we add it
                        {
                            Rhino.DocObjects.Layer onlayer = new Rhino.DocObjects.Layer(); //Make a new layer
                            onlayer.Name = layers;

                            layerIndex = layerTable.Add(onlayer); //Add the layer to the layer table
                            if (layerIndex > -1) //We managed to add layer!
                            {
                                att.LayerIndex = layerIndex;
                                Print("Added new layer to the document at position " + layerIndex + " named " + layers + ". ");
                            }
                            else
                                Print("Layer did not add. Try cleaning up your layers."); //This never happened to me.
                        }
                        else
                            att.LayerIndex = layerIndex; //We simply add to the existing layer

                    }

                    //validate space
                    string space = Space.Count > 0 ? Space[i % Space.Count] : null;

                    if (!string.IsNullOrEmpty(space))
                    {
                        //get the index of the layout
                        var pageViews = RhinoDoc.ActiveDoc.Views.GetPageViews();

                        Rhino.Display.RhinoPageView[] pages = Array.FindAll(pageViews, (x) => (x.PageName == space));

                        Rhino.Display.RhinoPageView page = pages.Length > 0 ? pages[0] : null;

                        if (page != null)
                        {
                            //need some exception handling in case the find doesn't return anything useful.

                            att.Space = Rhino.DocObjects.ActiveSpace.PageSpace;

                            att.ViewportId = page.MainViewport.Id; //Need the viewport Id, but don't know how to get this from a pageview.
                        }
                    }


                    Bake(Geo, att, groupIndex);

                    //set output bool
                    Done = true;
                    

                }
            }
            else
            {
                Print("Inactive");
            }
            */
            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("814259c2-8c76-4d2d-9e81-3d4ad4c28c7f"); }
        }
    }
}