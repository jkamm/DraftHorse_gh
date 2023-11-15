# ![DrafthorseLogo](https://github.com/jkamm/DraftHorse_gh/assets/9583495/06ac40b9-99bc-4328-9671-e6da55de96ec) DraftHorse 

Grasshopper plugin for Rhino 7 (win), helping automate Layout creation and management. 

![DH_ComponentSet](https://github.com/jkamm/DraftHorse_gh/assets/9583495/7946460c-24f0-4e05-aa82-76cbafe302c8)

![DH_ComponentsOnCanvas](https://github.com/jkamm/DraftHorse_gh/assets/9583495/b33f200d-0709-493f-8a3f-38773b4097d2)

Look at the Example files for how to use components for different workflows including: 
- Copying an existing layout, Sorting, and batch Printing to one or more PDFs
![DH_CopySortPrint_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/c6a1353f-4bb5-4a73-8d27-a6688386a587)
- Generating a new Layout and modifying existing Details
![DH_NewLayout_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/ed19d8e9-af3e-437d-9895-68a353a59175)
- Modifying an existing layout
![DH_NewLayout_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/a3e6a0e4-e23f-4834-a979-18eaa5b7d55a)
- Generating new Details on an existing Layout
![DH_NewDetails_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/5719ba13-ccfb-467c-a613-87011ada5139)
- Modifying Layout and Document User Text (useful for titleblocks)
![DocumentText_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/90e31c3b-f8cc-42c2-8b90-dc7f27a3c498)

v0.3.1
- bug fix: LayoutFromBBox component scale issue if model units different from page units
- bug fix: PDFLayout scale issue for non-inch units
  
v0.3.2
- bug fix: AutoValueList not implemented for DisplayModes on DetailNew and DetailEdit

v0.4.0
- improvement: added View input to New Detail, Edit Detail (good for custom views)
- BBox Layout now generates non-XY Plane views
- bugfix: add/fix Layout Index, Display, and Projection Auto-ValueList generation on New Detail

WIP/Goals:

- [x] Example files for all components to demonstrate basic workflows
	- [x] Layout from Bounding Box (multipart template printing?)
	- [x] Activate View (Bake geometry to different layouts, like a BOM)		
- [ ] Check that DisplayMode inputs work in other languages
- [ ] Bake to Layouts Component(to allow programmatic baking of geometry to paperspace with a layout as additional object attribute)
- [x] Switch view input for details from view attributes (target, displayMode, projection) to CurveComponents.Make2DViewParam
- [ ] Create custom gh params for referencing DetailViewObject and PageView (for RH7)
- [ ] Change object references to DetailView and Layout/PageView params in RH8
- [x] Add PaperName & Orientation as inputs to New Layout Component (not possible in RH7 - paperName is read-only)
- [x] Add Plane or View input for Layout from Bounding Box to allow non-XY views
- [ ] Add component to label details (name, auto-number, scale)
- [x] Add Layout Edit component to modify Layout attributes (pageName, width, height, pageNumber (?), keys, values)
- [ ] Add ChangeSpace capability
- [ ] Add capability to hide/show layers in details
- [ ] Add capability to hide/show objects in details
- [ ] Add Rhino PDF default Papernames to Modify Layout component
- [ ] Add units change to Modify Layout component
- [ ] Improve component icons
- [ ] Refactor for OSX compatibility
- [ ] Switch complex components to Variable Input Components to simplify
- [ ] Add RedrawViews component with self-instantiating Trigger or built-in trigger
