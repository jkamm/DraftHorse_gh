# ![DrafthorseLogo](https://github.com/jkamm/DraftHorse_gh/assets/9583495/06ac40b9-99bc-4328-9671-e6da55de96ec) DraftHorse 

Grasshopper plugin for Rhino 7, helping automate Layout creation and management

![DH_ComponentSet](https://github.com/jkamm/DraftHorse_gh/assets/9583495/f1a0c04b-e913-4790-bccd-82986e8662d5)

![DH_ComponentsOnCanvas](https://github.com/jkamm/DraftHorse_gh/assets/9583495/814d4af4-a2fc-4f68-a357-0beff742ad61)

Look at the Example files for how to use components for different workflows including: 
- Copying an existing layout, Sorting, and batch Printing to one or more PDFs
![DH_CopySortPrint_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/c6a1353f-4bb5-4a73-8d27-a6688386a587)
- Generating a new Layout and modifying existing Details
![DH_NewLayout_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/ed19d8e9-af3e-437d-9895-68a353a59175)
- Modifying an existing layout
![DH_ModifyLayouts_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/1b090913-a04b-490b-9caf-14534ffa1bfa)
- Modifying an existing detail
![DH_ModifyDetails_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/24c52aad-9b4e-47ae-bb92-8a809f64434e)
- Modifying Layout and Document User Text (useful for titleblocks)
![DocumentText_Example](https://github.com/jkamm/DraftHorse_gh/assets/9583495/90e31c3b-f8cc-42c2-8b90-dc7f27a3c498)

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
- [ ] Add ChangeSpace capability
- [ ] Add capability to hide/show layers in details
- [ ] Add capability to hide/show objects in details
