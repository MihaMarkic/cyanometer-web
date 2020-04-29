var loadedImages = 0;
const dateMask = "DD.MM.YYYY HH:mm";
var images;
var mainImage;
var mainImageIndex;
var averageBlueness;
var tlm = new TimelineMax({ repeat: -1, repeatDelay: 0.2, yoyo: true });
// menu stuff
var trigger;
var label;

function loaded(country, city, pollutionMeasurement, pollutionColor, pollutionText, levelsText) {
    insertPolutionData(pollutionMeasurement, pollutionColor, pollutionText, levelsText);
    $('.cyan-display-main').css("visibility", "hidden");
    $('.debug.meta li').css("visibility", "hidden");
    $('#thumbnails-wrapper').css("visibility", "hidden");
    $('#menu').css("visibility", "hidden");
    showLoadingGif();
    var url = `/api/images/${country}/${city}`;
    $.ajax({
        url: url,
        dataType: 'json',
        success: (data) => {
            images = data.images;
            averageBlueness = data.averageBlueness;
            mainImageIndex = 1;
            mainImage = images[mainImageIndex];
            populatePie();
            updateMainImage();
            $("#archive").attr('href', data.archiveUrl);
            preloadAllImages(data.images);
        },
        error: (xhr, status, err) => {
            console.error(url, status, err.toString());
        }
    });
}

function updateMainImage() {
    highlightCurrentImage(mainImage);
    $("#mainLocation").text(`${mainImage.city}, ${mainImage.country} SKY`);
    $("#mainTakenAt").text(moment(mainImage.takenAt).format(dateMask));
}

function showLoadingGif() {
    $('#cyan-display').css("background", "url(/images/loading.svg) no-repeat center");
    $('#cyan-display').css("height", "300px");
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function preloadAllImages(images) {
    var row1 = $("#thumbnails1");
    var row2 = $("#thumbnails2");
    images.forEach(function (image, index) {
        loadImage(image, index, row1, row2, images.length);
    });
}

const MaxImagesPerRow = 6;
var loadedImages = [];
function loadImage(image, index, row1, row2, count) {
    var path = image.thumbnailUrl;
    var takenAt = moment(image.takenAt).format("HH:mm");
    var imageId = `image${image.id}`;
    //$(`<img src="${path}"></img>`).load(function () {
    var div = $(`
<div class="thumbnail-wrapper">
    <div id='${imageId}' class='thumbnail' style="color:white;background-image:url('${path}')" onclick="sliceClick(event, ${index})">
                <p class="time">
                    ${takenAt}
                </div>
    </div>
</div>`);
    var target;
    console.log(`Loading image ${index}`);
    if (index < MaxImagesPerRow) {
        target = row1;
    } else {
        target = row2;
    }
    div.appendTo(target);
    loadedImages++;
    if (loadedImages === count) {
        imagesLoaded();
    }
    //});
}

function imagesLoaded() {
    var tl = new TimelineLite();
    $('.cyan-display-main').css("opacity", "0");
    $('.selected-slice').css("opacity", "0.5");
    $('.cyan-display-main').css("visibility", "visible");
    $('#thumbnails-wrapper').css("visibility", "visible");
    $('.debug.meta li').css("visibility", "visible");
    $('#menu').css("visibility", "visible");
    $('#cyan-display').css("background", "white");
    $('#cyan-display').css("height", "auto");

    // $('.cyan-display-main').css("opacity", 1);
    $('.cyan-display-main').css("background", `url(${mainImage.url}) no-repeat`);

    // if (window.mobilecheck()) {
    //   $('.cyan-display-main').css("background-size", "100% 201px");
    // }

    tl.to('.cyan-display-main', 1, { opacity: 1 });
    // tl.from('.cyan-display-main', 0.3, { x: -1200 });

    tl.staggerFrom(".debug.meta li", 0.3, { scale: 0.5, opacity: 0, delay: 0.1, ease: Elastic.easeOut, force3D: true }, 0.1);
    // open menu
    tl.from('#trigger', 0.3, { scale: 0.5, autoAlpha: 0, transformOrigin: "50% 50%", ease: Expo.easeInOut })
    tl.staggerFrom('.item', 0.2, { scale: 0.5, autoAlpha: 0, delay: 0.1 }, 0.05);

    tlm.to(".selected-slice", 0.5, { opacity: 1 });

    tl.staggerFrom(".cyan-display-main .time span", 0.3, { scale: 0.5, opacity: 0, delay: 0.1, ease: Elastic.easeOut, force3D: true }, 0.1)
        .staggerFrom(".cyan-display-main .blueness span", 0.3, { scale: 0.5, opacity: 0, delay: 0.1, ease: Elastic.easeOut, force3D: true }, 0.1)
        .to('.menu-trigger #blueness-label', 0.5, { autoAlpha: 1 })
        .to('.menu-trigger #blueness-label-suffix', 0.5, { autoAlpha: 1 })
        .add(tlm); //nested, looping TLM
}

function sliceClick(e, index) {
    e.preventDefault();
    console.log(`yolo on ${index}`);
    mainImageIndex = index;
    mainImage = images[index];

    populatePie();
    updateMainImage();
    imagesLoaded();

    //highlightCurrentImage(index);
    //$('.sector').removeClass('selected-slice');
    //$('.sector').css("opacity", "1");
    //tlm.pause();
    //tlm.clear();
    //console.log(`selecting #image-slice-${mainImage.id}`);
    //$(`#image-slice-${mainImage.id}`).addClass('selected-slice');
    //tlm.restart();
    //tlm.to(".selected-slice", 0.5, { opacity: 1 });
    ////updateMainImage();
    ////$('.cyan-display-main').css("background", `url(${mainImage.url}) no-repeat`);
    ////// open menu
    //var tl = new TimelineLite();
    //tl.from('#trigger', 0.3, { scale: 0.5, autoAlpha: 0, transformOrigin: "50% 50%", ease: Expo.easeInOut })
    //tl.staggerFrom('.item', 0.2, { scale: 0.5, autoAlpha: 0, delay: 0.1 }, 0.05);
    //populatePie();
}
var globalPie;
function populatePie() {
    var matrices = ["0.5,-0.86602,0.86602,0.5,-91.5063509461097,341.5063509461096",
        "0.86602,-0.49999,0.49999,0.86602,-91.50635094610965,158.4936490538903",
        "1,0,0,1,0,0",
        "0.86602,0.5,-0.5,0.86602,158.49364905389052,-91.5063509461097",
        "0.5,0.86602,-0.86602,0.5,341.5063509461096,-91.5063509461097",
        "0,1,-1,0,500.00000000000006,0",
        "-0.5,0.86602,-0.86602,-0.5,591.5063509461097,158.4936490538905",
        "-0.86602,0.5,-0.5,-0.86602,591.5063509461097,341.5063509461096",
        "-1,0,0,-1,500,500",
        "-0.86602,-0.49999,0.49999,-0.86602,341.5063509461097,591.5063509461097",
        "-0.49999,-0.86602,0.86602,-0.49999,158.49364905389024,591.5063509461097",
        "0,-1,1,0,0,500"];
    var pie = `
<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="-2 -2 504 504" id="menu">
    <g id="itemsContainer">`;
    for (i = 0; i < 12; i++) {
        pie = pie + createSlice(i, images[i], matrices[i]);
    }
    pie = pie + `
    </g>
    <g id="trigger" class="trigger menu-trigger" role="button">
        <circle cx="250" cy="250" r="125"></circle>
        <text xmlns="http://www.w3.org/2000/svg" id="blueness-label" text-anchor="middle" alignment-baseline="central" x="250" y="235" fill="#fff">
            ?
        </text>
        <text xmlns="http://www.w3.org/2000/svg" id="blueness-label-suffix" text-anchor="middle" alignment-baseline="central" x="250" y="285" fill="#fff">
            AVERAGE BLUENESS
        </text>
        <text xmlns="http://www.w3.org/2000/svg" id="label" text-anchor="middle" alignment-baseline="auto" x="250" y="340" fill="#fff">
            -
        </text>
    </g>
</svg>`;
    if (globalPie) {
        console.log("deleting existing pie");
        globalPie.remove();
    }
    globalPie = $(pie);
    globalPie.appendTo($("#mainDisplay"));
    // bind menu
    trigger = document.getElementById('trigger');
    label = trigger.querySelectorAll('#label')[0];
    //set up event handler
    trigger.addEventListener('click', toggleMenu, false);
}

function createSlice(index, image, matrix) {
    var foreColor = parseColor(bc[image.bluenessIndex]);
    var title = moment(image.takenAt).format(dateMask) + ", blueness index " + image.bluenessIndex;
    var selectedClass;
    if (index === mainImageIndex) {
        selectedClass = " selected-slice";
    } else {
        selectedClass = "";
    }
    var slice = `
<a onclick='sliceClick(event, ${index})' class="item" id="item-${index + 1}" role="link" tabindex="0"
    transform="matrix(${matrix})" data-svg-origin="250 250" style='color:white'>
    <path id=image-slice-${image.id} fill='${foreColor}' stroke="#111"
            d="M375,250 l125,0 A250,250 0 0,0 466.5063509461097,125.00000000000001 l-108.25317547305485,62.499999999999986 A125,125 0 0,1 375,250"
            class="sector${selectedClass}" >
            <title>${title}</title>
    </path>
    <use xlink:href="#icon-${index + 1}" width="35" height="35" x="437.2762756347656" y="177.63037109375"
        transform="rotate(75 454.7762756347656 195.13037109375)"></use>
</a>\n`;
    return slice;
}

function highlightCurrentImage(selectedImage) {
    var bpStyleColour = "#fff";
    if (selectedImage) {
        var bpStyleColour = parseColor(bc[averageBlueness - 1]);
    }

    $('.menu-trigger').attr('fill', bpStyleColour);
    $('.menu-trigger #blueness-label').css('visibility', 'hidden');
    $('.menu-trigger #blueness-label-suffix').css('visibility', 'hidden');
    $('.menu-trigger #blueness-label').text(averageBlueness);
    $('.blueness-swatch li').removeClass('border');
    $('.thumbnail').removeClass('border');
    $(`.blueness-swatch li:nth-of-type(${selectedImage.bluenessIndex})`).addClass('border');
    $('#image' + selectedImage.id).addClass('border');
}
function closeMenu() {
    var tl = new TimelineLite();
    svg = document.getElementById('menu');
    items = svg.querySelectorAll('.item');
    tl.to(items, .3, { scale: 0, ease: Back.easeIn }, 0.3)
        .to(trigger, 0.6, { scale: 0.5, transformOrigin: "50% 50%", ease: Expo.easeInOut }, 0)
    label.innerHTML = "+";

    svg.style.pointerEvents = "none";
}
function openMenu() {
    var tl = new TimelineLite();
    svg = document.getElementById('menu');
    items = svg.querySelectorAll('.item');
    tl.to(trigger, 0.6, { scale: 1, transformOrigin: "50% 50%", ease: Expo.easeInOut }, 0)
    tl.to(items, 0.2, { scale: 1, ease: Back.easeOut.config(4) }, 0.05);
    // MSP using this open/close animation skews the position of the first segment from 12 o'clock
    // for(var i=0; i<items.length; i++){
    //   tl.to(items[i], 0.7, {rotation:-i*angle + "deg", ease:Bounce.easeOut}, 0.35);
    // }
    label.innerHTML = "-";
    svg.style.pointerEvents = "auto";
}
function toggleMenu() {
    if (label.innerHTML !== "+") {
        closeMenu();
    } else {
        openMenu();
    }
}

function insertPolutionData(icon, color, pollutionText, levelsText) {
    var info = "";
    if (pollutionText && levelsText) {
        info = `
<p>
    The current level of air pollution is ${pollutionText}
    <br />
    There are increased levels of ${levelsText}
</p>;`;
    }
    var icon;
    switch (icon) {
        case "NO2":
            icon = createIconCar(color);
            break;
        case "SO2":
            icon = createIconFactory(color);
            break;
        case "PM10":
            icon = createIconHouse(color);
            break;
        case "O3":
            icon = createIconSun(color);
            break;
        case "LOW":
            icon = createIconCircle(color);
            break;
        default:
            icon = `<p>UNKNOWN AT THIS TIME</p>`;
            break;
    }
    var html = `
${icon}
${info}`;
    $(html).appendTo("#environmental-data");
}