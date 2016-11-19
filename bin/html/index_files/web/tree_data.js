d = new dTree('d');

// nodeId | parentNodeId | nodeName | nodeUrl
d.add(0,-1,'Introduction','../notes/Introduction.html');
d.add(2,-1,'Documentation','../notes/Start.html');
d.add(3,2,'Navigation','../notes/Navigation.html');
d.add(4,2,'Material Parameters','../notes/MaterialParameters.html');
d.add(5,2,'Formula Parameters','../notes/FormulaParameters.html');
d.add(6,2,'Image Parameters','../notes/ImageParameters.html');
d.add(7,2,'Animation','../notes/Animation.html');
d.add(8,-1,'Formula','../notes/Formula.html');
d.add(10,8,'Methods and Datatypes','../notes/MethodsAndDatatypes.html');
d.add(11,10,'Init','../notes/Init.html');
d.add(12,10,'Access to Global Parameters','../notes/AccessToGlobalParameters.html');
d.add(13,10,'Fractal Parameters','../notes/FractalParameters.html');
d.add(14,8,'Bugs','../notes/Bugs.html');
d.add(15,-1,'Tips','../notes/Tips.html');
d.add(16,-1,'WebGl Examples','../notes/WebGlExamples.html');
d.add(17,-1,'Acknowledgment','../notes/Acknowledgment.html');
document.write(d);
d.oAll(true);
//dTree.prototype.openAll();