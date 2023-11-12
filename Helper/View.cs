using Rhino.DocObjects;
using Rhino.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;
using Rhino.Geometry;
using Rhino.UI.Controls;

namespace DraftHorse.Helper
{
    internal class View
    {
        public static ViewportInfo ParallelViewFromRec(Rectangle3d target)
        {
            ViewportInfo vpInfo = new ViewportInfo();
            vpInfo.ChangeToParallelProjection(true);
            vpInfo.SetCameraLocation(target.Center);
            Plane plane = target.Plane;
            Vector3d upVec= plane.ZAxis * -1;
            vpInfo.SetCameraDirection(upVec);
            vpInfo.SetCameraUp(plane.YAxis);
            vpInfo.SetFrustum(-0.5 * target.Width, 0.5 * target.Width, -0.5 * target.Height, 0.5 * target.Height, 0.001, 0.5 * target.Circumference);
            vpInfo.SetScreenPort(0, 600, 0, 400, 0, 100);
            return vpInfo;
        }

        public static bool ViewportInfoToDetailView(ViewportInfo vpInfo, RhinoViewport detailView)
        {
            if (vpInfo.IsValidCamera &&  detailView.IsValidCamera) 
            {
                if (vpInfo.IsParallelProjection && !detailView.IsParallelProjection)
                    detailView.ChangeToParallelProjection(true);
                else if (vpInfo.IsPerspectiveProjection && !detailView.IsPerspectiveProjection)
                    detailView.ChangeToPerspectiveProjection(true, 50);
                else if (vpInfo.IsTwoPointPerspectiveProjection && !detailView.IsTwoPointPerspectiveProjection)
                    detailView.ChangeToTwoPointPerspectiveProjection(50);
                
                detailView.CameraUp = vpInfo.CameraUp;
                detailView.SetCameraTarget(vpInfo.TargetPoint, false);
                detailView.SetCameraDirection(vpInfo.CameraDirection,false);
                detailView.SetCameraLocation(vpInfo.CameraLocation, false);
                
                return true;
            }
            return false;
        }
    }
}
