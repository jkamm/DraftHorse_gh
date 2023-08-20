using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace DraftHorse.Helper
{
    public class Baking
    {
        public static void Bake(object obj, Rhino.DocObjects.ObjectAttributes att, int groupIndex)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc; 

            if (obj == null)
                return;

            //If there's a defined ViewportId, make that Viewport Active
            if (att.ViewportId != null)
            {
                //goal: add a check to make sure that ViewPort exists?
                doc.Views.ActiveView = doc.Views.Find(att.ViewportId);
            }
            // otherwise set the view to a standard view
            else if (!doc.Views.ModelSpaceIsActive)
            {
               var standardViews = doc.Views.GetStandardRhinoViews();
               doc.Views.ActiveView = standardViews[0];
            }

            //variables
            Guid id;

            //Print(obj.GetType().To);

            //Bake to the right type of object
            if (obj is GeometryBase)
            {
                GeometryBase geomObj = obj as GeometryBase;


                switch (geomObj.ObjectType)
                {
                    case Rhino.DocObjects.ObjectType.Brep:
                        id = doc.Objects.AddBrep(obj as Brep, att);
                        break;
                    case Rhino.DocObjects.ObjectType.Curve:
                        id = doc.Objects.AddCurve(obj as Curve, att);
                        break;
                    case Rhino.DocObjects.ObjectType.Point:
                        id = doc.Objects.AddPoint((obj as Rhino.Geometry.Point).Location, att);
                        break;
                    case Rhino.DocObjects.ObjectType.Surface:
                        id = doc.Objects.AddSurface(obj as Surface, att);
                        break;
                    case Rhino.DocObjects.ObjectType.Mesh:
                        id = doc.Objects.AddMesh(obj as Mesh, att);
                        break;
                    case (Rhino.DocObjects.ObjectType)1073741824://Rhino.DocObjects.ObjectType.Extrusion:
                        id = (Guid)typeof(Rhino.DocObjects.Tables.ObjectTable).InvokeMember("AddExtrusion", BindingFlags.Instance | BindingFlags.InvokeMethod, null, doc.Objects, new object[] { obj, att });
                        break;
                    case Rhino.DocObjects.ObjectType.PointSet:
                        id = doc.Objects.AddPointCloud(obj as Rhino.Geometry.PointCloud, att); //This is a speculative entry
                        break;

                    case Rhino.DocObjects.ObjectType.InstanceDefinition:
                        //InstanceDefinitionGeometry instanceDef = geomObj as InstanceDefinitionGeometry;
                        //Print("it's a definition");
                        //int ind = RhinoDocument.InstanceDefinitions.InstanceDefinitionIndex(instanceReference.ParentIdefId, true);
                        //id = doc.Objects.AddInstanceObject(ind, instanceReference.Xform, att);
                        break;


                    case Rhino.DocObjects.ObjectType.InstanceReference:
                        InstanceReferenceGeometry instanceReference = geomObj as InstanceReferenceGeometry;
                        int ind = doc.InstanceDefinitions.InstanceDefinitionIndex(instanceReference.ParentIdefId, true);
                        id = doc.Objects.AddInstanceObject(ind, instanceReference.Xform, att);
                        break;


                    case Rhino.DocObjects.ObjectType.Annotation:
                        //Print("this is text!");
                        TextEntity textObjs = geomObj as TextEntity;
                        //Print(textObjs.RichText);
                        string txt = textObjs.PlainText;
                        Rhino.Geometry.Plane pln = textObjs.Plane;
                        Rhino.Geometry.TextJustification txtJust = textObjs.Justification;
                        double H = textObjs.TextHeight;
                        string fN = textObjs.Font.RichTextFontName;
                        id = doc.Objects.AddText(txt, pln, H, fN, false, false, txtJust, att);

                        break;

                    default:
                        //Print("The script does not know how to handle this type of geometry: " + obj.GetType().FullName);
                        return;

                }
            }
            else
            {
                Type objectType = obj.GetType();

                if (objectType == typeof(Arc))
                {
                    id = doc.Objects.AddArc((Arc)obj, att);
                }
                else if (objectType == typeof(Box))
                {
                    id = doc.Objects.AddBrep(((Box)obj).ToBrep(), att);
                }
                else if (objectType == typeof(Circle))
                {
                    id = doc.Objects.AddCircle((Circle)obj, att);
                }
                else if (objectType == typeof(Ellipse))
                {
                    id = doc.Objects.AddEllipse((Ellipse)obj, att);
                }
                else if (objectType == typeof(Polyline))
                {
                    id = doc.Objects.AddPolyline((Polyline)obj, att);
                }
                else if (objectType == typeof(Sphere))
                {
                    id = doc.Objects.AddSphere((Sphere)obj, att);
                }
                else if (objectType == typeof(Point3d))
                {
                    id = doc.Objects.AddPoint((Point3d)obj, att);
                }
                else if (objectType == typeof(Line))
                {
                    id = doc.Objects.AddLine((Line)obj, att);
                }
                else if (objectType == typeof(Vector3d))
                {
                    //Print("Impossible to bake vectors");
                    return;
                }
                else if (objectType == typeof(Rectangle3d))
                {
                    id = doc.Objects.AddRectangle((Rectangle3d)obj, att);
                }
                else if (objectType == typeof(Tuple<string, Plane>))
                {

                    var dictionary = (Tuple<string, Plane>)obj;

                    string namer = dictionary.Item1;
                    Plane planer = dictionary.Item2;
                    Transform transform = Transform.PlaneToPlane(Rhino.Geometry.Plane.WorldXY, planer);
                    Rhino.DocObjects.InstanceDefinition def = doc.InstanceDefinitions.Find(namer);
                    if (def == null)
                        return;

                    id = doc.Objects.AddInstanceObject(def.Index, transform, att);


                    //Print("its a tuple");


                    return;
                }

                else if (obj is IEnumerable)
                {
                    int newGroupIndex;
                    if (groupIndex == -1)
                        newGroupIndex = doc.Groups.Add();
                    else
                        newGroupIndex = groupIndex;
                    foreach (object o in obj as IEnumerable)
                    {
                        Bake(o, att, newGroupIndex);
                    }
                    return;
                }


                else
                {
                    //Print("Unhandled type: " + objectType.FullName);
                    return;
                }

                if (groupIndex != -1)
                {//possible bake as a group
                    doc.Groups.AddToGroup(groupIndex, id);
                    //Print("Added " + obj.GetType().Name + " to group number " + groupIndex);
                }
                //else
                   //Print("Added " + obj.GetType().Name);
            }

        }

    }
}
