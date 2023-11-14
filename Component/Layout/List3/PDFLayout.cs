using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using static DraftHorse.Helper.ValList;


namespace DraftHorse.Component
{
    public class PDFLayout : Base.DH_ButtonComponent
    {
        #region GH_Component
        /// <summary>
        /// Initializes a new instance of the PDFLayout class.
        /// </summary>
        public PDFLayout()
          : base("PDF Layout", "DH pdf",
              "Print one or more Layouts to one or more PDFs",
              "DraftHorse", "Layout")
        {
            ButtonName = "Print";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //need to add default behavior for no indices - print all
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Set to true to Print \nUse Toggle only (not Button)", GH_ParamAccess.item)].Optional = true; 
            Params.Input[pManager.AddIntegerParameter("LayoutIndex", "Li[]", "List of Indices for Layouts to Print to PDF \nAttach Value List for list of Layouts", GH_ParamAccess.list)].Optional = true;
            Grasshopper.Kernel.Parameters.Param_FilePath filePath = new Grasshopper.Kernel.Parameters.Param_FilePath();
            pManager.AddParameter(filePath, "Folder", "F", "Target Folder to Save PDFs \nWill create if it does not exist", GH_ParamAccess.item);
            pManager.AddTextParameter("Filename", "N", "Filename", GH_ParamAccess.item, "Layout");
            Params.Input[pManager.AddIntegerParameter("DPI", "DPI", "Print Resolution (72-1200) Default is 100", GH_ParamAccess.item, 100)].Optional = true;
            Params.Input[pManager.AddIntegerParameter("ColorMode", "C", "0 = Black&White\n1 = Display Color\n2 = Print Color", GH_ParamAccess.item, 0)].Optional = true;
            Params.Input[pManager.AddBooleanParameter("UsePrintWidths", "U", "Use defined print widths (False prints Display values) ", GH_ParamAccess.item, true)].Optional = true;
            Params.Input[pManager.AddNumberParameter("DefaultWidth", "D", "Set default print width for undefined", GH_ParamAccess.item, 0.1)].Optional = true;
            Params.Input[pManager.AddNumberParameter("WireScale", "W", "Scale width of curves in print", GH_ParamAccess.item, 1)].Optional = true;
            

        }
          
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "R", "FilePath on Success", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            DA.GetData("Run", ref run);

            List<int> indexList = new List<int>();
            DA.GetDataList("LayoutIndex", indexList);

            string folder = String.Empty;
            if (!DA.GetData("Folder", ref folder)) return;
            //Should there be a default?  if so, should it be the same as the current definition?
            //could folder dialog open the folder browser?  I think the folder param does this, so it might be possible.

            string filename = String.Empty;
            if (!DA.GetData("Filename", ref filename)) return;
            //Default filename?

            int dpi = 100;
            if (!DA.GetData("DPI", ref dpi)) dpi = 100;
            if (dpi > 1200)
            {
                dpi = 1200;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Max DPI = 1200");
            }
            else if (dpi < 72)
            {
                dpi = 72;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Min DPI = 72");
            }

            string result = "Failed";
            List<String> Results = new List<string>();

            int colorMode = 0;
            if (!DA.GetData("ColorMode", ref colorMode)) colorMode = 0;
            ViewCaptureSettings.ColorMode color = new ViewCaptureSettings.ColorMode();
            if (colorMode == 1) color = ViewCaptureSettings.ColorMode.DisplayColor;
            else if (colorMode == 2) color = ViewCaptureSettings.ColorMode.PrintColor;
            else color = ViewCaptureSettings.ColorMode.BlackAndWhite;

            bool usePrintWidths = true;
            if (!DA.GetData("UsePrintWidths", ref usePrintWidths)) usePrintWidths = true;

            double defaultPrintWidth = 0.1;
            if (!DA.GetData("DefaultWidth", ref defaultPrintWidth)) defaultPrintWidth = 0.1;

            double wireScale = 1.0;
            if (!DA.GetData("WireScale", ref wireScale)) wireScale = 1.0;


            //Main
            if (run || Execute)
            {

                #region EscapeBehavior
                //Esc behavior code snippet from 
                // http://james-ramsden.com/you-should-be-implementing-esc-behaviour-in-your-grasshopper-development/
                if (GH_Document.IsEscapeKeyDown())
                {
                    GH_Document GHDocument = OnPingDocument();
                    GHDocument.RequestAbortSolution();
                }
                #endregion EscapeBehavior

                //Check that folder exists.  If not, make it
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);


                Rhino.FileIO.FilePdf pdf = Rhino.FileIO.FilePdf.Create();

                //initialize settings variable
                ViewCaptureSettings settings = new ViewCaptureSettings(); //not necessary to initialize if no settings are done globally

                //get pages based on indices
                RhinoPageView[] pages;
                RhinoPageView[] page_views = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews();


                if (indexList.Count != 0)
                {
                    pages = new RhinoPageView[indexList.Count];
                    for (int i = 0; i < indexList.Count; i++)
                        pages[i] = Layout.GetPage(indexList[i]);
                }
                else
                {
                    pages = page_views;
                    Layout.SortPagesByPageNumber(pages);
                }

                /*
                for (int i = 0; i < indexList.Count; i++)
                    pages[i] = Layout.GetPage(indexList[i]);
                 */

                foreach (RhinoPageView page in pages)
                {
                    System.Drawing.Size size = Layout.SetSize(page, dpi);
                    settings = new ViewCaptureSettings(page, size, dpi);
                    settings.OutputColor = color;
                    pdf.AddPage(settings);
                    settings.UsePrintWidths = usePrintWidths;
                    settings.DefaultPrintWidthMillimeters = defaultPrintWidth;
                    settings.WireThicknessScale = wireScale;
                }

                //if filename does not end in .pdf, add it.
                if (!filename.EndsWith(".pdf", true, System.Globalization.CultureInfo.CurrentCulture)) filename += ".pdf";

                string filePath = folder + "\\" + filename;

                pdf.Write(filePath);

                if (System.IO.File.Exists(filePath))
                {
                    result = "Success";
                    Results.Add(result);
                    Results.Add(filePath);
                }
                else Results.Add(result);

                DA.SetDataList("Result", Results);


                //return the filepath of the new PDF in the output, as well as an indicator of whether it succeeded.
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LayoutPDF;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9cc1fc76-ddd4-467d-846f-3206f1165ad5"); }
        }
        #endregion GH_Component

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add/Update ValueList", Menu_DoClick, true);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            var pageViews = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews();
            Layout.SortPagesByPageNumber(pageViews);
            List<string> pageNums = pageViews.Select(p => p.PageNumber.ToString()).ToList();
            List<string> pageNames = pageViews.Select(p => p.PageName.ToString()).ToList();

            /*
            string[] pageNumbers = new string[pageViews.Length];
            //string[] pageNames = new string[pageViews.Length];
            for (int i = 0; i < pageViews.Length; i++)
            {
                pageNumbers[i] = pageViews[i].PageNumber.ToString();
                //pageNames[i] = pageViews[i].PageName;
            }
             */
            /*
            var pageDictionary = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews().ToDictionary(v => v.PageNumber.ToString(), v => v.PageName);
            List<string> layoutIndices = pageDictionary.Keys.ToList();
            List<string> pageViewNames = new List<string>();
            for (int i = 0; i < pageViewNames.Count; i++)
                pageViewNames.Add(pageDictionary[layoutIndices[i]].ToString());
             */

            if (!AddOrUpdateValueList(this, 0, "Layouts", "Layouts To Print: ", pageNames, pageNums))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 1 + "] failed to update");

            ExpireSolution(true);
        }
        #endregion Add Value Lists

        #region AutoValueList

        //Update a value list if added to a given input(based on Elefront and FabTools)
        //on event for a source added to a given input

        private bool _handled = false;

        private void SetupEventHandlers()
        {
            if (_handled)
                return;

            Params.Input[0].ObjectChanged += InputParamChanged;

            _handled = true;
        }

        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();
            SetupEventHandlers();
        }


        public void InputParamChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {

            //optional feedback
            //if (e.Type == GH_ObjectEventType.Sources)
            //    Rhino.RhinoApp.WriteLine("Sources changed on input '{0}'.", sender.NickName);

            if (sender.NickName == Params.Input[1].NickName)
            {
                // optional feedback
                // Rhino.RhinoApp.WriteLine("This is the right input");

                var pageDictionary = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews().ToDictionary(v => v.PageName, v => v.PageNumber);
                List<string> pageViewNames = pageDictionary.Keys.ToList();
                List<string> layoutIndices = new List<string>();
                for (int i = 0; i < pageViewNames.Count; i++)
                    layoutIndices.Add(pageDictionary[pageViewNames[i]].ToString());

                //try to modify input as a valuelist
                try
                {
                    UpdateValueList(this, 0, "Layouts", "Layouts To Print: ", pageViewNames, layoutIndices);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList
    }
}