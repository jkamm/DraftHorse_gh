using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using Rhino.Display;


namespace DraftHorse.Helper
{
    /// <summary>
    /// ValList class contains code to copy and paste into a component to implement ValList code.
    /// </summary>
    public class NewValList
    {
        //ValList object should have:
        // - List Source
        // - list update?
        // - Constructor with a list that needs to be updated
        // - Constructor with a fixed list of keys, values.
        // - If no list of values, then keys are the values.
        // - Add method for generating a list of values from the keys? 
        // 
        // - when keyList is set, check if valueList exists.  if yes and keyList.Length != valueList.Length, clear valueList.
        // - List name (repeat for nickname + ": ")
        // - Input where valList gets instantiated
        // -- this must check that this input exists on the parent.  Set using method. Get using return on public variable.  

        private int input;
        private List<string> keys;
        private List<string> values;
        private string nickname;
        
        public int Input { get => input; }
        public List<string> Keys { get => keys; }
        public List<string> Values {
            get
            {
                if (keys.Count != values.Count) return keys;
                else return values;
            }
            }
        public string Name { get; set; }
        public string Nickname
        {
            get
            {
                if (nickname != null) return nickname;
                else return Name + ": ";
            }
            set
            {
                nickname = value;
            }
        }
        public Grasshopper.Kernel.Special.GH_ValueList _ValueList;

        public bool SetInput(GH_Component owner, int testInput)
        {
            //if input isValid (test if owner has input at that location)
            if (owner.Params.Input.Count > testInput)
            {
                input = testInput;
                return true;
            }
            else return false;  //if input is not valid
        }

        public void SetKeys(List<string> testKeys)
        {
            //reset values             
            values = new List<string>();
            keys = testKeys;
        }

        public bool SetVals(List<string> testVals)
        {
            if (testVals.Count == keys.Count)
            {
                values = testVals;
                return true;
            }
            else return false; //Value List does not match Key List
        }

        public void SetValsAsInts(int startInt = 0)
        {
            //Generate integer Value per Key
            values = new List<string>();
            for (int i = 0; i < keys.Count; i++)
                values.Add((startInt + i).ToString());
        }

        public void SetValsAsKeys()
        {   
            values = keys;
        }

        //Constructors
        public NewValList()
        { }

        // - Constructor with a list that needs to be updated
        // - Constructor with a fixed list of keys, values.
        public NewValList(int input, string name, GH_Component owner)
        {
            keys = new List<string>();
            values = new List<string>();
            nickname = null;
            SetInput(owner, input);
        }

        public NewValList(int input, string name, string nickname, GH_Component owner)
        {
            keys = new List<string>();
            values = new List<string>();
            Nickname = nickname;
            SetInput(owner, input);
        }

        public NewValList(int input, string name, string nickname, GH_Component owner, List<string> keys)
        {
            keys = new List<string>();
            values = new List<string>();
            Nickname = nickname;
            SetInput(owner, input);
            SetKeys(keys); 
            SetVals(keys);
        }


        /*in-Component Code for adding Value Lists using Helper_ValList methods

        #region Add Value Lists
        //Add menu item for adding/updating Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add/Update ValueLists", Menu_DoClick);
        }

        //Add behavior for menu item
        private void Menu_DoClick(object sender, EventArgs e)
        {
            //Define the lists to add to the Value Lists
            List<string> keys = Rhino.RhinoDoc.ActiveDoc.Views.GetStandardRhinoViews().ToDictionary(v => v.ActiveViewport.Name, v => v).Keys.ToList(); ;
            List<string> pageViewNames = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews().ToDictionary(v => v.PageName, v => v).Keys.ToList();

            //Call AddOrUpdate per List    
            if (!AddOrUpdateValueList(this, 0, "Standard Views", "Standard Views: ", keys, keys))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

            if (!AddOrUpdateValueList(this, 1, "PageViews", "Page Views: ", pageViewNames, pageViewNames))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 1 + "] failed to update");

            ExpireSolution(true);
        }
        #endregion Add Value Lists

    */
    }

    public static class ValList
    {
        #region Add and/or Update ValueList

        /// <summary>
        /// Adds a value list at the inputNumber
        /// Overloads to add:  replace List with Array, add values per key (default will be numbers in order)
        /// source/logic from: http://james-ramsden.com/grasshopper-automatically-create-a-value-list-in-c/
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="inputNumber"></param>
        /// <param name="name"></param>
        /// <param name="nickName"></param>
        public static void AddValueList(GH_Component owner, int inputNumber, string name, string nickName, List<string> keys)
        {
            //ExceptionHandling: Check that Param.input[inputNumber] of  type is string (?).  
            //The input will be able to cast to expected type as long as the string is in a favorable format.

            //ExceptionHandling: Check that input exists.
            if (owner.Params.Input.Count > inputNumber)
            {
                var valList = new Grasshopper.Kernel.Special.GH_ValueList();

                valList.Name = name;
                //recommended format for nickname "Something: "
                valList.NickName = nickName;
                valList.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
                valList.CreateAttributes();  //does this need to be prior to Name NickName declaration?
                valList.ListItems.Clear();

                //int inputcount = Params.Input[1].SourceCount; //testing whether this is useful/necessary
                valList.Attributes.Pivot = new System.Drawing.PointF((float)
                  owner.Attributes.DocObject.Attributes.Bounds.Left - valList.Attributes.Bounds.Width - 30,
                  (float)owner.Params.Input[inputNumber].Attributes.Bounds.Y);// + inputcount * 30); //This is not useful.  Needs to be relative to the input in question.

                //Add values to the ValueList
                for (int i = 0; i < keys.Count; i++)
                {
                    valList.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(keys[i], i.ToString()));
                }

                GH_Document GrasshopperDocument = owner.OnPingDocument();
                //this may return a null under certain circumstances.  check and add exception handling
                //https://www.grasshopper3d.com/forum/topics/get-current-grasshopperdocument-in-c
                if (GrasshopperDocument == null)
                    throw new ArgumentNullException("Grasshopper document is null");

                GrasshopperDocument.AddObject(valList, false);

                owner.Params.Input[inputNumber].AddSource(valList);
                valList.ExpireSolution(true);
            }

        }

        /// <summary>
        /// Adds a value list at the inputNumber
        /// Overloads to add:  replace List with Array, add values per key (default will be numbers in order)
        /// source/logic from: http://james-ramsden.com/grasshopper-automatically-create-a-value-list-in-c/
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="inputNumber"></param>
        /// <param name="name"></param>
        /// <param name="nickName"></param>
        public static void AddValueList(GH_Component owner, int inputNumber, string name, string nickName, List<string> keys, List<string> values)
        {
            //ExceptionHandling: Check that Param.input[inputNumber] of  type is string (?).  
            //The input will be able to cast to expected type as long as the string is in a favorable format.

            //ExceptionHandling: Check that input exists.
            if (owner.Params.Input.Count > inputNumber)
            {
                var valList = new Grasshopper.Kernel.Special.GH_ValueList();

                valList.Name = name;
                //recommended format for nickname "Something: "
                valList.NickName = nickName;
                valList.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

                valList.ListItems.Clear();
                //Add values to the ValueList
                for (int i = 0; i < keys.Count; i++)
                {
                    valList.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(keys[i], "\"" + values[i] + "\""));
                }

                valList.CreateAttributes();

                valList.Attributes.Pivot = new System.Drawing.PointF((float)
                  owner.Attributes.DocObject.Attributes.Bounds.Left - valList.Attributes.Bounds.Width - 30,
                  (float)owner.Params.Input[inputNumber].Attributes.Bounds.Y);

                GH_Document GrasshopperDocument = owner.OnPingDocument();
                //owner may return a null under certain circumstances.  check and add exception handling
                //https://www.grasshopper3d.com/forum/topics/get-current-grasshopperdocument-in-c
                if (GrasshopperDocument == null)
                    throw new ArgumentNullException("Grasshopper document is null");

                GrasshopperDocument.AddObject(valList, false);

                owner.Params.Input[inputNumber].AddSource(valList);

                valList.ExpireSolution(true);
            }

        }

        public static void UpdateValueList(GH_Component owner, int inputNum, string name, string nickName, List<string> keys)
        {
            //Reset Properties
            //Grasshopper.Kernel.Special.GH_ValueList valList = owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList;
            //valList.Name = name;
            //valList.NickName = nickName;
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).Name = name;
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).NickName = nickName;
            //(owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

            //Reset Values
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListItems.Clear();

            for (int i = 0; i < keys.Count; i++)
            {
                (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(keys[i], i.ToString()));
            }

            //Reset Component Preview
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ExpireSolution(true);
        }

        public static void UpdateValueList(GH_Component owner, int inputNum, string name, string nickName, List<string> keys, List<string> values)
        {
            //Reset Properties
            //Grasshopper.Kernel.Special.GH_ValueList valList = owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList;
            //valList.Name = "Test";
            //valList.NickName = nickName;
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).Name = name;
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).NickName = nickName;
            //(owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

            //Reset Values
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListItems.Clear();
            //store old list values and try to reinstate?

            for (int i = 0; i < keys.Count; i++)
            {
                (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(keys[i], "\"" + values[i] + "\""));
            }

            //Reset Component Preview
            (owner.Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ExpireSolution(true);

        }

        public static bool AddOrUpdateValueList(GH_Component owner, int inputNum, string name, string nickName, List<string> keys, List<string> values)
        {
            if (keys.Count == 0) return false;
            else if (keys.Count != values.Count) throw new Exception("key and value lists must have matching items");
            else if (owner.Params.Input[inputNum].SourceCount == 0)
            {
                //Call method to add valueList
                AddValueList(owner, inputNum, name, nickName, keys, values);
                return true;
            }
            else if (owner.Params.Input[inputNum].SourceCount == 1)
                try
                {
                    //Params.Input[inputNum].Sources[0] as Grasshopper.Kernel.Special.GH_ValueList).ListItems.Clear();
                    UpdateValueList(owner, inputNum, name, nickName, keys, values);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            else return false;
        }

        public static void RemoveComponentAtInput()
        {
            //GH_Document GrasshopperDocument = this.OnPingDocument();
            ////    //this may return a null under certain circumstances.  check and add exception handling
            ////    //https://www.grasshopper3d.com/forum/topics/get-current-grasshopperdocument-in-c

            //GrasshopperDocument.RemoveObject(Params.Input[inputNumber].Sources[0], false);
            //AddValueList(inputNumber, name, nickName, keys, values);

            //this.ExpireSolution(true);
        }
        #endregion Add and/or Update ValueList

        #region Special Value Lists

        public static List<string> GetLayoutList()
        {
            var pageViews = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews();
            
            HashSet<string> pageNames = new HashSet<string>();
            
            foreach (RhinoPageView page in pageViews) pageNames.Add(page.PageName);
                    
            return pageNames.ToList();

            //Goal: add/throw exception for when PageViewList is empty.
        }

        public static List<string> GetStandardViewList()
        {
            return Rhino.RhinoDoc.ActiveDoc.Views.GetStandardRhinoViews().ToDictionary(v => v.ActiveViewport.Name, v => v).Keys.ToList();
        }

        
        #endregion Special Value Lists

        #region Add boolean instantiator

        //Want to add this to the input menu list.  need to edit the boolean type and instantiate a special type of boolean.

        #endregion Add boolean instantiator

    }


}
   
