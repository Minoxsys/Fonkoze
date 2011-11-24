/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="jquery-ui-1.8.14.min.js" />


$(function () {

    var url = $("#autocomplete").attr("auturl");
    $("#autocomplete").autocomplete({
        source: url,  //$(this).attr("auturl")-- nu stiu dc nu merge
        select: function( event, ui ) {
                $("#TeamLeader_Id").val(ui.item.id);
			}
    });
});