var loadedImages = 0;
const dateMask = "DD.MM.YYYY HH:mm";

$(function () {
    $('.cyan-display-main').css("visibility", "hidden");
    $('.debug.meta li').css("visibility", "hidden");
    $('#menu').css("visibility", "hidden");
    showLoadingGif();

    $.ajax({
        url: "/api/landing/",
        dataType: 'json',
        success: (data) => {
            console.log('data', data);
            preloadAllImages(data);
        },
        error: (xhr, status, err) => {
            console.error(self.props.source, status, err.toString());
        }
    });
});

function showLoadingGif() {
    $('#cyan-display').css("background", "url(/images/loading.svg) no-repeat center");
    $('#cyan-display').css("height", "300px");
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function preloadAllImages(images) {
    var grid = $("#images");
    images.forEach(function (image) {
        console.log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! preloading: " + image.url);
        loadImage(image, grid, images.length);
    });
}

function loadImage(image, target, count) {
    var path = image.thumbnailUrl;
    var takenAt = moment(image.takenAt).format(dateMask);
    $(`<img src="${path}"></img>`).load(function () {
        var div = $(`
<div class='cyan-display-main landing-image' style="background-image:url('${path}')" onclick="handleLocationClick('${image.city}','${image.country}')">
            <div class="time">
                <span>${image.city}, ${image.country}</span>
                <br/>
                <span>${takenAt}</span>
            </div>
            <div class="calendar"></div>
</div>`);
        div.appendTo(target);
        loadedImages++;
        if (loadedImages === count) {
            imagesLoaded();
        }
    });
}

function handleLocationClick(city, country) {
    window.location.href = `/location/${country.toLowerCase()}/${city.toLowerCase()}`;
}

function imagesLoaded() {
    $('.cyan-display-main').css("opacity", "0");
    $('.cyan-display-main').css("visibility", "visible");
    $('#cyan-info').css("visibility", "visible");
    $('.debug.meta li').css("visibility", "visible");
    $('#cyan-display').css("background", "white");
    $('#cyan-display').css("height", "auto");

    var tl = new TimelineLite();
    tl.to('.cyan-display-main', 1, { opacity: 1 });
    tl.staggerFrom(".cyan-display-main .time span", 0.3, { scale: 0.5, opacity: 0, delay: 0.1, ease: Elastic.easeOut, force3D: true }, 0.1)
        .to('#cyan-info', 0.2, { autoAlpha: 1 });
}