$(document).ready(function () {
    var elements = $('.main_block li');
    elements.on('click', function () {
        var index = elements.index(this);
        GetReq(index);
    });


    function GetReq(i) {
        $(".load").show();
        if ($(".changed_panel div").children().length != 0) {
            $(".changed_panel div").empty();
        }
        const iframe = $('<iframe/>');
        iframe.on('load', function () {
            $(".load").hide();
        });
        iframe.attr({
            "height": "100%",
            "width": "100%",
            "frameborder": "0"
        });

        switch(i){
          case 0:
            iframe.attr("src", "AllRequests?id=-1");
            break;
          case 1:
            iframe.attr("src", "AllRequests?id=-1");
            break;
          case 2:
            iframe.attr("src", "Listeners");
            break;
          case 3:
            iframe.attr("src", "Requesters");
            break;
          case 4:
            iframe.attr("src", "Users");
            break;
        }
        iframe.height = '100%';
        iframe.width = '100%';
        $(iframe).css("border", "none");
        $(".changed_panel div").append(iframe);
      }

	$( ".user_button" ).click(function() {
		$(".user_panel").toggleClass('invisible');
	});

	$(document).mouseup(function (e) {
    var container = $(".user_panel");
    if (container.has(e.target).length === 0 && container.hasClass("invisible") == false){
        $(".user_panel").toggleClass('invisible');
    }
    });

});