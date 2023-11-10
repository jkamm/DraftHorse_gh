using Rhino.DocObjects;
using Rhino.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;

namespace DraftHorse.Helper
{
    internal class View
    {
        public static bool ViewportInfoToRhinoViewport(ViewportInfo vpInfo, RhinoViewport rhViewport, double lensLength = 50)
        {
            if (vpInfo.IsValidCamera && rhViewport.IsValidCamera )
            {
                bool isSymmetric = vpInfo.IsFrustumLeftRightSymmetric && vpInfo.IsFrustumTopBottomSymmetric;
                //Update Projection
                if (vpInfo.IsParallelProjection && !rhViewport.IsParallelProjection)
                {
                    rhViewport.ChangeToParallelProjection(isSymmetric);
                }
                else if (vpInfo.IsPerspectiveProjection && !rhViewport.IsPerspectiveProjection)
                {
                    rhViewport.ChangeToPerspectiveProjection(isSymmetric, lensLength); //revisit LensLength input as param?
                }
                else if (vpInfo.IsTwoPointPerspectiveProjection && !rhViewport.IsTwoPointPerspectiveProjection)
                {
                    rhViewport.ChangeToTwoPointPerspectiveProjection(lensLength);
                }

                //Update Camera
                rhViewport.SetCameraLocation(vpInfo.CameraLocation, true);
                rhViewport.SetCameraDirection(vpInfo.CameraDirection, true);
                rhViewport.SetCameraTarget(vpInfo.TargetPoint, false);

                //Update Scale?
                
                return true;
            }
            else return false;
        }
    }
}
