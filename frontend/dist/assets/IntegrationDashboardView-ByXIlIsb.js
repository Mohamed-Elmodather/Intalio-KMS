import{D as Q,E as X,U as Y,av as Z,g as h,y as p,F as z,m as K,G as b,h as t,C as A,l as ee,d as te,u as ne,e as P,r as _,o as ie,z as oe,j as r,w as a,i as e,p as S,L as x,t as l,I as J}from"./index-B16DQprE.js";import{u as re}from"./useReducedMotion-BYm_JCPF.js";/* empty css                                                               */import{P as ae}from"./PageHeader-CgaVs6DH.js";import{E as le}from"./ErrorState-CxTxDIJ-.js";import{s as y}from"./index-BiAryPta.js";import{s as M}from"./index-BTnPUR-D.js";import{s as F,a as v}from"./index-CPLmpzJj.js";import{s as se}from"./index-A4U0YwZ1.js";import{s as ce}from"./index-AhOuHBwx.js";import{_ as de}from"./_plugin-vue_export-helper-DlAUqK2U.js";import"./index-DuV-imhd.js";import"./index-BblUouRz.js";import"./index-DCjdiDpV.js";import"./index-CGRK9j9Z.js";import"./index-Df5LrEQ4.js";import"./index-D70qjff0.js";var ue=`
    .p-timeline {
        display: flex;
        flex-grow: 1;
        flex-direction: column;
        direction: ltr;
    }

    .p-timeline-left .p-timeline-event-opposite {
        text-align: right;
    }

    .p-timeline-left .p-timeline-event-content {
        text-align: left;
    }

    .p-timeline-right .p-timeline-event {
        flex-direction: row-reverse;
    }

    .p-timeline-right .p-timeline-event-opposite {
        text-align: left;
    }

    .p-timeline-right .p-timeline-event-content {
        text-align: right;
    }

    .p-timeline-vertical.p-timeline-alternate .p-timeline-event:nth-child(even) {
        flex-direction: row-reverse;
    }

    .p-timeline-vertical.p-timeline-alternate .p-timeline-event:nth-child(odd) .p-timeline-event-opposite {
        text-align: right;
    }

    .p-timeline-vertical.p-timeline-alternate .p-timeline-event:nth-child(odd) .p-timeline-event-content {
        text-align: left;
    }

    .p-timeline-vertical.p-timeline-alternate .p-timeline-event:nth-child(even) .p-timeline-event-opposite {
        text-align: left;
    }

    .p-timeline-vertical.p-timeline-alternate .p-timeline-event:nth-child(even) .p-timeline-event-content {
        text-align: right;
    }

    .p-timeline-vertical .p-timeline-event-opposite,
    .p-timeline-vertical .p-timeline-event-content {
        padding: dt('timeline.vertical.event.content.padding');
    }

    .p-timeline-vertical .p-timeline-event-connector {
        width: dt('timeline.event.connector.size');
    }

    .p-timeline-event {
        display: flex;
        position: relative;
        min-height: dt('timeline.event.min.height');
    }

    .p-timeline-event:last-child {
        min-height: 0;
    }

    .p-timeline-event-opposite {
        flex: 1;
    }

    .p-timeline-event-content {
        flex: 1;
    }

    .p-timeline-event-separator {
        flex: 0;
        display: flex;
        align-items: center;
        flex-direction: column;
    }

    .p-timeline-event-marker {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        position: relative;
        align-self: baseline;
        border-width: dt('timeline.event.marker.border.width');
        border-style: solid;
        border-color: dt('timeline.event.marker.border.color');
        border-radius: dt('timeline.event.marker.border.radius');
        width: dt('timeline.event.marker.size');
        height: dt('timeline.event.marker.size');
        background: dt('timeline.event.marker.background');
    }

    .p-timeline-event-marker::before {
        content: ' ';
        border-radius: dt('timeline.event.marker.content.border.radius');
        width: dt('timeline.event.marker.content.size');
        height: dt('timeline.event.marker.content.size');
        background: dt('timeline.event.marker.content.background');
    }

    .p-timeline-event-marker::after {
        content: ' ';
        position: absolute;
        width: 100%;
        height: 100%;
        border-radius: dt('timeline.event.marker.border.radius');
        box-shadow: dt('timeline.event.marker.content.inset.shadow');
    }

    .p-timeline-event-connector {
        flex-grow: 1;
        background: dt('timeline.event.connector.color');
    }

    .p-timeline-horizontal {
        flex-direction: row;
    }

    .p-timeline-horizontal .p-timeline-event {
        flex-direction: column;
        flex: 1;
    }

    .p-timeline-horizontal .p-timeline-event:last-child {
        flex: 0;
    }

    .p-timeline-horizontal .p-timeline-event-separator {
        flex-direction: row;
    }

    .p-timeline-horizontal .p-timeline-event-connector {
        width: 100%;
        height: dt('timeline.event.connector.size');
    }

    .p-timeline-horizontal .p-timeline-event-opposite,
    .p-timeline-horizontal .p-timeline-event-content {
        padding: dt('timeline.horizontal.event.content.padding');
    }

    .p-timeline-horizontal.p-timeline-alternate .p-timeline-event:nth-child(even) {
        flex-direction: column-reverse;
    }

    .p-timeline-bottom .p-timeline-event {
        flex-direction: column-reverse;
    }
`,me={root:function(n){var d=n.props;return["p-timeline p-component","p-timeline-"+d.align,"p-timeline-"+d.layout]},event:"p-timeline-event",eventOpposite:"p-timeline-event-opposite",eventSeparator:"p-timeline-event-separator",eventMarker:"p-timeline-event-marker",eventConnector:"p-timeline-event-connector",eventContent:"p-timeline-event-content"},pe=Q.extend({name:"timeline",style:ue,classes:me}),ve={name:"BaseTimeline",extends:X,props:{value:null,align:{mode:String,default:"left"},layout:{mode:String,default:"vertical"},dataKey:null},style:pe,provide:function(){return{$pcTimeline:this,$parentInstance:this}}};function C(i){"@babel/helpers - typeof";return C=typeof Symbol=="function"&&typeof Symbol.iterator=="symbol"?function(n){return typeof n}:function(n){return n&&typeof Symbol=="function"&&n.constructor===Symbol&&n!==Symbol.prototype?"symbol":typeof n},C(i)}function V(i,n,d){return(n=he(n))in i?Object.defineProperty(i,n,{value:d,enumerable:!0,configurable:!0,writable:!0}):i[n]=d,i}function he(i){var n=ge(i,"string");return C(n)=="symbol"?n:n+""}function ge(i,n){if(C(i)!="object"||!i)return i;var d=i[Symbol.toPrimitive];if(d!==void 0){var w=d.call(i,n);if(C(w)!="object")return w;throw new TypeError("@@toPrimitive must return a primitive value.")}return(n==="string"?String:Number)(i)}var B={name:"Timeline",extends:ve,inheritAttrs:!1,methods:{getKey:function(n,d){return this.dataKey?Z(n,this.dataKey):d},getPTOptions:function(n,d){return this.ptm(n,{context:{index:d,count:this.value.length}})}},computed:{dataP:function(){return Y(V(V({},this.layout,this.layout),this.align,this.align))}}},fe=["data-p"],ye=["data-p"],be=["data-p"],_e=["data-p"],xe=["data-p"],we=["data-p"],Se=["data-p"];function Ce(i,n,d,w,I,s){return p(),h("div",b({class:i.cx("root")},i.ptmi("root"),{"data-p":s.dataP}),[(p(!0),h(z,null,K(i.value,function(g,u){return p(),h("div",b({key:s.getKey(g,u),class:i.cx("event")},{ref_for:!0},s.getPTOptions("event",u),{"data-p":s.dataP}),[t("div",b({class:i.cx("eventOpposite",{index:u})},{ref_for:!0},s.getPTOptions("eventOpposite",u),{"data-p":s.dataP}),[A(i.$slots,"opposite",{item:g,index:u})],16,be),t("div",b({class:i.cx("eventSeparator")},{ref_for:!0},s.getPTOptions("eventSeparator",u),{"data-p":s.dataP}),[A(i.$slots,"marker",{item:g,index:u},function(){return[t("div",b({class:i.cx("eventMarker")},{ref_for:!0},s.getPTOptions("eventMarker",u),{"data-p":s.dataP}),null,16,xe)]}),u!==i.value.length-1?A(i.$slots,"connector",{key:0,item:g,index:u},function(){return[t("div",b({class:i.cx("eventConnector")},{ref_for:!0},s.getPTOptions("eventConnector",u),{"data-p":s.dataP}),null,16,we)]}):ee("",!0)],16,_e),t("div",b({class:i.cx("eventContent")},{ref_for:!0},s.getPTOptions("eventContent",u),{"data-p":s.dataP}),[A(i.$slots,"content",{item:g,index:u})],16,Se)],16,ye)}),128))],16,fe)}B.render=Ce;const ke={key:0,class:"grid"},De={key:1},Ae={class:"grid mb-4"},Pe={class:"col-12 md:col-6 lg:col-3"},Ie={class:"flex align-items-center gap-3"},Me={class:"text-2xl font-bold"},ze={class:"text-color-secondary"},Te={class:"col-12 md:col-6 lg:col-3"},Ee={class:"flex align-items-center gap-3"},Ne={class:"text-2xl font-bold text-green-600"},je={class:"text-color-secondary"},$e={class:"col-12 md:col-6 lg:col-3"},He={class:"flex align-items-center gap-3"},Oe={class:"text-2xl font-bold"},Re={class:"text-color-secondary"},Je={class:"col-12 md:col-6 lg:col-3"},Fe={class:"flex align-items-center gap-3"},Ve={class:"text-2xl font-bold"},Ke={class:"text-color-secondary"},Be={class:"grid"},Le={class:"col-12 lg:col-8"},Ue={class:"flex justify-content-between align-items-center"},We={class:"flex align-items-center gap-2"},qe={class:"font-medium"},Ge={class:"flex align-items-center gap-2"},Qe={class:"flex justify-content-between align-items-center"},Xe={key:0},Ye={class:"text-xs text-color-secondary"},Ze={key:1},et={class:"col-12 lg:col-4"},tt={class:"flex justify-content-between align-items-center"},nt={class:"mb-3"},it={class:"flex align-items-center gap-2 mb-1"},ot={class:"text-xs text-color-secondary"},rt={class:"m-0 text-sm"},at=te({__name:"IntegrationDashboardView",setup(i){const{t:n,locale:d}=ne(),w=re(),I=P(()=>d.value==="ar"),s=P(()=>!w.value),g=_(!1),u=P(()=>[{label:"Home",labelArabic:"الرئيسية",to:"/dashboard"},{label:n("integration.dashboard.title")}]),k=_(!0),D=_(null),f=_(null),T=_([]),E=_([]),N=_([]),L=P(()=>{if(!f.value)return{healthy:0,total:0,percentage:0};const c=f.value.healthyConnectors,m=f.value.totalConnectors;return{healthy:c,total:m,percentage:m>0?Math.round(c/m*100):0}});async function j(){k.value=!0,D.value=null;try{await new Promise(c=>setTimeout(c,600)),f.value={totalConnectors:6,activeConnectors:5,healthyConnectors:5,unhealthyConnectors:1,totalSyncJobs:15,runningSyncJobs:2,failedSyncJobs:1,todayEvents:245,todayErrors:3},T.value=[{id:"1",name:"Intalio Case",provider:"IntalioCase",category:"Workflow",status:"Active",isHealthy:!0,lastSync:new Date(Date.now()-18e5)},{id:"2",name:"Intalio IAM",provider:"IntalioIAM",category:"Identity",status:"Active",isHealthy:!0,lastSync:new Date(Date.now()-72e5)},{id:"3",name:"Intalio DMS",provider:"IntalioDMS",category:"DocumentManagement",status:"Active",isHealthy:!0,lastSync:new Date(Date.now()-9e5)},{id:"4",name:"Intalio Correspondence",provider:"IntalioCorrespondence",category:"Correspondence",status:"Active",isHealthy:!0,lastSync:new Date(Date.now()-36e5)},{id:"5",name:"SharePoint Online",provider:"SharePoint",category:"DocumentManagement",status:"Active",isHealthy:!0,lastSync:new Date(Date.now()-18e5)},{id:"6",name:"SAP S/4HANA",provider:"SAP",category:"ERP",status:"Error",isHealthy:!1,lastSync:new Date(Date.now()-864e5),errorMessage:"Connection timeout"}],E.value=[{id:"1",connectorName:"Intalio Case",jobName:"Task Sync",status:"Completed",recordCount:45,errorCount:0,completedAt:new Date(Date.now()-3e5)},{id:"2",connectorName:"Intalio DMS",jobName:"Document Sync",status:"Running",recordCount:120,processedCount:85,completedAt:null},{id:"3",connectorName:"Intalio IAM",jobName:"User Sync",status:"Completed",recordCount:320,errorCount:2,completedAt:new Date(Date.now()-72e5)},{id:"4",connectorName:"SAP S/4HANA",jobName:"Employee Sync",status:"Failed",recordCount:0,errorCount:1,errorMessage:"Connection refused",completedAt:new Date(Date.now()-864e5)}],N.value=[{id:"1",type:"TaskCompleted",connector:"Intalio Case",description:'Task "Document Review" completed',timestamp:new Date(Date.now()-3e5),status:"Success"},{id:"2",type:"DocumentSynced",connector:"Intalio DMS",description:"15 documents synced",timestamp:new Date(Date.now()-6e5),status:"Success"},{id:"3",type:"UserCreated",connector:"Intalio IAM",description:"New user provisioned: ahmed.hassan@intalio.com",timestamp:new Date(Date.now()-9e5),status:"Success"},{id:"4",type:"SyncFailed",connector:"SAP S/4HANA",description:"Employee sync failed: Connection timeout",timestamp:new Date(Date.now()-864e5),status:"Error"}],s.value?requestAnimationFrame(()=>{g.value=!0}):g.value=!0}catch(c){D.value=c,console.error("Failed to fetch dashboard:",c)}finally{k.value=!1}}function U(){k.value=!0,g.value=!1,j()}function $(c){switch(c){case"Active":case"Completed":case"Success":return"success";case"Running":case"Processing":return"info";case"Warning":return"warn";case"Error":case"Failed":return"danger";default:return"secondary"}}function W(c){switch(c){case"Workflow":return"pi-sitemap";case"Identity":return"pi-users";case"DocumentManagement":return"pi-folder";case"Correspondence":return"pi-envelope";case"ERP":return"pi-building";case"Calendar":return"pi-calendar";default:return"pi-link"}}function q(c){return c?new Intl.DateTimeFormat(d.value,{dateStyle:"short",timeStyle:"short"}).format(c):"-"}function H(c){const o=new Date().getTime()-c.getTime(),O=Math.floor(o/6e4),R=Math.floor(o/36e5),G=Math.floor(o/864e5);return O<60?`${O} ${n("common.minutesAgo")}`:R<24?`${R} ${n("common.hoursAgo")}`:`${G} ${n("common.daysAgo")}`}return ie(()=>{j()}),(c,m)=>(p(),h("div",{class:S(["integration-dashboard",{rtl:I.value}])},[D.value?(p(),oe(le,{key:0,error:D.value,title:I.value?"فشل تحميل لوحة التكامل":"Failed to load integration dashboard","show-retry":"",size:"lg",onRetry:U},null,8,["error","title"])):(p(),h(z,{key:1},[r(e(ae),{title:e(n)("integration.dashboard.title"),description:e(n)("integration.dashboard.subtitle"),breadcrumbs:u.value,"padding-bottom":"xl","background-variant":"gradient",animated:s.value},{actions:a(()=>[r(e(x),{label:e(n)("integration.manageConnectors"),icon:"pi pi-cog",class:"header-btn-secondary"},null,8,["label"]),r(e(x),{label:e(n)("integration.addConnector"),icon:"pi pi-plus",class:"header-btn-primary"},null,8,["label"])]),_:1},8,["title","description","breadcrumbs","animated"]),k.value?(p(),h("div",ke,[(p(),h(z,null,K(4,o=>t("div",{class:"col-12 md:col-6 lg:col-3",key:o},[r(e(y),null,{content:a(()=>[r(e(ce),{height:"80px"})]),_:1})])),64))])):(p(),h("div",De,[t("div",Ae,[t("div",Pe,[r(e(y),{class:"h-full"},{content:a(()=>[t("div",Ie,[m[0]||(m[0]=t("div",{class:"w-3rem h-3rem bg-blue-100 border-round flex align-items-center justify-content-center"},[t("i",{class:"pi pi-link text-blue-600 text-xl"})],-1)),t("div",null,[t("div",Me,l(f.value.activeConnectors)+" / "+l(f.value.totalConnectors),1),t("div",ze,l(e(n)("integration.dashboard.activeConnectors")),1)])])]),_:1})]),t("div",Te,[r(e(y),{class:"h-full"},{content:a(()=>[t("div",Ee,[m[1]||(m[1]=t("div",{class:"w-3rem h-3rem bg-green-100 border-round flex align-items-center justify-content-center"},[t("i",{class:"pi pi-heart text-green-600 text-xl"})],-1)),t("div",null,[t("div",Ne,l(L.value.percentage)+"%",1),t("div",je,l(e(n)("integration.dashboard.healthyConnectors")),1)])])]),_:1})]),t("div",$e,[r(e(y),{class:"h-full"},{content:a(()=>[t("div",He,[m[2]||(m[2]=t("div",{class:"w-3rem h-3rem bg-purple-100 border-round flex align-items-center justify-content-center"},[t("i",{class:"pi pi-sync text-purple-600 text-xl"})],-1)),t("div",null,[t("div",Oe,l(f.value.runningSyncJobs),1),t("div",Re,l(e(n)("integration.dashboard.runningSyncs")),1)])])]),_:1})]),t("div",Je,[r(e(y),{class:"h-full"},{content:a(()=>[t("div",Fe,[m[3]||(m[3]=t("div",{class:"w-3rem h-3rem bg-yellow-100 border-round flex align-items-center justify-content-center"},[t("i",{class:"pi pi-bolt text-yellow-600 text-xl"})],-1)),t("div",null,[t("div",Ve,l(f.value.todayEvents),1),t("div",Ke,l(e(n)("integration.dashboard.todayEvents")),1)])])]),_:1})])]),t("div",Be,[t("div",Le,[r(e(y),null,{title:a(()=>[t("div",Ue,[t("span",null,l(e(n)("integration.dashboard.connectorStatus")),1),r(e(x),{label:e(n)("common.viewAll"),text:"",size:"small"},null,8,["label"])])]),content:a(()=>[r(e(F),{value:T.value,responsiveLayout:"scroll"},{default:a(()=>[r(e(v),{field:"name",header:e(n)("integration.connector.name")},{body:a(({data:o})=>[t("div",We,[t("i",{class:S(["pi",W(o.category),"text-color-secondary"])},null,2),t("span",qe,l(o.name),1)])]),_:1},8,["header"]),r(e(v),{field:"category",header:e(n)("integration.connector.category")},null,8,["header"]),r(e(v),{field:"status",header:e(n)("integration.connector.status")},{body:a(({data:o})=>[t("div",Ge,[t("i",{class:S(["pi",{"pi-check-circle text-green-500":o.isHealthy,"pi-exclamation-circle text-red-500":!o.isHealthy}])},null,2),r(e(M),{value:o.status,severity:$(o.status)},null,8,["value","severity"])])]),_:1},8,["header"]),r(e(v),{field:"lastSync",header:e(n)("integration.connector.lastSync")},{body:a(({data:o})=>[J(l(H(o.lastSync)),1)]),_:1},8,["header"]),r(e(v),null,{body:a(()=>[r(e(x),{icon:"pi pi-sync",text:"",rounded:"",size:"small",title:e(n)("integration.syncNow")},null,8,["title"]),r(e(x),{icon:"pi pi-cog",text:"",rounded:"",size:"small",title:e(n)("common.settings")},null,8,["title"])]),_:1})]),_:1},8,["value"])]),_:1}),r(e(y),{class:"mt-4"},{title:a(()=>[t("div",Qe,[t("span",null,l(e(n)("integration.dashboard.recentSyncJobs")),1),r(e(x),{label:e(n)("common.viewAll"),text:"",size:"small"},null,8,["label"])])]),content:a(()=>[r(e(F),{value:E.value,responsiveLayout:"scroll"},{default:a(()=>[r(e(v),{field:"connectorName",header:e(n)("integration.syncJob.connector")},null,8,["header"]),r(e(v),{field:"jobName",header:e(n)("integration.syncJob.name")},null,8,["header"]),r(e(v),{field:"status",header:e(n)("integration.syncJob.status")},{body:a(({data:o})=>[r(e(M),{value:o.status,severity:$(o.status)},null,8,["value","severity"])]),_:1},8,["header"]),r(e(v),{field:"recordCount",header:e(n)("integration.syncJob.records")},{body:a(({data:o})=>[o.status==="Running"?(p(),h("div",Xe,[r(e(se),{value:Math.round(o.processedCount/o.recordCount*100),showValue:!0,style:{height:"8px"}},null,8,["value"]),t("span",Ye,l(o.processedCount)+" / "+l(o.recordCount),1)])):(p(),h("span",Ze,l(o.recordCount),1))]),_:1},8,["header"]),r(e(v),{field:"completedAt",header:e(n)("integration.syncJob.completedAt")},{body:a(({data:o})=>[J(l(q(o.completedAt)),1)]),_:1},8,["header"])]),_:1},8,["value"])]),_:1})]),t("div",et,[r(e(y),{class:"h-full"},{title:a(()=>[t("div",tt,[t("span",null,l(e(n)("integration.dashboard.recentEvents")),1),r(e(x),{label:e(n)("common.viewAll"),text:"",size:"small"},null,8,["label"])])]),content:a(()=>[r(e(B),{value:N.value,class:"integration-timeline"},{marker:a(({item:o})=>[t("span",{class:S(["flex align-items-center justify-content-center w-2rem h-2rem border-round",{"bg-green-100":o.status==="Success","bg-red-100":o.status==="Error"}])},[t("i",{class:S(["pi text-sm",{"pi-check text-green-600":o.status==="Success","pi-times text-red-600":o.status==="Error"}])},null,2)],2)]),content:a(({item:o})=>[t("div",nt,[t("div",it,[r(e(M),{value:o.connector,size:"small"},null,8,["value"]),t("span",ot,l(H(o.timestamp)),1)]),t("p",rt,l(o.description),1)])]),_:1},8,["value"])]),_:1})])])]))],64))],2))}}),Ct=de(at,[["__scopeId","data-v-c3ae4272"]]);export{Ct as default};
