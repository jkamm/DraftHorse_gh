# DraftHorse
Grasshopper plugin for Rhino 7, helping automate Layout creation and management

![DH_ComponentSet](https://github.com/jkamm/DraftHorse_gh/assets/9583495/665fee25-7ae5-40cb-b533-5126d90e8be4)

![DH_Components](https://github.com/jkamm/DraftHorse_gh/assets/9583495/a2e04489-bb64-4b7e-a49a-fedb89de7611)

WIP/Goals:
[ ] Example files for all components to demonstrate basic workflows
	[ ] Layout from Bounding Box (multipart template printing?)
	[ ] Activate View (Bake geometry to different layouts, like a BOM)		
[ ] Check that DisplayMode inputs work in other languages
[ ] Bake to Layouts (to allow programmatic baking of geometry to paperspace with a layout as additional object attribute)
[ ] Switch view input for details from view attributes (target, displayMode, projection) to CurveComponents.Make2DViewParam
[ ] Create custom gh params for referencing DetailViewObject and PageView
[ ] Change object references to DetailView and Layout/PageView params in RH8
[x] Add PaperName & Orientation as inputs to New Layout Component (not possible in RH7 - paperName is read-only)
[ ] Add Plane or View input for Layout from Bounding Box to allow non-XY views
[ ] Add component to label details (name, auto-number, scale)
[ ] Add Layout Edit component to modify Layout attributes (pageName, width, height, pageNumber (?), keys, values)
