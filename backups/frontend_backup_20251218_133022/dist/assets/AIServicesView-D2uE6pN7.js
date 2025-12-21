import{D as ie,X as be,E as J,ai as N,aj as Pe,ag as et,a4 as Se,a6 as ne,G as c,P as je,g as f,y as r,h as i,n as M,l as T,C as I,z as k,A as F,F as A,m as _,t as d,v as _e,ak as tt,w as g,p as L,W as nt,Y as he,U as at,k as E,T as st,al as ot,L as j,am as Fe,an as it,j as p,x as Ae,I as x,N as lt,d as rt,u as ut,e as ue,r as B,o as dt,i as o,Q as ct}from"./index-B16DQprE.js";import{u as pt}from"./useReducedMotion-BYm_JCPF.js";import{E as ft}from"./ErrorState-CxTxDIJ-.js";/* empty css                                                               */import{L as ae}from"./LoadingSpinner-DB35GP-M.js";import{s as D}from"./index-BiAryPta.js";import{s as mt}from"./index-BnifcyyH.js";import{s as bt}from"./index-Df5LrEQ4.js";import{b as ht,s as gt,a as H}from"./index-CPLmpzJj.js";import{s as pe}from"./index-A4U0YwZ1.js";import{s as Le}from"./index-CXPMSq9I.js";import{s as de}from"./index-C8vWZfKN.js";import{s as se}from"./index-BTnPUR-D.js";import{s as vt}from"./index-DCjdiDpV.js";import{_ as yt}from"./_plugin-vue_export-helper-DlAUqK2U.js";import"./index-DuV-imhd.js";import"./index-BblUouRz.js";import"./index-CGRK9j9Z.js";import"./index-D70qjff0.js";var wt=`
    .p-tabview-tablist-container {
        position: relative;
    }

    .p-tabview-scrollable > .p-tabview-tablist-container {
        overflow: hidden;
    }

    .p-tabview-tablist-scroll-container {
        overflow-x: auto;
        overflow-y: hidden;
        scroll-behavior: smooth;
        scrollbar-width: none;
        overscroll-behavior: contain auto;
    }

    .p-tabview-tablist-scroll-container::-webkit-scrollbar {
        display: none;
    }

    .p-tabview-tablist {
        display: flex;
        margin: 0;
        padding: 0;
        list-style-type: none;
        flex: 1 1 auto;
        background: dt('tabview.tab.list.background');
        border: 1px solid dt('tabview.tab.list.border.color');
        border-width: 0 0 1px 0;
        position: relative;
    }

    .p-tabview-tab-header {
        cursor: pointer;
        user-select: none;
        display: flex;
        align-items: center;
        text-decoration: none;
        position: relative;
        overflow: hidden;
        border-style: solid;
        border-width: 0 0 1px 0;
        border-color: transparent transparent dt('tabview.tab.border.color') transparent;
        color: dt('tabview.tab.color');
        padding: 1rem 1.125rem;
        font-weight: 600;
        border-top-right-radius: dt('border.radius.md');
        border-top-left-radius: dt('border.radius.md');
        transition:
            color dt('tabview.transition.duration'),
            outline-color dt('tabview.transition.duration');
        margin: 0 0 -1px 0;
        outline-color: transparent;
    }

    .p-tabview-tablist-item:not(.p-disabled) .p-tabview-tab-header:focus-visible {
        outline: dt('focus.ring.width') dt('focus.ring.style') dt('focus.ring.color');
        outline-offset: -1px;
    }

    .p-tabview-tablist-item:not(.p-highlight):not(.p-disabled):hover > .p-tabview-tab-header {
        color: dt('tabview.tab.hover.color');
    }

    .p-tabview-tablist-item.p-highlight > .p-tabview-tab-header {
        color: dt('tabview.tab.active.color');
    }

    .p-tabview-tab-title {
        line-height: 1;
        white-space: nowrap;
    }

    .p-tabview-next-button,
    .p-tabview-prev-button {
        position: absolute;
        top: 0;
        margin: 0;
        padding: 0;
        z-index: 2;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        background: dt('tabview.nav.button.background');
        color: dt('tabview.nav.button.color');
        width: 2.5rem;
        border-radius: 0;
        outline-color: transparent;
        transition:
            color dt('tabview.transition.duration'),
            outline-color dt('tabview.transition.duration');
        box-shadow: dt('tabview.nav.button.shadow');
        border: none;
        cursor: pointer;
        user-select: none;
    }

    .p-tabview-next-button:focus-visible,
    .p-tabview-prev-button:focus-visible {
        outline: dt('focus.ring.width') dt('focus.ring.style') dt('focus.ring.color');
        outline-offset: dt('focus.ring.offset');
    }

    .p-tabview-next-button:hover,
    .p-tabview-prev-button:hover {
        color: dt('tabview.nav.button.hover.color');
    }

    .p-tabview-prev-button {
        left: 0;
    }

    .p-tabview-next-button {
        right: 0;
    }

    .p-tabview-panels {
        background: dt('tabview.tab.panel.background');
        color: dt('tabview.tab.panel.color');
        padding: 0.875rem 1.125rem 1.125rem 1.125rem;
    }

    .p-tabview-ink-bar {
        z-index: 1;
        display: block;
        position: absolute;
        bottom: -1px;
        height: 1px;
        background: dt('tabview.tab.active.border.color');
        transition: 250ms cubic-bezier(0.35, 0, 0.25, 1);
    }
`,kt={root:function(t){var n=t.props;return["p-tabview p-component",{"p-tabview-scrollable":n.scrollable}]},navContainer:"p-tabview-tablist-container",prevButton:"p-tabview-prev-button",navContent:"p-tabview-tablist-scroll-container",nav:"p-tabview-tablist",tab:{header:function(t){var n=t.instance,s=t.tab,l=t.index;return["p-tabview-tablist-item",n.getTabProp(s,"headerClass"),{"p-tabview-tablist-item-active":n.d_activeIndex===l,"p-disabled":n.getTabProp(s,"disabled")}]},headerAction:"p-tabview-tab-header",headerTitle:"p-tabview-tab-title",content:function(t){var n=t.instance,s=t.tab;return["p-tabview-panel",n.getTabProp(s,"contentClass")]}},inkbar:"p-tabview-ink-bar",nextButton:"p-tabview-next-button",panelContainer:"p-tabview-panels"},Ct=ie.extend({name:"tabview",style:wt,classes:kt}),Tt={name:"BaseTabView",extends:J,props:{activeIndex:{type:Number,default:0},lazy:{type:Boolean,default:!1},scrollable:{type:Boolean,default:!1},tabindex:{type:Number,default:0},selectOnFocus:{type:Boolean,default:!1},prevButtonProps:{type:null,default:null},nextButtonProps:{type:null,default:null},prevIcon:{type:String,default:void 0},nextIcon:{type:String,default:void 0}},style:Ct,provide:function(){return{$pcTabs:void 0,$pcTabView:this,$parentInstance:this}}},$e={name:"TabView",extends:Tt,inheritAttrs:!1,emits:["update:activeIndex","tab-change","tab-click"],data:function(){return{d_activeIndex:this.activeIndex,isPrevButtonDisabled:!0,isNextButtonDisabled:!1}},watch:{activeIndex:function(t){this.d_activeIndex=t,this.scrollInView({index:t})}},mounted:function(){console.warn("Deprecated since v4. Use Tabs component instead."),this.updateInkBar(),this.scrollable&&this.updateButtonState()},updated:function(){this.updateInkBar(),this.scrollable&&this.updateButtonState()},methods:{isTabPanel:function(t){return t.type.name==="TabPanel"},isTabActive:function(t){return this.d_activeIndex===t},getTabProp:function(t,n){return t.props?t.props[n]:void 0},getKey:function(t,n){return this.getTabProp(t,"header")||n},getTabHeaderActionId:function(t){return"".concat(this.$id,"_").concat(t,"_header_action")},getTabContentId:function(t){return"".concat(this.$id,"_").concat(t,"_content")},getTabPT:function(t,n,s){var l=this.tabs.length,a={props:t.props,parent:{instance:this,props:this.$props,state:this.$data},context:{index:s,count:l,first:s===0,last:s===l-1,active:this.isTabActive(s)}};return c(this.ptm("tabpanel.".concat(n),{tabpanel:a}),this.ptm("tabpanel.".concat(n),a),this.ptmo(this.getTabProp(t,"pt"),n,a))},onScroll:function(t){this.scrollable&&this.updateButtonState(),t.preventDefault()},onPrevButtonClick:function(){var t=this.$refs.content,n=N(t),s=t.scrollLeft-n;t.scrollLeft=s<=0?0:s},onNextButtonClick:function(){var t=this.$refs.content,n=N(t)-this.getVisibleButtonWidths(),s=t.scrollLeft+n,l=t.scrollWidth-n;t.scrollLeft=s>=l?l:s},onTabClick:function(t,n,s){this.changeActiveIndex(t,n,s),this.$emit("tab-click",{originalEvent:t,index:s})},onTabKeyDown:function(t,n,s){switch(t.code){case"ArrowLeft":this.onTabArrowLeftKey(t);break;case"ArrowRight":this.onTabArrowRightKey(t);break;case"Home":this.onTabHomeKey(t);break;case"End":this.onTabEndKey(t);break;case"PageDown":this.onPageDownKey(t);break;case"PageUp":this.onPageUpKey(t);break;case"Enter":case"NumpadEnter":case"Space":this.onTabEnterKey(t,n,s);break}},onTabArrowRightKey:function(t){var n=this.findNextHeaderAction(t.target.parentElement);n?this.changeFocusedTab(t,n):this.onTabHomeKey(t),t.preventDefault()},onTabArrowLeftKey:function(t){var n=this.findPrevHeaderAction(t.target.parentElement);n?this.changeFocusedTab(t,n):this.onTabEndKey(t),t.preventDefault()},onTabHomeKey:function(t){var n=this.findFirstHeaderAction();this.changeFocusedTab(t,n),t.preventDefault()},onTabEndKey:function(t){var n=this.findLastHeaderAction();this.changeFocusedTab(t,n),t.preventDefault()},onPageDownKey:function(t){this.scrollInView({index:this.$refs.nav.children.length-2}),t.preventDefault()},onPageUpKey:function(t){this.scrollInView({index:0}),t.preventDefault()},onTabEnterKey:function(t,n,s){this.changeActiveIndex(t,n,s),t.preventDefault()},findNextHeaderAction:function(t){var n=arguments.length>1&&arguments[1]!==void 0?arguments[1]:!1,s=n?t:t.nextElementSibling;return s?ne(s,"data-p-disabled")||ne(s,"data-pc-section")==="inkbar"?this.findNextHeaderAction(s):Se(s,'[data-pc-section="headeraction"]'):null},findPrevHeaderAction:function(t){var n=arguments.length>1&&arguments[1]!==void 0?arguments[1]:!1,s=n?t:t.previousElementSibling;return s?ne(s,"data-p-disabled")||ne(s,"data-pc-section")==="inkbar"?this.findPrevHeaderAction(s):Se(s,'[data-pc-section="headeraction"]'):null},findFirstHeaderAction:function(){return this.findNextHeaderAction(this.$refs.nav.firstElementChild,!0)},findLastHeaderAction:function(){return this.findPrevHeaderAction(this.$refs.nav.lastElementChild,!0)},changeActiveIndex:function(t,n,s){!this.getTabProp(n,"disabled")&&this.d_activeIndex!==s&&(this.d_activeIndex=s,this.$emit("update:activeIndex",s),this.$emit("tab-change",{originalEvent:t,index:s}),this.scrollInView({index:s}))},changeFocusedTab:function(t,n){if(n&&(et(n),this.scrollInView({element:n}),this.selectOnFocus)){var s=parseInt(n.parentElement.dataset.pcIndex,10),l=this.tabs[s];this.changeActiveIndex(t,l,s)}},scrollInView:function(t){var n=t.element,s=t.index,l=s===void 0?-1:s,a=n||this.$refs.nav.children[l];a&&a.scrollIntoView&&a.scrollIntoView({block:"nearest"})},updateInkBar:function(){var t=this.$refs.nav.children[this.d_activeIndex];this.$refs.inkbar.style.width=N(t)+"px",this.$refs.inkbar.style.left=Pe(t).left-Pe(this.$refs.nav).left+"px"},updateButtonState:function(){var t=this.$refs.content,n=t.scrollLeft,s=t.scrollWidth,l=N(t);this.isPrevButtonDisabled=n===0,this.isNextButtonDisabled=parseInt(n)===s-l},getVisibleButtonWidths:function(){var t=this.$refs,n=t.prevBtn,s=t.nextBtn;return[n,s].reduce(function(l,a){return a?l+N(a):l},0)}},computed:{tabs:function(){var t=this;return this.$slots.default().reduce(function(n,s){return t.isTabPanel(s)?n.push(s):s.children&&s.children instanceof Array&&s.children.forEach(function(l){t.isTabPanel(l)&&n.push(l)}),n},[])},prevButtonAriaLabel:function(){return this.$primevue.config.locale.aria?this.$primevue.config.locale.aria.previous:void 0},nextButtonAriaLabel:function(){return this.$primevue.config.locale.aria?this.$primevue.config.locale.aria.next:void 0}},directives:{ripple:be},components:{ChevronLeftIcon:mt,ChevronRightIcon:bt}};function q(e){"@babel/helpers - typeof";return q=typeof Symbol=="function"&&typeof Symbol.iterator=="symbol"?function(t){return typeof t}:function(t){return t&&typeof Symbol=="function"&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},q(e)}function ze(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var s=Object.getOwnPropertySymbols(e);t&&(s=s.filter(function(l){return Object.getOwnPropertyDescriptor(e,l).enumerable})),n.push.apply(n,s)}return n}function P(e){for(var t=1;t<arguments.length;t++){var n=arguments[t]!=null?arguments[t]:{};t%2?ze(Object(n),!0).forEach(function(s){Bt(e,s,n[s])}):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):ze(Object(n)).forEach(function(s){Object.defineProperty(e,s,Object.getOwnPropertyDescriptor(n,s))})}return e}function Bt(e,t,n){return(t=It(t))in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function It(e){var t=Pt(e,"string");return q(t)=="symbol"?t:t+""}function Pt(e,t){if(q(e)!="object"||!e)return e;var n=e[Symbol.toPrimitive];if(n!==void 0){var s=n.call(e,t);if(q(s)!="object")return s;throw new TypeError("@@toPrimitive must return a primitive value.")}return(t==="string"?String:Number)(e)}var St=["tabindex","aria-label"],Ft=["data-p-active","data-p-disabled","data-pc-index"],At=["id","tabindex","aria-disabled","aria-selected","aria-controls","onClick","onKeydown"],Lt=["tabindex","aria-label"],zt=["id","aria-labelledby","data-pc-index","data-p-active"];function xt(e,t,n,s,l,a){var b=je("ripple");return r(),f("div",c({class:e.cx("root"),role:"tablist"},e.ptmi("root")),[i("div",c({class:e.cx("navContainer")},e.ptm("navContainer")),[e.scrollable&&!l.isPrevButtonDisabled?M((r(),f("button",c({key:0,ref:"prevBtn",type:"button",class:e.cx("prevButton"),tabindex:e.tabindex,"aria-label":a.prevButtonAriaLabel,onClick:t[0]||(t[0]=function(){return a.onPrevButtonClick&&a.onPrevButtonClick.apply(a,arguments)})},P(P({},e.prevButtonProps),e.ptm("prevButton")),{"data-pc-group-section":"navbutton"}),[I(e.$slots,"previcon",{},function(){return[(r(),k(F(e.prevIcon?"span":"ChevronLeftIcon"),c({"aria-hidden":"true",class:e.prevIcon},e.ptm("prevIcon")),null,16,["class"]))]})],16,St)),[[b]]):T("",!0),i("div",c({ref:"content",class:e.cx("navContent"),onScroll:t[1]||(t[1]=function(){return a.onScroll&&a.onScroll.apply(a,arguments)})},e.ptm("navContent")),[i("ul",c({ref:"nav",class:e.cx("nav")},e.ptm("nav")),[(r(!0),f(A,null,_(a.tabs,function(u,m){return r(),f("li",c({key:a.getKey(u,m),style:a.getTabProp(u,"headerStyle"),class:e.cx("tab.header",{tab:u,index:m}),role:"presentation"},{ref_for:!0},P(P(P({},a.getTabProp(u,"headerProps")),a.getTabPT(u,"root",m)),a.getTabPT(u,"header",m)),{"data-pc-name":"tabpanel","data-p-active":l.d_activeIndex===m,"data-p-disabled":a.getTabProp(u,"disabled"),"data-pc-index":m}),[M((r(),f("a",c({id:a.getTabHeaderActionId(m),class:e.cx("tab.headerAction"),tabindex:a.getTabProp(u,"disabled")||!a.isTabActive(m)?-1:e.tabindex,role:"tab","aria-disabled":a.getTabProp(u,"disabled"),"aria-selected":a.isTabActive(m),"aria-controls":a.getTabContentId(m),onClick:function(y){return a.onTabClick(y,u,m)},onKeydown:function(y){return a.onTabKeyDown(y,u,m)}},{ref_for:!0},P(P({},a.getTabProp(u,"headerActionProps")),a.getTabPT(u,"headerAction",m))),[u.props&&u.props.header?(r(),f("span",c({key:0,class:e.cx("tab.headerTitle")},{ref_for:!0},a.getTabPT(u,"headerTitle",m)),d(u.props.header),17)):T("",!0),u.children&&u.children.header?(r(),k(F(u.children.header),{key:1})):T("",!0)],16,At)),[[b]])],16,Ft)}),128)),i("li",c({ref:"inkbar",class:e.cx("inkbar"),role:"presentation","aria-hidden":"true"},e.ptm("inkbar")),null,16)],16)],16),e.scrollable&&!l.isNextButtonDisabled?M((r(),f("button",c({key:1,ref:"nextBtn",type:"button",class:e.cx("nextButton"),tabindex:e.tabindex,"aria-label":a.nextButtonAriaLabel,onClick:t[2]||(t[2]=function(){return a.onNextButtonClick&&a.onNextButtonClick.apply(a,arguments)})},P(P({},e.nextButtonProps),e.ptm("nextButton")),{"data-pc-group-section":"navbutton"}),[I(e.$slots,"nexticon",{},function(){return[(r(),k(F(e.nextIcon?"span":"ChevronRightIcon"),c({"aria-hidden":"true",class:e.nextIcon},e.ptm("nextIcon")),null,16,["class"]))]})],16,Lt)),[[b]]):T("",!0)],16),i("div",c({class:e.cx("panelContainer")},e.ptm("panelContainer")),[(r(!0),f(A,null,_(a.tabs,function(u,m){return r(),f(A,{key:a.getKey(u,m)},[!e.lazy||a.isTabActive(m)?M((r(),f("div",c({key:0,id:a.getTabContentId(m),style:a.getTabProp(u,"contentStyle"),class:e.cx("tab.content",{tab:u}),role:"tabpanel","aria-labelledby":a.getTabHeaderActionId(m)},{ref_for:!0},P(P(P({},a.getTabProp(u,"contentProps")),a.getTabPT(u,"root",m)),a.getTabPT(u,"content",m)),{"data-pc-name":"tabpanel","data-pc-index":m,"data-p-active":l.d_activeIndex===m}),[(r(),k(F(u)))],16,zt)),[[_e,e.lazy?!0:a.isTabActive(m)]]):T("",!0)],64)}),128))],16)],16)}$e.render=xt;var Dt={root:function(t){var n=t.instance;return["p-tabpanel",{"p-tabpanel-active":n.active}]}},Et=ie.extend({name:"tabpanel",classes:Dt}),jt={name:"BaseTabPanel",extends:J,props:{value:{type:[String,Number],default:void 0},as:{type:[String,Object],default:"DIV"},asChild:{type:Boolean,default:!1},header:null,headerStyle:null,headerClass:null,headerProps:null,headerActionProps:null,contentStyle:null,contentClass:null,contentProps:null,disabled:Boolean},style:Et,provide:function(){return{$pcTabPanel:this,$parentInstance:this}}},R={name:"TabPanel",extends:jt,inheritAttrs:!1,inject:["$pcTabs"],computed:{active:function(){var t;return tt((t=this.$pcTabs)===null||t===void 0?void 0:t.d_value,this.value)},id:function(){var t;return"".concat((t=this.$pcTabs)===null||t===void 0?void 0:t.$id,"_tabpanel_").concat(this.value)},ariaLabelledby:function(){var t;return"".concat((t=this.$pcTabs)===null||t===void 0?void 0:t.$id,"_tab_").concat(this.value)},attrs:function(){return c(this.a11yAttrs,this.ptmi("root",this.ptParams))},a11yAttrs:function(){var t;return{id:this.id,tabindex:(t=this.$pcTabs)===null||t===void 0?void 0:t.tabindex,role:"tabpanel","aria-labelledby":this.ariaLabelledby,"data-pc-name":"tabpanel","data-p-active":this.active}},ptParams:function(){return{context:{active:this.active}}}}};function _t(e,t,n,s,l,a){var b,u;return a.$pcTabs?(r(),f(A,{key:1},[e.asChild?I(e.$slots,"default",{key:1,class:L(e.cx("root")),active:a.active,a11yAttrs:a.a11yAttrs}):(r(),f(A,{key:0},[!((b=a.$pcTabs)!==null&&b!==void 0&&b.lazy)||a.active?M((r(),k(F(e.as),c({key:0,class:e.cx("root")},a.attrs),{default:g(function(){return[I(e.$slots,"default")]}),_:3},16,["class"])),[[_e,(u=a.$pcTabs)!==null&&u!==void 0&&u.lazy?!0:a.active]]):T("",!0)],64))],64)):I(e.$slots,"default",{key:0})}R.render=_t;var Ve={name:"UploadIcon",extends:nt};function $t(e){return Mt(e)||Ut(e)||Ot(e)||Vt()}function Vt(){throw new TypeError(`Invalid attempt to spread non-iterable instance.
In order to be iterable, non-array objects must have a [Symbol.iterator]() method.`)}function Ot(e,t){if(e){if(typeof e=="string")return fe(e,t);var n={}.toString.call(e).slice(8,-1);return n==="Object"&&e.constructor&&(n=e.constructor.name),n==="Map"||n==="Set"?Array.from(e):n==="Arguments"||/^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)?fe(e,t):void 0}}function Ut(e){if(typeof Symbol<"u"&&e[Symbol.iterator]!=null||e["@@iterator"]!=null)return Array.from(e)}function Mt(e){if(Array.isArray(e))return fe(e)}function fe(e,t){(t==null||t>e.length)&&(t=e.length);for(var n=0,s=Array(t);n<t;n++)s[n]=e[n];return s}function Kt(e,t,n,s,l,a){return r(),f("svg",c({width:"14",height:"14",viewBox:"0 0 14 14",fill:"none",xmlns:"http://www.w3.org/2000/svg"},e.pti()),$t(t[0]||(t[0]=[i("path",{"fill-rule":"evenodd","clip-rule":"evenodd",d:"M6.58942 9.82197C6.70165 9.93405 6.85328 9.99793 7.012 10C7.17071 9.99793 7.32234 9.93405 7.43458 9.82197C7.54681 9.7099 7.61079 9.55849 7.61286 9.4V2.04798L9.79204 4.22402C9.84752 4.28011 9.91365 4.32457 9.98657 4.35479C10.0595 4.38502 10.1377 4.40039 10.2167 4.40002C10.2956 4.40039 10.3738 4.38502 10.4467 4.35479C10.5197 4.32457 10.5858 4.28011 10.6413 4.22402C10.7538 4.11152 10.817 3.95902 10.817 3.80002C10.817 3.64102 10.7538 3.48852 10.6413 3.37602L7.45127 0.190618C7.44656 0.185584 7.44176 0.180622 7.43687 0.175736C7.32419 0.063214 7.17136 0 7.012 0C6.85264 0 6.69981 0.063214 6.58712 0.175736C6.58181 0.181045 6.5766 0.186443 6.5715 0.191927L3.38282 3.37602C3.27669 3.48976 3.2189 3.6402 3.22165 3.79564C3.2244 3.95108 3.28746 4.09939 3.39755 4.20932C3.50764 4.31925 3.65616 4.38222 3.81182 4.38496C3.96749 4.3877 4.11814 4.33001 4.23204 4.22402L6.41113 2.04807V9.4C6.41321 9.55849 6.47718 9.7099 6.58942 9.82197ZM11.9952 14H2.02883C1.751 13.9887 1.47813 13.9228 1.22584 13.8061C0.973545 13.6894 0.746779 13.5241 0.558517 13.3197C0.370254 13.1154 0.22419 12.876 0.128681 12.6152C0.0331723 12.3545 -0.00990605 12.0775 0.0019109 11.8V9.40005C0.0019109 9.24092 0.065216 9.08831 0.1779 8.97579C0.290584 8.86326 0.443416 8.80005 0.602775 8.80005C0.762134 8.80005 0.914966 8.86326 1.02765 8.97579C1.14033 9.08831 1.20364 9.24092 1.20364 9.40005V11.8C1.18295 12.0376 1.25463 12.274 1.40379 12.4602C1.55296 12.6463 1.76817 12.7681 2.00479 12.8H11.9952C12.2318 12.7681 12.447 12.6463 12.5962 12.4602C12.7453 12.274 12.817 12.0376 12.7963 11.8V9.40005C12.7963 9.24092 12.8596 9.08831 12.9723 8.97579C13.085 8.86326 13.2378 8.80005 13.3972 8.80005C13.5565 8.80005 13.7094 8.86326 13.8221 8.97579C13.9347 9.08831 13.998 9.24092 13.998 9.40005V11.8C14.022 12.3563 13.8251 12.8996 13.45 13.3116C13.0749 13.7236 12.552 13.971 11.9952 14Z",fill:"currentColor"},null,-1)])),16)}Ve.render=Kt;var Nt=`
    .p-message {
        display: grid;
        grid-template-rows: 1fr;
        border-radius: dt('message.border.radius');
        outline-width: dt('message.border.width');
        outline-style: solid;
    }

    .p-message-content-wrapper {
        min-height: 0;
    }

    .p-message-content {
        display: flex;
        align-items: center;
        padding: dt('message.content.padding');
        gap: dt('message.content.gap');
    }

    .p-message-icon {
        flex-shrink: 0;
    }

    .p-message-close-button {
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        margin-inline-start: auto;
        overflow: hidden;
        position: relative;
        width: dt('message.close.button.width');
        height: dt('message.close.button.height');
        border-radius: dt('message.close.button.border.radius');
        background: transparent;
        transition:
            background dt('message.transition.duration'),
            color dt('message.transition.duration'),
            outline-color dt('message.transition.duration'),
            box-shadow dt('message.transition.duration'),
            opacity 0.3s;
        outline-color: transparent;
        color: inherit;
        padding: 0;
        border: none;
        cursor: pointer;
        user-select: none;
    }

    .p-message-close-icon {
        font-size: dt('message.close.icon.size');
        width: dt('message.close.icon.size');
        height: dt('message.close.icon.size');
    }

    .p-message-close-button:focus-visible {
        outline-width: dt('message.close.button.focus.ring.width');
        outline-style: dt('message.close.button.focus.ring.style');
        outline-offset: dt('message.close.button.focus.ring.offset');
    }

    .p-message-info {
        background: dt('message.info.background');
        outline-color: dt('message.info.border.color');
        color: dt('message.info.color');
        box-shadow: dt('message.info.shadow');
    }

    .p-message-info .p-message-close-button:focus-visible {
        outline-color: dt('message.info.close.button.focus.ring.color');
        box-shadow: dt('message.info.close.button.focus.ring.shadow');
    }

    .p-message-info .p-message-close-button:hover {
        background: dt('message.info.close.button.hover.background');
    }

    .p-message-info.p-message-outlined {
        color: dt('message.info.outlined.color');
        outline-color: dt('message.info.outlined.border.color');
    }

    .p-message-info.p-message-simple {
        color: dt('message.info.simple.color');
    }

    .p-message-success {
        background: dt('message.success.background');
        outline-color: dt('message.success.border.color');
        color: dt('message.success.color');
        box-shadow: dt('message.success.shadow');
    }

    .p-message-success .p-message-close-button:focus-visible {
        outline-color: dt('message.success.close.button.focus.ring.color');
        box-shadow: dt('message.success.close.button.focus.ring.shadow');
    }

    .p-message-success .p-message-close-button:hover {
        background: dt('message.success.close.button.hover.background');
    }

    .p-message-success.p-message-outlined {
        color: dt('message.success.outlined.color');
        outline-color: dt('message.success.outlined.border.color');
    }

    .p-message-success.p-message-simple {
        color: dt('message.success.simple.color');
    }

    .p-message-warn {
        background: dt('message.warn.background');
        outline-color: dt('message.warn.border.color');
        color: dt('message.warn.color');
        box-shadow: dt('message.warn.shadow');
    }

    .p-message-warn .p-message-close-button:focus-visible {
        outline-color: dt('message.warn.close.button.focus.ring.color');
        box-shadow: dt('message.warn.close.button.focus.ring.shadow');
    }

    .p-message-warn .p-message-close-button:hover {
        background: dt('message.warn.close.button.hover.background');
    }

    .p-message-warn.p-message-outlined {
        color: dt('message.warn.outlined.color');
        outline-color: dt('message.warn.outlined.border.color');
    }

    .p-message-warn.p-message-simple {
        color: dt('message.warn.simple.color');
    }

    .p-message-error {
        background: dt('message.error.background');
        outline-color: dt('message.error.border.color');
        color: dt('message.error.color');
        box-shadow: dt('message.error.shadow');
    }

    .p-message-error .p-message-close-button:focus-visible {
        outline-color: dt('message.error.close.button.focus.ring.color');
        box-shadow: dt('message.error.close.button.focus.ring.shadow');
    }

    .p-message-error .p-message-close-button:hover {
        background: dt('message.error.close.button.hover.background');
    }

    .p-message-error.p-message-outlined {
        color: dt('message.error.outlined.color');
        outline-color: dt('message.error.outlined.border.color');
    }

    .p-message-error.p-message-simple {
        color: dt('message.error.simple.color');
    }

    .p-message-secondary {
        background: dt('message.secondary.background');
        outline-color: dt('message.secondary.border.color');
        color: dt('message.secondary.color');
        box-shadow: dt('message.secondary.shadow');
    }

    .p-message-secondary .p-message-close-button:focus-visible {
        outline-color: dt('message.secondary.close.button.focus.ring.color');
        box-shadow: dt('message.secondary.close.button.focus.ring.shadow');
    }

    .p-message-secondary .p-message-close-button:hover {
        background: dt('message.secondary.close.button.hover.background');
    }

    .p-message-secondary.p-message-outlined {
        color: dt('message.secondary.outlined.color');
        outline-color: dt('message.secondary.outlined.border.color');
    }

    .p-message-secondary.p-message-simple {
        color: dt('message.secondary.simple.color');
    }

    .p-message-contrast {
        background: dt('message.contrast.background');
        outline-color: dt('message.contrast.border.color');
        color: dt('message.contrast.color');
        box-shadow: dt('message.contrast.shadow');
    }

    .p-message-contrast .p-message-close-button:focus-visible {
        outline-color: dt('message.contrast.close.button.focus.ring.color');
        box-shadow: dt('message.contrast.close.button.focus.ring.shadow');
    }

    .p-message-contrast .p-message-close-button:hover {
        background: dt('message.contrast.close.button.hover.background');
    }

    .p-message-contrast.p-message-outlined {
        color: dt('message.contrast.outlined.color');
        outline-color: dt('message.contrast.outlined.border.color');
    }

    .p-message-contrast.p-message-simple {
        color: dt('message.contrast.simple.color');
    }

    .p-message-text {
        font-size: dt('message.text.font.size');
        font-weight: dt('message.text.font.weight');
    }

    .p-message-icon {
        font-size: dt('message.icon.size');
        width: dt('message.icon.size');
        height: dt('message.icon.size');
    }

    .p-message-sm .p-message-content {
        padding: dt('message.content.sm.padding');
    }

    .p-message-sm .p-message-text {
        font-size: dt('message.text.sm.font.size');
    }

    .p-message-sm .p-message-icon {
        font-size: dt('message.icon.sm.size');
        width: dt('message.icon.sm.size');
        height: dt('message.icon.sm.size');
    }

    .p-message-sm .p-message-close-icon {
        font-size: dt('message.close.icon.sm.size');
        width: dt('message.close.icon.sm.size');
        height: dt('message.close.icon.sm.size');
    }

    .p-message-lg .p-message-content {
        padding: dt('message.content.lg.padding');
    }

    .p-message-lg .p-message-text {
        font-size: dt('message.text.lg.font.size');
    }

    .p-message-lg .p-message-icon {
        font-size: dt('message.icon.lg.size');
        width: dt('message.icon.lg.size');
        height: dt('message.icon.lg.size');
    }

    .p-message-lg .p-message-close-icon {
        font-size: dt('message.close.icon.lg.size');
        width: dt('message.close.icon.lg.size');
        height: dt('message.close.icon.lg.size');
    }

    .p-message-outlined {
        background: transparent;
        outline-width: dt('message.outlined.border.width');
    }

    .p-message-simple {
        background: transparent;
        outline-color: transparent;
        box-shadow: none;
    }

    .p-message-simple .p-message-content {
        padding: dt('message.simple.content.padding');
    }

    .p-message-outlined .p-message-close-button:hover,
    .p-message-simple .p-message-close-button:hover {
        background: transparent;
    }

    .p-message-enter-active {
        animation: p-animate-message-enter 0.3s ease-out forwards;
        overflow: hidden;
    }

    .p-message-leave-active {
        animation: p-animate-message-leave 0.15s ease-in forwards;
        overflow: hidden;
    }

    @keyframes p-animate-message-enter {
        from {
            opacity: 0;
            grid-template-rows: 0fr;
        }
        to {
            opacity: 1;
            grid-template-rows: 1fr;
        }
    }

    @keyframes p-animate-message-leave {
        from {
            opacity: 1;
            grid-template-rows: 1fr;
        }
        to {
            opacity: 0;
            margin: 0;
            grid-template-rows: 0fr;
        }
    }
`,Ht={root:function(t){var n=t.props;return["p-message p-component p-message-"+n.severity,{"p-message-outlined":n.variant==="outlined","p-message-simple":n.variant==="simple","p-message-sm":n.size==="small","p-message-lg":n.size==="large"}]},contentWrapper:"p-message-content-wrapper",content:"p-message-content",icon:"p-message-icon",text:"p-message-text",closeButton:"p-message-close-button",closeIcon:"p-message-close-icon"},Rt=ie.extend({name:"message",style:Nt,classes:Ht}),qt={name:"BaseMessage",extends:J,props:{severity:{type:String,default:"info"},closable:{type:Boolean,default:!1},life:{type:Number,default:null},icon:{type:String,default:void 0},closeIcon:{type:String,default:void 0},closeButtonProps:{type:null,default:null},size:{type:String,default:null},variant:{type:String,default:null}},style:Rt,provide:function(){return{$pcMessage:this,$parentInstance:this}}};function W(e){"@babel/helpers - typeof";return W=typeof Symbol=="function"&&typeof Symbol.iterator=="symbol"?function(t){return typeof t}:function(t){return t&&typeof Symbol=="function"&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},W(e)}function xe(e,t,n){return(t=Wt(t))in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function Wt(e){var t=Qt(e,"string");return W(t)=="symbol"?t:t+""}function Qt(e,t){if(W(e)!="object"||!e)return e;var n=e[Symbol.toPrimitive];if(n!==void 0){var s=n.call(e,t);if(W(s)!="object")return s;throw new TypeError("@@toPrimitive must return a primitive value.")}return(t==="string"?String:Number)(e)}var Oe={name:"Message",extends:qt,inheritAttrs:!1,emits:["close","life-end"],timeout:null,data:function(){return{visible:!0}},mounted:function(){var t=this;this.life&&setTimeout(function(){t.visible=!1,t.$emit("life-end")},this.life)},methods:{close:function(t){this.visible=!1,this.$emit("close",t)}},computed:{closeAriaLabel:function(){return this.$primevue.config.locale.aria?this.$primevue.config.locale.aria.close:void 0},dataP:function(){return at(xe(xe({outlined:this.variant==="outlined",simple:this.variant==="simple"},this.severity,this.severity),this.size,this.size))}},directives:{ripple:be},components:{TimesIcon:he}};function Q(e){"@babel/helpers - typeof";return Q=typeof Symbol=="function"&&typeof Symbol.iterator=="symbol"?function(t){return typeof t}:function(t){return t&&typeof Symbol=="function"&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},Q(e)}function De(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var s=Object.getOwnPropertySymbols(e);t&&(s=s.filter(function(l){return Object.getOwnPropertyDescriptor(e,l).enumerable})),n.push.apply(n,s)}return n}function Ee(e){for(var t=1;t<arguments.length;t++){var n=arguments[t]!=null?arguments[t]:{};t%2?De(Object(n),!0).forEach(function(s){Jt(e,s,n[s])}):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):De(Object(n)).forEach(function(s){Object.defineProperty(e,s,Object.getOwnPropertyDescriptor(n,s))})}return e}function Jt(e,t,n){return(t=Gt(t))in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function Gt(e){var t=Yt(e,"string");return Q(t)=="symbol"?t:t+""}function Yt(e,t){if(Q(e)!="object"||!e)return e;var n=e[Symbol.toPrimitive];if(n!==void 0){var s=n.call(e,t);if(Q(s)!="object")return s;throw new TypeError("@@toPrimitive must return a primitive value.")}return(t==="string"?String:Number)(e)}var Zt=["data-p"],Xt=["data-p"],en=["data-p"],tn=["aria-label","data-p"],nn=["data-p"];function an(e,t,n,s,l,a){var b=E("TimesIcon"),u=je("ripple");return r(),k(st,c({name:"p-message",appear:""},e.ptmi("transition")),{default:g(function(){return[l.visible?(r(),f("div",c({key:0,class:e.cx("root"),role:"alert","aria-live":"assertive","aria-atomic":"true","data-p":a.dataP},e.ptm("root")),[i("div",c({class:e.cx("contentWrapper")},e.ptm("contentWrapper")),[e.$slots.container?I(e.$slots,"container",{key:0,closeCallback:a.close}):(r(),f("div",c({key:1,class:e.cx("content"),"data-p":a.dataP},e.ptm("content")),[I(e.$slots,"icon",{class:L(e.cx("icon"))},function(){return[(r(),k(F(e.icon?"span":null),c({class:[e.cx("icon"),e.icon],"data-p":a.dataP},e.ptm("icon")),null,16,["class","data-p"]))]}),e.$slots.default?(r(),f("div",c({key:0,class:e.cx("text"),"data-p":a.dataP},e.ptm("text")),[I(e.$slots,"default")],16,en)):T("",!0),e.closable?M((r(),f("button",c({key:1,class:e.cx("closeButton"),"aria-label":a.closeAriaLabel,type:"button",onClick:t[0]||(t[0]=function(m){return a.close(m)}),"data-p":a.dataP},Ee(Ee({},e.closeButtonProps),e.ptm("closeButton"))),[I(e.$slots,"closeicon",{},function(){return[e.closeIcon?(r(),f("i",c({key:0,class:[e.cx("closeIcon"),e.closeIcon],"data-p":a.dataP},e.ptm("closeIcon")),null,16,nn)):(r(),k(b,c({key:1,class:[e.cx("closeIcon"),e.closeIcon],"data-p":a.dataP},e.ptm("closeIcon")),null,16,["class","data-p"]))]})],16,tn)),[[u]]):T("",!0)],16,Xt))],16)],16,Zt)):T("",!0)]}),_:3},16)}Oe.render=an;var sn=`
    .p-fileupload input[type='file'] {
        display: none;
    }

    .p-fileupload-advanced {
        border: 1px solid dt('fileupload.border.color');
        border-radius: dt('fileupload.border.radius');
        background: dt('fileupload.background');
        color: dt('fileupload.color');
    }

    .p-fileupload-header {
        display: flex;
        align-items: center;
        padding: dt('fileupload.header.padding');
        background: dt('fileupload.header.background');
        color: dt('fileupload.header.color');
        border-style: solid;
        border-width: dt('fileupload.header.border.width');
        border-color: dt('fileupload.header.border.color');
        border-radius: dt('fileupload.header.border.radius');
        gap: dt('fileupload.header.gap');
    }

    .p-fileupload-content {
        border: 1px solid transparent;
        display: flex;
        flex-direction: column;
        gap: dt('fileupload.content.gap');
        transition: border-color dt('fileupload.transition.duration');
        padding: dt('fileupload.content.padding');
    }

    .p-fileupload-content .p-progressbar {
        width: 100%;
        height: dt('fileupload.progressbar.height');
    }

    .p-fileupload-file-list {
        display: flex;
        flex-direction: column;
        gap: dt('fileupload.filelist.gap');
    }

    .p-fileupload-file {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        padding: dt('fileupload.file.padding');
        border-block-end: 1px solid dt('fileupload.file.border.color');
        gap: dt('fileupload.file.gap');
    }

    .p-fileupload-file:last-child {
        border-block-end: 0;
    }

    .p-fileupload-file-info {
        display: flex;
        flex-direction: column;
        gap: dt('fileupload.file.info.gap');
    }

    .p-fileupload-file-thumbnail {
        flex-shrink: 0;
    }

    .p-fileupload-file-actions {
        margin-inline-start: auto;
    }

    .p-fileupload-highlight {
        border: 1px dashed dt('fileupload.content.highlight.border.color');
    }

    .p-fileupload-basic .p-message {
        margin-block-end: dt('fileupload.basic.gap');
    }

    .p-fileupload-basic-content {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        gap: dt('fileupload.basic.gap');
    }
`,on={root:function(t){var n=t.props;return["p-fileupload p-fileupload-".concat(n.mode," p-component")]},header:"p-fileupload-header",pcChooseButton:"p-fileupload-choose-button",pcUploadButton:"p-fileupload-upload-button",pcCancelButton:"p-fileupload-cancel-button",content:"p-fileupload-content",fileList:"p-fileupload-file-list",file:"p-fileupload-file",fileThumbnail:"p-fileupload-file-thumbnail",fileInfo:"p-fileupload-file-info",fileName:"p-fileupload-file-name",fileSize:"p-fileupload-file-size",pcFileBadge:"p-fileupload-file-badge",fileActions:"p-fileupload-file-actions",pcFileRemoveButton:"p-fileupload-file-remove-button",basicContent:"p-fileupload-basic-content"},ln=ie.extend({name:"fileupload",style:sn,classes:on}),rn={name:"BaseFileUpload",extends:J,props:{name:{type:String,default:null},url:{type:String,default:null},mode:{type:String,default:"advanced"},multiple:{type:Boolean,default:!1},accept:{type:String,default:null},disabled:{type:Boolean,default:!1},auto:{type:Boolean,default:!1},maxFileSize:{type:Number,default:null},invalidFileSizeMessage:{type:String,default:"{0}: Invalid file size, file size should be smaller than {1}."},invalidFileTypeMessage:{type:String,default:"{0}: Invalid file type, allowed file types: {1}."},fileLimit:{type:Number,default:null},invalidFileLimitMessage:{type:String,default:"Maximum number of files exceeded, limit is {0} at most."},withCredentials:{type:Boolean,default:!1},previewWidth:{type:Number,default:50},chooseLabel:{type:String,default:null},uploadLabel:{type:String,default:null},cancelLabel:{type:String,default:null},customUpload:{type:Boolean,default:!1},showUploadButton:{type:Boolean,default:!0},showCancelButton:{type:Boolean,default:!0},chooseIcon:{type:String,default:void 0},uploadIcon:{type:String,default:void 0},cancelIcon:{type:String,default:void 0},style:null,class:null,chooseButtonProps:{type:null,default:null},uploadButtonProps:{type:Object,default:function(){return{severity:"secondary"}}},cancelButtonProps:{type:Object,default:function(){return{severity:"secondary"}}}},style:ln,provide:function(){return{$pcFileUpload:this,$parentInstance:this}}},Ue={name:"FileContent",hostName:"FileUpload",extends:J,emits:["remove"],props:{files:{type:Array,default:function(){return[]}},badgeSeverity:{type:String,default:"warn"},badgeValue:{type:String,default:null},previewWidth:{type:Number,default:50},templates:{type:null,default:null}},methods:{formatSize:function(t){var n,s=1024,l=3,a=((n=this.$primevue.config.locale)===null||n===void 0?void 0:n.fileSizeTypes)||["B","KB","MB","GB","TB","PB","EB","ZB","YB"];if(t===0)return"0 ".concat(a[0]);var b=Math.floor(Math.log(t)/Math.log(s)),u=parseFloat((t/Math.pow(s,b)).toFixed(l));return"".concat(u," ").concat(a[b])}},components:{Button:j,Badge:ot,TimesIcon:he}},un=["alt","src","width"];function dn(e,t,n,s,l,a){var b=E("Badge"),u=E("TimesIcon"),m=E("Button");return r(!0),f(A,null,_(n.files,function(C,y){return r(),f("div",c({key:C.name+C.type+C.size,class:e.cx("file")},{ref_for:!0},e.ptm("file")),[i("img",c({role:"presentation",class:e.cx("fileThumbnail"),alt:C.name,src:C.objectURL,width:n.previewWidth},{ref_for:!0},e.ptm("fileThumbnail")),null,16,un),i("div",c({class:e.cx("fileInfo")},{ref_for:!0},e.ptm("fileInfo")),[i("div",c({class:e.cx("fileName")},{ref_for:!0},e.ptm("fileName")),d(C.name),17),i("span",c({class:e.cx("fileSize")},{ref_for:!0},e.ptm("fileSize")),d(a.formatSize(C.size)),17)],16),p(b,{value:n.badgeValue,class:L(e.cx("pcFileBadge")),severity:n.badgeSeverity,unstyled:e.unstyled,pt:e.ptm("pcFileBadge")},null,8,["value","class","severity","unstyled","pt"]),i("div",c({class:e.cx("fileActions")},{ref_for:!0},e.ptm("fileActions")),[p(m,{onClick:function(G){return e.$emit("remove",y)},text:"",rounded:"",severity:"danger",class:L(e.cx("pcFileRemoveButton")),unstyled:e.unstyled,pt:e.ptm("pcFileRemoveButton")},{icon:g(function($){return[n.templates.fileremoveicon?(r(),k(F(n.templates.fileremoveicon),{key:0,class:L($.class),file:C,index:y},null,8,["class","file","index"])):(r(),k(u,c({key:1,class:$.class,"aria-hidden":"true"},{ref_for:!0},e.ptm("pcFileRemoveButton").icon),null,16,["class"]))]}),_:2},1032,["onClick","class","unstyled","pt"])],16)],16)}),128)}Ue.render=dn;function ce(e){return fn(e)||pn(e)||Me(e)||cn()}function cn(){throw new TypeError(`Invalid attempt to spread non-iterable instance.
In order to be iterable, non-array objects must have a [Symbol.iterator]() method.`)}function pn(e){if(typeof Symbol<"u"&&e[Symbol.iterator]!=null||e["@@iterator"]!=null)return Array.from(e)}function fn(e){if(Array.isArray(e))return me(e)}function oe(e,t){var n=typeof Symbol<"u"&&e[Symbol.iterator]||e["@@iterator"];if(!n){if(Array.isArray(e)||(n=Me(e))||t){n&&(e=n);var s=0,l=function(){};return{s:l,n:function(){return s>=e.length?{done:!0}:{done:!1,value:e[s++]}},e:function(C){throw C},f:l}}throw new TypeError(`Invalid attempt to iterate non-iterable instance.
In order to be iterable, non-array objects must have a [Symbol.iterator]() method.`)}var a,b=!0,u=!1;return{s:function(){n=n.call(e)},n:function(){var C=n.next();return b=C.done,C},e:function(C){u=!0,a=C},f:function(){try{b||n.return==null||n.return()}finally{if(u)throw a}}}}function Me(e,t){if(e){if(typeof e=="string")return me(e,t);var n={}.toString.call(e).slice(8,-1);return n==="Object"&&e.constructor&&(n=e.constructor.name),n==="Map"||n==="Set"?Array.from(e):n==="Arguments"||/^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)?me(e,t):void 0}}function me(e,t){(t==null||t>e.length)&&(t=e.length);for(var n=0,s=Array(t);n<t;n++)s[n]=e[n];return s}var Ke={name:"FileUpload",extends:rn,inheritAttrs:!1,emits:["select","uploader","before-upload","progress","upload","error","before-send","clear","remove","remove-uploaded-file"],duplicateIEEvent:!1,data:function(){return{uploadedFileCount:0,files:[],messages:[],focused:!1,progress:null,uploadedFiles:[]}},methods:{upload:function(){this.hasFiles&&this.uploader()},onBasicUploaderClick:function(t){t.button===0&&this.$refs.fileInput.click()},onFileSelect:function(t){if(t.type!=="drop"&&this.isIE11()&&this.duplicateIEEvent){this.duplicateIEEvent=!1;return}this.isBasic&&this.hasFiles&&(this.files=[]),this.messages=[],this.files=this.files||[];var n=t.dataTransfer?t.dataTransfer.files:t.target.files,s=oe(n),l;try{for(s.s();!(l=s.n()).done;){var a=l.value;!this.isFileSelected(a)&&!this.isFileLimitExceeded()&&this.validate(a)&&(this.isImage(a)&&(a.objectURL=window.URL.createObjectURL(a)),this.files.push(a))}}catch(b){s.e(b)}finally{s.f()}this.$emit("select",{originalEvent:t,files:this.files}),this.fileLimit&&this.checkFileLimit(),this.auto&&this.hasFiles&&!this.isFileLimitExceeded()&&this.uploader(),t.type!=="drop"&&this.isIE11()?this.clearIEInput():this.clearInputElement()},choose:function(){this.$refs.fileInput.click()},uploader:function(){var t=this;if(this.customUpload)this.fileLimit&&(this.uploadedFileCount+=this.files.length),this.$emit("uploader",{files:this.files});else{var n=new XMLHttpRequest,s=new FormData;this.$emit("before-upload",{xhr:n,formData:s});var l=oe(this.files),a;try{for(l.s();!(a=l.n()).done;){var b=a.value;s.append(this.name,b,b.name)}}catch(u){l.e(u)}finally{l.f()}n.upload.addEventListener("progress",function(u){u.lengthComputable&&(t.progress=Math.round(u.loaded*100/u.total)),t.$emit("progress",{originalEvent:u,progress:t.progress})}),n.onreadystatechange=function(){if(n.readyState===4){if(t.progress=0,n.status>=200&&n.status<300){var u;t.fileLimit&&(t.uploadedFileCount+=t.files.length),t.$emit("upload",{xhr:n,files:t.files}),(u=t.uploadedFiles).push.apply(u,ce(t.files))}else t.$emit("error",{xhr:n,files:t.files});t.clear()}},this.url&&(n.open("POST",this.url,!0),this.$emit("before-send",{xhr:n,formData:s}),n.withCredentials=this.withCredentials,n.send(s))}},clear:function(){this.files=[],this.messages=null,this.$emit("clear"),this.isAdvanced&&this.clearInputElement()},onFocus:function(){this.focused=!0},onBlur:function(){this.focused=!1},isFileSelected:function(t){if(this.files&&this.files.length){var n=oe(this.files),s;try{for(n.s();!(s=n.n()).done;){var l=s.value;if(l.name+l.type+l.size===t.name+t.type+t.size)return!0}}catch(a){n.e(a)}finally{n.f()}}return!1},isIE11:function(){return!!window.MSInputMethodContext&&!!document.documentMode},validate:function(t){return this.accept&&!this.isFileTypeValid(t)?(this.messages.push(this.invalidFileTypeMessage.replace("{0}",t.name).replace("{1}",this.accept)),!1):this.maxFileSize&&t.size>this.maxFileSize?(this.messages.push(this.invalidFileSizeMessage.replace("{0}",t.name).replace("{1}",this.formatSize(this.maxFileSize))),!1):!0},isFileTypeValid:function(t){var n=this.accept.split(",").map(function(u){return u.trim()}),s=oe(n),l;try{for(s.s();!(l=s.n()).done;){var a=l.value,b=this.isWildcard(a)?this.getTypeClass(t.type)===this.getTypeClass(a):t.type==a||this.getFileExtension(t).toLowerCase()===a.toLowerCase();if(b)return!0}}catch(u){s.e(u)}finally{s.f()}return!1},getTypeClass:function(t){return t.substring(0,t.indexOf("/"))},isWildcard:function(t){return t.indexOf("*")!==-1},getFileExtension:function(t){return"."+t.name.split(".").pop()},isImage:function(t){return/^image\//.test(t.type)},onDragEnter:function(t){!this.disabled&&(!this.hasFiles||this.multiple)&&(t.stopPropagation(),t.preventDefault())},onDragOver:function(t){!this.disabled&&(!this.hasFiles||this.multiple)&&(!this.isUnstyled&&it(this.$refs.content,"p-fileupload-highlight"),this.$refs.content.setAttribute("data-p-highlight",!0),t.stopPropagation(),t.preventDefault())},onDragLeave:function(){this.disabled||(!this.isUnstyled&&Fe(this.$refs.content,"p-fileupload-highlight"),this.$refs.content.setAttribute("data-p-highlight",!1))},onDrop:function(t){if(!this.disabled){!this.isUnstyled&&Fe(this.$refs.content,"p-fileupload-highlight"),this.$refs.content.setAttribute("data-p-highlight",!1),t.stopPropagation(),t.preventDefault();var n=t.dataTransfer?t.dataTransfer.files:t.target.files,s=this.multiple||n&&n.length===1;s&&this.onFileSelect(t)}},remove:function(t){this.clearInputElement();var n=this.files.splice(t,1)[0];this.files=ce(this.files),this.$emit("remove",{file:n,files:this.files})},removeUploadedFile:function(t){var n=this.uploadedFiles.splice(t,1)[0];this.uploadedFiles=ce(this.uploadedFiles),this.$emit("remove-uploaded-file",{file:n,files:this.uploadedFiles})},clearInputElement:function(){this.$refs.fileInput.value=""},clearIEInput:function(){this.$refs.fileInput&&(this.duplicateIEEvent=!0,this.$refs.fileInput.value="")},formatSize:function(t){var n,s=1024,l=3,a=((n=this.$primevue.config.locale)===null||n===void 0?void 0:n.fileSizeTypes)||["B","KB","MB","GB","TB","PB","EB","ZB","YB"];if(t===0)return"0 ".concat(a[0]);var b=Math.floor(Math.log(t)/Math.log(s)),u=parseFloat((t/Math.pow(s,b)).toFixed(l));return"".concat(u," ").concat(a[b])},isFileLimitExceeded:function(){return this.fileLimit&&this.fileLimit<=this.files.length+this.uploadedFileCount&&this.focused&&(this.focused=!1),this.fileLimit&&this.fileLimit<this.files.length+this.uploadedFileCount},checkFileLimit:function(){this.isFileLimitExceeded()&&this.messages.push(this.invalidFileLimitMessage.replace("{0}",this.fileLimit.toString()))},onMessageClose:function(){this.messages=null}},computed:{isAdvanced:function(){return this.mode==="advanced"},isBasic:function(){return this.mode==="basic"},chooseButtonClass:function(){return[this.cx("pcChooseButton"),this.class]},basicFileChosenLabel:function(){var t;if(this.auto)return this.chooseButtonLabel;if(this.hasFiles){var n;return this.files&&this.files.length===1?this.files[0].name:(n=this.$primevue.config.locale)===null||n===void 0||(n=n.fileChosenMessage)===null||n===void 0?void 0:n.replace("{0}",this.files.length)}return((t=this.$primevue.config.locale)===null||t===void 0?void 0:t.noFileChosenMessage)||""},hasFiles:function(){return this.files&&this.files.length>0},hasUploadedFiles:function(){return this.uploadedFiles&&this.uploadedFiles.length>0},chooseDisabled:function(){return this.disabled||this.fileLimit&&this.fileLimit<=this.files.length+this.uploadedFileCount},uploadDisabled:function(){return this.disabled||!this.hasFiles||this.fileLimit&&this.fileLimit<this.files.length},cancelDisabled:function(){return this.disabled||!this.hasFiles},chooseButtonLabel:function(){return this.chooseLabel||this.$primevue.config.locale.choose},uploadButtonLabel:function(){return this.uploadLabel||this.$primevue.config.locale.upload},cancelButtonLabel:function(){return this.cancelLabel||this.$primevue.config.locale.cancel},completedLabel:function(){return this.$primevue.config.locale.completed},pendingLabel:function(){return this.$primevue.config.locale.pending}},components:{Button:j,ProgressBar:pe,Message:Oe,FileContent:Ue,PlusIcon:ht,UploadIcon:Ve,TimesIcon:he},directives:{ripple:be}},mn=["multiple","accept","disabled"],bn=["accept","disabled","multiple"];function hn(e,t,n,s,l,a){var b=E("Button"),u=E("ProgressBar"),m=E("Message"),C=E("FileContent");return a.isAdvanced?(r(),f("div",c({key:0,class:e.cx("root")},e.ptmi("root")),[i("input",c({ref:"fileInput",type:"file",onChange:t[0]||(t[0]=function(){return a.onFileSelect&&a.onFileSelect.apply(a,arguments)}),multiple:e.multiple,accept:e.accept,disabled:a.chooseDisabled},e.ptm("input")),null,16,mn),i("div",c({class:e.cx("header")},e.ptm("header")),[I(e.$slots,"header",{files:l.files,uploadedFiles:l.uploadedFiles,chooseCallback:a.choose,uploadCallback:a.uploader,clearCallback:a.clear},function(){return[p(b,c({label:a.chooseButtonLabel,class:a.chooseButtonClass,style:e.style,disabled:e.disabled,unstyled:e.unstyled,onClick:a.choose,onKeydown:Ae(a.choose,["enter"]),onFocus:a.onFocus,onBlur:a.onBlur},e.chooseButtonProps,{pt:e.ptm("pcChooseButton")}),{icon:g(function(y){return[I(e.$slots,"chooseicon",{},function(){return[(r(),k(F(e.chooseIcon?"span":"PlusIcon"),c({class:[y.class,e.chooseIcon],"aria-hidden":"true"},e.ptm("pcChooseButton").icon),null,16,["class"]))]})]}),_:3},16,["label","class","style","disabled","unstyled","onClick","onKeydown","onFocus","onBlur","pt"]),e.showUploadButton?(r(),k(b,c({key:0,class:e.cx("pcUploadButton"),label:a.uploadButtonLabel,onClick:a.uploader,disabled:a.uploadDisabled,unstyled:e.unstyled},e.uploadButtonProps,{pt:e.ptm("pcUploadButton")}),{icon:g(function(y){return[I(e.$slots,"uploadicon",{},function(){return[(r(),k(F(e.uploadIcon?"span":"UploadIcon"),c({class:[y.class,e.uploadIcon],"aria-hidden":"true"},e.ptm("pcUploadButton").icon,{"data-pc-section":"uploadbuttonicon"}),null,16,["class"]))]})]}),_:3},16,["class","label","onClick","disabled","unstyled","pt"])):T("",!0),e.showCancelButton?(r(),k(b,c({key:1,class:e.cx("pcCancelButton"),label:a.cancelButtonLabel,onClick:a.clear,disabled:a.cancelDisabled,unstyled:e.unstyled},e.cancelButtonProps,{pt:e.ptm("pcCancelButton")}),{icon:g(function(y){return[I(e.$slots,"cancelicon",{},function(){return[(r(),k(F(e.cancelIcon?"span":"TimesIcon"),c({class:[y.class,e.cancelIcon],"aria-hidden":"true"},e.ptm("pcCancelButton").icon,{"data-pc-section":"cancelbuttonicon"}),null,16,["class"]))]})]}),_:3},16,["class","label","onClick","disabled","unstyled","pt"])):T("",!0)]})],16),i("div",c({ref:"content",class:e.cx("content"),onDragenter:t[1]||(t[1]=function(){return a.onDragEnter&&a.onDragEnter.apply(a,arguments)}),onDragover:t[2]||(t[2]=function(){return a.onDragOver&&a.onDragOver.apply(a,arguments)}),onDragleave:t[3]||(t[3]=function(){return a.onDragLeave&&a.onDragLeave.apply(a,arguments)}),onDrop:t[4]||(t[4]=function(){return a.onDrop&&a.onDrop.apply(a,arguments)})},e.ptm("content"),{"data-p-highlight":!1}),[I(e.$slots,"content",{files:l.files,uploadedFiles:l.uploadedFiles,removeUploadedFileCallback:a.removeUploadedFile,removeFileCallback:a.remove,progress:l.progress,messages:l.messages},function(){return[a.hasFiles?(r(),k(u,{key:0,value:l.progress,showValue:!1,unstyled:e.unstyled,pt:e.ptm("pcProgressbar")},null,8,["value","unstyled","pt"])):T("",!0),(r(!0),f(A,null,_(l.messages,function(y){return r(),k(m,{key:y,severity:"error",onClose:a.onMessageClose,unstyled:e.unstyled,pt:e.ptm("pcMessage")},{default:g(function(){return[x(d(y),1)]}),_:2},1032,["onClose","unstyled","pt"])}),128)),a.hasFiles?(r(),f("div",{key:1,class:L(e.cx("fileList"))},[p(C,{files:l.files,onRemove:a.remove,badgeValue:a.pendingLabel,previewWidth:e.previewWidth,templates:e.$slots,unstyled:e.unstyled,pt:e.pt},null,8,["files","onRemove","badgeValue","previewWidth","templates","unstyled","pt"])],2)):T("",!0),a.hasUploadedFiles?(r(),f("div",{key:2,class:L(e.cx("fileList"))},[p(C,{files:l.uploadedFiles,onRemove:a.removeUploadedFile,badgeValue:a.completedLabel,badgeSeverity:"success",previewWidth:e.previewWidth,templates:e.$slots,unstyled:e.unstyled,pt:e.pt},null,8,["files","onRemove","badgeValue","previewWidth","templates","unstyled","pt"])],2)):T("",!0)]}),e.$slots.empty&&!a.hasFiles&&!a.hasUploadedFiles?(r(),f("div",lt(c({key:0},e.ptm("empty"))),[I(e.$slots,"empty")],16)):T("",!0)],16)],16)):a.isBasic?(r(),f("div",c({key:1,class:e.cx("root")},e.ptmi("root")),[(r(!0),f(A,null,_(l.messages,function(y){return r(),k(m,{key:y,severity:"error",onClose:a.onMessageClose,unstyled:e.unstyled,pt:e.ptm("pcMessage")},{default:g(function(){return[x(d(y),1)]}),_:2},1032,["onClose","unstyled","pt"])}),128)),i("div",c({class:e.cx("basicContent")},e.ptm("basicContent")),[p(b,c({label:a.chooseButtonLabel,class:a.chooseButtonClass,style:e.style,disabled:e.disabled,unstyled:e.unstyled,onMouseup:a.onBasicUploaderClick,onKeydown:Ae(a.choose,["enter"]),onFocus:a.onFocus,onBlur:a.onBlur},e.chooseButtonProps,{pt:e.ptm("pcChooseButton")}),{icon:g(function(y){return[I(e.$slots,"chooseicon",{},function(){return[(r(),k(F(e.chooseIcon?"span":"PlusIcon"),c({class:[y.class,e.chooseIcon],"aria-hidden":"true"},e.ptm("pcChooseButton").icon),null,16,["class"]))]})]}),_:3},16,["label","class","style","disabled","unstyled","onMouseup","onKeydown","onFocus","onBlur","pt"]),e.auto?T("",!0):I(e.$slots,"filelabel",{key:0,class:L(e.cx("filelabel")),files:l.files},function(){return[i("span",{class:L(e.cx("filelabel"))},d(a.basicFileChosenLabel),3)]}),i("input",c({ref:"fileInput",type:"file",accept:e.accept,disabled:e.disabled,multiple:e.multiple,onChange:t[5]||(t[5]=function(){return a.onFileSelect&&a.onFileSelect.apply(a,arguments)}),onFocus:t[6]||(t[6]=function(){return a.onFocus&&a.onFocus.apply(a,arguments)}),onBlur:t[7]||(t[7]=function(){return a.onBlur&&a.onBlur.apply(a,arguments)})},e.ptm("input")),null,16,bn)],16)],16)):T("",!0)}Ke.render=hn;const gn={class:"flex justify-content-between align-items-center mb-4"},vn={class:"text-2xl font-bold m-0"},yn={class:"text-color-secondary mt-1"},wn={class:"flex align-items-center gap-4"},kn={class:"flex-1"},Cn={class:"flex justify-content-between mb-2"},Tn={class:"font-medium"},Bn={class:"text-color-secondary"},In={class:"text-right"},Pn={class:"text-sm text-color-secondary"},Sn={key:0,class:"font-medium"},Fn={class:"grid"},An={class:"col-12 md:col-6"},Ln={class:"field mb-3"},zn={class:"block mb-2"},xn={class:"col-12 md:col-6"},Dn={key:1},En={class:"flex justify-content-between align-items-center mb-3"},jn={class:"surface-ground p-3 border-round mb-3",style:{"max-height":"200px","overflow-y":"auto"}},_n={class:"text-sm text-color-secondary"},$n={key:2,class:"text-center py-4 text-color-secondary"},Vn={class:"grid"},On={class:"col-12 md:col-6"},Un={class:"field mb-3"},Mn={class:"block mb-2"},Kn={class:"grid mb-3"},Nn={class:"col-6"},Hn={class:"block mb-2"},Rn={class:"col-6"},qn={class:"block mb-2"},Wn={class:"col-12 md:col-6"},Qn={key:1},Jn={class:"surface-ground p-3 border-round mb-3"},Gn={class:"mb-3"},Yn={class:"mt-0 mb-2"},Zn={class:"m-0 pl-4"},Xn={class:"flex justify-content-between text-sm text-color-secondary"},ea={key:2,class:"text-center py-4 text-color-secondary"},ta={class:"grid"},na={class:"col-12 md:col-6"},aa={class:"field mb-3"},sa={class:"block mb-2"},oa={class:"field mb-3"},ia={class:"block mb-2"},la={class:"col-12 md:col-6"},ra={key:1},ua={class:"flex justify-content-between align-items-center mb-3"},da={class:"surface-ground p-3 border-round mb-3"},ca={key:0},pa={class:"mt-0 mb-2"},fa={key:2,class:"text-center py-4 text-color-secondary"},ma={key:0},ba={class:"field"},ha={class:"font-bold"},ga={class:"m-0"},va={class:"field"},ya={class:"font-bold"},wa={class:"m-0"},ka={class:"field"},Ca={class:"font-bold"},Ta={class:"field"},Ba={class:"font-bold"},Ia={class:"m-0"},Pa={key:0,class:"field"},Sa={class:"font-bold"},Fa={class:"m-0"},Aa={key:1,class:"field"},La={class:"font-bold"},za=600,xa=rt({__name:"AIServicesView",setup(e){const{t,locale:n}=ut(),s=pt(),l=ue(()=>n.value==="ar"),a=B(!1),b=ue(()=>!s.value),u=B(!0),m=B(null),C=B(0),y=B(null),$=B([]),G=B(null),ge=B("ar"),V=B(null),Y=B(!1),K=B(""),ve=B("medium"),ye=B("ar"),O=B(null),Z=B(!1),X=B(""),we=B(""),U=B(null),ee=B(!1),le=B(!1),S=B(null),ke=[{label:"العربية",value:"ar"},{label:"English",value:"en"}],Ne=[{label:t("ai.short"),value:"short"},{label:t("ai.medium"),value:"medium"},{label:t("ai.long"),value:"long"}],He=ue(()=>y.value?Math.round(y.value.usedTokens/y.value.totalTokens*100):0);async function Re(){try{y.value={totalTokens:1e6,usedTokens:25e4,remainingTokens:75e4,resetDate:new Date(Date.now()+720*60*60*1e3)}}catch(v){console.error("Failed to fetch quota:",v)}}async function qe(){try{$.value=[{id:"1",type:"Transcription",status:"Completed",fileName:"meeting-recording.mp3",createdAt:new Date,duration:45},{id:"2",type:"Summarization",status:"Completed",fileName:"quarterly-report.pdf",createdAt:new Date(Date.now()-36e5),duration:12},{id:"3",type:"Translation",status:"Processing",fileName:"policy-document.docx",createdAt:new Date(Date.now()-72e5),progress:65}]}catch(v){console.error("Failed to fetch jobs:",v)}}function We(v){G.value=v.files[0]}async function Qe(){if(G.value){Y.value=!0;try{await new Promise(v=>setTimeout(v,3e3)),V.value={text:"هذا نص تجريبي للنسخ الصوتي. تم تحويل الملف الصوتي إلى نص بنجاح.",confidence:.95,duration:45,segments:[{start:0,end:10,text:"مرحباً بكم في الاجتماع",speaker:"Speaker 1"},{start:10,end:25,text:"سنناقش اليوم خطة العمل",speaker:"Speaker 1"},{start:25,end:45,text:"شكراً لحضوركم",speaker:"Speaker 2"}]}}catch(v){console.error("Transcription failed:",v)}finally{Y.value=!1}}}async function Je(){if(K.value.trim()){Z.value=!0;try{await new Promise(v=>setTimeout(v,2e3)),O.value={summary:"ملخص النص: يتناول هذا النص موضوعاً مهماً يتعلق بالتخطيط الاستراتيجي للمشروع.",keyPoints:["النقطة الأولى: أهمية التخطيط المسبق","النقطة الثانية: تحديد الأهداف بوضوح","النقطة الثالثة: متابعة التنفيذ"],wordCount:{original:K.value.split(" ").length,summary:25}}}catch(v){console.error("Summarization failed:",v)}finally{Z.value=!1}}}async function Ge(){if(X.value.trim()){ee.value=!0;try{await new Promise(v=>setTimeout(v,1500)),U.value={answer:"بناءً على المعلومات المتاحة، الإجابة على سؤالك هي...",confidence:.89,sources:[{title:"دليل السياسات",section:"الفصل الثالث"},{title:"الإجراءات التشغيلية",section:"القسم 2.1"}]}}catch(v){console.error("Q&A failed:",v)}finally{ee.value=!1}}}function Ye(v){S.value=v,le.value=!0}function Ce(v){switch(v){case"Completed":return"success";case"Processing":return"info";case"Failed":return"danger";case"Pending":return"warn";default:return"secondary"}}function re(v){return new Intl.DateTimeFormat(n.value,{dateStyle:"medium",timeStyle:"short"}).format(v)}function Ze(v){navigator.clipboard.writeText(v)}async function Te(){try{m.value=null,u.value=!0,await Promise.all([Re(),qe()]),await new Promise(v=>setTimeout(v,za)),u.value=!1,b.value?requestAnimationFrame(()=>{a.value=!0}):a.value=!0}catch(v){m.value=v,u.value=!1}}async function Xe(){await Te()}return dt(()=>{Te()}),(v,w)=>(r(),f("div",{class:L(["ai-services-view",{rtl:l.value}])},[i("div",gn,[i("div",null,[i("h1",vn,d(o(t)("ai.title")),1),i("p",yn,d(o(t)("ai.subtitle")),1)])]),u.value?(r(),k(o(ae),{key:0,size:"xl",variant:"spinner",label:"Loading...","label-arabic":"جاري التحميل...","show-label":!0,centered:"",class:"loading-state"})):m.value?(r(),k(ft,{key:1,error:m.value,title:l.value?"فشل تحميل خدمات الذكاء الاصطناعي":"Failed to load AI Services","show-retry":"",onRetry:Xe},null,8,["error","title"])):(r(),f(A,{key:2},[p(o(D),{class:"mb-4"},{content:g(()=>{var h,z,te,Be,Ie;return[i("div",wn,[i("div",kn,[i("div",Cn,[i("span",Tn,d(o(t)("ai.quota.usage")),1),i("span",Bn,d((z=(h=y.value)==null?void 0:h.usedTokens)==null?void 0:z.toLocaleString())+" / "+d((Be=(te=y.value)==null?void 0:te.totalTokens)==null?void 0:Be.toLocaleString())+" "+d(o(t)("ai.quota.tokens")),1)]),p(o(pe),{value:He.value,showValue:!0},null,8,["value"])]),i("div",In,[i("div",Pn,d(o(t)("ai.quota.resetDate")),1),(Ie=y.value)!=null&&Ie.resetDate?(r(),f("div",Sn,d(re(y.value.resetDate)),1)):T("",!0)])])]}),_:1}),p(o($e),{activeIndex:C.value,"onUpdate:activeIndex":w[7]||(w[7]=h=>C.value=h)},{default:g(()=>[p(o(R),{value:"0",header:o(t)("ai.transcription.title")},{default:g(()=>[i("div",Fn,[i("div",An,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.transcription.upload")),1)]),content:g(()=>[p(o(Ke),{mode:"basic",accept:"audio/*,video/*",maxFileSize:104857600,auto:!1,chooseLabel:"اختر ملف",onSelect:We,class:"mb-3"}),i("div",Ln,[i("label",zn,d(o(t)("ai.transcription.language")),1),p(o(de),{modelValue:ge.value,"onUpdate:modelValue":w[0]||(w[0]=h=>ge.value=h),options:ke,optionLabel:"label",optionValue:"value",class:"w-full"},null,8,["modelValue"])]),p(o(j),{label:o(t)("ai.transcription.start"),icon:"pi pi-play",onClick:Qe,loading:Y.value,disabled:!G.value,class:"w-full"},null,8,["label","loading","disabled"])]),_:1})]),i("div",xn,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.transcription.result")),1)]),content:g(()=>{var h;return[Y.value?(r(),k(o(ae),{key:0,size:"lg",variant:"dots",label:o(t)("ai.transcription.processing"),"show-label":!0,class:"py-4"},null,8,["label"])):V.value?(r(),f("div",Dn,[i("div",En,[p(o(se),{value:`${o(t)("ai.transcription.confidence")}: ${Math.round(V.value.confidence*100)}%`,severity:"success"},null,8,["value"]),p(o(j),{icon:"pi pi-copy",text:"",rounded:"",onClick:w[1]||(w[1]=z=>Ze(V.value.text))})]),i("div",jn,d(V.value.text),1),i("div",_n,d(o(t)("ai.transcription.segments"))+": "+d((h=V.value.segments)==null?void 0:h.length),1)])):(r(),f("div",$n,[w[9]||(w[9]=i("i",{class:"pi pi-microphone text-4xl mb-3"},null,-1)),i("p",null,d(o(t)("ai.transcription.noResult")),1)]))]}),_:1})])])]),_:1},8,["header"]),p(o(R),{value:"1",header:o(t)("ai.summarization.title")},{default:g(()=>[i("div",Vn,[i("div",On,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.summarization.input")),1)]),content:g(()=>[i("div",Un,[i("label",Mn,d(o(t)("ai.summarization.text")),1),p(o(Le),{modelValue:K.value,"onUpdate:modelValue":w[2]||(w[2]=h=>K.value=h),rows:"8",class:"w-full",placeholder:o(t)("ai.summarization.placeholder")},null,8,["modelValue","placeholder"])]),i("div",Kn,[i("div",Nn,[i("label",Hn,d(o(t)("ai.summarization.length")),1),p(o(de),{modelValue:ve.value,"onUpdate:modelValue":w[3]||(w[3]=h=>ve.value=h),options:Ne,optionLabel:"label",optionValue:"value",class:"w-full"},null,8,["modelValue"])]),i("div",Rn,[i("label",qn,d(o(t)("ai.summarization.outputLanguage")),1),p(o(de),{modelValue:ye.value,"onUpdate:modelValue":w[4]||(w[4]=h=>ye.value=h),options:ke,optionLabel:"label",optionValue:"value",class:"w-full"},null,8,["modelValue"])])]),p(o(j),{label:o(t)("ai.summarization.summarize"),icon:"pi pi-list",onClick:Je,loading:Z.value,disabled:!K.value.trim(),class:"w-full"},null,8,["label","loading","disabled"])]),_:1})]),i("div",Wn,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.summarization.result")),1)]),content:g(()=>[Z.value?(r(),k(o(ae),{key:0,size:"lg",variant:"dots",label:o(t)("ai.summarization.processing"),"show-label":!0,class:"py-4"},null,8,["label"])):O.value?(r(),f("div",Qn,[i("div",Jn,d(O.value.summary),1),i("div",Gn,[i("h4",Yn,d(o(t)("ai.summarization.keyPoints")),1),i("ul",Zn,[(r(!0),f(A,null,_(O.value.keyPoints,(h,z)=>(r(),f("li",{key:z,class:"mb-1"},d(h),1))),128))])]),i("div",Xn,[i("span",null,d(o(t)("ai.summarization.originalWords"))+": "+d(O.value.wordCount.original),1),i("span",null,d(o(t)("ai.summarization.summaryWords"))+": "+d(O.value.wordCount.summary),1)])])):(r(),f("div",ea,[w[10]||(w[10]=i("i",{class:"pi pi-file-edit text-4xl mb-3"},null,-1)),i("p",null,d(o(t)("ai.summarization.noResult")),1)]))]),_:1})])])]),_:1},8,["header"]),p(o(R),{value:"2",header:o(t)("ai.qa.title")},{default:g(()=>[i("div",ta,[i("div",na,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.qa.askQuestion")),1)]),content:g(()=>[i("div",aa,[i("label",sa,d(o(t)("ai.qa.question")),1),p(o(vt),{modelValue:X.value,"onUpdate:modelValue":w[5]||(w[5]=h=>X.value=h),class:"w-full",placeholder:o(t)("ai.qa.questionPlaceholder")},null,8,["modelValue","placeholder"])]),i("div",oa,[i("label",ia,d(o(t)("ai.qa.context"))+" ("+d(o(t)("common.optional"))+")",1),p(o(Le),{modelValue:we.value,"onUpdate:modelValue":w[6]||(w[6]=h=>we.value=h),rows:"4",class:"w-full",placeholder:o(t)("ai.qa.contextPlaceholder")},null,8,["modelValue","placeholder"])]),p(o(j),{label:o(t)("ai.qa.ask"),icon:"pi pi-question-circle",onClick:Ge,loading:ee.value,disabled:!X.value.trim(),class:"w-full"},null,8,["label","loading","disabled"])]),_:1})]),i("div",la,[p(o(D),null,{title:g(()=>[x(d(o(t)("ai.qa.answer")),1)]),content:g(()=>{var h;return[ee.value?(r(),k(o(ae),{key:0,size:"lg",variant:"pulse",label:o(t)("ai.qa.thinking"),"show-label":!0,class:"py-4"},null,8,["label"])):U.value?(r(),f("div",ra,[i("div",ua,[p(o(se),{value:`${o(t)("ai.qa.confidence")}: ${Math.round(U.value.confidence*100)}%`,severity:"success"},null,8,["value"])]),i("div",da,d(U.value.answer),1),(h=U.value.sources)!=null&&h.length?(r(),f("div",ca,[i("h4",pa,d(o(t)("ai.qa.sources")),1),(r(!0),f(A,null,_(U.value.sources,(z,te)=>(r(),f("div",{key:te,class:"flex align-items-center gap-2 mb-2"},[w[11]||(w[11]=i("i",{class:"pi pi-file text-color-secondary"},null,-1)),i("span",null,d(z.title)+" - "+d(z.section),1)]))),128))])):T("",!0)])):(r(),f("div",fa,[w[12]||(w[12]=i("i",{class:"pi pi-comments text-4xl mb-3"},null,-1)),i("p",null,d(o(t)("ai.qa.noAnswer")),1)]))]}),_:1})])])]),_:1},8,["header"]),p(o(R),{value:"3",header:o(t)("ai.jobs.title")},{default:g(()=>[p(o(D),null,{content:g(()=>[p(o(gt),{value:$.value,paginator:!0,rows:10,responsiveLayout:"scroll"},{default:g(()=>[p(o(H),{field:"type",header:o(t)("ai.jobs.type"),sortable:""},null,8,["header"]),p(o(H),{field:"fileName",header:o(t)("ai.jobs.fileName")},null,8,["header"]),p(o(H),{field:"status",header:o(t)("ai.jobs.status")},{body:g(({data:h})=>[p(o(se),{value:h.status,severity:Ce(h.status)},null,8,["value","severity"])]),_:1},8,["header"]),p(o(H),{field:"createdAt",header:o(t)("ai.jobs.createdAt"),sortable:""},{body:g(({data:h})=>[x(d(re(h.createdAt)),1)]),_:1},8,["header"]),p(o(H),{header:o(t)("common.actions")},{body:g(({data:h})=>[p(o(j),{icon:"pi pi-eye",text:"",rounded:"",onClick:z=>Ye(h)},null,8,["onClick"])]),_:1},8,["header"])]),_:1},8,["value"])]),_:1})]),_:1},8,["header"])]),_:1},8,["activeIndex"]),p(o(ct),{visible:le.value,"onUpdate:visible":w[8]||(w[8]=h=>le.value=h),header:o(t)("ai.jobs.details"),style:{width:"500px"},modal:!0},{default:g(()=>[S.value?(r(),f("div",ma,[i("div",ba,[i("label",ha,d(o(t)("ai.jobs.type")),1),i("p",ga,d(S.value.type),1)]),i("div",va,[i("label",ya,d(o(t)("ai.jobs.fileName")),1),i("p",wa,d(S.value.fileName),1)]),i("div",ka,[i("label",Ca,d(o(t)("ai.jobs.status")),1),p(o(se),{value:S.value.status,severity:Ce(S.value.status)},null,8,["value","severity"])]),i("div",Ta,[i("label",Ba,d(o(t)("ai.jobs.createdAt")),1),i("p",Ia,d(re(S.value.createdAt)),1)]),S.value.duration?(r(),f("div",Pa,[i("label",Sa,d(o(t)("ai.jobs.duration")),1),i("p",Fa,d(S.value.duration)+"s",1)])):T("",!0),S.value.progress!==void 0?(r(),f("div",Aa,[i("label",La,d(o(t)("ai.jobs.progress")),1),p(o(pe),{value:S.value.progress},null,8,["value"])])):T("",!0)])):T("",!0)]),_:1},8,["visible","header"])],64))],2))}}),Za=yt(xa,[["__scopeId","data-v-2972407f"]]);export{Za as default};
