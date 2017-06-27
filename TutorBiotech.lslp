list Configuracion=[];
integer tiempoTimer=0;
integer lastUnixTime=0;
integer tiempoSiguiente=0;
list lsAvatares;
list lsLogs;
list lsPracticas;
list lsConfiguraciones;
list lsBloqueos;
list lsFases;
list lsUsuarios;

integer zonaHoraria=1;


string DeleteSubstring(string src, list substr){
    integer i;
    for(i=0;i<llGetListLength(substr);i++){
        integer index=llSubStringIndex(src,llList2String(substr,i));
        while(index>=0){
            src=llDeleteSubString(src,index,index);
            index=llSubStringIndex(src,llList2String(substr,i));
        }
    }
    return src;
}

AgregarErrorNotecard(key userKey, string linea){
    llMessageLinked(LINK_THIS, 1, linea, userKey);//Escritura de log de errores
}

string GetLineaLogAvatar(key Avatar, integer indexAccion){
    integer index= llListFindList(lsAvatares,[Avatar]);
    string content=llList2String(lsLogs,index);
    list lsContent=llParseString2List(content,"\n","");
    return llList2String(lsContent,indexAccion);
}

integer GetLengthLogAvatar(key Avatar){
    integer index= llListFindList(lsAvatares,Avatar);
    string content=llList2String(lsLogs,index);
    list lsContent=llParseString2List(content,"\n","");
    return llGetListLength(lsContent);
}

string GetCodPractica(key Avatar){
    list lsTemp=[];
    integer i=0;
    string codPractica;
    for(;i<llGetListLength(lsFases);i++){
        lsTemp=llParseString2List(llList2String(lsFases,i),"|","");
        if(llList2Key(lsTemp,0)==Avatar){
            codPractica=llList2String(lsTemp,2);
            jump Encontrado;
        }
    }
    @Encontrado;
    return codPractica;
}

integer GetFaseAvatar(key Avatar){
    list lsTemp=[];
    integer i=0;
    string fase;
    for(;i<llGetListLength(lsFases);i++){
        lsTemp=llParseString2List(llList2String(lsFases,i),"|","");
        if(llList2Key(lsTemp,0)==Avatar){
            fase=llList2String(lsTemp,1);
            fase=llGetSubString(fase,1,llStringLength(fase)-1);
            jump Encontrado;
        }
    }
    @Encontrado;
    return (integer)fase;
}

list GetConfiguracion(key Avatar){
    string codPractica=GetCodPractica(Avatar);
    return llParseString2List(llList2String(lsConfiguraciones,llListFindList(lsPracticas,codPractica)),"\n","");
}

list GetProtocolo(key Avatar){
    integer index= llListFindList(lsAvatares,[Avatar]);
    string content=llList2String(lsLogs,index);
    list lsContent=llParseString2List(content,"\n","");
    return lsContent;
}

ModificarLogAvatar(key Avatar, string content){
    integer index= llListFindList(lsAvatares,[Avatar]);
    lsLogs=llListReplaceList(lsLogs,[content],index,index);
}

//retorna al objeto (canal 2): 1-accion --> Válido
//                                 0-accion --> No válido bloqueante
//                                 -1-accion --> No válido no bloqueante
//la funcion retorna: Mensaje a escribir en nota
string ValidarAccion(string accion, key userKey, string objectName){
    list lineaConf=[];
    string encontrado="0";
    string errorDep="";
    string mostErrDep="1";
    string faseTarea="";
    string erroresdependencia=" ";
    string erroresincompatibilidad=" ";
 
    integer i=0;
    list lsConfiguracion=GetConfiguracion(userKey);
    integer num=llGetListLength(lsConfiguracion);
    for(i=1; i<num; i++) {
        string linea=llList2String(lsConfiguracion,i);
        integer indtmp = llSubStringIndex(linea,"}");
        if(indtmp>0)
            linea=llDeleteSubString(linea,indtmp,indtmp);
        if(llStringLength(llStringTrim(linea,STRING_TRIM))>0){
            lineaConf=llParseString2List(linea,["|"],[]);
            faseTarea=llList2String(lineaConf,0);
            //Obtener fase de la práctica en la que se encuentra el avatar
            integer faseLinea=(integer)llGetSubString(faseTarea,1,1);
            integer faseActual=ObtenerFaseActualDelAvatar(userKey);
            if(llList2String(lineaConf,16)=="1"){
                faseActual++;
            }
            if(llList2String(lineaConf,1)==accion && faseLinea==faseActual){
                //Validar si el objeto se encuentra bloqueado
                string banderaBloq=llList2String(lineaConf,9);
                if(ValidarBloqueo(objectName, userKey)==1){
                    llInstantMessage(userKey,"El objeto "+objectName+" está siendo utilizado por otro avatar");
                    AgregarErrorNotecard(userKey, "objetobloqueado|"+faseTarea+"|"+accion+"|"+objectName+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                    jump Error;
                }
                
                //Validar Acción ya realizada
                integer accRealizada=ValidarAccionYaRealizada(faseTarea, accion, userKey);
                string banderaAccRealizada=llList2String(lineaConf,14);
                if(accRealizada==1 && banderaAccRealizada=="1"){
                    llInstantMessage(userKey,"Error: Acción ya realizada.");
                    AgregarErrorNotecard(userKey, "accionrealizada|"+faseTarea+"|"+accion+"|"+accion+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                    jump Error;
                }else if(accRealizada==1 && banderaAccRealizada=="0"){
                    llRegionSay(2,"1"+accion+"|"+userKey);
                    return "";
                }
                
                //Validar Dependencia
                string lineadepend=llStringTrim(llList2String(lineaConf,4),STRING_TRIM);
                if(llStringLength(lineadepend)>0){
                    if(llGetSubString(lineadepend,0,0)=="["){
                        lineadepend=llGetSubString(lineadepend,1,llSubStringIndex(lineadepend,"]")-1);
                        erroresdependencia=DependenciasSinOrden(lineaConf, faseTarea, userKey, lineadepend,erroresdependencia);
                    }else if(llGetSubString(lineadepend,0,0)=="("){
                        lineadepend=llGetSubString(lineadepend,1,llSubStringIndex(lineadepend,")")-1);
                        erroresdependencia=DependenciasConOrden(lineaConf, faseTarea, userKey, lineadepend,erroresdependencia);
                        
                    }else{
                        llOwnerSay("Error: Formato de dependencias inválido.");
                        jump Error;
                    }
                    integer indexMsgError=llSubStringIndex(erroresdependencia,"error");
                    if(indexMsgError>=0){
                        indexMsgError=llSubStringIndex(erroresdependencia,"_");
                        llInstantMessage(userKey,llGetSubString(erroresdependencia,indexMsgError+1,llStringLength(erroresdependencia)-1));
                        jump Error;
                    }else{
                        integer indextmp=llSubStringIndex(lineadepend,"[");
                        if(indextmp>0)
                            lineadepend=llDeleteSubString(lineadepend,indextmp,indextmp);
                        indextmp=llSubStringIndex(lineadepend,"]");
                        if(indextmp>0)
                            lineadepend=llDeleteSubString(lineadepend,indextmp,indextmp);
                        indextmp=llSubStringIndex(lineadepend,"(");
                        if(indextmp>0)
                            lineadepend=llDeleteSubString(lineadepend,indextmp,indextmp);
                        indextmp=llSubStringIndex(lineadepend,")");
                        if(indextmp>0)
                            lineadepend=llDeleteSubString(lineadepend,indextmp,indextmp);
                        list dependenciastmp=llParseString2List(lineadepend,"-","");
                        integer countDep=0;
                        list depQuitarTiempo;
                        for(; countDep<llGetListLength(dependenciastmp);countDep++){
                            string faseTareaDep=llList2String(dependenciastmp,countDep);
                            if(((integer)BuscarCampoConfPorOtroCampo(faseTareaDep, 0, 10, userKey))>0){
                                depQuitarTiempo+=[faseTareaDep];
                            }
                        }
                        //Quitar tiempo de temporizador máximo
                        if(llGetListLength(depQuitarTiempo)>0){
                            string content;
                            integer linesTmp=0;
                            string userName=llKey2Name(userKey);
                            for(; linesTmp<GetLengthLogAvatar(userKey); linesTmp++) {
                                string lineaTmp=GetLineaLogAvatar(userKey, linesTmp);
                                list lstLineaTmp;
                                if(llStringLength(llStringTrim(lineaTmp,STRING_TRIM))>0){
                                    lstLineaTmp=llParseString2List(lineaTmp,["|"],[]);
                                    if(llListFindList(depQuitarTiempo,llList2String(lstLineaTmp,0))>=0){
                                        string tmpctn=llList2String(lstLineaTmp,0)+"|"+llList2String(lstLineaTmp,1)+"| |"+llList2String(lstLineaTmp,3)+"|"+llList2String(lstLineaTmp,4)+"|"+llList2String(lstLineaTmp,5)+"\n";
                                        content+=tmpctn;
                                    }else{
                                        content+=lineaTmp+"\n";
                                    }
                                }
                            }
                            
                            llMessageLinked(LINK_THIS, 0, content, userKey);//Escritura de log de acciones
                            ModificarLogAvatar(userKey,content);
                        }
                        if(llStringLength(llStringTrim(erroresdependencia,STRING_TRIM))>0)
                            AgregarErrorNotecard(userKey, "dependenciasnobloq|"+faseTarea+"|"+accion+"|"+erroresdependencia+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                    }
                }
                
                //Valida Incompatibilidades
                string lineaincompat=llStringTrim(llList2String(lineaConf,7),STRING_TRIM);
                if(llStringLength(lineaincompat)>0){
                    list incompatibilidades=llParseString2List(lineaincompat,["-"],[]);
                    integer countListIncom;
                    for(countListIncom=0;countListIncom<llGetListLength(incompatibilidades);countListIncom++){
                        string incompatibilidad=llList2String(incompatibilidades,countListIncom);
                        if(llStringLength(BuscarCampoLogPorOtroCampo(userKey, incompatibilidad, 0, 0))!=0){
                            string msgerrorincomp = llList2String(lineaConf,8);
                            if(llGetSubString(msgerrorincomp,0,0)!="1"){
                                if(llStringLength(llStringTrim(erroresincompatibilidad,STRING_TRIM))==0)
                                    erroresincompatibilidad=incompatibilidad;
                                else
                                    erroresincompatibilidad+="-"+incompatibilidad;
                            }
                            else{
                                llInstantMessage(userKey,llGetSubString(msgerrorincomp,2,llStringLength(msgerrorincomp)-1));
                                AgregarErrorNotecard(userKey, "incompatibilidadbloq|"+faseTarea+"|"+accion+"|"+incompatibilidad+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                                jump Error;
                            }
                        }
                    }
                    if(llStringLength(llStringTrim(erroresincompatibilidad,STRING_TRIM))>0)
                        AgregarErrorNotecard(userKey, "incompatibilidadnobloq|"+faseTarea+"|"+accion+"|"+erroresincompatibilidad+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));         
                }
                
                //Bloquear y desbloquear objeto
                if(banderaBloq=="1" || banderaBloq=="0"){
                    integer j=0;
                    string content = "";
                    integer numBloq=llGetListLength(lsBloqueos);
                    content = "";
                    for(j=0; j<=numBloq; j++) {
                        string lineabloqueo=llList2String(lsBloqueos,j);
                        integer index = llSubStringIndex(lineabloqueo,"|");
                        if(index<0)
                            index=llStringLength(lineabloqueo);
                        string object = llStringTrim(llGetSubString(lineabloqueo,0,index-1),STRING_TRIM);
                        if(llStringLength(object)>0){
                            if(object==objectName){
                                if(banderaBloq=="1")
                                    content+=objectName+"|"+userKey+"\n";
                                else if(banderaBloq=="0")
                                    content+=objectName+"|"+"\n";
                            }else
                                content+=lineabloqueo+"\n";
                        }
                    }
                    
                    llMessageLinked(LINK_THIS, 2, content, "");//Escritura de bloqueos
                    lsBloqueos=llParseString2List(content,"\n","");
                }
                //Validar si existen errores no bloqueantes en la fase
                if(llList2String(lineaConf,15)=="1"){
                    if(ValidarErrores(faseTarea, userKey)==1){
                        jump Error;
                    }
                }
                
                //Incrementar la fase en la nota de Fase por alumno
                if(llList2String(lineaConf,16)=="1"){
                    integer l=0;
                    string content = "";
                    for(l=0; l<=llGetListLength(lsFases); l++) {
                        string lineaFase=llList2String(lsFases,l);
                        integer indexFase = llSubStringIndex(lineaFase,"|");
                        string keyAvatarFase = llStringTrim(llGetSubString(lineaFase,0,indexFase-1),STRING_TRIM);
                        if(userKey==keyAvatarFase){
                            integer fase = (integer)llGetSubString(llStringTrim(llGetSubString(lineaFase,indexFase+1,llStringLength(lineaFase)-1),STRING_TRIM),1,1);
                            fase++;
                            content += userKey+"|f"+fase+"|"+GetCodPractica(userKey)+"\n";
                        }else{
                            content += lineaFase+"\n";
                        }
                    }
                    llMessageLinked(LINK_THIS, 3, content, "");//Escritura de fases
                    lsFases=llParseString2List(content,"\n","");
                }
                encontrado="1";
                jump Done;
            }
        }
    }

    if(encontrado=="0"){
        llInstantMessage(userKey,"Error: No se ha encontrado configuración para dicha acción, o es una acción de la fase anterior o posterior.");
        AgregarErrorNotecard(userKey, "accionnoencontrada| |"+accion+"| |"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
        jump Error;
    }
    @Done;
        string mensaje=llList2String(lineaConf,2);
        string tiempomax=llStringTrim(llList2String(lineaConf,10),STRING_TRIM);
        string tiempomin=llStringTrim(llList2String(lineaConf,11),STRING_TRIM);
        string mostrar=llGetSubString(mensaje,0,0);
        mensaje=llGetSubString(mensaje,2,llStringLength(mensaje)-1);
    
        string lineaLog="";
        string unixtime=" ";
        if(((llStringLength(tiempomin)>0 && ((integer)tiempomin)>0))||(llStringLength(tiempomax)>0 && ((integer)tiempomax)>0)){
            integer intUnixTime=llGetUnixTime();
            integer tiempoTranscurrido=0;
            if(lastUnixTime>0)
                tiempoTranscurrido=intUnixTime-lastUnixTime;
            integer tiempoRestante=tiempoTimer-tiempoTranscurrido;
            unixtime="|"+(string)intUnixTime;
            
            integer tiempotmp=0;
            if(llStringLength(tiempomin)>0){
                faseTarea+="_tmin";
                tiempotmp=(integer)tiempomin;
            }else{
                tiempotmp=(integer)tiempomax;
            }
            
            if((tiempotmp < tiempoRestante || tiempoRestante==0)){
                lastUnixTime=intUnixTime;
                tiempoTimer=tiempotmp;
                llSetTimerEvent(tiempotmp);
            }            
        }else{
            if(mostrar=="1"){
                llInstantMessage(userKey,mensaje);
            }
            string msgDialog=llStringTrim(llList2String(lineaConf,3),STRING_TRIM);
            if(llStringLength(msgDialog)>0){
                   integer index=llSubStringIndex(msgDialog,"\\n");
                   if(index>=0){
                    string resto=msgDialog;
                    msgDialog="";
                    while(index>=0){
                        string frase=llGetSubString(resto,0,index-1);
                        msgDialog+=llGetSubString(resto,0,index-1)+"\n";
                        resto=llGetSubString(resto,index+2,llStringLength(resto));
                        index=llSubStringIndex(resto,"\\n");
                    }
                    msgDialog+=llGetSubString(resto,0,llStringLength(resto)-1);
                }
                llDialog(userKey, msgDialog,["Aceptar"],-9999);
                llInstantMessage(userKey,msgDialog);
            }
        }
           
        lineaLog=faseTarea+"|"+accion+"|"+unixtime+"|"+erroresdependencia+"|"+erroresincompatibilidad+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600);
        if(llStringLength(llStringTrim(erroresdependencia,STRING_TRIM))>0)
            llRegionSay(2,"-1"+accion+"|"+userKey);
        else
            llRegionSay(2,"1"+accion+"|"+userKey);
        return lineaLog;
    @Error;
        llRegionSay(2,"0"+accion+"|"+userKey);
        return "";
}

integer ValidarAccionYaRealizada(string accKey, string accion, key userKey){
    string userName=osKey2Name(userKey);
    string keyAccionEnNota;
    string accionEnNota;
    integer i=0;
    integer realizado=0;
    for(i=0; i<GetLengthLogAvatar(userKey); i++) {
        string linea=llStringTrim(GetLineaLogAvatar(userKey, i),STRING_TRIM);
        integer index = llSubStringIndex(linea,"|");
        keyAccionEnNota=llGetSubString(linea,0,index-1);
        string resto=llGetSubString(linea,index+1,llStringLength(linea)-1);
        index = llSubStringIndex(resto,"|");
        accionEnNota=llGetSubString(resto,0,index-1);

        if(llStringLength(linea)>0){
            if((accKey==keyAccionEnNota || accKey+"_tmin"==keyAccionEnNota) && accion==accionEnNota){
                realizado=1;
                jump Fin;
            }
        }
    }
    @Fin;
        return realizado;
}

/*Obtiene un campo de la configuración.
  Parámetros: filtro --> Cadena por que se desea buscar.
                posFiltro --> Columna donde se desea buscar.
                posicionCampo --> Columna del campo a devolver
*/
string BuscarCampoConfPorOtroCampo(string filtro, integer posFiltro, integer posicionCampo, key userKey){
    string mensaje="";
    list lineaConf=[];
    integer i=0;
    list lsConfiguracion=GetConfiguracion(userKey);
    for(i=1; i<=llGetListLength(lsConfiguracion); i++) {
        string linea=llList2String(lsConfiguracion,i);
        if(llStringLength(llStringTrim(linea,STRING_TRIM))>0){
            lineaConf=llParseString2List(linea,["|"],[]);
            if(llList2String(lineaConf,posFiltro)==filtro){
                mensaje=llList2String(lineaConf,posicionCampo);
                jump Fin;
            }
        }
    }
    @Fin;
        return mensaje;
}

/*Obtiene un campo de un log de usuario.
  Parámetros: userKey --> UUID del avatar
                filtro --> Cadena por que se desea buscar.
                posFiltro --> Columna donde se desea buscar.
                posicionCampo --> Columna del campo a devolver
*/
string BuscarCampoLogPorOtroCampo(key userKey, string filtro, integer posFiltro, integer posicionCampo){
    string mensaje="";
    list lineaLog=[];
    integer i=0;
    string userName=osKey2Name(userKey);
    for(i=0; i<GetLengthLogAvatar(userKey); i++) {
        string linea=GetLineaLogAvatar(userKey, i);
        if(llStringLength(llStringTrim(linea,STRING_TRIM))>0){
            lineaLog=llParseString2List(linea,["|"],[]);
            if(llList2String(lineaLog,posFiltro)==filtro){
                mensaje=llList2String(lineaLog,posicionCampo);
                jump Fin;
            }
        }
    }
    @Fin;
        return mensaje;
}

string DependenciasSinOrden(list lineaConf, string faseTarea, key userKey, string lineadepend, string erroresdependencia){
    list dependencias=llParseString2List(lineadepend,["-"],[]);
    list dependenciasOriginales=llParseString2List(DeleteSubstring(llList2String(lineaConf,4),["[","]","(",")"]),["-"],[]);
    list msgerrores=llParseString2List(llList2String(lineaConf,5),["\\"],[]);
    string conorden="";
    integer countListDep;
    integer banderaconorden=0;
    string accion=BuscarCampoConfPorOtroCampo(faseTarea,0,1, userKey);
    for(countListDep=0;countListDep<llGetListLength(dependencias);countListDep++){
        string dependencia=llList2String(dependencias,countListDep);
        if(banderaconorden==1 || llGetSubString(dependencia,0,0)=="("){
            if(banderaconorden!=1){
                conorden+=llGetSubString(dependencia,1,llStringLength(dependencia)-1)+"-";
                banderaconorden=1;
            }
            else{
                if(llGetSubString(dependencia,llStringLength(dependencia)-1,llStringLength(dependencia)-1)==")"){
                    conorden+=llGetSubString(dependencia,0,llStringLength(dependencia)-2);
                    banderaconorden=0;
                    erroresdependencia+=DependenciasConOrden(lineaConf, faseTarea, userKey, conorden, erroresdependencia);
                    if(llSubStringIndex(erroresdependencia,"error")>=0)
                        jump Fin;
                }
                else
                    conorden+=dependencia+"-";
            }
        }
        else{
            if(llStringLength(BuscarCampoLogPorOtroCampo(userKey, dependencia, 0, 0))==0){
                string msgerror=llList2String(msgerrores,llListFindList(dependenciasOriginales,[dependencia]));
                if(llGetSubString(msgerror,0,0)=="1"){
                    erroresdependencia="error_"+llGetSubString(msgerror,2,llStringLength(msgerror)-1);
                    AgregarErrorNotecard(userKey, "dependenciabloq|"+faseTarea+"|"+accion+"|"+dependencia+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                    jump Fin;
                }
                else{
                    if(llStringLength(llStringTrim(erroresdependencia,STRING_TRIM))==0)
                        erroresdependencia=dependencia;
                    else
                        erroresdependencia+="-"+dependencia;
                }
            }
        }
    }
    @Fin;
        return erroresdependencia;
}

string DependenciasConOrden(list lineaConf, string faseTarea, key userKey, string lineadepend, string erroresdependencia){
    list dependencias=llParseString2List(lineadepend,["-"],[]);
    list dependenciasTmp=dependencias;
    list dependenciasOriginales=llParseString2List(DeleteSubstring(llList2String(lineaConf,4),["[","]","(",")"]),["-"],[]);
    list msgerrores=llParseString2List(llList2String(lineaConf,5),["\\"],[]);
    list encontrados=[];
    string sinorden="";
    string erroressinorden;
    string erroresconorden;
    integer countListDep;
    integer banderasinorden=0;
    string iniSinOrden;
    string finSinOrden;
    string accion=BuscarCampoConfPorOtroCampo(faseTarea,0,1, userKey);
    for(countListDep=0;countListDep<llGetListLength(dependencias);countListDep++){
        string dependencia=llList2String(dependencias,countListDep);
        if(banderasinorden==1 || llGetSubString(dependencia,0,0)=="["){
            if(banderasinorden!=1){
                iniSinOrden=dependencia;
                sinorden+=llGetSubString(dependencia,1,llStringLength(dependencia)-1)+"-";
                banderasinorden=1;
            }
            else{
                if(llGetSubString(dependencia,llStringLength(dependencia)-1,llStringLength(dependencia)-1)=="]"){
                    finSinOrden=dependencia;
                    sinorden+=llGetSubString(dependencia,0,llStringLength(dependencia)-2);
                    banderasinorden=0;
                    erroressinorden=DependenciasSinOrden(lineaConf, faseTarea, userKey, sinorden, erroresdependencia);
                    if(llSubStringIndex(erroressinorden,"error")>=0)
                        jump Fin;
                    else if(llStringLength(llStringTrim(erroressinorden,STRING_TRIM))==0){
                        dependenciasTmp=llListReplaceList(dependenciasTmp,["sinord"],llListFindList(dependenciasTmp,[iniSinOrden]),llListFindList(dependenciasTmp,[finSinOrden]));
                        encontrados+=["sinord"];
                    }else
                        erroresconorden+=erroressinorden;
                }
                else
                    sinorden+=dependencia+"-";
            }
        }
        else{
            string msgerror=llList2String(msgerrores,llListFindList(dependenciasOriginales,[dependencia]));
            if(llStringLength(BuscarCampoLogPorOtroCampo(userKey, dependencia, 0, 0))==0){
                if(llGetSubString(msgerror,0,0)=="1"){
                    erroresconorden="error_"+llGetSubString(msgerror,2,llStringLength(msgerror)-1);
                    AgregarErrorNotecard(userKey, "dependenciabloq|"+faseTarea+"|"+accion+"|"+dependencia+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                    jump Fin;
                }
                else{
                    if(llStringLength(llStringTrim(erroresconorden,STRING_TRIM))==0)
                        erroresconorden=dependencia;
                    else
                        erroresconorden+="-"+dependencia;
                }                   
            }
            else{
                if(llGetSubString(msgerror,0,0)!="1")
                    encontrados+=[dependencia];
                else{
                    integer indexDepTmp=llListFindList(dependenciasTmp,[dependencia]);
                    dependenciasTmp=llDeleteSubList(dependenciasTmp,indexDepTmp,indexDepTmp);
                }
            }
        }
    }
    if(llGetListLength(encontrados)>0 && dependenciasTmp!=encontrados){
        erroresconorden+="-ord_"+faseTarea;
    }
    @Fin;
        erroresdependencia+=erroresconorden;
        return erroresdependencia;
}

integer ValidarErrores(string fasetarea, key userKey){
    integer i=0;
    integer error=0;
    list errores;
    list msgErrores="";
    string userName=osKey2Name(userKey);
    integer faseant=0;
    integer fase=0;
    integer logLength=GetLengthLogAvatar(userKey);
    
    for(i=0; i<logLength; i++) {
        string linea=llStringTrim(GetLineaLogAvatar(userKey, i),STRING_TRIM);
        list lineaLog = llParseString2List(linea,["|"],[]);
        string erroresDep=llStringTrim(llList2String(lineaLog,3),STRING_TRIM);
        string erroresInc=llStringTrim(llList2String(lineaLog,4),STRING_TRIM);
        string faseTareaLog=llList2String(lineaLog,0);
        fase=(integer)llGetSubString(faseTareaLog,1,1);
        if(llStringLength(erroresDep)>0){
            if(faseant!=fase){
                msgErrores+="\nFase "+fase+"\n";
                //msgErrores+="Phase "+fase+"\n";
                faseant=fase;
            }
            //error=1;
            list lstErrDep = llParseString2List(erroresDep,["-"],[""]);
            integer j;
            string dependecias= BuscarCampoConfPorOtroCampo(faseTareaLog,0,4,userKey);
            integer indextmp=llSubStringIndex(dependecias,"[");
            if(indextmp>=0)
                dependecias=llDeleteSubString(dependecias,indextmp,indextmp);
            indextmp=llSubStringIndex(dependecias,"]");
            if(indextmp>=0)
                dependecias=llDeleteSubString(dependecias,indextmp,indextmp);
            indextmp=llSubStringIndex(dependecias,"(");
            if(indextmp>=0)
                dependecias=llDeleteSubString(dependecias,indextmp,indextmp);
            indextmp=llSubStringIndex(dependecias,")");
            if(indextmp>=0)
                dependecias=llDeleteSubString(dependecias,indextmp,indextmp);
            list lstdepend=llParseString2List(dependecias,["-"],[""]);
            string msgDepnd= BuscarCampoConfPorOtroCampo(faseTareaLog,0,5,userKey);
            list lstmsgdepend=llParseString2List(msgDepnd,["\\"],[""]);
            for(j=0;j<llGetListLength(lstErrDep);j++){
                string ftError = llList2String(lstErrDep,j);
                string msgError="";
                if(llSubStringIndex(ftError,"ord_")>=0){
                    msgError = BuscarCampoConfPorOtroCampo(faseTareaLog,0,6,userKey);
                }
                else{
                    msgError = llList2String(lstmsgdepend,llListFindList(lstdepend,[ftError]));
                }
                msgError=llGetSubString(msgError,2,llStringLength(msgError)-1);
                
                if(llListFindList(msgErrores,[msgError])==-1)
                    msgErrores+=msgError;
            }
            
        }
        if(llStringLength(erroresInc)>0){
            if(faseant!=fase){
                msgErrores+="\nFase "+fase+"\n";
                //msgErrores+="Phase "+fase+"\n";
                faseant=fase;
            }
            //error=1;
            list lstErrInc = llParseString2List(erroresInc,["-"],[""]);
            integer j;
            string incomp= BuscarCampoConfPorOtroCampo(faseTareaLog,0,7,userKey);
            list lstincomp=llParseString2List(incomp,["-"],[""]);
            string msgIncomp= BuscarCampoConfPorOtroCampo(faseTareaLog,0,8,userKey);
            list lstmsgincomp=llParseString2List(msgIncomp,["\\"],[""]);
            for(j=0;j<llGetListLength(lstErrInc);j++){
                string ftError = llList2String(lstErrInc,j);
                string msgError;
                msgError = llList2String(lstmsgincomp,llListFindList(lstincomp,[ftError]));
                msgError=llGetSubString(msgError,2,llStringLength(msgError)-1);
                
                if(llListFindList(msgErrores,[msgError])==-1)
                    msgErrores+=msgError;
            }
        }
    }
    string strMsgErrores="";
    for(i=0; i<llGetListLength(msgErrores); i++){
        strMsgErrores+=llList2String(msgErrores,i)+"\n";
    }
    if(llStringLength(llStringTrim(strMsgErrores,STRING_TRIM))>0){
        llDialog(userKey, "Existen los siguientes errores: "+strMsgErrores+"\n Sin embargo te dejaremos seguir realizando la práctica.",["Aceptar"],-9999);
        llInstantMessage(userKey,"Existen los siguientes errores: \n"+strMsgErrores+"\n Sin embargo te dejaremos seguir realizando la práctica.");
    }
    return error;
}

integer ValidarBloqueo(string objectName, key userKey){
    integer i=0;
    integer bloqueado=0;
    for(i=0; i<=llGetListLength(lsBloqueos); i++) {
        string lineabloqueo=llList2String(lsBloqueos,i);
        integer index = llSubStringIndex(lineabloqueo,"|");
        if(index>0){
            string object = llGetSubString(lineabloqueo,0,index-1);
            string rest = llGetSubString(lineabloqueo,index+1,llStringLength(lineabloqueo));
            key keyBloqueado=llStringTrim(rest,STRING_TRIM);
            
            if(object==objectName && llStringLength(llStringTrim(rest,STRING_TRIM))>0 && keyBloqueado!=userKey){
                bloqueado=1;
                jump Fin;
            }
        }
    }
    @Fin;
        return bloqueado;
}

integer ObtenerFaseActualDelAvatar(key userKey){
    integer fase=1;
    integer i=0;
    for(i=0; i<=llGetListLength(lsFases); i++) {
        string lineaFase=llList2String(lsFases,i);
        integer indexFase = llSubStringIndex(lineaFase,"|");
        string keyAvatarFase = llStringTrim(llGetSubString(lineaFase,0,indexFase-1),STRING_TRIM);
                    
        if(userKey==keyAvatarFase){
            fase = (integer)llGetSubString(llStringTrim(llGetSubString(lineaFase,indexFase+1,llStringLength(lineaFase)-1),STRING_TRIM),1,1);
        }
    }
    return fase;
}

default
{
    state_entry(){
        llSetPrimitiveParams([PRIM_SIZE, <1, 1, 1>]);
        llListen(1,"","","");
        llListen(-1,"","","");
        
        lsAvatares=[];
        lsLogs=[];
        lsPracticas=[];
        lsConfiguraciones=[];
        
        list lsTemp=[];
        lsTemp=llParseString2List(osGetNotecard("PracticaConfiguracion"),"\n","");
        integer i=0;
        for(;i<llGetListLength(lsTemp);i++){
            list lsTemp2=llParseString2List(llList2String(lsTemp,i),"|","");
            lsPracticas+=llList2String(lsTemp2,0);
            lsConfiguraciones+=osGetNotecard(llList2String(lsTemp2,1));
        }
        
        string content=osGetNotecard("Bloqueo");
        lsBloqueos=llParseString2List(llStringTrim(content,STRING_TRIM),"\n","");
        content=osGetNotecard("Fase");
        lsFases=llParseString2List(llStringTrim(content,STRING_TRIM),"\n","");
        content=osGetNotecard("Usuarios");
        lsUsuarios=llParseString2List(llStringTrim(content,STRING_TRIM),"\n","");
        
        integer num=llGetInventoryNumber( INVENTORY_NOTECARD );
        for(i=0;i<num;i++){
            string name = llGetInventoryName(INVENTORY_NOTECARD, i);
            if(name!="Bloqueo" && name!="PracticaConfiguracion" && name!="Usuarios" && name!="Fase" && llSubStringIndex(name,"_errores")<=0 && llSubStringIndex(name,"Configuracion")<0){
                integer index=llSubStringIndex(name," ");
                string nombre=llGetSubString(name,0,index-1);
                integer index2=llSubStringIndex(name,"_");
                string apellido=llGetSubString(name,index+1,index2-1);
                key keyAvatar=(string)osAvatarName2Key(nombre, apellido);
                if(llListFindList(lsAvatares,[(string)keyAvatar])<0){
                    integer j=0;
                    string codigoPractica;
                    for(;j<llGetListLength(lsFases);j++){
                        list linea=llParseString2List(llList2String(lsFases,j),["|"],[]);
                        key userkeyFase=llList2String(linea,0);
                        if(userkeyFase==keyAvatar){
                            codigoPractica=llList2String(linea,2);
                            jump encontrado;
                        }
                    }
                    @encontrado;
                    if(keyAvatar!=NULL_KEY){
                        lsAvatares+=[(string)keyAvatar];
                        lsLogs+=[llStringTrim(osGetNotecard(nombre+" "+apellido +"_"+codigoPractica),STRING_TRIM)];
                    }
                }
            }//else if(name=="Bloqueo" || name=="Fase" || name=="Usuarios"){
                
        }
        llSetTimerEvent(0.001);
    }
    
    //timer para cambiar acciones con tiempo
    timer(){
        integer actUnixTime=llGetUnixTime();
        integer tiempoTMP=0;
        llSetTimerEvent(0);
        
        integer iniTimer=actUnixTime;
        integer k=0;
        integer num=llGetListLength(lsLogs);
        for(;k<num;k++){
            integer banderaCambio=0;
            key userKey=llList2Key(lsAvatares,k);
            string name = llKey2Name(userKey);
            integer i=0;
                string content="";
                integer numLines=GetLengthLogAvatar(userKey);
                for(i=0; i<numLines; i++) {
                    string linea=llStringTrim(GetLineaLogAvatar(userKey,i),STRING_TRIM);
                    if(llStringLength(linea)>0){
                        list lineaLog = llParseString2List(linea,["|"],[]);
                        string fasetarea=llList2String(lineaLog,0);
                        integer unixTime=(integer)llList2String(lineaLog,2);
                        integer tiempotmp=0;
                        if(unixTime>0){
                            integer index=llSubStringIndex(fasetarea,"_tmin");
                            integer tmax=0;
                            if(index>0){
                                fasetarea=llGetSubString(fasetarea,0,index-1);
                                integer tmin=(integer)BuscarCampoConfPorOtroCampo(fasetarea,0,11,userKey);
                                tmax=(integer)BuscarCampoConfPorOtroCampo(fasetarea,0,10,userKey);
                                if((unixTime+tmin)<=actUnixTime && tmin>0){
                                    string strLog2;
                                    if(tmax>0){
                                        strLog2=llList2String(lineaLog,2);
                                        tiempotmp=(integer)tmax;
                                    }
                                    else
                                        strLog2=" ";
                                    string tmpctn=fasetarea+"|"+llList2String(lineaLog,1)+"|"+strLog2+"|"+llList2String(lineaLog,3)+"|"+llList2String(lineaLog,4)+"|"+llList2String(lineaLog,5)+"\n";
                                    content+=tmpctn;
                                        
                                    banderaCambio=1;
                                    
                                    llRegionSay(2,"1"+BuscarCampoConfPorOtroCampo(fasetarea,0,1,userKey)+"_"+userKey);
                                    string msg=BuscarCampoConfPorOtroCampo(fasetarea,0,2,userKey);
                                    if(llGetSubString(msg,0,0)=="1")
                                        llInstantMessage(userKey,llGetSubString(msg,2,llStringLength(msg)-1));
                                    string msgDialog=llStringTrim(BuscarCampoConfPorOtroCampo(fasetarea,0,3,userKey),STRING_TRIM);
                                    if(llStringLength(msgDialog)>0){
                                        index=llSubStringIndex(msgDialog,". ");
                                        if(index>=0){
                                            string resto=msgDialog;
                                            msgDialog="";
                                            while(index>=0){
                                                string frase=llGetSubString(resto,0,index-1);
                                                msgDialog+=llGetSubString(resto,0,index-1)+".\n";
                                                resto=llGetSubString(resto,index+2,llStringLength(resto));
                                                index=llSubStringIndex(resto,". ");
                                            }
                                            msgDialog+=llGetSubString(resto,0,llStringLength(resto)-1);
                                        }
                                        llDialog(userKey, msgDialog,["Aceptar"],-9999);
                                        llInstantMessage(userKey,msgDialog);
                                    }
                                }
                                else{
                                    content+=linea+"\n";
                                    tiempotmp=(integer)tmin;
                                }
                            }
                            else{
                                tmax=(integer)BuscarCampoConfPorOtroCampo(fasetarea,0,10,userKey);
                                if((unixTime+tmax)==actUnixTime && tmax>0){
                                    banderaCambio=1;
                                    string msg=BuscarCampoConfPorOtroCampo(fasetarea,0,12,userKey);
                                    if(llGetSubString(msg,0,0)=="1")
                                        llInstantMessage(userKey,llGetSubString(msg,2,llStringLength(msg)-1));
                                }
                                else{
                                    content+=linea+"\n";
                                    tiempotmp=(integer)tmax;
                                }
                            }
                            
                            if(tiempotmp>0){
                                integer tiempoTranscurrido=actUnixTime-unixTime;
                                integer tiempoRestante=tiempotmp-tiempoTranscurrido;
                                if(tiempoRestante < tiempoTMP || tiempoTMP==0){
                                    tiempoTMP=tiempoRestante;
                                }
                            }
                        }
                        else
                            content+=linea+"\n";
                    }
                }
                if(banderaCambio==1){
                    ModificarLogAvatar(userKey, content);
                    llMessageLinked(LINK_THIS, 0, content, userKey);//Escritura de log de acciones
                }
        }
        integer newUnixTime=0;
        if(tiempoTMP>0){
            newUnixTime=llGetUnixTime();
            tiempoTMP=tiempoTMP-(newUnixTime-iniTimer);
            float tmp=tiempoTMP;
            if(tiempoTMP<=0)
                tmp=0.00001;
            llSetTimerEvent(tmp);
        }
        tiempoTimer=tiempoTMP;
        lastUnixTime=newUnixTime;
    }
    
    listen(integer channel, string names, key id, string message)
    {
        list llMsg=llParseString2List(message,["|"],[""]);
        
        string msg = llList2String(llMsg,0);
        key userKey = llList2Key(llMsg,1);
        string accion = llList2String(llMsg,2);
        string objectName = llList2String(llMsg,3);
        integer bandera = 1;
        if(llGetListLength(llMsg)>4)
            bandera=llList2Integer(llMsg,4);
        
        string userName = llKey2Name(userKey);
        if(channel==-1){
            list vars=llParseString2List(message,"|","");
            integer i;
            for(i=0;i<llGetListLength(vars);i++){
                list var=llParseString2List(llList2String(vars,i),"=","");
                if(llList2String(var,0)=="zonahoraria")
                    zonaHoraria=llList2Integer(var,1);
            }
            llListen(3, "","","");
        }else if(channel==1){
            if(llGetInventoryKey("Usuarios")!=NULL_KEY){
                if(llListFindList(lsUsuarios, [userName])>=0){
                    if(msg=="crearLog"){
                        string ky = llGetInventoryKey( userName );
                        if(ky==NULL_KEY){
                            string codPractica=llList2String(llMsg,2);
                            string faseTarea="";
                            list lsConfiguracion=llParseString2List(llList2String(lsConfiguraciones,llListFindList(lsPracticas,codPractica)),"\n","");
                            integer i;
                            for(i=1; i<=llGetListLength(lsConfiguracion); i++) {
                                string linea=llList2String(lsConfiguracion,i);
                                if(llStringLength(llStringTrim(linea,STRING_TRIM))>0){
                                    list lineaConf=llParseString2List(linea,["|"],[]);
                                    if(llList2String(lineaConf,1)==msg){
                                        faseTarea=llList2String(lineaConf,0);
                                        jump Encontrada;
                                    }
                                }
                            }
                            @Encontrada;
                            integer indexAvatar= llListFindList(lsAvatares,[userKey]);
                            if(indexAvatar>=0){
                                lsAvatares=llDeleteSubList(lsAvatares,indexAvatar,indexAvatar);
                                lsLogs=llDeleteSubList(lsLogs,indexAvatar,indexAvatar);
                            }
                            lsAvatares+=[(string)userKey];
                            string tmplog=faseTarea+"|"+msg+"| | | |"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600);
                            lsLogs+=[tmplog];
                            
                            integer k=0;
                            for(;k<llGetListLength(lsFases);k++){
                                list lineafase=llParseString2List(llList2String(lsFases,k),["|"],[]);
                                key userkeyFase=llList2String(lineafase,0);
                                if(userkeyFase==userKey){
                                    lsFases=llDeleteSubList(lsFases,k,k);
                                    jump encontrado;
                                }
                            }
                            @encontrado;
                            string tmpfase=userKey +"|f"+llGetSubString(faseTarea,1,1)+"|"+codPractica
                            ;lsFases+=[tmpfase];
                            string content=llDumpList2String(lsFases,"\n");
                            llMessageLinked(LINK_THIS, 3, content, "");//Iniciación de la fase del avatar en la nota
                            llMessageLinked(LINK_THIS, 4, faseTarea+"|"+msg+"| | | |"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600)+"\n", userKey);//Escritura de la primera acción de crearLog
                            llMessageLinked(LINK_THIS, 1, "", userKey);//Creación del log de errores
                            string instMesg=BuscarCampoConfPorOtroCampo(msg,1,2,userKey);
                            string mostrar=llGetSubString(instMesg,0,0);
                            if(mostrar=="1")
                                llInstantMessage(userKey, llGetSubString(instMesg,2,llStringLength(instMesg)-1));
                            string msgDialog=llStringTrim(BuscarCampoConfPorOtroCampo(faseTarea, 0, 3,userKey),STRING_TRIM);
                            if(llStringLength(msgDialog)>0){
                                integer index=llSubStringIndex(msgDialog,"\\n");
                                if(index>=0){
                                    string resto=msgDialog;
                                    msgDialog="";
                                    while(index>=0){
                                        string frase=llGetSubString(resto,0,index-1);
                                        msgDialog+=llGetSubString(resto,0,index-1)+"\n";
                                        resto=llGetSubString(resto,index+2,llStringLength(resto));
                                        index=llSubStringIndex(resto,"\\n");
                                    }
                                    msgDialog+=llGetSubString(resto,0,llStringLength(resto)-1);
                                }
                                llDialog(userKey, msgDialog,["Aceptar"],-9999);
                                llInstantMessage(userKey, msgDialog);
                            }
                        }else{
                            string instMesg=BuscarCampoConfPorOtroCampo(msg,1,5,userKey);
                            string mostrar=llGetSubString(instMesg,0,0);
                            if(mostrar=="1"){
                                instMesg=llGetSubString(instMesg,2,llStringLength(instMesg)-1);
                                llInstantMessage(userKey, instMesg);
                            }
                        }
                    }else if(msg=="borrarFase"){
                        string ky = llGetInventoryKey( userName );
                        integer fase=-1;
                        if(ky!=NULL_KEY){
                            integer i=0;
                            string content = "";
                            integer numLogLines=GetLengthLogAvatar(userKey);
                            string lineaLog=GetLineaLogAvatar(userKey,numLogLines-1);
                            integer ultimaFase=(integer)llGetSubString(lineaLog,1,1);
                            
                            for(i=0; i<=llGetListLength(lsFases); i++) {
                                string lineaFase=llList2String(lsFases,i);
                                integer indexFase = llSubStringIndex(lineaFase,"|");
                                string keyAvatarFase = llStringTrim(llGetSubString(lineaFase,0,indexFase-1),STRING_TRIM);
                                if(userKey!=keyAvatarFase){
                                    content += lineaFase+"\n";
                                }else{
                                    fase = (integer)llStringTrim(llGetSubString(lineaFase,llStringLength(lineaFase)-1,llStringLength(lineaFase)-1),STRING_TRIM);
                                    integer fasenueva=fase-1;
                                    if(fase>0){
                                        content += keyAvatarFase+"|f"+fasenueva+"\n";
                                    }
                                }
                            }
                            if(fase>0){
                                string contentLog="";
                                integer num=numLogLines;
                                string ultimaFaseTarea="";
                                for(i=0; i<=num; i++) {
                                    lineaLog=GetLineaLogAvatar(userKey,i);
                                    if(fase>(integer)llGetSubString(lineaLog,1,1) || fase==-1){
                                        contentLog+=lineaLog+"\n";
                                        integer indexFase = llSubStringIndex(lineaLog,"|");
                                        ultimaFaseTarea=llGetSubString(lineaLog,0,indexFase-1);
                                    }else{
                                        i=num;
                                        string msgDialog=llStringTrim(BuscarCampoConfPorOtroCampo(ultimaFaseTarea, 0, 3,userKey),STRING_TRIM);
                                        if(llStringLength(msgDialog)>0){
                                            llDialog(userKey, msgDialog,["Aceptar"],-9999);
                                            llInstantMessage(userKey, msgDialog);
                                        }
                                    }
                                }
                                ModificarLogAvatar(userKey,contentLog);
                                llMessageLinked(LINK_THIS, 0, contentLog, userKey);//Modificación del log de acciones

                            }
                            else{
                                llRemoveInventory(userName);
                                llInstantMessage(userKey,"Vuelve a seleccionar la práctica");
                            }
                            lsFases=llParseString2List(content,"\n","");
                            llMessageLinked(LINK_THIS, 3, content, "");//Modificación de la fase en la nota

                            AgregarErrorNotecard(userKey, "borradodefase|"+fase+"|"+osUnixTimeToTimestamp((integer)llGetUnixTime()+zonaHoraria * 3600));
                            string instMesg=BuscarCampoConfPorOtroCampo(msg,1,2,userKey);
                            llInstantMessage(userKey, llGetSubString(instMesg,2,llStringLength(instMesg)-1));
                        }else{
                            string instMesg=BuscarCampoConfPorOtroCampo(msg,1,5,userKey);
                            string mostrar=llGetSubString(instMesg,0,0);
                            if(mostrar=="1"){
                                instMesg=llGetSubString(instMesg,2,llStringLength(instMesg)-1);
                                llInstantMessage(userKey, instMesg);
                            }
                        }
                    }else if(msg=="validar" && llStringLength(accion)>0){
                        string resultado=ValidarAccion(accion, userKey, objectName);
                        if(llStringLength(resultado)>0){
                            //string content = llStringTrim(osGetNotecard(userName),STRING_TRIM)+"\n" + resultado;
                            integer index= llListFindList(lsAvatares,[userKey]);
                            string content=llList2String(lsLogs,index)+"\n" +resultado;
                            ModificarLogAvatar(userKey,content);
                            llMessageLinked(LINK_THIS, 0, content, userKey);//Agregar la nueva acción a la nota del avatar
                        }
                    }else if(msg=="borrar" && llStringLength(accion)>0){
                        integer i=0;
                        string content = "";
                        integer lenLog=GetLengthLogAvatar(userKey);
                        integer fase=GetFaseAvatar(userKey);
                        //string faseTareBorrar=BuscarCampoConfPorOtroCampo(accion,1,0,userKey);
                        for(i=0; i<=lenLog; i++) {
                            string lineaLog=GetLineaLogAvatar(userKey,i);
                            list lslineaLog=llParseString2List(lineaLog,["|"],[""]);
                            string faseTarea = llList2String(lslineaLog,0);
                            integer faseTemp = (integer)llGetSubString(faseTarea,1,llSubStringIndex(faseTarea,"t")-1);
                            string accLog = llList2String(lslineaLog,1);
                            if(llStringLength(faseTarea)>0){
                                //if(faseTarea!=faseTareBorrar){
                                if(fase!=faseTemp || (fase==faseTemp && accion!=accLog)){
                                    content += lineaLog+"\n";
                                }
                            }
                        }
                        ModificarLogAvatar(userKey,content);
                        llMessageLinked(LINK_THIS, 0, content, userKey);//Modificación del log de acciones
                        if(bandera)
                            llInstantMessage(userKey, "Ha existido un error, vuelve a realizar la acción anterior");
                    }
                }else{
                    llInstantMessage(userKey,"No estás autorizado para realizar esta práctica de laboratorio.");
                }
            }else{
                llOwnerSay("Error: No se existe el archivo de usuarios.");
            }
        }
    }
}