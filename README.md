# ![DrafthorseLogo](https://github.com/jkamm/DraftHorse_gh/assets/9583495/06ac40b9-99bc-4328-9671-e6da55de96ec) DraftHorse 

Grasshopper plugin for Rhino 7, helping automate Layout creation and management

![DH_ComponentSet](https://github.com/jkamm/DraftHorse_gh/assets/9583495/f1a0c04b-e913-4790-bccd-82986e8662d5)

![DH_ComponentsOnCanvas](https://github.com/jkamm/DraftHorse_gh/assets/9583495/814d4af4-a2fc-4f68-a357-0beff742ad61)



WIP/Goals:

- [ ] Example files for all components to demonstrate basic workflows
	- [ ] Layout from Bounding Box (multipart template printing?)
	- [ ] Activate View (Bake geometry to different layouts, like a BOM)		
- [ ] Check that DisplayMode inputs work in other languages
- [ ] Bake to Layouts (to allow programmatic baking of geometry to paperspace with a layout as additional object attribute)
- [ ] Switch view input for details from view attributes (target, displayMode, projection) to CurveComponents.Make2DViewParam
- [ ] Create custom gh params for referencing DetailViewObject and PageView
- [ ] Change object references to DetailView and Layout/PageView params in RH8
- [x] Add PaperName & Orientation as inputs to New Layout Component (not possible in RH7 - paperName is read-only)
- [ ] Add Plane or View input for Layout from Bounding Box to allow non-XY views
- [ ] Add component to label details (name, auto-number, scale)
- [x] Add Layout Edit component to modify Layout attributes (pageName, width, height, pageNumber (?), keys, values)
