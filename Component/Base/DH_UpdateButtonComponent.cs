using Grasshopper.Kernel;

namespace DraftHorse.Component.Base
{
    //Borrowed from Metahopper with permission from Andrew Heumann, 2023

    public abstract class DH_UpdateButtonComponent : GH_Component
    {
        public DH_UpdateButtonComponent(string name, string nickname, string description, string category, string subcategory)
            : base(name, nickname, description, category, subcategory)
        {
        }

        protected abstract override void RegisterInputParams(GH_InputParamManager pManager);

        protected abstract override void RegisterOutputParams(GH_OutputParamManager pManager);

        protected abstract override void SolveInstance(IGH_DataAccess DA);

        public override void CreateAttributes()
        {
            m_attributes = new DH_UpdateButtonComponent_Attributes(this);
        }
    }
}
