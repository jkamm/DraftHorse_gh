using System;
using Grasshopper.Kernel;
using CurveComponents;
using System.Net.Mail;

namespace DraftHorse.Params
{
    /*
    Goal is to generate DetailView as a Param object
    */

    public class Param_View : Make2DViewParameter
    {
        public Param_View() :
            base("View", "V", "View Projection (define using Make2d View Projection components)", "Drafthorse", "Detail", GH_ParamAccess.item)
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            Menu_AppendWireDisplay(menu);
            Menu_AppendDisconnectWires(menu);
            Menu_AppendPrincipalParameter(menu);
            Menu_AppendReverseParameter(menu);
            Menu_AppendGraftParameter(menu);
            Menu_AppendFlattenParameter(menu);
            Menu_AppendSimplifyParameter(menu);
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

