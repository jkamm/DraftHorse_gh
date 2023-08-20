namespace DraftHorse.Params
{
    class Param_Directory : Grasshopper.Kernel.Parameters.Param_FilePath
    {
        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            //Grasshopper.Kernel.GH_PersistentParam<GH_String>.Menu_
            //base.AppendAdditionalMenuItems(menu);
            //AppendAdditionalMenuItems(menu);
            Menu_AppendWireDisplay(menu);
            Menu_AppendDisconnectWires(menu);
            Menu_AppendSeparator(menu);
            //Menu_CreateMultilineTextEditItem();
            //Menu_CustomSingleValueItem();
            //Menu_CustomSingleDirectoryItem();
            //Menu_AppendCustomItem(menu,System.Windows.Forms.Control.)
            Menu_AppendManageCollection(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendDestroyPersistent(menu);
            Menu_AppendInternaliseData(menu);
        }
    }
}
