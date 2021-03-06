/*
  This is in vendor so it can be used old school JS stylee
  http://www.phoenixframework.org/docs/static-assets
*/

var bc = [ "#f4fbfe", "#f4fbfe", "#ecf8fd", "#e5f5fd", "#def2fc", "#d6effc",
           "#cfebfa", "#c7e4f7", "#c0dff3", "#b9d9ed", "#b3d4e8", "#accfe4",
           "#a5cbdf", "#9ec6db", "#98c1d7", "#91bdd2", "#86b7cf", "#7db2cb",
           "#77adc8", "#6ea8c7", "#62a3c3", "#5a9dbe", "#5298bc", "#4693ba",
           "#3a8eb6", "#378bb3", "#3485ad", "#2d80a9", "#287da5", "#2677a1",
           "#24739c", "#1e6d96", "#1c6991", "#10648e", "#0c5f89", "#055b86",
           "#005682", "#00537b", "#004e74", "#064b6e", "#094767", "#0e4361",
           "#113d57", "#133951", "#14354a", "#173244", "#182f40", "#192c3b",
           "#192937", "#182532", "#18222d", "#161e28", "#101822" ];

function parseColor(color) {
	if (typeof color === 'number') {
		//make sure our hexadecimal number is padded out
		color = '#' + ('00000' + (color | 0).toString(16)).substr(-6);
	}

	return color;
};

var is_chrome = navigator.userAgent.indexOf('Chrome') > -1;
var is_explorer = navigator.userAgent.indexOf('MSIE') > -1;
var is_firefox = navigator.userAgent.indexOf('Firefox') > -1;
var is_safari = navigator.userAgent.indexOf("Safari") > -1;
var is_opera = navigator.userAgent.toLowerCase().indexOf("op") > -1;
if ((is_chrome)&&(is_safari)) {is_safari=false;}
if ((is_chrome)&&(is_opera)) {is_chrome=false;}

function getQueryParameter ( parameterName ) {
  var queryString = window.top.location.search.substring(1);
  var parameterName = parameterName + "=";
  if ( queryString.length > 0 ) {
    var begin = queryString.indexOf ( parameterName );
    if ( begin != -1 ) {
      begin += parameterName.length;
      var end = queryString.indexOf ( "&" , begin );
        if ( end == -1 ) {
        end = queryString.length
      }
      return unescape ( queryString.substring ( begin, end ) );
    }
  }
  return "null";
}

window.mobilecheck = function() {
  var check = false;
  (function(a){if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a)||/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0,4)))check = true})(navigator.userAgent||navigator.vendor||window.opera);
  return check;
}

$(document).ready(function() {
  var colour;
  var index = 1;
  bc.forEach(function(bcColour) {
    colour = parseColor(bcColour);
    $(".debug .blueness-swatch").append("<li style='background-color:"+colour+"'>"+index+": "+colour+"</li>");
    index++;
  });

  // hide things we animate into view
  $('#thumbnails-wrapper').css("visibility", "hidden");
  $('#menu').css("visibility", "hidden");

  // having trouble ovrriding if is this is done in CSS
  $('.menu-trigger').attr('fill', 'white');
  $('.sector').attr('fill', 'white');

  if (is_safari) {
    if (window.mobilecheck() == true) {
      $('.cyan-display-main').css('padding-top', '30px');
    } else {
      $('.cyan-display-main').css('padding-top', '0px');
    }
  }

  TweenMax.staggerFrom(".debug.colour li", 1,
                      {
                        scale:0.5, opacity:0, delay:0.1,
                        ease:Elastic.easeOut, force3D:true},
                      0.05);

});

function createIconCar(color) {
    return `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
      	 width="391px" height="381.038px" viewBox="0 0 391 381.038" enable-background="new 0 0 391 381.038" xmlSpace="preserve">
      <g>
      	<circle fill="${color}" cx="195.275" cy="190.115" r="132.303"/>
      	<circle fill="none" stroke="#FFFFFF" strokeWidth="12" strokeMiterlimit="10" cx="195.275" cy="190.115" r="132.303"/>
      	<path fill="#555555" d="M244.131,173.214h-21.594l-18.449-13.59c-0.684-0.523-1.52-0.41-2.383-0.41h-26.004
      		c-1.195,0-2.324,0.146-3.07,1.072l-10.732,12.928h-15.361c-2.176,0-4.537,1.499-4.537,3.677v29.572
      		c0,2.178,2.361,4.751,4.537,4.751h9.543c1.32,6,6.453,10.025,12.578,10.025s11.25-4.025,12.566-10.025h31.215
      		c1.309,6,6.441,10.025,12.566,10.025s11.258-4.025,12.574-10.025h6.551c2.172,0,3.869-2.573,3.869-4.751v-29.572
      		C248,174.713,246.303,173.214,244.131,173.214z M168.658,213.845c-1.984,0-3.723-0.631-4.695-2.631h9.387
      		C172.377,213.214,170.639,213.845,168.658,213.845z M225.006,213.845c-1.98,0-3.721-0.631-4.691-2.631h9.387
      		C228.725,213.214,226.99,213.845,225.006,213.845z M240,203.214h-92v-22h15.779c1.195,0,2.32-1.148,3.066-2.078l10.742-11.922
      		h22.789l18.457,12.594c0.688,0.516,1.518,1.406,2.375,1.406H240V203.214z"/>
      </g>
      </svg>`;
}

function createIconCircle(color) {
    return `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
      	 width="391px" height="381.038px" viewBox="0 0 391 381.038" enable-background="new 0 0 391 381.038" xmlSpace="preserve">
      <g>
      	<circle fill="${color}" cx="195.5" cy="190.519" r="132.303"/>
      	<circle fill="none" stroke="#FFFFFF" stroke-width="12" stroke-miterlimit="10" cx="195.5" cy="190.519" r="132.303"/>
      </g>
      </svg>`;
}

function createIconFactory(color) {
    return `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
      	 width="391px" height="381.038px" viewBox="0 0 391 381.038" enable-background="new 0 0 391 381.038" xmlSpace="preserve">
      <g>
      	<circle fill="${color}" cx="195.5" cy="190.519" r="132.303"/>
      	<circle fill="none" stroke="#FFFFFF" stroke-width="12" stroke-miterlimit="10" cx="195.5" cy="190.519" r="132.303"/>
      	<path fill="#555555" d="M239.694,228.214h-88.719c-2.174,0-3.975-1.433-3.975-3.611v-69c0-2.178,1.801-3.389,3.975-3.389h15.773
      		c2.18,0,4.251,1.211,4.251,3.389v31.018l30.168-20.487c1.207-0.816,1.61-0.9,2.901-0.213c1.289,0.682,0.931,2.021,0.931,3.48
      		v18.042l31.344-21.268c1.207-0.844,3.001-0.953,4.31-0.273c1.301,0.68,2.346,2.029,2.346,3.5v55.202
      		C243,226.781,241.87,228.214,239.694,228.214z M155,220.214h80v-43.22l-29.887,21.262c-1.207,0.848-3.729,0.953-5.04,0.275
      		c-1.303-0.678-3.074-2.027-3.074-3.496v-18.211l-29.185,20.488c-1.203,0.816-2.095,0.902-3.392,0.215
      		c-1.287-0.684-1.423-2.023-1.423-3.48v-33.833h-8V220.214z"/>
      </g>
      </svg>`;
}

function createIconHouse(color) {
    return `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
      	 width="391px" height="381.038px" viewBox="0 0 391 381.038" enable-background="new 0 0 391 381.038" xmlSpace="preserve">
      <g>
      	<g>
      		<circle fill="${color}" cx="195.5" cy="190.519" r="132.303"/>
      		<circle fill="none" stroke="#FFFFFF" stroke-width="12" stroke-miterlimit="10" cx="195.5" cy="190.519" r="132.303"/>
      	</g>
      	<g>
      		<path fill="#555555" d="M222.569,228.214h-57.434c-2.117,0-6.135-2.164-6.135-4.078v-30.422c0-1.037,1.671-2.016,2.55-2.674
      			l27.379-19.9c1.605-1.188,3.122-1.01,4.485,0.406L205,184.462v-4.592c0-1.908,3.967-5.657,6.083-5.657h11.486
      			c2.109,0,2.431,3.749,2.431,5.657v44.266C225,226.05,224.678,228.214,222.569,228.214z M167,220.214h50v-38h-4v11.5
      			c0,1.445-0.036,2.734-1.532,3.242c-1.496,0.506-2.703,0.121-3.746-0.969l-17.453-17.363L167,195.349V220.214z"/>
      		<path fill="#555555" d="M212.928,171.49c-0.676,0-1.355-0.25-1.881-0.76c-0.518-0.5-4.959-5.23-0.658-16.787
      			c1.326-3.574,5.375-14.443,2.742-16.572c-1.164-0.939-1.355-2.645-0.41-3.813c0.943-1.164,2.648-1.346,3.813-0.406
      			c4.727,3.813,3.098,11.504-1.066,22.682c-2.945,7.922-0.686,10.975-0.656,10.994c1.078,1.041,1.109,2.756,0.07,3.834
      			C214.342,171.214,213.639,171.49,212.928,171.49z"/>
      		<path fill="#555555" d="M221.799,171.49c-0.676,0-1.352-0.25-1.879-0.76c-0.52-0.5-4.961-5.23-0.66-16.787
      			c1.332-3.574,5.379-14.443,2.742-16.572c-1.164-0.939-1.348-2.645-0.41-3.813c0.941-1.164,2.648-1.346,3.813-0.406
      			c4.727,3.813,3.094,11.504-1.063,22.682c-2.949,7.922-0.688,10.975-0.664,10.994c1.082,1.041,1.113,2.756,0.074,3.834
      			C223.215,171.214,222.51,171.49,221.799,171.49z"/>
      	</g>
      </g>
      </svg>`;
}

function createIconSun(color) {
    return `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
      	 width="391px" height="381.038px" viewBox="0 0 391 381.038" enable-background="new 0 0 391 381.038" xmlSpace="preserve">
      <g>
      	<circle fill="${color}" cx="195.275" cy="190.115" r="132.303"/>
      	<circle fill="none" stroke="#FFFFFF" stroke-width="12" stroke-miterlimit="10" cx="195.275" cy="190.115" r="132.303"/>
      	<g>
      		<g>
      			<path fill="#555555" d="M195.659,163.124c-2.092,0-3.811-1.697-3.811-3.801v-19.32c0-2.107,1.719-3.821,3.811-3.821
      				c2.109,0,3.82,1.714,3.82,3.821v19.32C199.479,161.426,197.768,163.124,195.659,163.124z"/>
      			<path fill="#555555" d="M195.659,214.446c-13.268,0-24.059-10.785-24.059-24.048c0-13.253,10.791-24.048,24.059-24.048
      				s24.057,10.795,24.057,24.048C219.715,203.661,208.926,214.446,195.659,214.446z M195.659,173.985
      				c-9.045,0-16.432,7.357-16.432,16.413c0,9.062,7.387,16.415,16.432,16.415c9.072,0,16.436-7.354,16.436-16.415
      				C212.094,181.342,204.731,173.985,195.659,173.985z"/>
      			<path fill="#555555" d="M195.659,244.625c-2.113,0-3.811-1.716-3.811-3.823v-19.308c0-2.114,1.697-3.809,3.811-3.809
      				c2.109,0,3.82,1.694,3.82,3.809v19.308C199.479,242.909,197.768,244.625,195.659,244.625z"/>
      			<path fill="#555555" d="M246.063,194.215h-19.305c-2.107,0-3.826-1.729-3.826-3.817c0-2.103,1.719-3.79,3.826-3.79h19.305
      				c2.121,0,3.82,1.688,3.82,3.79C249.883,192.487,248.184,194.215,246.063,194.215z"/>
      			<path fill="#555555" d="M164.567,194.215h-19.32c-2.096,0-3.805-1.729-3.805-3.817c0-2.103,1.709-3.79,3.805-3.79h19.32
      				c2.113,0,3.824,1.688,3.824,3.79C168.391,192.487,166.68,194.215,164.567,194.215z"/>
      			<path fill="#555555" d="M174.735,173.079c-0.973,0-1.977-0.388-2.705-1.148l-12.27-12.279c-1.514-1.467-1.514-3.877,0-5.383
      				c1.49-1.479,3.885-1.479,5.381,0l12.271,12.261c1.5,1.496,1.5,3.917,0,5.401C176.663,172.691,175.702,173.079,174.735,173.079z"
      				/>
      			<path fill="#555555" d="M216.805,173.111c-0.982,0-1.947-0.377-2.688-1.119c-1.467-1.485-1.467-3.905,0-5.409l12.283-12.277
      				c1.512-1.482,3.916-1.467,5.383,0c1.516,1.506,1.516,3.915,0,5.41l-12.277,12.276
      				C218.78,172.734,217.797,173.111,216.805,173.111z"/>
      			<path fill="#555555" d="M162.45,227.48c-1,0-1.949-0.376-2.689-1.121c-1.514-1.485-1.514-3.896,0-5.384l12.27-12.291
      				c1.5-1.494,3.912-1.494,5.383,0c1.5,1.484,1.5,3.898,0,5.396l-12.271,12.278C164.399,227.104,163.422,227.48,162.45,227.48z"/>
      			<g>
      				<path fill="#555555" d="M218.874,229.174c-0.467-0.402-0.865-1.112-0.865-1.863c0-1.357,1.154-2.525,2.51-2.525
      					c0.75,0,1.266,0.305,1.674,0.639c1.355,1.185,2.791,1.781,4.658,1.781c1.973,0,3.363-1.129,3.363-2.877v-0.091
      					c0-1.922-1.729-3.015-4.662-3.015h-0.801c-1.238,0-2.268-1.014-2.268-2.247c0-0.708,0.316-1.326,1.135-2.132l4.613-4.686h-7.305
      					c-1.232,0-2.242-1.022-2.242-2.235c0-1.217,1.01-2.229,2.242-2.229h11.59c1.535,0,2.668,0.86,2.668,2.298
      					c0,1.296-0.605,2.018-1.584,2.924l-4.645,4.461c3.219,0.556,6.385,2.244,6.385,6.672v0.063c0,4.479-3.256,7.775-8.582,7.775
      					C223.391,231.887,220.823,230.846,218.874,229.174z"/>
      			</g>
      		</g>
      	</g>
      </g>
      </svg>`;
}