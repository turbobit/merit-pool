/*! Built with http://stenciljs.com */
const{h:e}=window.DocsSite;class o{render(){return e("footer",null,e("div",{class:"container"},e("div",{class:"footer__open-source"},e("a",{href:"http://ionicframework.com/",title:"IonicFramework.com",rel:"noopener"},e("img",{src:"/assets/img/ionic-os-logo.png",alt:"Ionic Open Source Logo"})),e("p",null,"Released under ",e("span",{id:"mit"},"MIT License")," | Copyright @ ",(new Date).getFullYear())),e("div",{class:"footer-menu"},e("a",{href:"cheatsheet.html"},"Cheatsheet"),e("a",{href:"/v1"},"v1"),e("a",{href:"/v2"},"v2"),e("a",{href:"https://ionicframework.com/docs/components/#icons"},"v3"),e("a",{href:"https://ionicframework.com/"},"Ionic Framework"))))}static get is(){return"footer-bar"}static get style(){return"footer-bar footer{width:100%;background:var(--color-white-lilac);-webkit-box-flex:0;-webkit-flex:0 0 8em;-ms-flex:0 0 8em;flex:0 0 8em}footer-bar .container{display:-webkit-box;display:-webkit-flex;display:-ms-flexbox;display:flex;-webkit-box-pack:justify;-webkit-justify-content:space-between;-ms-flex-pack:justify;justify-content:space-between;-webkit-box-align:center;-webkit-align-items:center;-ms-flex-align:center;align-items:center;width:100%;padding-top:40px;padding-bottom:40px}footer-bar img{width:50%}footer-bar p{margin-top:0;margin-bottom:0;color:var(--color-cadet-blue);font-size:10px;letter-spacing:0}footer-bar .footer-menu a{-webkit-transition:color .3s;transition:color .3s;font-size:11px;font-weight:600;text-decoration:none;color:var(--color-gull-gray)}footer-bar .footer-menu a:hover{color:var(--color-shark)}footer-bar .footer-menu a+a{margin-left:18px}\@media screen and (max-width:768px){footer-bar .container{-webkit-box-orient:vertical;-webkit-box-direction:reverse;-webkit-flex-direction:column-reverse;-ms-flex-direction:column-reverse;flex-direction:column-reverse;text-align:center}footer-bar .footer-menu{margin-bottom:36px}}"}}export{o as FooterBar};