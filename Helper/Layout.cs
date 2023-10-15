using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Render.ChangeQueue;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DraftHorse.Helper
{
    public class Layout
    {
        /// <summary>
        /// Generate a layout with a single detail view that zooms to a list of objects
        /// </summary>

        //No properties currently in use

        #region Properties
        //define class Properties private
        private string template;
        private RhinoPageView page;

        //define Public Properties
        public string Template
        {
            get
            {
                return template;
            }
            set
            {
                template = value;
                //goal: add checking to make sure the template is found in the document
            }
        }
        public string Name { get; set; }
        public RhinoPageView Page
        {
            get
            {
                return page;
            }
            set
            {
                page = Page;
            }
        }
        #endregion

        #region in-process
        //Define constructors - none currently in use
        public Layout(string template, string newName)
        {
            this.Template = template;
            this.Name = newName;
        }

        public Layout(string template)
        {
            this.Template = template;
        }


        //Not being used - 6/14/19
        public void DupThisLayout()
        {
            var page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //Find page matching template name
            int index = Array.FindIndex(page_views, (x) => (x.PageName == this.Template));
            //goal: if not found, Throw exception
            try
            {
                //Duplicate it
                var dup = page_views[index].Duplicate(true);
                this.Page = dup;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("No template found with name: " + this.Template);
            }
        }

        //Not being used - 6/14/19
        public bool CheckforTemplate()
        {
            var page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //Find page matching template name
            int index = Array.FindIndex(page_views, (x) => (x.PageName == this.Template));
            if (index != -1) return true;
            else return false;
            //this doesn't really help unless it returns the index of the template in the pageviews.  
            //Pageviews are still needed to duplicate, so it's repeated work.
            //{
            //    //GH_ActiveObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "no template found matching" + this.Template);
            //}
        }
        #endregion

        public static RhinoPageView DupLayout(string name)
        {
            var page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //Find page matching template name
            int index = Array.FindIndex(page_views, (x) => (x.PageName == name));
            //goal: if not found, Throw exception
            try
            {
                //Duplicate it
                var dup = page_views[index].Duplicate(true);
                return dup;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("No template found with name: " + name);
            }
        }
        public static Rhino.Commands.Result ModifyLayout(DetailViewObject detail, RhinoPageView pageview, double scale, Point3d target, Rhino.RhinoDoc doc)
        {
            if (detail != null)
            {

                pageview.SetActiveDetail(detail.Id);
                detail.DetailGeometry.IsProjectionLocked = false;
                detail.CommitChanges();

                pageview.SetActiveDetail(detail.Id);
                detail.Viewport.ZoomExtents();
                detail.Viewport.SetCameraTarget(target, true);
                // CommitViewPortChanges modifies the Viewport only
                detail.CommitViewportChanges();



                pageview.SetActiveDetail(detail.Id);
                detail.DetailGeometry.IsProjectionLocked = true;
                detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
                // Commit changes tells the document to replace the document's detail object
                // with the modified one that we just adjusted
                detail.CommitChanges();

                pageview.SetPageAsActive();
                doc.Views.ActiveView = pageview;
                doc.Views.Redraw();
                return Rhino.Commands.Result.Success;

            }
            return Rhino.Commands.Result.Failure;
        }

        public static Rhino.Commands.Result ReviseDetails(string detailName, RhinoPageView page, Rectangle3d rec, int modelScale, string namedView)
        {
            //This is garbage

            /* Replace Details
            * - make named detail active (input detailName)
            * - change named view to thisNamedView (input namedView)
            * - point to target
            * - set scale - add as input
            * - check other attributes
            * - deactivate
            * - return Success/Failure
            * - goal: change to qualified success/failure with more info. 
            */

            //get all details 
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            var details = page.GetDetailViews();
            //get each instance of a named detail
            DetailViewObject[] detailsMatching = Array.FindAll(details, (x) => (x.Name == detailName));
            if (detailsMatching.Length == 0) throw new Exception("no details match name: " + detailName);

            //get Named View index
            int nViewIndex = doc.NamedViews.FindByName(namedView);
            if (nViewIndex == -1) throw new Exception("named view not found: " + namedView);

            //generate target for views
            foreach (DetailViewObject matchedDet in detailsMatching)
            {
                page.SetActiveDetail(matchedDet.Id);
                doc.NamedViews.Restore(nViewIndex, matchedDet.Viewport);

                // Zoom bounding box.
                BoundingBox bounds = new BoundingBox(rec.Corner(0), rec.Corner(2));
                matchedDet.Viewport.ZoomBoundingBox(bounds);

                //Set scale of image
                matchedDet.DetailGeometry.SetScale(modelScale, doc.ModelUnitSystem, 1, doc.PageUnitSystem);

                matchedDet.CommitChanges();
                RefreshView(page);
            }

            //get all NamedViews from NamedViewTable

            return Rhino.Commands.Result.Success;
        }

        /// <summary>
        /// Change the Center and scale of a Detail
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="target"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rhino.Commands.Result ReviseDetail(DetailViewObject detail, Point3d target, double scale)
        {
            /* Replace Details
            * - make named detail active (input detailName)
            * - change named view to thisNamedView (input namedView)
            * - point to target
            * - set scale - add as input
            * - check other attributes
            * - deactivate
            * - return Success/Failure
            * - goal: change to qualified success/failure with more info. 
            */

            //get all details 
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            RhinoPageView[] page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            RhinoPageView page = Array.Find(page_views, (x) => x.MainViewport.Id.Equals(detail.Attributes.ViewportId));

            //get Named View index
            //int nViewIndex = doc.NamedViews.FindByName(namedView);
            //if (nViewIndex == -1) throw new Exception("named view not found: " + namedView);
            //need better exception handling here, deliver Result.Failure? 

            page.SetActiveDetail(detail.Id);
            //doc.NamedViews.Restore(nViewIndex, detail.Viewport);
            detail.Viewport.SetCameraTarget(target, true);
            detail.CommitViewportChanges();

            detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
            detail.CommitChanges();

            page.SetPageAsActive();
            doc.Views.ActiveView = page;
            doc.Views.Redraw();
            return Rhino.Commands.Result.Success;
        }
        public static Rhino.Commands.Result ReviseDetail(DetailViewObject detail, Point3d target, double scale, DefinedViewportProjection projection)
        {
            /* Replace Details
            * - make named detail active (input detailName)
            * - change named view to thisNamedView (input namedView)
            * - point to target
            * - set scale - add as input
            * - check other attributes
            * - deactivate
            * - return Success/Failure
            * - goal: change to qualified success/failure with more info. 
            */

            //get all details 
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            RhinoPageView[] page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            RhinoPageView page = Array.Find(page_views, (x) => x.MainViewport.Id.Equals(detail.Attributes.ViewportId));

            /*
            //get Named View index
            int nViewIndex = doc.NamedViews.FindByName(namedView);
            if (nViewIndex == -1) throw new Exception("named view not found: " + namedView);
            //need better exception handling here, deliver Result.Failure? 

            RhinoView sView = doc.Views.Find(namedView, false);
             */

            page.SetActiveDetail(detail.Id);
            if (!projection.Equals(DefinedViewportProjection.None)) detail.Viewport.SetProjection(projection, projection.ToString(), true);
            //doc.NamedViews.Restore(nViewIndex, detail.Viewport);
            detail.Viewport.SetCameraTarget(target, true);
            detail.CommitViewportChanges();

            detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
            detail.CommitChanges();

            page.SetPageAsActive();
            doc.Views.ActiveView = page;
            doc.Views.Redraw();
            return Rhino.Commands.Result.Success;
        }

        public static Rhino.Commands.Result ReviseDetail(DetailViewObject detail, Point3d target, double scale, DefinedViewportProjection projection, DisplayModeDescription displayMode)
        {
            /* Replace Details
            * - make named detail active (input detailName)
            * - change named view to thisNamedView (input namedView)
            * - point to target
            * - set scale - add as input
            * - check other attributes
            * - deactivate
            * - return Success/Failure
            * - goal: change to qualified success/failure with more info. 
            */

            //get all details 
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            RhinoPageView[] page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            RhinoPageView page = Array.Find(page_views, (x) => x.MainViewport.Id.Equals(detail.Attributes.ViewportId));

            /*
            //get Named View index
            int nViewIndex = doc.NamedViews.FindByName(namedView);
            if (nViewIndex == -1) throw new Exception("named view not found: " + namedView);
            //need better exception handling here, deliver Result.Failure? 

            RhinoView sView = doc.Views.Find(namedView, false);
             */

            page.SetActiveDetail(detail.Id);
            if (!projection.Equals(DefinedViewportProjection.None)) detail.Viewport.SetProjection(projection, projection.ToString(), true);
            //doc.NamedViews.Restore(nViewIndex, detail.Viewport);
            detail.Viewport.SetCameraTarget(target, true);

            detail.Viewport.DisplayMode = displayMode;
            detail.CommitViewportChanges();

            detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
            
            detail.CommitChanges();

            page.SetPageAsActive();
            doc.Views.ActiveView = page;
            doc.Views.Redraw();
            return Rhino.Commands.Result.Success;
        }

        public static void RefreshView(RhinoPageView page)
        {
            page.SetPageAsActive();
            RhinoDoc.ActiveDoc.Views.ActiveView = page;
            page.ActiveViewport.ZoomExtents();
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        public static RhinoPageView GetPage(int index)
        {
            var pageViews = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //Find page matching template name
            //int index = Array.FindIndex(page_views, (x) => (x.PageName == name));
            //goal: if not found, Throw exception
            try
            {
                return pageViews[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("No page found at index " + index);
            }
        }

        public static int[] GetPages(string name)
        {
            var page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //Find page matching template name

            RhinoPageView[] pages = Array.FindAll(page_views, (x) => (x.PageName == name));
            int[] indices = new int[pages.Length];
            for (int i = 0; i < pages.Length; i++)
            {
                indices[i] = Array.FindIndex(page_views, (x) => (x.PageNumber == pages[i].PageNumber));
            }
            //goal: if not found, Throw exception
            if (pages.Length != 0)
                return indices;
            else throw new IndexOutOfRangeException("No page found with name: " + name);
        }

        /// <summary>
        /// Replace text value of list of named Text objects on a layout page
        /// </summary>
        /// <param name="textKeys"></param>
        /// <param name="textVals"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static List<Rhino.Commands.Result> ReplaceText(List<string> textKeys, List<string> textVals, RhinoPageView page)
        {
            /* Reset Text
            * -get text objects(from activeDoc)
            * -get names per object to change(input textKeys)
            * - foreach name, find text object(s)
            * -change text to new text(input textValues)
            */

            //Key/Value match tested in Main script body
            List<Rhino.Commands.Result> results = new List<Rhino.Commands.Result>();

            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            for (int i = 0; i < textKeys.Count; i++)
            {
                //find text entities with the corresponding name
                settings.NameFilter = textKeys[i];
                settings.ObjectTypeFilter = ObjectType.Annotation;
                settings.ViewportFilter = page.ActiveViewport;

                var objs = RhinoDoc.ActiveDoc.Objects.GetObjectList(settings);

                foreach (RhinoObject obj in objs)
                {
                    try
                    {
                        TextObject txtObj = obj as TextObject;
                        txtObj.TextGeometry.PlainText = textVals[i];
                        txtObj.CommitChanges();
                    }
                    catch (Exception)
                    {
                        results.Add(Rhino.Commands.Result.Failure);
                        continue;
                    }
                    results.Add(Rhino.Commands.Result.Success);
                }
                //goal: if none found, display message, or just send notification
                //this.GH_RuntimeMessage("Key not found: " + textKeys[i], GH_RuntimeMessageLevel.Warning);
            }
            return results;
        }


        public static List<Grasshopper.Kernel.Types.GH_Guid> GetPageGhGuids(RhinoPageView page)
        {
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            List<Grasshopper.Kernel.Types.GH_Guid> guidList = new List<Grasshopper.Kernel.Types.GH_Guid>();

            settings.ObjectTypeFilter = ObjectType.AnyObject;
            settings.ViewportFilter = page.MainViewport;
            IEnumerable<RhinoObject> objs = RhinoDoc.ActiveDoc.Objects.GetObjectList(settings);

            foreach (RhinoObject obj in objs)
            {
                if (obj.HasId)
                {
                    Grasshopper.Kernel.Types.GH_Guid guid = new Grasshopper.Kernel.Types.GH_Guid();
                    if (!GH_Convert.ToGHGuid_Primary(obj, ref guid))
                        throw new Exception("Conversion failed");
                    guidList.Add(guid);
                }
                //else throw new Exception("has no id");
            }

            return guidList;
        }

        public static List<Guid> GetPageGeoGuids(RhinoPageView page)
        {
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            List<Guid> guidList = new List<Guid>();

            settings.ObjectTypeFilter = ObjectType.AnyObject;
            settings.ViewportFilter = page.MainViewport;
            IEnumerable<RhinoObject> objs = RhinoDoc.ActiveDoc.Objects.GetObjectList(settings);

            foreach (RhinoObject obj in objs)
            {
                guidList.Add(obj.Id);

                //else throw new Exception("has no id");
            }

            return guidList;
        }


        public static IEnumerable<RhinoObject> GetPageObjects(RhinoPageView page)
        {
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings
            {
                ObjectTypeFilter = ObjectType.AnyObject,
                ViewportFilter = page.MainViewport
            };
            IEnumerable<RhinoObject> objs = RhinoDoc.ActiveDoc.Objects.GetObjectList(settings);

            return objs;
        }

        public static List<Grasshopper.Kernel.Types.IGH_GeometricGoo> GetPageGeoGoos(RhinoPageView page)
        {
            var objs = GetPageObjects(page);

            List<Grasshopper.Kernel.Types.IGH_GeometricGoo> gooList = new List<Grasshopper.Kernel.Types.IGH_GeometricGoo>();

            foreach (RhinoObject obj in objs)
            {
                ObjRef objRef = new ObjRef(obj);
                var goo = GH_Convert.ObjRefToGeometry(objRef);
                if (goo != null) gooList.Add(goo);
            }
            return gooList;
        }

        public static void SortPagesByPageNumber(RhinoPageView[] pages)
        {
            PageNumCompare pageCompare = new PageNumCompare();
            Array.Sort(pages, pageCompare);
            //return pages;
        }

        public static List<Grasshopper.Kernel.Types.IGH_Goo> GetPageGoos(RhinoPageView page)
        {
            var objs = GetPageObjects(page);

            List<Grasshopper.Kernel.Types.IGH_Goo> gooList = new List<Grasshopper.Kernel.Types.IGH_Goo>();

            foreach (RhinoObject obj in objs)
            {
                //ObjRef objRef = new ObjRef(obj);
                var goo = GH_Convert.ToGoo(obj);
                if (goo != null) gooList.Add(goo);
            }
            return gooList;
        }

        public static void TestSome()
        {
            //Rhino.DocObjects.Tables.ViewTable viewTable = Rhino.RhinoDoc.ActiveDoc.Views;
            var pageViews = RhinoDoc.ActiveDoc.Views.GetPageViews();
            //pageViews[0].Page
            var namedViews = RhinoDoc.ActiveDoc.Views.GetStandardRhinoViews().ToDictionary(v => v.ActiveViewport.Name, v => v);
            //var viewInfo = new Rhino.DocObjects.ViewInfo()

            var views = RhinoDoc.ActiveDoc.Views.ToDictionary(v => v.ActiveViewport.Name, v => v);


            //RhinoDoc.ActiveDoc.Views.ToDictionary(v => v.ActiveViewport.Name, v => v);
        }

        /// <summary>
        /// Based on code example on RhinoCommon SDK
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pageUnits"></param>
        /// <returns></returns>
        public static Rhino.Commands.Result AddLayout(RhinoDoc doc, UnitSystem pageUnits)
        {
            doc.PageUnitSystem = pageUnits;
            var page_views = doc.Views.GetPageViews();
            int page_number = (page_views == null) ? 1 : page_views.Length + 1;
            var pageview = doc.Views.AddPageView(string.Format("A0_{0}", page_number), 1189, 841);
            if (pageview != null)
            {
                Point2d top_left = new Point2d(20, 821);
                Point2d bottom_right = new Point2d(1169, 20);
                var detail = pageview.AddDetailView("ModelView", top_left, bottom_right, DefinedViewportProjection.Top);
                if (detail != null)
                {
                    pageview.SetActiveDetail(detail.Id);
                    detail.Viewport.ZoomExtents();
                    detail.DetailGeometry.IsProjectionLocked = true;
                    detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 10, doc.PageUnitSystem);
                    // Commit changes tells the document to replace the document's detail object
                    // with the modified one that we just adjusted
                    detail.CommitChanges();
                }
                pageview.SetPageAsActive();
                doc.Views.ActiveView = pageview;
                doc.Views.Redraw();
                return Rhino.Commands.Result.Success;
            }
            return Rhino.Commands.Result.Failure;
        }
        /// <summary>
        /// Alternative AddLayout with additional options
        /// </summary>
        public static Rhino.Commands.Result AddLayout(string name, string detTitle, double width, double height, Rectangle3d detailRec, double scale, out RhinoPageView layout)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            var page_views = doc.Views.GetPageViews();
            int page_number = (page_views == null) ? 1 : page_views.Length + 1;

            var pageview = doc.Views.AddPageView(name, width, height);

            if (pageview != null)
            {
                Point2d top_left = new Point2d(0, height);
                Point2d bottom_right = new Point2d(width, 0);
                var detail = pageview.AddDetailView(DefinedViewportProjection.Top.ToString(), top_left, bottom_right, DefinedViewportProjection.Top);
                if (detail != null)
                {

                    pageview.SetActiveDetail(detail.Id);
                    detail.Viewport.SetCameraTarget(detailRec.Center, true);
                    // CommitViewPortChanges modifies the Viewport only
                    detail.CommitViewportChanges();

                    pageview.SetActiveDetail(detail.Id);
                    detail.Name = detTitle;
                    detail.DetailGeometry.IsProjectionLocked = false;
                    detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
                    // Commit changes tells the document to replace the document's detail object
                    // with the modified one that we just adjusted
                    detail.CommitChanges();

                }
                pageview.SetPageAsActive();
                doc.Views.ActiveView = pageview;
                doc.Views.Redraw();
                layout = pageview;
                return Rhino.Commands.Result.Success;
            }
            layout = null;
            return Rhino.Commands.Result.Failure;
        }

        public static Rhino.Commands.Result AddLayout(string name, double width, double height, Point3d target, int detailCount, double scale, out RhinoPageView layout)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            var page_views = doc.Views.GetPageViews();
            //int page_number = (page_views == null) ? 1 : page_views.Length + 1;

            var pageview = doc.Views.AddPageView(name, width, height);

            //goal: check direction, if landscape, switch height and width for points
                
            if (pageview != null)
            {
                double margin = doc.ModelUnitSystem == UnitSystem.Inches ? .5 : doc.ModelUnitSystem == UnitSystem.Centimeters ? 2.5 : 25;

                double xRight = width - margin;
                double xLeft = 0 + margin;
                double xMid = width / 2;
                double yTop = height - margin;
                double yBottom = 0 + margin;
                double yMid = height / 2;

                Point2d leftTop = new Point2d(xLeft, yTop);
                Point2d leftCenter = new Point2d(xLeft, yMid);
                Point2d midTop = new Point2d(xMid, yTop);
                Point2d center = new Point2d(xMid, yMid);
                Point2d midBottom = new Point2d(xMid, yBottom);
                Point2d rightCenter = new Point2d(xRight, yMid);
                Point2d rightBottom = new Point2d(xRight, yBottom);

                switch (detailCount)
                {
                    case 1:
                        AddDetail(leftTop, rightBottom, pageview,target, "Top", scale, DefinedViewportProjection.Top);
                        break;
                    case 2:
                        AddDetail(leftTop, midBottom, pageview, target, "Top", scale, DefinedViewportProjection.Top);
                        AddDetail(midTop, rightBottom, pageview, target, "Perspective", scale, DefinedViewportProjection.Perspective);
                        break; 
                    case 3:
                        AddDetail(leftTop, rightCenter, pageview, target, "Top", scale, DefinedViewportProjection.Top);
                        AddDetail(leftCenter, midBottom, pageview, target, "Top", scale, DefinedViewportProjection.Front);
                        AddDetail(center, rightBottom, pageview, target, "Perspective", scale, DefinedViewportProjection.Right);
                        break;
                    case 4:
                        AddDetail(leftTop, center, pageview, target, "Top", scale, DefinedViewportProjection.Top);                       
                        AddDetail(midTop, rightCenter, pageview, target, "Perspective", scale, DefinedViewportProjection.Perspective);
                        AddDetail(center, rightBottom, pageview, target, "Right", scale, DefinedViewportProjection.Right);
                        AddDetail(leftCenter, midBottom, pageview, target, "Front", scale, DefinedViewportProjection.Front);
                        break;
                    default:
                        break;
                }
                
                pageview.SetPageAsActive();
                doc.Views.ActiveView = pageview;
                doc.Views.Redraw();
                layout = pageview;
                return Rhino.Commands.Result.Success;
            }
            layout = null;
            return Rhino.Commands.Result.Failure;
        }

        public static Rhino.Commands.Result AddDetail(Point2d top_left, Point2d bottom_right, RhinoPageView pageview, Point3d target, string detTitle, double scale, DefinedViewportProjection projection)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            var detail = pageview.AddDetailView(detTitle, top_left, bottom_right, projection);
            if (detail != null)
            {
                pageview.SetActiveDetail(detail.Id);
                detail.Viewport.SetCameraTarget(target, true);
                // CommitViewPortChanges modifies the Viewport only
                detail.CommitViewportChanges();

                pageview.SetActiveDetail(detail.Id);
                detail.Name = detTitle;
                detail.DetailGeometry.IsProjectionLocked = false;
                detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
                // Commit changes tells the document to replace the document's detail object
                // with the modified one that we just adjusted
                detail.CommitChanges();
                return Rhino.Commands.Result.Success;
            }
            return Rhino.Commands.Result.Failure;
        }
        /// <summary>
        /// Sets the size of a Layout for Printing
        /// </summary>
        /// <param name="page"> Layout Page</param>
        /// <param name="dpi">print dpi</param>
        /// <returns></returns>
        public static System.Drawing.Size SetSize(RhinoPageView page, int dpi)
        {
            double width = page.PageWidth;
            double height = page.PageHeight;
            if (IsLayoutFlipped(page))
            {
                width = page.PageHeight;
                height = page.PageWidth;
            }
            System.Drawing.Size size = new System.Drawing.Size(Convert.ToInt32(width * dpi), Convert.ToInt32(height * dpi));
            return size;
        }

        /// <summary>
        /// Checks the orientation of a Layout
        /// </summary>
        /// <param name="page"> Layout to check</param>
        /// <returns></returns>
        public static bool IsLayoutFlipped(RhinoPageView page)
        {
            double sizeRatio = page.Size.Width / page.Size.Height;
            double pageRatio = page.PageWidth / page.PageHeight;
            bool same = sizeRatio == pageRatio;
            bool flipped = sizeRatio == 1 / pageRatio;
            if (!same & !flipped) return false;
            else if (sizeRatio < 1 == pageRatio < 1) return false;
            else return true;
        }

    }
    public class PageNumCompare : IComparer<RhinoPageView>
    {
        public int Compare(RhinoPageView x, RhinoPageView y)
        {
            return x.PageNumber.CompareTo(y.PageNumber);
        }
    }

    public class PageNameCompare : IComparer<RhinoPageView>
    {
        public int Compare(RhinoPageView x, RhinoPageView y)
        {
            return ((new System.Collections.CaseInsensitiveComparer()).Compare(x.PageName, y.PageName));
        }
    }


}

