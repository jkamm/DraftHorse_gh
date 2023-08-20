using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace DraftHorse
{
    public class DraftHorseInfo : GH_AssemblyInfo
    {
        public override string Name => "DraftHorse";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Tools for managing Rhino Layouts programmatically through Grasshopper";

        public override Guid Id => new Guid("ee57be77-5df8-4d13-b942-fa7177d208d1");

        //Return a string identifying you or your company.
        public override string AuthorName => "Jo Kamm";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "jokamm@gmail.com";
    }
}