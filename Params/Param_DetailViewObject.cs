using System;
using Grasshopper.Kernel;
using CurveComponents;

namespace DraftHorse.Params
{
    /*
    Goal is to generate DetailView as a Param object
    */

    public class Param_View : Make2DViewParameter
    {
        public Param_View() :
            base("View", "V", "View Settings for Detail Viewports", "Drafthorse", "Detail", GH_ParamAccess.item)
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            //Grasshopper.Kernel.GH_PersistentParam<GH_String>.Menu_
            //base.AppendAdditionalMenuItems(menu);
            //AppendAdditionalMenuItems(menu);
            Menu_AppendWireDisplay(menu);
            Menu_AppendDisconnectWires(menu);
            //Menu_AppendSeparator(menu);
            //Menu_CreateMultilineTextEditItem();
            //Menu_CustomSingleValueItem();
            //Menu_CustomSingleDirectoryItem();
            //Menu_AppendCustomItem(menu,System.Windows.Forms.Control.)
            //Menu_AppendManageCollection(menu);
            //Menu_AppendSeparator(menu);
            //Menu_AppendDestroyPersistent(menu);
            //Menu_AppendInternaliseData(menu);
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("1ba581c1-3ab7-45cc-af15-d30a98f6ab01"); }
        }

    }
}


    /*
    public class Param_DetailViewObject : GH_Param<GH_DetailView>
    {
        public Param_DetailViewObject()
        { }

        public Param_DetailViewObject(Rhino.Geometry.DetailView)
        {

        }
        
    
        
        

    }
    */

