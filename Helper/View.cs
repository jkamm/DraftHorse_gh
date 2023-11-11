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
    }
}
