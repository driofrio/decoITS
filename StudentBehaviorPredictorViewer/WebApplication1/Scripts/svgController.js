"use strict";
var svgdoc, svgRoot;
var svg;
var imgScale = 1,
    clusters = null,
    students = null;
var currentColumns = 0;
var totalStudent = 0;
var totalEvents = 0;
var currentRows = {
    correctFlow: 1,
    irrelevantErrors: 1,
    relevantErrors: 1
}
var posted = 0;
var received = 0;
var manager = new MapManager();
var showBar = false;
//var loader = new MapLoader();
var href = window.location.origin + "/";
//var href = window.location.origin + "/SBPViewer2/";
var minMap = 0;

// Opera 8.0+
var isOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
// Firefox 1.0+
var isFirefox = typeof InstallTrigger !== 'undefined';
// At least Safari 3+: "[object HTMLElementConstructor]"
var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
// Internet Explorer 6-11
var isIE = /*@cc_on!@*/false || !!document.documentMode;
// Edge 20+
var isEdge = !isIE && !!window.StyleMedia;
// Chrome 1+
var isChrome = !!window.chrome && !!window.chrome.webstore;
// Blink engine detection
var isBlink = (isChrome || isOpera) && !!window.CSS;

var CorrectStatesDic = new Dictionary();
var TimeErrorStateDic = new Dictionary();
var IncompatibilityErrorStateDic = new Dictionary();
var WorldErrorStateDic = new Dictionary();
var OtherErrorStateDic = new Dictionary();
var SeqComplexDependenceErrorStateDic = new Dictionary();
var SimpleDependenceErrorStateDic = new Dictionary();
var EventsDic = new Dictionary();
var LastStates = new Dictionary();
var ErrorCorrectStatesDic = new Dictionary();

var arrowsDarkBlue = null;
var phaseSelection = null;
var TxtPhaseSelection = null;
var TxtConfigurationSelection = null;
var modeSelection = 0;
var clusterSelection = null;
var studentSelection = null;
var normalEventId = 0;
var vectorEventId = 0;
var groupEventId = 0;
var correctStateId = 1;
var TimeErrorStateId = 1;
var IncompatibilityErrorStateId = 1;
var WorldErrorStateId = 1;
var OtherErrorStateId = 1;
var SeqComplexDependenceErrorStateId = 1;
var SimpleDependenceErrorStateId = 1;
var twConstants =
{
    DIALECT_SVG: 'svg',
    DIALECT_VML: 'vml',
    NS_SVG: 'http://www.w3.org/2000/svg',
    NS_XLINK: 'http://www.w3.org/1999/xlink'
}
var selectedElement = null;
//-------------------test value---------
var iniNode;
var instances = 0;
var map = new Array(new Array());

//===========================SVG Handler=====================================
//-------------checking browser--------------------
var isIE = false;
function checkBrowser() {
    return navigator.appName == "Microsoft Internet Explorer";
}
isIE = checkBrowser();

//-------------getting svg doc--------------------
function getSVGDocument(svg) {
    var result = null;
    if (isIE) {
        if (svg.tagName.toLowerCase() == "embed") {
            try {
                result = svg.getSVGDocument();
            } catch (e) {
                alert(e + " may be svg embed not init");
            }
        }
    } else {
        result = svg.ownerDocument;
    }
    return result;
}
//--------------getting root node--------------
function getSVGRoot(svg, doc) {
    if (svg.tagName.toLowerCase() == "embed") {
        if (doc) {
            return doc.documentElement;
        } else {
            return getSVGDocument(svg).documentElement;
        }
    } else if (svg.tagName.toLowerCase() == "svg") {
        return svg;
    } return null;
}

//------create svg node with id and parent,create <embed /> for IE，and <SVG /> for other browser----
function createSVG(id, parent) {
    if (isIE) {
        svg = document.createElement("embed");
        svg.setAttribute("id", id);
        svg.setAttribute("type", 'image/svg+xml');
        svg.setAttribute("src", emptySVGSrc);
    } else {
        svg = document.createElementNS(twConstants.NS_SVG, 'svg');
    }
    svg.setAttribute("width", '100%');
    svg.setAttribute("height", (screen.height / 100) * 60);
    svg.setAttribute("viewBox", '0, 0, 500, 500');
    svg.setAttribute("id", id);
    parent.appendChild(svg);
    if (isIE) {
        doLater(
            function (svg) {
                var svgdoc = getSVGDocument(svg);
                var svgRoot = getSVGRoot(svg, svgdoc);
                svgRoot.setAttribute("height", '100%');
                svgRoot.setAttribute("width", '100%');
                svgRoot.setAttribute("viewBox", '0, 0, 500, 500');
            }, 100, svg);
    }
    return svg;
}
//---------Extending setTimeout method--------
function doLater(callback, timeout, param) {
    var args = Array.prototype.slice.call(arguments, 2);
    var _cb = function () {
        callback.apply(null, args);
    }
    setTimeout(_cb, timeout);
}

function percentNum(num, num2) {
    return (Math.round(num / num2 * 10000) / 100.00); //After the decimal point two percentage
}

function modo() {
    if (modeSelection == "Errors")
        return 0;
    if (modeSelection == "ErrorsAndTime")
        return 1;
    if (modeSelection == "EventsByZone")
        return 2;
    if (modeSelection == "NoClusters")
        return 3;
}

function showBar1() {
    if (showBar == false) {
        showBar = true;
        document.getElementById("barHome").classList.remove("hidden-xs");
        document.getElementById("barStudent").classList.remove("hidden-xs");
        document.getElementById("barDate").classList.remove("hidden-xs");
    }
    else {
        showBar = false;
        document.getElementById("barHome").classList.add("hidden-xs");
        document.getElementById("barStudent").classList.add("hidden-xs");
        document.getElementById("barDate").classList.add("hidden-xs");
    }
}

var resetSelector = function (value) {
    var phaseSelection = document.getElementById("phase");
    $.post(href + "Map/GetDomains", function (data) {
        var dataObj = JSON.parse(data);
        var domains = dataObj.Domains;
        phaseSelection.options.add(new Option("", 0));
        for (var i = 0; i < domains.length; i++) {
            phaseSelection.options.add(new Option(domains[i]));
        }
    });
    phaseSelection.options[0].selected = true;
    document.getElementById("cluster").options.length = 1;
    /*if (value == 0) {
        document.getElementById("phase").options[0].selected = true;
        document.getElementById("cluster").options.length = 1;
    }*/
    if (value == 1) {
        //document.getElementById("phase").options[0].selected = true;
        //document.getElementById("cluster").options.length = 1;
        document.getElementById("student").options.length = 1;
    }else if (value == 2) {
        document.getElementById("mode").options[0].selected = true;
       // document.getElementById("phase").options[0].selected = true;
       // document.getElementById("cluster").options.length = 1;
    }
}

var selectCluster = function (value) {
    if (value == 1) {
        document.getElementById("student").options.length = 1;
    }
    modeSelection = document.getElementById("mode").value;
    if (modeSelection == 0) {
        alert("Seleccione un modo de clusterización")
        return 0;
    }
    var phaseSelection = document.getElementById("phase");
    var clusterSelection = document.getElementById("cluster");
    clusterSelection.options.length = 0;
    TxtPhaseSelection = phaseSelection.options[phaseSelection.selectedIndex].text;
    if (TxtPhaseSelection.length > 0 && modeSelection.length > 0) {
        var modo1 = modo();
        $.post(href + "Map/GetClusters", { modo: modo1, strDomainName: TxtPhaseSelection }, function (data) {
            var dataObj = JSON.parse(data);
            clusters = dataObj.NumOfClusters;
            clusterSelection.options.add(new Option("", 0));
            for (var i = 0; i < clusters; i++) {
                clusterSelection.options.add(new Option("cluster " + i, i + 1));
            }
        });
    }
}

var selectClusterDate = function () {
    var dateSelection = document.getElementById("date").value;
    if (dateSelection.length == 10 && (dateSelection.indexOf("-")) == 4 && (dateSelection.lastIndexOf("-")) == 7) {
        var phaseSelection = document.getElementById("phase");
        var clusterSelection = document.getElementById("cluster");
        clusterSelection.options.length = 0;
        TxtPhaseSelection = phaseSelection.options[phaseSelection.selectedIndex].text;
        if (TxtPhaseSelection.length > 0 && dateSelection.length == 10) {
            $.post(href + "Map/GetClustersDate", { date: dateSelection, strDomainName: TxtPhaseSelection }, function (data) {
                var dataObj = JSON.parse(data);
                if (dataObj.ErrorDate == 1)
                    alert("La fecha es demasiado cercana, seleccione otra fecha")
                else {
                    clusters = dataObj.NumOfClusters;
                    clusterSelection.options.add(new Option("", 0));
                    for (var i = 0; i < clusters; i++) {
                        clusterSelection.options.add(new Option("cluster " + i, i + 1));
                    }
                }
            });
        }
    }
    else {
        if (isIE || isFirefox)
            alert("Formato incorrecto de fecha, introduzca una fecha con el formato aaaa-mm-dd");
        else
            alert("Seleccione una fecha");
    }
}

var selectStudent = function () {
    modeSelection = document.getElementById("mode").value;
    var phaseSelection = document.getElementById("phase");
    var clusterSelection = document.getElementById("cluster").value;
    var studentSelection = document.getElementById("student");
    studentSelection.options.length = 0;
    if (clusterSelection.length > 0) {
        var modo1 = modo();
        $.post(href + "Map/GetStudents", { strDomainName: TxtPhaseSelection, Cluster: clusterSelection, modo: modo1 }, function (data) {
            var dataObj = JSON.parse(data);
            //students = dataObj.NumOfStudents;
            studentSelection.options.add(new Option("", 0));
            for (var i = 0; i < dataObj.length; i++) {
                studentSelection.options.add(new Option("student " + dataObj[i], dataObj[i]));
            }
        });
    }
}
//=========================initiate cluster map==========================
function loadClusterMap() {
    if (CheckForm()) {
        phaseSelection = document.getElementById("phase");
        TxtPhaseSelection = phaseSelection.options[phaseSelection.selectedIndex].text;
        clusterSelection = document.getElementById("cluster").value;
        //================ Reset all attributes ================
        imgScale = 0.5;
        currentColumns = 0;
        clearAllDictionary();
        resetAllCounters();
        map = new Array(new Array());
        manager.currentScreen = {
            x: 0,
            y: 0,
            xScale: 10,
            yScale: 8
        }
        var nodeListContainer = document.getElementById("currentNodeContainer");
        var nodeList = document.getElementById("currentNode");
        if (nodeList != null) nodeListContainer.removeChild(nodeList);
        var arrowListContainer = document.getElementById("currentArrowContainer");
        var arrowList = document.getElementById("currentArrow");
        if (arrowList != null) arrowListContainer.removeChild(arrowList);
        modeSelection = document.getElementById("mode").value;
        var modo1 = modo();
        $.post(href + "Map/GetIniNode", { modo: modo1, strDomainName: TxtPhaseSelection, Cluster: clusterSelection }, function (data) {
            var parseData = JSON.parse(data);
            totalEvents = parseData.TotalEvents;
            totalStudent = parseData.TotalStudent;
            iniNode = new CorrectState();
            iniNode.Key = parseData.Key;
            iniNode.area = 0;
            iniNode.Frequency = parseData.FrequencyS;
            iniNode.FrequencyE = parseData.FrequencyE;
            iniNode.ShowingDetail = false;
            iniNode.id = 0;
            iniNode.through = false;
            iniNode.column = 0;
            instances++;
            CorrectStatesDic.Add(iniNode.Key, iniNode);
            map[0][0] = iniNode;
            LastStates.Add(iniNode.Key, iniNode);
            //loader.onLoading();
            loadMapParts();
            currentColumns++;
        });
    }
}

function loadStudentMap() {
    if (CheckForm()) {
        phaseSelection = document.getElementById("phase");
        TxtPhaseSelection = phaseSelection.options[phaseSelection.selectedIndex].text;
        clusterSelection = document.getElementById("cluster").value;
        studentSelection = document.getElementById("student").value;
        if (studentSelection == 0) {
            alert("please select a student!");
            return 0;
        }
        //================ Reset all attributes ================
        imgScale = 0.5;
        currentColumns = 0;
        clearAllDictionary();
        resetAllCounters();
        map = new Array(new Array());
        manager.currentScreen = {
            x: 0,
            y: 0,
            xScale: 10,
            yScale: 8
        }
        var nodeListContainer = document.getElementById("currentNodeContainer");
        var nodeList = document.getElementById("currentNode");
        if (nodeList != null) nodeListContainer.removeChild(nodeList);
        var arrowListContainer = document.getElementById("currentArrowContainer");
        var arrowList = document.getElementById("currentArrow");
        if (arrowList != null) arrowListContainer.removeChild(arrowList);
        //loader.onLoading();
        modeSelection = document.getElementById("mode").value;
        var modo1 = modo();
        totalStudent = 1;
        $.post(href + "Map/GetNodeStudents", { strDomainName: TxtPhaseSelection, Cluster: clusterSelection, StudentKey: studentSelection, modo: modo1 }, function (data) {
            $.each(data, function (i, items) {
                var newItem = JSON.parse(items);
                var nElement = newItem.Element;
                if (nElement == "NormalEvent") {
                    var nEvent = new NormalEvent();
                    nEvent.id = "NormalEvent" + normalEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.Frequency = newItem.Frequency;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    normalEventId++;
                }
                else if (nElement == "VectorEvent") {
                    var nEvent = new VectorEvent();
                    nEvent.id = "VectorEvent" + vectorEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.freqVect = newItem.Frequency;
                    var freq = 0;
                    for (i = 0; i < eval(newItem.Frequency).length; i++) {
                        freq += eval(newItem.Frequency)[i];
                    }
                    nEvent.Frequency = freq;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    vectorEventId++;
                }
                else if (nElement == "GroupEvent") {
                    var nEvent = new GroupEvent();
                    nEvent.id = "GroupEvent" + groupEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.Frequency = newItem.Frequency;
                    nEvent.Info = newItem.Info;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    groupEventId++;
                }
                else if (nElement == "state") {
                    var nStateType = newItem.StateType;
                    if (findNodeByNodeKey(newItem.Key) == null) {
                        $('#nodeList').append('<option value="' + newItem.Key + '"></option>');
                    }
                    if (nStateType == "Init") {
                        iniNode = new CorrectState();
                        iniNode.Key = newItem.Key;
                        iniNode.area = 0;
                        iniNode.Frequency = newItem.Frequency;
                        iniNode.ShowingDetail = false;
                        iniNode.id = 0;
                        iniNode.through = false;
                        iniNode.column = 0;
                        CorrectStatesDic.Add(iniNode.Key, iniNode);
                        map[0][0] = iniNode;
                        LastStates.Add(iniNode.Key, iniNode);
                        for (var i = 0; i < EventsDic.arrValues.length; i++) {
                            var key = EventsDic.arrValues[i];
                            if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                key.fromNode = nCurrentState.Key;
                                nCurrentState.ArcsOut.Add(key.id, key);
                            }
                            if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                key.toNode = nCurrentState.Key;
                                nCurrentState.ArcsIn.Add(key.id, key);
                            }
                        }
                    }
                    else if (nStateType == "Correct") {
                        if (!CorrectStatesDic.ContainsKey(newItem.Key) && !ErrorCorrectStatesDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new CorrectState();
                            nCurrentState.id = correctStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.actionName = newItem.Name;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "CorrectFlow") {
                                CorrectStatesDic.Add(nCurrentState.Key, nCurrentState);
                                nCurrentState.area = 0;
                            }
                            else {
                                ErrorCorrectStatesDic.Add(nCurrentState.Key, nCurrentState);
                                if (newItem.Area == "IrrelevantErrors")
                                    nCurrentState.area = -1;
                                else
                                    nCurrentState.area = 1;
                            }
                            correctStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "TimeError") {
                        if (!TimeErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new TimeErrorState()
                            nCurrentState.id = TimeErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.errorAssociated = newItem.Message;
                            nCurrentState.time = newItem.Time;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            TimeErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                            TimeErrorStateId++;
                        }
                    }
                    else if (nStateType == "IncompatibilityError") {
                        if (!IncompatibilityErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new IncompatibilityErrorState();
                            nCurrentState.id = IncompatibilityErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.incompatibilityFailed = newItem.Message;
                            nCurrentState.incompatibilityAction = newItem.Action;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            IncompatibilityErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            IncompatibilityErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "WorldError") {
                        if (!WorldErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new WorldErrorState();
                            nCurrentState.id = WorldErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.errorAssociated = newItem.Message;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            WorldErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            WorldErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "OtherError") {
                        if (!OtherErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new OtherErrorState();
                            nCurrentState.id = OtherErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Columns;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            OtherErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            OtherErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "SeqComplexDependenceError") {
                        if (!SeqComplexDependenceErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new SeqComplexDependenceErrorState();
                            nCurrentState.id = SeqComplexDependenceErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.DependenceError = newItem.Message;
                            nCurrentState.ComplexDependenceKey = newItem.ComplexDependenceKey;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            SeqComplexDependenceErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            SeqComplexDependenceErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "SimpleDependenceError") {
                        if (!SimpleDependenceErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new SimpleDependenceErrorState();
                            nCurrentState.id = SimpleDependenceErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.Frequency;
                            nCurrentState.DependenceError = newItem.Message;
                            nCurrentState.Action = newItem.Action;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            SimpleDependenceErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            SimpleDependenceErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    //=======================adding to grahpic====================
                }
            });//========$each======
            //----------------Getting iniNode----------------------------
            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                var key = EventsDic.arrValues[i];
                if (key.fromNode.Key == iniNode.Key && !iniNode.ArcsOut.ContainsKey(key.id)) {
                    key.fromNode = iniNode;
                    iniNode.ArcsOut.Add(key.id, key);
                }
            }
            //------------Drawing arrows--------------
            for (var i = 0 ; i < EventsDic.arrValues.length; i++) {
                if (EventsDic.arrValues[i].fromNode.x != null && EventsDic.arrValues[i].toNode.x != null && EventsDic.arrValues[i].isDrawn == false) {
                }
            }
            OrderMap();
            iniCall();
            manager.generateMap();
        });
    }
}

//================function for map loading========================
function loadMapParts() {
    modeSelection = document.getElementById("mode").value;
    var correctArray = new Array();
    correctArray[0] = iniNode;
    var fromNodes = iniNode.Key;
    document.getElementById('events1').value = 0;
    if (findNodeByNodeKey(iniNode.Key).through == false) {
        posted++;
        var modo1 = modo();
        $.post(href + "Map/GetAllIds", { modo: modo1, strDomainName: TxtPhaseSelection, Cluster: clusterSelection, fromNodeKey: iniNode.Key }, function (data) {
            $.each(data, function (i, items) {
                instances++;
                var newItem = JSON.parse(items);
                var nElement = newItem.Element;
                if (nElement == "NormalEvent") {
                    var nEvent = new NormalEvent();
                    nEvent.id = "NormalEvent" + normalEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.Frequency = newItem.Frequency;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    normalEventId++;
                }
                else if (nElement == "VectorEvent") {
                    var nEvent = new VectorEvent();
                    nEvent.id = "VectorEvent" + vectorEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.freqVect = newItem.Frequency;
                    var freq = 0;
                    for (i = 0; i < eval(newItem.Frequency).length; i++) {
                        freq += eval(newItem.Frequency)[i];
                    }
                    nEvent.Frequency = freq;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    vectorEventId++;
                }
                else if (nElement == "GroupEvent") {
                    var nEvent = new GroupEvent();
                    nEvent.id = "GroupEvent" + groupEventId;
                    nEvent.fromNode = newItem.NodeKey1;
                    nEvent.toNode = newItem.NodeKey2;
                    nEvent.Frequency = newItem.Frequency;
                    nEvent.Info = newItem.Info;
                    var fn = findNodeByNodeKey(newItem.NodeKey1);
                    var tn = findNodeByNodeKey(newItem.NodeKey2);
                    if (fn != null) {
                        if (!fn.ArcsOut.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey1).ArcsOut.Add(nEvent.id, nEvent);
                        }
                    }
                    if (tn != null) {
                        if (!tn.ArcsIn.ContainsKey(nEvent.id)) {
                            findNodeByNodeKey(newItem.NodeKey2).ArcsIn.Add(nEvent.id, nEvent);
                        }
                    }
                    EventsDic.Add(nEvent.id, nEvent);
                    groupEventId++;
                }
                else if (nElement == "state") {
                    var nStateType = newItem.StateType;
                    if (findNodeByNodeKey(newItem.Key) == null) {
                        $('#nodeList').append('<option value="' + newItem.Key + '"></option>');
                    }
                    if (nStateType == "Correct") {
                        if (!CorrectStatesDic.ContainsKey(newItem.Key) && !ErrorCorrectStatesDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new CorrectState();
                            nCurrentState.id = correctStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.actionName = newItem.Name;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "CorrectFlow") {
                                CorrectStatesDic.Add(nCurrentState.Key, nCurrentState);
                                nCurrentState.area = 0;
                            }
                            else {
                                ErrorCorrectStatesDic.Add(nCurrentState.Key, nCurrentState);
                                if (newItem.Area == "IrrelevantErrors")
                                    nCurrentState.area = -1;
                                else
                                    nCurrentState.area = 1;
                            }
                            correctStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "TimeError") {
                        if (!TimeErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new TimeErrorState()
                            nCurrentState.id = TimeErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.errorAssociated = newItem.Message;
                            nCurrentState.time = newItem.Time;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            TimeErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                            TimeErrorStateId++;
                        }
                    }
                    else if (nStateType == "IncompatibilityError") {
                        if (!IncompatibilityErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new IncompatibilityErrorState();
                            nCurrentState.id = IncompatibilityErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.incompatibilityFailed = newItem.Message;
                            nCurrentState.incompatibilityAction = newItem.Action;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            IncompatibilityErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            IncompatibilityErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "WorldError") {
                        if (!WorldErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new WorldErrorState();
                            nCurrentState.id = WorldErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.errorAssociated = newItem.Message;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            WorldErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            WorldErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "OtherError") {
                        if (!OtherErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new OtherErrorState();
                            nCurrentState.id = OtherErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = totalStudent;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            OtherErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            OtherErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "SeqComplexDependenceError") {
                        if (!SeqComplexDependenceErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new SeqComplexDependenceErrorState();
                            nCurrentState.id = SeqComplexDependenceErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.DependenceError = newItem.Message;
                            nCurrentState.ComplexDependenceKey = newItem.ComplexDependenceKey;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            SeqComplexDependenceErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            SeqComplexDependenceErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    else if (nStateType == "SimpleDependenceError") {
                        if (!SimpleDependenceErrorStateDic.ContainsKey(newItem.Key)) {
                            var nCurrentState = new SimpleDependenceErrorState();
                            nCurrentState.id = SimpleDependenceErrorStateId;
                            nCurrentState.Key = newItem.Key;
                            nCurrentState.column = newItem.Column;
                            nCurrentState.Frequency = newItem.FrequencyS;
                            nCurrentState.FrequencyE = newItem.FrequencyE;
                            nCurrentState.DependenceError = newItem.Message;
                            nCurrentState.Action = newItem.Action;
                            nCurrentState.ShowingDetail = false;
                            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                                var key = EventsDic.arrValues[i];
                                if (key.fromNode == nCurrentState.Key && !nCurrentState.ArcsOut.ContainsKey(key.id)) {
                                    key.fromNode = nCurrentState.Key;
                                    nCurrentState.ArcsOut.Add(key.id, key);
                                }
                                if (key.toNode == nCurrentState.Key && !nCurrentState.ArcsIn.ContainsKey(key.id)) {
                                    key.toNode = nCurrentState.Key;
                                    nCurrentState.ArcsIn.Add(key.id, key);
                                }
                            }
                            if (newItem.Area == "IrrelevantErrors")
                                nCurrentState.area = -1;
                            else
                                nCurrentState.area = 1;
                            SimpleDependenceErrorStateDic.Add(nCurrentState.Key, nCurrentState);
                            SimpleDependenceErrorStateId++;
                            LastStates.Add(nCurrentState.Key, nCurrentState);
                        }
                    }
                    //=======================adding to grahpic====================
                }
            });//========$each======
            //----------------Getting iniNode----------------------------
            for (var i = 0; i < EventsDic.arrValues.length; i++) {
                var key = EventsDic.arrValues[i];
                if (key.fromNode.Key == iniNode.Key && !iniNode.ArcsOut.ContainsKey(key.id)) {
                    key.fromNode = iniNode;
                    iniNode.ArcsOut.Add(key.id, key);
                }
            }
            //------------Drawing arrows--------------
            for (var i = 0 ; i < EventsDic.arrValues.length; i++) {
                if (EventsDic.arrValues[i].fromNode.x != null && EventsDic.arrValues[i].toNode.x != null && EventsDic.arrValues[i].isDrawn == false) {
                }
            }
            OrderMap();
            iniCall();
            manager.generateMap();
        });//======$post=====
    }//======if===== 
}

function OrderMap() {
    var j = 0;
    while (j < ErrorCorrectStatesDic.arrValues.length) {
        var i = 1;
        var fin = false;
        var nodo = ErrorCorrectStatesDic.arrValues[j];
        while (!fin) {
            if (map[nodo.column] == undefined)
                map[nodo.column] = new Array();
            if (map[nodo.column][i] == undefined) {
                nodo.area = i;
                nodo.x = nodo.column * 200 + 200;
                if (nodo.column % 2 == 0)
                    nodo.y = i * 150 + 160;
                else
                    nodo.y = i * 150 + 120;
                map[nodo.column][i] = nodo;
                fin = true
            }
            i++;
        }
        j++;
    }
    j = 1;
    while (j < LastStates.arrValues.length) {
        var nodo = LastStates.arrValues[j];
        if (nodo.area == 0) {
            if (map[nodo.column] == undefined)
                map[nodo.column] = new Array();
            nodo.x = nodo.column * 200 + 200;
            if (nodo.column % 2 == 0)
                nodo.y = 160;
            else
                nodo.y = 120;
            map[nodo.column][0] = nodo;
        }
        else if (nodo.area < 0) {
            var i = -1;
            var fin = false;
            while (!fin) {
                if (map[nodo.column] == undefined)
                    map[nodo.column] = new Array();
                if (map[nodo.column][i] == undefined) {
                    nodo.area = i;
                    if (Math.abs(i) % 2 == 0)
                        nodo.x = nodo.column * 200 + 230;
                    else
                        nodo.x = nodo.column * 200 + 170;
                    nodo.y = i * 150 + 150;
                    map[nodo.column][i] = nodo;
                    fin = true;
                    if (i < minMap)
                        minMap = i;
                }
                i--;
            }
        }
        else {
            var found = false;
            for (var k = 0; k < ErrorCorrectStatesDic.arrValues.length && !found; k++) {
                if (ErrorCorrectStatesDic.arrValues[k].Key == nodo.Key)
                    found = true;
            }
            if (!found) {
                var i = 1;
                var fin = false;
                while (!fin) {
                    if (map[nodo.column] == undefined)
                        map[nodo.column] = new Array();
                    if (map[nodo.column][i] == undefined) {
                        if (Math.abs(i) % 2 == 0)
                            nodo.x = nodo.column * 200 + 230;
                        else
                            nodo.x = nodo.column * 200 + 170;
                        nodo.y = i * 150 + 150;
                        map[nodo.column][i] = nodo;
                        fin = true
                    }
                    i++;
                }
            }
        }
        j++;
    }
}

function CheckForm() {
    var c = true;
    var phaseLength = document.getElementById("phase").value;
    var clusterLength = document.getElementById("cluster").value;
    if (phaseLength == 0 || clusterLength == 0) {
        alert("please select a phase and a cluster!");
        c = false;
    }
    return c;
}

//===========clear all dictionaries================
function clearAllDictionary() {
    CorrectStatesDic.Clear();
    TimeErrorStateDic.Clear();
    IncompatibilityErrorStateDic.Clear();
    WorldErrorStateDic.Clear();
    OtherErrorStateDic.Clear();
    SeqComplexDependenceErrorStateDic.Clear();
    SimpleDependenceErrorStateDic.Clear();
    EventsDic.Clear();
    LastStates.Clear();
    ErrorCorrectStatesDic.Clear();
}
//==============reset all counters========
function resetAllCounters() {
    normalEventId = 0;
    vectorEventId = 0;
    groupEventId = 0;
    correctStateId = 1;
    TimeErrorStateId = 1;
    IncompatibilityErrorStateId = 1;
    WorldErrorStateId = 1;
    OtherErrorStateId = 1;
    SeqComplexDependenceErrorStateId = 1;
    SimpleDependenceErrorStateId = 1;
    minMap = 0;
    showBar = false;
    arrowsDarkBlue = null;
    document.getElementById('nodes1').value = "0";
    document.getElementById('nodes').value = "0";
    document.getElementById('events1').value = "0";
    document.getElementById('events').value = "0";
    modeSelection = 0;
}
//========hidding the scroll==========
function init(value) {
    document.getElementById('body').setAttributeNS(null, 'style', 'height:' + window.innerHeight * 3 / 5 + 'px');
    document.getElementById('panelPrincipal').setAttributeNS(null, 'style', 'height:' + window.innerHeight * 3 / 5 + 'px');
    document.getElementById('svgid').setAttributeNS(null, 'style', 'height:' + window.innerHeight * 3 / 5 + 'px');
    var svgid = document.getElementById('svgid');
    var moveUp = document.getElementById('moveUp');
    var moveDown = document.getElementById('moveDown');
    var moveRight = document.getElementById('moveRight');
    var moveLeft = document.getElementById('moveLeft');
    var marginUp = document.getElementById('searchBar').clientWidth;
    var marginDown = document.getElementById('moveDownDiv').clientWidth;
    var marginRight = document.getElementById('svg').clientHeight;
    moveUp.setAttributeNS(null, 'style', 'margin-left:' + marginUp / 4 + 'px');
    moveDown.setAttributeNS(null, 'style', 'margin-left:' + (marginDown / 100) * 45 + 'px');
    moveRight.setAttributeNS(null, 'style', 'margin-top:' + (marginRight / 100) * 43 + 'px');
    moveLeft.setAttributeNS(null, 'style', 'margin-top:' + (marginRight / 100) * 28 + 'px');
    if (value == 1) {
        var viewBox = svg.getAttribute('viewBox');
        var viewBoxValues = viewBox.split(' ');
        viewBoxValues[0] = parseFloat(viewBoxValues[0]);
        viewBoxValues[1] = parseFloat(viewBoxValues[1]);
        viewBoxValues[2] = parseFloat(viewBoxValues[2]);
        viewBoxValues[3] = parseFloat(viewBoxValues[3]);
        moveUp.setAttributeNS
    }
    if (value == 2 && (isFirefox || isIE || isSafari))
        document.getElementById("date-message").innerHTML = "Introduzca una fecha con el formato aaaa-mm-dd";
}

function MapManager() {
    this.currentScreen = null;
    this.generateMap = function () {
        for (var i = 0; i < map.length; i++) {
            if (map[i] != undefined) {
                for (var j = minMap; j < map[i].length; j++) {
                    if (map[i][j] != undefined && !map[i][j].isDrawn)
                        map[i][j].draw();
                }
            }
        }
    }
    this.regenerateMap = function () {
        var limit = document.getElementById("nodes1").value;
        for (var i = 0; i < map.length; i++) {
            if (map[i] != undefined) {
                for (var j = minMap; j < map[i].length; j++) {
                    if (map[i][j] != undefined && map[i][j].isDrawn)
                        map[i][j].unDraw();
                }
            }
        }
        for (var i = 0; i < map.length; i++) {
            if (map[i] != undefined) {
                for (var j = minMap; j < map[i].length; j++) {
                    if (map[i][j] != undefined) {
                        var freq = (parseInt(map[i][j].Frequency) / totalStudent) * 100;
                        if (freq >= limit)
                            map[i][j].draw();
                    }
                }
            }
        }
        for (var i = 0; i < EventsDic.Count; i++) {
            if (EventsDic.arrValues[i].isDrawn) {
                var enter = EventsDic.arrValues[i].fromNode;
                var salid = EventsDic.arrValues[i].toNode;
                enter = findNodeByNodeKey(enter);
                salid = findNodeByNodeKey(salid);
                if (!enter.isDrawn || !salid.isDrawn)
                    EventsDic.arrValues[i].unDraw();
            }
        }
    }
}

//------------Creating svg element--------------

// ======================= Initial call =========================
function iniCall() {
    var body = document.getElementById('svgContainer');
    var cleanSVG = document.getElementById('svgid');
    body.removeChild(cleanSVG);
    //========== Recreate svg element =====================
    svg = createSVG('svgid', body);
    svgdoc = getSVGDocument(svg);
    svgRoot = getSVGRoot(svg);
    //=======================================================
    var svgdef = svgdoc.createElementNS(twConstants.NS_SVG, 'defs');
    svgRoot.appendChild(svgdef);
    //==============Marker==============
    var svgmarker = svgdoc.createElementNS(twConstants.NS_SVG, 'marker');
    svgmarker.setAttribute("id", "arrow");
    svgmarker.setAttribute("refX", 0);
    svgmarker.setAttribute("refY", 0);
    svgmarker.setAttribute("markerWidth", 10);
    svgmarker.setAttribute("markerHeight", 10);
    svgmarker.setAttribute("orient", "auto");
    svgmarker.setAttribute("viewBox", "0, -3, 10, 6");
    svgdef.appendChild(svgmarker);
    var svgpath = svgdoc.createElementNS(twConstants.NS_SVG, 'path');
    svgpath.setAttribute("d", "M0 -3 L0 3 L10 0");
    svgpath.setAttribute("style", "fill: #ffff;");
    svgmarker.appendChild(svgpath);
    //================backgroud==================
    var background = svgdoc.createElementNS(twConstants.NS_SVG, "rect");
    background.setAttribute("id", "background");
    background.setAttribute("x", "-1000");
    background.setAttribute("y", "-1000");
    background.setAttribute("width", "20000px");
    background.setAttribute("height", "20000px");
    background.setAttribute("fill", "blue");
    background.setAttribute("opacity", 0);
    svgRoot.appendChild(background);
    //--------------move screen----------------
    background.onmousedown = function (event) {
        var viewBox = svg.getAttribute('viewBox');	// Grab the object representing the SVG element's viewBox attribute.
        var viewBoxValues = viewBox.split(' ');				// Create an array and insert each individual view box attribute value (assume they're seperated by a single whitespace character).
        var bx = background.getAttribute('x');
        var by = background.getAttribute('y');
        var pos = windowToSVG(svg, event.clientX, event.clientY);
        viewBoxValues[0] = parseFloat(viewBoxValues[0]);	// Represent the x-coordinate on the viewBox attribute.
        viewBoxValues[1] = parseFloat(viewBoxValues[1]);	// Represent the y coordinate on the viewBox attribute.
        viewBoxValues[2] = parseFloat(viewBoxValues[2]);
        viewBoxValues[3] = parseFloat(viewBoxValues[3]);
        var tempx = 0;
        var tempy = 0;
        background.onmousemove = function (event) {
            svg.style.cursor = "move";
            var pos1 = windowToSVG(svg, event.clientX, event.clientY);
            var x = pos1.x - pos.x;
            var y = pos1.y - pos.y;
            pos = pos1;
            var t = viewBoxValues[0] - x * imgScale;
            if (t > 0) {
                viewBoxValues[0] -= x * imgScale;
                tempx -= x * imgScale;
                if (tempx > 200) {
                    tempx = 0;
                    manager.currentScreen.x++;
                }
                else if (tempx < -200) {
                    tempx = 0;
                    manager.currentScreen.x--;
                }
                bx -= x * imgScale;
                background.setAttribute("x", bx);
            }
            viewBoxValues[1] -= y * imgScale;
            tempy -= y * imgScale;
            if (tempy > 100) {
                tempy = 0;
                manager.currentScreen.y++;
            }
            else if (tempy < -100) {
                tempy = 0;
                manager.currentScreen.y--;
            }
            by -= y * imgScale;
            background.setAttribute("y", by);
            svg.setAttribute('viewBox', viewBoxValues.join(' '));
        }
        background.onmouseup = function () {
            background.onmousemove = null;
            background.onmouseup = null;
            svg.style.cursor = "default";
            deselectNode();
        }
        //----------------------zoom----------------------
        svg.onmousewheel = svg.onwheel = function (event) {
            var viewBox = svg.getAttribute('viewBox');	// Grab the object representing the SVG element's viewBox attribute.
            var viewBoxValues = viewBox.split(' ');				// Create an array and insert each individual view box attribute value (assume they're seperated by a single whitespace character).
            /* The array is filled with strings, convert the first two viewBox values to floats: */
            viewBoxValues[0] = parseFloat(viewBoxValues[0]);	// Represent the x-coordinate on the viewBox attribute.
            viewBoxValues[1] = parseFloat(viewBoxValues[1]);	// Represent the y coordinate on the viewBox attribute.
            viewBoxValues[2] = parseFloat(viewBoxValues[2]);
            viewBoxValues[3] = parseFloat(viewBoxValues[3]);
            var pos = windowToSVG(svg, event.clientX, event.clientY);
            if (event.wheelDelta > 0 && imgScale > 0.25) {
                imgScale /= 1.25;
                viewBoxValues[2] = 1000 * imgScale;
                viewBoxValues[3] = 1000 * imgScale;
                manager.currentScreen.xScale = Math.ceil(10 * imgScale);
                manager.currentScreen.yScale = Math.ceil(10 * imgScale);
                svg.setAttribute('viewBox', viewBoxValues.join(' '));
            } else if (event.wheelDelta < 0 && imgScale < 4) {
                imgScale *= 1.25;
                viewBoxValues[2] = 1000 * imgScale;
                viewBoxValues[3] = 1000 * imgScale;
                manager.currentScreen.xScale = Math.ceil(10 * imgScale);
                manager.currentScreen.yScale = Math.ceil(10 * imgScale);
                svg.setAttribute('viewBox', viewBoxValues.join(' '));
            }
        }
    }
}

//------------------locate pointer in svg element-----------------
function windowToSVG(svg, x, y) {
    var bbox = svg.getBoundingClientRect();
    return {
        x: x - bbox.left,
        y: y - bbox.top
    };
}

//-------------------loop up events from Dics------------------
function findEventByEventKey(EventKey) {
    for (var i = 0; i < EventsDic.arrValues.length; i++) {
        if (EventsDic.arrValues[i].id == EventKey) {
            return EventsDic.arrValues[i];
        }
    }
}

//--------------------loop up nodes from Dics-------------------
function findNodeByNodeKey(NodeKey) {
    for (var i = 0; i < CorrectStatesDic.arrValues.length; i++) {
        if (CorrectStatesDic.arrValues[i].Key == NodeKey) {
            return CorrectStatesDic.arrValues[i];
        }
    }
    for (var i = 0; i < ErrorCorrectStatesDic.arrValues.length; i++) {
        if (ErrorCorrectStatesDic.arrValues[i].Key == NodeKey) {
            return ErrorCorrectStatesDic.arrValues[i];
        }
    }
    for (var i = 0; i < TimeErrorStateDic.arrValues.length; i++) {
        if (TimeErrorStateDic.arrValues[i].Key == NodeKey) {
            return TimeErrorStateDic.arrValues[i];
        }
    }
    for (var i = 0; i < IncompatibilityErrorStateDic.arrValues.length; i++) {
        if (IncompatibilityErrorStateDic.arrValues[i].Key == NodeKey) {
            return IncompatibilityErrorStateDic.arrValues[i];
        }
    }
    for (var i = 0; i < WorldErrorStateDic.arrValues.length; i++) {
        if (WorldErrorStateDic.arrValues[i].Key == NodeKey) {
            return WorldErrorStateDic.arrValues[i];
        }
    }
    for (var i = 0; i < OtherErrorStateDic.arrValues.length; i++) {
        if (OtherErrorStateDic.arrValues[i].Key == NodeKey) {
            return OtherErrorStateDic.arrValues[i];
        }
    }
    for (var i = 0; i < SeqComplexDependenceErrorStateDic.arrValues.length; i++) {
        if (SeqComplexDependenceErrorStateDic.arrValues[i].Key == NodeKey) {
            return SeqComplexDependenceErrorStateDic.arrValues[i];
        }
    }
    for (var i = 0; i < SimpleDependenceErrorStateDic.arrValues.length; i++) {
        if (SimpleDependenceErrorStateDic.arrValues[i].Key == NodeKey) {
            return SimpleDependenceErrorStateDic.arrValues[i];
        }
    }
    return null;
}

//--------------------Inputs events--------------------------
function changeNodes(val) {
    manager.regenerateMap();
}

function changeEvents(val) {
    manager.regenerateMap();
}

function changeRange(val) {
    document.getElementById('nodes').value = val;
}
function changeNumber(val) {
    document.getElementById('nodes1').value = val;
}

function changeRange1(val) {
    document.getElementById('events').value = val;
}

function changeNumber1(val) {
    document.getElementById('events1').value = val;
}

function move(iniM) {
    var viewBox = svg.getAttribute('viewBox');	// Grab the object representing the SVG element's viewBox attribute.
    var viewBoxValues = viewBox.split(' ');				// Create an array and insert each individual view box attribute value (assume they're seperated by a single whitespace character).
    var bx = background.getAttribute('x');
    var by = background.getAttribute('y');
    viewBoxValues[0] = parseFloat(viewBoxValues[0]);	// Represent the x-coordinate on the viewBox attribute.
    viewBoxValues[1] = parseFloat(viewBoxValues[1]);	// Represent the y coordinate on the viewBox attribute.
    viewBoxValues[2] = parseFloat(viewBoxValues[2]);
    viewBoxValues[3] = parseFloat(viewBoxValues[3]);
    var tempx = 0;
    var tempy = 0;
    var x = 0;
    var y = 0;
    if (iniM > 0) {//vertical
        if (iniM == 1)//up
            y = -300;
        else//down
            y = 300;
    }
    else {//horizontal
        if (iniM == -1)//Left
            x = 300;
        else //Right
            x = -300;
    }
    var t = viewBoxValues[0] - x * imgScale;
    viewBoxValues[0] -= x * imgScale;
    tempx -= x * imgScale;
    bx -= x * imgScale;
    background.setAttribute("x", bx);
    viewBoxValues[1] -= y * imgScale;
    tempy -= y * imgScale;
    by -= y * imgScale;
    background.setAttribute("y", by);
    svg.setAttribute('viewBox', viewBoxValues.join(' '));
}

function zoom(iniZ) {
    var viewBox = svg.getAttribute('viewBox');	// Grab the object representing the SVG element's viewBox attribute.
    var viewBoxValues = viewBox.split(' ');				// Create an array and insert each individual view box attribute value (assume they're seperated by a single whitespace character).
    /* The array is filled with strings, convert the first two viewBox values to floats: */
    viewBoxValues[0] = parseFloat(viewBoxValues[0]);	// Represent the x-coordinate on the viewBox attribute.
    viewBoxValues[1] = parseFloat(viewBoxValues[1]);	// Represent the y coordinate on the viewBox attribute.
    viewBoxValues[2] = parseFloat(viewBoxValues[2]);
    viewBoxValues[3] = parseFloat(viewBoxValues[3]);
    if (iniZ < 0 && imgScale > 0.25) { //zoomIn
        imgScale /= 1.25;
        viewBoxValues[2] = 1000 * imgScale;
        viewBoxValues[3] = 1000 * imgScale;
        manager.currentScreen.xScale = Math.ceil(10 * imgScale);
        manager.currentScreen.yScale = Math.ceil(10 * imgScale);
        svg.setAttribute('viewBox', viewBoxValues.join(' '));
    }
    else if (iniZ > 0 && imgScale < 4) { //zoomOut
        imgScale *= 1.25;
        viewBoxValues[2] = 1000 * imgScale;
        viewBoxValues[3] = 1000 * imgScale;
        manager.currentScreen.xScale = Math.ceil(10 * imgScale);
        manager.currentScreen.yScale = Math.ceil(10 * imgScale);
        svg.setAttribute('viewBox', viewBoxValues.join(' '));
    }
}

//--------------------Nodes events---------------------------
function mouseOverNodes(evt) {
    svg.style.cursor = "pointer";
    evt.target.setAttributeNS(null, "opacity", 0.5);
}
function mouseOutNodes(evt) {
    svg.style.cursor = "default";
    var id = evt.target.getAttribute("id");
    var nodo = findNodeByNodeKey(id.replace('Cubierta', ''));
    if (nodo.through)
        evt.target.setAttributeNS(null, "opacity", 0.25);
    else
        evt.target.setAttributeNS(null, "opacity", 0);
    var node = findNodeByNodeKey(evt.target.id);
}
function mouseDownNodes(evt) {
    selectedElement = evt.target;
    var id = evt.target.id;
    var posi = id.search("Cubierta");
    id = id.substring(0, posi);
    var selectedNode = findNodeByNodeKey(id);
    var pos = windowToSVG(svg, event.clientX, event.clientY);
    var pos1 = null;
    var x = 0;
    var y = 0;
    var currentMatrix = selectedElement.getAttributeNS(null, "transform").slice(7, -1).split(' ');
    for (var i = 0; i < currentMatrix.length; i++) {
        currentMatrix[i] = parseFloat(currentMatrix[i]);
    }
    selectedElement.onmousemove = function (evt) {
        svg.style.cursor = "move";
        pos1 = windowToSVG(svg, event.clientX, event.clientY);
        x = pos1.x - pos.x;
        y = pos1.y - pos.y;
        selectedNode.x += x * imgScale*1.7;
        selectedNode.y += y * imgScale*1.7;
        selectedNode.reDraw();
        pos = pos1;
    }
    selectedElement.onmouseup = function () {
        svg.style.cursor = "pointer";
        deselectNode();
    }
}
function deselectNode() {
    if (selectedElement != null) {
        selectedElement.onmousemove = null;
        selectedElement.onmouseup = null;
        selectedElement = null;
    }
}

//---------------Arrows Events---------------------------
function mouseOverArrows(evt) {
    var pos = windowToSVG(svg, evt.clientX, evt.clientY);
    svg.style.cursor = "pointer";
    var showline = document.getElementById(evt.target.id.replace("Back", ""));
    showline.setAttributeNS(null, "opacity", 0.5);
    showline.style.stroke = "#00B2EE";
    var showTxt = document.getElementById(evt.target.id.replace("Back", "") + "TextFrequency");
    if (showTxt.innerHTML.length > 11) {
        var freqstr = showTxt.innerHTML;
        showTxt.innerHTML = freqstr.slice(11);
    }
    showTxt.setAttributeNS(null, "display", "");
}

function mouseOutArrows(evt) {
    svg.style.cursor = "default";
    var id = evt.target.id;
    var showline = document.getElementById(evt.target.id.replace("Back", ""));
    showline.setAttributeNS(null, "opacity", 1);
    var fstr = showline.getAttribute('data-freq');
    var freq = parseInt(fstr);
    var event = findEventByEventKey(id);
    var desde = event.fromNode;
    var hasta = event.toNode;
    var desde1 = findNodeByNodeKey(desde);
    var hasta1 = findNodeByNodeKey(hasta);
    if (desde1.through)
        showline.style.stroke = "9933FF"
    else if (hasta1.through)
        showline.style.stroke = "33FFFF"
    else {
        var rgb = 200 - freq * 2;
        showline.style.stroke = "rgb(" + rgb + "," + rgb + "," + rgb + ")";
    }
    var showFrequency = document.getElementById(evt.target.id.replace("Back", "") + "TextFrequency");
    showFrequency.setAttributeNS(null, "display", "none");
}

//---------------------create classes-------------------------------
//------------------------State classes----------------------------
function State() {
    this.id = null;
    this.column = null;
    this.area = null;
    this.Key = null;
    this.Frequency = null;
    this.FrequencyE = null;
    this.isDrawn = false;
    this.through = false;
    this.x = this.column * 200 + 200;
    this.y = this.area * 150 + 150;
    this.unDraw = function () {
        this.Node = document.getElementById(this.Key);
        this.text = document.getElementById(this.Key + "Text");
        this.Node1 = document.getElementById(this.Key + "Cubierta")
        svgRoot.removeChild(this.Node);
        svgRoot.removeChild(this.text);
        svgRoot.removeChild(this.Node1);
        this.isDrawn = false;
        if (this.ShowingDetail) {
            this.Detail = document.getElementById(this.Key + "Detail");
            this.DetailText = document.getElementById(this.Key + "DetailText");
            svgRoot.removeChild(this.Detail);
            svgRoot.removeChild(this.DetailText);
            this.ShowingDetail = false;
        }
        for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
            this.ArcsOut.arrValues[i].unDraw();
        }
        for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
            this.ArcsIn.arrValues[i].unDraw();
        }
    }
    this.reDraw = function () {
        this.Node = svgdoc.getElementById(this.Key);
        this.Node.setAttributeNS(null, "x", this.x);
        this.Node.setAttributeNS(null, "y", this.y);
        this.Node1.setAttributeNS(null, "x", this.x);
        this.Node1.setAttributeNS(null, "y", this.y);
        this.text.setAttributeNS(null, "x", this.x + 5);
        this.text.setAttributeNS(null, "y", this.y + 10);
        var key = this.Key;
        var pos = key.lastIndexOf('_');
        if (pos != -1)
            key = key.substring(0, pos);
        if (this.other == null) {
            var aux_size = key.length * 4 + 20;
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x + 7 - aux_size / 2);
        }
        else {
            var aux_size = key.length * 4 + 30;
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x + 7 - aux_size / 2);
            this.text.setAttributeNS(null, "y", this.y + 20);
        }
        for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
            this.ArcsOut.arrValues[i].reDraw();
        }
        for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
            this.ArcsIn.arrValues[i].reDraw();
        }
    }
}
//====================================================================
function CorrectState() {
    this.actionName = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:#00FF80");
            else
                this.Node.setAttributeNS(null, "style", "stroke:#00FF80;stroke-width:5;fill:#00FF80");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Key + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            this.text.textContent = this.Key;
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7)
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
CorrectState.prototype = new State();
//====================================================================
function TimeErrorState() {
    this.time = null;
    this.errorAssociated = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:#FFFF00");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:#FFFF00");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7)
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
TimeErrorState.prototype = new State();
//====================================================================
function IncompatibilityErrorState() {
    this.incompatibilityFailed = null;
    this.incompatibilityAction = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:rgb(215,104,89)");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:rgb(215,104,89)");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7)
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
IncompatibilityErrorState.prototype = new State();
//====================================================================
function WorldErrorState() {
    this.errorAssociated = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:rgb(204,204,0)");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:rgb(204,204,0)");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7);
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
WorldErrorState.prototype = new State();
//====================================================================
function OtherErrorState() {
    this.other = true;
    this.info = null;
    this.errorAssociated = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:#F16AEF");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:#F16AEF");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "60px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 20;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 30;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "60px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7);
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
OtherErrorState.prototype = new State();
//====================================================================
function DependenceErrorState() {
    this.DependenceError = null;
}
DependenceErrorState.prototype = new State();
//====================================================================
function SeqComplexDependenceErrorState() {
    this.complexDependenceKey = null;
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:rgb(255,153,0)");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:rgb(255,153,0)");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(\"" + this.Key + "\")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7);
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
SeqComplexDependenceErrorState.prototype = new DependenceErrorState();
//====================================================================
function SimpleDependenceErrorState() {
    this.ArcsIn = new Dictionary();
    this.ArcsOut = new Dictionary();
    this.Action = null;
    this.draw = function () {
        if (!this.isDrawn) {
            this.Node = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node.id = this.Key;
            this.Node.setAttributeNS(null, "x", this.x);
            this.Node.setAttributeNS(null, "y", this.y);
            if (this.area > 0)
                this.Node.setAttributeNS(null, "style", "stroke:red;stroke-width:5;fill:#FF8000");
            else
                this.Node.setAttributeNS(null, "style", "stroke:yellow;stroke-width:5;fill:#FF8000");
            this.Node.setAttributeNS(null, "width", "50px");
            this.Node.setAttributeNS(null, "height", "30px");
            this.Node.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.text = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
            this.text.id = this.Node.id + "Text";
            var xp = this.x + 5;
            var yp = this.y + 10;
            this.text.setAttributeNS(null, "x", xp);
            this.text.setAttributeNS(null, "y", yp);
            this.text.setAttributeNS(null, "transform", "translate(-3, +4)");
            this.text.setAttributeNS(null, "font-size", 10);
            this.text.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
            this.text.setAttributeNS(null, "display", "");
            var key = this.Key;
            var pos = key.lastIndexOf('_');
            if (pos != -1)
                key = key.substring(0, pos);
            var aux_size = key.length * 4 + 20;
            this.text.textContent = key;
            this.Node1 = svgdoc.createElementNS(twConstants.NS_SVG, 'rect');
            this.Node1.id = this.Key + "Cubierta";
            this.Node1.setAttributeNS(null, "x", this.x);
            this.Node1.setAttributeNS(null, "y", this.y);
            this.Node1.setAttributeNS(null, "width", "50px");
            this.Node1.setAttributeNS(null, "height", "30px");
            this.Node1.setAttributeNS(null, "onmouseover", "mouseOverNodes(evt)");
            this.Node1.setAttributeNS(null, "onmouseout", "mouseOutNodes(evt)");
            this.Node1.setAttributeNS(null, "onmousedown", "mouseDownNodes(evt)");
            this.Node1.setAttributeNS(null, "transform", "matrix(1 0 0 1 0 0)");
            this.Node1.setAttributeNS(null, "onclick", "showCurrentNode(" + this.Key + ")");
            this.Node1.setAttributeNS(null, "width", aux_size);
            this.Node1.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.Node1.setAttributeNS(null, "style", "stroke-width:5");
            this.Node1.setAttribute("opacity", 0);
            this.Node.setAttributeNS(null, "width", aux_size);
            this.Node.setAttributeNS(null, "x", this.x - aux_size / 2);
            this.text.setAttributeNS(null, "x", this.x - aux_size / 2 + 7);
            this.isDrawn = true;
            var filtro = document.getElementById("events1").value;
            for (var i = 0; i < this.ArcsOut.arrValues.length; i++) {
                if (filtro < this.ArcsOut.arrValues[i].percentFreq || this.ArcsOut.arrValues[i].percentFreq == undefined) {
                    this.ArcsOut.arrValues[i].draw();
                    this.ArcsOut.arrValues[i].reDraw();
                }
            }
            for (var i = 0; i < this.ArcsIn.arrValues.length; i++) {
                if (filtro < this.ArcsIn.arrValues[i].percentFreq || this.ArcsIn.arrValues[i].percentFreq == undefined) {
                    this.ArcsIn.arrValues[i].draw();
                    this.ArcsIn.arrValues[i].reDraw();
                }
            }
            svgRoot.appendChild(this.Node);
            svgRoot.appendChild(this.text);
            svgRoot.appendChild(this.Node1);
        }
    }
}
SimpleDependenceErrorState.prototype = new DependenceErrorState();

//-------------------------------Events classes--------------------------
function Event() {
    this.id = null;
    this.fromNode = null;
    this.toNode = null;
    this.Frequency = null;
    this.percentFreq = null;
    this.isDrawn = false;
    this.reDraw = function () {
        var startP = findNodeByNodeKey(this.fromNode);
        var endP = findNodeByNodeKey(this.toNode);
        if (this.isDrawn) {
            if (startP.area < 0)
                this.percentFreq = this.Frequency / totalEvents;
            else
                this.percentFreq = percentNum(this.Frequency, startP.Frequency);
            if (this.arrow != null) {
                var degree = Math.atan2(endP.y - startP.y, endP.x - startP.x);
                this.arrow.setAttributeNS(null, "degree", degree);
                var key = startP.Key;
                var pos = key.lastIndexOf('_');
                if (pos != -1)
                    key = key.substring(0, pos);
                var x1, x2, y1, y2;
                var key1 = endP.Key;
                var pos = key1.lastIndexOf('_');
                if (pos != -1)
                    key1 = key1.substring(0, pos);
                if (startP.other != null) {
                    var size_aux = key.length * 4 + 30;
                    var size_auy = key1.length * 4 + 30;
                }
                else {
                    var size_aux = key.length * 4 + 20;
                    var size_auy = key1.length * 4 + 20;
                }
                var ran1 = Math.random();
                var ran2 = Math.random();
                if (ran1 < 0.1)
                    ran1 = 0.1;
                if (ran2 < 0.1)
                    ran2 = 0.1;
                if (ran1 > 0.9)
                    ran1 = 0.9;
                if (ran2 > 0.9)
                    ran2 = 0.9;
                if (degree < 0.79 && degree > -0.79) { //Derecha-izquierda
                    x1 = startP.x + size_aux / 2;
                    y1 = startP.y + 30 * ran1;
                    if (startP.other != null)
                        y1 = startP.y + 60 * ran1;
                    if (degree <= 0.27 && degree >= -0.27)
                        x2 = endP.x - 23 - size_auy / 2;
                    else
                        x2 = endP.x - 10 - 13 * (1 - (Math.abs(degree) / 0.79)) - size_auy / 2;
                    y2 = endP.y + 30 * ran2;
                    if (endP.other != null) {
                        y2 = endP.y + 60 * ran2;
                    }
                }
                else if (degree >= 0.79 && degree < 2.36) { //Abajo-arriba
                    x1 = startP.x - size_aux / 2 + size_aux * ran1;
                    y1 = startP.y + 30;
                    x2 = endP.x - size_auy / 2 + size_auy * ran2;
                    if (degree >= 1.31 && degree <= 1.84) {
                        y2 = endP.y - 23;
                    }
                    else {
                        if (degree < 1.57)
                            y2 = endP.y - 10 - 13 * (1 - (degree / 1.57));
                        else
                            y2 = endP.y - 10 - 13 * (1 - ((degree - 1.57) / 1.57));
                    }
                    if (startP.other != null) {
                        y1 = startP.y + 60;
                    }
                }
                else if (degree >= 2.36 || degree <= -2.36) { //Izquierda-derecha
                    x1 = startP.x - size_aux / 2;
                    y1 = startP.y + 30 * ran1;
                    if (degree <= -2.87 && degree >= 2.87)
                        x2 = endP.x + size_auy / 2 + 20;
                    else
                        x2 = endP.x + size_auy / 2 + 10 + 10 * (1 - (Math.abs(degree) / 3.15));
                    y2 = endP.y + 30 * ran2;
                    if (startP.other != null)
                        y1 = startP.y + 60 * ran1;
                    if (endP.other != null)
                        y2 = endP.y + 60 * ran2;
                }
                else { //Arriba-abajo -2.36 to -0.79
                    x1 = startP.x - size_aux / 2 + size_aux * ran1;
                    y1 = startP.y;
                    x2 = endP.x - size_auy / 2 + size_auy * ran2;
                    if (degree <= -1.31 && degree >= -1.84)
                        y2 = endP.y + 20 + 23;
                    else {
                        var pii = 30;
                        if (endP.other != null)
                            pii = 60;
                        if (degree > -1.57)
                            y2 = endP.y + pii + 10 + 13 * (1 - (Math.abs(degree) / 1.57));
                        else
                            y2 = endP.y + pii + 10 + 13 * (1 - ((Math.abs(degree) - 1.57) / 1.57));
                    }
                }
                this.arrow.setAttribute("x1", x1);
                this.arrow.setAttribute("y1", y1);
                this.arrow.setAttribute("x2", x2);
                this.arrow.setAttribute("y2", y2);
                if (this.percentFreq == Infinity)
                    this.percentFreq = 0;
                var freq = parseInt(this.percentFreq);
                var rgb = 200 - freq * 2;
                this.arrow.setAttribute("data-freq", this.percentFreq);
                this.arrow.setAttributeNS(null, "style", "stroke:rgb(" + rgb + "," + rgb + "," + rgb + ");stroke-width:2");
                this.arrowBack.setAttribute("x1", x1);
                this.arrowBack.setAttribute("y1", y1);
                this.arrowBack.setAttribute("x2", x2);
                this.arrowBack.setAttribute("y2", y2);
                this.arrowBack.setAttribute("opacity", 0);
                var xn, yn = 0;
                if (x1 > x2) {
                    if (x2 >= 0 || x1 <= 0)
                        xn = x1 - x2;
                    else
                        xn = x1 + Math.abs(x2);
                    xn = x1 - xn / 2;
                }
                else {
                    if (x1 >= 0 || x2 <= 0)
                        xn = x2 - x1;
                    else
                        xn = x2 + Math.abs(x1);
                    xn = x1 + xn / 2;
                }
                if (y1 > y2) {
                    if (y2 >= 0 || y1 <= 0)
                        yn = y1 - y2;
                    else
                        yn = y1 + Math.abs(y2);
                    yn = y1 - yn / 2;
                }
                else {
                    if (y1 >= 0 || y2 <= 0)
                        yn = y2 - y1;
                    else
                        yn = y2 + Math.abs(y1);
                    yn = y1 + yn / 2;
                }
                this.txtFrequency.setAttribute("x", xn);
                this.txtFrequency.setAttribute("y", yn);
            }
        }
    }
    this.unDraw = function () {
        if (this.isDrawn) {
            this.arrow = document.getElementById(this.id);
            this.arrowBack = document.getElementById(this.id + "Back");
            this.txtFrequency = document.getElementById(this.id + "TextFrequency");
            svgRoot.removeChild(this.arrowBack);
            svgRoot.removeChild(this.arrow);
            svgRoot.removeChild(this.txtFrequency);
            this.isDrawn = false;
        }
    }
}

function VectorEvent() {
    this.freqVect = null;
    this.draw = function () {
        var startP = findNodeByNodeKey(this.fromNode);
        var endP = findNodeByNodeKey(this.toNode);
        if (startP.x != null && endP.x != null) {
            if (!this.isDrawn) {
                if (startP.area < 0)
                    this.percentFreq = this.Frequency / totalEvents;
                else
                    this.percentFreq = percentNum(this.Frequency, startP.Frequency);
                var degree = Math.atan2(endP.y - startP.y, endP.x - startP.x);
                this.arrow = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrow.setAttribute("id", this.id);
                this.arrow.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrow.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrow.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrow.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                if (this.percentFreq == Infinity)
                    this.percentFreq = 0;
                var freq = parseInt(this.percentFreq);
                var rgb = 200 - freq * 2;
                this.arrow.setAttribute("data-freq", this.percentFreq);
                this.arrow.setAttributeNS(null, "style", "stroke:rgb(" + rgb + "," + rgb + "," + rgb + ");stroke-width:2");
                this.arrow.setAttribute("marker-end", "url(#arrow)");
                this.arrow.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrow.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrow.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrowBack.setAttribute("id", this.id + "Back");
                this.arrowBack.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrowBack.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrowBack.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrowBack.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                this.arrowBack.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack.setAttribute("opacity", 0);
                this.txtFrequency = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
                this.txtFrequency.id = this.arrow.id + "TextFrequency";
                this.txtFrequency.setAttributeNS(null, "transform", "translate(-3, +4)");
                this.txtFrequency.setAttributeNS(null, "font-size", 10);
                this.txtFrequency.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
                this.txtFrequency.setAttributeNS(null, "display", "none");
                this.txtFrequency.textContent = "Frequency: " + this.percentFreq;
                svgRoot.appendChild(this.arrowBack);
                svgRoot.appendChild(this.arrow);
                svgRoot.appendChild(this.txtFrequency);
                this.isDrawn = true;
            }
        }
    }
}
VectorEvent.prototype = new Event();

function NormalEvent() {
    this.draw = function () {
        var startP = findNodeByNodeKey(this.fromNode);
        var endP = findNodeByNodeKey(this.toNode);
        if (startP.x != null && endP.x != null) {
            if (!this.isDrawn) {
                if (startP.area < 0)
                    this.percentFreq = this.Frequency / totalEvents;
                else
                    this.percentFreq = percentNum(this.Frequency, startP.Frequency);
                var degree = Math.atan2(endP.y - startP.y, endP.x - startP.x);
                this.arrow = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrow.setAttribute("id", this.id);
                this.arrow.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrow.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrow.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrow.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                if (this.percentFreq == Infinity)
                    this.percentFreq = 0;
                var freq = parseInt(this.percentFreq);
                var rgb = 200 - freq * 2;
                this.arrow.setAttribute("data-freq", this.percentFreq);
                this.arrow.setAttributeNS(null, "style", "stroke:rgb(" + rgb + "," + rgb + "," + rgb + ");stroke-width:2");
                this.arrow.setAttribute("marker-end", "url(#arrow)");
                this.arrow.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrow.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrow.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrowBack.setAttribute("id", this.id + "Back");
                this.arrowBack.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrowBack.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrowBack.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrowBack.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                this.arrowBack.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack.setAttribute("opacity", 0);
                this.txtFrequency = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
                this.txtFrequency.id = this.arrow.id + "TextFrequency";
                this.txtFrequency.setAttributeNS(null, "transform", "translate(-3, +4)");
                this.txtFrequency.setAttributeNS(null, "font-size", 10);
                this.txtFrequency.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
                this.txtFrequency.setAttributeNS(null, "display", "none");
                this.txtFrequency.textContent = "Frequency: " + this.percentFreq;
                svgRoot.appendChild(this.arrowBack);
                svgRoot.appendChild(this.arrow);
                svgRoot.appendChild(this.txtFrequency);
                this.isDrawn = true;
            }
        }
    }
}
NormalEvent.prototype = new Event();

function GroupEvent() {
    this.Info = null;
    this.draw = function () {
        var startP = findNodeByNodeKey(this.fromNode);
        var endP = findNodeByNodeKey(this.toNode);
        if (startP.x != null && endP.x != null) {
            if (!this.isDrawn) {
                if (startP.area < 0)
                    this.percentFreq = this.Frequency / totalEvents;
                else
                    this.percentFreq = percentNum(this.Frequency, startP.Frequency);
                var degree = Math.atan2(endP.y - startP.y, endP.x - startP.x);
                this.arrow = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrow.setAttribute("id", this.id);
                this.arrow.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrow.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrow.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrow.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                if (this.percentFreq == Infinity)
                    this.percentFreq = 0;
                var freq = parseInt(this.percentFreq);
                var rgb = 200 - freq * 2;
                this.arrow.setAttribute("data-freq", this.percentFreq);
                this.arrow.setAttributeNS(null, "style", "stroke:rgb(" + rgb + "," + rgb + "," + rgb + ");stroke-width:2");
                this.arrow.setAttribute("marker-end", "url(#arrow)");
                this.arrow.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrow.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrow.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack = svgdoc.createElementNS(twConstants.NS_SVG, 'line');
                this.arrowBack.setAttribute("id", this.id + "Back");
                this.arrowBack.setAttribute("x1", 25 + startP.x + 25 * Math.cos(degree));
                this.arrowBack.setAttribute("y1", 25 + startP.y + 25 * Math.sin(degree));
                this.arrowBack.setAttribute("x2", 25 + endP.x - 45 * Math.cos(degree));
                this.arrowBack.setAttribute("y2", 25 + endP.y - 45 * Math.sin(degree));
                this.arrowBack.setAttributeNS(null, "onmouseover", "mouseOverArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onmouseout", "mouseOutArrows(evt)");
                this.arrowBack.setAttributeNS(null, "onclick", "showCurrentArrow(" + this.id + ")");
                this.arrowBack.setAttribute("opacity", 0);
                this.txtFrequency = svgdoc.createElementNS(twConstants.NS_SVG, 'text');
                this.txtFrequency.id = this.arrow.id + "TextFrequency";
                this.txtFrequency.setAttributeNS(null, "transform", "translate(-3, +4)");
                this.txtFrequency.setAttributeNS(null, "font-size", 10);
                this.txtFrequency.setAttributeNS(null, "font-family", "TimesNewRomanPS-BoldMT");
                this.txtFrequency.setAttributeNS(null, "display", "none");
                this.txtFrequency.textContent = "Frequency: " + this.percentFreq;
                svgRoot.appendChild(this.arrowBack);
                svgRoot.appendChild(this.arrow);
                svgRoot.appendChild(this.txtFrequency);
                this.isDrawn = true;
            }
        }
    }
}
GroupEvent.prototype = new Event();

//================================Class Dictionary=================================
function Dictionary() {
    var me = this;            //Guarde el puntero this a una variable me
    this.CompareMode = 1;        //Comparación del modelo de la igualdad fundamental, 0-- binaria; 1-- texto
    this.Count = 0;            //El número de elementos en el diccionario
    this.arrKeys = new Array();    //array clave
    this.arrValues = new Array();    //matriz de valores
    this.ThrowException = true;    //Cuando se encuentra un error, si se produce una excepción con la sentencia throw
    this.Item = function (key)        //El método para obtener un valor correspondiente a la clave especificada. Si no existe la clave, se produce una excepción
    {
        var idx = GetElementIndexInArray(me.arrKeys, key);
        if (idx != -1) {
            return me.arrValues[idx];
        }
        else {
            if (me.ThrowException)
                throw "Se produjo un error al recuperar las claves de valor correspondientes, no existe la clave.";
        }
    }
    this.Keys = function ()        //Obtiene todas las claves de una matriz
    {
        return me.arrKeys;
    }
    this.Values = function ()        //Obtiene una matriz de todos los valores
    {
        return me.arrValues;
    }
    this.Add = function (key, value)    //Agrega la clave especificada y el valor al diccionario
    {
        if (CheckKey(key)) {
            me.arrKeys[me.Count] = key;
            me.arrValues[me.Count] = value;
            me.Count++;
        }
        else {
            //if (me.ThrowException)
            //    throw "Se produjo un error al agregar claves y valores al diccionario, puede ser la clave o la clave ya existe válida.";
            console.log(me);
        }
    }
    this.BatchAdd = function (keys, values)        //Aumento elemento claves y valores variedad de lotes, si tiene éxito, añadir todas las entradas, devuelve true; de lo contrario, no agregue cualquier artículo, return false.
    {
        var bSuccessed = false;
        if (keys != null && keys != undefined && values != null && values != undefined) {
            if (keys.length == values.length && keys.length > 0)    //El número de elementos en las claves y valores de la matriz debe ser el mismo
            {
                var allKeys = me.arrKeys.concat(keys);    //Algunas combinaciones clave de diccionario Planicies Centrales y la nueva clave a una nueva matriz
                if (!IsArrayElementRepeat(allKeys))    //Probar si existe copia de la llave nueva matriz
                {
                    me.arrKeys = allKeys;
                    me.arrValues = me.arrValues.concat(values);
                    me.Count = me.arrKeys.length;
                    bSuccessed = true;
                }
            }
        }
        return bSuccessed;
    }
    this.Clear = function ()            //Clear in the dictionary all the keys and values
    {
        if (me.Count != 0) {
            me.arrKeys.splice(0, me.Count);
            me.arrKeys.length = 0;
            me.arrValues.length = 0;
            me.Count = 0;
        }
    }
    this.ContainsKey = function (key)    //Determine whether the dictionary contains the specified key
    {
        return GetElementIndexInArray(me.arrKeys, key) != -1;
    }
    this.ContainsValue = function (value)    //Determine whether the dictionary contains the specified value
    {
        return GetElementIndexInArray(me.arrValues, value) != -1;
    }
    this.Remove = function (key)        //Removes the specified key value from the dictionary
    {
        var idx = GetElementIndexInArray(me.arrKeys, key);
        if (idx != -1) {
            me.arrKeys.splice(idx, 1);
            me.arrValues.splice(idx, 1);
            me.Count--;
            return true;
        }
        else
            return false;
    }
    this.TryGetValue = function (key, defaultValue)    //Try to obtain a value corresponding to the specified key from the dictionary, if the specified key does not exist, returns the default value defaultValue
    {
        var idx = GetElementIndexInArray(me.arrKeys, key);
        if (idx != -1) {
            return me.arrValues[idx];
        }
        else
            return defaultValue;
    }
    this.ToString = function ()        //Returns a string value dictionary, arranged as follows: a comma-separated list of values semicolon comma-separated list of keys
    {
        if (me.Count == 0)
            return "";
        else
            return me.arrKeys.toString() + ";" + me.arrValues.toString();
    }
    function CheckKey(key)            //Check key eligibility, and whether the existing key repeat
    {
        if (key == null || key == undefined || key == "" || key == NaN)
            return false;
        return !me.ContainsKey(key);
    }
    function GetElementIndexInArray(arr, e)    //Obtain the specified element in the array index, if the element exists in the array, which returns the index; otherwise -1.
    {
        var idx = -1;    //The resulting index
        var i;        //Variable cycle for
        if (!(arr == null || arr == undefined || typeof (arr) != "object")) {
            try {
                for (i = 0; i < arr.length; i++) {
                    var bEqual;
                    if (me.CompareMode == 0)
                        bEqual = (arr[i] === e);    //Binary comparison
                    else
                        bEqual = (arr[i] == e);        //Text Comparison
                    if (bEqual) {
                        idx = i;
                        break;
                    }
                }
            }
            catch (err) {
            }
        }
        return idx;
    }
    function IsArrayElementRepeat(arr)    //Analyzing duplication of an array element exists, if there are duplicate elements exist, returns true, otherwise false.
    {
        var bRepeat = false;
        if (arr != null && arr != undefined && typeof (arr) == "object") {
            var i;
            for (i = 0; i < arr.length - 1; i++) {
                var bEqual;
                if (me.CompareMode == 0)
                    bEqual = (arr[i] === arr[i + 1]);    //Binary comparison
                else
                    bEqual = (arr[i] == arr[i + 1]);        //Text Comparison
                if (bEqual) {
                    bRepeat = true;
                    break;
                }
            }
        }
        return bRepeat;
    }
}

//=============search function========================
function goToNode() {
    var searchNode = document.getElementById("searchNode").value;
    var state = findNodeByNodeKey(searchNode);
    if (state != null) {
        var viewBox = svg.getAttribute('viewBox');
        var background = document.getElementById("background");
        var viewBoxValues = viewBox.split(' ');
        viewBoxValues[0] = parseFloat(viewBoxValues[0]);	// Represent the x-coordinate on the viewBox attribute.
        viewBoxValues[1] = parseFloat(viewBoxValues[1]);	// Represent the y coordinate on the viewBox attribute.
        viewBoxValues[2] = parseFloat(viewBoxValues[2]);
        viewBoxValues[3] = parseFloat(viewBoxValues[3]);
        viewBoxValues[0] = state.x - viewBoxValues[2] / 4;
        viewBoxValues[1] = state.y - viewBoxValues[3] / 8;
        svg.setAttribute('viewBox', viewBoxValues.join(' '));
        background.setAttribute("x", viewBoxValues[0] - 1000);
        background.setAttribute("y", viewBoxValues[1] - 1000);
        showCurrentNode(state);
    }
    else alert(searchNode + " not found!");
}

function createSVGtspan(textDoc, caption, x, y) {
    if (caption == null) return 0;
    caption = caption.toString();
    var MAXIMUM_CHARS_PER_LINE = 30;
    var LINE_HEIGHT = 16;
    var words = caption.split(" ");
    var line = "";
    var num = 0;
    for (var n = 0; n < words.length; n++) {
        var testLine = line + words[n] + " ";
        if (testLine.length > MAXIMUM_CHARS_PER_LINE) {
            // Add a new <tspan> element
            var svgTSpan = document.createElementNS(twConstants.NS_SVG, 'tspan');
            svgTSpan.setAttributeNS(null, 'x', x);
            svgTSpan.setAttributeNS(null, 'y', y);
            var tSpanTextNode = document.createTextNode(line);
            svgTSpan.appendChild(tSpanTextNode);
            textDoc.appendChild(svgTSpan);
            line = words[n] + " ";
            y += LINE_HEIGHT;
            num++;
        }
        else {
            line = testLine;
        }
    }
    var svgTSpan = document.createElementNS(twConstants.NS_SVG, 'tspan');
    svgTSpan.setAttributeNS(null, 'x', x);
    svgTSpan.setAttributeNS(null, 'y', y);
    var tSpanTextNode = document.createTextNode(line);
    svgTSpan.appendChild(tSpanTextNode);
    textDoc.appendChild(svgTSpan);
    num++;
    return num;
}

//=============================================
function showCurrentNode(nodeKey) {
    var node = null;
    if (nodeKey.Key == undefined)
        node = findNodeByNodeKey(nodeKey.id);
    else
        node = findNodeByNodeKey(nodeKey.Key);
    if (node == undefined)
        node = findNodeByNodeKey(nodeKey);
    if (node != null) {
        if (node.through == false) {
            if (arrowsDarkBlue != null)
                showCurrentNode(arrowsDarkBlue);
            node.through = true;
            for (var i = 0; i < node.ArcsIn.arrValues.length; i++) {
                if (node.ArcsIn.arrValues[i].isDrawn) {
                    var idaux = node.ArcsIn.arrValues[i].id;
                    var showline = document.getElementById(node.ArcsIn.arrValues[i].id.replace("Back", ""));
                    showline.setAttributeNS(null, "opacity", 1);
                    showline.style.stroke = "33FFFF";
                }
            }
            for (var j = 0; j < node.ArcsOut.arrValues.length; j++) {
                if (node.ArcsOut.arrValues[j].isDrawn) {
                    var idaux = node.ArcsOut.arrValues[j].id;
                    var showline = document.getElementById(node.ArcsOut.arrValues[j].id.replace("Back", ""));
                    showline.setAttributeNS(null, "opacity", 1);
                    showline.style.stroke = "9933FF";
                }
            }
            var cubierta = document.getElementById(node.Key + "Cubierta");
            cubierta.setAttributeNS(null, "opacity", 0.25);
            arrowsDarkBlue = nodeKey;
        }
        else {
            arrowsDarkBlue = null;
            node.through = false;
            for (var i = 0; i < node.ArcsIn.arrValues.length; i++) {
                if (node.ArcsIn.arrValues[i].isDrawn) {
                    var showline = document.getElementById(node.ArcsIn.arrValues[i].id.replace("Back", ""));
                    showline.setAttributeNS(null, "opacity", 1);
                    var fstr = showline.getAttribute('data-freq');
                    var freq = parseInt(fstr);
                    var rgb = 200 - freq * 2;
                    showline.style.stroke = "rgb(" + rgb + "," + rgb + "," + rgb + ")";
                }
            }
            for (var j = 0; j < node.ArcsOut.arrValues.length; j++) {
                if (node.ArcsOut.arrValues[j].isDrawn) {
                    var showline = document.getElementById(node.ArcsOut.arrValues[j].id.replace("Back", ""));
                    showline.setAttributeNS(null, "opacity", 1);
                    var fstr = showline.getAttribute('data-freq');
                    var freq = parseInt(fstr);
                    var rgb = 200 - freq * 2;
                    showline.style.stroke = "rgb(" + rgb + "," + rgb + "," + rgb + ")";
                }
            }
            var cubierta = document.getElementById(node.Key + "Cubierta");
            cubierta.setAttributeNS(null, "opacity", 0);
        }
        var listContainer = document.getElementById("currentNodeContainer");
        var list = document.getElementById("currentNode");
        /*if (node.area < 0)
            var percent = node.Frequency / totalEvents * 100;
        else*/
        var percent = node.Frequency / totalStudent * 100;
        var percent1 = node.FrequencyE / totalEvents * 100;
        if (list != null) listContainer.removeChild(list);
        $('#currentNodeContainer').append('<dl class="dl-horizontal" id="currentNode"></dl>');
        $('#currentNode').append('<dt> State Key: </dt>');
        var hasta = node.Key.lastIndexOf('_');
        if (hasta == -1)
            $('#currentNode').append('<dd>' + node.Key + '</dd>');
        else
            $('#currentNode').append('<dd>' + node.Key.substring(0, hasta) + '</dd>');
        if(node.Frequency!=null){
            $('#currentNode').append('<dt> StudentFrequency: </dt>');
            $('#currentNode').append('<dd>' + node.Frequency + ' (' + percent.toFixed(2) + '%)' + '</dd>');
        }
        if(node.FrequencyE!=null){
            $('#currentNode').append('<dt> EventFrequency: </dt>');
            $('#currentNode').append('<dd>' + node.FrequencyE + ' (' + percent1.toFixed(2) + '%)' + '</dd>');
        }
        $('#currentNode').append('<dt> Area: </dt>');
        var areaAux = null;
        if (node.area == 0)
            areaAux = "CorrectFlow"
        else if (node.area < 0)
            areaAux = "IrrelevantErrors"
        else
            areaAux = "RelevantErrors"
        $('#currentNode').append('<dd>' + areaAux + '</dd>');
        if (node.actionName != null) {
            $('#currentNode').append('<dt> Action: </dt>');
            $('#currentNode').append('<dd>' + node.actionName + '</dd>');
        }
        if (node.errorAssociated != null) {
            $('#currentNode').append('<dt> Error: </dt>');
            $('#currentNode').append('<dd>' + node.errorAssociated + '</dd>');
        }
        if (node.time != null) {
            $('#currentNode').append('<dt> Time: </dt>');
            $('#currentNode').append('<dd>' + node.time + '</dd>');
        }
        if (node.type != null) {
            $('#currentNode').append('<dt> Type: </dt>');
            $('#currentNode').append('<dd>' + node.type + '</dd>');
        }
        if (node.incompatibilityAction != null) {
            $('#currentNode').append('<dt> Incompatibility Action: </dt>');
            $('#currentNode').append('<dd>' + node.incompatibilityAction + '</dd>');
        }
        if (node.incompatibilityFailed != null) {
            $('#currentNode').append('<dt> Incompatibility Failed: </dt>');
            $('#currentNode').append('<dd>' + node.incompatibilityFailed + '</dd>');
        }
        if (node.DependenceError != null) {
            $('#currentNode').append('<dt> Dependence Error: </dt>');
            $('#currentNode').append('<dd>' + node.DependenceError + '</dd>');
        }
        if (node.ComplexDependenceKey != null) {
            $('#currentNode').append('<dt> Complex DependenceKey: </dt>');
            $('#currentNode').append('<dd>' + node.ComplexDependenceKey + '</dd>');
        }
    }
}

function showCurrentArrow(arrow) {
    var arr = EventsDic.TryGetValue(arrow.id);
    if (arr != null) {
        var listContainer = document.getElementById("currentArrowContainer");
        var list = document.getElementById("currentArrow");
        var percent = arr.Frequency / totalStudent * 100;
        if (list != null) listContainer.removeChild(list);
        $('#currentArrowContainer').append('<dl class="dl-horizontal" id="currentArrow"></dl>');
        $('#currentArrow').append('<dt> Arrow Id: </dt>');
        $('#currentArrow').append('<dd>' + arr.id + '</dd>');
        $('#currentArrow').append('<dt> Frequency: </dt>');
        $('#currentArrow').append('<dd>' + arr.Frequency + ' (' + arr.percentFreq + '%)' + '</dd>');
        if (arr.freqVect != null) {
            $('#currentArrow').append('<dt> Vector Frequency: </dt>');
            $('#currentArrow').append('<dd>' + arr.freqVect + '</dd>');
        }
        $('#currentArrow').append('<dt> From: </dt>');
        $('#currentArrow').append('<dd>' + arr.fromNode + '</dd>');
        $('#currentArrow').append('<dt> To: </dt>');
        $('#currentArrow').append('<dd>' + arr.toNode + '</dd>');
        if (arr.Info != null) {
            $('#currentArrow').append('<dt> Info: </dt>');
            $('#currentArrow').append('<dd>' + arr.Info.replace(/====/g, "</dd><dd>") + '</dd>');
        }
    }
}