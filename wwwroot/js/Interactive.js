$(document).mousemove(function (e) {

    // elements
    let cursor = $('.cursor');
    let innerCursor = $('.inner-cursor');
    let c2 = $('.circle');

    let pos = {
        left: e.pageX - 25,
        top: e.pageY - 20
    };
    cursor.css(pos);

    innerCursor.css({
        left: pos.left - c2.offset().left,
        top: pos.top - c2.offset().top
    });

    // circles


    // radius
    var d1 = cursor.outerWidth(true) / 2;
    var d2 = c2.outerWidth(true) / 2;

    // centers of first circle
    var x1 = cursor.offset().left + cursor.width() / 2;
    var y1 = cursor.offset().top + cursor.height() / 2;

    // centers of second circle
    var x2 = c2.offset().left + c2.width() / 2;
    var y2 = c2.offset().top + c2.height() / 2;

    var i1 = c2.find('.inter1');
    var i2 = c2.find('.inter2');
    var o = cursor.find('.overlap');

    function calc() {
        var a = d2;
        var b = d1;
        var c = Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        var d = (b * b + c * c - a * a) / (2 * c);
        var h = Math.sqrt((b * b) - (d * d));
        // console.log(a, b, c, d, h);
        if (d < 0 || $.isNumeric(h)) {
            c2.css('border-color', 'red');
        } else {
            c2.css('border-color', 'black');
        }
        var x3 = (x2 - x1) * d / c + (y2 - y1) * h / c + x1;
        var y3 = (y2 - y1) * d / c - (x2 - x1) * h / c + y1;
        var x4 = (x2 - x1) * d / c - (y2 - y1) * h / c + x1;
        var y4 = (y2 - y1) * d / c + (x2 - x1) * h / c + y1;

        if ($.isNumeric(h)) {
            i1.show();
            i2.show();
        } else {
            i1.hide();
            i2.hide();
        }
        i1.offset({
            top: y3 - 5,
            left: x3 - 5
        });
        i2.offset({
            top: y4 - 5,
            left: x4 - 5
        });
    }
    calc();

});